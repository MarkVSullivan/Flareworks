using System;
using System.Collections.Generic;

namespace FlareWorks.Models.Work
{
    /// <summary> Information about a single title within the Flareworks system </summary>
    public class TitleInfo
    {
        /// <summary> Primary key to this title </summary>
        public int PrimaryKey { get; set; }

        /// <summary> SUS Aleph library system number associated with this title </summary>
        public string AlephNum { get; set; }

        /// <summary> SUS Sierra library system number associated with this title </summary>
        public string SierraNum { get; set; }

        /// <summary> ISSN number for this title (for serials) </summary>
        public string ISSN { get; set; }

        /// <summary> Title of this material </summary>
        public string Title { get; set; }

        /// <summary> Title description bibliographic level ( i.e., was it described 
        /// as a monograph, monographic series, or serial ) </summary>
        public string BibliographicLevel { get; set; }

        /// <summary> Cataloging type, which tells if the catalog record is 
        /// an original type, copy, processed, or an add </summary>
        public string CatalogingType { get; set; }

        /// <summary> Document type, which provides information on the general
        /// type of the document ( i.e., Federal Government, EU, Florida, Planning, Headings )  </summary>
        public string DocumentType { get; set; }

        /// <summary> Federal agency, if this was created by a federal agency </summary>
        public string FederalAgency { get; set; }

        /// <summary> Adminstrative notes associated with this complete title </summary>
        public string AdminNotes { get; set; }

        /// <summary> Date this title was first added to the database </summary>
        public DateTime DateAdded { get; set; }

        /// <summary> Flag indicates if this material should be sent to cataloging </summary>
        public bool SendToCataloging { get; set; }

        /// <summary> Flag indicates if it was confirmed that no ISSN exists for this material </summary>
        public bool NoIssnConfirmed { get; set; }

        /// <summary> Date it was confirmed no ISSN exists for this material, or NULL </summary>
        public DateTime? NoIssnConfirmedDate { get; set; }

        /// <summary> User that confirmed no ISSN exists for this material </summary>
        public string NoIssnConfirmedUser { get; set; }

        /// <summary> List of item work sets associated with this title </summary>
        public List<ItemWork> Items { get; private set; }

        /// <summary> Clean-up tasks associated with this title, which may or may not have been cleared </summary>
        public List<TitleInfo_CleanUp> Tasks { get; private set; } 

        /// <summary> Constructor for a new instance of the <see cref="TitleInfo"/> class </summary>
        public TitleInfo()
        {
            Items = new List<ItemWork>();
            Tasks = new List<TitleInfo_CleanUp>();
        }
    }
}
