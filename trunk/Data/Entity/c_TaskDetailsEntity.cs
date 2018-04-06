using System.Collections.Generic;
using System.Data.Common;
using Dapper;
namespace SimpleCrawler
{
   
    
	///<summary>
    /// c_TaskDetailsInfo实体控制类
    ///</summary>
	public partial class c_TaskDetails
	{
        private DbConnection _connection;
		#region Get
		///<param name="id">主键.</param>	
		/// <returns>c_TaskDetailsInfo</returns>
		 public  c_TaskDetailsInfo Get(int id)
        {
            using (_connection = Utilities.GetOpenConnection())
            {
                return _connection.Get<c_TaskDetailsInfo>(id);
            }
            return null;
            
		}	
		#endregion
        #region Insert
        /// <summary>
        /// ?c_TaskDetailsInfo对象插入数据库
        /// </summary>
        /// <param name="entity">c_TaskDetailsInfo实体.</param>
        public  int Insert(c_TaskDetailsInfo entity)
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
        /// ?c_TaskDetailsInfo更新到数据库
        /// </summary>
        /// <param name="entity">c_TaskDetailsInfo ??.</param>
        /// <returns>????.</returns>
        public void Update(c_TaskDetailsInfo entity)
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
                _connection.Delete<c_TaskDetailsInfo>(id);
            }
        }
        #endregion
        
		#region List
        /// <summary>
        ///  c_TaskDetailsInfo集合
        /// </summary>
        /// <returns> c_TaskDetailsInfo??.</returns>
        public  IEnumerable<c_TaskDetailsInfo>  List()
        {
          using (_connection = Utilities.GetOpenConnection())
            {
                return _connection.GetList<c_TaskDetailsInfo>();
            }
            return null;
            
        }
          /// <summary>
        ///  c_TaskDetailsInfo集合
        /// </summary>
        /// <returns> c_TaskDetailsInfo</returns>
        public IEnumerable<c_TaskDetailsInfo> ListWhere(string where)
        {
            using (_connection = Utilities.GetOpenConnection())
            {
                return _connection.GetList<c_TaskDetailsInfo>(where);
            }
            return null;

        }
        
        /// <summary>
        /// c_TaskDetailsInfo集合，包含完整的SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IEnumerable<c_TaskDetailsInfo> ListQuery(string sql)
        {
            using (_connection = Utilities.GetOpenConnection())
            {
                return _connection.Query<c_TaskDetailsInfo>(sql);
            }
            return null;
        }
         
		#endregion
		#region Paged
        /// <summary>
        /// 根据分页获得c_TaskDetailsInfo集合
        /// </summary>
        /// <returns>c_TaskDetailsInfo??.</returns>
        public  IEnumerable<c_TaskDetailsInfo> ListPaged(PagerDto dto)
        {
            using (_connection = Utilities.GetOpenConnection())
            {
               return _connection.GetListPaged<c_TaskDetailsInfo>(dto);
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
                return _connection.RecordCount<c_TaskDetailsInfo>(where);
            }
            return 0;
        }
        
       #endregion
        
	}
}

