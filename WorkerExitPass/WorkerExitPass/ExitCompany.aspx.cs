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
            SqlDataAdapter da = new SqlDataAdapter("select distinct company from EmpList", con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            ds.Tables[0].Columns.Add("Company", typeof(string), "company");

            ddlCompany.DataTextField = "Company";
            ddlCompany.DataValueField = "company";
            ddlCompany.DataSource = ds;
            ddlCompany.DataBind();
            ddlCompany.Items.Insert(0, new ListItem("Select Company", "0"));
        }
    }
}