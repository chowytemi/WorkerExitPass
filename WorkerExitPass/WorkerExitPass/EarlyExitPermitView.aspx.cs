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
    public partial class WebForm4 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {

                if ((Request.QueryString["approval"] != null))
                {

                    string myempno = Request.QueryString["approval"];
                    Session["empID"] = myempno;

                }
                if ((Request.QueryString["exprmtid"] != null))
                {

                    string exitid = Request.QueryString["exprmtid"];
                    Session["exitid"] = exitid;
                }

                CheckAccess();
            }
        }

        protected void CheckAccess()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            //using test access 87, pjm access 83
            string PJM = ConfigurationManager.AppSettings["PJM"].ToString();
            string RO = ConfigurationManager.AppSettings["RO"].ToString();
            //Connect to database
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            //for testing
            string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + PJM + "' and EmpList.EmpID = '" + empID + "' ; ";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                GetPending();
                dr.Close();
            }
            else
            {
                //string sql2 = "select distinct RO from EmpList where RO IS NOT NULL AND RO = '" + empID + "';";
                string sql2 = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + RO + "' and EmpList.EmpID = '" + empID + "' ; ";
                SqlCommand cmd2 = new SqlCommand(sql2, con);
                SqlDataReader dr2 = cmd2.ExecuteReader();
                if (dr2.HasRows)
                {
                    GetPendingRO();
                }
                else
                {
                    Response.Redirect("http://eservices.dyna-mac.com/error");
                }
                dr2.Close();

            }
            con.Close();

        }

        private DataTable GetPending()
        {
            DataTable dt = new DataTable();
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            string statussql = "select distinct exitapproval.exitID, exitapproval.createddate, exitapproval.exittime, exitapproval.reason, exitapproval.approve, EmpList.RO from exitapproval, EmpList where approve IS NULL AND reason NOT IN('Medical Injury') and exitapproval.createdby = EmpList.EmpID AND EmpList.RO IS NULL order by exitID desc;";
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
        private DataTable GetPendingRO()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            DataTable dt = new DataTable();
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            string statussql = "select distinct exitapproval.exitID, exitapproval.createddate, exitapproval.exittime, exitapproval.reason, exitapproval.approve, EmpList.RO from exitapproval,  EmpList where approve IS NULL AND reason NOT IN('Medical Injury') and exitapproval.createdby = EmpList.EmpID AND EmpList.RO IS NOT NULL order by exitID desc;";
            //string statussql = "select distinct exitapproval.exitID, exitapproval.createddate, exitapproval.exittime, exitapproval.reason, exitapproval.approve, EmpList.RO from exitapproval,  EmpList where approve IS NULL AND reason NOT IN('Medical Injury') and exitapproval.createdby = EmpList.EmpID AND EmpList.RO IS NOT NULL AND EmpList.RO = '" + empID + "' order by exitID desc;";
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
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

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

                e.Row.Attributes["onclick"] = $"location.href = 'EarlyExitPermitApproval.aspx?exitid={GridView1.DataKeys[e.Row.RowIndex]["exitID"]}&approval={empID}'";
                e.Row.ToolTip = "Click to select this row.";

            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            GetPending();
            DataTable dt = this.GetPending();
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        
    }
}