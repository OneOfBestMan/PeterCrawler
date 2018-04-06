using System.Collections.Generic;
using System.Data.Common;
using Dapper;


namespace SimpleCrawler
{
    public class BaseEntity
    {
        private DbConnection _connection;
        /// <summary>
        /// 执行SQL，返回影响的行数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql)
        {
            int result = 0;
            using (_connection = Utilities.GetOpenConnection())
            {
                result = _connection.Execute(sql);
            }
            return result;
        }
    }
}
