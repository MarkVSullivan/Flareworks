using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FlareWorks.Library.DataService
{
    public class DataServiceHandler : IHttpHandler
    {
        /// <summary> Main method handles each incoming request </summary>
        /// <param name="context"> Context contains all the information about this request </param>
        public void ProcessRequest(HttpContext context)
        {

            // Get the original query string
            string queryString = context.Request.QueryString["urlrelative"];
            if (!String.IsNullOrEmpty(queryString))
            {

                // Collect the requested paths
                string[] splitter = queryString.Split("/".ToCharArray());
                List<string> paths = splitter.ToList();

                context.Response.StatusCode = 400;
                context.Response.ContentType = "text/html";
                context.Response.Write("<html><body>Welcome to the Flareworks data provider.<br /><br /></body></html>");
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "text/html";
                //context.Response.Write("<html><body>Welcome to the DMS Middle Tier<br /><br />Invalid URI - No endpoint requested<br /><br />See endpoint help on the <a href=\"http://dev0/wiki/index.php/DMS_Mid_Tier\">Development Wiki</a>.</body></html>");
                context.Response.Write("<html><body>Welcome to the Flareworks data provider.<br /><br />Invalid URI - No endpoint requested<br /><br /></body></html>");
            }
        }


        /// <summary> Method indicates this handler is reusable, and does not need to be created indivdiually
        /// for each request </summary>
        /// <value>This property always returns TRUE</value>
        public bool IsReusable { get { return true; } }
    }
}
