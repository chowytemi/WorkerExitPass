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
        //string empID = "M988";
        protected void Page_Load(object sender, EventArgs e)
        {
            //GetPending();
            //GetApplicationById();
            if (!IsPostBack)
            {
                if ((Request.QueryString["approval"] != null))
                {
                    var exitID = Request.QueryString["exitid"];
                    string myempno = Request.QueryString["approval"];
                    Session["empID"] = myempno;
                    
                }
                CheckAccess();
            }
            

        }

        protected void CheckAccess()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            string PJM = ConfigurationManager.AppSettings["PJM"].ToString();
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id ='" + PJM + "' and EmpList.EmpID = '" + empID + "' ; ";
            //string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = 85 and ((IDNo like CONCAT('" + firstId + "', '%')) and (IDNo like CONCAT('%', '" + lastFiveId + "')));";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                IsApprove();
            }
            else
            {
                Response.Redirect("http://eservices.dyna-mac.com/error");
            }

            dr.Close();
            con.Close();

        }

        protected void IsApprove()
        {
            var exitID = Request.QueryString["exitid"];
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string sqlquery = "select approve from exitapproval where exitID = '" + exitID + "';";
            SqlCommand cmdlineno = new SqlCommand(sqlquery, conn);
            SqlDataReader dr = cmdlineno.ExecuteReader();
            while (dr.Read())
            {
                if (string.IsNullOrEmpty(dr[0].ToString()))
                {
                    GetPending();
                    GetApplicationById();
                }
                else
                {
                    Response.Redirect("http://eservices.dyna-mac.com/error");
                }

            }
            dr.Close();
            conn.Close();
        }

        private DataTable GetPending()
        {
            DataTable dt = new DataTable();
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            string statussql = "select distinct exitID, createddate, exittime, reason, approve from exitapproval where approve IS NULL AND reason NOT IN('Medical Injury') order by exitID desc;";
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

            }
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

                //string statussql = "select exitapproval.createddate, exitapproval.exittime, exitapproval.projectdesc, EmpList.Employee_Name, exitapproval.company, exitapproval.reason, exitapproval.remarks from exitapproval, EmpList where exitapproval.createdby = EmpList.EmpID and exitapproval.exitID = '" + exitID + "';";
                string statussql = "select distinct createddate, exittime, projectdesc, company, reason, remarks from exitapproval where exitID = '" + exitID +"';";
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
                //tbName.Text = dt.Rows[0]["Employee_Name"].ToString();
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

                string sql2 = "select EmpList.Employee_Name from EmpList, exitapproval where exitapproval.toexit = EmpList.EmpID and exitapproval.exitID = '" + exitID + "';";
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, conn);

                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
                DataTable dt2 = ds2.Tables[0];

                string empName = "";
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    empName += dt2.Rows[i]["Employee_Name"] + ",";
                }
                empName = empName.TrimEnd(',');
                tbName.Text = empName;

                mpeApproval.Show();

            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void ApproveBtn_Click(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            //string approverID = "T203";
            DateTime approveddate = DateTime.Now;
            var exitID = Request.QueryString["exitid"];
            //int exitID = Convert.ToInt32(GridView1.SelectedRow.Cells[0].Text);
            int approve = 1;

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string sqlquery = "update exitapproval set approver = '" + empID + "', approve = " + approve + ", approveddate = '" + approveddate + "' where exitID = '" + exitID + "'";

            using (SqlCommand update = new SqlCommand(sqlquery, conn))
            {
                update.ExecuteNonQuery();

                conn.Close();
            }

            mpeApproval.Hide();
            Response.Redirect("WebForm4.aspx?approval=" + empID);
        }

        protected void RejectBtn_Click(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            //string approverID = "T203";
            DateTime approveddate = DateTime.Now;
            var exitID = Request.QueryString["exitid"];
            //int exitID = Convert.ToInt32(GridView1.SelectedRow.Cells[0].Text);
            int approve = 0;

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string sqlquery = "update exitapproval set approver = '" + empID + "', approve = " + approve + ", approveddate = '" + approveddate + "' where exitID = '" + exitID + "'";

            using (SqlCommand update = new SqlCommand(sqlquery, conn))
            {
                update.ExecuteNonQuery();

                conn.Close();
            }

            mpeApproval.Hide();
            Response.Redirect("WebForm4.aspx?approval=" + empID);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            Response.Redirect("WebForm4.aspx?approval=" + empID);

        }
    }
}