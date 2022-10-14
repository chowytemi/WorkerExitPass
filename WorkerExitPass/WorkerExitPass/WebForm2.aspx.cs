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
    public partial class WebForm2 : System.Web.UI.Page
    {

        //string empID = "BOB-2008";
        //string empID = "MIZU ENGRGW0100";
        //string firstId = "G";
        //string lastFiveId = "9574R";
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //SoloBtn.Attributes.Add("class", "activeBtn");
            //}
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

        protected void SoloBtn_Click(object sender, EventArgs e)
        {
            namesddl.Visible = false;
            nametb.Visible = true;
        }

        protected void TeamBtn_Click(object sender, EventArgs e)
        {
            namesddl.Visible = true;
            nametb.Visible = false;
            GetListOfEmployees();


        }

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

        protected void GetListOfEmployees()
        {
            string constr = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            string company = companytb.Text;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //using (SqlCommand cmd = new SqlCommand("select Employee_Name, EmpID from EmpList where Department = 'SUBCON' AND IsActive = 1 AND company = '" + company + "';"))
                using (SqlCommand cmd = new SqlCommand("select CONCAT(Employee_Name, ' (', RTRIM(EmpID), ')') AS 'empNameID' from EmpList where Department = 'SUBCON' AND IsActive = 1 AND company = '" + company + "' order by EmpID;"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    namesddl.DataSource = cmd.ExecuteReader();
                    namesddl.DataTextField = "empNameID";
                    //namesddl.DataValueField = "EmpID";
                    namesddl.DataBind();
                    con.Close();
                }
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


        //Get data from Login - currently hardcoded
        protected void RetrieveDataFromLogin()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            //Connect to database
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            string sqlquery = "select ID, IDType, IDNo, EmpID, Employee_Name, Department, Section, Company, designation, JobCode from Emplist where isActive = 1 and EmpID = '" + empID + "' ; ";
            //string sqlquery = "select ID, IDType, IDNo, EmpID, Employee_Name, Department, Section, Company, designation, JobCode from Emplist where isActive = 1 and ((IDNo like CONCAT('" + firstId + "', '%')) and (IDNo like CONCAT('%', '" + lastFiveId + "')));";
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
        protected void CheckAccess()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = 85 and EmpList.EmpID = '" + empID + "' ; ";
            //string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = 85 and ((IDNo like CONCAT('" + firstId + "', '%')) and (IDNo like CONCAT('%', '" + lastFiveId + "')));";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                RetrieveDataFromLogin();
                BindDataSetDataProjects();
                GetListOfEmployees();
            }
            else
            {
                Response.Redirect("http://eservices.dyna-mac.com/error");
            }

            dr.Close();
            con.Close();

        }

        protected void submitForm()
        {
            try
            {
                string empID = Session["empID"].ToString();
                Session["empID"] = empID;
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

                sendEmailForApproval();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        protected void namesddl_SelectedIndexChanged(object sender, EventArgs e)
        {

            int empCount = 0;
            //string empName = "";
            for (int i = 0; i < namesddl.Items.Count; i++)
            {
                if (namesddl.Items[i].Selected)
                {
                    empCount++;
                    //empName += namesddl.Items[i].Text + ",";
                    namesddl.SelectedItem.Selected = true;

                }
            }
            //empName = empName.TrimEnd(',');
            namesddl.Texts.SelectBoxCaption = empCount + " selected";

        }

        protected void SoloSubmit()
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



        protected void TeamSubmit()
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

            try
            {

                //get code
                string sqlquery = " select code from PROJECT where description = '" + description + "' and IsActive = 1";
                SqlCommand cmdlineno = new SqlCommand(sqlquery, conn);
                using (SqlDataReader dr = cmdlineno.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string projectcode = dr[0].ToString();

                        //insert request

                        int counter = 0;

                        for (int i = 0; i < namesddl.Items.Count; i++)
                        {
                           

                            if (namesddl.Items[i].Selected)
                            {
                                

                                //get EmpID
                                string empquery = " select empID from EmpList where Employee_Name = '" + namesddl.Items[i].Text + "' and IsActive = 1";
                                SqlCommand empcmd = new SqlCommand(empquery, appcon);
                                using (SqlDataReader empdr = empcmd.ExecuteReader())
                                {
                                    while (empdr.Read())
                                    {
                                        if (counter == 0)
                                        {
                                            string insertsinglequery = "";
                                            if (ReasonDropdown.Text == "Medical Injury")
                                            {
                                                insertsinglequery = "insert into exitapproval(exitID, approve, createdby, createddate, toexit, company, reason, Remarks, exittime, projectdesc, projcode) values((NEXT VALUE FOR exitID_Sequence), 1, @createdby, @createddate, @toexit, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                                            } else 
                                            {
                                                insertsinglequery = "insert into exitapproval(exitID, createdby, createddate, toexit, company, reason, Remarks, exittime, projectdesc, projcode) values((NEXT VALUE FOR exitID_Sequence), @createdby, @createddate, @toexit, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                                            }
                                                
                                         
                                            string employeeIDToExit = empdr[0].ToString();
                                            using (SqlCommand insert = new SqlCommand(insertsinglequery, appcon))
                                            {

                                                var time = Request["timeInput"];
                                                var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;


                                                insert.CommandType = CommandType.Text;
                                                insert.Parameters.AddWithValue("@createdby", empID);
                                                insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
                                                insert.Parameters.AddWithValue("@toexit", employeeIDToExit);
                                                insert.Parameters.AddWithValue("@company", HttpUtility.HtmlDecode(companytb.Text));
                                                insert.Parameters.AddWithValue("@reason", HttpUtility.HtmlDecode(ReasonDropdown.Text));
                                                insert.Parameters.AddWithValue("@Remarks", HttpUtility.HtmlDecode(remarkstb.Text));
                                                insert.Parameters.AddWithValue("@exittime", dateInput);
                                                insert.Parameters.AddWithValue("@projectdesc", projectInput);
                                                insert.Parameters.AddWithValue("@projectcode", projectcode);

                                                insert.ExecuteNonQuery();

                                                counter += 1;
                                            }
                                        }
                                        else if (counter > 0)
                                        {
                                            string insertmultiplequery = "";
                                            if (ReasonDropdown.Text == "Medical Injury")
                                            {
                                                insertmultiplequery = "insert into exitapproval(exitID, approve, createdby, createddate, toexit, company, reason, Remarks, exittime, projectdesc, projcode) values(CONVERT(int, (SELECT current_value FROM sys.sequences WHERE name = 'exitID_Sequence')), 1, @createdby, @createddate, @toexit, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                                            }
                                            else
                                            {
                                                insertmultiplequery = "insert into exitapproval(exitID, createdby, createddate, toexit, company, reason, Remarks, exittime, projectdesc, projcode) values(CONVERT(int, (SELECT current_value FROM sys.sequences WHERE name = 'exitID_Sequence')), @createdby, @createddate, @toexit, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                                            }
                                            

                                            string employeeIDToExit = empdr[0].ToString();
                                            using (SqlCommand insert = new SqlCommand(insertmultiplequery, appcon))
                                            {

                                                var time = Request["timeInput"];
                                                var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;


                                                insert.CommandType = CommandType.Text;
                                                insert.Parameters.AddWithValue("@createdby", empID);
                                                insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
                                                insert.Parameters.AddWithValue("@toexit", employeeIDToExit);
                                                insert.Parameters.AddWithValue("@company", HttpUtility.HtmlDecode(companytb.Text));
                                                insert.Parameters.AddWithValue("@reason", HttpUtility.HtmlDecode(ReasonDropdown.Text));
                                                insert.Parameters.AddWithValue("@Remarks", HttpUtility.HtmlDecode(remarkstb.Text));
                                                insert.Parameters.AddWithValue("@exittime", dateInput);
                                                insert.Parameters.AddWithValue("@projectdesc", projectInput);
                                                insert.Parameters.AddWithValue("@projectcode", projectcode);

                                                insert.ExecuteNonQuery();

                                                counter += 1;
                                            }
                                        }

                                        
                                    }
                                }



                            }
                            
                        }
                    }



                }

            } catch (Exception ex)
            {
                throw ex;
            }
            

        }

        protected void sendEmailForApproval()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            string FromEmail = ConfigurationManager.AppSettings["FromMail"].ToString();
            string EmailPassword = ConfigurationManager.AppSettings["Password"].ToString();


            var time = Request["timeInput"];
            var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;

            try
            {
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

                                                                MailMessage mm = new MailMessage();
                                                                mm.From = new MailAddress(FromEmail);
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


                                                                mm.Bcc.Add(new MailAddress(ROcemail));


                                                                SmtpClient smtp = new SmtpClient();
                                                                smtp.Host = "smtp-mail.outlook.com";
                                                                smtp.EnableSsl = true;
                                                                NetworkCredential NetworkCred = new NetworkCredential(FromEmail, EmailPassword);
                                                                smtp.UseDefaultCredentials = false;
                                                                smtp.Credentials = NetworkCred;
                                                                smtp.Port = 587;
                                                                smtp.Send(mm);


                                                            }
                                                        }

                                                    }
                                                }

                                            }
                                            else if (dr[2].ToString() == "SUBCON")
                                            {
                                                //subcon - email to project managers
                                                Label2.Text = "subcon";

                                                string pjmquery = "select distinct   EmpList.EmpID,EmpList.CEmail " +
                                                                  "from Access, UserAccess, ARole, EmpList " +
                                                                  "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                                                  "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                                                  "and Access.id = 83";
                                                using (SqlCommand pjmcmd = new SqlCommand(pjmquery, conn))
                                                {
                                                    using (SqlDataReader pjmdr = pjmcmd.ExecuteReader())
                                                    {
                                                        while (pjmdr.Read())
                                                        {
                                                            //string ROcemail = hoddr[0].ToString();
                                                            Label2.Text = Request.Url.AbsoluteUri.Replace("WebForm1.aspx", "WebForm4.aspx?exitid=" + exitid);

                                                            MailMessage mm = new MailMessage();
                                                            mm.From = new MailAddress(FromEmail);
                                                            mm.Subject = "Early Exit Permit Pending for Approval - SubCon";
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

                                                            mm.From = new MailAddress(FromEmail);
                                                            SmtpClient smtp = new SmtpClient();
                                                            smtp.Host = "smtp-mail.outlook.com";
                                                            smtp.EnableSsl = true;
                                                            NetworkCredential NetworkCred = new NetworkCredential(FromEmail, EmailPassword);


                                                            string pjmID = "";
                                                            if (!pjmdr.IsDBNull(1))
                                                            {
                                                                pjmID = pjmdr.GetString(1);
                                                                Label1.Text += pjmID;
                                                                //mm.Bcc.Add(new MailAddress(pjmID));
                                                            }

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



                                }

                            }

                        }



                    }

                }

            } catch (Exception ex)
            {
                throw ex;
            }
            
            
        }

        

        protected void Submit(object sender, EventArgs e)
        {
            int counter = 0;
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
                        for (int i = 0; i < namesddl.Items.Count; i++)
                        {
                            if (namesddl.Items[i].Selected)
                            {
                                counter += 1;
                            }
                            if (counter > 1)
                            {
                                TeamSubmit();
                                sendEmailForApproval();
                                Response.Redirect("Webform3.aspx");

                            }
                            else if (counter == 1)
                            {
                                SoloSubmit();
                                sendEmailForApproval();
                                Response.Redirect("Webform3.aspx");
                            }
                        }

                        SoloSubmit();
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

        }

    }
}