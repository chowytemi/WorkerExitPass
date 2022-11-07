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
                CheckAccess();
                //CheckClockInEmp();
            }
        }

        protected void CheckClockInEmp()
        {
            string company = companytb.Text;


            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            string cmsstr = ConfigurationManager.ConnectionStrings["cms"].ConnectionString;

            if (company != "DMES")
            {
                SqlConnection con = new SqlConnection(cmsstr);
                con.Open();

                string sql = "select EmpID, StartTime ,EndTime from TimeLog where EndTime IS NULL AND CAST(StartTime AS Date) = CAST(GETDATE() AS Date) AND EmpID = '" + empID + "'; ";
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    CheckAccess();
                }
                else
                {
                    Response.Redirect("http://eservices.dyna-mac.com/error");
                }
            }
            else
            {
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

            string sqlcheck = "select AC.menu  from UserAccess as UA, Access as AC, EmpList as emp where UA.accessid = AC.ID " +
               "and emp.ID = UA.EmpID and UA.IsActive = 1 " +
               "and emp.EmpID = '" + empID + "'  and emp.isactive = 1   and AC.Application = 'Service Request' and ac.menu = 'btnexit'";
            SqlCommand cmdline = new SqlCommand(sqlcheck, con);
            SqlDataReader drcheck = cmdline.ExecuteReader();
            if (drcheck.HasRows)
            {
                string sql = "select EmpID from EmpList where IsActive = 1 and CEmail IS NOT NULL and EmpID = '" + empID + "';";
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    RetrieveDataFromLogin();
                    BindDataSetDataProjects();
                    BindDataSetDataReason();
                    mpePopUp.Show();
                }
                else
                {
                    Response.Redirect("http://eservices.dyna-mac.com/error");
                }

                dr.Close();
            }
            else
            {
                Response.Redirect("http://eservices.dyna-mac.com/error");
            }

            drcheck.Close();
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


                var time5pm = DateTime.Now.ToString("yyyy-MM-dd ") + "17:00:00.000";
                DateTime date5pm = DateTime.Parse(time5pm);
                var time6pm = DateTime.Now.ToString("yyyy-MM-dd ") + "18:00:00.000";
                DateTime date6pm = DateTime.Parse(time6pm);


                string projectInput = projectddl.Text;
                string nameInput = nametb.Text;
                string companyInput = companytb.Text;
                string reasonInput = ReasonDropdown.Text;
                string remarksInput = remarkstb.Text;

                if (projectInput != "" || nameInput != "" || companyInput != "")
                {
                    int compare = DateTime.Compare(dateinput, currentdate);

                    if (currentdate < date5pm || currentdate > date6pm)
                    {
                        if (compare > 0)
                        {
                            if (ReasonDropdown.SelectedValue == "Select")
                            {
                                ScriptManager.RegisterClientScriptBlock
                                  (this, this.GetType(), "alertMessage", "alert" +
                                  "('Please choose a valid reason')", true);
                                return;
                            }
                            else
                            {
                                CheckSubmission();
                            }
                        }
                        else if (compare <= 0)
                        {
                            ScriptManager.RegisterClientScriptBlock
                              (this, this.GetType(), "alertMessage", "alert" +
                              "('Please choose a time after the current time')", true);
                            return;
                        }


                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock
                             (this, this.GetType(), "alertMessage", "alert" +
                             "('Unable to submit permit between 5PM to 6PM. Please try again after 6PM')", true);
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

        protected void BindDataSetDataReason()
        {
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("select description from clockingreason where Sertype = 'Early Exit Permit'", con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            ds.Tables[0].Columns.Add("Description", typeof(string), "description");

            ReasonDropdown.DataTextField = "Description";
            ReasonDropdown.DataValueField = "description";
            ReasonDropdown.DataSource = ds;
            ReasonDropdown.DataBind();
        }

        //Get data from Login 
        protected void RetrieveDataFromLogin()
        {

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


        protected void CheckSubmission()
        {
            var time = Request["timeInput"];
            var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;

            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            //Connect to database

            string connectionstring = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection appcon = new SqlConnection(connectionstring);
            appcon.Open();

            //check for duplicate
            string sqlquery1 = "select exitID from exitapproval where  CAST(createddate AS Date ) = CAST(GETDATE() AS Date ) and empID = '" + empID + "' and exittime = '" + dateInput + "';";

            SqlCommand cmd1 = new SqlCommand(sqlquery1, appcon);
            SqlDataReader dr1 = cmd1.ExecuteReader();

            if (!dr1.HasRows)
            {
                //check for submit time after approved time
                string sqlquerycheck = "select exitid from exitapproval where cast('" + dateInput + "' as time) > cast(exittime as time) and CAST(createddate AS Date ) = CAST(GETDATE() AS Date ) and empID = '" + empID + "' and (approve = 1 or approve is not null)";

                SqlCommand cmdlinenocheck = new SqlCommand(sqlquerycheck, appcon);
                SqlDataReader drcheck = cmdlinenocheck.ExecuteReader();

                if (!drcheck.HasRows)
                {
                    submitForm();
                    //DateTime timeinput = Convert.ToDateTime(time);
                    //DateTime permitexpiry = timeinput.AddHours(1);
                    //valid.Text += permitexpiry.ToString("dd/MM/yyyy hh:mm tt") + ".";
                    //ModalPopupExtender1.Show();
                }
                else if (drcheck.HasRows)
                {
                    ScriptManager.RegisterClientScriptBlock
                         (this, this.GetType(), "alertMessage", "alert" +
                         "('There is already an approved permit before submitted time')", true);
                    return;
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock
                       (this, this.GetType(), "alertMessage", "alert" +
                       "('Duplicate Submission of time')", true);
                return;
            }

        }
        protected void submitForm()
        {
            try
            {
                string description = projectddl.Text;
                string projectInput = projectddl.Text;
                var time = Request["timeInput"];
                var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;

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
                        //string sqlinsertapprovequery = "insert into exitapproval(approve, createdby, createddate, EmpID, company, reason, Remarks, exittime, projectdesc, projcode) values(1, @createdby, @createddate, @EmpID, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                        string sqlinsertapprovequery = "insert into exitapproval(exitID, approveddate, approve, createdby, createddate, EmpID, company, reason, Remarks, exittime, projectdesc, projcode) " +
                            "values((NEXT VALUE FOR exitID_Sequence), @approveddate, 1, @createdby, @createddate, @EmpID, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";

                        using (SqlCommand insert = new SqlCommand(sqlinsertapprovequery, appcon))
                        {


                            insert.CommandType = CommandType.Text;
                            insert.Parameters.AddWithValue("@createdby", empID);
                            insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
                            insert.Parameters.AddWithValue("@approveddate", DateTime.Now.ToString());
                            insert.Parameters.AddWithValue("@EmpID", empID);
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
                        string sqlinsertquery = "insert into exitapproval(exitID, createdby, createddate, EmpID, company, reason, Remarks, exittime, projectdesc, projcode) " +
                            "values((NEXT VALUE FOR exitID_Sequence), @createdby, @createddate, @EmpID, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                        using (SqlCommand insert = new SqlCommand(sqlinsertquery, appcon))
                        {


                            insert.CommandType = CommandType.Text;
                            insert.Parameters.AddWithValue("@createdby", empID);
                            insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
                            insert.Parameters.AddWithValue("@EmpID", empID);
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

                sendEmailForApproval();
                DateTime timeinput = Convert.ToDateTime(time);
                DateTime permitexpiry = timeinput.AddHours(1);
                valid.Text += permitexpiry.ToString("dd/MM/yyyy hh:mm tt") + ".";
                ModalPopupExtender1.Show();
                //Response.Redirect("EarlyExitPermitStatus.aspx?exprmitstatus=" + empID);

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

            string PJM = ConfigurationManager.AppSettings["PJM"].ToString();
            string Test = ConfigurationManager.AppSettings["Test"].ToString();
            string RO = ConfigurationManager.AppSettings["RO"].ToString();
            string MailFrom = ConfigurationManager.AppSettings["MailFrom"].ToString();
            string smtpserver = ConfigurationManager.AppSettings["smtpserver"].ToString();
            string smtport = ConfigurationManager.AppSettings["smtport"].ToString();
            int smtpport = Convert.ToInt32(smtport);
            string link = ConfigurationManager.AppSettings["link"].ToString();
            string MailTo = ConfigurationManager.AppSettings["MailTo"].ToString();
            string MailToSafety = ConfigurationManager.AppSettings["MailToSafety"].ToString();

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
                            string exitquery = "select exitID, projectdesc, exittime, EmpID, reason from exitapproval where createdby = @empID and company = @company and exittime = @time";
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
                                        string EmpID = exitdr[3].ToString();
                                        string reason = exitdr[4].ToString();

                                        string query3 = "select Employee_Name from EmpList where EmpID = '" + EmpID + "';";

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
                                                            //worker - email to RO
                                                            string ROname = dr[5].ToString();
                                                            //string hodquery = "select distinct EmpList.EmpID,EmpList.CEmail " +
                                                            //              "from Access, UserAccess, ARole, EmpList " +
                                                            //              "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                                            //              "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                                            //              "and Access.id = '" + PJM + "' and EmpList.EmpID = 'T203'";
                                                            string hodquery = "select cemail from EmpList where EmpID='" + ROname + "' and isActive = 1";
                                                            using (SqlCommand hodcmd = new SqlCommand(hodquery, conn))
                                                            {
                                                                MailMessage mm = new MailMessage();
                                                                SmtpClient smtp = new SmtpClient(smtpserver, smtpport);

                                                                using (SqlDataReader hoddr = hodcmd.ExecuteReader())
                                                                {
                                                                    while (hoddr.Read())
                                                                    {
                                                                        string ROcemail = hoddr[0].ToString();
                                                                        //string ROcemail = hoddr[1].ToString(); //TESTING

                                                                        //MailMessage mm = new MailMessage();
                                                                        mm.From = new MailAddress(MailFrom);
                                                                        //string body = "Hello,";
                                                                        String body = "The following application was submitted:";
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
                                                                        body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Employee name</th>";
                                                                        body += "<td style=\" border: 1px solid\">" + exitName + "</td></tr></table>";
                                                                        string body1 = "";
                                                                        if (ReasonDropdown.Text == "Medical Injury")
                                                                        {

                                                                            mm.Subject = "Early Exit Permit Medical Injury Notification";

                                                                        }
                                                                        else
                                                                        {
                                                                            mm.Subject = "Early Exit Permit Pending RO for Approval";
                                                                            body1 += "<br />Please click <a href = '" + link + "default.aspx?exprmtid=" + exitid + "'>here</a> to approve or reject the application.";

                                                                        }

                                                                        body1 += "<br /><br />This is an automatically generated email, please do not reply.";
                                                                        mm.Body = body + body1;
                                                                        mm.IsBodyHtml = true;
                                                                        mm.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"].ToString());
                                                                        mm.To.Add(new MailAddress(ROcemail));
                                                                        //mm.To.Add(new MailAddress(MailTo));
                                                                        //SmtpClient smtp = new SmtpClient(smtpserver, smtpport);
                                                                        smtp.EnableSsl = false;
                                                                        //smtp.Send(mm);

                                                                    }
                                                                }
                                                                if (ReasonDropdown.Text == "Medical Injury")
                                                                {
                                                                    mm.To.Add(new MailAddress(MailTo));
                                                                    mm.To.Add(new MailAddress(MailToSafety));
                                                                }
                                                                else
                                                                {
                                                                    mm.To.Add(new MailAddress(MailTo));
                                                                }
                                                                smtp.Send(mm);
                                                            }
                                                        }

                                                    }
                                                    else if (dr[2].ToString() == "SUBCON")
                                                    {
                                                        //subcon - email to project managers
                                                        //for testing
                                                        string pjmquery = "select distinct EmpList.EmpID,EmpList.CEmail " +
                                                                          "from Access, UserAccess, ARole, EmpList " +
                                                                          "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                                                          "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                                                          "and Access.id = '" + PJM + "' and EmpList.EmpID = 'T203'";
                                                        using (SqlCommand pjmcmd = new SqlCommand(pjmquery, conn))
                                                        {
                                                            MailMessage mm = new MailMessage();
                                                            SmtpClient smtp = new SmtpClient(smtpserver, smtpport); //Gmail smtp                                                                        

                                                            using (SqlDataReader pjmdr = pjmcmd.ExecuteReader())
                                                            {
                                                                while (pjmdr.Read())
                                                                {

                                                                    //SqlDataAdapter da = new SqlDataAdapter(pjmquery, conn);
                                                                    //DataSet ds = new DataSet();
                                                                    //da.Fill(ds);
                                                                    //DataTable dt = ds.Tables[0];
                                                                    string name = pjmdr[0].ToString();


                                                                    //MailMessage mm = new MailMessage();
                                                                    mm.From = new MailAddress(MailFrom);
                                                                    //string body = "Hello,";
                                                                    String body = "The following application was submitted:";
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
                                                                    body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Exit Time</th>";
                                                                    body += "<td style=\" border: 1px solid\">" + exittime + "</td>";
                                                                    body += "<tr style=\" height: 0.5em;\">";
                                                                    body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Employee Name</th>";
                                                                    body += "<td style=\" border: 1px solid\">" + exitName + "</td></tr></table>";
                                                                    string body1 = "";
                                                                    if (ReasonDropdown.Text == "Medical Injury")
                                                                    {

                                                                        mm.Subject = "Early Exit Permit Medical Injury Notification";

                                                                    }
                                                                    else
                                                                    {
                                                                        mm.Subject = "Early Exit Permit Pending PJM for Approval";
                                                                        body1 += "<br />Please click <a href = '" + link + "default.aspx?exprmtid=" + exitid + "'>here</a> to approve or reject the application.";
                                                                    }
                                                                    mm.Body = body + body1;
                                                                    mm.IsBodyHtml = true;
                                                                    //SmtpClient smtp = new SmtpClient(smtpserver, smtpport); //Gmail smtp                                                                        
                                                                    smtp.EnableSsl = false;
                                                                    //for (int i = 0; i < dt.Rows.Count; i++)
                                                                    //{
                                                                    //    string pjmID = dt.Rows[i][1].ToString();
                                                                    //    mm.To.Add(new MailAddress(pjmID));
                                                                    //}
                                                                    
                                                                    string pjmID = pjmdr[1].ToString();
                                                                    mm.To.Add(new MailAddress(pjmID));
                                                                    
                                                                    //mm.To.Add(new MailAddress(MailTo));

                                                                    smtp.UseDefaultCredentials = false;
                                                                    //smtp.Send(mm);
                                                                }

                                                            }

                                                            if (ReasonDropdown.Text == "Medical Injury")
                                                            {
                                                                mm.To.Add(new MailAddress(MailTo));
                                                                //mm.To.Add(new MailAddress(MailToSafety));
                                                                mm.To.Add(new MailAddress("yutong.chow@dyna-mac.com"));
                                                            }
                                                            else
                                                            {
                                                                mm.To.Add(new MailAddress(MailTo));
                                                            }

                                                            smtp.Send(mm);
                                                        }

                                                    }
                                                    //else //for testing
                                                    //{
                                                    //    string hodquery = "select distinct EmpList.EmpID,EmpList.CEmail " +
                                                    //                      "from Access, UserAccess, ARole, EmpList " +
                                                    //                      "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                                    //                      "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                                    //                      "and Access.id = '" + RO + "'";
                                                    //    using (SqlCommand hodcmd = new SqlCommand(hodquery, conn))
                                                    //    {
                                                    //        using (SqlDataReader hoddr = hodcmd.ExecuteReader())
                                                    //        {
                                                    //            while (hoddr.Read())
                                                    //            {
                                                    //                string ROid = hoddr[0].ToString();
                                                    //                string ROcemail = hoddr[1].ToString();

                                                    //                MailMessage mm = new MailMessage();
                                                    //                mm.From = new MailAddress(MailFrom);
                                                    //                string body = "Hello,";
                                                    //                body += "<br /><br />The following application was submitted:";
                                                    //                body += "<br /><br /><table style=\"table-layout: fixed; text-align:left; border-collapse: collapse; border: 1px solid; width: 70%;\">";
                                                    //                body += "<tr style=\" height: 0.5em;\">";
                                                    //                body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Exit ID</th>";
                                                    //                body += "<td style=\" border: 1px solid\">" + exitid + "</td>";
                                                    //                body += "<tr style=\" height: 0.5em;\">";
                                                    //                body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Project</th>";
                                                    //                body += "<td style=\" border: 1px solid\">" + project + "</td>";
                                                    //                body += "<tr style=\" height: 0.5em;\">";
                                                    //                body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Reason</th>";
                                                    //                body += "<td style=\" border: 1px solid\">" + reason + "</td>";
                                                    //                body += "<tr style=\" height: 0.5em;\">";
                                                    //                body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Requested time</th>";
                                                    //                body += "<td style=\" border: 1px solid\">" + exittime + "</td>";
                                                    //                body += "<tr style=\" height: 0.5em;\">";
                                                    //                body += "<th style=\" color: #004B7A; text-align:left; border: 1px solid\">Employee name</th>";
                                                    //                body += "<td style=\" border: 1px solid\">" + exitName + "</td></tr></table>";
                                                    //                string body1 = "";

                                                    //                if (ReasonDropdown.Text == "Medical Injury")
                                                    //                {

                                                    //                    mm.Subject = "Early Exit Permit Medical Injury Notification";

                                                    //                }
                                                    //                else
                                                    //                {
                                                    //                    mm.Subject = "Early Exit Permit Pending Test for Approval";
                                                    //                    //body += "<br />Please click <a href = '" + Request.Url.AbsoluteUri.Replace("EarlyExitPermit.aspx?exprmit=" + empID, "EarlyExitPermitView.aspx?approval=" + ROid) + "'>here</a> to approve or reject the application.";
                                                    //                    //body1 += "<br />Please click <a href = '" + link + "EarlyExitPermitView.aspx?approval=" + ROid + "'>here</a> to approve or reject the application.";
                                                    //                    body1 += "<br />Please click <a href = '" + link + "default.aspx?exprmtid=" + exitid + "'>here</a> to approve or reject the application.";

                                                    //                }

                                                    //                body1 += "<br /><br />This is an automatically generated email, please do not reply.";
                                                    //                mm.Body = body + body1;
                                                    //                mm.IsBodyHtml = true;
                                                    //                mm.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"].ToString());
                                                    //                mm.To.Add(new MailAddress(ROcemail));
                                                    //                SmtpClient smtp = new SmtpClient(smtpserver, smtpport);
                                                    //                smtp.EnableSsl = false;
                                                    //                mm.To.Add(new MailAddress(MailTo));
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


            }

        }

        protected void CancelBtn_Click(object sender, EventArgs e)
        {
            string myApp = ConfigurationManager.AppSettings["myApp"].ToString();
            Response.Redirect(myApp);
        }

        protected void btnHelp_Click(object sender, EventArgs e)
        {
            mpePopUp.Show();
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            mpePopUp.Hide();
        }

        protected void viewStatus_Click(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            Response.Redirect("EarlyExitPermitStatus.aspx?exprmitstatus=" + empID);

        }

    }
}