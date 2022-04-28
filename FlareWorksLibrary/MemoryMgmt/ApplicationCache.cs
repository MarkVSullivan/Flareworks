using System;
using System.Collections.Generic;
using System.Data;
using FlareWorks.Library.Database;
using FlareWorks.Library.Models.ControlledValues;
using FlareWorks.Models.ControlledValues;

namespace FlareWorks.MemoryMgmt
{
    public class ApplicationCache
    {

        public static string Last_Error { get; set; }

        /// <summary> Locak used to ensure no collision or multiple simultaneous attempts 
        /// to pull the data from the database and populate the controlled values </summary>
        private static readonly Object controlledValuesLock = new Object();

        // PRIVATE values
        private static List<BibliographicLevelInfo> bibliographicLevels;
        private static List<CatalogingTypeInfo> catalogingTypes;
        private static List<CleanUpTypeInfo> cleanupTypes;
        private static List<DocumentTypeInfo> documentTypes;
        private static List<ErrorTypeInfo> errorTypes;
        private static List<FederalAgencyInfo> federalAgencies;
        private static List<InstitutionInfo> institutions;
        private static List<LocationInfo> locations;
        private static List<MaterialTypeInfo> materialTypes;
        private static List<RecordTypeInfo> recordTypes;
        private static List<AuthorityRecordTypeInfo> authorityRecordTypes;
        private static List<ItemHolActionTypeInfo> itemHolActionTypes;
        private static List<PccCategoryInfo> pccCategoryTypes;
        private static List<BriefWorkerInfo> workers;

        // Some required dictionaries
        private static Dictionary<int, DocumentTypeInfo> documentTypeDictionary;
        private static Dictionary<int, FederalAgencyInfo> federalAgenciesDictionary;
        private static Dictionary<string, InstitutionInfo> institutionDictionary;
        private static Dictionary<string, MaterialTypeInfo> materialDictionary;

        /// <summary> Gets the complete list of the bibliographic levels controlled value </summary>
        public static List<BibliographicLevelInfo> BibliographicLevels
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (bibliographicLevels == null)
                    {
                        populate_controlled_values();
                    }

