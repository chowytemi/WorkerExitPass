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
    public partial class ExitCompany : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindDataSetDataCompany();
        }

        protected void BindDataSetDataCompany()
        {
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            using (SqlCommand cmd = new SqlCommand("select distinct company from EmpList WHERE isActive = 1"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                companyddl.DataSource = cmd.ExecuteReader();
                companyddl.DataTextField = "company";
                companyddl.DataBind();
                con.Close();
            }

        }
        protected void createBtn_Click(object sender, EventArgs e)
        {
            updateBtn.CssClass = updateBtn.CssClass.Replace("activeBtn", "inactiveBtn");
            createBtn.CssClass = createBtn.CssClass.Replace("inactiveBtn", "activeBtn");

            CreateNew();
        }

        protected void updateBtn_Click(object sender, EventArgs e)
        {

            createBtn.CssClass = createBtn.CssClass.Replace("activeBtn", "inactiveBtn");
            updateBtn.CssClass = updateBtn.CssClass.Replace("inactiveBtn", "activeBtn");

        }
        protected void companyddl_SelectedIndexChanged(object sender, EventArgs e)
        {

            int companyCount = 0;
            for (int i = 0; i < companyddl.Items.Count; i++)
            {
                if (companyddl.Items[i].Selected)
                {
                    companyCount++;
                    companyddl.SelectedItem.Selected = true;

                }
            }
            companyddl.Texts.SelectBoxCaption = companyCount + " selected";
        }


        protected void ClickSubmit()
        {
            //string empID = Session["empID"].ToString();
            //Session["empID"] = empID;

            //var currentdate = DateTime.Now;

            //string employeeInput = .Text;
            //string companyInput = .Text;

            //if (employeeInput != "" || companyInput != "")
            //{
            //    CheckDuplicate();
            //}
            //else
            //{
            //    Page.ClientScript.RegisterStartupScript(this.GetType(), "showSaveMessage",
            //                    "<script language='javascript'>alert('Please fill in the fields required');</script>");
            //    return;
            //}
        }

        protected void CheckDuplicate()
        {
            //string empID = Session["empID"].ToString();
            //Session["empID"] = empID;

            ////Connect to database

            //string connectionstring = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            //SqlConnection appcon = new SqlConnection(connectionstring);
            //appcon.Open();

            ////check for duplicate
            //string sqlquery1 = "select exitID from exitapproval where  CAST(createddate AS Date ) = CAST(GETDATE() AS Date ) and empID = '" + empID + "' and exittime = '" + dateInput + "';";

            //SqlCommand cmd1 = new SqlCommand(sqlquery1, appcon);
            //SqlDataReader dr1 = cmd1.ExecuteReader();

            //if (!dr1.HasRows)
            //{
            //    CreateNew();
            //}
            //else
            //{
            //    Page.ClientScript.RegisterStartupScript(this.GetType(), "showSaveMessage",
            //                    "<script language='javascript'>alert('Duplicate Submission');</script>");
            //    return;
            //}
            //return;
        }
        protected void CreateNew()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            string connectionstring = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection appcon = new SqlConnection(connectionstring);
            appcon.Open();

            string employeeInput = lblEmpID.ToString();

            for (int i = 0; i < companyddl.Items.Count; i++)
            {
                if (companyddl.Items[i].Selected)
                {
                    string sqlinsertquery = "INSERT INTO exitCompany(EmpID, Company, IsActive, CreatedBy, CreatedDate) values(@employee, @company, '1', @createdby, @createddate);";

                    using (SqlCommand insert = new SqlCommand(sqlinsertquery, appcon))
                    {


                        insert.CommandType = CommandType.Text;
                        insert.Parameters.AddWithValue("@createdby", empID);
                        insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
                        insert.Parameters.AddWithValue("@employee", employeeInput);
                        insert.Parameters.AddWithValue("@company", HttpUtility.HtmlDecode(companyddl.ToString()));

                        insert.ExecuteNonQuery();
                    }
                }
            }
            appcon.Close();



        }

        protected void UpdateCompany()
        {
            //string empID = Session["empID"].ToString();
            //Session["empID"] = empID;

            //string connectionstring = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            //SqlConnection appcon = new SqlConnection(connectionstring);
            //appcon.Open();

            //string employeeInput = lblEmpID.ToString();

            //for (int i = 0; i < companyddl.Items.Count; i++)
            //{
            //    if (companyddl.Items[i].Selected)
            //    {
            //        string sqlupdatequery = "update exitCompany set Company = @company, IsActive = @isactive, UpdateBy = @updateby, UpdateDate = @updatedate where ID = ''";

            //        using (SqlCommand update = new SqlCommand(sqlupdatequery, appcon))
            //        {


            //            update.CommandType = CommandType.Text;
            //            update.Parameters.AddWithValue("@updateby", empID);
            //            update.Parameters.AddWithValue("@updatedate", DateTime.Now.ToString());
            //            update.Parameters.AddWithValue("@employee", employeeInput);
            //            update.Parameters.AddWithValue("@isactive", isactive);
            //            update.Parameters.AddWithValue("@company", HttpUtility.HtmlDecode(companyddl.ToString()));

            //            update.ExecuteNonQuery();
            //        }
            //    }
            //}
            //appcon.Close();
        }

        protected void GetList()
        {

        }
    }
}