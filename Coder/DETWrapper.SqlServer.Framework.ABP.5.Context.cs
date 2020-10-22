using EnvDTE;
using ISoft.Metabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ISoft.Coder
{
    public partial class SqlServerClassGenWrapper
    {
        private void _Do_4(Func<string, bool> doneToConfirmContinue = null)
        {
            if (string.IsNullOrEmpty(_Namespace))
            {
                MessageBox.Show("Please provide a namespace:extensionClassName:objectPrefix");
                return;
            }

            if (!_Namespace.Contains(":"))
            {
                MessageBox.Show("Please provide a namespace:extensionClassName:objectPrefix");
                return;
            }

            var parts = _Namespace.SplitEx(':');
            var template = GetTemplatePath("ABP.Context");
            var now = DateTime.Now;
            var projects = (Array)_App.ActiveSolutionProjects;

            var boPrefix = parts[2];

            // 准备插入代码
            ProjectItem file = null;
            Window win = null;
            TextSelection ts = null;
            StringBuilder sb = null;

            var batchIndex = -1;
            var saved = true;

            Action<ProjectItem> _newFile = f =>
            {
                batchIndex++;
                file = f.ProjectItems.AddFromTemplate(template, $"{parts[1]}.cs");
                win = file.Open(Constants.vsViewKindCode);
                sb = new StringBuilder();

                ts = win.Document.Selection as TextSelection;
                // ts.EndOfDocument();

                // 插入生成日期
                sb.AppendLine(@"/// <summary>");
                sb.AppendLine($"/// {now}");
                sb.AppendLine(@"/// </summary>");

                // 插入 namespace 行
                sb.AppendLine("namespace " + parts[0]);
                sb.AppendLine("{");

                saved = false;
            };

            Action _saveAndClose = () =>
            {
                ts.EndOfDocument();
                ts.Insert(sb.ToString());
                ts.Insert("}");
                ts.NewLine();
                ts.SelectAll();

                win.Activate();
                win.Document.Activate();

                _App.ExecuteCommand("Edit.FormatDocument");

                win.Close(vsSaveChanges.vsSaveChangesYes);
                saved = true;
            };

            if (projects.Length > 0)
            {
                foreach (Project p in projects)
                {
                    ProjectItem folder = p.ProjectItems
                            .AddFolder("_Database", Constants.vsProjectItemKindPhysicalFolder);
                    List<MBTable> tables =
                        _Context.Tables.Where(_Filter).OrderBy(t => t.Name).ToList();

                    _newFile(folder);

                    sb.AppendLine($"public static class {parts[1]}");
                    sb.AppendLine("{");

                    sb.AppendLine($"public static void ConfigureInternal(this ModelBuilder builder)");
                    sb.AppendLine("{");

                    sb.AppendLine($"Check.NotNull(builder, nameof(builder));");

                    for (int i = 0; i < tables.Count; i++)
                    {
                        var t = tables[i];

                        List<string> keys = new List<string>();
                        if (!
                            string.IsNullOrEmpty(t.KeyInfo))
                            foreach (string part in t.KeyInfo.Split(','))
                                if (part.Trim().Length > 0) keys.Add(part.Trim());

                        if (keys.Count == 0)
                        {
                            doneToConfirmContinue($"Error: no primary key found for {t.Name}");
                            continue;
                        }

                        var columns = _Context.Columns
                            .Where(c => c.TableId == t.TableId).OrderBy(c => c.Name).ToList();
                        var properties = _Context.Properties
                            .Where(d => d.TableId == t.TableId).ToList();


                        sb.AppendLine($"            builder.Entity<{boPrefix}_{t.Name}>(b =>");
                        sb.AppendLine("             {");
                        sb.AppendLine($"                b.ToTable(\"{t.Name}\", AbpCommonDbProperties.DbSchema);");
                        sb.AppendLine("                 b.ConfigureByConvention();");

                        if (keys.Count > 1)
                        {
                            sb.AppendLine($"                b.HasKey({ string.Join(", ", keys.Select(k => $"\"{k}\"")) });");
                        }
                        else if (keys.Count == 1)
                        {
                            sb.AppendLine($"                b.HasKey(\"Id\");");
                        }

                        columns.ForEach(c =>
                        {
                            var propConfig = $"                b.Property(x => x.{c.Name})";
                            var shouldConfig = false;

                            if (keys.Count == 1 && c.Name == keys[0])
                            {
                                propConfig = $"                b.Property(x => x.Id)";

                                if (!c.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    propConfig = $"                b.Property(x => x.Id).HasColumnName(\"{(c.Name)}\")";
                                    shouldConfig = true;
                                }
                            }

                            if (!c.Nullable)
                            {
                                propConfig += ".IsRequired()";
                                shouldConfig = true;
                            }

                            if (c.CharMaxLength.HasValue && c.CharMaxLength > 0)
                            {
                                propConfig += $".HasMaxLength({c.CharMaxLength})";
                                shouldConfig = true;
                            }

                            if (c.Type.Contains("decimal") ||
                                c.Type.Contains("datetime2") ||
                                c.Type.Contains("numeric") ||
                                c.Type.Contains("money") ||
                                c.Type.Contains("float") ||
                                c.Type.Contains("binary") ||
                                c.Type.Contains("time")
                                )
                            {
                                propConfig += $".HasColumnType(\"{_getSqlType(c)}\")";
                                shouldConfig = true;
                            }

                            if (shouldConfig)
                            {
                                sb.AppendLine($"                {propConfig};");
                            }

                        });

                        sb.AppendLine("            });");

                        if (doneToConfirmContinue != null)
                        {
                            if (!doneToConfirmContinue(t.Name)) break;
                        }

                    }

                    sb.AppendLine("}");
                    sb.AppendLine("}");

                    if (!saved) _saveAndClose();

                }
            }
        }

    }
}
