using System.Collections.Generic;
using System.Data.Common;
using Dapper;
namespace SimpleCrawler
{
   
    
	///<summary>
    /// c_TasksInfo实体控制类
    ///</summary>
	public partial class c_Tasks
	{
        private DbConnection _connection;
		#region Get
		///<param name="id">主键.</param>	
		/// <returns>c_TasksInfo</returns>
		 public  c_TasksInfo Get(int id)
        {
            using (_connection = Utilities.GetOpenConnection())
            {
                return _connection.Get<c_TasksInfo>(id);
            }
            return null;
            
		}	
		#endregion
        #region Insert
        /// <summary>
        /// ?c_TasksInfo对象插入数据库
        /// </summary>
        /// <param name="entity">c_TasksInfo实体.</param>
        public  int Insert(c_TasksInfo entity)
        {
            int result = 0;
            using (_connection = Utilities.GetOpenConnection())
            {
              result = _connection.Insert(entity).Value;
            }
             return result;
        }

        #endregion
	    #region Update

        /// <summary>
        /// ?c_TasksInfo更新到数据库
        /// </summary>
        /// <param name="entity">c_TasksInfo ??.</param>
        /// <returns>????.</returns>
        public void Update(c_TasksInfo entity)
        {
             using (_connection = Utilities.GetOpenConnection())
            {
                _connection.Update(entity);
            }
        }

        #endregion
		#region Delete
        /// <summary>
        /// 删除数据库记录
        /// </summary>
        public  void Delete(int id)
        {
            using (_connection = Utilities.GetOpenConnection())
            {
                _connection.Delete<c_TasksInfo>(id);
            }
        }
        #endregion
        
		#region List
        /// <summary>
        ///  c_TasksInfo集合
        /// </summary>
        /// <returns> c_TasksInfo??.</returns>
        public  IEnumerable<c_TasksInfo>  List()
        {
          using (_connection = Utilities.GetOpenConnection())
            {
                return _connection.GetList<c_TasksInfo>();
            }
            return null;
            
        }
          /// <summary>
        ///  c_TasksInfo集合
        /// </summary>
        /// <returns> c_TasksInfo</returns>
        public IEnumerable<c_TasksInfo> ListWhere(string where)
        {
            using (_connection = Utilities.GetOpenConnection())
            {
                return _connection.GetList<c_TasksInfo>(where);
            }
            return null;

        }
        
        /// <summary>
        /// c_TasksInfo集合，包含完整的SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IEnumerable<c_TasksInfo> ListQuery(string sql)
        {
            using (_connection = Utilities.GetOpenConnection())
            {
                return _connection.Query<c_TasksInfo>(sql);
            }
            return null;
        }
         
		#endregion
		#region Paged
        /// <summary>
        /// 根据分页获得c_TasksInfo集合
        /// </summary>
        /// <returns>c_TasksInfo??.</returns>
        public  IEnumerable<c_TasksInfo> ListPaged(PagerDto dto)
        {
            using (_connection = Utilities.GetOpenConnection())
            {
               return _connection.GetListPaged<c_TasksInfo>(dto);
            }
            return null;
        }

        #endregion	
       
       #region RecordCount
        /// <summary>
        /// 根据条件获取行数
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public int RecordCount(string where)
        {
            using (_connection = Utilities.GetOpenConnection())
            {
                return _connection.RecordCount<c_TasksInfo>(where);
            }
            return 0;
        }
        
       #endregion
        
	}
}

