using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace APIsASManager.CoreV3.MyClass
{
    public static class DbContextHelper
    {
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                //foreach (var pro in properties)
                //{
                //    if (columnNames.Contains(pro.Name.ToLower()))
                //    {
                //        try
                //        {
                //            pro.SetValue(objT, row[pro.Name]);
                //        }
                //        catch (Exception ex) { }
                //    }
                //}
                Parallel.ForEach(properties, pro =>
                {
                    if (columnNames.Contains(pro.Name.ToLower()))
                    {
                        try
                        {
                            var vValue = Convert.ChangeType(row[pro.Name], pro.PropertyType);
                            pro.SetValue(objT, vValue);
                            //pro.SetValue(objT, row[pro.Name]);
                            return;
                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (Exception ex) { }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                    }
                });
                return objT;
            }).ToList();
        }
        public static bool EFExecQuery(string pSql, DbContext pDbContext)
        {
            bool vRet = false;
            DbConnection vConn = pDbContext.Database.GetDbConnection();
            DbProviderFactory vDbFactory = DbProviderFactories.GetFactory(vConn);
            try
            {
                vConn.Open();
                using (var vCmd = vDbFactory.CreateCommand())
                {
                    vCmd.Connection = vConn;
                    vCmd.CommandType = CommandType.Text;
                    vCmd.CommandText = pSql;
                    if (vCmd.ExecuteNonQuery() >= 0)
                    {
                        vRet = true;
                    }
                }
            }
            catch (Exception exp)
            { }
            return vRet;
        }
        public static DataTable EFExecSQL(string pSql, DbContext pDbContext, List<SqlParameter> pParam = null)
        {
            DataTable vDt = new DataTable();
            DbConnection vConn = pDbContext.Database.GetDbConnection();
            DbProviderFactory vDbFactory = DbProviderFactories.GetFactory(vConn);
            using (var vCmd = vDbFactory.CreateCommand())
            {
                vCmd.Connection = vConn;
                vCmd.CommandType = CommandType.Text;
                vCmd.CommandText = pSql;
                if (pParam != null)
                {
                    foreach (var vItem in pParam)
                    {
                        vCmd.Parameters.Add(vItem);
                    }
                }
                using (DbDataAdapter vDbDataAdapter = vDbFactory.CreateDataAdapter())
                {
                    vDbDataAdapter.SelectCommand = vCmd;
                    vDbDataAdapter.Fill(vDt);
                }
            }
            return vDt;
        }
        public static void AddEntities<T>(List<T> entities, DbContext db) where T : class
        {
            if (entities.Count == 0)
                return;
            using (db)
            {
                var set = db.Set<T>();

                var entityType = db.Model.FindEntityType(typeof(T));
                var primaryKey = entityType.FindPrimaryKey();
                var keyValues = new object[primaryKey.Properties.Count];

                foreach (T e in entities)
                {
                    for (int i = 0; i < keyValues.Length; i++)
                        keyValues[i] = primaryKey.Properties[i].GetGetter().GetClrValue(e);

                    //Debug.WriteLine(keyValues);
                    try
                    {
                        var obj = set.Find(keyValues);

                        if (obj == null)
                        {
                            set.Add(e);
                        }
                        else
                        {
                            db.Entry(obj).CurrentValues.SetValues(e);
                        }
                    }
                    catch (Exception exp)
                    { }
                    //Debug.WriteLine(e);
                    db.SaveChanges();
                }
                //db.SaveChangesAsync();
                //Parallel.ForEach(entities.AsEnumerable(), e =>
                //{
                //    for (int i = 0; i < keyValues.Length; i++)
                //    {
                //        keyValues[i] = primaryKey.Properties[i].GetGetter().GetClrValue(e);
                //    }
                //    var obj = set.Find(keyValues);
                //    if (obj == null)
                //        set.Add(e);
                //    else
                //        db.Entry(obj).CurrentValues.SetValues(e);
                //});
                //db.SaveChanges();
            }
        }

        public static DataTable ExecuteReadQuery(string query, DbContext pDbContext)
        {
            DataTable entries = new DataTable();
            DbConnection vConn = pDbContext.Database.GetDbConnection();
            string vConnStr = vConn.ConnectionString;
            using(SqliteConnection db = new SqliteConnection(vConnStr))
            {
                SqliteCommand selectCommand = new SqliteCommand(query, db);
                try
                {
                    db.Open();
                    SqliteDataReader reader = selectCommand.ExecuteReader();

                    if (reader.HasRows)
                        for (int i = 0; i < reader.FieldCount; i++)
                            entries.Columns.Add(new DataColumn(reader.GetName(i)));

                    int j = 0;
                    while (reader.Read())
                    {
                        DataRow row = entries.NewRow();
                        entries.Rows.Add(row);

                        for (int i = 0; i < reader.FieldCount; i++)
                            entries.Rows[j][i] = (reader.GetValue(i));

                        j++;
                    }

                }
                catch (Exception exp)
                { }
                finally { db.Close(); }
            }
            return entries;
        }
        public static void RefreshContext(DbContext pDbContext)
        {
            pDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        internal static DataTable EFExecSQL(string pSql, DatabaseFacade database)
        {
            //throw new NotImplementedException();
            DataTable vDt = new DataTable();
            DbConnection vConn = database.GetDbConnection();
            DbProviderFactory vDbFactory = DbProviderFactories.GetFactory(vConn);
            using (var vCmd = vDbFactory.CreateCommand())
            {
                vCmd.Connection = vConn;
                vCmd.CommandType = CommandType.Text;
                vCmd.CommandText = pSql;
                using (DbDataAdapter vDbDataAdapter = vDbFactory.CreateDataAdapter())
                {
                    vDbDataAdapter.SelectCommand = vCmd;
                    vDbDataAdapter.Fill(vDt);
                }
            }
            return vDt;
        }
        internal static bool EFExecSQL(string pSql, DatabaseFacade database, ref List<SqlParameter> pParam)
        {
            bool vRet = false;
            DataTable vDt = new DataTable();
            DbConnection vConn = database.GetDbConnection();
            DbProviderFactory vDbFactory = DbProviderFactories.GetFactory(vConn);
            try
            {
                using (var vCmd = vDbFactory.CreateCommand())
                {
                    vCmd.Connection = vConn;
                    vCmd.CommandType = CommandType.Text;
                    vCmd.CommandText = pSql;
                    foreach (var vItem in pParam)
                    {
                        vCmd.Parameters.Add(vItem);
                    }
                    if (vCmd.ExecuteNonQuery() > 0)
                    {
                        vRet = true;
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'exp' is declared but never used
            catch (Exception exp)
#pragma warning restore CS0168 // The variable 'exp' is declared but never used
            { }
            return vRet;
        }
    }
    public static class DbSetExtensions
    {
        public static EntityEntry<TEnt> AddIfNotExists<TEnt, TKey>(this DbSet<TEnt> dbSet, TEnt entity, Func<TEnt, TKey> predicate) where TEnt : class
        {
            var exists = dbSet.Any(c => predicate(entity).Equals(predicate(c)));
            return exists
                ? null
                : dbSet.Add(entity);
        }

        public static void AddRangeIfNotExists<TEnt, TKey>(this DbSet<TEnt> dbSet, IEnumerable<TEnt> entities, Func<TEnt, TKey> predicate) where TEnt : class
        {
            var entitiesExist = from ent in dbSet
                                where entities.Any(add => predicate(ent).Equals(predicate(add)))
                                select ent;

            dbSet.AddRange(entities.ToList().Except(entitiesExist.ToList()));
        }
    }
}
