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
            string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList " +
                "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid " +
                "and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + PJM + "' and EmpList.EmpID = '" + empID + "' ; ";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                GetPending();
                //GetApproved();
                dr.Close();
            }
            else
            {
                string sql2 = "select distinct RO from EmpList where RO IS NOT NULL AND RO = '" + empID + "';";
                //string sql2 = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + RO + "' and EmpList.EmpID = '" + empID + "' ; ";
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

        private DataSet GetPending()
        {           
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;

            string statussql = "select distinct exitapproval.exitID, exitapproval.exittime, exitapproval.company, exitapproval.projectdesc, count(exitapproval.EmpID) as 'no of emp' from exitapproval, EmpList " +
            "where approve IS NULL AND reason NOT IN('Workplace Injury') and exitapproval.createdby = EmpList.EmpID AND(EmpList.RO IS NULL OR EmpList.RO = 'NONE') and DATEADD(hour,1,exittime) > CURRENT_TIMESTAMP " +
            "group by exitapproval.exitID, exitapproval.exittime, exitapproval.reason, exitapproval.company, exitapproval.projectdesc order by exitID desc;";

            using (SqlConnection conn = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand(statussql))
                {
                    cmd.Connection = conn;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        sda.Fill(ds);
                        DataTable dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                                                     
                            GridView1.DataSource = dt;
                            GridView1.DataBind();
                            noPending.Visible = false;

                        } else 
                        {
                            noPending.Text = "No Pending Early Exit Permit";
                            noPending.Visible = true;
                        }
                        return ds;
                    }

                }
            }
           


        }

        private DataSet GetPendingRO()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            {

                string statussql = "select distinct exitapproval.exitID, exitapproval.exittime, exitapproval.projectdesc, exitapproval.company, count(exitapproval.EmpID) as 'no of emp' from exitapproval, EmpList " +
                    "where approve IS NULL AND reason NOT IN('Workplace Injury') and exitapproval.createdby = EmpList.EmpID AND EmpList.RO = '" + empID + "' and DATEADD(hour, 1, exittime) > CURRENT_TIMESTAMP " +
                    "group by exitapproval.exitID, exitapproval.createddate, exitapproval.exittime, exitapproval.reason, exitapproval.company, exitapproval.projectdesc order by exitID desc;";

                using (SqlCommand cmd3 = new SqlCommand(statussql, conn))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd3))
                    {
                        DataSet ds = new DataSet();
                        sda.Fill(ds);
                        DataTable dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                           
                            GridView1.DataSource = dt;
                            GridView1.DataBind();
                            noPending.Visible = false;

                        }
                        else
                        {
                            noPending.Text = "No Pending Early Exit Permit";
                            noPending.Visible = true;

                        }
                        return ds;

                    }

                }
            }


        }

        private DataSet GetApproved()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            //string statussql = "select distinct exitapproval.exitID, exitapproval.approveddate, exitapproval.approve,  exitapproval.projectdesc, exitapproval.company, exitapproval.approver from exitapproval, EmpList where reason NOT IN('Medical Injury') and exitapproval.createdby = EmpList.EmpID and exitapproval.approver = '" + empID + "' order by exitID desc";
            string statussql = "select distinct exitapproval.exitID, exitapproval.approve,  exitapproval.projectdesc, exitapproval.company, exitapproval.approver from exitapproval, EmpList where reason NOT IN('Workplace Injury') and exitapproval.createdby = EmpList.EmpID and exitapproval.approver = '" + empID + "' order by exitID desc";
            using (SqlConnection conn = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand(statussql))
                {
                    cmd.Connection = conn;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        sda.Fill(ds);
                        DataTable dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            GridView2.DataSource = dt;
                            GridView2.DataBind();
                            noPending.Visible = false;
                        }
                        else
                        {
                            noPending.Text = "No Approved/Rejected Early Exit Permit";
                            noPending.Visible = true;

                        }
                        return ds;
                        
                    }
                }
            }

        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string status = ddlStatus.SelectedItem.Text;
            if (status == "Approved/Rejected")
            {
                GetApproved();
                GridView1.Visible = false;
                GridView2.Visible = true;

            }
            else if (status == "Pending")
            {
                CheckAccess();
                GridView1.Visible = true;
                GridView2.Visible = false;
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                DateTime time1 = Convert.ToDateTime(e.Row.Cells[1].Text);
                e.Row.Cells[1].Text = time1.ToString("dd/MM/yyyy hh:mm tt");

                e.Row.Attributes["onclick"] = $"location.href = 'EarlyExitPermitApproval.aspx?exitid={GridView1.DataKeys[e.Row.RowIndex]["exitID"]}&approval={empID}'";
                e.Row.ToolTip = "Click to select this row.";

            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            //GetPending();
            //DataSet ds = this.GetPending();
            //GridView1.DataSource = ds;
            //GridView1.DataBind();
            CheckAccess();
        }

        protected void Eservice_Click(object sender, EventArgs e)
        {
            string myApp = ConfigurationManager.AppSettings["myApp"].ToString();
            Response.Redirect(myApp);
        }

        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.Cells[3].Text) == "True")
                {
                    e.Row.Cells[3].Text = "Approved";

                }
                else
                {
                    e.Row.Cells[3].Text = "Rejected";
                }
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GridView2, "Select$" + e.Row.RowIndex);
                e.Row.ToolTip = "Click to select this row.";
            }

        }

        protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView2.PageIndex = e.NewPageIndex;
            GetApproved();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            mpePopUp.Hide();
        }

        protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            string PJM = ConfigurationManager.AppSettings["PJM"].ToString();
            string Test = ConfigurationManager.AppSettings["Test"].ToString();
            string RO = ConfigurationManager.AppSettings["RO"].ToString();
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();

            //try
            //{
            int exitID = Convert.ToInt32(GridView2.SelectedRow.Cells[0].Text);
            string approve = GridView2.SelectedRow.Cells[3].Text;
            lblexitID.Text = "Early Exit Permit ID #" + exitID + " Details";
            lblStatus.Text = approve;
            if (approve == "Approved")
            {
                approve = "True";
            }
            else if (approve == "Rejected")
            {
                approve = "False";
            }
            else if (approve == "Pending")
            {
                approve = null;
            }
            bool isApprove = Convert.ToBoolean(approve);

            string sql = "select distinct(select distinct EmpList.Employee_Name from exitapproval, EmpList where exitapproval.approver = '" + empID + "' and exitapproval.approver = EmpList.EmpID and exitapproval.exitID = '" + exitID + "') AS 'approver', " +
                "exitapproval.createddate, exitapproval.exittime, exitapproval.projectdesc, exitapproval.company, exitapproval.reason, exitapproval.remarks from exitapproval, EmpList where exitapproval.createdby = EmpList.EmpID " +
                "and exitapproval.exitID = '" + exitID + "';";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt = ds.Tables[0];

            DateTime date = Convert.ToDateTime(dt.Rows[0][1]);
            DateTime time = Convert.ToDateTime(dt.Rows[0][2]);

            lblApprover.Text = dt.Rows[0][0].ToString();
            lblDate.Text = date.ToString("dd/MM/yyyy");
            lblTime.Text = time.ToString("hh:mm tt");
            lblProject.Text = dt.Rows[0][3].ToString();
            lblCompany.Text = dt.Rows[0][4].ToString();
            lblReason.Text = dt.Rows[0][5].ToString();
            
            if (dt.Rows[0][6].ToString() == "")
            {
                lblRemarks.Text = "N.A";
                remarks.Attributes.Add("class", "hide");
                lblRemarks.Attributes.Add("class", "hide");
            }
            else
            {
                lblRemarks.Attributes.Add("class", "label");
                remarks.Attributes.Add("class", "textbox");
                lblRemarks.Text = dt.Rows[0]["remarks"].ToString();
            }     

            string sql3 = "select distinct exitapproval.approve, exitapproval.approveddate, CONCAT(RTRIM(EmpList.EmpID), ' - ', EmpList.Employee_Name) as 'emp' " +
            "from exitapproval, EmpList where EmpList.EmpID = exitapproval.EmpID and exitapproval.exitID = '" + exitID + "' and exitapproval.approve = '" + isApprove + "' AND exitapproval.approver = '" + empID + "' order by emp";
            SqlDataAdapter da3 = new SqlDataAdapter(sql3, conn);
            DataSet ds3 = new DataSet();
            da3.Fill(ds3);
            DataTable dt3 = ds3.Tables[0];

          
            DateTime approvedate = Convert.ToDateTime(dt3.Rows[0]["approveddate"]);
            lblWhen.Text = approvedate.ToString("dd/MM/yyyy hh:mm tt");

            string empname = "";
            for (int i = 0; i < dt3.Rows.Count; i++)
            {

                empname += dt3.Rows[i][2].ToString() + "<br />";
            }

            lblEmpName.Text = empname;

            string sql2 = "select CONCAT(RTRIM(EmpList.EmpID), ' - ' , EmpList.Employee_Name) as 'emp' from EmpList, exitapproval " +
            "where exitapproval.exitID = '" + exitID + "' and EmpList.EmpID = exitapproval.EmpID order by emp;";
            SqlDataAdapter da2 = new SqlDataAdapter(sql2, conn);
            DataSet ds2 = new DataSet();
            da2.Fill(ds2);
            DataTable dt2 = ds2.Tables[0];

            string empName = "";
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                empName += dt2.Rows[i][0].ToString() + "<br />";

            }
            lblName.Text = empName;

            mpePopUp.Show();
            conn.Close();

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
    }

}