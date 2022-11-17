using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

            if (!IsPostBack)
            {
                if ((Request.QueryString["exitcompany"] != null))
                {

                    string empID = Request.QueryString["exitcompany"];
                    Session["empID"] = empID;

                }
                BindDataSetDataCompany();
            }

        }

        protected void BindDataSetDataCompany()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            using (SqlCommand cmd = new SqlCommand("select distinct Company from EmpList WHERE isActive = 1;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                companyddl.DataSource = cmd.ExecuteReader();
                companyddl.DataTextField = "Company";
                companyddl.DataBind();
                con.Close();
            }

        }
        protected void createBtn_Click(object sender, EventArgs e)
        {
            updateDetailsBtn.CssClass = updateDetailsBtn.CssClass.Replace("activeBtn", "inactiveBtn");
            createBtn.CssClass = createBtn.CssClass.Replace("inactiveBtn", "activeBtn");
            Panel1.Visible = true;
            Panel2.Visible = false;

            //CreateNew();
        }

        protected void updateDetailsBtn_Click(object sender, EventArgs e)
        {

            createBtn.CssClass = createBtn.CssClass.Replace("activeBtn", "inactiveBtn");
            updateDetailsBtn.CssClass = updateDetailsBtn.CssClass.Replace("inactiveBtn", "activeBtn");
            Panel1.Visible = false;
            Panel2.Visible = true;

        }
        protected void companyddl_SelectedIndexChanged(object sender, EventArgs e)
        {

            int companyCount = 0;
            //string company = "";
            for (int i = 0; i < companyddl.Items.Count; i++)
            {
                if (companyddl.Items[i].Selected)
                {
                    companyCount++;
                    companyddl.SelectedItem.Selected = true;
                    //company += companyddl.Items[i].Value + ", ";
                    //companyddl.SelectedItem.Selected = true;

                }
            }
            companyddl.Texts.SelectBoxCaption = companyCount + " selected";
            //companyddl.Texts.SelectBoxCaption = company;
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
        protected void submitBtn_Click(object sender, EventArgs e)
        {
            string employeeInput = lblEmpID.Text;
            string companyInput = companyddl.Text;

            if (employeeInput == "" && companyInput == "")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showSaveMessage",
                                "<script language='javascript'>alert('Please fill in the required fields');</script>");
                return;
            } else
            {
                CreateNew();
            } 
            
        }

        protected void CreateNew()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            string connectionstring = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection appcon = new SqlConnection(connectionstring);
            appcon.Open();

            string employeeInput = lblEmpID.Text;
            string selectedCompany = "";
            for (int i = 0; i < companyddl.Items.Count; i++)
            {
                if (companyddl.Items[i].Selected)
                {
                    //string sqlinsertquery = "INSERT INTO exitCompany(EmpID, Company, IsActive, CreatedBy, CreatedDate) values(@employee, @company, '1', @createdby, @createddate);";

            //for (int i = 0; i < companyddl.Items.Count; i++)
            //{
            //    if (companyddl.Items[i].Selected)
            //    {
            //        string sqlinsertquery = "INSERT INTO exitCompany(EmpID, Company, IsActive, CreatedBy, CreatedDate) values(@employee, @company, '1', @createdby, @createddate);";

            //        using (SqlCommand insert = new SqlCommand(sqlinsertquery, appcon))
            //        {


                        insert.CommandType = CommandType.Text;
                        insert.Parameters.AddWithValue("@createdby", empID);
                        insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
                        insert.Parameters.AddWithValue("@employee", employeeInput);
                        insert.Parameters.AddWithValue("@company", companyddl.Items[i].Text);

                    //    insert.ExecuteNonQuery();
                    //}
                    selectedCompany += companyddl.Items[i].Value + ",";
                }
            }
            appcon.Close();
            mpePopUp.Show();
            string companyInput = selectedCompany.TrimEnd(',');
            valid.Text = "You have successfully assign employee ID " + employeeInput + " to " + companyInput + ".";



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
        
        protected void GetCompanyByEmpID()
        {
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();

            string employeeInput = lblFindEmpID.Text;

            string empNameSql = "select distinct EmpList.Employee_Name from EmpList, exitCompany where Emplist.EmpID = exitCompany.EmpID and exitCompany.EmpID = '" + employeeInput + "'";
            using (SqlCommand cmd = new SqlCommand(empNameSql, con))
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    string empName = dr[0].ToString();
                    lblEmpName.Visible = true;
                    lblDataEmpName.Visible = true;
                    lblDataEmpName.Text = empName;
                }
                string sql = "select Company, IsActive from exitCompany where EmpID = '" + employeeInput + "'";
                using (SqlCommand cmd2 = new SqlCommand(sql, con))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd2))
                    {
                        DataSet ds = new DataSet();
                        sda.Fill(ds);
                        DataTable dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {

                            GridView1.DataSource = dt;
                            GridView1.DataBind();

                        }

                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSaveMessage",
                                        "<script language='javascript'>alert('This Emp ID has not been added!');</script>");
                            return;
                        }


                    }
                }
            }

        }

        protected void SearchBtn_Click(object sender, EventArgs e)
        {
            string empIDInput = lblFindEmpID.Text;
            if (empIDInput != "")
            {
                GetCompanyByEmpID();
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showSaveMessage",
                                "<script language='javascript'>alert('Please fill in the fields required');</script>");
                return;
            }

        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Button btnStatus = e.Row.FindControl("btnStatus") as Button;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.Cells[1].Text) == "True")
                {
                    e.Row.Cells[1].Text = "Active";
                    btnStatus.Text = "Deactivate";

                }
                else if ((e.Row.Cells[1].Text) == "False")
                {

                    e.Row.Cells[1].Text = "Inactive";
                    btnStatus.Text = "Activate";

                }
            }

        }
        //protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    string empID = Session["empID"].ToString();
        //    Session["empID"] = empID;

        //    string company = GridView1.SelectedRow.Cells[0].Text;
        //    string status = GridView1.SelectedRow.Cells[1].Text;
        //    if (status == "Active")
        //    {
        //        status = "1";
        //    }
        //    else if (status == "Inactive")
        //    {
        //        status = "0";
        //    }

        //    string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
        //    SqlConnection con = new SqlConnection(cs);
        //    con.Open();

        //    string employeeInput = lblEmpID.Text;

        //    string sqlupdatequery = "update exitCompany set IsActive = @isactive, UpdateBy = @updateby, UpdateDate = @updatedate where EmpID = @employee AND Company = @company";

        //    using (SqlCommand update = new SqlCommand(sqlupdatequery, con))
        //    {
        //        update.CommandType = CommandType.Text;
        //        update.Parameters.AddWithValue("@updateby", empID);
        //        update.Parameters.AddWithValue("@updatedate", DateTime.Now.ToString());
        //        update.Parameters.AddWithValue("@employee", employeeInput);
        //        update.Parameters.AddWithValue("@isactive", status);
        //        update.Parameters.AddWithValue("@company", HttpUtility.HtmlDecode(companyddl.ToString()));

        //        update.ExecuteNonQuery();

        //    }

        //    con.Close();

        //}

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateItem")
            {
                string empID = Session["empID"].ToString();
                Session["empID"] = empID;


                //int countid;
                //int.TryParse(e.CommandArgument.ToString(), out countid);

                //string company = GridView1.DataKeys[countid].Values["Company"].ToString();
                //string status = GridView1.DataKeys[countid].Values["IsActive"].ToString();

                int rowIndex = Convert.ToInt32(e.CommandArgument);

                GridViewRow gvr = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                int rowIndex = gvr.RowIndex;

                //int countid;
                //int.TryParse(e.CommandArgument.ToString(), out countid);

                //int rowIndex = Convert.ToInt32(e.CommandArgument);
                string company = GridView1.DataKeys[rowIndex].Values["Company"].ToString();
                string status = GridView1.DataKeys[rowIndex].Values["IsActive"].ToString();
                int updatedstatus = 2;

                //string company = row.Cells[0].Text;
                //string status = row.Cells[1].Text;
                if (status == "True")
                {
                    updatedstatus = 0;
                }
                else if (status == "False")
                {
                    updatedstatus = 1;
                }
                Label1.Text = updatedstatus.ToString();
                string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
                SqlConnection con = new SqlConnection(cs);
                con.Open();

                string employeeInput = lblFindEmpID.Text;

                //string sqlupdatequery = "update exitCompany set IsActive = @updatedstatus, UpdateBy = @updateby, UpdateDate = @updatedate where EmpID = @employee AND Company = @company";
                string sqlupdatequery = "update exitCompany set IsActive = '" + updatedstatus + "', UpdateBy = '" + empID + "', UpdateDate = '" + DateTime.Now + "' where EmpID = '" + employeeInput + "' AND Company = '" + company +"';";

                using (SqlCommand update = new SqlCommand(sqlupdatequery, con))
                {
                    update.CommandType = CommandType.Text;
                    update.Parameters.AddWithValue("@updateby", empID);
                    update.Parameters.AddWithValue("@updatedate", DateTime.Now.ToString());
                    update.Parameters.AddWithValue("@employee", employeeInput);
                    update.Parameters.AddWithValue("@updatedstatus", updatedstatus);
                    update.Parameters.AddWithValue("@company", company);

                    update.ExecuteNonQuery();

                }

                con.Close();
            }
        }

        protected void lblFindEmpID_TextChanged(object sender, EventArgs e)
        {

        }
    }
}