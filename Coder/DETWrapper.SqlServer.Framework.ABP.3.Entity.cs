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
        private void _Do_2(Func<string, bool> doneToConfirmContinue = null)
        {
            if (string.IsNullOrEmpty(_Namespace))
            {
                MessageBox.Show("Please provide a namespace");
                return;
            }

            if (!_Namespace.Contains(":"))
            {
                MessageBox.Show("Please provide a namespace:objectPrefix");
                return;
            }

            var parts = _Namespace.SplitEx(':');
            var template = GetTemplatePath("ABP.Context");
            var now = DateTime.Now;
            var projects = (Array)_App.ActiveSolutionProjects;

            var boPrefix = parts[1];
            var nameSpace = parts[0];


            // 准备插入代码
            ProjectItem file = null;
            Window win = null;
            TextSelection ts = null;
            StringBuilder sb = null;

            var batchIndex = -1;
            var batchSize = 10000;
            var saved = true;

            Action<ProjectItem> _newFile = f =>
            {
                batchIndex++;
                file = f.ProjectItems.AddFromTemplate(template, $"{boPrefix}.ALL.cs");
                win = file.Open(Constants.vsViewKindCode);
                sb = new StringBuilder();

                ts = win.Document.Selection as TextSelection;
                // ts.EndOfDocument();

                // 插入生成日期
                sb.AppendLine(@"/// <summary>");
                sb.AppendLine($"/// {now}");
                sb.AppendLine(@"/// </summary>");

                // 插入 namespace 行
                sb.AppendLine("namespace " + nameSpace);
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
                            .AddFolder("_Entities", Constants.vsProjectItemKindPhysicalFolder);
                    List<MBTable> tables =
                        _Context.Tables.Where(_Filter).OrderBy(t => t.Name).ToList();

                    for (int i = 0; i < tables.Count; i++)
                    {
                        if (i % batchSize == 0)
                        {
                            _newFile(folder);
                        }

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

                        // 说明
                        if (_Context.IsMySql)
                        {
                        }
                        else
                        {
                            properties.SingleOrDefault(d => d.TableId == t.TableId &&
                                d.Field == string.Empty && d.Name == FIELD_SUMMARY &&
                                !string.IsNullOrEmpty(d.Value)).IfNN(d =>
                                {
                                    sb.AppendLine(@"/// <summary>");
                                    sb.AppendLine($"/// {d.Value}");
                                    sb.AppendLine(@"/// </summary>");
                                });
                        }

                        if (keys.Count == 0)
                        {
                            doneToConfirmContinue($"Error: no primary key found for {t.Name}");
                            continue;
                        }

                        // Decide base class
                        var isCompoundKey = keys.Count > 1;
                        var singleKeyType = string.Empty;
                        var baseType = "Entity";
                        var ignoreKeys = new List<string>();

                        if (!
                            isCompoundKey)
                        {
                            singleKeyType = _getType(columns.First(c => c.Name == keys[0]));
                            ignoreKeys.Add(keys[0]);
                        }

                        if (columns.Exists(c => c.Name == "CreationTime" && c.Type.StartsWith("datetime")) &&
                            columns.Exists(c => c.Name == "CreatorId" && c.Type.StartsWith("uniqueidentifier")))
                        {
                            baseType = "CreationAuditedEntity";
                            ignoreKeys.AddRange(new string[] { "CreationTime", "CreatorId" });

                            if (columns.Exists(c => c.Name == "LastModificationTime" && c.Type.StartsWith("datetime")) &&
                                columns.Exists(c => c.Name == "LastModifierId" && c.Type.StartsWith("uniqueidentifier")))
                            {
                                ignoreKeys.AddRange(new string[] { "LastModificationTime", "LastModifierId" });
                                baseType = "AuditedEntity";

                                if (columns.Exists(c => c.Name == "DeletionTime" && c.Type.StartsWith("datetime")) &&
                                    columns.Exists(c => c.Name == "DeleterId" && c.Type.StartsWith("uniqueidentifier")) &&
                                    columns.Exists(c => c.Name == "IsDeleted" && c.Type.StartsWith("bit")))
                                {
                                    ignoreKeys.AddRange(new string[] { "DeletionTime", "DeleterId", "IsDeleted" });
                                    baseType = "FullAuditedEntity";
                                }
                            }
                        }

                        if (!isCompoundKey)
                        {
                            baseType += $"<{singleKeyType}>";
                        }

                        // 表格名字
                        sb.AppendLine($"public partial class {boPrefix}_{t.Name}:{baseType}{{");

                        // public constructor
                        sb.AppendLine($"public {boPrefix}_{t.Name}(){{}}");

                        if (isCompoundKey)
                        {
                            sb.AppendLine($"public override object[] GetKeys() => new object[] {{ {t.KeyInfo} }};");
                        }

                        columns.ForEach(c =>
                        {
                            if (ignoreKeys.Contains(c.Name)) return;

                            // Summary
                            if (_Context.IsMySql)
                            {
                            }
                            else
                            {
                                // 说明
                                properties.SingleOrDefault(d =>
                                    d.TableId == t.TableId &&
                                    d.Field == c.Name && d.Name == FIELD_SUMMARY &&
                                    !string.IsNullOrEmpty(d.Value)).IfNN(d =>
                                    {
                                        sb.AppendLine(@"/// <summary>");
                                        sb.AppendLine($"/// {d.Value}");
                                        sb.AppendLine(@"/// </summary>");
                                    });
                            }

                            var s = "public ";
                            s += _getType(c) + " ";
                            s += c.Name;
                            s += " { get; set; }";

                            sb.AppendLine(s);
                        });

                        sb.AppendLine("}");

                        if (doneToConfirmContinue != null)
                        {
                            if (!doneToConfirmContinue(t.Name)) break;
                        }

                        if (i > 0 && ((i + 1) % batchSize) == 0)
                        {
                            _saveAndClose();
                        }

                    }

                    if (!saved) _saveAndClose();

                }
            }
        }

    }
}
