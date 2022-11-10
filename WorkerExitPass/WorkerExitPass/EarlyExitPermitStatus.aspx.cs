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
        static string prevPage = String.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if ((Request.QueryString["exprmitstatus"] != null))
                {

                    string myempno = Request.QueryString["exprmitstatus"];
                    Session["empID"] = myempno;
                    

                    if (Request.UrlReferrer == null)
                    {
                        CheckAccess();

                    }
                    else if (Request.UrlReferrer != null)
                    {
                        FormStatus();
                    }
                    
                       
                    
                }

               
            }
            
        
        }

        private void FormStatus()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            //Connect to database
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            //string statussql = "select exitID, createddate, exittime, approve from exitapproval where createdby = '" + empID + "' order by exitID desc;";
            string statussql = "select distinct exitID, createddate, exittime, approve from exitapproval where EmpID = '" + empID + "' or createdby = '" + empID + "' order by exitID desc;";

            SqlDataAdapter da = new SqlDataAdapter(statussql, conn);
            using (DataTable dt = new DataTable())
            {
                da.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();

            }
            conn.Close();
        }

        protected void CheckAccess()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();

            string sqlcheck = "select AC.menu  from UserAccess as UA, Access as AC, EmpList as emp where UA.accessid = AC.ID " +
                "and emp.ID = UA.EmpID and UA.IsActive = 1 " +
                "and emp.EmpID = '" + empID + "'  and emp.isactive = 1   and AC.Application = 'Service Request' and ac.menu = 'btnexit'";
            SqlCommand cmdline = new SqlCommand(sqlcheck, con);
            SqlDataReader drcheck = cmdline.ExecuteReader();
            if (drcheck.HasRows)
            {
                FormStatus();
            }
            else
            {

                Response.Redirect("http://eservices.dyna-mac.com/error");


            }

            drcheck.Close();
            con.Close();


        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DateTime date1 = Convert.ToDateTime(e.Row.Cells[1].Text);
                e.Row.Cells[1].Text = date1.ToString("dd/MM/yyyy");

                //if ((e.Row.Cells[2].Text) == "&nbsp;")
                //{
                    
                //    e.Row.Cells[2].Text = "NULL";

                //}
                //else
                //{
                    DateTime time1 = Convert.ToDateTime(e.Row.Cells[2].Text);
                    e.Row.Cells[2].Text = time1.ToString("hh:mm tt");
                //}
                //DateTime expirytime = Convert.ToDateTime(e.Row.Cells[4].Text);
                //e.Row.Cells[4].Text = expirytime.ToString("dd/MM/yyyy hh:mm tt");

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
                int exitID = Convert.ToInt32(GridView1.SelectedRow.Cells[0].Text);
                string approve = GridView1.SelectedRow.Cells[3].Text;

                DateTime exittime = Convert.ToDateTime(GridView1.SelectedRow.Cells[2].Text);
                string expirytime = Convert.ToDateTime(exittime).AddHours(1).ToString("hh:mm tt");
                
                if (approve == "Approved")
                {
                    lblStatus.Text = approve + " (Valid Till: " + expirytime + ")";
                    approve = "True";
                    

                } else if (approve == "Rejected")
                {
                    lblStatus.Text = approve;
                    approve = "False"; 

                } else if (approve == "Pending")
                {
                    lblStatus.Text = approve;
                    approve = null;
                   
                }
                bool isApprove = Convert.ToBoolean(approve);
                //string sql = "select distinct exitapproval.approve, (select distinct EmpList.Employee_Name from exitapproval, EmpList where exitapproval.approver = EmpList.EmpID and exitapproval.exitID = '" + exitID + "') AS 'approver', exitapproval.approveddate, exitapproval.createddate, exitapproval.exittime, exitapproval.projectdesc, exitapproval.company, exitapproval.reason, exitapproval.remarks from exitapproval, EmpList where exitapproval.createdby = EmpList.EmpID and exitapproval.exitID = '" + exitID + "';";
                string sql = "select distinct (select distinct EmpList.Employee_Name from exitapproval, EmpList where exitapproval.approver = EmpList.EmpID and exitapproval.exitID = '" 
                + exitID + "') AS 'approver', exitapproval.createddate, exitapproval.exittime, exitapproval.projectdesc, exitapproval.company, exitapproval.reason," +
                "exitapproval.remarks from exitapproval, EmpList where exitapproval.createdby = EmpList.EmpID and exitapproval.exitID = '" + exitID + "';";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                DataTable dt = ds.Tables[0];

                if (!string.IsNullOrEmpty(dt.Rows[0]["approver"].ToString()))
                {
                    lblApprover.Text = dt.Rows[0]["approver"].ToString();
                }
                else
                {
                    if (dt.Rows[0]["reason"].ToString() == "Medical Injury")
                    {
                        lblApprover.Text = "N.A";
                        lblWhen.Text = "N.A";
                    }
                    else
                    {
                        lblWhen.Text = "Pending";
                        lblEmpName.Text = "Pending";
                        string sqlquery = "select EmpID, Employee_Name, JobCode, Department, designation, RO from EmpList where EmpID = '" + empID + "' and isActive = 1;";
                        using (SqlCommand cmd = new SqlCommand(sqlquery, conn))
                        {
                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    string ROname = dr[5].ToString();
                                    if (dr[2].ToString() == "WK")
                                    {
                                        string hodquery = "select Employee_Name from EmpList where EmpID='" + ROname + "' and isActive = 1";

                                        using (SqlCommand cmd2 = new SqlCommand(hodquery, conn))
                                        {
                                            SqlDataAdapter da4 = new SqlDataAdapter(hodquery, conn);
                                            DataSet ds4 = new DataSet();
                                            da4.Fill(ds4);
                                            DataTable dt4 = ds4.Tables[0];

                                            string RONames = "";
                                            for (int i = 0; i < dt4.Rows.Count; i++)
                                            {
                                            RONames += dt4.Rows[i][0].ToString() + "<br />";

                                            }
                                            lblApprover.Text = "Pending approval from: <br />" + RONames;
                                        }
                                    }
                                    else if (dr[2].ToString() == "SUBCON")
                                    {

                                        //for testing - supposed to be PJM
                                        string pjmquery = "select distinct EmpList.Employee_Name from Access, UserAccess,ARole,EmpList where UserAccess.RoleID = ARole.ID " +
                                            "and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + PJM + "'";
                                        using (SqlCommand cmd3 = new SqlCommand(pjmquery, conn))
                                        {
                                            SqlDataAdapter da5 = new SqlDataAdapter(pjmquery, conn);
                                            DataSet ds5 = new DataSet();
                                            da5.Fill(ds5);
                                            DataTable dt5 = ds5.Tables[0];

                                            string PJMNames = "";
                                            for (int i = 0; i < dt5.Rows.Count; i++)
                                            {
                                                PJMNames += dt5.Rows[i][0].ToString() + "<br />";

                                            }
                                            lblApprover.Text = "Pending approval from: <br />" + PJMNames;
                                        }
                                    }
                                    //else //for testing
                                    //{
                                    //    string pjmquery = "select distinct EmpList.Employee_Name from Access, UserAccess,ARole,EmpList where UserAccess.RoleID = ARole.ID " +
                                    //        "and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + RO + "'";
                                    //    using (SqlCommand cmd2 = new SqlCommand(pjmquery, conn))
                                    //    {
                                    //        SqlDataAdapter da6 = new SqlDataAdapter(pjmquery, conn);
                                    //        DataSet ds6 = new DataSet();
                                    //        da6.Fill(ds6);
                                    //        DataTable dt6 = ds6.Tables[0];

                                    //        string pjmNames = "";
                                    //        for (int i = 0; i < dt6.Rows.Count; i++)
                                    //        {
                                    //            pjmNames += dt6.Rows[i][0].ToString() + "<br />";

                                    //        }
                                    //        lblApprover.Text = "Pending approval from: <br />" + pjmNames;
                                    //    }
                                    //}
                                }
                            }
                        }
                    }


                }

                DateTime date = Convert.ToDateTime(dt.Rows[0]["createddate"]);
                DateTime time = Convert.ToDateTime(dt.Rows[0]["exittime"]);

                lblDate.Text = date.ToString("dd/MM/yyyy");
                lblTime.Text = time.ToString("hh:mm tt");
                lblProject.Text = dt.Rows[0]["projectdesc"].ToString();
                lblCompany.Text = dt.Rows[0]["company"].ToString();
                lblReason.Text = dt.Rows[0]["reason"].ToString();

                if (dt.Rows[0]["remarks"].ToString() == "")
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
                lblexitID.Text = "Early Exit Permit ID #" + exitID + " Details";

                string sql3 = "select distinct exitapproval.approve, exitapproval.approveddate, CONCAT(RTRIM(EmpList.EmpID), ' - ', EmpList.Employee_Name) as 'emp' " +
                "from exitapproval, EmpList where EmpList.EmpID = exitapproval.EmpID and exitapproval.exitID = '" + exitID + "' and exitapproval.approve = '" + isApprove + "' order by emp";
                SqlDataAdapter da3 = new SqlDataAdapter(sql3, conn);
                DataSet ds3 = new DataSet();
                da3.Fill(ds3);
                DataTable dt3 = ds3.Tables[0];

                using (DataTableReader reader = new DataTableReader(dt3))
                {
                    if (reader.HasRows)
                    {
                        DateTime approvedate = Convert.ToDateTime(dt3.Rows[0]["approveddate"]);
                        lblWhen.Text = approvedate.ToString("dd/MM/yyyy hh:mm tt");
                        if (dt3.Rows.Count == 1)
                        {
                            lblEmpName.Text = dt3.Rows[0]["emp"].ToString();

                        }
                        else
                        {
                            lblEmpName.Text += "<table>";

                            for (int i = 0; i < dt3.Rows.Count; i++)
                            {

                                lblEmpName.Text += "<tr><td>" + dt3.Rows[i][2].ToString() + "</td>";
                            }

                            lblEmpName.Text += "</table>";

                        }
                    }
                }
               
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Server.TransferRequest(Request.Url.AbsolutePath, false);
            //mpePopUp.Hide();
            //Response.Redirect(Request.RawUrl);
        }

        protected void InvalidatePermits()
        {
            //string empID = Session["empID"].ToString();
            //Session["empID"] = empID;

            ////Connect to database
            //string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            //SqlConnection conn = new SqlConnection(cs);
            //conn.Open();
            ////string statussql = "select exitID, createddate, exittime, approve from exitapproval where createdby = '" + empID + "' order by exitID desc;";
            //string statussql = "select distinct exitID, createddate, exittime, approve from exitapproval where EmpID = '" + empID + "' or createdby = '" + empID + "' order by exitID desc;";

            //SqlDataAdapter da = new SqlDataAdapter(statussql, conn);
            //using (DataTable dt = new DataTable())
            //{
            //    da.Fill(dt);
            //    GridView1.DataSource = dt;
            //    GridView1.DataBind();

            //}
        }

        protected void CreateNew_Click(object sender, EventArgs e)
        {
            string link = ConfigurationManager.AppSettings["link"].ToString();
            string TK = ConfigurationManager.AppSettings["TK"].ToString();
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name " +
                "from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + TK + "' and EmpList.EmpID = '" + empID + "' ; ";
            SqlCommand cmdline = new SqlCommand(sql, con);
            SqlDataReader drcheck = cmdline.ExecuteReader();
            if (drcheck.HasRows)
            {
                //Response.Redirect(link + "EarlyExitPermitTK.aspx?exprmit=" + empID);
                Response.Redirect(link + "exitpermit/EarlyExitPermitTK.aspx?exprmit=" + empID);
            }
            else
            {
                //Response.Redirect(link + "EarlyExitPermit.aspx?exprmit=" + empID);
                Response.Redirect(link + "exitpermit/EarlyExitPermit.aspx?exprmit=" + empID);

            }
        }
    }
}