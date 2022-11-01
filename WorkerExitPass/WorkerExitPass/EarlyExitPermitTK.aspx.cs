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
            string cmsstr = ConfigurationManager.ConnectionStrings["cms"].ConnectionString;
            string company = companytb.Text;


            SqlConnection appcon = new SqlConnection(cmsstr);
            appcon.Open();

            SqlConnection con = new SqlConnection(constr);


            if (company != "DMES")
            {
                string sqlquery = "select distinct EmpID, JobCode from EmpList where JobCode IN('SUBCON', 'WK') AND IsActive = 1 AND company = '" + company + "' order by EmpID; ";
                con.Open();
                SqlCommand cmdlineno = new SqlCommand(sqlquery, con);
                SqlDataReader dr = cmdlineno.ExecuteReader();
                while (dr.Read())
                {

                    string employeesCompID = dr[0].ToString();
                    string jobcode = dr[1].ToString();

                    string query = "select distinct EmpID, StartTime ,EndTime from TimeLog where EndTime IS NULL AND CAST(StartTime AS Date) = CAST(GETDATE() AS Date) AND EmpID = '" + employeesCompID + "' order by EmpID; ";


                    using (SqlCommand cmd2 = new SqlCommand(query, appcon))
                    {

                        SqlDataReader timelogdr = cmd2.ExecuteReader();


                        while (timelogdr.Read())
                        {

                            string workersIn = timelogdr[0].ToString();

                            string query2 = "select distinct CONCAT(Employee_Name, ' (', RTRIM(EmpID), ')') AS 'empNameID' from EmpList where JobCode IN('SUBCON', 'WK') AND IsActive = 1 " +
                       "AND company = '" + company + "' AND EmpID = '" + workersIn + "' order by empNameID;";

                            using (SqlCommand namecmd = new SqlCommand(query2, con))
                            {

                                SqlDataReader namedr = namecmd.ExecuteReader();


                                while (namedr.Read())
                                {

                                    string empNameID = namedr[0].ToString();

                                    namesddl.Items.Add(empNameID);
                                    //namesddl.DataBind();

                                }
                            }

                        }


                    }



                }
                con.Close();
            }
            else
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("select CONCAT(Employee_Name, ' (', RTRIM(EmpID), ')') AS 'empNameID' from EmpList where JobCode IN('SUBCON', 'WK') AND IsActive = 1 AND company = '" + company + "' order by EmpID;"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    namesddl.DataSource = cmd.ExecuteReader();
                    namesddl.DataTextField = "empNameID";
                    namesddl.DataBind();
                    con.Close();
                }

            }




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


        //Get data from Login 
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


            //string sqlcheck = "select AC.menu  from UserAccess as UA, Access as AC, EmpList as emp where UA.accessid = AC.ID " +
            //    "and emp.ID = UA.EmpID and UA.IsActive = 1 " +
            //    "and emp.EmpID = '" + empID + "'  and emp.isactive = 1   and AC.Application = 'Service Request' and ac.menu = 'btnexit'";
            //SqlCommand cmdline = new SqlCommand(sqlcheck, con);
            //SqlDataReader drcheck = cmdline.ExecuteReader();
            //if (drcheck.HasRows)
            //{
                //for testing
                //string sql = "select EmpID from EmpList";
                string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + TK + "' and EmpList.EmpID = '" + empID + "' ; ";
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    RetrieveDataFromLogin();
                    BindDataSetDataProjects();
                    GetListOfEmployees();
                    BindDataSetDataReason();
                }
                else
                {

                    Response.Redirect("http://eservices.dyna-mac.com/error");


                }

                dr.Close();
            //}
            //else
            //{

            //    Response.Redirect("http://eservices.dyna-mac.com/error");


            //}

            //drcheck.Close();
            con.Close();


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
                        string sqlinsertapprovequery = "insert into exitapproval(exitID, approve, createdby, createddate, EmpID, company, reason, Remarks, exittime, projectdesc, projcode) values((NEXT VALUE FOR exitID_Sequence), 1, @createdby, @createddate, @EmpID, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";

                        using (SqlCommand insert = new SqlCommand(sqlinsertapprovequery, appcon))
                        {

                            var time = Request["timeInput"];
                            var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;


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
                    else
                    {
                        //insert request
                        string sqlinsertquery = "insert into exitapproval(exitID, createdby, createddate, EmpID, company, reason, Remarks, exittime, projectdesc, projcode) values((NEXT VALUE FOR exitID_Sequence), @createdby, @createddate, @EmpID, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";

                        using (SqlCommand insert = new SqlCommand(sqlinsertquery, appcon))
                        {

                            var time = Request["timeInput"];
                            var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;


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

                }
                conn.Close();
                appcon.Close();
                sendEmailForApproval();
                Response.Redirect("EarlyExitPermitStatus.aspx?exprmitstatus=" + empID);
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

            //try
            //{

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
                            //string empquery = "select EmpID from EmpList where Employee_Name = LEFT('" + namesddl.Items[i].Text + "', CHARINDEX('(', '" + namesddl.Items[i].Text + "') - 1) and IsActive = 1 and Company = '" + companyInput + "';";
                            string empquery = "Select SUBSTRING('" + namesddl.Items[i].Text + "',CHARINDEX('(','" + namesddl.Items[i].Text + "')+1 ,CHARINDEX(')','" + namesddl.Items[i].Text + "')-CHARINDEX('(','" + namesddl.Items[i].Text + "')-1)";
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
                                            insertsinglequery = "insert into exitapproval(exitID, approve, createdby, createddate, EmpID, company, reason, Remarks, exittime, projectdesc, projcode) values((NEXT VALUE FOR exitID_Sequence), 1, @createdby, @createddate, @EmpID, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                                        }
                                        else
                                        {
                                            insertsinglequery = "insert into exitapproval(exitID, createdby, createddate, EmpID, company, reason, Remarks, exittime, projectdesc, projcode) values((NEXT VALUE FOR exitID_Sequence), @createdby, @createddate, @EmpID, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                                        }


                                        string employeeIDToExit = empdr[0].ToString();
                                        //Label1.Text = employeeIDToExit;
                                        using (SqlCommand insert = new SqlCommand(insertsinglequery, appcon))
                                        {

                                            var time = Request["timeInput"];
                                            var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;


                                            insert.CommandType = CommandType.Text;
                                            insert.Parameters.AddWithValue("@createdby", empID);
                                            insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
                                            insert.Parameters.AddWithValue("@EmpID", employeeIDToExit);
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
                                            insertmultiplequery = "insert into exitapproval(exitID, approve, createdby, createddate, EmpID, company, reason, Remarks, exittime, projectdesc, projcode) values(CONVERT(int, (SELECT current_value FROM sys.sequences WHERE name = 'exitID_Sequence')), 1, @createdby, @createddate, @EmpID, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                                        }
                                        else
                                        {
                                            insertmultiplequery = "insert into exitapproval(exitID, createdby, createddate, EmpID, company, reason, Remarks, exittime, projectdesc, projcode) values(CONVERT(int, (SELECT current_value FROM sys.sequences WHERE name = 'exitID_Sequence')), @createdby, @createddate, @EmpID, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";
                                        }


                                        string employeeIDToExit = empdr[0].ToString();
                                        using (SqlCommand insert = new SqlCommand(insertmultiplequery, appcon))
                                        {

                                            var time = Request["timeInput"];
                                            var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;


                                            insert.CommandType = CommandType.Text;
                                            insert.Parameters.AddWithValue("@createdby", empID);
                                            insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
                                            insert.Parameters.AddWithValue("@EmpID", employeeIDToExit);
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

            conn.Close();
            appcon.Close();
            sendEmailForApproval();
            Response.Redirect("EarlyExitPermitStatus.aspx?exprmitstatus=" + empID);

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}


        }

        protected void sendEmailForApproval()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            string PJM = ConfigurationManager.AppSettings["PJM"].ToString();
            //string Test = ConfigurationManager.AppSettings["Test"].ToString();
            string RO = ConfigurationManager.AppSettings["RO"].ToString();
            string MailFrom = ConfigurationManager.AppSettings["MailFrom"].ToString();
            string MailTo = ConfigurationManager.AppSettings["MailTo"].ToString();
            //string EmailPassword = ConfigurationManager.AppSettings["Password"].ToString();
            string smtpserver = ConfigurationManager.AppSettings["smtpserver"].ToString();
            string smtport = ConfigurationManager.AppSettings["smtport"].ToString();
            int smtpport = Convert.ToInt32(smtport);
            string link = ConfigurationManager.AppSettings["link"].ToString();
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
                                //string exitquery = "select distinct exitID from exitapproval where createdby = @empID and company = @company and exittime = @time";
                                string exitquery = "select distinct exitID, projectdesc, reason, exittime from exitapproval where createdby = @empID and company = @company and exittime = @time;";
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
                                            string project = exitdr[1].ToString();
                                            string reason = exitdr[2].ToString();
                                            DateTime exittime = Convert.ToDateTime(exitdr[3].ToString());
                                            string exittime1 = exittime.ToString("dd/MM/yyyy hh:mm tt");
                                            //string query3 = "select EmpList.Employee_Name, exitapproval.exittime, exitapproval.reason from EmpList, exitapproval where EmpList.EmpID =  exitapproval.toexit and exitapproval.exitID= '" + exitid + "';";
                                            string query3 = "select CONCAT(RTRIM(EmpList.EmpID), ' - ' , EmpList.Employee_Name) from EmpList, exitapproval where exitapproval.exitID = '" + exitid + "' and EmpList.EmpID = exitapproval.EmpID;";

                                            using (SqlCommand cmd3 = new SqlCommand(query3, conn))
                                            {
                                                SqlDataAdapter da = new SqlDataAdapter(query3, conn);
                                                DataSet ds = new DataSet();
                                                da.Fill(ds);
                                                DataTable dt = ds.Tables[0];

                                                string exitNames = "";

                                                body += "<br /><br /><table style=\"table-layout: fixed; text-align:left; border-collapse: collapse; border: 1px solid; width: 70%;\">";
                                                body += "<tr style=\" height: 0.5em;\">";
                                                body += "<th style=\" color: #004B7A; text-align: left; border: 1px solid\">Exit ID</th>";
                                                body += "<td style=\" border: 1px solid\">" + exitid + "</td></tr>";
                                                body += "<tr style=\" height: 0.5em;\">";
                                                body += "<th style=\" color: #004B7A; text-align: left; border: 1px solid\">Project</th>";
                                                body += "<td style=\" border: 1px solid\">" + project + "</td></tr>";
                                                body += "<tr style=\" height: 0.5em;\">";
                                                body += "<th style=\" color: #004B7A; text-align: left; border: 1px solid\">Reason</th>";
                                                body += "<td style=\" border: 1px solid\">" + reason + "</td></tr>";
                                                body += "<tr style=\" height: 0.5em;\">";
                                                body += "<th style=\" color: #004B7A; text-align: left; border: 1px solid\">Exit Time</th>";
                                                body += "<td style=\" border: 1px solid\">" + exittime1 + "</td></tr>";
                                                body += "<tr style=\" height: 0.5em;\">";
                                                body += "<th style=\"color: #004B7A; text-align: left; border: 1px solid\">Employee Name(s)</th>";
                                                for (int i = 0; i < dt.Rows.Count; i++)
                                                {
                                                    exitNames += dt.Rows[i][0].ToString() + "<br />";

                                                }
                                                body += "<td style=\" border: 1px solid\">" + exitNames + "</td></tr>";
                                                body += "<tr style=\" height: 0.5em;\">";
                                                body += "<th style=\" color: #004B7A; text-align: left; border: 1px solid\">Created By</th>";
                                                body += "<td style=\" border: 1px solid\">" + createdby + "</td></tr></table>";

                                            }


                                            //check if worker or subcon
                                            if (dr[2].ToString() == "WK")
                                            {
                                                if (!string.IsNullOrEmpty(dr[5].ToString()))
                                                {
                                                    //worker - email to HOD
                                                    //string ROname = dr[5].ToString();
                                                    //string hodquery = "select cemail from EmpList where EmpID='" + ROname + "' and isActive = 1";
                                                    //for testing
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
                                                                string body1 = "";
                                                                string ROid = hoddr[0].ToString();
                                                                string ROcemail = hoddr[1].ToString();

                                                                MailMessage mm = new MailMessage();
                                                                mm.From = new MailAddress(MailFrom);
                                                                if (ReasonDropdown.Text == "Medical Injury")
                                                                {

                                                                    mm.Subject = "Early Exit Permit Medical Injury Notification";

                                                                }
                                                                else
                                                                {
                                                                    mm.Subject = "Early Exit Permit Pending RO for Approval";
                                                                    body1 += "<br />Please click <a href = '" + link + "EarlyExitPermitView.aspx?approval=" + ROid + "'>here</a> to approve or reject the application.";

                                                                    //body += "<br />Please click <a href = '" + Request.Url.AbsoluteUri.Replace("EarlyExitPermitTK.aspx?exprmit=" + empID, "EarlyExitPermitView.aspx?approval=" + ROid) + "'>here</a> to approve or reject the application.";
                                                                    //body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("EarlyExitPermitTK.aspx?exprmit=" + empID, "EarlyExitPermitApproval.aspx?exitid=" + exitid + "&approver=" + ROid + "&status=1") + "'>Approve</a>";
                                                                    //body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("EarlyExitPermitTK.aspx?exprmit=" + empID, "EarlyExitPermitApproval.aspx?exitid=" + exitid + "&approver=" + ROid + "&status=0") + "'>Reject</a>";
                                                                    //body += "<br /><br /><a href = '" + Request.Url.AbsoluteUri.Replace("EarlyExitPermitTK.aspx?exprmit=" + empID, "EarlyExitPermitApproval.aspx?exitid=" + exitid + "&approver=" + ROid + "&status=1") + "'>Approve this application</a>" + " or " + "<a href = '" + Request.Url.AbsoluteUri.Replace("EarlyExitPermitTK.aspx?exprmit=" + empID, "EarlyExitPermitApproval.aspx?exitid=" + exitid + "&approver=" + ROid + "&status=0") + "'>Reject this application</a>";

                                                                }


                                                                body1 += "<br /><br />This is an automatically generated email, please do not reply.";

                                                                //mm.Body = body;
                                                                mm.Body = body + body1;
                                                                mm.IsBodyHtml = true;
                                                                mm.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"].ToString());
                                                                mm.To.Add(new MailAddress(ROcemail));
                                                                SmtpClient smtp = new SmtpClient(smtpserver, smtpport);
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

                                                //for testing
                                                string pjmquery = "select distinct EmpList.EmpID,EmpList.CEmail " +
                                                                          "from Access, UserAccess, ARole, EmpList " +
                                                                          "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                                                          "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                                                          "and Access.id = '" + PJM + "'";


                                                using (SqlCommand pjmcmd = new SqlCommand(pjmquery, conn))
                                                {

                                                    using (SqlDataReader pjmdr = pjmcmd.ExecuteReader())
                                                    {
                                                        while (pjmdr.Read())
                                                        {
                                                            string body1 = "";
                                                            string name = pjmdr[0].ToString();

                                                            MailMessage mm = new MailMessage();
                                                            mm.From = new MailAddress(MailFrom);
                                                            if (ReasonDropdown.Text == "Medical Injury")
                                                            {

                                                                mm.Subject = "Early Exit Permit Medical Injury Notification";

                                                            }
                                                            else
                                                            {
                                                                mm.Subject = "Early Exit Permit Pending PJM for Approval";
                                                                body1 += "<br />Please click <a href = '" + link + "EarlyExitPermitView.aspx?approval=" + name + "'>here</a> to approve or reject the application.";

                                                            }
                                                            body1 += "<br /><br />This is an automatically generated email, please do not reply.";

                                                            mm.Body = body + body1;
                                                            mm.IsBodyHtml = true;
                                                            SmtpClient smtp = new SmtpClient(smtpserver, smtpport); //Gmail smtp                                                                        
                                                            smtp.EnableSsl = false;

                                                            string pjmID = "";
                                                            if (!pjmdr.IsDBNull(0))
                                                            {
                                                                pjmID = pjmdr.GetString(1);
                                                                mm.To.Add(new MailAddress(pjmID));

                                                            }


                                                            smtp.UseDefaultCredentials = false;
                                                            mm.To.Add(new MailAddress(MailTo));
                                                            smtp.Send(mm);
                                                        }
                                                    }

                                                }

                                            }
                                            else //for testing 
                                            {

                                                string pjmquery = "select distinct EmpList.EmpID,EmpList.CEmail " +
                                                                          "from Access, UserAccess, ARole, EmpList " +
                                                                          "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                                                          "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                                                          "and Access.id = '" + RO + "'";


                                                using (SqlCommand pjmcmd = new SqlCommand(pjmquery, conn))
                                                {

                                                    using (SqlDataReader pjmdr = pjmcmd.ExecuteReader())
                                                    {
                                                        while (pjmdr.Read())
                                                        {
                                                            string body1 = "";
                                                            string name = pjmdr[0].ToString();

                                                            MailMessage mm = new MailMessage();
                                                            mm.From = new MailAddress(MailFrom);
                                                            if (ReasonDropdown.Text == "Medical Injury")
                                                            {

                                                                mm.Subject = "Early Exit Permit Medical Injury Notification";

                                                            }
                                                            else
                                                            {
                                                                mm.Subject = "Early Exit Permit Pending Test for Approval";
                                                                body1 += "<br />Please click <a href = '" + link + "EarlyExitPermitView.aspx?approval=" + name + "'>here</a> to approve or reject the application.";

                                                            }
                                                            body1 += "<br /><br />This is an automatically generated email, please do not reply.";

                                                            mm.Body = body + body1;
                                                            mm.IsBodyHtml = true;
                                                            SmtpClient smtp = new SmtpClient(smtpserver, smtpport); //Gmail smtp                                                                        
                                                            smtp.EnableSsl = false;

                                                            string pjmID = "";
                                                            if (!pjmdr.IsDBNull(0))
                                                            {
                                                                pjmID = pjmdr.GetString(1);
                                                                mm.To.Add(new MailAddress(pjmID));
                                                            }

                                                            smtp.UseDefaultCredentials = false;
                                                            mm.To.Add(new MailAddress(MailTo));
                                                            smtp.Send(mm);
                                                        }
                                                    }

                                                }

                                            }
                                            //sends one email, url need to log in
                                            //else if (dr[2].ToString() == "SUBCON")
                                            //{
                                            //    //subcon - email to project managers

                                            //    //string pjmquery = "select distinct EmpList.EmpID,EmpList.CEmail " +
                                            //    //                  "from Access, UserAccess, ARole, EmpList " +
                                            //    //                  "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                            //    //                  "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                            //    //                  "and Access.id = '" + Test
                                            //    //                  //+ "'";
                                            //    //                  + "' and EmpList.EmpID = 'T202' OR EmpList.EmpID = 'T203'";

                                            //    //for testing
                                            //    string pjmquery = "select distinct EmpList.EmpID,EmpList.CEmail " +
                                            //                              "from Access, UserAccess, ARole, EmpList " +
                                            //                              "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                            //                              "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                                            //                              "and Access.id = '" + PJM + "'";


                                            //    using (SqlCommand pjmcmd = new SqlCommand(pjmquery, conn))
                                            //    {

                                            //        SqlDataAdapter da = new SqlDataAdapter(pjmquery, conn);
                                            //        DataSet ds = new DataSet();
                                            //        da.Fill(ds);
                                            //        DataTable dt = ds.Tables[0];

                                            //        string body1 = "";
                                            //        //string name = pjmdr[0].ToString();
                                            //        string name = dt.Rows[0].ToString();
                                            //                MailMessage mm = new MailMessage();
                                            //                mm.From = new MailAddress(MailFrom);
                                            //                if (ReasonDropdown.Text == "Medical Injury")
                                            //                {

                                            //                    mm.Subject = "Early Exit Permit Medical Injury Notification";

                                            //                }
                                            //                else
                                            //                {
                                            //                    mm.Subject = "Early Exit Permit Pending PJM for Approval";
                                            //                    body1 += "<br />Please click <a href = '" + link + "EarlyExitPermitApproval.aspx?exitid=" + exitid + "'>here</a> to approve or reject the application.";

                                            //                }
                                            //                body1 += "<br /><br />This is an automatically generated email, please do not reply.";

                                            //                mm.Body = body + body1;
                                            //                mm.IsBodyHtml = true;
                                            //                SmtpClient smtp = new SmtpClient(smtpserver, smtpport); //Gmail smtp                                                                        
                                            //                smtp.EnableSsl = false;
                                            //        for (int i = 0; i < dt.Rows.Count; i++)
                                            //        {
                                            //            string pjmID = dt.Rows[i][1].ToString();
                                            //            mm.To.Add(new MailAddress(pjmID));
                                            //        }


                                            //            mm.To.Add(new MailAddress(MailTo));

                                            //                smtp.UseDefaultCredentials = false;

                                            //                smtp.Send(mm);
                                            //        //    }
                                            //        //}

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
            catch (Exception ex)
            {
                throw ex;
            }


        }



        protected void CheckSubmissionSolo()
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
                string sqlquerycheck = "select exitid from exitapproval where cast('" + dateInput + "' as time) > cast(exittime as time) and empID = '" + empID + "' and (approve = 1 or approve is not null)  ";

                SqlCommand cmdlinenocheck = new SqlCommand(sqlquerycheck, appcon);
                SqlDataReader drcheck = cmdlinenocheck.ExecuteReader();

                if (!drcheck.HasRows)
                {
                    SoloSubmit();
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

        protected void CheckSubmissionTeam()
        {
            var time = Request["timeInput"];
            var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;
            string companyInput = companytb.Text;


            string empID = Session["empID"].ToString();
            Session["empID"] = empID;



            //Connect to database

            string connectionstring = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection appcon = new SqlConnection(connectionstring);
            appcon.Open();

            for (int i = 0; i < namesddl.Items.Count; i++)
            {
                if (namesddl.Items[i].Selected)
                {
                    //get EmpID
                    string empquery = "select EmpID from EmpList where Employee_Name = LEFT('" + namesddl.Items[i].Text + "', CHARINDEX('(', '" + namesddl.Items[i].Text + "') - 1) and IsActive = 1 and Company = '" + companyInput + "';";
                    SqlCommand empcmd = new SqlCommand(empquery, appcon);
                    using (SqlDataReader empdr = empcmd.ExecuteReader())
                    {
                        while (empdr.Read())
                        {

                            string checkEmpID = empdr[0].ToString();
                            //check for duplicate
                            string sqlquery1 = "select exitID from exitapproval where  CAST(createddate AS Date ) = CAST(GETDATE() AS Date ) and empID = '" + checkEmpID + "' and exittime = '" + dateInput + "';";

                            SqlCommand cmd1 = new SqlCommand(sqlquery1, appcon);
                            SqlDataReader dr1 = cmd1.ExecuteReader();

                            if (!dr1.HasRows)
                            {
                                //check for submit time after approved time
                                string sqlquerycheck = "select exitid from exitapproval where cast('" + dateInput + "' as time) > cast(exittime as time) and empID = '" + checkEmpID + "' and (approve = 1 or approve is not null)  ";

                                SqlCommand cmdlinenocheck = new SqlCommand(sqlquerycheck, appcon);
                                SqlDataReader drcheck = cmdlinenocheck.ExecuteReader();

                                if (!drcheck.HasRows)
                                {
                                    TeamSubmit();
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
                    }
                }
            }



        }



        protected void SubmitAsTeam_Click(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            int counter = 0;
            //try
            //{
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

                        if (ReasonDropdown.SelectedValue == "Select")
                        {
                            ScriptManager.RegisterClientScriptBlock
                              (this, this.GetType(), "alertMessage", "alert" +
                              "('Please choose a valid reason')", true);
                            return;
                        }
                        else
                        {
                            CheckSubmissionTeam();
                            //TeamSubmit();
                            //sendEmailForApproval();
                            //Response.Redirect("EarlyExitPermitStatus.aspx?exprmitstatus=" + empID);
                        }

                    }
                    else if (counter == 0)
                    {
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

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
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
                        if (ReasonDropdown.SelectedValue == "Select")
                        {
                            ScriptManager.RegisterClientScriptBlock
                              (this, this.GetType(), "alertMessage", "alert" +
                              "('Please choose a valid reason')", true);
                            return;
                        }
                        else
                        {
                            CheckSubmissionSolo();
                            //SoloSubmit();
                            //sendEmailForApproval();
                            //Response.Redirect("EarlyExitPermitStatus.aspx?exprmitstatus=" + empID);
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




    }


}