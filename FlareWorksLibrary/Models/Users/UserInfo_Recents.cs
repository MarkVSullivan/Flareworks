using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlareWorks.Models.Users
{
    /// <summary> Recently used values by a single user, allowing data input to rememeber
    /// the last entered values or the last search criteria used </summary>
    public class UserInfo_Recents
    {
        /// <summary> Information about the last item that was added by this user </summary>
        public UserInfo_Recents_LastAdded LastAdded { get; private set; }

        /// <summary>  </summary>
        public UserInfo_Recents_LastSearch LastSearch { get; private set; }

        /// <summary> Constructor for a new instance of the <see cref="UserInfo_Recents"/> class </summary>
        public UserInfo_Recents()
        {
            LastAdded = new UserInfo_Recents_LastAdded();
            LastSearch = new UserInfo_Recents_LastSearch();
        }


    }
}
