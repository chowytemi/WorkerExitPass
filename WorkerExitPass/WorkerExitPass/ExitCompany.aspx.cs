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
                //BindDataSetDataCompany();
                CheckAccess();
            }

        }

        protected void CheckAccess()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            //string Safety = ConfigurationManager.AppSettings["Safety"].ToString();
            string Safety = ConfigurationManager.AppSettings["TK"].ToString();
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
                string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name " +
                    "from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid " +
                    "and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + Safety + "' and EmpList.EmpID = '" + empID + "' ; ";
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (!dr.HasRows)
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

        protected void BindDataSetDataCompany()
        {
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();

            string employeeID = lblEmpID.Text;

            string getCompanyInquery = "select Company from exitCompany where EmpID = '" + employeeID + "';";
            SqlCommand cmd = new SqlCommand(getCompanyInquery, con);
            SqlDataAdapter da = new SqlDataAdapter(getCompanyInquery, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt = ds.Tables[0];

            string getCompanyquery = "";
            string companyIn = "";

            if (dt.Rows.Count > 1)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    companyIn += "'" + dt.Rows[i][0].ToString() + "'" + ",";
                }
                companyIn = companyIn.TrimEnd(',');
                getCompanyquery = "select distinct Company FROM EmpList WHERE isActive = 1 AND EmpList.Company NOT IN(" + companyIn + ");";

                using (SqlCommand cmd2 = new SqlCommand(getCompanyquery, con))
                {
                    cmd2.CommandType = CommandType.Text;
                    cmd2.Connection = con;
                    companyddl.DataSource = cmd2.ExecuteReader();
                    companyddl.DataTextField = "Company";
                    companyddl.DataBind();

                }
            }
            else
            {
                getCompanyquery = "select distinct Company FROM EmpList WHERE isActive = 1;";

                using (SqlCommand cmd2 = new SqlCommand(getCompanyquery, con))
                {
                    cmd2.CommandType = CommandType.Text;
                    cmd2.Connection = con;
                    companyddl.DataSource = cmd2.ExecuteReader();
                    companyddl.DataTextField = "Company";
                    companyddl.DataBind();

                }
            }



            con.Close();
        }
        protected void createBtn_Click(object sender, EventArgs e)
        {
            updateDetailsBtn.CssClass = updateDetailsBtn.CssClass.Replace("activeBtn", "inactiveBtn");
            createBtn.CssClass = createBtn.CssClass.Replace("inactiveBtn", "activeBtn");
            Panel1.Visible = true;
            Panel2.Visible = false;

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

        protected void submitBtn_Click(object sender, EventArgs e)
        {

            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            var currentdate = DateTime.Now;

            string employeeInput = lblEmpID.Text;
            int counter = 0;
            if (employeeInput != "")
            {

                for (int i = 0; i < companyddl.Items.Count; i++)
                {
                    if (companyddl.Items[i].Selected)
                    {
                        counter += 1;
                    }

                }

                if (counter > 0)
                {

                    CreateNew();

                }
                else if (counter == 0)
                {
                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "showSaveMessage",
                    //      "<script language='javascript'>alert('Please select names of company');</script>");
                    //return;
                    mpePopUp.Show();
                    labelSuccess.Text = "Error!";
                    valid.Text = "Please select names of company";
                }
            }
            else
            {
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "showSaveMessage",
                //                "<script language='javascript'>alert('Please input employee ID');</script>");
                //return;
                mpePopUp.Show();
                labelSuccess.Text = "Error!";
                valid.Text = "Please input employee ID";
            }
        }

        protected void CheckEmp()
        {
            string employeeInput = lblFindEmpID.Text;

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();

            string sql = "select EmpID from EmpList where EmpID = '" + employeeInput + "'";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                //CreateNew();
                GetCompany();
                //return;
            }
            else
            {
                mpePopUp.Show();
                labelSuccess.Text = "Error!";
                valid.Text = employeeInput + " does not exist!";
                lblEmpName.Visible = false;
                lblDataEmpName.Visible = false;
                GridView1.Visible = false;
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
                    string sqlinsertquery = "INSERT INTO exitCompany(EmpID, Company, IsActive, CreatedBy, CreatedDate) values(@employee, @company, '1', @createdby, @createddate);";


                    using (SqlCommand insert = new SqlCommand(sqlinsertquery, appcon))
                    {
                        insert.CommandType = CommandType.Text;
                        insert.Parameters.AddWithValue("@createdby", empID);
                        insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
                        insert.Parameters.AddWithValue("@employee", employeeInput);
                        insert.Parameters.AddWithValue("@company", companyddl.Items[i].Text);

                        insert.ExecuteNonQuery();
                    }
                    selectedCompany += companyddl.Items[i].Value + ",";
                }
            }
            appcon.Close();
            mpePopUp.Show();
            string companyInput = selectedCompany.TrimEnd(',');
            labelSuccess.Text = "Success!";
            valid.Text = "You have successfully assign employee ID " + employeeInput + " to " + companyInput + ".";

        }

        protected void GetEmpNameByEmpID()
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

            }

        }
        private DataSet GetCompany()
        {
            string employeeInput = lblFindEmpID.Text;

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();
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
                        GridView1.Visible = true;
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                        GetEmpNameByEmpID();

                    }
                    else if (dt.Rows.Count == 0)
                    {
                        mpePopUp.Show();
                        labelSuccess.Text = "Error!";
                        valid.Text = "This employee has not been assigned any company!";
                        lblEmpName.Visible = false;
                        lblDataEmpName.Visible = false;
                        GridView1.Visible = false;

                    }

                    return ds;

                }
            }
        }


        protected void SearchBtn_Click(object sender, EventArgs e)
        {
            string empIDInput = lblFindEmpID.Text;
            if (empIDInput != "")
            {
                //GetEmpNameByEmpID();
                //GetCompany();
                CheckEmp();
            }
            else
            {
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "showSaveMessage",
                //                "<script language='javascript'>alert('Please fill in the fields required');</script>");
                //return;
                lblEmpName.Visible = false;
                lblDataEmpName.Visible = false;
                GridView1.Visible = false;
                mpePopUp.Show();
                labelSuccess.Text = "Error!";
                valid.Text = "Please input employee ID";

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

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateItem")
            {
                string empID = Session["empID"].ToString();
                Session["empID"] = empID;

                GridViewRow gvr = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                int rowIndex = gvr.RowIndex;

                string company = GridView1.DataKeys[rowIndex].Values["Company"].ToString();
                string status = GridView1.DataKeys[rowIndex].Values["IsActive"].ToString();
                int updatedstatus = 2;

                if (status == "True")
                {
                    updatedstatus = 0;
                }
                else if (status == "False")
                {
                    updatedstatus = 1;
                }
                string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
                SqlConnection con = new SqlConnection(cs);
                con.Open();

                string employeeInput = lblFindEmpID.Text;

                //string sqlupdatequery = "update exitCompany set IsActive = @updatedstatus, UpdateBy = @updateby, UpdateDate = @updatedate where EmpID = @employee AND Company = @company";
                string sqlupdatequery = "update exitCompany set IsActive = '" + updatedstatus + "', UpdateBy = '" + empID + "', UpdateDate = '" + DateTime.Now + "' where EmpID = '" + employeeInput + "' AND Company = '" + company + "';";

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
                GetCompany();
                mpePopUp.Show();
                labelSuccess.Text = "Success!";
                valid.Text = "You have successfully updated the status.";


            }
        }

        protected void lblFindEmpID_TextChanged(object sender, EventArgs e)
        {
            //GetCompany();
            CheckEmp();
        }
        protected void NextBtn_Click(object sender, EventArgs e)
        {

            string empIDInput = lblEmpID.Text;
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();

            if (empIDInput != "")
            {
                string sql = "select EmpID from EmpList where EmpID = '" + empIDInput + "'";
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    lblCompanyName.Visible = true;
                    companyddl.Visible = true;
                    BindDataSetDataCompany();
                    submitBtn.Visible = true;
                }
                else
                {
                    mpePopUp.Show();
                    labelSuccess.Text = "Error!";
                    valid.Text = empIDInput + " does not exist!";
                    lblCompanyName.Visible = false;
                    companyddl.Visible = false;
                    submitBtn.Visible = false;

                }
            }
            else
            {
                mpePopUp.Show();
                labelSuccess.Text = "Error!";
                valid.Text = "Please input employee ID";
                lblCompanyName.Visible = false;
                companyddl.Visible = false;
                submitBtn.Visible = false;
            }

        }

    }
}