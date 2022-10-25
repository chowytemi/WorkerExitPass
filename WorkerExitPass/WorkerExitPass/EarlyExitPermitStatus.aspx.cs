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
    public partial class WebForm3 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if ((Request.QueryString["exprmitstatus"] != null))
                {

                    string myempno = Request.QueryString["exprmitstatus"];
                    Session["empID"] = myempno;

                }
                FormStatus();
            }
            
        
        }

        private void FormStatus()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;

            //Connect to database
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            //string statussql = "select exitID, createddate, exittime, approve from exitapproval where createdby = '" + empID + "' order by exitID desc;";
            string statussql = "select distinct exitID, createddate, exittime, approve from exitapproval where EmpID = '" + empID + "' or createdby = '" + empID + "' order by exitID desc;";

            SqlDataAdapter da = new SqlDataAdapter(statussql, conn);
            using (DataTable dt = new DataTable())
            {
                da.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();

            }
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
                

                if ((e.Row.Cells[3].Text) == "True")
                {
                    e.Row.Cells[3].Text = "Approved";

                } else if ((e.Row.Cells[3].Text) == "&nbsp;") {

                    e.Row.Cells[3].Text = "Pending";

                } else {
                    e.Row.Cells[3].Text = "Rejected";
                }

                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GridView1, "Select$" + e.Row.RowIndex);
                e.Row.ToolTip = "Click to select this row.";

            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            FormStatus();
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            //string PJM = ConfigurationManager.AppSettings["PJM"].ToString();
            string Test = ConfigurationManager.AppSettings["Test"].ToString();
            string RO = ConfigurationManager.AppSettings["RO"].ToString();

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();

            try
            {
                int exitID = Convert.ToInt32(GridView1.SelectedRow.Cells[0].Text);

                string sql = "select distinct exitapproval.approve, (select distinct EmpList.Employee_Name from exitapproval, EmpList where exitapproval.approver = EmpList.EmpID and exitapproval.exitID = '" + exitID + "') AS 'approver', exitapproval.approveddate, exitapproval.createddate, exitapproval.exittime, exitapproval.projectdesc, exitapproval.company, exitapproval.reason, exitapproval.remarks from exitapproval, EmpList where exitapproval.createdby = EmpList.EmpID and exitapproval.exitID = '" + exitID + "';";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                DataTable dt = ds.Tables[0];

                lblexitID.Text = "Early Exit Permit ID #" + exitID + " Details";

                if (string.IsNullOrEmpty(dt.Rows[0]["approve"].ToString()))
                {
                    lblStatus.Text = "Pending";

                }
                else if (dt.Rows[0][0].ToString() == "True")
                {
                    lblStatus.Text = "Approved";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblStatus.Text = "Rejected";
                    lblStatus.ForeColor = System.Drawing.Color.Red;

                }

                if (!string.IsNullOrEmpty(dt.Rows[0]["approveddate"].ToString()) && !string.IsNullOrEmpty(dt.Rows[0]["approver"].ToString()))
                {
                    DateTime when = Convert.ToDateTime(dt.Rows[0]["approveddate"]);
                    lblWhen.Text = when.ToString("dd/MM/yyyy hh:mm tt");
                    lblApprover.Text = dt.Rows[0]["approver"].ToString();
                }
                else
                {
                    if (dt.Rows[0]["reason"].ToString() == "Medical Injury")
                    {
                        lblWhen.Text = "N.A";
                        lblApprover.Text = "N.A";
                    } else
                    {
                        lblWhen.Text = "Pending";
                        string sqlquery = "select EmpID, Employee_Name, JobCode, Department, designation, RO from EmpList where EmpID = '" + empID + "' and isActive = 1;";
                        using (SqlCommand cmd = new SqlCommand(sqlquery,conn))
                        {
                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    if (dr[2].ToString() == "WK")
                                    {
                                        string pjmquery = "select distinct EmpList.Employee_Name from Access, UserAccess,ARole,EmpList where UserAccess.RoleID = ARole.ID " +
                                            "and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + RO + "'";
                                        using (SqlCommand cmd2 = new SqlCommand(pjmquery, conn))
                                        {
                                            SqlDataAdapter da3 = new SqlDataAdapter(pjmquery, conn);
                                            DataSet ds3 = new DataSet();
                                            da3.Fill(ds3);
                                            DataTable dt3 = ds3.Tables[0];

                                            string pjmNames = "";
                                            for (int i = 0; i < dt3.Rows.Count; i++)
                                            {
                                                pjmNames += dt3.Rows[i][0].ToString() + "<br />";

                                            }
                                            lblApprover.Text = "Pending approval from: <br />" + pjmNames;
                                        }
                                    } else if (dr[2].ToString() == "SUBCON")
                                    {
                                        string roquery = "select distinct EmpList.Employee_Name from Access, UserAccess,ARole,EmpList where UserAccess.RoleID = ARole.ID " +
                                            "and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + Test + "'";
                                        using (SqlCommand cmd3 = new SqlCommand(roquery, conn))
                                        {                                       
                                            SqlDataAdapter da4 = new SqlDataAdapter(roquery, conn);
                                            DataSet ds4 = new DataSet();
                                            da4.Fill(ds4);
                                            DataTable dt3 = ds4.Tables[0];

                                            string roNames = "";
                                            for (int i = 0; i < dt3.Rows.Count; i++)
                                            {
                                                roNames += dt3.Rows[i][0].ToString() + "<br />";

                                            }
                                            lblApprover.Text = "Pending approval from: <br />" + roNames;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    
                }

                DateTime date = Convert.ToDateTime(dt.Rows[0]["createddate"]);
                DateTime time = Convert.ToDateTime(dt.Rows[0]["exittime"]);

                lblDate.Text = date.ToString("dd/MM/yyyy");
                lblTime.Text = time.ToString("hh:mm tt");
                lblProject.Text = dt.Rows[0]["projectdesc"].ToString();
                lblCompany.Text = dt.Rows[0]["company"].ToString();
                lblReason.Text = dt.Rows[0]["reason"].ToString();

                if (dt.Rows[0]["remarks"].ToString() == "")
                {
                    lblRemarks.Text = "N.A";
                    remarks.Attributes.Add("class", "hide");
                    lblRemarks.Attributes.Add("class", "hide");
                }
                else
                {
                    lblRemarks.Attributes.Add("class", "label");
                    remarks.Attributes.Add("class", "textbox");
                    lblRemarks.Text = dt.Rows[0]["remarks"].ToString();
                }

                string sql2 = "select EmpList.Employee_Name from EmpList, exitapproval where exitapproval.exitID = '" + exitID + "' and EmpList.EmpID = exitapproval.EmpID;";
                SqlDataAdapter da2 = new SqlDataAdapter(sql2, conn);
                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
                DataTable dt2 = ds2.Tables[0];

                string empName = "";
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    empName += dt2.Rows[i]["Employee_Name"].ToString() + "<br />";

                }
                //empName = empName.TrimEnd(',');
                lblName.Text = empName;

                mpePopUp.Show();
                conn.Close();

            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {          
            Server.TransferRequest(Request.Url.AbsolutePath, false);
        }
    }
}