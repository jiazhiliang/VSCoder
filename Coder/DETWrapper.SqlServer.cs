﻿using EnvDTE;

using EnvDTE80;

using ISoft.Metabase;

using LinqKit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
namespace ISoft.Coder
{
    /// <summary>
    /// Wrapper class for SqlServer
    /// </summary>
    public partial class SqlServerClassGenWrapper : BaseDTEWrapper
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
                case 2:
                    _Do_2(doneToConfirmContinue);
                    break;
                case 3:
                    _Do_3(doneToConfirmContinue);
                    break;
                case 4:
                    _Do_4(doneToConfirmContinue);
                    break;
                case 5:
                    _Do_5(doneToConfirmContinue);
                    break;
                case 6:
                    _Do_6(doneToConfirmContinue);
                    break;
                case 7:
                    _Do_7(doneToConfirmContinue);
                    break;
            }
        }

        private void _Do_0(Func<string, bool> doneToConfirmContinue = null)
        {
            if (_Context != null)
            {
                _Context.Rebuild(
                    tableExcludeContains: new string[] { "__MigrationHistory" },
                    doneToConfirmContinue: doneToConfirmContinue);
            }
        }

        private string _getType(MBColumn c)
        {
            switch (c.Type)
            {
                case _Types.t_bit:
                case _Types.t_boolean:
                    return (c.Nullable) ? "bool?" : "bool";
                case _Types.t_tinyint:
                    return (c.Nullable) ? "byte?" : "byte";
                case _Types.t_smallint:
                    return (c.Nullable) ? "short?" : "short";
                case _Types.t_year:
                    return (c.Nullable) ? "short?" : "short";
                case _Types.t_int:
                    return (c.Nullable) ? "int?" : "int";
                case _Types.t_integer:
                    return (c.Nullable) ? "int?" : "int";
                case _Types.t_mediumint:
                    return (c.Nullable) ? "int?" : "int";
                case _Types.t_bigint:
                    return (c.Nullable) ? "long?" : "long";
                case _Types.t_float:
                    return (c.Nullable) ? "double?" : "double";
                case _Types.t_double:
                case _Types.t_real:
                    return (c.Nullable) ? "float?" : "float";
                case _Types.t_rowversion:
                    return "byte[]";
                case _Types.t_numeric:
                case _Types.t_decimal:
                case _Types.t_dec:
                case _Types.t_fixed:
                case _Types.t_serial:
                    return (c.Nullable) ? "decimal?" : "decimal";
                case _Types.t_date:
                case _Types.t_datetime:
                case _Types.t_datetime2:
                    return (c.Nullable) ? "DateTime?" : "DateTime";
                case _Types.t_timestamp:
                    if (_Context.IsMySql) return (c.Nullable) ? "DateTime?" : "DateTime";
                    return "byte[]";
                case _Types.t_datetimeoffset:
                    return (c.Nullable) ? "DateTimeOffset?" : "DateTimeOffset";
                case _Types.t_time:
                    return (c.Nullable) ? "TimeSpan?" : "TimeSpan";

                case _Types.t_smalldatetime:
                    return (c.Nullable) ? "DateTime?" : "DateTime";
                case _Types.t_image:
                    return "byte[]";
                case _Types.t_money:
                case _Types.t_smallmoney:
                    return (c.Nullable) ? "decimal?" : "decimal";
                case _Types.t_uniqueidentifier:
                    return (c.Nullable) ? "Guid?" : "Guid";
                case _Types.t_char:
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
        }

        private string _getSqlType(MBColumn c)
        {
            switch (c.Type)
            {
                case _Types.t_bit:
                case _Types.t_boolean:
                case _Types.t_tinyint:
                case _Types.t_smallint:
                case _Types.t_year:
                case _Types.t_int:
                case _Types.t_integer:
                case _Types.t_mediumint:
                case _Types.t_bigint:
                case _Types.t_float:
                case _Types.t_double:
                case _Types.t_real:
                case _Types.t_rowversion:
                case _Types.t_dec:
                case _Types.t_fixed:
                case _Types.t_serial:
                case _Types.t_date:
                case _Types.t_datetime:
                case _Types.t_timestamp:
                case _Types.t_smalldatetime:
                case _Types.t_image:
                case _Types.t_money:
                case _Types.t_smallmoney:
                case _Types.t_uniqueidentifier:
                case _Types.t_tinytext:
                case _Types.t_text:
                case _Types.t_mediumtext:
                case _Types.t_longtext:
                case _Types.t_set:
                case _Types.t_enum:
                case _Types.t_ntext:
                case _Types.t_xml:
                case _Types.t_blob:
                case _Types.t_tinyblob:
                case _Types.t_mediumblob:
                case _Types.t_longblob:
                case _Types.t_spatial_geometry:
                case _Types.t_spatial_geography:
                case _Types.t_sql_variant:
                    return c.Type;
                case _Types.t_datetime2:
                case _Types.t_datetimeoffset:
                case _Types.t_time:
                    return $"{c.Type}({c.DateTimePrecision})";
                case _Types.t_numeric:
                case _Types.t_decimal:
                    return $"{c.Type}({c.NumericPrecision},{c.NumericScale})";
                case _Types.t_char:
                case _Types.t_varchar:
                case _Types.t_nchar:
                case _Types.t_nvarchar:
                case _Types.t_binary:
                case _Types.t_varbinary:
                    return $"{c.Type}({c.CharMaxLength})";
            }

            return string.Empty;
        }

        private void _Do_1(Func<string, bool> doneToConfirmContinue = null)
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
                        sb.AppendLine(string.Format("public partial class TB_{0}:TBObject<TB_{0}>{{", t.Name));
                        //sb.AppendLine(string.Format("public partial class ET_{0} {{", t.Name));
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

                            sb.AppendLine(string.Format(@"[Column(Order = {0})]", c.Ordinal));

                            if (c.CharMaxLength.HasValue &&
                                    !c.Type.Contains("blob") &&
                                    !c.Type.Contains("long") &&
                                    !c.Type.Contains("text")
                                    //!c.Spec.Contains("char(36)") // guid
                                    )
                            {
                                sb.AppendLine(string.Format(@"[MaxLength({0})]", c.CharMaxLength));
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

        public SqlServerClassGenWrapper(
            SqlServerEFContext context, DTE2 app, string ns, Expression<Func<MBTable, bool>> tableFilter = null,
            int batchSize = 50)
            : this(app, ns, tableFilter)
        {
            _BatchSize = 10000;
            _Context = context;
        }
        public SqlServerClassGenWrapper(
            DTE2 app, string ns, Expression<Func<MBTable, bool>> tableFilter = null)
            : base(app, ns)
        {
            _Filter = tableFilter;
            _Identifier = "EF";
            _Operations.Add("Re-populate meta info ____table");
            _Operations.Add("Generate class files（POCO）");
            _Operations.Add("ABP: generate entity class");
            _Operations.Add("ABP: generate DTO");
            _Operations.Add("ABP: generate EF table spec");
            _Operations.Add("ABP: generate migration");
            _Operations.Add("ABP: generate airtable");
            _Operations.Add("EFCore: generate table as entity class");
        }

    }
}
