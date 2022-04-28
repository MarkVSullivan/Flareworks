using System;
using System.Collections.Generic;
using FlareWorks.Library.Models.ControlledValues;
using FlareWorks.Models.ControlledValues;

namespace FlareWorks.Library.Models.QC
{
    public class WorkSet
    {
        /// <summary> Primary key for this item work set from the database </summary>
        public int PrimaryKey { get; set; }

        public string AlephNum { get; set; }

        /// <summary> Primary key to this title in the database </summary>
        public int TitleID { get; set; }

        /// <summary> Processor notes related to this work set </summary>
        public string ProcessorNotes { get; set; }

        /// <summary> Latest QC notes related to this work set, if it has been QC'd </summary>
        public string QcNotes { get; set; }

        /// <summary> User who performed the latest QC work on this work set, if it has been QC'd </summary>
        public string QcWorker { get; set; }

        /// <summary> Date these items were submitted for QC by the worker, or NULL </summary>
        public DateTime? DateSubmitted { get; set; }

        /// <summary> Date this set was rejected through QC, or NULL </summary>
        public DateTime? DateRejected { get; set; }

        /// <summary> Date this set was approved by QC, or NULL </summary>
        public DateTime? DateApproved { get; set; }

        /// <summary> Worker who performed the ingest/work for thess items within the title </summary>
        public string Worker { get; set; }

        /// <summary> List of item work sets associated with this work set </summary>
        public List<WorkSetItem> Items { get; private set; }

        /// <summary> List of errors already associated with this workset </summary>
        public List<WorkSet_Error> Errors { get; private set; }

        /// <summary> Cataloging type, which tells if the catalog record is 
        /// an original type, copy, processed, or an add </summary>
        public CatalogingTypeInfo CatalogingType { get; set; }

        /// <summary> Item HOL action type, such as 'Process' or 'Edit Only' </summary>
        public ItemHolActionTypeInfo ItemHolActionType { get; set; }

        /// <summary> PCC category type, such as 'New Authentication', 
        /// 'Maintenance', or 'Non-CONSER Maintenance' </summary>
        public PccCategoryInfo PccCategory { get; set; }

        /// <summary> Flag indicates this PCC Category subtype is new authentitation, versus maintenace </summary>
        public bool Pcc_NewAuthentication { get; set; }

        /// <summary> Flag indicates this PCC Category subtype is maintenance, versus new authentitation </summary>
        public bool Pcc_Maintenance { get; set; }

        /// <summary> Source institution for these items within the title </summary>
        public InstitutionInfo Institution { get; set; }

        /// <summary> Material type ( i.e., Print, CD, VHS, Map, or Microform ) </summary>
        public MaterialTypeInfo MaterialType { get; set; }

        /// <summary> Flag indicates if this is the last copy for this instution </summary>
        public bool LastCopy { get; set; }

        /// <summary> Institution for which this is the last copy ( may differ from the Institution 
        /// field which may indicate that it was moved to UF for example ) </summary>
        public string LastCopyInstitution { get; set; }

        // If this was an edit of the hol, how many edited?
        public int ItemHol_EditCount { get; set; }

        public List<WorkSet_AuthorityWork> AuthorityWork { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="WorkSet"/> class </summary>
        public WorkSet()
        {
            PrimaryKey = -1;
            Items = new List<WorkSetItem>();
            Errors = new List<WorkSet_Error>();
            AuthorityWork = new List<WorkSet_AuthorityWork>();
        }
    }
}
