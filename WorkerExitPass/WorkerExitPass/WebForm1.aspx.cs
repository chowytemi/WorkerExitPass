using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WorkerExitPass
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        //Get login id
        string empID = "MB638";
        //string empID = "PXE6563";


        protected void Page_Load(object sender, EventArgs e)
        {   
            if (!IsPostBack)
            {
                BindDataSetDataProjects();
                RetrieveDataFromLogin();
            }
        }

        protected void ReasonDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReasonDropdown.SelectedValue == "Others")
            {
                lblremarks.Visible = true;
                remarkstb.Visible = true;

            }
            else
            {
                lblremarks.Visible = false;
                remarkstb.Visible = false;
            }
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var time = Request["timeInput"];
                var date = DateTime.Now.ToString("yyyy-MM-dd ") + time;
                DateTime dateinput = DateTime.Parse(date);
                var currentdate = DateTime.Now;
                string projectInput = projectddl.Text;
                string nameInput = nametb.Text;
                string companyInput = companytb.Text;
                string reasonInput = ReasonDropdown.Text;
                string remarksInput = remarkstb.Text;

                if (projectInput != "" || nameInput != "" || companyInput != "")
                {
                    int compare = DateTime.Compare(dateinput, currentdate);
                    if (compare > 0)
                    {
                        submitForm();
                        sendEmailForApproval();
                        Response.Redirect("Webform3.aspx");
                    }
                    else if (compare <= 0)
                    {
                        ScriptManager.RegisterClientScriptBlock
                          (this, this.GetType(), "alertMessage", "alert" +
                          "('Please choose a time after the current time')", true);
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            //GetGridDataEmail();

            //submitForm();

            //sendEmailForApproval();
            //approveForm();
            //formStatus();
            //CheckFormInputs();
            //checkForAccess();
        }

        //Fill Project Dropdown with data
        protected void BindDataSetDataProjects()
        {
            string cs = ConfigurationManager.ConnectionStrings["service"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("select ID, code, description from PROJECT where isActive = 1 and Type = 'PR'", con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            ds.Tables[0].Columns.Add("Description", typeof(string), "description");

            projectddl.DataTextField = "Description";
            projectddl.DataValueField = "description";
            projectddl.DataSource = ds;
            projectddl.DataBind();
        }

        //Get data from Login - currently hardcoded
        protected void RetrieveDataFromLogin()
        {
            //Connect to database
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            string sqlquery = "select ID, IDType, IDNo, EmpID, Employee_Name, Department, Section, Company, designation, JobCode from Emplist where isActive = 1 and EmpID = '" + empID + "' ; ";
            conn.Open();
            SqlCommand cmdlineno = new SqlCommand(sqlquery, conn);
            SqlDataReader dr = cmdlineno.ExecuteReader();
            while (dr.Read())
            {
                nametb.Text = dr[4].ToString();
                companytb.Text = dr[7].ToString();

            }
            dr.Close();
            conn.Close();
        }

        protected void CheckFormInputs()
        {
            var time = Request["timeInput"];
            var date = DateTime.Now.ToString("yyyy-MM-dd ") + time;
            DateTime dateinput = DateTime.Parse(date);
            var currentdate = DateTime.Now;
            string projectInput = projectddl.Text;
            string nameInput = nametb.Text;
            string companyInput = companytb.Text;
            string reasonInput = ReasonDropdown.Text;
            string remarksInput = remarkstb.Text;

            if (projectInput != "" || nameInput != "" || companyInput != "")
            {
                int compare = DateTime.Compare(dateinput, currentdate);
                if (compare > 0)
                {
                    Label1.Text = "After Current Time";
                }
                else if (compare <= 0)
                {
                    ScriptManager.RegisterClientScriptBlock
                      (this, this.GetType(), "alertMessage", "alert" +
                      "('Please choose a time after the current time')", true);
                    return;
                }
            }
        }

        protected void submitForm()
        {
            try
            {
                string description = projectddl.Text;
                string projectInput = projectddl.Text;


                //Connect to database
                string cs = ConfigurationManager.ConnectionStrings["service"].ConnectionString;
                SqlConnection conn = new SqlConnection(cs);
                conn.Open();

                string connectionstring = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
                SqlConnection appcon = new SqlConnection(connectionstring);
                appcon.Open();

                //get code
                string sqlquery = " select code from PROJECT where description = '" + description + "' and IsActive = 1";
                SqlCommand cmdlineno = new SqlCommand(sqlquery, conn);
                SqlDataReader dr = cmdlineno.ExecuteReader();

                while (dr.Read())
                {
                    string projectcode = dr[0].ToString();

                    if (ReasonDropdown.Text == "Medical Injury")
                    {
                        //insert request
                        string sqlinsertapprovequery = "insert into exitapproval(approve, createdby, createddate, toexit, company, reason, Remarks, exittime, projectdesc, projcode) values(1, @createdby, @createddate, @toexit, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";

                        using (SqlCommand insert = new SqlCommand(sqlinsertapprovequery, appcon))
                        {

                            var time = Request["timeInput"];
                            var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;


                            insert.CommandType = CommandType.Text;
                            insert.Parameters.AddWithValue("@createdby", empID);
                            insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
                            insert.Parameters.AddWithValue("@toexit", empID);
                            insert.Parameters.AddWithValue("@company", HttpUtility.HtmlDecode(companytb.Text));
                            insert.Parameters.AddWithValue("@reason", HttpUtility.HtmlDecode(ReasonDropdown.Text));
                            insert.Parameters.AddWithValue("@Remarks", HttpUtility.HtmlDecode(remarkstb.Text));
                            insert.Parameters.AddWithValue("@exittime", dateInput);
                            insert.Parameters.AddWithValue("@projectdesc", projectInput);
                            insert.Parameters.AddWithValue("@projectcode", projectcode);

                            insert.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        //insert request
                        string sqlinsertquery = "insert into exitapproval(createdby, createddate, toexit, company, reason, Remarks, exittime, projectdesc, projcode) values( @createdby, @createddate, @toexit, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";

                        using (SqlCommand insert = new SqlCommand(sqlinsertquery, appcon))
                        {

                            var time = Request["timeInput"];
                            var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;


                            insert.CommandType = CommandType.Text;
                            insert.Parameters.AddWithValue("@createdby", empID);
                            insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
                            insert.Parameters.AddWithValue("@toexit", empID);
                            insert.Parameters.AddWithValue("@company", HttpUtility.HtmlDecode(companytb.Text));
                            insert.Parameters.AddWithValue("@reason", HttpUtility.HtmlDecode(ReasonDropdown.Text));
                            insert.Parameters.AddWithValue("@Remarks", HttpUtility.HtmlDecode(remarkstb.Text));
                            insert.Parameters.AddWithValue("@exittime", dateInput);
                            insert.Parameters.AddWithValue("@projectdesc", projectInput);
                            insert.Parameters.AddWithValue("@projectcode", projectcode);

                            insert.ExecuteNonQuery();
                        }
                    }


                    appcon.Close();

                }
                dr.Close();
                conn.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        protected void sendEmailForApproval()
        {

            var time = Request["timeInput"];
            var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;


            //Connect to database
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                string sqlquery = "select EmpID, Employee_Name, JobCode, Department, designation, RO from EmpList where EmpID = '" + empID + "' and isActive = 1";
                using (SqlCommand cmdlineno = new SqlCommand(sqlquery, conn))
                {

                    using (SqlDataReader dr = cmdlineno.ExecuteReader())
                    {

                        while (dr.Read())
                        {

                            //get exitid
                            string exitquery = "select exitID, createdby, exittime, toexit, reason from exitapproval where createdby = @empID and company = @company and exittime = @time";
                            //string exitquery = "select exitID from exitapproval where createdby = @empID and company = @company and exittime = @time";
                            using (SqlCommand exitcmd = new SqlCommand(exitquery, conn))
                            {
                                exitcmd.Parameters.AddWithValue("@empID", empID);
                                exitcmd.Parameters.AddWithValue("@company", companytb.Text);
                                exitcmd.Parameters.AddWithValue("@time", dateInput);
                                using (SqlDataReader exitdr = exitcmd.ExecuteReader())
                                {

                                    while (exitdr.Read())
                                    {
                                        string exitid = exitdr[0].ToString();
                                        string createdby = exitdr[1].ToString();
                                        //string exittime = exitdr[2].ToString();
                                        DateTime date = Convert.ToDateTime(exitdr[2]);
                                        string exittime = date.ToString("dd/MM/yyyy hh:mm tt");
                                        string toexit = exitdr[3].ToString();
                                        string reason = exitdr[4].ToString();

                                        //check if worker or subcon
                                        if (dr[2].ToString() == "WK")
                                        {
                                            if (!string.IsNullOrEmpty(dr[5].ToString()))
                                            {
                                                //worker - email to HOD
                                                string ROname = dr[5].ToString();

                                                string hodquery = "select cemail from EmpList where EmpID='" + ROname + "' and isActive = 1";
                                                using (SqlCommand hodcmd = new SqlCommand(hodquery, conn))
                                                {
                                                    using (SqlDataReader hoddr = hodcmd.ExecuteReader())
                                                    {
                                                        while (hoddr.Read())
                                                        {
                                                            //string ROcemail = hoddr[0].ToString();

                                                      
                                                            //Label2.Text = Request.Url.AbsoluteUri.Replace("WebForm1.aspx", "WebForm4.aspx?exitid=" + exitid);
                                                            
                                                            using (MailMessage mm = new MailMessage("@outlook.com", ROcemail))
                                                            {
                                                                //mm.Subject = "Account Activation";
                                                                mm.Subject = "Early Exit Permit Pending for Approval";
                                                                string body = "Hello,";
                                                                body += "<br /><br />The following application was submitted:";
                                                                body += "<br /><br /><table style=\"table-layout: fixed; text-align:center; border-collapse: collapse; border: 1px solid; width: 70%;\">";
                                                                body += "<tr style=\text-align:center; height: 0.5em;\">";
                                                                body += "<th style=\"color: #004B7A; border: 1px solid\">Exit ID</th>";
                                                                body += "<th style=\"color: #004B7A; border: 1px solid\">Created by</th>";
                                                                body += "<th style=\"color: #004B7A; border: 1px solid\">Employees exiting</th>";
                                                                body += "<th style=\"color: #004B7A; border: 1px solid\">Requested time</th>";
                                                                body += "<th style=\"color: #004B7A; border: 1px solid\">Reason</th></tr>";
                                                                body += "<tr style=\"text-align:center; height: 0.5em;\" > ";
                                                                body += "<td style=\" border: 1px solid\">" + exitid + "</td>";
                                                                body += "<td style=\" border: 1px solid\">" + createdby + "</td>";
                                                                body += "<td style=\" border: 1px solid\">" + toexit + "</td>";
                                                                body += "<td style=\" border: 1px solid\">" + exittime + "</td>";
                                                                body += "<td style=\" border: 1px solid\">" + reason + "</td></tr></table>";
                                                                body += "<br />Please click the following link to approve or reject the application:";
                                                                body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("WebForm1.aspx", "WebForm4.aspx?exitid=" + exitid) + "'>View Application</a>";
                                                                body += "<br /><br />Thank you";
                                                                mm.Body = body;
                                                                mm.IsBodyHtml = true;
                                                                SmtpClient smtp = new SmtpClient();
                                                                smtp.Host = "smtp-mail.outlook.com";
                                                                smtp.EnableSsl = true;
                                                                NetworkCredential NetworkCred = new NetworkCredential("@outlook.com", "");
                                                                
                                                                smtp.UseDefaultCredentials = false;
                                                                smtp.Credentials = NetworkCred;
                                                                smtp.Port = 587;
                                                                smtp.Send(mm);
                                                            }

                                                        }
                                                    }

                                                }
                                            }

                                        }
                                        else if (dr[2].ToString() == "SUBCON")
                                        {
                                            //subcon - email to project managers
                                            //Label2.Text = "subcon";

                                            //string hodquery = "select distinct   EmpList.EmpID,EmpList.designation,EmpList.Employee_Name,EmpList.CEmail " +
                                            //                  "from Access, UserAccess, ARole, EmpList " +
                                            //                  "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                            //                  "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                            //                  "and Access.id = 83";
                                            //using (SqlCommand hodcmd = new SqlCommand(hodquery, conn))
                                            //{
                                            //    using (SqlDataReader hoddr = hodcmd.ExecuteReader())
                                            //    {
                                            //        while (hoddr.Read())
                                            //        {
                                            //            //string ROcemail = hoddr[0].ToString();
                                            //            string ROcemail = "chowytemi07.20@ichat.sp.edu.sg";
                                            //            Label2.Text = Request.Url.AbsoluteUri.Replace("WebForm1.aspx", "WebForm4.aspx?exitid=" + exitid);

                                            //            using (MailMessage mm = new MailMessage("@outlook.com", ROcemail))
                                            //            {
                                            //                mm.Subject = "Account Activation";
                                            //                string body = "Hello,";
                                            //                body += "<br /><br />Please click the following link to approve or reject the application";
                                            //                body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("WebForm1.aspx", "WebForm4.aspx?exitid=" + exitid) + "'>Click here to approve or deny applications.</a>";
                                            //                body += "<br /><br />Thanks";
                                            //                mm.Body = body;
                                            //                mm.IsBodyHtml = true;
                                            //                SmtpClient smtp = new SmtpClient();
                                            //                smtp.Host = "smtp-mail.outlook.com";
                                            //                smtp.EnableSsl = true;
                                            //                NetworkCredential NetworkCred = new NetworkCredential("@outlook.com", "");
                                            //                smtp.UseDefaultCredentials = false;
                                            //                smtp.Credentials = NetworkCred;
                                            //                smtp.Port = 587;
                                            //                smtp.Send(mm);
                                            //            }

                                            //        }
                                            //    }

                                            //}
                                        }


                                    }
                                }



                            }

                        }

                    }



                }

            }


        }

        protected void checkForAccess()
        {

            //Connect to database
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string sqlquery = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList " +
            "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
            "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = 85";
            SqlDataAdapter sda = new SqlDataAdapter(sqlquery, conn);
            using (DataTable dt = new DataTable())
            {
                sda.Fill(dt);
                //GridView1.DataSource = dt;
                //GridView1.DataBind();

            }


        }
        protected void approveForm()
        {

            //get approverID
            //get formID
            //btn for approve/reject

            string approverID = "T202";
            DateTime approveddate = DateTime.Now;
            int exitID = 23;
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
        }

        //protected void formStatus()
        //{
        //    //Connect to database
        //    string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
        //    SqlConnection conn = new SqlConnection(cs);
        //    conn.Open();
        //    string statussql = "select createddate, exittime, approve from exitapproval where createdby = '" + empID + "'";
        //    SqlDataAdapter da = new SqlDataAdapter(statussql, conn);
        //    using (DataTable dt = new DataTable())
        //    {
        //        da.Fill(dt);
        //        //GridView1.DataSource = dt;
        //        //GridView1.DataBind();

        //        //if approve == null, pending

        //    }
        //}
    }
}