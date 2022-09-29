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
    public partial class WebForm1 : System.Web.UI.Page
    {

        //Get login id
        //string empID = "MB638";
        string empID = "PXE6563";


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

            } else
            {
                lblremarks.Visible = false;
                remarkstb.Visible = false;
            }
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            //approveForm();
            //formStatus();
            //submitForm();
            //inCharge();
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
                nametb.Text = dr[4].ToString() + "-" + dr[3].ToString();
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
                } else if(compare <= 0)
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
                

                string sqlquery = " select code from PROJECT where description = '" + description +"' and IsActive = 1";
                SqlCommand cmdlineno = new SqlCommand(sqlquery, conn);
                SqlDataReader dr = cmdlineno.ExecuteReader();
                while (dr.Read())
                {
                    string projectcode = dr[0].ToString();
                    Label1.Text = dr[0].ToString();
                }
                dr.Close();
                conn.Close();

                string connectionstring = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
                SqlConnection appcon = new SqlConnection(connectionstring);
                appcon.Open();
                string sqlinsertquery = "insert into exitapproval(createdby, createddate, company, reason, Remarks, exittime, projectdesc, projcode) values( @createdby, @createddate, @company, @reason, @Remarks, @exittime, @projectdesc, @projectcode)";

                using ( SqlCommand insert = new SqlCommand(sqlinsertquery, appcon))
                {

                    string data = nametb.Text;
                    string[] splitData = data.Split('-');
                    string name = splitData[0];
                    string empID = splitData[1];
                    var time = Request["timeInput"];
                    var dateInput = DateTime.Now.ToString("yyyy-MM-dd ") + time;



                    insert.CommandType = CommandType.Text;
                    insert.Parameters.AddWithValue("@createdby", empID);
                    insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
                    insert.Parameters.AddWithValue("@company", HttpUtility.HtmlDecode(companytb.Text));
                    insert.Parameters.AddWithValue("@reason", HttpUtility.HtmlDecode(ReasonDropdown.Text));
                    insert.Parameters.AddWithValue("@Remarks", HttpUtility.HtmlDecode(remarkstb.Text));
                    insert.Parameters.AddWithValue("@exittime", dateInput);
                    insert.Parameters.AddWithValue("@projectdesc", projectInput);
                    insert.Parameters.AddWithValue("@projectcode", "P102115000");

                    insert.ExecuteNonQuery();
                }

                appcon.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        //get in charge to send email to 
        protected void inCharge()
        {
            //Connect to database
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();

            string sqlquery = "select EmpID, Employee_Name, JobCode, Department, designation, RO from EmpList where EmpID = '" + empID +"' and isActive = 1";
            SqlCommand cmdlineno = new SqlCommand(sqlquery, conn);
            SqlDataReader dr = cmdlineno.ExecuteReader();
            while (dr.Read())
            {
                //check if worker or subcon
                if (dr[2].ToString() == "WK")
                {
                    if (!string.IsNullOrEmpty(dr[5].ToString()))
                    {
                        //worker - email to HOD
                        string ROname = dr[5].ToString();
                        Label2.Text = ROname;
                    }
                } else if(dr[2].ToString() == "SUBCON")
                {
                    //subcon - email to project managers
                    Label2.Text = "subcon";
                }
               
               
            }
            dr.Close();
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
            //string datetime = DateTime.Now.ToString();

            string approverID = "T202";
            DateTime approveddate = DateTime.Now;
            int exitID = 10;
            int approve = 1;

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string sqlquery = "update exitapproval set approver = '" + approverID + "', approve = " + approve + ", approveddate = '" + approveddate +"' where exitID = '" + exitID + "'";

            using (SqlCommand update = new SqlCommand(sqlquery, conn))
            {
                update.ExecuteNonQuery();

                conn.Close();
            }
        }

        protected void formStatus()
        {
            //Connect to database
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string statussql = "select createddate, exittime, approve from exitapproval where createdby = '" + empID + "'";
            SqlDataAdapter da = new SqlDataAdapter(statussql, conn);
            using (DataTable dt = new DataTable())
            {
                da.Fill(dt);
                //GridView1.DataSource = dt;
                //GridView1.DataBind();

            //if approve == null, pending

            }
        }
    }
}