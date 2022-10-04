using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WorkerExitPass
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        //Get login id
        string empID = "PXE6563";
        //string empID = "T202";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FormStatus();
            }
        }
        private void FormStatus()
        {

            //Connect to database
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string statussql = "select createddate, exittime, approve from exitapproval where createdby = '" + empID + "' order by createddate desc;";
            SqlDataAdapter da = new SqlDataAdapter(statussql, conn);
            using (DataTable dt = new DataTable())
            {
                da.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();

                //if approve == null, pending

            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string setColorClass = string.Empty;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DateTime date1 = Convert.ToDateTime(e.Row.Cells[0].Text);
                e.Row.Cells[0].Text = date1.ToString("dd/MM/yyyy");

                if ((e.Row.Cells[1].Text) == "&nbsp;")
                {
                    
                    e.Row.Cells[1].Text = "NULL";

                }
                else
                {
                    DateTime time1 = Convert.ToDateTime(e.Row.Cells[1].Text);
                    e.Row.Cells[1].Text = time1.ToString("hh:mm tt");
                }
                

                if ((e.Row.Cells[2].Text) == "True")
                {
                    e.Row.Cells[2].Text = "Approved";
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.Green;


                } else if ((e.Row.Cells[2].Text) == "&nbsp;") {

                    e.Row.Cells[2].Text = "Pending";

                } else {

                    e.Row.Cells[2].Text = "Rejected";
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.Red;

                }
             
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            FormStatus();
        }
    }
}