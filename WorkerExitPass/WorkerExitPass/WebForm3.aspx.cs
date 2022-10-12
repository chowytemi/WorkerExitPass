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
        //string empID = "PXE6563";
        string empID = "MB638";

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
            string statussql = "select exitID, createddate, exittime, approve from exitapproval where createdby = '" + empID + "' order by createddate desc;";
            SqlDataAdapter da = new SqlDataAdapter(statussql, conn);
            using (DataTable dt = new DataTable())
            {
                da.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();

            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DateTime date1 = Convert.ToDateTime(e.Row.Cells[1].Text);
                e.Row.Cells[1].Text = date1.ToString("dd/MM/yyyy");

                if ((e.Row.Cells[2].Text) == "&nbsp;")
                {
                    
                    e.Row.Cells[2].Text = "NULL";

                }
                else
                {
                    DateTime time1 = Convert.ToDateTime(e.Row.Cells[2].Text);
                    e.Row.Cells[2].Text = time1.ToString("hh:mm tt");
                }
                

                if ((e.Row.Cells[3].Text) == "True")
                {
                    e.Row.Cells[3].Text = "Approved";

                } else if ((e.Row.Cells[3].Text) == "&nbsp;") {

                    e.Row.Cells[3].Text = "Pending";

                } else {
                    e.Row.Cells[3].Text = "Rejected";
                }

                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GridView1, "Select$" + e.Row.RowIndex);
                e.Row.ToolTip = "Click to select this row.";

            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            FormStatus();
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();

            try
            {
                int exitID = Convert.ToInt32(GridView1.SelectedRow.Cells[0].Text);
                //string status = GridView1.SelectedRow.Cells[3].Text;

                string sql = "select exitapproval.approve, (select EmpList.Employee_Name from exitapproval, EmpList where exitapproval.approver = EmpList.EmpID and exitapproval.exitID = '" + exitID + "') AS 'approver', exitapproval.approveddate, exitapproval.createddate, exitapproval.exittime, exitapproval.projectdesc, EmpList.Employee_Name, exitapproval.company, exitapproval.reason, exitapproval.remarks from exitapproval, EmpList where exitapproval.createdby = EmpList.EmpID and exitapproval.exitID = '" + exitID + "';";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                DataTable dt = ds.Tables[0];

                lblexitID.Text = "Early Exit Permit ID #" + exitID + " Details";                

                if (string.IsNullOrEmpty(dt.Rows[0]["approve"].ToString()))
                {
                    lblStatus.Text = "Pending";     

                }
                else if (dt.Rows[0][0].ToString() == "True")
                {
                    lblStatus.Text = "Approved";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblStatus.Text = "Rejected";
                    lblStatus.ForeColor = System.Drawing.Color.Red;

                }

                if (!string.IsNullOrEmpty(dt.Rows[0]["approveddate"].ToString()) && !string.IsNullOrEmpty(dt.Rows[0]["approver"].ToString()))
                {
                    DateTime when = Convert.ToDateTime(dt.Rows[0]["approveddate"]);
                    lblWhen.Text = when.ToString("dd/MM/yyyy hh:mm tt");
                    lblApprover.Text = dt.Rows[0]["approver"].ToString();
                }
                else
                {
                    lblWhen.Text = "NULL";
                    lblApprover.Text = "NULL";
                }

                DateTime date = Convert.ToDateTime(dt.Rows[0]["createddate"]);
                DateTime time = Convert.ToDateTime(dt.Rows[0]["exittime"]);

                tbDate.Text = date.ToString("dd/MM/yyyy");
                tbTime.Text = time.ToString("hh:mm tt");
                tbProject.Text = dt.Rows[0]["projectdesc"].ToString();
                tbName.Text = dt.Rows[0]["Employee_Name"].ToString();
                tbCompany.Text = dt.Rows[0]["company"].ToString();
                tbReason.Text = dt.Rows[0]["reason"].ToString();

                if (dt.Rows[0]["remarks"].ToString() == "")
                {
                    tbRemarks.Text = "N.A";
                    lblRemarks.Attributes.Add("class", "hide");
                    tbRemarks.Attributes.Add("class", "hide");
                }
                else
                {
                    lblRemarks.Attributes.Add("class", "label");
                    tbRemarks.Attributes.Add("class", "textbox");
                    tbRemarks.Text = dt.Rows[0]["remarks"].ToString();
                }
               
                mpePopUp.Show();
                conn.Close();

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}