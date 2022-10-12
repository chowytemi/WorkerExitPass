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
    public partial class WebForm5 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GetPending();
            GetApplicationById();
        }

        private DataTable GetPending()
        {
            DataTable dt = new DataTable();
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            string statussql = "select exitID, createddate, exittime, reason, approve from exitapproval where approve IS NULL AND reason NOT IN('Medical Injury') order by exitID desc;";
            using (SqlConnection conn = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand(statussql))
                {
                    cmd.Connection = conn;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                    }
                }
            }

            return dt;
        }

        protected void GetApplicationById()
        {
            try
            {

                var exitID = Request.QueryString["exitid"];

                //Connect to database
                string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
                SqlConnection conn = new SqlConnection(cs);
                conn.Open();

                string statussql = "select exitapproval.createddate, exitapproval.exittime, exitapproval.projectdesc, EmpList.Employee_Name, exitapproval.company, exitapproval.reason, exitapproval.remarks from exitapproval, EmpList where exitapproval.createdby = EmpList.EmpID and exitapproval.exitID = '" + exitID + "';";
                SqlDataAdapter da = new SqlDataAdapter(statussql, conn);

                DataSet ds = new DataSet();
                da.Fill(ds);
                DataTable dt = ds.Tables[0];

                DateTime date = Convert.ToDateTime(dt.Rows[0]["createddate"]);

                DateTime time = Convert.ToDateTime(dt.Rows[0]["exittime"]);

                //Binding TextBox From dataTable    
                lblexitID.Text = "Early Exit Permit ID #" + exitID + " Details";
                tbDate.Text = date.ToString("dd/MM/yyyy");
                tbTime.Text = time.ToString("hh:mm tt");
                tbProject.Text = dt.Rows[0]["projectdesc"].ToString();
                tbName.Text = dt.Rows[0]["Employee_Name"].ToString();
                tbCompany.Text = dt.Rows[0]["company"].ToString();
                tbReason.Text = dt.Rows[0]["reason"].ToString();

                if (dt.Rows[0]["remarks"].ToString() == "")
                {
                    tbRemarks.Text = "N.A";
                }
                else
                {
                    tbRemarks.Text = dt.Rows[0]["remarks"].ToString();
                }


                mpeApproval.Show();

            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void ApproveBtn_Click(object sender, EventArgs e)
        {

            string approverID = "T202";
            DateTime approveddate = DateTime.Now;
            int exitID = Convert.ToInt32(GridView1.SelectedRow.Cells[0].Text);
            //int exitID = 12;
            int approve = 1;

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string sqlquery = "update exitapproval set approver = '" + approverID + "', approve = " + approve + ", approveddate = '" + approveddate + "' where exitID = '" + exitID + "'";

            using (SqlCommand update = new SqlCommand(sqlquery, conn))
            {
                update.ExecuteNonQuery();

                conn.Close();
            }

            Response.Redirect(Request.RawUrl);
        }
    }
}