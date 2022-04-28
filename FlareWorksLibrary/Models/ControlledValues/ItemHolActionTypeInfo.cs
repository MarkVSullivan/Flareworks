using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlareWorks.Library.Models.ControlledValues
{
    /// <summary> One selectable Item HOL action type, such as 'Process' or 'Edit Only' </summary>
    public class ItemHolActionTypeInfo
    {
        /// <summary> Text for this item HOL action type </summary>
        public string ActionType { get; set; }

        /// <summary> Primary key for this item HOL action  type </summary>
        public int ID { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="ItemHolActionTypeInfo"/> class </summary>
        public ItemHolActionTypeInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="ItemHolActionTypeInfo"/> class </summary>
        /// <param name="ID"> Primary key for this item HOL action  type </param>
        /// <param name="ActionType"> Text for this item HOL action  type </param>
        public ItemHolActionTypeInfo(int ID, string ActionType)
        {
            this.ID = ID;
            this.ActionType = ActionType;
        }
    }
}
