//====================================================================
// 文件： c_TaskDetails.cs
// 创建时间：2017/11/8
// ===================================================================
using System;
namespace SimpleCrawler
{
    /// <summary>
	/// 
	/// </summary>
	public partial class c_TaskDetailsDto:BaseDto     
    {

        /// <summary>
		/// 
		/// </summary>
		public Int32 Id { get; set; }
        /// <summary>
		/// 
		/// </summary>
		public Int32 TaskId { get; set; }
        /// <summary>
		/// 
		/// </summary>
		public String CrawUrl { get; set; }
        /// <summary>
		/// 
		/// </summary>
		public String Html { get; set; }
        /// <summary>
		/// 
		/// </summary>
		public DateTime CreationTime { get; set; }
      
	}
}