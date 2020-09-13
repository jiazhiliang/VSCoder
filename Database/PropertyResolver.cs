using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;

namespace ISoft.Metabase
{

    public abstract class PropertyResolver
    {
        public abstract void ResolveTableAttributes(DataRowView rv, MBTable table);
        public abstract void ResolveTablePrimaryKey(
            Database db, DbSet<MBProperty> properties, MBTable table);
        public abstract void ResolveTableProperties(
            Database db, DbSet<MBProperty> properties, MBTable table);
        public abstract void ResolveColumnAttributes(DataRowView rv, MBColumn column);
        public abstract void ResolveColumnProperties(
            Database db, DbSet<MBProperty> properties, MBTable table, MBColumn column);

    }

    public sealed class SqlServerPropertyResolver : PropertyResolver
    {
        private string propertyGet_Table =
            "select * from sys.fn_listextendedproperty (default, 'schema', 'dbo', 'table', '{0}', null, null)";
        private string propertyGet_Column =
            "select * from sys.fn_listextendedproperty (default, 'schema', 'dbo', 'table', '{0}', 'column', '{1}')";

        public override void ResolveTableAttributes(DataRowView rv, MBTable table)
        {
        }

        public override void ResolveTablePrimaryKey(Database db, DbSet<MBProperty> properties, MBTable table)
        {
            var cmd = db.Connection.CreateCommand();
            cmd.CommandText = "exec sp_helpconstraint '" + table.Name + "'";
            db.Connection.Open();
            using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                reader.NextResult();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(0).ToString().StartsWith("PRIMARY KEY"))
                            table.KeyInfo = reader.GetString(6);
                    }
                }
            }
        }

        public override void ResolveTableProperties(Database db, DbSet<MBProperty> properties, MBTable table)
        {
            var cmd = db.Connection.CreateCommand();
            cmd.CommandText = string.Format(propertyGet_Table, table.Name);
            db.Connection.Open();
            using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var objValue = reader.GetValue(3);
                        var mProperty = new MBProperty()
                        {
                            TableId = table.TableId,
                            Field = string.Empty,
                            Name = reader.GetString(2).ToString(),
                            Value = objValue == null ? null : objValue.ToString()
                        };

                        switch (mProperty.Name)
                        {
                            case "Caption":
                                table.Caption = mProperty.Value;
                                break;
                        }

                        properties.Add(mProperty);
                    }
                }
            }
        }

        public override void ResolveColumnAttributes(DataRowView rv, MBColumn column)
        {
            column.CharsetSchema = rv["CHARACTER_SET_SCHEMA"].ToString();
            column.CollationCatalog = rv["COLLATION_CATALOG"].ToString();

            if (rv["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                column.CharMaxLength =
                    int.Parse(rv["CHARACTER_MAXIMUM_LENGTH"].ToString());

            if (rv["CHARACTER_OCTET_LENGTH"] != DBNull.Value)
                column.CharOctLength =
                    int.Parse(rv["CHARACTER_OCTET_LENGTH"].ToString());

            if (rv["NUMERIC_PRECISION"] != DBNull.Value)
                column.NumericPrecision =
                    int.Parse(rv["NUMERIC_PRECISION"].ToString());

            if (rv["NUMERIC_PRECISION_RADIX"] != DBNull.Value)
                column.NumericPrecisionRadix =
                    int.Parse(rv["NUMERIC_PRECISION_RADIX"].ToString());

            if (rv["NUMERIC_SCALE"] != DBNull.Value)
                column.NumericScale =
                    int.Parse(rv["NUMERIC_SCALE"].ToString());

            if (rv["DATETIME_PRECISION"] != DBNull.Value)
                column.DateTimePrecision =
                    int.Parse(rv["DATETIME_PRECISION"].ToString());
        }

        public override void ResolveColumnProperties(Database db, DbSet<MBProperty> properties, MBTable table, MBColumn column)
        {
            var cmd = db.Connection.CreateCommand();
            cmd = db.Connection.CreateCommand();
            cmd.CommandText = string.Format(propertyGet_Column, table.Name, column.Name);
            db.Connection.Open();
            using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var objValue = reader.GetValue(3);
                        var mProperty = new MBProperty()
                        {
                            TableId = table.TableId,
                            Field = column.Name,
                            Name = reader.GetString(2).ToString(),
                            Value = objValue == null ? null : objValue.ToString()
                        };

                        switch (mProperty.Name)
                        {
                            case "Caption":
                                column.Caption = mProperty.Value;
                                break;
                        }

                        properties.Add(mProperty);
                    }
                }
            }
        }
    }

    public sealed class MySqlPropertyResolver : PropertyResolver
    {
        public override void ResolveTableAttributes(DataRowView rv, MBTable table)
        {
            if (rv["TABLE_COMMENT"] != DBNull.Value)
                table.Caption = rv["TABLE_COMMENT"].ToString();
        }

        public override void ResolveTablePrimaryKey(Database db, DbSet<MBProperty> properties, MBTable table)
        {
            var cmd = db.Connection.CreateCommand();
            cmd.CommandText = string.Format(
                @"select column_name from information_schema.key_column_usage where table_schema = '{0}' and table_name = '{1}' and constraint_name = 'PRIMARY' order by ordinal_position",
                db.Connection.Database, table.Name);
            db.Connection.Open();
            var keys = new List<string>();
            using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                if (reader.HasRows)
                {
                    while (reader.Read()) keys.Add(reader.GetString(0));
                }
            }

            if (keys.Count > 0)
            {
                table.KeyInfo = keys.ToFlat();
            }
        }

        public override void ResolveTableProperties(Database db, DbSet<MBProperty> properties, MBTable table)
        {
        }

        public override void ResolveColumnAttributes(DataRowView rv, MBColumn column)
        {
            // column.CharsetSchema = rv["CHARACTER_SET_SCHEMA"].ToString();
            column.CollationCatalog = rv["COLLATION_NAME"].ToString();

            if (rv["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                column.CharMaxLength =
                    long.Parse(rv["CHARACTER_MAXIMUM_LENGTH"].ToString());

            //if (rv["CHARACTER_OCTET_LENGTH"] != DBNull.Value)
            //    column.CharOctLength =
            //        int.Parse(rv["CHARACTER_OCTET_LENGTH"].ToString());

            if (rv["NUMERIC_PRECISION"] != DBNull.Value)
                column.NumericPrecision =
                    long.Parse(rv["NUMERIC_PRECISION"].ToString());

            //if (rv["NUMERIC_PRECISION_RADIX"] != DBNull.Value)
            //    column.NumericPrecisionRadix =
            //        int.Parse(rv["NUMERIC_PRECISION_RADIX"].ToString());

            if (rv["NUMERIC_SCALE"] != DBNull.Value)
                column.NumericScale =
                    long.Parse(rv["NUMERIC_SCALE"].ToString());

            if (rv["DATETIME_PRECISION"] != DBNull.Value)
                column.DateTimePrecision =
                    long.Parse(rv["DATETIME_PRECISION"].ToString());

            if (rv["COLUMN_COMMENT"] != DBNull.Value)
                column.Caption = rv["COLUMN_COMMENT"].ToString();

            if (rv["COLUMN_TYPE"] != DBNull.Value)
                column.Spec = rv["COLUMN_TYPE"].ToString();

        }

        public override void ResolveColumnProperties(Database db, DbSet<MBProperty> properties, MBTable table, MBColumn column)
        {
        }

    }

}

