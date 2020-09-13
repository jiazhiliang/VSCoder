using EnvDTE;
using ISoft.Metabase;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var template = GetTemplatePath("ABP.Entity");
            var now = DateTime.Now;
            var projects = (Array)_App.ActiveSolutionProjects;
            if (projects.Length > 0)
            {
                foreach (Project p in projects)
                {
                    ProjectItem folder = p.ProjectItems
                            .AddFolder("Entities", Constants.vsProjectItemKindPhysicalFolder);
                    List<MBTable> tables =
                        _Context.Tables.Where(_Filter).OrderBy(t => t.Name).ToList();

                    // 准备插入代码
                    ProjectItem file = null;
                    Window win = null;
                    TextSelection ts = null;

                    for (int i = 0; i < tables.Count; i++)
                    {
                        var t = tables[i];

                        // 创建新文件，并且在最后方插入
                        file = folder.ProjectItems.AddFromTemplate(template, $"{t.Name}.cs");
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
                        ts.Insert("namespace " + _Namespace);
                        ts.NewLine();
                        ts.Insert("{");
                        ts.NewLine();

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
                                    ts.Insert(@"/// <summary>");
                                    ts.NewLine();
                                    ts.Insert($"{d.Value}");
                                    ts.NewLine();
                                    ts.Insert(@"</summary>");
                                    ts.NewLine();
                                    ts.SelectLine();
                                    ts.Insert(" ");
                                });
                        }

                        if (keys.Count == 0)
                        {
                            doneToConfirmContinue($"Error: no primary key found for {t.Name}");
                            return;
                        }

                        // Decide base class
                        var isCompoundKey = keys.Count > 1;
                        var singleKeyType = string.Empty;
                        var baseType = "Entity";

                        if (!
                            isCompoundKey)
                        {
                            singleKeyType = _getType(columns.First(c => c.Name == keys[0]));
                        }

                        if (columns.Exists(c => c.Name == "CreationTime" && c.Type.StartsWith("datetime")) &&
                            columns.Exists(c => c.Name == "CreatorId" && c.Type.StartsWith("uniqueidentifier")))
                        {
                            baseType = "CreationAuditedEntity";

                            if (columns.Exists(c => c.Name == "LastModificationTime" && c.Type.StartsWith("datetime")) &&
                                columns.Exists(c => c.Name == "LastModifierId" && c.Type.StartsWith("uniqueidentifier")))
                            {
                                baseType = "AuditedEntity";

                                if (columns.Exists(c => c.Name == "DeletionTime" && c.Type.StartsWith("datetime")) &&
                                    columns.Exists(c => c.Name == "DeleterId" && c.Type.StartsWith("uniqueidentifier")) &&
                                    columns.Exists(c => c.Name == "IsDeleted" && c.Type.StartsWith("bit")))
                                {
                                    baseType = "FullAuditedEntity";
                                }
                            }
                        }

                        if (!isCompoundKey)
                        {
                            baseType += $"<{singleKeyType}>";
                        }

                        // 表格名字
                        ts.NewLine();
                        ts.Insert($"public partial class BO_{t.Name}:{baseType}{{");
                        ts.NewLine();

                        // public constructor
                        ts.Insert($"public BO_{t.Name}();");
                        ts.NewLine();

                        if (isCompoundKey)
                        {
                            ts.Insert($"public override object[] GetKeys() => new object[] {{ {t.KeyInfo} }};");
                            ts.NewLine();
                        }

                        columns.ForEach(c =>
                        {
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
                                        ts.Insert(@"/// <summary>");
                                        ts.NewLine();
                                        ts.Insert($"{d.Value}");
                                        ts.NewLine();
                                        ts.Insert(@"</summary>");
                                        ts.NewLine();
                                        ts.SelectLine();
                                        ts.Insert(" ");
                                    });
                            }

                            var s = "public ";
                            s += _getType(c) + " ";
                            s += c.Name;
                            s += " { get; set; }";

                            ts.Insert(s);
                            ts.NewLine();
                        });

                        ts.Insert("}");
                        ts.NewLine();

                        if (doneToConfirmContinue != null)
                        {
                            if (!doneToConfirmContinue(t.Name)) break;
                        }

                        ts.Insert("}");
                        ts.NewLine();
                        ts.SelectAll();

                        _App.ExecuteCommand("Edit.FormatDocument");
                        win.Close(vsSaveChanges.vsSaveChangesYes);

                    }
                }
            }
        }

    }
}
