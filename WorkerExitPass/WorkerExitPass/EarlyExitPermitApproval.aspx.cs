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
    public partial class WebForm5 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if ((Request.QueryString["approval"] != null))
                {

                    string myempno = Request.QueryString["approval"];
                    Session["empID"] = myempno;

                }
                if ((Request.QueryString["exprmtid"] != null))
                {

                    string exitid = Request.QueryString["exprmtid"];
                    Session["exitid"] = exitid;
                }
                CheckAccess();
            }


        }

        protected void CheckAccess()
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            var exitID = Request.QueryString["exitid"];
            //using test access 87, pjm access 83
            string PJM = ConfigurationManager.AppSettings["PJM"].ToString();
            string Test = ConfigurationManager.AppSettings["Test"].ToString();
            string RO = ConfigurationManager.AppSettings["RO"].ToString();
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();

            string sql3 = "select exitapproval.createdby, EmpList.JobCode, EmpList.RO from exitapproval, EmpList where exitapproval.createdby = EmpList.EmpID and exitapproval.exitID = '" + exitID + "'";
            SqlCommand cmd3 = new SqlCommand(sql3, con);
            SqlDataReader dr3 = cmd3.ExecuteReader();


            if (dr3.HasRows)
            {
                while (dr3.Read())
                {
                    string ROid = dr3[2].ToString();
                
                    //check if worker or subcon
                    if (dr3[1].ToString() == "WK")
                    {
                        string sql = "select exitapproval.createdby, EmpList.RO from exitapproval, EmpList where exitapproval.createdby = EmpList.EmpID and exitID = '" + exitID + "' and EmpList.RO = '" + empID + "';";
                        SqlCommand cmd = new SqlCommand(sql, con);
                        SqlDataReader dr = cmd.ExecuteReader();
                    

                        if (dr.HasRows)
                        {
                            IsApprove();
                            //CheckApprovalAccess();
                            dr.Close();
                        }
                        else
                        {
                            Response.Redirect("http://eservices.dyna-mac.com/error");
                        }
                    }
                    else if (dr3[1].ToString() == "SUBCON")
                    {
                        string sql2 = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList " +
                                            "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                            "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id ='" + PJM + "' and EmpList.EmpID = '" + empID + "';";
                        SqlCommand cmd2 = new SqlCommand(sql2, con);
                        SqlDataReader dr2 = cmd2.ExecuteReader();
                        if (dr2.HasRows)
                        {
                            IsApprove();
                            //CheckApprovalAccess();
                            dr2.Close();
                        }
                        else
                        {
                            Response.Redirect("http://eservices.dyna-mac.com/error");
                        }
                    }
                    //testing
                    else if (dr3[1].ToString() == "STAFF")
                    {
                        string sql2 = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList " +
                                            "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                                            "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id ='" + PJM + "' and EmpList.EmpID = '" + empID + "';";
                        SqlCommand cmd2 = new SqlCommand(sql2, con);
                        SqlDataReader dr2 = cmd2.ExecuteReader();
                        if (dr2.HasRows)
                        {
                            IsApprove();
                            dr2.Close();
                        }
                        else
                        {
                            Response.Redirect("http://eservices.dyna-mac.com/error");
                        }
                    }
                }
            }
            else
            {
                Response.Redirect("http://eservices.dyna-mac.com/error");
            }


            con.Close();
        }

        //protected void CheckAccess()
        //{
        //    string empID = Session["empID"].ToString();
        //    Session["empID"] = empID;
        //    //using test access 87, pjm access 83
        //    string PJM = ConfigurationManager.AppSettings["PJM"].ToString();
        //    string Test = ConfigurationManager.AppSettings["Test"].ToString();
        //    string RO = ConfigurationManager.AppSettings["RO"].ToString();
        //    string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
        //    SqlConnection con = new SqlConnection(cs);
        //    con.Open();
        //    //for testing
        //    string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList " +
        //        "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
        //        "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id ='" + PJM + "' and EmpList.EmpID = '" + empID + "' ; ";
        //    SqlCommand cmd = new SqlCommand(sql, con);
        //    SqlDataReader dr = cmd.ExecuteReader();

        //    if (dr.HasRows)
        //    {
        //        //IsApprove();
        //        CheckApprovalAccess();
        //        dr.Close();
        //    }
        //    else
        //    {
        //        string sql2 = "select distinct RO from EmpList where RO IS NOT NULL AND RO = '" + empID + "';";
        //        //string sql2 = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id = '" + RO + "' and EmpList.EmpID = '" + empID + "' ; ";
        //        SqlCommand cmd2 = new SqlCommand(sql2, con);
        //        SqlDataReader dr2 = cmd2.ExecuteReader();
        //        if (dr2.HasRows)
        //        {
        //            //IsApprove();
        //            CheckApprovalAccess();
        //        }
        //        else
        //        {
        //            Response.Redirect("http://eservices.dyna-mac.com/error");
        //        }
        //        dr2.Close();

        //    }
        //    con.Close();
        //}

        protected void IsApprove()
        {
            var exitID = Request.QueryString["exitid"];
            string exitPermitLink = ConfigurationManager.AppSettings["exitPermitLink"].ToString();



            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string sql = "select approve from exitapproval where exitID = '" + exitID + "' and approve IS NULL";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader dr = cmd.ExecuteReader();



            if (dr.HasRows)
            {
                string sqlquery = "select approve from exitapproval where exitID = '" + exitID + "' and approve IS NULL and DATEADD(hour,1,exittime) > CURRENT_TIMESTAMP";
                SqlCommand cmdlineno = new SqlCommand(sqlquery, conn);
                SqlDataReader dr2 = cmdlineno.ExecuteReader();
                if (dr2.HasRows)
                {
                    GetApplicationById();



                }
                else
                {
                    labelExpiry.Text = "This early exit permit application has expired.";
                    ModalPopupExtender1.Show();
                }
                dr2.Close();
            }
            else
            {
                labelExpiry.Text = "This early exit permit application has been approved/rejected.";
                ModalPopupExtender1.Show();
            }



            dr.Close();
            conn.Close();
        }


        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DateTime date1 = Convert.ToDateTime(e.Row.Cells[1].Text);
                e.Row.Cells[1].Text = date1.ToString("dd/MM/yyyy");

                DateTime time1 = Convert.ToDateTime(e.Row.Cells[2].Text);
                e.Row.Cells[2].Text = time1.ToString("hh:mm tt");

            }
        }

        protected void GetApplicationById()
        {
            //try
            //{

                var exitID = Request.QueryString["exitid"];
                //Connect to database
                string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
                SqlConnection conn = new SqlConnection(cs);
                conn.Open();

                string statussql = "select distinct createddate, exittime, projectdesc, reason, remarks from exitapproval where exitID = '" + exitID + "';";
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
                //tbCompany.Text = dt.Rows[0]["company"].ToString();
                tbReason.Text = dt.Rows[0]["reason"].ToString();

                if (dt.Rows[0]["remarks"].ToString() == "")
                {
                    //tbRemarks.Text = "N.A";
                    remarks.Visible = false;
                    tbRemarks.Visible = false;
                }
                else
                {
                    tbRemarks.Text = dt.Rows[0]["remarks"].ToString();
                }
                string companysql = "select distinct company from exitapproval where exitid = '" + exitID + "'";
                SqlDataAdapter da3 = new SqlDataAdapter(companysql, conn);

                DataSet ds3 = new DataSet();
                da3.Fill(ds3);
                DataTable dt3 = ds3.Tables[0];
                string companyname = "";
                for (int i = 0; i < dt3.Rows.Count; i++)
                {

                    companyname += dt3.Rows[i][0].ToString() + ",";
                }
                companyname = companyname.TrimEnd(',');
                tbCompany.Text = companyname;
                string sql2 = "select CONCAT(EmpList.Employee_Name, ' (', RTRIM(EmpList.EmpID), ')') AS 'empNameID' " +
                    "from EmpList, exitapproval where exitapproval.EmpID = EmpList.EmpID and exitapproval.exitID = '" + exitID + "' and approve IS NULL;";
                //string sql2 = "select EmpList.Employee_Name from EmpList, exitapproval where exitapproval.EmpID = EmpList.EmpID and exitapproval.exitID = '" + exitID + "';";

                SqlDataAdapter da2 = new SqlDataAdapter(sql2, conn);

                DataSet ds2 = new DataSet();
                da2.Fill(ds2);
                DataTable dt2 = ds2.Tables[0];
                //if (dt2.Rows.Count == 1)
                //{
                //    chkAll.Visible = false;
                //    tbName.Text = dt2.Rows[0]["Employee_Name"].ToString();
                //}
                //else
                //{
                CheckBoxList1.DataSource = dt2;
                CheckBoxList1.DataTextField = "empNameID";
                CheckBoxList1.DataValueField = "empNameID";
                CheckBoxList1.DataBind();
                tbName.Visible = false;
                //}


                mpeApproval.Show();

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        protected void sendEmail()
        {
            var exitID = Request.QueryString["exitid"];
            string MailFrom = ConfigurationManager.AppSettings["MailFrom"].ToString();
            string smtpserver = ConfigurationManager.AppSettings["smtpserver"].ToString();
            string smtport = ConfigurationManager.AppSettings["smtport"].ToString();
            int smtpport = Convert.ToInt32(smtport);

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                //string sqlquery = "select distinct exitapproval.exitID, (select distinct EmpList.Employee_Name from EmpList, exitapproval where exitapproval.createdby = EmpList.EmpID and exitapproval.exitID = '" + exitID + "') as 'createdBy', exitapproval.projectdesc, exitapproval.exittime, exitapproval.reason, EmpList.CEmail, (select distinct EmpList.Employee_Name from EmpList, exitapproval where exitapproval.approver = EmpList.EmpID and exitapproval.exitID = '" + exitID + "') as 'approver', exitapproval.approve, exitapproval.approveddate from EmpList, exitapproval where exitapproval.exitID = '" + exitID + "' AND EmpList.EmpID = exitapproval.createdby;";
                string sqlquery = "select distinct exitapproval.exitID, (select distinct EmpList.Employee_Name from EmpList, exitapproval " +
                    "where exitapproval.createdby = EmpList.EmpID and exitapproval.exitID = '" + exitID + "') " +
                    "as 'createdBy', exitapproval.projectdesc, exitapproval.exittime, exitapproval.reason, EmpList.CEmail " +
                    "from EmpList, exitapproval where exitapproval.exitID = '" + exitID + "' AND EmpList.EmpID = exitapproval.createdby;";
                using (SqlCommand cmd = new SqlCommand(sqlquery, con))
                {
                    SqlDataAdapter da2 = new SqlDataAdapter(sqlquery, con);
                    DataSet ds2 = new DataSet();
                    da2.Fill(ds2);
                    DataTable dt2 = ds2.Tables[0];


                    string id = dt2.Rows[0][0].ToString();
                    string createdByName = dt2.Rows[0][1].ToString();
                    string project = dt2.Rows[0][2].ToString();
                    DateTime date = Convert.ToDateTime(dt2.Rows[0][3]);
                    string exittime = date.ToString("dd/MM/yyyy hh:mm tt");
                    string reason = dt2.Rows[0][4].ToString();
                    string createdByEmail = dt2.Rows[0][5].ToString();

                    //string approver = dt2.Rows[0][6].ToString();

                    DateTime permitexpiry = date.AddHours(1);
                    string validTill = permitexpiry.ToString("dd/MM/yyyy hh:mm tt");

                    string approverquery = "select distinct EmpList.Employee_Name from EmpList, exitapproval where exitapproval.approver = EmpList.EmpID and exitapproval.exitID = '"
                    + exitID + "'";
                    using (SqlCommand cmd4 = new SqlCommand(approverquery, con))
                    {
                        SqlDataAdapter da4 = new SqlDataAdapter(approverquery, con);
                        DataSet ds4 = new DataSet();
                        da4.Fill(ds4);
                        DataTable dt4 = ds4.Tables[0];

                        string approver = "";
                        for (int i = 0; i < dt4.Rows.Count; i++)
                        {
                            approver += dt4.Rows[i][0].ToString() + "<br />";

                        }

                        string sqlquery3 = "select CONCAT(RTRIM(EmpList.EmpID), ' - ' , EmpList.Employee_Name) as 'emp', exitapproval.approve, exitapproval.approveddate " +
                       "from EmpList, exitapproval where exitapproval.exitID = '" + exitID + "' and EmpList.EmpID = exitapproval.EmpID;";
                        using (SqlCommand cmd3 = new SqlCommand(sqlquery3, con))
                        {
                            SqlDataAdapter da = new SqlDataAdapter(sqlquery3, con);
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            DataTable dt = ds.Tables[0];

                            //string exitNames = "";
                            string body = "";
                            body += "Hello, " + createdByName + ".";
                            body += "<br /><br />Your application status for early exit permit on " + exittime + " has been updated.";
                            body += "<br /><br /><table style=\"table-layout: fixed; text-align:left; border-collapse: collapse; border: 1px solid; width: 70%;\">";
                            body += "<tr style=\" height: 0.5em;\">";
                            body += "<th style=\" text-align:left; color: #004B7A; border: 1px solid\">Exit ID</th>";
                            body += "<td style=\" border: 1px solid\">" + id + "</td>";
                            body += "<tr style=\" height: 0.5em;\">";
                            body += "<th style=\" text-align:left; color: #004B7A; border: 1px solid\">Project</th>";
                            body += "<td style=\" border: 1px solid\">" + project + "</td>";
                            body += "<tr style=\" height: 0.5em;\">";
                            body += "<th style=\" text-align:left; color: #004B7A; border: 1px solid\">Reason</th>";
                            body += "<td style=\" border: 1px solid\">" + reason + "</td>";
                            body += "<tr style=\" height: 0.5em;\">";
                            body += "<th style=\" text-align:left; color: #004B7A; border: 1px solid\">Exit Time</th>";
                            body += "<td style=\" border: 1px solid\">" + exittime + "</td>";
                            body += "<tr style=\" height: 0.5em;\">";
                            body += "<th style=\" text-align:left; color: #004B7A; border: 1px solid\">Approver</th>";
                            body += "<td style=\" border: 1px solid\">" + approver + "</td></tr></table>";

                            body += "<br /><br /><table style=\"table-layout: fixed; text-align:left; border-collapse: collapse; border: 1px solid; width: 70%;\">";
                            body += "<tr style=\" height: 0.5em;\">";
                            body += "<th style=\" text-align:left; color: #004B7A; border: 1px solid\">Employee Name(s)</th>";
                            body += "<th style=\" text-align:left; color: #004B7A; border: 1px solid\">Status</th>";
                            body += "<th style=\" text-align:left; color: #004B7A; border: 1px solid\">Approval Date</th></tr>";
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string status = dt.Rows[i][1].ToString();
                                if (status == "True")
                                {
                                    status = "Approved (Valid Till: " + validTill + ")";
                                }
                                else
                                {
                                    status = "Rejected";
                                }
                                DateTime date1 = Convert.ToDateTime(dt.Rows[i][2]);
                                string approveddate = date1.ToString("dd/MM/yyyy hh:mm tt");
                                body += "<tr><td style=\" border: 1px solid\">" + dt.Rows[i][0].ToString() + "</td>";
                                body += "<td style=\" border: 1px solid\">" + status + "</td>";
                                body += "<td style=\" border: 1px solid\">" + approveddate + "</td></tr>";


                            }
                            body += "</table>";
                            body += "<br />This is an automatically generated email, please do not reply.";

                            string PJM = ConfigurationManager.AppSettings["PJM"].ToString();

                            //for testing
                            //send email to person who created the application to update status
                            string sqlquery2 = "select distinct EmpList.Employee_Name, exitapproval.createdby, EmpList.CEmail from EmpList, exitapproval" +
                              " where exitapproval.exitID = '" + exitID + "' and EmpList.EmpID = exitapproval.createdby;";
                            //string sqlquery2 = "select distinct EmpList.EmpID,EmpList.CEmail " +
                            //                                                  "from Access, UserAccess, ARole, EmpList " +
                            //                                                  "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                            //                                                  "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 " +
                            //                                                  "and Access.id = '" + PJM + "' and EmpList.EmpID = 'T203'";

                            using (SqlCommand cmd2 = new SqlCommand(sqlquery2, con))
                            {
                                using (SqlDataReader dr2 = cmd2.ExecuteReader())
                                {
                                    while (dr2.Read())
                                    {
                                        string email = dr2[2].ToString();
                                        //string email = dr2[1].ToString();

                                        MailMessage mm = new MailMessage();
                                        mm.From = new MailAddress(MailFrom);
                                        mm.Subject = "Early Exit Permit Application Status is Updated";
                                        mm.Body = body;
                                        mm.IsBodyHtml = true;
                                        mm.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"].ToString());
                                        mm.To.Add(new MailAddress(email));
                                        SmtpClient smtp = new SmtpClient(smtpserver, smtpport); //Gmail smtp  
                                        smtp.EnableSsl = false;
                                        smtp.Send(mm);
                                    }


                                }
                            }
                        }

                    }



                    con.Close();
                }
            }
        }

        protected void Check_UnCheckAll(object sender, EventArgs e)
        {
            foreach (ListItem li in CheckBoxList1.Items)
            {
                li.Selected = chkAll.Checked;
            }
        }

        protected void CheckBox_Checked_Unchecked(object sender, EventArgs e)
        {
            bool isAllChecked = true;
            foreach (ListItem li in CheckBoxList1.Items)
            {
                if (!li.Selected)
                {
                    isAllChecked = false;
                    break;
                }
            }

            chkAll.Checked = isAllChecked;
        }

        protected void ApproveBtn_Click(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            DateTime approveddate = DateTime.Now;
            var exitID = Request.QueryString["exitid"];
            int approve = 1;

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            string exitPermitLink = ConfigurationManager.AppSettings["exitPermitLink"].ToString();
            string myApp = ConfigurationManager.AppSettings["myApp"].ToString();
            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                {
                    conn.Open();

                    foreach (ListItem li in CheckBoxList1.Items)
                    {
                        if (li.Selected == true)
                        {
                            //string getIDquery = "select EmpID from EmpList where Employee_Name = LEFT(@empName, CHARINDEX('(', @empName) - 1);";
                            string getIDquery = "Select SUBSTRING(@empName,CHARINDEX('(',@empName)+1 ,CHARINDEX(')',@empName)-CHARINDEX('(',@empName)-1)";
                            using (SqlCommand select = new SqlCommand(getIDquery, conn))
                            {
                                select.CommandType = CommandType.Text;
                                select.Parameters.AddWithValue("@empName", li.Value);
                                select.ExecuteNonQuery();

                                using (SqlDataReader dr = select.ExecuteReader())
                                {
                                    while (dr.Read())
                                    {
                                        string selectedEmpID = dr[0].ToString();

                                        string sqlquery = "update exitapproval set approver = '" + empID + "', approve = " + approve + ", approveddate = '" + approveddate + "' where exitID = '" + exitID + "'"
                                         + "AND EmpID = '" + selectedEmpID + "'";

                                        using (SqlCommand update = new SqlCommand(sqlquery, conn))
                                        {

                                            update.ExecuteNonQuery();


                                        }
                                    }
                                    dr.Close();
                                }
                            }
                        }
                    }
                    string sql3 = "select createddate, exittime, projectdesc, company, reason, remarks from exitapproval where exitID = '" + exitID + "' and approve IS NULL;";
                    SqlCommand cmdlineno = new SqlCommand(sql3, conn);
                    SqlDataReader dr2 = cmdlineno.ExecuteReader();

                    if (dr2.HasRows)
                    {
                        GetApplicationById();
                        dr2.Close();
                    }
                    else
                    {
                        sendEmail();
                        mpeApproval.Hide();
                        //Response.Redirect(exitPermitLink + "EarlyExitPermitView.aspx?approval=" + empID);
                        Response.Redirect("EarlyExitPermitView.aspx?approval=" + empID);
                    }

                    conn.Close();

                }
            }
            catch
            {
                //Response.Redirect(myApp);
                //Response.Redirect(exitPermitLink + "EarlyExitPermitView.aspx?approval=" + empID);
                Response.Redirect("EarlyExitPermitView.aspx?approval=" + empID);
            }



        }

        protected void RejectBtn_Click(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            DateTime approveddate = DateTime.Now;
            var exitID = Request.QueryString["exitid"];
            int approve = 0;
            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            string exitPermitLink = ConfigurationManager.AppSettings["link"].ToString();
            string myApp = ConfigurationManager.AppSettings["myApp"].ToString();
            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                {
                    conn.Open();

                    foreach (ListItem li in CheckBoxList1.Items)
                    {
                        if (li.Selected == true)
                        {
                            //insert to database, the value is in item.Value
                            //string getIDquery = "select EmpID from EmpList where Employee_Name = LEFT(@empName, CHARINDEX('(', @empName) - 1);";
                            string getIDquery = "Select SUBSTRING(@empName,CHARINDEX('(',@empName)+1 ,CHARINDEX(')',@empName)-CHARINDEX('(',@empName)-1)";

                            using (SqlCommand select = new SqlCommand(getIDquery, conn))
                            {
                                select.CommandType = CommandType.Text;
                                select.Parameters.AddWithValue("@empName", li.Value);
                                select.ExecuteNonQuery();
                                using (SqlDataReader dr = select.ExecuteReader())
                                {
                                    while (dr.Read())
                                    {
                                        string selectedEmpID = dr[0].ToString();

                                        string sqlquery = "update exitapproval set approver = '" + empID + "', approve = " + approve + ", approveddate = '" + approveddate + "' where exitID = '" + exitID + "'"
                                         + "AND EmpID = '" + selectedEmpID + "'";

                                        using (SqlCommand update = new SqlCommand(sqlquery, conn))
                                        {

                                            update.ExecuteNonQuery();


                                        }
                                    }
                                }
                            }
                        }
                    }

                    string sql3 = "select createddate, exittime, projectdesc, company, reason, remarks from exitapproval where exitID = '" + exitID + "' and approve IS NULL;";
                    SqlCommand cmdlineno = new SqlCommand(sql3, conn);
                    SqlDataReader dr2 = cmdlineno.ExecuteReader();

                    if (dr2.HasRows)
                    {
                        GetApplicationById();
                        dr2.Close();
                    }
                    else
                    {
                        sendEmail();
                        mpeApproval.Hide();
                        Response.Redirect("EarlyExitPermitView.aspx?approval=" + empID);
                        //Response.Redirect(exitPermitLink + "EarlyExitPermitView.aspx?approval=" + empID);

                        //Response.Redirect("EarlyExitPermitView.aspx?approval=" + empID);
                    }
                    conn.Close();
                }
            }
            catch
            {
                //Response.Redirect(myApp);
                //Response.Redirect(exitPermitLink + "EarlyExitPermitView.aspx?approval=" + empID);
                Response.Redirect("EarlyExitPermitView.aspx?approval=" + empID);
            }

        }

        protected void ApproveByEmail()
        {

            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            DateTime approveddate = DateTime.Now;
            var exitID = Request.QueryString["exitid"];
            var status = Request.QueryString["status"];

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string sqlquery = "update exitapproval set approver = '" + empID + "', approve = " + status + ", approveddate = '" + approveddate + "' where exitID = '" + exitID + "'";


            using (SqlCommand update = new SqlCommand(sqlquery, conn))
            {
                update.ExecuteNonQuery();

                conn.Close();
            }
            sendEmail();
            mpeApproval.Hide();
            Response.Redirect("EarlyExitPermitView.aspx?approval=" + empID);

        }


        protected void btnBack_Click(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            Response.Redirect("EarlyExitPermitView.aspx?approval=" + empID);

        }
        protected void continueBtn_Click(object sender, EventArgs e)
        {
            string empID = Session["empID"].ToString();
            Session["empID"] = empID;
            Response.Redirect("EarlyExitPermitView.aspx?approval=" + empID);

        }

        protected void CheckApprovalAccess()
        {
            string myempno = Request.QueryString["approval"];
            Session["empID"] = myempno;
            var exitID = Request.QueryString["exitid"];
            string PJM = ConfigurationManager.AppSettings["PJM"].ToString();
            string RO = ConfigurationManager.AppSettings["RO"].ToString();

            string cs = ConfigurationManager.ConnectionStrings["appusers"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                string sqlquery = "select exitapproval.createdby, EmpList.EmpID, EmpList.RO from exitapproval, EmpList where exitapproval.createdby = EmpList.EmpID and exitID = '" + exitID + "' " +
                    "and EmpList.RO = '" + myempno + "'";
                SqlCommand cmd = new SqlCommand(sqlquery, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {

                    IsApprove();
                }
                else
                {
                    //for testing
                    string sql = "select distinct EmpList.EmpID,EmpList.designation,EmpList.Employee_Name from Access, UserAccess, ARole, EmpList " +
                        "where UserAccess.RoleID = ARole.ID and ARole.ID = UserAccess.RoleID and UserAccess.AccessID = Access.ID " +
                        "and EmpList.ID = UserAccess.empid and UserAccess.IsActive = 1 and emplist.IsActive = 1 and Access.id ='" + PJM + "' and EmpList.EmpID = '" + myempno + "'";
                    SqlCommand cmd1 = new SqlCommand(sql, conn);
                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    if (dr1.HasRows)
                    {
                        IsApprove();
                    }
                    else
                    {
                        Response.Redirect("http://eservices.dyna-mac.com/error");
                    }
                }



            }
        }

    }

}