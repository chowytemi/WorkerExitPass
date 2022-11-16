﻿using System;
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
            using (SqlCommand cmd = new SqlCommand("select distinct company from EmpList"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                companyddl.DataSource = cmd.ExecuteReader();
                companyddl.DataTextField = "company";
                companyddl.DataBind();
                con.Close();
            }

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
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            var currentdate = DateTime.Now;

            string employeeInput = .Text;
            string companyInput = .Text;

            if (employeeInput != "" || companyInput != "")
            {
                CheckDuplicate();
            } else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showSaveMessage",
                                "<script language='javascript'>alert('Please fill in the fields required');</script>");
                return;
            }
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

                insert.ExecuteNonQuery();
            }

        }

        protected void UpdateCompany()
        {

        }

        protected void GetList()
        {

        }
    }
}