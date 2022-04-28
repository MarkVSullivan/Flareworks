using System;
using System.Web;

namespace FlareworksWeb.UserMgmt
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clear the current user
            HttpContext.Current.Session["CurrentUser"] = null;

            // Delete any user cookie
            HttpCookie userCookie = new HttpCookie("FlareUser");
            userCookie.Values["userid"] = String.Empty;
            userCookie.Values["security_hash"] = String.Empty;
            userCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(userCookie);
        }
    }
}