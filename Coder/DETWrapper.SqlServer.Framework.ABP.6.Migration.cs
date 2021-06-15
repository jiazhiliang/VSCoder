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
        private string _getMigratingType(MBColumn c)
        {
            switch (c.Type)
            {
                case _Types.t_bit:
                case _Types.t_boolean:
                    return "Boolean()";
                case _Types.t_tinyint:
                    return "Byte()";
                case _Types.t_smallint:
                case _Types.t_year:
                    return "Int16()";
                case _Types.t_int:
                case _Types.t_integer:
                case _Types.t_mediumint:
                    return "Int32()";
                case _Types.t_bigint:
                    return "Int64()";
                case _Types.t_float:
                case _Types.t_double:
                case _Types.t_real:
                    return "Float()";
                case _Types.t_rowversion:
                case _Types.t_image:
                case _Types.t_timestamp:
                    return "Binary()";
                case _Types.t_numeric:
                case _Types.t_decimal:
                    return $"Decimal({c.NumericPrecision},{c.NumericScale})";
                case _Types.t_dec:
                case _Types.t_fixed:
                case _Types.t_serial:
                    return "Decimal()";
                case _Types.t_date:
                case _Types.t_datetime:
                    return "DateTime()";
                case _Types.t_datetime2:
                    return "DateTime2()";
                case _Types.t_datetimeoffset:
                case _Types.t_time:
                    return "DateTimeOffset()";
                case _Types.t_smalldatetime:
                    return "DateTime()";
                case _Types.t_money:
                case _Types.t_smallmoney:
                    return "Decimal()";
                case _Types.t_uniqueidentifier:
                    return "Guid()";
                case _Types.t_char:
                    return "String()";
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
                    return "String()";
                case _Types.t_binary:
                case _Types.t_varbinary:
                case _Types.t_tinyblob:
                case _Types.t_blob:
                case _Types.t_mediumblob:
                case _Types.t_longblob:
                case _Types.t_spatial_geometry:
                case _Types.t_spatial_geography:
                case _Types.t_sql_variant:
                    return "Binary()";
            }

            return string.Empty;
        }


        private void _Do_5(Func<string, bool> doneToConfirmContinue = null)
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
            var template = GetTemplatePath("ABP.Migration");
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

                    sb.AppendLine($"public class {parts[1]} : AutoReversingMigration");
                    sb.AppendLine("{");

                    sb.AppendLine($"public override void Up()");
                    sb.AppendLine("{");

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

                        sb.AppendLine($"            var b{t.Name} = Create.Table(\"{t.Name}\")");

                        columns.ForEach(c =>
                        {
                            sb.Append($"            .WithColumn(\"{c.Name}\").As{_getMigratingType(c)}");

                            if (!c.Nullable)
                            {
                                sb.Append(".NotNullable()");
                            }

                            if (keys.Contains(c.Name))
                            {
                                sb.Append(".PrimaryKey()");
                            }
                        });

                        sb.AppendLine(";");
                        sb.AppendLine();

                    }

                    sb.AppendLine("}");
                    sb.AppendLine("}");

                    if (!saved) _saveAndClose();

                }
            }
        }

    }
}
