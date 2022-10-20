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
                //SoloBtn.Attributes.Add("class", "activeBtn");
            }

        }

        protected void SoloBtn_Click(object sender, EventArgs e)
        {
            namesddl.Visible = false;
            nametb.Visible = true;
            submitAsTeam.Visible = false;
            submitAsSolo.Visible = true;
            TeamBtn.CssClass = TeamBtn.CssClass.Replace("activeBtn", "submitAsButton");
            SoloBtn.CssClass = SoloBtn.CssClass.Replace("submitAsButton", "activeBtn");
        }

        protected void TeamBtn_Click(object sender, EventArgs e)
        {
            namesddl.Visible = true;
            nametb.Visible = false;
            submitAsTeam.Visible = true;
            submitAsSolo.Visible = false;
            GetListOfEmployees();
            SoloBtn.CssClass = SoloBtn.CssClass.Replace("activeBtn", "submitAsButton");
            TeamBtn.CssClass = SoloBtn.CssClass.Replace("submitAsButton", "activeBtn");

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
                using (SqlCommand cmd = new SqlCommand("select CONCAT(Employee_Name, ' (', RTRIM(EmpID), ')') AS 'empNameID' from EmpList where JobCode IN('SUBCON', 'WK') AND IsActive = 1 AND company = '" + company + "' order by EmpID;"))
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
            //using test access 87, tk access 85
            string TK = ConfigurationManager.AppSettings["TK"].ToString();
            //string Test = ConfigurationManager.AppSettings["Test"].ToString();
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + TK + "' and EmpList.EmpID = '" + empID + "' ; ";
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
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        protected void namesddl_SelectedIndexChanged(object sender, EventArgs e)
        {

            int empCount = 0;
            for (int i = 0; i < namesddl.Items.Count; i++)
            {
                if (namesddl.Items[i].Selected)
                {
                    empCount++;
                    namesddl.SelectedItem.Selected = true;

                }
            }
            namesddl.Texts.SelectBoxCaption = empCount + " selected";
        }

        protected void SoloSubmit()
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

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        protected void TeamSubmit()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            string description = projectddl.Text;
            string projectInput = projectddl.Text;
            string companyInput = companytb.Text;
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
                                namesddl.Items[i].Text.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);

                                //get EmpID
                                //string empquery = " select empID from EmpList where Employee_Name = '" + namesddl.Items[i].Text + "' and IsActive = 1";
                                string empquery = "select EmpID from EmpList where Employee_Name = LEFT('" + namesddl.Items[i].Text + "', CHARINDEX('(', '" + namesddl.Items[i].Text + "') - 1) and IsActive = 1 and Company = '" + companyInput +"';";
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
                                            }
                                            else
                                            {
                                                insertsinglequery = "insert into exitapproval(exitID, createdby, createddate, toexit, company, reason, Remarks, exittime, projectdesc, projcode) values((NEXT VALUE FOR exitID_Sequence), @createdby, @createddate, @toexit, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                                            }


                                            string employeeIDToExit = empdr[0].ToString();
                                            Label1.Text = employeeIDToExit;
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

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        protected void sendEmailForApproval()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            //string PJM = ConfigurationManager.AppSettings["PJM"].ToString();
            string Test = ConfigurationManager.AppSettings["Test"].ToString();
            string RO = ConfigurationManager.AppSettings["RO"].ToString();
            string MailFrom = ConfigurationManager.AppSettings["MailFrom"].ToString();
            //string EmailPassword = ConfigurationManager.AppSettings["Password"].ToString();
            string smtpserver = ConfigurationManager.AppSettings["smtpserver"].ToString();
            string smtport = ConfigurationManager.AppSettings["smtport"].ToString();
            int smtpport = Convert.ToInt32(smtport);

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
                                string createdby = dr[1].ToString();
                                //get exitid
                                string exitquery = "select distinct exitID from exitapproval where createdby = @empID and company = @company and exittime = @time";

                                using (SqlCommand exitcmd = new SqlCommand(exitquery, conn))
                                {
                                    exitcmd.Parameters.AddWithValue("@empID", empID);
                                    exitcmd.Parameters.AddWithValue("@company", companytb.Text);
                                    exitcmd.Parameters.AddWithValue("@time", dateInput);
                                    using (SqlDataReader exitdr = exitcmd.ExecuteReader())
                                    {

                                        while (exitdr.Read())
                                        {
                                            string body = "";
                                            body += "Hello,";
                                            body += "<br /><br />The following application was submitted:";
                                            
                                            string exitid = exitdr[0].ToString();

                                            string query3 = "select EmpList.Employee_Name, exitapproval.exittime, exitapproval.reason from EmpList, exitapproval where EmpList.EmpID =  exitapproval.toexit and exitapproval.exitID= '" + exitid + "';";

                                            using (SqlCommand cmd3 = new SqlCommand(query3, conn))
                                            {

                                                using (SqlDataReader dr3 = cmd3.ExecuteReader())
                                                {

                                                    while (dr3.Read())
                                                    {
                                                       
                                                        //DateTime date = Convert.ToDateTime(dr[1]);
                                                        string exittime = dr3[1].ToString();
                                                        string reason = dr3[2].ToString();
                                                        DataTable schemaTable = dr3.GetSchemaTable();
                                                        string exitName = dr3[0].ToString();

                                                                                                              

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
                                                        body += "<td style=\" border: 1px solid\">" + exitName + "</td>";
                                                        body += "<td style=\" border: 1px solid\">" + exittime + "</td>";
                                                        body += "<td style=\" border: 1px solid\">" + reason + "</td></tr></table>";
                                                        
                                                    }
                                                }
                                            }


                                            //check if worker or subcon
                                            if (dr[2].ToString() == "WK")
                                            {
                                                if (!string.IsNullOrEmpty(dr[5].ToString()))
                                                {
                                                     //worker - email to HOD
                                                    //string ROname = dr[5].ToString();
                                                    //body += "<br />Please click the following link to approve or reject the application:";
                                                    //body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("WebForm2.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + ROname) + "'>View Application</a>";
                                                    //body += "<br />Please click <a href = '" + Request.Url.AbsoluteUri.Replace("WebForm1.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + ROname) + "'>here</a> to approve or reject the application:";
                                                    //body += "<br /><br />Thank you";
                                                    //string hodquery = "select cemail from EmpList where EmpID='" + ROname + "' and isActive = 1";
                                                    //string hodquery = "select approveremail from testtable";
                                                    string hodquery = "select distinct EmpList.EmpID,EmpList.CEmail " +
                                                                          "from Access, UserAccess, ARole, EmpList " +
                                                                          "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                                                          "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                                                          "and Access.id = '" + RO + "'";


                                                    using (SqlCommand hodcmd = new SqlCommand(hodquery, conn))
                                                     {
                                                           using (SqlDataReader hoddr = hodcmd.ExecuteReader())
                                                           {
                                                                   while (hoddr.Read())
                                                                   {
                                                                       string ROid = hoddr[0].ToString();
                                                                       string ROcemail = hoddr[1].ToString();
                                                                    
                                                                       //body += "<br />Please click <a href = '" + Request.Url.AbsoluteUri.Replace("WebForm2.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + ROid) + "'>here</a> to approve or reject the application:";
                                                                       

                                                                       MailMessage mm = new MailMessage();
                                                                       mm.From = new MailAddress(MailFrom);
                                                                       if (ReasonDropdown.Text == "Medical Injury")
                                                                       {

                                                                            mm.Subject = "Early Exit Permit Medical Injury Notification";

                                                                       } else
                                                                       {
                                                                            mm.Subject = "Early Exit Permit Pending RO for Approval";
                                                                            body += "<br />Please click <a href = '" + Request.Url.AbsoluteUri.Replace("WebForm2.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + ROid) + "'>here</a> to approve or reject the application:";
        
                                                                       }


                                                                       body += "<br /><br />Thank you";
                                                                       mm.Body = body;
                                                                       mm.IsBodyHtml = true;
                                                                       mm.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"].ToString());
                                                                       mm.To.Add(new MailAddress(ROcemail));
                                                                       SmtpClient smtp = new SmtpClient(smtpserver, smtpport); //Gmail smtp
                                                                       //smtp.Host = "smtp-mail.outlook.com";
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

                                                string pjmquery = "select distinct EmpList.EmpID,EmpList.CEmail " +
                                                                  "from Access, UserAccess, ARole, EmpList " +
                                                                  "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                                                  "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                                                  "and Access.id = '" + Test + "' and EmpList.EmpID = 'T202' OR EmpList.EmpID = 'T203'";

                                                //string pjmquery = "select approveremail from testtable";
                                                using (SqlCommand pjmcmd = new SqlCommand(pjmquery, conn))
                                                {
                                                      using (SqlDataReader pjmdr = pjmcmd.ExecuteReader())
                                                      {
                                                            while (pjmdr.Read())
                                                            {
                                                                  string name = pjmdr[0].ToString();
                                                            //body += "<br />Please click the following link to approve or reject the application:";
                                                            //body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("WebForm2.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + name) + "'>View Application</a>";
                                                                  //body += "<br />Please click <a href = '" + Request.Url.AbsoluteUri.Replace("WebForm2.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + name) + "'>here</a> to approve or reject the application:";
                                                                  //body += "<br /><br />Thank you";
                                                                  //Label2.Text = Request.Url.AbsoluteUri.Replace("WebForm1.aspx", "WebForm4.aspx?exitid=" + exitid);

                                                                  MailMessage mm = new MailMessage();
                                                                  mm.From = new MailAddress(MailFrom);
                                                                  if (ReasonDropdown.Text == "Medical Injury")
                                                                  {

                                                                      mm.Subject = "Early Exit Permit Medical Injury Notification";

                                                                  }
                                                                  else
                                                                  {
                                                                      mm.Subject = "Early Exit Permit Pending PJM for Approval";
                                                                      body += "<br />Please click <a href = '" + Request.Url.AbsoluteUri.Replace("WebForm2.aspx?exprmit=" + empID, "WebForm4.aspx?approval=" + name) + "'>here</a> to approve or reject the application:";

                                                                  }
                                                                  //mm.Subject = "Early Exit Permit Pending PJM for Approval";
                                                                  mm.Body = body;
                                                                  mm.IsBodyHtml = true;
                                                                  mm.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"].ToString());
                                                                  SmtpClient smtp = new SmtpClient(smtpserver, smtpport); //Gmail smtp                                                                        
                                                                  smtp.EnableSsl = false;
                                               
                                                                  string pjmID = "";
                                                                  if (!pjmdr.IsDBNull(0))
                                                                  {
                                                                        pjmID = pjmdr.GetString(1);
                                                                        mm.Bcc.Add(new MailAddress(pjmID));
                                                                  }

                                                                  smtp.UseDefaultCredentials = false;
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

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }



        protected void Submit(object sender, EventArgs e)
        {
            //int counter = 0;
            //try
            //{
            //    var time = Request["timeInput"];
            //    var date = DateTime.Now.ToString("yyyy-MM-dd ") + time;
            //    DateTime dateinput = DateTime.Parse(date);
            //    var currentdate = DateTime.Now;
            //    string projectInput = projectddl.Text;
            //    string nameInput = nametb.Text;
            //    string companyInput = companytb.Text;
            //    string reasonInput = ReasonDropdown.Text;
            //    string remarksInput = remarkstb.Text;

            //    if (projectInput != "" || nameInput != "" || companyInput != "")
            //    {
            //        int compare = DateTime.Compare(dateinput, currentdate);
            //        if (compare > 0)
            //        {
            //            for (int i = 0; i < namesddl.Items.Count; i++)
            //            {
            //                if (namesddl.Items[i].Selected)
            //                {
            //                    counter += 1;
            //                }
            //                if (counter > 1)
            //                {
            //                    TeamSubmit();
            //                    sendEmailForApproval();
            //                    Response.Redirect("Webform3.aspx");

            //                }
            //                else if (counter == 1)
            //                {
            //                    SoloSubmit();
            //                    sendEmailForApproval();
            //                    Response.Redirect("Webform3.aspx");
            //                }
            //            }

            //            //SoloSubmit();
            //            //sendEmailForApproval();
            //            //Response.Redirect("Webform3.aspx");
            //        }
            //        else if (compare <= 0)
            //        {
            //            ScriptManager.RegisterClientScriptBlock
            //              (this, this.GetType(), "alertMessage", "alert" +
            //              "('Please choose a time after the current time')", true);
            //            return;
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

        }

        protected void SubmitAsTeam_Click(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
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

                        }

                        if (counter > 0)
                        {
                            TeamSubmit();
                            sendEmailForApproval();
                            Response.Redirect("Webform3.aspx?extprmitstatus=" + empID );

                        } else if (counter == 0){
                            ScriptManager.RegisterClientScriptBlock
                         (this, this.GetType(), "alertMessage", "alert" +
                         "('Please select names of workers')", true);
                            return;
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

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void SubmitAsSolo_Click(object sender, EventArgs e)
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
                        SoloSubmit();
                        sendEmailForApproval();
                        Response.Redirect("Webform3.aspx?extprmitstatus=" + empID);
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