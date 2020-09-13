﻿using EnvDTE;
using ISoft.Metabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ISoft.Coder
{
    public partial class SqlServerClassGenWrapper
    {
        private void _Do_4(Func<string, bool> doneToConfirmContinue = null)
        {
            if (string.IsNullOrEmpty(_Namespace))
            {
                MessageBox.Show("Please provide a namespace");
                return;
            }

            var parts = _Namespace.SplitEx(':');
            var template = GetTemplatePath("ABP.Context");
            var now = DateTime.Now;
            var projects = (Array)_App.ActiveSolutionProjects;

            // 准备插入代码
            ProjectItem file = null;
            Window win = null;
            TextSelection ts = null;

            var batchIndex = -1;
            var saved = true;

            Action<ProjectItem> _newFile = f =>
            {
                batchIndex++;
                file = f.ProjectItems.AddFromTemplate(template, $"{parts[1]}.cs");
                win = file.Open(Constants.vsViewKindCode);
                win.Activate();
                win.Document.Activate();

                ts = _App.ActiveDocument.Selection as TextSelection;
                ts.EndOfDocument();

                // 插入生成日期
                ts.Insert(@"/// <summary>");
                ts.NewLine();
                ts.Insert($"{now}");
                ts.NewLine();
                ts.Insert(@"</summary>");
                ts.NewLine();
                ts.SelectLine();
                ts.Insert(" ");

                // 插入 namespace 行
                ts.Insert("namespace " + parts[0]);
                ts.NewLine();
                ts.Insert("{");
                ts.NewLine();

                saved = false;
            };

            Action _saveAndClose = () =>
            {
                ts.Insert("}");
                ts.NewLine();
                ts.SelectAll();

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

                    ts.Insert($"public static class {parts[1]}");
                    ts.NewLine();
                    ts.Insert("{");
                    ts.NewLine();

                    ts.Insert($"public static void ConfigureInternal(this ModelBuilder builder)");
                    ts.NewLine();
                    ts.Insert("{");
                    ts.NewLine();

                    ts.Insert($"Check.NotNull(builder, nameof(builder));");
                    ts.NewLine();

                    for (int i = 0; i < tables.Count; i++)
                    {
                        var t = tables[i];

                        List<string> keys = new List<string>();
                        if (!
                            string.IsNullOrEmpty(t.KeyInfo))
                            foreach (string part in t.KeyInfo.Split(','))
                                if (part.Trim().Length > 0) keys.Add(part.Trim());

                        var columns = _Context.Columns
                            .Where(c => c.TableId == t.TableId).OrderBy(c => c.Name).ToList();
                        var properties = _Context.Properties
                            .Where(d => d.TableId == t.TableId).ToList();


                        ts.Insert($"            builder.Entity<BO_{t.Name}>(b =>");
                        ts.NewLine();
                        ts.Insert("{");
                        ts.NewLine();
                        ts.Insert($"                b.ToTable(\"{t.Name}\", AbpCommonDbProperties.DbSchema);");
                        ts.NewLine();
                        ts.Insert("                b.ConfigureByConvention();");
                        ts.NewLine();

                        columns.ForEach(c =>
                        {
                            var propConfig = $"                b.Property(x => x.{c.Name})";
                            var shouldConfig = false;

                            if (keys.Count == 1 && c.Name == "Id")
                            {
                                propConfig += $"                b.Property(x => x.Id).HasColumnName(\"{(c.Name)}\")";
                                shouldConfig = true;
                            }

                            if (!c.Nullable)
                            {
                                propConfig += ".IsRequired()";
                                shouldConfig = true;
                            }

                            if (c.CharMaxLength.HasValue)
                            {
                                propConfig += $".HasMaxLength({c.CharMaxLength})";
                                shouldConfig = true;
                            }

                            if (c.Type.StartsWith("decimal") ||
                                c.Type.StartsWith("datetime2")
                                )
                            {
                                propConfig += $".HasColumnType(\"{_getSqlType(c)}\")";
                                shouldConfig = true;
                            }

                            if (shouldConfig)
                            {
                                ts.Insert($"                {propConfig};");
                                ts.NewLine();
                            }

                        });

                        ts.Insert("            });");
                        ts.NewLine();

                        if (doneToConfirmContinue != null)
                        {
                            if (!doneToConfirmContinue(t.Name)) break;
                        }

                    }

                    ts.Insert("}");
                    ts.NewLine();
                    ts.Insert("}");
                    ts.NewLine();

                    if (!saved) _saveAndClose();

                }
            }
        }

    }
}
