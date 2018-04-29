using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LinqKit;

namespace ISoft.Metabase
{
    public class SqlServerEFContext : DbContext
    {
        private const string SCHEMA_COLLECTIONNAME = "CollectionName";
        private const string SCHEMA_TABLES = "Tables";
        private const string SCHEMA_COLUMNS = "Columns";
        private PropertyResolver _Resolver = null;

        public bool IsMySql
        {
            get
            {
                if (_Resolver == null) return false;
                return (_Resolver is MySqlPropertyResolver);
            }
        }

        public ObjectContext Core
        {
            get { return (this as IObjectContextAdapter).ObjectContext; }
        }

        public DbSet<MBTable> Tables
        {
            get { return this.Set<MBTable>(); }
        }

        public DbSet<MBColumn> Columns
        {
            get { return this.Set<MBColumn>(); }
        }

        public DbSet<MBProperty> Properties
        {
            get { return this.Set<MBProperty>(); }
        }

        public string TableName<T>() where T : class
        {
            var sql = Core.CreateObjectSet<T>().ToTraceString();
            var regex = new Regex(@"FROM\s+(?<table>.+)\s+AS");
            var match = regex.Match(sql);
            var tableName = match.Groups["table"].Value;
            return tableName;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Properties<string>().Configure(x => x.IsUnicode());
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>(); // 默认级联删除
        }

        public void ReCreate()
        {
            Database.ExecuteSqlCommand(
                string.Format("DELETE FROM {0}", TableName<MBTable>()));
            Database.ExecuteSqlCommand(
                string.Format("DELETE FROM {0}", TableName<MBColumn>()));
            Database.ExecuteSqlCommand(
                string.Format("DELETE FROM {0}", TableName<MBProperty>()));
        }

        /// <summary>
        /// 为目标服务器注入 ____table ____column ____property，记录表格的所有信息
        /// </summary>
        public void Rebuild(
            string[] tableExcludeContains = null,
            string[] tableExcludePrefices = null,
            string[] columnExcludes = null,
            Func<string, bool> doneToConfirmContinue = null)
        {
            Database.ExecuteSqlCommand(
                string.Format("DELETE FROM {0}", TableName<MBTable>()));
            Database.ExecuteSqlCommand(
                string.Format("DELETE FROM {0}", TableName<MBColumn>()));
            Database.ExecuteSqlCommand(
                string.Format("DELETE FROM {0}", TableName<MBProperty>()));

            Database.Connection.Open();
            DataTable meta = Database.Connection.GetSchema();
            var alCollections = new ArrayList();
            foreach (DataRowView rv in new DataView(meta)) alCollections.Add(rv[0].ToString());
            var tbSchema = Database.Connection.GetSchema(SCHEMA_TABLES);
            var dvT = new DataView(tbSchema);
            var cSchema = Database.Connection.GetSchema(SCHEMA_COLUMNS);
            var dvC = new DataView(cSchema);
            Database.Connection.Close();

            dvT.Sort = "TABLE_TYPE ASC, TABLE_NAME ASC";
            for (int i = 0; i < dvT.Count; i++)
            {
                var name = dvT[i]["TABLE_NAME"].ToString();
                if (tableExcludePrefices != null &&
                    tableExcludePrefices.Any(e => name.ToLower().StartsWith(e.ToLower()))) continue;
                if (tableExcludeContains != null &&
                    tableExcludeContains.Any(e => name.ToLower().Contains(e.ToLower()))) continue;

                var tableId = Guid.NewGuid();
                var mTable = new MBTable()
                {
                    TableId = tableId,
                    Catalog = dvT[i]["TABLE_CATALOG"].ToString(),
                    Scheme = dvT[i]["TABLE_SCHEMA"].ToString(),
                    Name = name,
                    Type = dvT[i]["TABLE_TYPE"].ToString()
                };

                Tables.Add(mTable);

                _Resolver.ResolveTableAttributes(dvT[i], mTable);
                _Resolver.ResolveTablePrimaryKey(Database, Properties, mTable);
                _Resolver.ResolveTableCaption(Database, Properties, mTable);

                dvC.Sort = "TABLE_NAME ASC, COLUMN_NAME ASC";
                dvC.RowFilter = string.Format(
                    "TABLE_CATALOG = '{0}' AND TABLE_NAME = '{1}'", mTable.Catalog, mTable.Name);
                for (int j = 0; j < dvC.Count; j++)
                {
                    var cName = dvC[j]["COLUMN_NAME"].ToString();
                    if (columnExcludes != null && columnExcludes.Any(e => name.ToLower().StartsWith(e.ToLower()))) continue;
                    var mColumn = new MBColumn()
                    {
                        TableId = tableId,
                        Name = cName,
                        Ordinal = int.Parse(dvC[j]["ORDINAL_POSITION"].ToString()),
                        Nullable = (dvC[j]["IS_NULLABLE"].ToString() == "YES"),
                        Type = dvC[j]["DATA_TYPE"].ToString(),
                        CharsetName = dvC[j]["CHARACTER_SET_NAME"].ToString()
                    };

                    Columns.Add(mColumn);

                    _Resolver.ResolveColumnAttributes(dvC[j], mColumn);
                    _Resolver.ResolveColumnCaption(Database, Properties, mTable, mColumn);

                }

                if (doneToConfirmContinue != null) doneToConfirmContinue(mTable.Name);
            }

            if (doneToConfirmContinue != null) doneToConfirmContinue("保存中，请稍等...");
            SaveChanges();
            if (doneToConfirmContinue != null) doneToConfirmContinue("完成！");

        }

        public SqlServerEFContext(DbConnection connection, PropertyResolver resolver)
            : base(connection, true) { _Resolver = resolver; }

    }

}

