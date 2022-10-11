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

        protected void Submit(object sender, EventArgs e)
        {

            string projectInput = projectddl.Text;
            //string empNames = namesddl.Texts.SelectBoxCaption;
            string empID = namesddl.DataValueField;

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();


        }

    }
}