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

        string empID = "MB638";
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //SoloBtn.Attributes.Add("class", "activeBtn");
            //}
            if (!IsPostBack)
            {
                BindDataSetDataProjects();
                RetrieveDataFromLogin();
            }

        }

        protected void SoloBtn_Click(object sender, EventArgs e)
        {
            namesddl.Visible = false;
            nametb.Visible = true;
            //SoloBtn.Attributes.Add("class", "activeBtn");
        }

        protected void TeamBtn_Click(object sender, EventArgs e)
        {
            namesddl.Visible = true;
            nametb.Visible = false;
            //TeamBtn.Attributes.Add("class", "activeBtn");
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
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("select Employee_Name, EmpID from EmpList where Department = 'SUBCON' AND IsActive = 1;"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    namesddl.DataSource = cmd.ExecuteReader();
                    namesddl.DataTextField = "Employee_Name";
                    namesddl.DataValueField = "EmpID";
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
            List<String> empNamesList = new List<string>();
            foreach (ListItem item in namesddl.Items)
            {
                if (item.Selected)
                {
                    empNamesList.Add(item.Text);
                    empNamesList.Add(item.Value);
                }
            }
            namesddl.Texts.SelectBoxCaption = String.Join(", ", empNamesList.ToArray());
            namesddl.DataValueField = String.Join(", ", empNamesList.ToArray());

        }

        protected void GetSelected()
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
            using (SqlDataReader dr = cmdlineno.ExecuteReader())
            {
                while (dr.Read())
                {
                    string projectcode = dr[0].ToString();

                    //insert request
                    string sqlinsertquery = "insert into exitapproval(createdby, createddate, toexit, company, reason, Remarks, exittime, projectdesc, projcode) values( @createdby, @createddate, @toexit, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode);";

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
                                    string employeeIDToExit = empdr[0].ToString();
                                    using (SqlCommand insert = new SqlCommand(sqlinsertquery, appcon))
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
                                    }
                                }
                            }

                        }

                      

                    }
                }



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

                            string exitquery = "select exitID from exitapproval where createdby = @empID and company = @company and exittime = @time";
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
                                                            string ROcemail = "chowytemi07.20@ichat.sp.edu.sg";
                                                            //Label2.Text = Request.Url.AbsoluteUri.Replace("WebForm1.aspx", "WebForm4.aspx?exitid=" + exitid);

                                                            using (MailMessage mm = new MailMessage("@outlook.com", ROcemail))
                                                            {
                                                                mm.Subject = "Account Activation";
                                                                string body = "Hello,";
                                                                body += "<br /><br />Please click the following link to approve or reject the application";
                                                                body += "<br /><a href = '" + Request.Url.AbsoluteUri.Replace("WebForm1.aspx", "WebForm4.aspx?exitid=" + exitid) + "'>Click here to approve or deny applications.</a>";
                                                                body += "<br /><br />Thanks";
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

                                        }


                                    }
                                }



                            }

                        }

                    }



                }

            }


        }


        protected void Submit(object sender, EventArgs e)
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
                        GetSelected();
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