//====================================================================
// 文件： c_Tasks.cs
// 创建时间：2017/11/8
// ===================================================================
using System;
namespace SimpleCrawler
{
    /// <summary>
	/// 
	/// </summary>
	public partial class c_TasksDto:BaseDto     
    {

        /// <summary>
		/// 
		/// </summary>
		public Int32 Id { get; set; }
        /// <summary>
		/// 
		/// </summary>
		public String TaskName { get; set; }
        /// <summary>
		/// 
		/// </summary>
		public String BaseUrl { get; set; }
        /// <summary>
		/// 
		/// </summary>
		public DateTime CreationTime { get; set; }
      
	}
}