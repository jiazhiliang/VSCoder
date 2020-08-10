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
        /// <summary>
        /// Write an entity file
        /// </summary>
        /// <param name="doneToConfirmContinue"></param>
        private void _Do_2(Func<string, bool> doneToConfirmContinue = null)
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
                        file = folder.ProjectItems.AddFromTemplate(
                            TemplateFile, string.Format("{0}.cs", t.Name));
                        win = file.Open(Constants.vsViewKindCode);
                        win.Activate();
                        win.Document.Activate();

                        ts = _App.ActiveDocument.Selection as TextSelection;
                        ts.EndOfDocument();

                        // 插入生成日期
                        ts.Insert(@"/// <summary>");
                        ts.NewLine();
                        ts.Insert(string.Format(@"{0}", now.ToString()));
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
                            if (!string.IsNullOrEmpty(t.Caption))
                            {
                                ts.Insert(@"/// <summary>");
                                ts.NewLine();
                                ts.Insert(string.Format(@"{0}", t.Caption.ToStringEx()));
                                ts.NewLine();
                                ts.Insert(@"</summary>");
                                ts.NewLine();
                                ts.SelectLine();
                                ts.Insert(" ");
                            }
                        }
                        else
                        {
                            properties.SingleOrDefault(d => d.TableId == t.TableId &&
                                d.Field == string.Empty && d.Name == FIELD_SUMMARY &&
                                !string.IsNullOrEmpty(d.Value)).IfNN(d =>
                                {
                                    ts.Insert(@"/// <summary>");
                                    ts.NewLine();
                                    ts.Insert(string.Format(@"{0}", d.Value));
                                    ts.NewLine();
                                    ts.Insert(@"</summary>");
                                    ts.NewLine();
                                    ts.SelectLine();
                                    ts.Insert(" ");
                                });
                        }

                        // 表格名字
                        ts.Insert("[Serializable]");
                        ts.NewLine();
                        ts.Insert(string.Format("[Table(\"{0}\")]", t.Name));
                        ts.NewLine();
                        ts.Insert(string.Format("public partial class TB_{0}:TBObject<TB_{0}>{{", t.Name));
                        ts.NewLine();
                        //ts.Insert(string.Format("public partial class ET_{0} {{", t.Name));
                        //ts.NewLine();
                        columns.ForEach(c =>
                        {
                            if (_Context.IsMySql)
                            {
                                if (!string.IsNullOrEmpty(c.Caption))
                                {
                                    ts.Insert(@"/// <summary>");
                                    ts.NewLine();
                                    ts.Insert(string.Format(@"{0}", c.Caption));
                                    ts.NewLine();
                                    ts.Insert(@"</summary>");
                                    ts.NewLine();
                                    ts.SelectLine();
                                    ts.Insert(" ");
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
                                        ts.Insert(@"/// <summary>");
                                        ts.NewLine();
                                        ts.Insert(string.Format(@"{0}", d.Value));
                                        ts.NewLine();
                                        ts.Insert(@"</summary>");
                                        ts.NewLine();
                                        ts.SelectLine();
                                        ts.Insert(" ");
                                    });
                            }

                            if (t.KeyInfo.ToStringEx(string.Empty).Contains(c.Name))
                            {
                                //var singleKey = !t.KeyInfo.ToStringEx(string.Empty).Contains(",");
                                //if (singleKey && c.Type.Contains("int"))
                                //{
                                //    ts.Insert(@"[Key*]"); // 人为编译不成功，mySql 的问题
                                //}
                                //else
                                //{
                                //    ts.Insert(@"[Key]");
                                //}

                                ts.Insert(@"[Key]");
                                ts.NewLine();

                            }

                            ts.Insert(string.Format(@"[Column(Order = {0})]", c.Ordinal));
                            ts.NewLine();

                            if (c.CharMaxLength.HasValue &&
                                    !c.Type.Contains("blob") &&
                                    !c.Type.Contains("long") &&
                                    !c.Type.Contains("text")
                                    //!c.Spec.Contains("char(36)") // guid
                                    )
                            {
                                ts.Insert(string.Format(@"[MaxLength({0})]", c.CharMaxLength));
                                ts.NewLine();
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
