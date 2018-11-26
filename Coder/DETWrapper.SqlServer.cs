using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.IO;

using LinqKit;
using ISoft.Metabase;
namespace ISoft.Coder
{
    /// <summary>
    /// Wrapper class for SqlServer
    /// </summary>
    public class SqlServerClassGenWrapper : BaseDTEWrapper
    {
        private const string FIELD_SUMMARY = "Caption";
        private SqlServerEFContext _Context;
        private int _BatchSize = 0;
        private int _BatchIndex = 0;
        protected Expression<Func<MBTable, bool>> _Filter = PredicateBuilder.New<MBTable>();

        private class _Types
        {
            public const string t_bit = "bit";
            public const string t_boolean = "boolean";

            public const string t_tinyint = "tinyint";
            public const string t_smallint = "smallint";
            public const string t_year = "year";
            public const string t_int = "int";
            public const string t_integer = "integer";
            public const string t_mediumint = "mediumint";
            public const string t_bigint = "bigint";

            public const string t_float = "float";
            public const string t_double = "double";
            public const string t_decimal = "decimal";
            public const string t_dec = "dec";
            public const string t_fixed = "fixed";
            public const string t_serial = "serial";
            public const string t_real = "real";

            public const string t_rowversion = "rowversion";

            public const string t_date = "date";
            public const string t_datetime = "datetime";
            public const string t_datetime2 = "datetime2";
            public const string t_datetimeoffset = "datetimeoffset";
            public const string t_time = "time";
            public const string t_smalldatetime = "smalldatetime";
            public const string t_timestamp = "timestamp";
            
            public const string t_char = "char";
            public const string t_varchar = "varchar";
            public const string t_tinytext = "tinytext";
            public const string t_text = "text";
            public const string t_mediumtext = "mediumtext";
            public const string t_longtext = "longtext";
            public const string t_set = "set";
            public const string t_enum = "enum";

            public const string t_uniqueidentifier = "uniqueidentifier";
            public const string t_image = "image";
            public const string t_money = "money";
            public const string t_smallmoney = "smallmoney";
            public const string t_nchar = "nchar";
            public const string t_ntext = "ntext";
            public const string t_numeric = "numeric";
            public const string t_nvarchar = "nvarchar";

            public const string t_binary = "binary";
            public const string t_varbinary = "varbinary";
            public const string t_tinyblob = "tinyblob";
            public const string t_blob = "blob";
            public const string t_mediumblob = "mediumblob";
            public const string t_longblob = "longblob";

            public const string t_spatial_geometry = "spatial_geometry";
            public const string t_spatial_geography = "spatial_geography";
            public const string t_sql_variant = "sql_variant";
            public const string t_xml = "xml";

        }

        protected override void _Do(int operation, Func<string, bool> doneToConfirmContinue = null)
        {
            switch (operation)
            {
                case 0:
                    _Do_0(doneToConfirmContinue);
                    break;
                case 1:
                    _Do_1(doneToConfirmContinue);
                    break;
            }
        }

