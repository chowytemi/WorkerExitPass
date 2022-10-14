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
    public partial class WebForm4 : System.Web.UI.Page
    {
        //Get login id
        //string empID = "PXE6563";
        //string empID = "T202";

        protected void Page_Load(object sender, EventArgs e)
        {
            var exitid = Request.QueryString["exitid"];
            //Label1.Text = exitid;

            if (!IsPostBack)
            {
                //CheckForAccess();
                //MultiView1.SetActiveView(View2);
                //btnShowPending.Attributes.Add("class", "btnActive");
                DataTable dt = this.GetPending();
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        protected void CheckForAccess()
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

        private DataTable GetPending()
        {
            DataTable dt = new DataTable();
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            string statussql = "select exitID, createddate, exittime, reason, approve from exitapproval where approve IS NULL AND reason NOT IN('Medical Injury') order by exittime desc;";
            using (SqlConnection conn = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand(statussql))
                {
                    cmd.Connection = conn;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }

        //private DataTable GetAll()
        //{
        //    DataTable dt = new DataTable();
        //    string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
        //    string statussql = "select exitID, createddate, exittime, reason, approve from exitapproval";
        //    using (SqlConnection conn = new SqlConnection(cs))
        //    {
        //        using (SqlCommand cmd = new SqlCommand(statussql))
        //        {
        //            cmd.Connection = conn;
        //            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
        //            {
        //                sda.Fill(dt);
        //            }
        //        }
        //    }

        //    return dt;
        //}

        //private DataTable GetApproved()
        //{
        //    DataTable dt = new DataTable();
        //    string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
        //    string statussql = "select exitID, createddate, exittime, reason from exitapproval where approve = 1 order by createddate, exittime;";
        //    using (SqlConnection conn = new SqlConnection(cs))
        //    {
        //        using (SqlCommand cmd = new SqlCommand(statussql))
        //        {
        //            cmd.Connection = conn;
        //            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
        //            {
        //                sda.Fill(dt);
        //            }
        //        }
        //    }

        //    return dt;
        //}

        //private DataTable GetRejected()
        //{
        //    DataTable dt = new DataTable();
        //    string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
        //    string statussql = "select exitID, createddate, exittime, reason from exitapproval where approve = 0 order by createddate, exittime;";
        //    using (SqlConnection conn = new SqlConnection(cs))
        //    {
        //        using (SqlCommand cmd = new SqlCommand(statussql))
        //        {
        //            cmd.Connection = conn;
        //            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
        //            {
        //                sda.Fill(dt);
        //            }
        //        }
        //    }

        //    return dt;
        //}

        protected void ddlReason_SelectedIndexChanged(object sender, EventArgs e)
        {
            string reason = ddlReason.SelectedItem.Value;
            DataTable dt = this.GetPending();
            DataView dataView = dt.DefaultView;
            if (!string.IsNullOrEmpty(reason))
            {
                dataView.RowFilter = "Reason = '" + reason + "'";
            }
            GridView1.DataSource = dataView;
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DateTime date1 = Convert.ToDateTime(e.Row.Cells[1].Text);
                e.Row.Cells[1].Text = date1.ToString("dd/MM/yyyy");

                if ((e.Row.Cells[2].Text) == "&nbsp;")
                {

                    e.Row.Cells[2].Text = "NULL";

                }
                else
                {
                    DateTime time1 = Convert.ToDateTime(e.Row.Cells[2].Text);
                    e.Row.Cells[2].Text = time1.ToString("hh:mm tt");
                }

                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GridView1, "Select$" + e.Row.RowIndex);
                e.Row.ToolTip = "Click to select this row.";

            }
        }


        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                //string exitID = GridView1.SelectedRow.Cells[0].Text;
                int exitID = Convert.ToInt32(GridView1.SelectedRow.Cells[0].Text);

                //Connect to database
                string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
                SqlConnection conn = new SqlConnection(cs);
                conn.Open();

                string statussql = "select exitapproval.createddate, exitapproval.exittime, exitapproval.projectdesc, EmpList.Employee_Name, exitapproval.company, exitapproval.reason, exitapproval.remarks from exitapproval, EmpList where exitapproval.createdby = EmpList.EmpID and exitapproval.exitID = '" + exitID + "';";
                SqlDataAdapter da = new SqlDataAdapter(statussql, conn);

                DataSet ds = new DataSet();
                da.Fill(ds);
                DataTable dt = ds.Tables[0];

                DateTime date = Convert.ToDateTime(dt.Rows[0]["createddate"]);

                DateTime time = Convert.ToDateTime(dt.Rows[0]["exittime"]);

                //Binding TextBox From dataTable    
                lblexitID.Text = "Early Exit Permit ID #" + exitID + " Details";
                tbDate.Text = date.ToString("dd/MM/yyyy");
                tbTime.Text = time.ToString("hh:mm tt");
                tbProject.Text = dt.Rows[0]["projectdesc"].ToString();
                tbName.Text = dt.Rows[0]["Employee_Name"].ToString();
                tbCompany.Text = dt.Rows[0]["company"].ToString();
                tbReason.Text = dt.Rows[0]["reason"].ToString();

                if (dt.Rows[0]["remarks"].ToString() == "")
                {
                    tbRemarks.Text = "N.A";
                } else
                {
                    tbRemarks.Text = dt.Rows[0]["remarks"].ToString();
                }
                

                mpeApproval.Show();

            }
            catch (Exception)
            {
                throw;    
            }
        }

        protected void ApproveBtn_Click(object sender, EventArgs e)
        {
            //lblConfirmation.Text = "Do you want to approve this early exit permit?";
            //mpeConfirmation.Show();

            string approverID = "T202";
            DateTime approveddate = DateTime.Now;
            int exitID = Convert.ToInt32(GridView1.SelectedRow.Cells[0].Text);
            //int exitID = 12;
            int approve = 1;

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string sqlquery = "update exitapproval set approver = '" + approverID + "', approve = " + approve + ", approveddate = '" + approveddate + "' where exitID = '" + exitID + "'";

            using (SqlCommand update = new SqlCommand(sqlquery, conn))
            {
                update.ExecuteNonQuery();

                conn.Close();
            }

            Response.Redirect(Request.RawUrl);
        }

        protected void RejectBtn_Click(object sender, EventArgs e)
        {
            //string approverID = "T202";
            //DateTime approveddate = DateTime.Now;
            //int exitID = Convert.ToInt32(GridView1.SelectedRow.Cells[0].Text);
            ////int exitID = 12;
            //int approve = 0;

            //string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            //SqlConnection conn = new SqlConnection(cs);
            //conn.Open();
            //string sqlquery = "update exitapproval set approver = '" + approverID + "', approve = " + approve + ", approveddate = '" + approveddate + "' where exitID = '" + exitID + "'";

            //using (SqlCommand update = new SqlCommand(sqlquery, conn))
            //{
            //    update.ExecuteNonQuery();

            //    conn.Close();
            //}

            //Response.Redirect(Request.RawUrl);
        }

        //protected void btnShowAll_Click(object sender, EventArgs e)
        //{
        //    MultiView1.ActiveViewIndex = 0;
        //    DataTable dt = this.GetAll();
        //    GridView2.DataSource = dt;
        //    GridView2.DataBind();
        //}

        //protected void btnShowPending_Click(object sender, EventArgs e)
        //{
        //    MultiView1.ActiveViewIndex = 1;
        //    GetPending();
        //}

        //protected void btnShowApproved_Click(object sender, EventArgs e)
        //{
        //    MultiView1.ActiveViewIndex = 2;
        //    DataTable dt = this.GetApproved();
        //    GridView3.DataSource = dt;
        //    GridView3.DataBind();
        //}

        //protected void btnShowRejected_Click(object sender, EventArgs e)
        //{
        //    MultiView1.ActiveViewIndex = 3;
        //    DataTable dt = this.GetRejected();
        //    GridView4.DataSource = dt;
        //    GridView4.DataBind();
        //}

        //protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if ((e.Row.Cells[4].Text) == "True")
        //    {
        //        e.Row.Cells[4].Text = "Approved";

        //    }
        //    else if ((e.Row.Cells[4].Text) == "&nbsp;")
        //    {

        //        e.Row.Cells[4].Text = "Pending";

        //    }
        //    else
        //    {
        //        e.Row.Cells[4].Text = "Rejected";
        //    }
        //}

        //protected void MultiView1_ActiveViewChanged(object sender, EventArgs e)
        //{
            //int activeView;
            //activeView = MultiView1.ActiveViewIndex;

            //if (activeView == 0)
            //{
            //    btnShowAll.Attributes.Add("class", "btnActive");
            //}
            //else if (activeView == 1)
            //{
            //    btnShowPending.Attributes.Add("class", "btnActive");
            //}
            //else if (activeView == 2)
            //{
            //    btnShowApproved.Attributes.Add("class", "btnActive");
            //}
            //else
            //{
            //    btnShowRejected.Attributes.Add("class", "btnActive");
            //}

        //}

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            GetPending();
            DataTable dt = this.GetPending();
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
    }
}