using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WorkerExitPass
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
                //SoloBtn.Attributes.Add("class", "activeBtn");
            //}

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
    }
}