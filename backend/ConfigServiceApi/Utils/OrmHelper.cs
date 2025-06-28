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
using System.Reflection;
using Npgsql;

namespace ConfigServiceApi.Utils
{
    public enum DBType
    {
        ORACLE,
        SQLSERVER,
        MYSQL,
        NPGSQL,
        ODBC,
    }
    public class OrmHelper
    {
        private readonly IDbConnection _connection;
        private readonly DBType _dbType =  DBType.ORACLE;

        private bool IsPositionalParam => _dbType == DBType.ODBC;

        private string PRETAG = ":";


        public DBType DBType
        {
            get
            {
                return _dbType;
            }
        }

        public OrmHelper() 
        {
            var connectString = RisConfigService.GetDbConnection(out _dbType);
            _connection = DbConnectionFactory.Produce(_dbType, connectString);


            // 设置参数前缀
            switch (_dbType)
            {
                case DBType.ORACLE:
                case DBType.NPGSQL:
                    PRETAG = ":";
                    break;
                case DBType.ODBC: // 人大金仓使用问号参数
                    PRETAG = "?";
                    break;
                default:
                    PRETAG = "@";
                    break;
            }
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
            var properties = GetOrderedProperties<T>(key).ToList();

            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = IsPositionalParam
                ? string.Join(", ", properties.Select(_ => "?"))
                : string.Join(", ", properties.Select(p => PRETAG + p.Name));

            var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

            if (IsPositionalParam)
            {
                var parameters = properties.ToDictionary(p => p.Name, p => p.GetValue(entity));
                return _connection.Execute(sql, parameters);
            }

            return _connection.Execute(sql, entity);
        }


        public int Update<T>(T entity, string tableName = "", string key = "Id")
        {
            if (tableName == "") tableName = typeof(T).Name;

            var allProperties = GetOrderedProperties<T>().ToList();
            var keyProperty = allProperties.FirstOrDefault(p => p.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (keyProperty == null) throw new ArgumentException("Key property not found");

            var setProperties = allProperties.Where(p => !p.Name.Equals(key, StringComparison.OrdinalIgnoreCase)).ToList();

            // 构建SQL语句
            var setClause = IsPositionalParam
                ? string.Join(", ", setProperties.Select(p => $"{p.Name} = ?"))
                : string.Join(", ", setProperties.Select(p => $"{p.Name} = {PRETAG}{p.Name}"));

            var sql = $"UPDATE {tableName} SET {setClause} WHERE {key} = {(IsPositionalParam ? "?" : PRETAG + key)}";

            // 构建参数
            if (IsPositionalParam)
            {
                var parameters = setProperties.ToDictionary(p => p.Name, p => p.GetValue(entity));
                parameters.Add(keyProperty.Name, keyProperty.GetValue(entity));
                return _connection.Execute(sql, parameters);
            }

            return _connection.Execute(sql, entity);
        }



        public int Delete<T>(int id, string tableName = "", string key = "Id")
        {
            if (tableName == "") tableName = typeof(T).Name;

            var sql = IsPositionalParam
                ? $"DELETE FROM {tableName} WHERE {key} = ?"
                : $"DELETE FROM {tableName} WHERE {key} = {PRETAG}Id";

            if (IsPositionalParam)
            {
                var parameters = new Dictionary<string,string>();
                parameters.Add("Id", id.ToString());
                return _connection.Execute(sql, parameters);
            }
            else
            {
                return _connection.Execute(sql, new { Id = id });
            }
        }

        private static IEnumerable<System.Reflection.PropertyInfo> GetProperties<T>(string key = "Id")
        {
            return typeof(T).GetProperties().Where(p => p.Name != key);
        }

        // 获取有序的属性列表（确保参数顺序一致）
        private IEnumerable<PropertyInfo> GetOrderedProperties<T>(string excludeKey = "")
        {
            return typeof(T).GetProperties()
                .Where(p => !p.Name.Equals(excludeKey, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Name); // 按属性名称排序保证顺序一致
        }

        public IEnumerable<T> ExecuteStoredProcedure<T>(string storedProcedureName, object param = null)
        {
            return _connection.Query<T>(storedProcedureName, param, commandType: CommandType.StoredProcedure);
        }


        // 分页查询
        public async Task<PagedResult<T>> QueryPagedAsync<T>(
            string dataSql,
            int pageNO,
            int pageSize,
            object parameters = null,
            CommandType commandType = CommandType.Text)
        {
            var offset = (pageNO - 1) * pageSize;

            string countSql = $"select count(*) from ({dataSql})";


            switch (_dbType)
            {
                case DBType.ORACLE:
                    dataSql = $"select * from ({dataSql}) offset {offset} rows fetch next {pageSize} rows only";
                    break;
                case DBType.NPGSQL:
                    dataSql = $"select * from ({dataSql}) limit {pageSize} offset {offset}";
                    break;
                default:
                    throw new NotImplementedException("未识别的数据库类型");
            }

            _connection.Open();
            using (var multi = await _connection.QueryMultipleAsync(
                $"{dataSql}; {countSql}",
                parameters,
                commandType: commandType))
            {
                var items = (await multi.ReadAsync<T>()).ToList();
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
            //else if (dbType == DBType.MYSQL)
            //{
            //    return new MySqlConnection(connString);
            //}
            //else if (dbType == DBType.ORACLE)
            //{
            //    return new OracleConnection(connString);
            //}
            else if(dbType == DBType.ODBC)
            {
                // 连接人大京仓数据库
                return new OdbcConnection(connString);
            }
            else if(dbType == DBType.NPGSQL)
            {
                return new NpgsqlConnection(connString);
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


