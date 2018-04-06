using SimpleCrawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CrawlerWeb
{
    public partial class Detail : System.Web.UI.Page
    {
        protected string Html;
        protected int DetailId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DetailId = int.Parse(Request.QueryString["id"].ToString());
                if (DetailId > 0)
                {
                    c_TaskDetails detail = new c_TaskDetails();
                    var tempDetail = detail.Get(DetailId);
                    Html = tempDetail.SelectHtml;
                }


            }
        }
    }
}