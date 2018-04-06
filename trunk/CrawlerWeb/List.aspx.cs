using SimpleCrawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CrawlerWeb
{
    public partial class List : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Bind();
                BindGrid();
            }

        }

        private void Bind()
        {
            c_Tasks task = new c_Tasks();
            ddlTask.DataTextField = "TaskName";
            ddlTask.DataValueField = "Id";

            ddlTask.DataSource = task.List();
            ddlTask.DataBind();
        }
        private void BindGrid()
        {
            int taskId = int.Parse(ddlTask.SelectedValue);
            c_TaskDetails detail = new c_TaskDetails();
            var list = detail.ListWhere(string.Format(" where TaskId={0}", taskId));
            gvList.DataSource = list;
            gvList.DataBind();
        }

        protected void ddlTask_TextChanged(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}