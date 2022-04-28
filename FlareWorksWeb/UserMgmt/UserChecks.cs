using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlareWorks.Library.Database;
using FlareWorks.Models.Users;

namespace FlareworksWeb.UserMgmt
{
    public static class UserChecks
    {
        public static UserInfo Check_Current_User()
        {
            // First, check the session
            UserInfo currentUser = HttpContext.Current.Session["CurrentUser"] as UserInfo;
            if (currentUser != null)
            {
                return currentUser;
            }

            // If the user information is still missing , but the FlareUser cookie exists, then pull 
            // the user information from the SobekUser cookie in the current requests
            if (HttpContext.Current.Request.Cookies["FlareUser"] != null)
            {
                string userid_string = HttpContext.Current.Request.Cookies["FlareUser"]["userid"];
                int userid = -1;

                bool valid_perhaps = userid_string.All(Char.IsNumber);
                if (valid_perhaps)
                    Int32.TryParse(userid_string, out userid);

                if (userid > 0)
                {
                    UserInfo possible_user = DatabaseGateway.Get_User(userid);
                    if (possible_user != null)
                    {
                        string cookie_security_hash = HttpContext.Current.Request.Cookies["FlareUser"]["security_hash"];
                        if (cookie_security_hash == possible_user.Security_Hash(HttpContext.Current.Request.UserHostAddress))
                        {
                            HttpContext.Current.Session["CurrentUser"] = possible_user;
                            return possible_user;
                        }
                        else
                        {
                            // Security hash did not match, so clear the cookie
                            HttpCookie userCookie = new HttpCookie("FlareUser");
                            userCookie.Values["userid"] = String.Empty;
                            userCookie.Values["security_hash"] = String.Empty;
                            userCookie.Expires = DateTime.Now.AddDays(-1);
                            HttpContext.Current.Response.Cookies.Add(userCookie);
                        }
                    }
                }
            }

            return null;
        }
    }
}