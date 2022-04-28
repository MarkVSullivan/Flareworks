using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlareWorks.Library.Models.QC
{
    public class WorkSet_AuthorityWork
    {
        public int AuthorityRecordTypeID { get; set; }
        public string AuthorityRecordType { get; set; }
        public bool OriginalWork { get; set; }

        public WorkSet_AuthorityWork(int AuthorityRecordTypeID, string AuthorityRecordType, bool OriginalWork)
        {
            this.AuthorityRecordTypeID = AuthorityRecordTypeID;
            this.AuthorityRecordType = AuthorityRecordType;
            this.OriginalWork = OriginalWork;
        }
    }
}
