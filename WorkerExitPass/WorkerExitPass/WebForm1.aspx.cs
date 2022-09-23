using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WorkerExitPass
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ReasonDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReasonDropdown.SelectedValue == "4")
            {
                remarks.Visible = true;
                remarksInput.Visible = true;

            } else
            {
                remarks.Visible = false;
                remarksInput.Visible = false;
            }
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {

        }
    }
}