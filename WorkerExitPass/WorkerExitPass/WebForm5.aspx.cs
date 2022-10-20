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
            //using test access 87, pjm access 83
            //string PJM = ConfigurationManager.AppSettings["PJM"].ToString();
            string Test = ConfigurationManager.AppSettings["Test"].ToString();
            string RO = ConfigurationManager.AppSettings["RO"].ToString();
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id ='" + Test + "' and EmpList.EmpID = '" + empID + "' ; ";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                IsApprove();
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
                    IsApprove();
                }
                else
                {
                    Response.Redirect("http://eservices.dyna-mac.com/error");
                }
                dr2.Close();

            }
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

        protected void sendEmail()
        {
            var exitID = Request.QueryString["exitid"];
            string MailFrom = ConfigurationManager.AppSettings["MailFrom"].ToString();
            string smtpserver = ConfigurationManager.AppSettings["smtpserver"].ToString();
            string smtport = ConfigurationManager.AppSettings["smtport"].ToString();
            int smtpport = Convert.ToInt32(smtport);

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                //string sqlquery = "select exitapproval.ID, exitapproval.toexit, EmpList.Employee_Name, EmpList.CEmail, exitapproval.approve, " +
                //    "exitapproval.exittime from EmpList, exitapproval where EmpList.EmpID = exitapproval.toexit and exitapproval.exitID = '" + exitID + "';";
                //string sqlquery = "select distinct exitapproval.createdby, EmpList.Employee_Name, EmpList.CEmail, exitapproval.approve, exitapproval.exittime from EmpList, exitapproval where EmpList.EmpID = exitapproval.createdby and exitapproval.exitID = '" + exitID + "' AND EmpList.CEmail IS NOT NULL;";
                //string sqlquery = "select distinct exitapproval.exitID, exitapproval.createdby, EmpList.Employee_Name, EmpList.CEmail, exitapproval.approve, exitapproval.exittime, exitapproval.reason from EmpList, exitapproval where EmpList.EmpID = exitapproval.createdby and exitapproval.exitID = '" + exitID + "' AND EmpList.CEmail IS NOT NULL;";
                string sqlquery = "select distinct exitapproval.exitID, (select distinct EmpList.Employee_Name from EmpList, exitapproval where exitapproval.createdby = EmpList.EmpID and exitapproval.exitID = '" + exitID + "') as 'createdBy', exitapproval.projectdesc, exitapproval.exittime, exitapproval.reason, EmpList.CEmail, (select distinct EmpList.Employee_Name from EmpList, exitapproval where exitapproval.approver = EmpList.EmpID and exitapproval.exitID = '" + exitID + "') as 'approver', exitapproval.approve, exitapproval.approveddate from EmpList, exitapproval where exitapproval.exitID = '" + exitID + "' AND EmpList.EmpID = exitapproval.createdby;";
                using (SqlCommand cmd = new SqlCommand(sqlquery, con))
                {
                    SqlDataAdapter da2 = new SqlDataAdapter(sqlquery, con);
                    DataSet ds2 = new DataSet();
                    da2.Fill(ds2);
                    DataTable dt2 = ds2.Tables[0];
                    

                    string id = dt2.Rows[0][0].ToString();
                    string createdByName = dt2.Rows[0][1].ToString();
                    string project = dt2.Rows[0][2].ToString();
                    DateTime date = Convert.ToDateTime(dt2.Rows[0][3]);
                    string exittime = date.ToString("dd/MM/yyyy hh:mm tt");
                    string reason = dt2.Rows[0][4].ToString();
                    string createdByEmail = dt2.Rows[0][5].ToString();
                    string approver = dt2.Rows[0][6].ToString();
                    DateTime date1 = Convert.ToDateTime(dt2.Rows[0][8]);
                    string approveddate = date1.ToString("dd/MM/yyyy hh:mm tt");
                    string status = dt2.Rows[0][7].ToString();
                    if (status == "True")
                    {
                        status = "approved";
                    } else
                    {
                        status = "rejected";
                    }
                    
                    

                    string sqlquery3 = "select EmpList.Employee_Name from EmpList, exitapproval where exitapproval.exitID = '" + exitID + "' and EmpList.EmpID = exitapproval.toexit;";
                            using (SqlCommand cmd3 = new SqlCommand(sqlquery3, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(sqlquery3, con);
                                DataSet ds = new DataSet();
                                da.Fill(ds);
                                DataTable dt = ds.Tables[0];

                                string exitNames = "";
                                string body = "";
                                body += "Hello, " + createdByName + ".";
                                body += "<br /><br />Your application for early exit permit on " + date + " has been " + status + ".";
                                body += "<br /><br /><table style=\"table-layout: fixed; text-align:center; border-collapse: collapse; border: 1px solid; width: 100%;\">";
                                body += "<tr style=\text-align:center; height: 0.5em;\">";
                                body += "<th style=\"color: #004B7A; border: 1px solid\">Exit ID</th>";
                                body += "<th style=\"color: #004B7A; border: 1px solid\">Employees exiting</th>";
                                body += "<th style=\"color: #004B7A; border: 1px solid\">Project</th>";
                                body += "<th style=\"color: #004B7A; border: 1px solid\">Exit time</th>";
                                body += "<th style=\"color: #004B7A; border: 1px solid\">Reason</th>";
                                body += "<th style=\"color: #004B7A; border: 1px solid\">Approver</th>";
                                body += "<th style=\"color: #004B7A; border: 1px solid\">Approved Date</th></tr>";
                                body += "<tr style=\"text-align:center; height: 0.5em;\" > ";
                                body += "<td style=\" border: 1px solid\">" + id + "</td>";

                                if (dt.Rows.Count == 1)
                                {
                                    //body += "<br /><br />Your application for early exit permit on " + date + " has been " + status + ".";
                                    body += "<td style=\" border: 1px solid\">" + createdByName + "</td>";
                                }
                                else
                                {
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        exitNames += dt.Rows[i][0].ToString() + "<br />";

                                    }
                                    //exitNames = exitNames.TrimEnd(',');
                                    //body += "<br /><br />Your application for early exit permit on " + date + " for these employees: " + exitNames + " has been " + status + ".";
                                    body += "<td style=\" border: 1px solid\">" + exitNames + "</td>";

                                }
                                body += "<td style=\" border: 1px solid\">" + project + "</td>";
                                body += "<td style=\" border: 1px solid\">" + exittime + "</td>";
                                body += "<td style=\" border: 1px solid\">" + reason + "</td>";
                                body += "<td style=\" border: 1px solid\">" + approver + "</td>";
                                body += "<td style=\" border: 1px solid\">" + approveddate + "</td></tr></table>";
                                body += "<br />Thank you";

                                string sqlquery2 = "select approveremail from testtable";
                                using (SqlCommand cmd2 = new SqlCommand(sqlquery2, con))
                                {
                                    using (SqlDataReader dr2 = cmd2.ExecuteReader())
                                    {   
                                        while (dr2.Read())
                                        {
                                            string email = dr2[0].ToString();

                                            MailMessage mm = new MailMessage();
                                            mm.From = new MailAddress(MailFrom);
                                            mm.Subject = "Early Exit Permit Is " + status;                                           
                                            mm.Body = body;
                                            mm.IsBodyHtml = true;
                                            mm.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"].ToString());
                                            mm.To.Add(new MailAddress(email));
                                            SmtpClient smtp = new SmtpClient(smtpserver, smtpport); //Gmail smtp  
                                            smtp.EnableSsl = false;
                                            smtp.Send(mm);
                                        }
                                    

                                    }
                                }    
                            }
                            
                        
                        con.Close();
                    //}
                }
            }
        }

        protected void ApproveBtn_Click(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            DateTime approveddate = DateTime.Now;
            var exitID = Request.QueryString["exitid"];
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
            sendEmail();
            mpeApproval.Hide();
            Response.Redirect("WebForm4.aspx?approval=" + empID);
            
            
        }

        protected void RejectBtn_Click(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            DateTime approveddate = DateTime.Now;
            var exitID = Request.QueryString["exitid"];
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
            sendEmail();
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