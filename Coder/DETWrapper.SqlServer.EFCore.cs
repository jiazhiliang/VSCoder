using EnvDTE;

using ISoft.Metabase;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace ISoft.Coder
{
    /// <summary>
    /// Wrapper class for SqlServer
    /// </summary>
    public partial class SqlServerClassGenWrapper : BaseDTEWrapper
    {
        private void _Do_7(Func<string, bool> doneToConfirmContinue = null)
        {
            if (string.IsNullOrEmpty(_Namespace))
            {
                MessageBox.Show("Please provide a namespace");
                return;
            }

            var now = DateTime.Now;
            var projects = (Array)_App.ActiveSolutionProjects;
            if (projects.Length > 0)
            {
                foreach (Project p in projects)
                {
                    ProjectItem folder = p.ProjectItems
                            .AddFolder("___ENTITIES", Constants.vsProjectItemKindPhysicalFolder);
                    List<MBTable> tables =
                        _Context.Tables.Where(_Filter).OrderBy(t => t.Name).ToList();

                    // _App Configure


                    // 准备插入代码
                    _BatchIndex = -1;
                    ProjectItem file = null;
                    Window win = null;
                    TextSelection ts = null;
                    StringBuilder sb = null;

                    var index = 0;
                    var count = -1;
                    for (int i = 0; i < tables.Count; i++)
                    {
                        index = i / _BatchSize;
                        count++;

                        if (index != _BatchIndex)
                        {
                            // 创建新文件，并且在最后方插入
                            file = folder.ProjectItems.AddFromTemplate(
                                DefaultTemplateFile, string.Format("{0}.{1}.cs", _Identifier, "ALL"));
                            win = file.Open(Constants.vsViewKindCode);
                            win.Activate();
                            win.Document.Activate();

                            ts = _App.ActiveDocument.Selection as TextSelection;
                            ts.EndOfDocument();
                            _BatchIndex = index;

                            sb = new StringBuilder();

                            // 插入生成日期
                            sb.AppendLine(@"/// <summary>");
                            sb.AppendLine(string.Format(@"/// {0}", now.ToString()));
                            sb.AppendLine(@"/// </summary>");

                            // 插入 namespace 行
                            sb.AppendLine("namespace " + _Namespace);
                            sb.AppendLine("{");
                        }

                        MBTable t = tables[i];
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
                            if (!string.IsNullOrEmpty(t.Caption))
                            {
                                sb.AppendLine(@"/// <summary>");
                                sb.AppendLine(string.Format(@"/// {0}", t.Caption.ToStringEx()));
                                sb.AppendLine(@"/// </summary>");
                            }
                        }
                        else
                        {
                            properties.SingleOrDefault(d => d.TableId == t.TableId &&
                                d.Field == string.Empty && d.Name == FIELD_SUMMARY &&
                                !string.IsNullOrEmpty(d.Value)).IfNN(d =>
                                {
                                    sb.AppendLine(@"/// <summary>");
                                    sb.AppendLine(string.Format(@"/// {0}", d.Value));
                                    sb.AppendLine(@"/// </summary>");
                                });
                        }

                        // 表格名字
                        sb.AppendLine("[Serializable]");
                        sb.AppendLine(string.Format("[Table(\"{0}\")]", t.Name));

                        if (!string.IsNullOrWhiteSpace(t.KeyInfo) && t.KeyInfo.Contains(','))
                        {
                            sb.AppendLine($"[PrimaryKey({t.KeyInfo.Split(',').Select(x => $"nameof({x})").ToList().ToFlat()})]");
                        }

                        sb.AppendLine(string.Format("public partial class TB_{0}{{", t.Name));
                        columns.ForEach(c =>
                        {
                            if (_Context.IsMySql)
                            {
                                if (!string.IsNullOrEmpty(c.Caption))
                                {
                                    sb.AppendLine(@"/// <summary>");
                                    sb.AppendLine(string.Format(@"/// {0}", c.Caption));
                                    sb.AppendLine(@"/// </summary>");
                                }
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
                                        sb.AppendLine(string.Format(@"/// {0}", d.Value));
                                        sb.AppendLine(@"/// </summary>");
                                    });
                            }

                            if (t.KeyInfo.ToStringEx(string.Empty).Contains(c.Name))
                            {
                                //var singleKey = !t.KeyInfo.ToStringEx(string.Empty).Contains(",");
                                //if (singleKey && c.Type.Contains("int"))
                                //{
                                //    sb.AppendLine(@"[Key*]"); // 人为编译不成功，mySql 的问题
                                //}
                                //else
                                //{
                                //    sb.AppendLine(@"[Key]");
                                //}

                                sb.AppendLine(@"[Key]");
                            }

                            var columnAttr = string.Format(@"[Column(Order = {0})]", c.Ordinal);

                            if (c.Type.Contains("decimal") ||
                                c.Type.Contains("datetime2") ||
                                c.Type.Contains("numeric") ||
                                c.Type.Contains("money") ||
                                c.Type.Contains("float") ||
                                c.Type.Contains("binary") ||
                                c.Type.Contains("time")
                                )
                            {
                                columnAttr = string.Format(@"[Column(Order = {0}, TypeName = ""{1}"")]", c.Ordinal, _getSqlType(c));
                            }

                            sb.AppendLine(columnAttr);

                            if (c.CharMaxLength.HasValue &&
                                    !c.Type.Contains("blob") &&
                                    !c.Type.Contains("long") &&
                                    !c.Type.Contains("text")
                                    //!c.Spec.Contains("char(36)") // guid
                                    )
                            {
                                sb.AppendLine(string.Format(@"[MaxLength({0})]", c.CharMaxLength));
                            }

                            if (!c.Nullable)
                            {
                                sb.AppendLine("[Required]");
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

                        if (count == _BatchSize - 1)
                        {
                            sb.AppendLine("}");

                            ts.Insert(sb.ToString());
                            ts.SelectAll();

                            _App.ExecuteCommand("Edit.FormatDocument");
                            win.Close(vsSaveChanges.vsSaveChangesYes);
                            count = -1;
                        }
                    }

                    // closing
                    if (count != -1)
                    {
                        sb.AppendLine("}");

                        ts.Insert(sb.ToString());
                        ts.SelectAll();

                        _App.ExecuteCommand("Edit.FormatDocument");
                        win.Close(vsSaveChanges.vsSaveChangesYes);
                        count = -1;
                    }

                }
            }
        }
    }
}
