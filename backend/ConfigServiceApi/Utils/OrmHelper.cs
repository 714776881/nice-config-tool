using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Data.Odbc;
using ConfigServiceApi.Services;
using MySql.Data.MySqlClient;

namespace ConfigServiceApi.Utils
{
    public enum DBType
    {
        ORACLE,
        SQLSERVER,
        MYSQL,
        ODBC,
    }
    public class OrmHelper
    {
        private readonly IDbConnection _connection;
        public OrmHelper(DBType dbType, string connString)
        {
            _connection = DbConnectionFactory.Produce(dbType, connString);
        }

        public OrmHelper() 
        {
            var dbType = DBType.ORACLE;
            var connectString = RisConfigService.GetDbConnection(out dbType);
            _connection = DbConnectionFactory.Produce(dbType, connectString);
        }

        public int Execute(string sql, object param = null)
        {
            return _connection.Execute(sql, param);
        }
        public bool ExecuteMultipleSql(IEnumerable<string> sqlStatements)
        {
            using (IDbConnection dbConnection = _connection)
            {
                dbConnection.Open();

                using (var transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        foreach (var sql in sqlStatements)
                        {
                            dbConnection.Execute(sql, transaction: transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        return false;
                        transaction.Rollback();
                        throw new Exception("Error executing multiple SQL statements", ex);
                    }
                }
            }
            return true;
        }
        public T QueryFirst<T>(string sql, object param = null)
        {
            return _connection.QueryFirstOrDefault<T>(sql, param);
        }

        public IEnumerable<T> Query<T>(string sql, object param = null)
        {
            return _connection.Query<T>(sql, param);
        }

        public int Insert<T>(T entity, string tableName = "", string key = "")
        {
            if (tableName == "") tableName = typeof(T).Name;
            var sql = $"INSERT INTO {tableName} ({string.Join(",", GetProperties<T>(key).Select(p => p.Name))}) VALUES (:{string.Join(", :", GetProperties<T>().Select(p => p.Name))})";
            return _connection.Execute(sql, entity);
        }
        public int Update<T>(T entity, string tableName = "", string key = "Id")
        {
            if (tableName == "") tableName = typeof(T).Name;
            var sql = $"UPDATE {tableName} SET {string.Join(", ", GetProperties<T>().Select(p => $"{p.Name} = :{p.Name}"))} WHERE {key} = :{key}";
            return _connection.Execute(sql, entity);
        }
        public int Delete<T>(int id, string tableName = "", string key = "Id")
        {
            if (tableName == "") tableName = typeof(T).Name;
            var sql = $"DELETE FROM {tableName} WHERE {key} = :Id";
            return _connection.Execute(sql, new { Id = key });
        }
        private static IEnumerable<System.Reflection.PropertyInfo> GetProperties<T>(string key = "Id")
        {
            return typeof(T).GetProperties().Where(p => p.Name != key);
        }
        public IEnumerable<T> ExecuteStoredProcedure<T>(string storedProcedureName, object param = null)
        {
            return _connection.Query<T>(storedProcedureName, param, commandType: CommandType.StoredProcedure);
        }

        // 分页查询
        public async Task<PagedResult<T>> QueryPagedAsync<T>(
            string dataSql,
            string countSql,
            object parameters = null,
            CommandType commandType = CommandType.Text)
        {
            _connection.Open();
            using (var multi = await _connection.QueryMultipleAsync(
                $"{dataSql}; {countSql}",
                parameters,
                commandType: commandType))
            {
                var items = await multi.ReadAsync<T>();
                var total = await multi.ReadSingleAsync<int>();
                return new PagedResult<T>(items, total);
            }
        }
    }



public class DbConnectionFactory
    {
        public static IDbConnection Produce(DBType dbType, string connString)
        {
            if (dbType == DBType.ORACLE)
            {
                return new OracleConnection(connString);
            }
            //if (dbType == DBType.SQLSERVER)
            //{
            //    return new SqlConnection(connString);
            //}
            else if (dbType == DBType.MYSQL)
            {
                return new MySqlConnection(connString);
            }
            //else if (dbType == DBType.ORACLE)
            //{
            //    return new OracleConnection(connString);
            //}
            else if(dbType == DBType.ODBC)
            {
                // 连接人大京仓数据库
                return new OdbcConnection(connString);
            }
            else
            {
                throw new ArgumentException($"Unsupported database type: {dbType}");
            }
        }
    }
    // 分页结果对象
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; }
        public int TotalCount { get; }

        public PagedResult(IEnumerable<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
    }

}


