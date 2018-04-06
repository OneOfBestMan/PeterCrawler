//====================================================================
// 文件： c_TaskDetailsInfo.cs
// 创建时间：2017/11/8
// ===================================================================
using System;
using Dapper;
namespace SimpleCrawler
{
    /// <summary>
	/// 
	/// </summary>
	[Table("c_TaskDetails")]
	public partial class c_TaskDetailsInfo
    {

        /// <summary>
		/// 
		/// </summary>
        [Key]
		public  int Id { get; set; }
        
        /// <summary>
		/// 
		/// </summary>
		public  int TaskId { get; set; }
        
        public string Title { get; set; }

        /// <summary>
		/// 
		/// </summary>
		public  String CrawUrl { get; set; }
        
        public int Depth { get; set; }
        /// <summary>
        /// 原始html全文
        /// </summary>
        public  String RawHtml { get; set; }

        public String SelectHtml { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public  DateTime CreationTime { get; set; }
        
      
	}
}
