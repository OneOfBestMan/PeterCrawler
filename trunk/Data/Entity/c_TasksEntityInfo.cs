//====================================================================
// 文件： c_TasksInfo.cs
// 创建时间：2017/11/8
// ===================================================================
using System;
using Dapper;
namespace SimpleCrawler
{
    /// <summary>
	/// 
	/// </summary>
	[Table("c_Tasks")]
	public partial class c_TasksInfo
    {

        /// <summary>
		/// 
		/// </summary>
        [Key]
		public  int Id { get; set; }
        
        /// <summary>
		/// 
		/// </summary>
		public  String TaskName { get; set; }
        
        /// <summary>
		/// 
		/// </summary>
		public  String BaseUrl { get; set; }
        
        /// <summary>
		/// 
		/// </summary>
		public  DateTime CreationTime { get; set; }
        
      
	}
}
