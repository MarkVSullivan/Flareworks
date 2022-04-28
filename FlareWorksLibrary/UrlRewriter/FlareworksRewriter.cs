using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FlareWorks.Library.UrlRewriter
{
    public class FlareworksRewriter : IHttpModule
    {
        void RewriteModule_BeginRequest(object sender, EventArgs e)
        {
            // Get the current execution path from the incoming request
            string appRelative = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath;
            if (appRelative == null)
                return;

            // Convert to lower case for the rest of the processing
            appRelative = appRelative.ToLower();

            // This will catch almost ALL requests... if there is no "data" in the URL, then return immediately
            if (appRelative.IndexOf("data", StringComparison.InvariantCultureIgnoreCase) < 0)
                return;

            // Remove any double slashes
            appRelative = appRelative.Replace("//", "/");

            // Get rid of first part 
            if (appRelative.IndexOf("~/") == 0)
                appRelative = appRelative.Substring(2);

            // Get rid of leading '/'
            if ((appRelative.Length > 0) && (appRelative[0] == '/'))
            {
                appRelative = appRelative.Length > 1 ? appRelative.Substring(1) : String.Empty;
            }

            // Is this requesting /data/?
            if ((appRelative.Length > 0) && (appRelative.IndexOf("data") == 0))
            {
                // Favicon.ico is a common request.. abort right here
                if (appRelative.IndexOf("favicon.ico", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return;

                // If this is a standard HTML request.. also abort right here
                if (appRelative.IndexOf(".html", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return;

                // If this is a standard ASPX request.. also abort right here
                if (appRelative.IndexOf(".aspx", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return;

                // If this is a standard ASPX request.. also abort right here
                if (appRelative.IndexOf(".jpg", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return;

                // If this is a standard ASPX request.. also abort right here
                if (appRelative.IndexOf(".css", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return;

                // Save the original URL 
                HttpContext.Current.Items.Add("Original_URL", HttpContext.Current.Request.Url.ToString());

                // Get the current query string
                string current_querystring = HttpContext.Current.Request.QueryString.ToString();

                // Rewrite the URL
                if (current_querystring.Length > 0)
                {
                    HttpContext.Current.RewritePath("~/data.csvc?urlrelative=" + appRelative + "&" + current_querystring, true);
                }
                else
                {
                    HttpContext.Current.RewritePath("~/data.csvc?urlrelative=" + appRelative, true);
                }
            }
        }


        /// <summary> Upon initialization, attaches to the context's BeginRequest event to allow for url rewriting 
        /// at the beginning of each incoming request </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += RewriteModule_BeginRequest;
        }

        /// <summary> Dispose of this class </summary>
        public void Dispose() { }
    }
}