                    return bibliographicLevels;
                }
            }
        }

        /// <summary> Gets the complete list of the cataloging types controlled value </summary>
        public static List<CatalogingTypeInfo> CatalogingTypes
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (catalogingTypes == null)
                    {
                        populate_controlled_values();
                    }

                    return catalogingTypes;
                }
            }
        }

        /// <summary> Gets the complete list of the cleanup types controlled value </summary>
        public static List<CleanUpTypeInfo> CleanupTypes
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (cleanupTypes == null)
                    {
                        populate_controlled_values();
                    }

                    return cleanupTypes;
                }
            }
        }

        /// <summary> Gets the complete list of the document types controlled value </summary>
        public static List<DocumentTypeInfo> DocumentTypes
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (documentTypes == null)
                    {
                        populate_controlled_values();
                    }

                    return documentTypes;
                }
            }
        }

        /// <summary> Get a document type, by primary key </summary>
        /// <param name="ID"></param>
        /// <returns> Document type information object, or null </returns>
        public static DocumentTypeInfo Get_Document_Type(int ID)
        {
            // Ensure the dictionary was built
            if ((documentTypeDictionary == null) || (documentTypeDictionary.Count == 0))
            {
                documentTypeDictionary = new Dictionary<int, DocumentTypeInfo>();
                foreach (DocumentTypeInfo thisDocument in DocumentTypes)
                {
                    documentTypeDictionary[thisDocument.ID] = thisDocument;
                }
            }

            // Was there a match?
            if (documentTypeDictionary.ContainsKey(ID))
                return documentTypeDictionary[ID];

            // Otherwise NULL
            return null;
        }

        /// <summary> Gets the complete list of the error types controlled value </summary>
        public static List<ErrorTypeInfo> ErrorTypes
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (errorTypes == null)
                    {
                        populate_controlled_values();
                    }

                    return errorTypes;
                }
            }
        }

        /// <summary> Gets the complete list of the federal agAgencies controlled value </summary>
        public static List<FederalAgencyInfo> FederalAgencies
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (federalAgencies == null)
                    {
                        populate_controlled_values();
                    }

                    return federalAgencies;
                }
            }
        }

        /// <summary> Get a federal agency, by primary key </summary>
        /// <param name="ID"></param>
        /// <returns> Federal agency information object, or null </returns>
        public static FederalAgencyInfo Get_Federal_Agency(int ID)
        {
            // Ensure the dictionary was built
            if ((federalAgenciesDictionary == null) || (federalAgenciesDictionary.Count == 0))
            {
                federalAgenciesDictionary = new Dictionary<int, FederalAgencyInfo>();
                foreach (FederalAgencyInfo thisAgency in FederalAgencies)
                {
                    federalAgenciesDictionary[thisAgency.ID] = thisAgency;
                }
            }

            // Was there a match?
            if (federalAgenciesDictionary.ContainsKey(ID))
                return federalAgenciesDictionary[ID];

            // Otherwise NULL
            return null;
        }

        /// <summary> Gets the complete list of the institutions controlled value </summary>
        public static List<InstitutionInfo> Institutions
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (institutions == null)
                    {
                        populate_controlled_values();
                    }

                    return institutions;
                }
            }
        }

        /// <summary> Get an institution, by institution name </summary>
        /// <param name="InstitutionName"> Full name of institution </param>
        /// <returns> Institution information object, or null </returns>
        public static InstitutionInfo Get_Institution(string InstitutionName)
        {
            // Ensure the dictionary was built
            if ((institutionDictionary == null) || (institutionDictionary.Count == 0))
            {
                institutionDictionary = new Dictionary<string, InstitutionInfo>();
                foreach (InstitutionInfo thisDocument in Institutions)
                {
                    institutionDictionary[thisDocument.Code] = thisDocument;
                }
            }

            // Was there a match?
            if (institutionDictionary.ContainsKey(InstitutionName))
                return institutionDictionary[InstitutionName];

            // Otherwise NULL
            return null;
        }

        /// <summary> Gets the complete list of the locations controlled value </summary>
        public static List<LocationInfo> Locations
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (locations == null)
                    {
                        populate_controlled_values();
                    }

                    return locations;
                }
            }
        }

        /// <summary> Gets the complete list of the material types controlled value </summary>
        public static List<MaterialTypeInfo> MaterialTypes
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (materialTypes == null)
                    {
                        populate_controlled_values();
                    }

                    return materialTypes;
                }
            }
        }

        /// <summary> Get a material type, by material text </summary>
        /// <param name="MaterialType"> Material type text </param>
        /// <returns> Material type information object, or null </returns>
        public static MaterialTypeInfo Get_Material_Type(string MaterialType)
        {
            // Ensure the dictionary was built
            if ((materialDictionary == null) || (materialDictionary.Count == 0))
            {
                materialDictionary = new Dictionary<string, MaterialTypeInfo>();
                foreach (MaterialTypeInfo thisDocument in MaterialTypes)
                {
                    materialDictionary[thisDocument.Text] = thisDocument;
                }
            }

            // Was there a match?
            if (materialDictionary.ContainsKey(MaterialType))
                return materialDictionary[MaterialType];

            // Otherwise NULL
            return null;
        }

        /// <summary> Gets the complete list of the record types controlled value </summary>
        public static List<RecordTypeInfo> RecordTypes
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (recordTypes == null)
                    {
                        populate_controlled_values();
                    }

                    return recordTypes;
                }
            }
        }

        /// <summary> Gets the complete list of the authority record types </summary>
        public static List<AuthorityRecordTypeInfo> AuthorityRecordTypes
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (authorityRecordTypes == null)
                    {
                        populate_controlled_values();
                    }

                    return authorityRecordTypes;
                }
            }
        }

        /// <summary> Gets the complete list of the item HOL action types </summary>
        public static List<ItemHolActionTypeInfo> ItemHolActionTypes
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (itemHolActionTypes == null)
                    {
                        populate_controlled_values();
                    }

                    return itemHolActionTypes;
                }
            }
        }

        /// <summary> Gets the complete list of the PCC Category types </summary>
        public static List<PccCategoryInfo> PccCategoryTypes
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (pccCategoryTypes == null)
                    {
                        populate_controlled_values();
                    }

                    return pccCategoryTypes;
                }
            }
        }

        /// <summary> Gets the complete list of workers who have done some work </summary>
        public static List<BriefWorkerInfo> Workers
        {
            get
            {
                lock (controlledValuesLock)
                {
                    if (workers == null)
                    {
                        populate_controlled_values();
                    }

                    return workers;
                }
            }
        }

        private static void populate_controlled_values()
        {
            Last_Error = String.Empty;

            try
            {
                // Get the data
                DataSet controlledValues = DatabaseGateway.Get_Controlled_Values();

                // Get the bibliographic levels
                bibliographicLevels = new List<BibliographicLevelInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[0].Rows)
                {
                    BibliographicLevelInfo thisObject = new BibliographicLevelInfo
                    {
                        Level = thisRow[0].ToString(),
                        ID = Int32.Parse(thisRow[1].ToString())
                    };
                    bibliographicLevels.Add(thisObject);
                }

                // Get the cataloging types
                catalogingTypes = new List<CatalogingTypeInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[1].Rows)
                {
                    CatalogingTypeInfo thisObject = new CatalogingTypeInfo
                    {
                        Text = thisRow[0].ToString(),
                        ID = Int32.Parse(thisRow[1].ToString())
                    };
                    catalogingTypes.Add(thisObject);
                }

                // Get the cleanup types
                cleanupTypes = new List<CleanUpTypeInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[2].Rows)
                {
                    CleanUpTypeInfo thisObject = new CleanUpTypeInfo
                    {
                        Text = thisRow[0].ToString(),
                        Description = thisRow[1].ToString(),
                        ID = Int32.Parse(thisRow[2].ToString())
                    };
                    cleanupTypes.Add(thisObject);
                }

                // Get the document types
                documentTypes = new List<DocumentTypeInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[3].Rows)
                {
                    DocumentTypeInfo thisObject = new DocumentTypeInfo
                    {
                        Text = thisRow[0].ToString(),
                        Description = thisRow[1].ToString(),
                        ID = Int32.Parse(thisRow[2].ToString())
                    };
                    documentTypes.Add(thisObject);
                }

                // Get the error types
                errorTypes = new List<ErrorTypeInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[4].Rows)
                {
                    ErrorTypeInfo thisObject = new ErrorTypeInfo
                    {
                        Text = thisRow[0].ToString(),
                        Description = thisRow[1].ToString(),
                        ID = Int32.Parse(thisRow[2].ToString())
                    };
                    errorTypes.Add(thisObject);
                }

                // Get the federal agencies
                federalAgencies = new List<FederalAgencyInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[5].Rows)
                {
                    FederalAgencyInfo thisObject = new FederalAgencyInfo
                    {
                        Agency = thisRow[0].ToString(),
                        ID = Int32.Parse(thisRow[1].ToString())
                    };
                    federalAgencies.Add(thisObject);
                }

                // Get the institutions
                institutions = new List<InstitutionInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[6].Rows)
                {
                    InstitutionInfo thisObject = new InstitutionInfo
                    {
                        Code = thisRow[0].ToString(),
                        Name = thisRow[1].ToString(),
                        ID = Int32.Parse(thisRow[2].ToString())
                    };
                    institutions.Add(thisObject);
                }

                // Get the locations
                locations = new List<LocationInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[7].Rows)
                {
                    LocationInfo thisObject = new LocationInfo
                    {
                        Code = thisRow[0].ToString(),
                        Name = thisRow[1].ToString(),
                        ID = Int32.Parse(thisRow[2].ToString())
                    };
                    locations.Add(thisObject);
                }

                // Get the material types
                materialTypes = new List<MaterialTypeInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[8].Rows)
                {
                    MaterialTypeInfo thisObject = new MaterialTypeInfo
                    {
                        Text = thisRow[0].ToString(),
                        ID = Int32.Parse(thisRow[1].ToString())
                    };
                    materialTypes.Add(thisObject);
                }

                // Get the record types
                recordTypes = new List<RecordTypeInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[9].Rows)
                {
                    RecordTypeInfo thisObject = new RecordTypeInfo
                    {
                        Text = thisRow[0].ToString(),
                        Description = thisRow[1].ToString(),
                        ID = Int32.Parse(thisRow[2].ToString())
                    };
                    recordTypes.Add(thisObject);
                }

                // Get the authority record types
                authorityRecordTypes = new List<AuthorityRecordTypeInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[10].Rows)
                {
                    AuthorityRecordTypeInfo thisObject = new AuthorityRecordTypeInfo
                    {
                        RecordType = thisRow[0].ToString(),
                        ID = Int32.Parse(thisRow[1].ToString())
                    };
                    authorityRecordTypes.Add(thisObject);
                }

                // Get the Item HOL action types
                itemHolActionTypes = new List<ItemHolActionTypeInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[11].Rows)
                {
                    ItemHolActionTypeInfo thisObject = new ItemHolActionTypeInfo
                    {
                        ActionType = thisRow[0].ToString(),
                        ID = Int32.Parse(thisRow[1].ToString())
                    };
                    itemHolActionTypes.Add(thisObject);
                }

                // Get the PCC category
                pccCategoryTypes = new List<PccCategoryInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[12].Rows)
                {
                    PccCategoryInfo thisObject = new PccCategoryInfo
                    {
                        Category = thisRow[0].ToString(),
                        ID = Int32.Parse(thisRow[1].ToString())
                    };
                    pccCategoryTypes.Add(thisObject);
                }

                // Get the list of workers
                workers = new List<BriefWorkerInfo>();
                foreach (DataRow thisRow in controlledValues.Tables[13].Rows)
                {
                    BriefWorkerInfo thisObject = new BriefWorkerInfo
                    {
                        Name = thisRow[0].ToString(),
                        ID = Int32.Parse(thisRow[1].ToString())
                    };
                    workers.Add(thisObject);
                }

            }
            catch (Exception ee)
            {
                Last_Error = ee.Message;
            }   
        }
    }
}
