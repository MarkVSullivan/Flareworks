using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlareWorks.Library.Tools
{
    public class EmailInfo
    {
        /// <summary> List of recipients, seperated by semicolons </summary>
        public string RecipientsList { get; set; }

        /// <summary> Subject line for the email to be sent </summary>
        public string Subject { get; set; }

        /// <summary> Body of the email to tbe sent </summary>
        public string Body { get; set; }

        /// <summary> From email address for this email to be sent </summary>
        /// <remarks> This can include the format 'Name &lt;name@domain.com&gt;'</remarks>
        public string FromAddress { get; set; }

        /// <summary> Email address that responses should be sent to (or NULL) </summary>
        public string ReplyTo { get; set; }

        /// <summary> Flag indicates if this should be sent as HTML or not </summary>
        public bool isHTML { get; set; }
    }
}
