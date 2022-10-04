﻿using System;
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
            //    MultiView1.SetActiveView(SoloView);
            //}
        }

        protected void SoloBtn_Click(object sender, EventArgs e)
        {
            namesddl.Visible = false;
            nametb.Visible = true;
            //    MultiView1.ActiveViewIndex = 0;
        }

        protected void TeamBtn_Click(object sender, EventArgs e)
        {
            namesddl.Visible = true;
            nametb.Visible = false;
            //    MultiView1.ActiveViewIndex = 1;
        }

        //protected void MultiView1_ActiveViewChanged(object sender, EventArgs e)
        //{

        //}

        protected void ReasonDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReasonDropdown.SelectedValue == "4")
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