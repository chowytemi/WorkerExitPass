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
        //Panel pnlDropDownList;
        //protected void Page_PreInit(object sender, EventArgs e)
        //{
        //    //Create a Dynamic Panel
        //    pnlDropDownList = new Panel();
        //    pnlDropDownList.ID = "pnlDropDownList";
        //    //pnlDropDownList.BorderWidth = 1;
        //    pnlDropDownList.Width = 300;
        //    this.form1.Controls.Add(pnlDropDownList);

        //    //Create a LinkDynamic Button to Add TextBoxes
        //    LinkButton btnAddDdl = new LinkButton();
        //    btnAddDdl.ID = "btnAddDdl";
        //    btnAddDdl.Text = "Add";
        //    btnAddDdl.Click += new System.EventHandler(btnAdd_Click);
        //    this.form1.Controls.Add(btnAddDdl);

        //    //Recreate Controls
        //    RecreateControls("ddlDynamic", "DropDownList");

        //}

        //protected void btnAdd_Click(object sender, EventArgs e)

        //{
        //    int cnt = FindOccurence("ddlDynamic");
        //    CreateDropDownList("ddlDynamic-" + Convert.ToString(cnt + 1));

        //}

        //private int FindOccurence(string substr)

        //{
        //    string reqstr = Request.Form.ToString();
        //    return ((reqstr.Length - reqstr.Replace(substr, "").Length)/ substr.Length);

        //}
        //private void CreateDropDownList(string ID)
        //{
        //    DropDownList ddl = new DropDownList();
        //    ddl.ID = ID;

        //        string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
        //        SqlConnection con = new SqlConnection(cs);
        //        SqlDataAdapter da = new SqlDataAdapter("select distinct company from EmpList", con);
        //        DataSet ds = new DataSet();
        //        da.Fill(ds);
        //        ds.Tables[0].Columns.Add("Company", typeof(string), "company");

        //    ddl.DataTextField = "Company";
        //    ddl.DataValueField = "company";
        //    ddl.DataSource = ds;
        //    ddl.DataBind();
        //    ddl.Items.Insert(0, new ListItem("Select Company", "0"));

        //    //ddl.Items.Add(new ListItem("--Select--", ""));
        //    //ddl.Items.Add(new ListItem("One", "1"));
        //    //ddl.Items.Add(new ListItem("Two", "2"));       
        //    //ddl.Items.Add(new ListItem("Three", "3"));
        //    ddl.AutoPostBack = true;
        //    ddl.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
        //    pnlDropDownList.Controls.Add(ddl);

        //    Literal lt = new Literal();
        //    lt.Text = "<br />";
        //    pnlDropDownList.Controls.Add(lt);

        //}
        //protected void OnSelectedIndexChanged(object sender, EventArgs e)
        //{
        //    DropDownList ddl = (DropDownList)sender;
        //    string ID = ddl.ID;
        //    ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert",
        //                 "<script type = 'text/javascript'>alert('" + ID +
        //                  " fired SelectedIndexChanged event');</script>");
        //    //Place the functionality here

        //}

        //private void RecreateControls(string ctrlPrefix, string ctrlType)
        //{
        //    string[] ctrls = Request.Form.ToString().Split('&');
        //    int cnt = FindOccurence(ctrlPrefix);
        //    if (cnt > 0)
        //    {
        //        for (int k = 1; k <= cnt; k++)
        //        {
        //            for (int i = 0; i < ctrls.Length; i++)
        //            {
        //                if (ctrls[i].Contains(ctrlPrefix + "-" + k.ToString())
        //                    && !ctrls[i].Contains("EVENTTARGET"))                       
        //                {
        //                    string ctrlID = ctrls[i].Split('=')[0];
        //                    if (ctrlType == "DropDownList")
        //                    {
        //                        CreateDropDownList(ctrlID);
        //                    }

        //                    break;

        //                }

        //            }

        //        }

        //    }

        //}
        protected void Page_Load(object sender, EventArgs e)
        {
          
            BindDataSetDataCompany();
        }

        protected void BindDataSetDataCompany()
        {
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

            //int companyCount = 0;
            string company = "";
            for (int i = 0; i < companyddl.Items.Count; i++)
            {
                if (companyddl.Items[i].Selected)
                {
                    //companyCount++;
                    company = companyddl.Items[i].Value + ", ";
                    companyddl.SelectedItem.Selected = true;

                }
            }
            //companyddl.Texts.SelectBoxCaption = companyCount + " selected";
            companyddl.Texts.SelectBoxCaption = company;
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
            CreateNew();
        }

        protected void CreateNew()
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
            //        string sqlinsertquery = "INSERT INTO exitCompany(EmpID, Company, IsActive, CreatedBy, CreatedDate) values(@employee, @company, '1', @createdby, @createddate);";

            //        using (SqlCommand insert = new SqlCommand(sqlinsertquery, appcon))
            //        {


            //            insert.CommandType = CommandType.Text;
            //            insert.Parameters.AddWithValue("@createdby", empID);
            //            insert.Parameters.AddWithValue("@createddate", DateTime.Now.ToString());
            //            insert.Parameters.AddWithValue("@employee", employeeInput);
            //            insert.Parameters.AddWithValue("@company", HttpUtility.HtmlDecode(companyddl.ToString()));

            //            insert.ExecuteNonQuery();
            //        }
            //    }
            //}
            //appcon.Close();



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
        protected void GetCompanyByEmpID()
        {
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();

            string employeeInput = lblFindEmpID.Text;
            string sql = "select EmpList.Employee_Name, exitCompany.Company, exitCompany.IsActive from EmpList, exitCompany where exitCompany.EmpID = '" + employeeInput + "' and exitCompany.EmpID = EmpList.EmpID";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {

                        GridView1.DataSource = dt;
                        GridView1.DataBind();

                    }
                    //SqlDataReader dr = cmd.ExecuteReader();
                    //if (dr.HasRows)
                    //{
                    //    while (dr.Read())
                    //    {
                    //        string company = dr[0].ToString();
                    //        //showCompanyddl.Texts.SelectBoxCaption = company;
                    //        //BindDataSetDataCompany();
                    //        //showCompany.Visible = true;
                    //        //showCompanyddl.Visible = true;
                    //        UpdateBtn.Visible = true;

                    //    }

                     else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "showSaveMessage",
                                    "<script language='javascript'>alert('This Emp ID has not been added!');</script>");
                        return;
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
    }
}