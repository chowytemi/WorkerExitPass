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

        protected void Page_Load(object sender, EventArgs e)
        { 

            if (!IsPostBack)
            {
                if ((Request.QueryString["exprmit"] != null))
                {

                    string myempno = Request.QueryString["exprmit"];
                    Session["empID"] = myempno;

                }
                //BindDataSetDataProjects();
                //RetrieveDataFromLogin();
                CheckAccess();
            }
        }

        protected void CheckAccess()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            string sql = "select EmpID from  EmpList where IsActive = 1 and CEmail IS NOT NULL and JobCode IN('SUBCON', 'WK') and EmpID = '" + empID + "';";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                RetrieveDataFromLogin();
                BindDataSetDataProjects();
            }
            else
            {

                Response.Redirect("http://eservices.dyna-mac.com/error");


            }

            dr.Close();
            con.Close();

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
                string empID = Session["empID"].ToString();
                Session["empID"] = empID;

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
                        Response.Redirect("Webform3.aspx?exprmitstatus=" + empID);
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

            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

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

                string empID = Session["empID"].ToString();
                Session["empID"] = empID;

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
                        //string sqlinsertapprovequery = "insert into exitapproval(approve, createdby, createddate, toexit, company, reason, Remarks, exittime, projectdesc, projcode) values(1, @createdby, @createddate, @toexit, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                        string sqlinsertapprovequery = "insert into exitapproval(exitID, approve, createdby, createddate, toexit, company, reason, Remarks, exittime, projectdesc, projcode) values((NEXT VALUE FOR exitID_Sequence), 1, @createdby, @createddate, @toexit, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";

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
                        //string sqlinsertquery = "insert into exitapproval(createdby, createddate, toexit, company, reason, Remarks, exittime, projectdesc, projcode) values( @createdby, @createddate, @toexit, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                        string sqlinsertquery = "insert into exitapproval(exitID, createdby, createddate, toexit, company, reason, Remarks, exittime, projectdesc, projcode) values((NEXT VALUE FOR exitID_Sequence), @createdby, @createddate, @toexit, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
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
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            //string PJM = ConfigurationManager.AppSettings["PJM"].ToString();
            string Test = ConfigurationManager.AppSettings["Test"].ToString();
            string RO = ConfigurationManager.AppSettings["RO"].ToString();
            string MailFrom = ConfigurationManager.AppSettings["MailFrom"].ToString();
            string smtpserver = ConfigurationManager.AppSettings["smtpserver"].ToString();
            string smtport = ConfigurationManager.AppSettings["smtport"].ToString();
            int smtpport = Convert.ToInt32(smtport);

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
                            //string createdby = dr[1].ToString();

                            //get exitid
                            string exitquery = "select exitID, projectdesc, exittime, toexit, reason from exitapproval where createdby = @empID and company = @company and exittime = @time";
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
                                        string project = exitdr[1].ToString();
                                        DateTime date = Convert.ToDateTime(exitdr[2]);
                                        string exittime = date.ToString("dd/MM/yyyy hh:mm tt");
                                        string toexit = exitdr[3].ToString();
                                        string reason = exitdr[4].ToString();

                                        string query3 = "select Employee_Name from EmpList where EmpID = '" + toexit + "';";

                                        using (SqlCommand cmd3 = new SqlCommand(query3, conn))
                                        {
                                            using (SqlDataReader dr3 = cmd3.ExecuteReader())
                                            {

                                                while (dr3.Read())
                                                {

                                                    string exitName = dr3[0].ToString();

                                                    //check if worker or subcon
                                                    if (dr[2].ToString() == "WK")
                                                    {
                                                        if (!string.IsNullOrEmpty(dr[5].ToString()))
                                                        {
                                                            //worker - email to HOD
                                                            //string ROname = dr[5].ToString();
                                                            string hodquery = "select distinct EmpList.EmpID,EmpList.CEmail " +
                                                                          "from Access, UserAccess, ARole, EmpList " +
                                                                          "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                                                          "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                                                          "and Access.id = '" + RO + "'";
                                                            //string hodquery = "select approveremail from testtable";
                                                            //string hodquery = "select cemail from EmpList where EmpID='" + ROname + "' and isActive = 1";
                                                            using (SqlCommand hodcmd = new SqlCommand(hodquery, conn))
                                                            {
                                                                using (SqlDataReader hoddr = hodcmd.ExecuteReader())
                                                                {
                                                                    while (hoddr.Read())
                                                                    {
                                                                        string ROid = hoddr[0].ToString();
                                                                        string ROcemail = hoddr[1].ToString(); 

                                                                        //Label2.Text = Request.Url.AbsoluteUri.Replace("WebForm1.aspx", "WebForm4.aspx?exitid=" + exitid);

                                                                        MailMessage mm = new MailMessage();
                                                                        mm.From = new MailAddress(MailFrom);
                                                                        //mm.Subject = "Early Exit Permit Pending RO for Approval";
                                                                        string body = "Hello,";
                                                                        body += "<br /><br />The following application was submitted:";
                                                                        body += "<br /><br /><table style=\"table-layout: fixed; text-align:left; border-collapse: collapse; border: 1px solid; width: 70%;\">";
                                                                        body += "<tr style=\" height: 0.5em;\">";
                                                                        body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Exit ID</th>";
                                                                        body += "<td style=\" border: 1px solid\">" + exitid + "</td>";
                                                                        body += "<tr style=\" height: 0.5em;\">";
                                                                        body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Project</th>";
                                                                        body += "<td style=\" border: 1px solid\">" + project + "</td>";
                                                                        body += "<tr style=\" height: 0.5em;\">";
                                                                        body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Reason</th>";
                                                                        body += "<td style=\" border: 1px solid\">" + reason + "</td>";
                                                                        body += "<tr style=\" height: 0.5em;\">";
                                                                        body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Requested time</th>";
                                                                        body += "<td style=\" border: 1px solid\">" + exittime + "</td>";
                                                                        body += "<tr style=\" height: 0.5em;\">";
                                                                        body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Employees exiting</th>";
                                                                        body += "<td style=\" border: 1px solid\">" + exitName + "</td></tr></table>";
                                                                        
                                                                        if (ReasonDropdown.Text == "Medical Injury")
                                                                        {

                                                                            mm.Subject = "Early Exit Permit Medical Injury Notification";

                                                                        }
                                                                        else
                                                                        {
                                                                            mm.Subject = "Early Exit Permit Pending RO for Approval";
                                                                            body += "<br />Please click <a href = '" + Request.Url.AbsoluteUri.Replace("WebForm1.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + ROid) + "'>here</a> to approve or reject the application";
                                                                            body += "<br /><br />Thank you";
                                                                        }
                                                                        //body += "<br />Please click <a href = '" + Request.Url.AbsoluteUri.Replace("WebForm1.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + ROid) + "'>here</a> to approve or reject the application:";                                                                        
                                                                        //body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("WebForm1.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + ROname) + "'>View Application</a>";
                                                                        
                                                                        mm.Body = body;
                                                                        mm.IsBodyHtml = true;
                                                                        mm.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"].ToString());
                                                                        mm.To.Add(new MailAddress(ROcemail));
                                                                        SmtpClient smtp = new SmtpClient(smtpserver, smtpport); //Gmail smtp  
                                                                        smtp.EnableSsl = false;                
                                                                        smtp.Send(mm);


                                                                    }
                                                                }

                                                            }
                                                        }

                                                    }
                                                    else if (dr[2].ToString() == "SUBCON")
                                                    {
                                                        //subcon - email to project managers
                                                        //Label2.Text = "subcon";

                                                        string pjmquery = "select distinct EmpList.EmpID,EmpList.CEmail " +
                                                                          "from Access, UserAccess, ARole, EmpList " +
                                                                          "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                                                          "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                                                          "and Access.id = '" + Test + "' and EmpList.EmpID = 'T202' OR EmpList.EmpID = 'T203'";
                                                        using (SqlCommand pjmcmd = new SqlCommand(pjmquery, conn))
                                                        {
                                                            using (SqlDataReader pjmdr = pjmcmd.ExecuteReader())
                                                            {
                                                                while (pjmdr.Read())
                                                                {
                                                                    string name = pjmdr[0].ToString();

                                                                    //string ROcemail = hoddr[0].ToString();
                                                                    //Label2.Text = Request.Url.AbsoluteUri.Replace("WebForm1.aspx", "WebForm4.aspx?exitid=" + exitid);

                                                                    MailMessage mm = new MailMessage();
                                                                    mm.From = new MailAddress(MailFrom);
                                                                    //mm.Subject = "Early Exit Permit Pending PJM for Approval";
                                                                    string body = "Hello,";
                                                                    body += "<br /><br />The following application was submitted:";
                                                                    body += "<br /><br /><table style=\"table-layout: fixed; text-align:left; border-collapse: collapse; border: 1px solid; width: 70%;\">";
                                                                    body += "<tr style=\" height: 0.5em;\">";
                                                                    body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Exit ID</th>";
                                                                    body += "<td style=\" border: 1px solid\">" + exitid + "</td>";
                                                                    body += "<tr style=\" height: 0.5em;\">";
                                                                    body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Project</th>";
                                                                    body += "<td style=\" border: 1px solid\">" + project + "</td>";
                                                                    body += "<tr style=\" height: 0.5em;\">";
                                                                    body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Reason</th>";
                                                                    body += "<td style=\" border: 1px solid\">" + reason + "</td>";
                                                                    body += "<tr style=\" height: 0.5em;\">";
                                                                    body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Requested time</th>";
                                                                    body += "<td style=\" border: 1px solid\">" + exittime + "</td>";
                                                                    body += "<tr style=\" height: 0.5em;\">";
                                                                    body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Employees exiting</th>";
                                                                    body += "<td style=\" border: 1px solid\">" + exitName + "</td></tr></table>";
                                                                    if (ReasonDropdown.Text == "Medical Injury")
                                                                    {

                                                                        mm.Subject = "Early Exit Permit Medical Injury Notification";

                                                                    }
                                                                    else
                                                                    {
                                                                        mm.Subject = "Early Exit Permit Pending PJM for Approval";
                                                                        body += "<br />Please click <a href = '" + Request.Url.AbsoluteUri.Replace("WebForm1.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + name) + "'>here</a> to approve or reject the application:";
                                                                        body += "<br /><br />Thank you";
                                                                    }
                                                                    //body += "<br />Please click <a href = '" + Request.Url.AbsoluteUri.Replace("WebForm1.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + name) + "'>here</a> to approve or reject the application";
                                                                    //body += "<br />Please click the following link to approve or reject the application:";
                                                                    //body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("WebForm1.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + name) + "'>View Application</a>";
                                                                    
                                                                    mm.Body = body;
                                                                    mm.IsBodyHtml = true;

                                                                    mm.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"].ToString());
                                                                    SmtpClient smtp = new SmtpClient(smtpserver, smtpport); //Gmail smtp
                                                                    smtp.EnableSsl = false;

                                                                    string pjmID = "";
                                                                    if (!pjmdr.IsDBNull(1))
                                                                    {
                                                                        pjmID = pjmdr.GetString(1);
                                                                        mm.Bcc.Add(new MailAddress(pjmID));
                                                                    }

                                                                    smtp.Send(mm);


                                                                }
                                                            }

                                                        }
                                                    }

                                                }
                                        }   }
                                    }
                                }



                            }

                        }

                    }



                }

            }


        }

        //protected void checkForAccess()
        //{

        //    //Connect to database
        //    string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
        //    SqlConnection conn = new SqlConnection(cs);
        //    conn.Open();
        //    string sqlquery = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList " +
        //    "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
        //    "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = 85";
        //    SqlDataAdapter sda = new SqlDataAdapter(sqlquery, conn);
        //    using (DataTable dt = new DataTable())
        //    {
        //        sda.Fill(dt);
        //        //GridView1.DataSource = dt;
        //        //GridView1.DataBind();

        //    }


        //}
        //protected void approveForm()
        //{

        //    //get approverID
        //    //get formID
        //    //btn for approve/reject

        //    string approverID = "T202";
        //    DateTime approveddate = DateTime.Now;
        //    int exitID = 23;
        //    int approve = 1;

        //    string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
        //    SqlConnection conn = new SqlConnection(cs);
        //    conn.Open();
        //    string sqlquery = "update exitapproval set approver = '" + approverID + "', approve = " + approve + ", approveddate = '" + approveddate + "' where exitID = '" + exitID + "'";

        //    using (SqlCommand update = new SqlCommand(sqlquery, conn))
        //    {
        //        update.ExecuteNonQuery();

        //        conn.Close();
        //    }
        //}

    }
}