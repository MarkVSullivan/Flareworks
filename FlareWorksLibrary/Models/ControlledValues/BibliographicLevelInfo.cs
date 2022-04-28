namespace FlareWorks.Models.ControlledValues
{
    /// <summary> One selectable bibliographic level, which determines how the title is described, as a 
    /// monograph, monographic series, or serial </summary>
    public class BibliographicLevelInfo
    {
        /// <summary> Text for this bibliographic level </summary>
        public string Level { get; set; }

        /// <summary> Primary key for this bibliographic level </summary>
        public int ID { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="BibliographicLevelInfo"/> class </summary>
        public BibliographicLevelInfo()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="BibliographicLevelInfo"/> class </summary>
        /// <param name="ID"> Primary key for this bibliographic level </param>
        /// <param name="Level"> Text for this bibliographic level </param>
        public BibliographicLevelInfo(int ID, string Level)
        {
            this.ID = ID;
            this.Level = Level;
        }
    }
}