        private void _Do_0(Func<string, bool> doneToConfirmContinue = null)
        {
            if (string.IsNullOrEmpty(_Namespace))
            {
                MessageBox.Show("Please provide a namespace");
                return;
            }

            Func<MBColumn, string> _getType = c =>
            {
                switch (c.Type)
                {
                    case _Types.t_bit:
#if MYSQL
                        if (c.Spec.Contains("(1)")) return (c.Nullable) ? "Nullable<bool>" : "bool";
                        return (c.Nullable) ? "Nullable<bool>" : "bool";
#endif
                    case _Types.t_boolean:
                        return (c.Nullable) ? "Nullable<bool>" : "bool";

                    case _Types.t_tinyint:
#if MYSQL
                        if (c.Spec.EndsWith("unsigned")) return (c.Nullable) ? "Nullable<byte>" : "byte";
                        return (c.Nullable) ? "Nullable<sbyte>" : "sbyte";
#else
                        return (c.Nullable) ? "Nullable<byte>" : "byte";
#endif
                    case _Types.t_smallint:
#if MYSQL
                        if (c.Spec.EndsWith("unsigned")) return (c.Nullable) ? "Nullable<int>" : "int";
#endif
                        return (c.Nullable) ? "Nullable<short>" : "short";
                    case _Types.t_year:
                        return (c.Nullable) ? "Nullable<short>" : "short";
                    case _Types.t_int:
#if MYSQL
                        if (c.Spec.EndsWith("unsigned")) return (c.Nullable) ? "Nullable<long>" : "long";
#endif
                        return (c.Nullable) ? "Nullable<int>" : "int";
                    case _Types.t_integer:
#if MYSQL
                        if (c.Spec.EndsWith("unsigned")) return (c.Nullable) ? "Nullable<long>" : "long";
#endif
                        return (c.Nullable) ? "Nullable<int>" : "int";
                    case _Types.t_mediumint:
                        return (c.Nullable) ? "Nullable<int>" : "int";
                    case _Types.t_bigint:
#if MYSQL
                        if (c.Spec.EndsWith("unsigned")) return (c.Nullable) ? "Nullable<decimal>" : "decimal";
#endif
                        return (c.Nullable) ? "Nullable<long>" : "long";
                    case _Types.t_float:
#if MYSQL
                        if (c.Spec.EndsWith("unsigned")) return (c.Nullable) ? "Nullable<decimal>" : "decimal";
                        return (c.Nullable) ? "Nullable<float>" : "float";
#else
                        return (c.Nullable) ? "Nullable<double>" : "double";
#endif
                    case _Types.t_double:
#if MYSQL
                        if (c.Spec.EndsWith("unsigned")) return (c.Nullable) ? "Nullable<decimal>" : "decimal";
                        return (c.Nullable) ? "Nullable<double>" : "double";
#endif
                    case _Types.t_real:
#if MYSQL
                        return (c.Nullable) ? "Nullable<double>" : "double";
#else
                        return (c.Nullable) ? "Nullable<float>" : "float";
#endif

                    case _Types.t_rowversion:

                        return "byte[]";

                    case _Types.t_numeric:
                    case _Types.t_decimal:
                    case _Types.t_dec:
                    case _Types.t_fixed:
                    case _Types.t_serial:
                        return (c.Nullable) ? "Nullable<decimal>" : "decimal";

                    case _Types.t_date:
                    case _Types.t_datetime:
                    case _Types.t_datetime2:
                        return (c.Nullable) ? "Nullable<DateTime>" : "DateTime";
                    case _Types.t_timestamp:
                        if (_Context.IsMySql) return (c.Nullable) ? "Nullable<DateTime>" : "DateTime";
                        return "byte[]";
                    case _Types.t_datetimeoffset:
                        return (c.Nullable) ? "Nullable<System.DateTimeOffset>" : "System.DateTimeOffset";
                    case _Types.t_time:
                        return (c.Nullable) ? "Nullable<System.TimeSpan>" : "System.TimeSpan";

                    case _Types.t_smalldatetime:
                        return (c.Nullable) ? "Nullable<DateTime>" : "DateTime";
                    case _Types.t_image:
                        return "byte[]";
                    case _Types.t_money:
                    case _Types.t_smallmoney:
                        return (c.Nullable) ? "Nullable<decimal>" : "decimal";
                    case _Types.t_uniqueidentifier:
                        return (c.Nullable) ? "Nullable<Guid>" : "Guid";

                    case _Types.t_char:
#if MYSQL
                        if (_Context.IsMySql && c.Spec == "char(36)")
                        {
                            // char(36) 被认为是 MySql 的一个标志性实现
                            return (c.Nullable) ? "Nullable<Guid>" : "Guid";
                        }
#endif
                        return "string";
                    case _Types.t_varchar:
                    case _Types.t_tinytext:
                    case _Types.t_text:
                    case _Types.t_mediumtext:
                    case _Types.t_longtext:
                    case _Types.t_set:
                    case _Types.t_enum:
                    case _Types.t_nchar:
                    case _Types.t_nvarchar:
                    case _Types.t_ntext:
                    case _Types.t_xml:
                        return "string";

                    case _Types.t_binary:
                    case _Types.t_varbinary:
                    case _Types.t_tinyblob:
                    case _Types.t_blob:
                    case _Types.t_mediumblob:
                    case _Types.t_longblob:
                        return "byte[]";

                    case _Types.t_spatial_geometry:
                        return (c.Nullable) ? "Nullable<System.Data.Spatial.DbGeometry>" : "System.Data.Spatial.DbGeometry";
                    case _Types.t_spatial_geography:
                        return (c.Nullable) ? "Nullable<System.Data.Spatial.DbGeography>" : "System.Data.Spatial.DbGeography";
                    case _Types.t_sql_variant:
                        return "object";
                }

                return string.Empty;
            };

            Func<ProjectItem, int, ProjectItem> _newBatch = (folder, index) =>
            {
                return folder.ProjectItems.AddFromTemplate(
                    TemplateFile, string.Format("{0}.{1}.cs", _Identifier, index.ToString("D2")));
            };

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
                                TemplateFile, string.Format("{0}.{1}.cs", _Identifier, index.ToString("D2")));
                            win = file.Open(Constants.vsViewKindCode);
                            win.Activate();
                            win.Document.Activate();

                            ts = _App.ActiveDocument.Selection as TextSelection;
                            ts.EndOfDocument();
                            _BatchIndex = index;

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

                        if (count == _BatchSize - 1)
                        {
                            ts.Insert("}");
                            ts.NewLine();
                            ts.SelectAll();
                            _App.ExecuteCommand("Edit.FormatDocument");
                            win.Close(vsSaveChanges.vsSaveChangesYes);
                            count = -1;
                        }
                    }

                    // closing
                    if (count != -1)
                    {
                        ts.Insert("}");
                        ts.NewLine();
                        ts.SelectAll();
                        _App.ExecuteCommand("Edit.FormatDocument");
                        win.Close(vsSaveChanges.vsSaveChangesYes);
                        count = -1;
                    }

                }
            }
        }

        private void _Do_1(Func<string, bool> doneToConfirmContinue = null)
        {
            if (_Context != null)
            {
                _Context.Rebuild(
                    tableExcludeContains: new string[] { "__MigrationHistory" },
                    doneToConfirmContinue: doneToConfirmContinue);
            }
        }

        public SqlServerClassGenWrapper(
            SqlServerEFContext context, DTE2 app, string ns, Expression<Func<MBTable, bool>> tableFilter = null,
            int batchSize = 50)
            : this(app, ns, tableFilter)
        {
            _BatchSize = batchSize;
            _Context = context;
        }
        public SqlServerClassGenWrapper(
            DTE2 app, string ns, Expression<Func<MBTable, bool>> tableFilter = null)
            : base(app, ns)
        {
            _Filter = tableFilter;
            _Identifier = "EF";
            _Operations.Add("Generate class files（POCO）");
            _Operations.Add("Re-populate meta info ____table");
        }

    }
}
