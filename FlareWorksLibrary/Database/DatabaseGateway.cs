using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlareWorks.Library.Models.ControlledValues;
using FlareWorks.Library.Models.QC;
using FlareWorks.Library.Tools;
using FlareWorks.MemoryMgmt;
using FlareWorks.Models.ControlledValues;
using FlareWorks.Models.Users;
using FlareWorks.Models.Work;

namespace FlareWorks.Library.Database
{
    public static class DatabaseGateway
    {
        /// <summary> Database connection string </summary>
        public static string DbConnectionString { get; set; }

        public static string LastError { get; set; }

        /// <summary> Static constructor initializes the <see cref="DatabaseGateway"/> class </summary>
        static DatabaseGateway()
        {
            // Set the database connection string from the app.config file
            string dbConnection = ConfigurationManager.ConnectionStrings["FlareworksDB"].ConnectionString;
            DbConnectionString = dbConnection;
        }

        /// <summary> Returns the complete set of controlled values to display </summary>
        /// <returns> DataSet with all the controlled values </returns>
        public static DataSet Get_Controlled_Values()
        {

                DataSet returnedSet = new DataSet();
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("Get_Controlled_Values", connection)
                    {
                        SelectCommand = { CommandType = CommandType.StoredProcedure }
                    };

                    adapter.Fill(returnedSet);
                }

                return returnedSet;

        }

        /// <returns> DataSet with all the user dashboard data to display </returns>
        public static DataSet Get_User_Dashboard_Data( string Worker )
        {

            DataSet returnedSet = new DataSet();
            using (SqlConnection connection = new SqlConnection(DbConnectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("Get_User_Dashboard_Data", connection)
                {
                    SelectCommand = { CommandType = CommandType.StoredProcedure }
                };
                adapter.SelectCommand.Parameters.Add("Worker", SqlDbType.VarChar).Value = Worker;

                adapter.Fill(returnedSet);
            }

            return returnedSet;

        }

        /// <returns> DataSet with all the quality control admin dashboard data to display </returns>
        public static DataSet Get_QC_Dashboard_Data( string FilterLocation, string FilterUser, int UserID )
        {

            DataSet returnedSet = new DataSet();
            using (SqlConnection connection = new SqlConnection(DbConnectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("Get_QC_Dashboard_Data", connection)
                {
                    SelectCommand = { CommandType = CommandType.StoredProcedure }
                };

                adapter.SelectCommand.Parameters.AddWithValue("FilterLocation", FilterLocation);
                adapter.SelectCommand.Parameters.AddWithValue("FilterUser", FilterUser);
                adapter.SelectCommand.Parameters.AddWithValue("UserID", UserID);

                adapter.Fill(returnedSet);
            }

            return returnedSet;

        }

        #region Methods for working with the user objects

        /// <summary> Checks to see if a username or email exist </summary>
        /// <param name="UserName"> Username to check</param>
        /// <param name="Email"> Email address to check</param>
        /// <param name="UserNameExists"> [OUT] Flag indicates if the username exists</param>
        /// <param name="EmailExists"> [OUT] Flag indicates if the email exists </param>
        /// <returns> TRUE if successful, otherwise FALSE</returns>
        /// <remarks> This calls the 'mySobek_UserName_Exists' stored procedure<br /><br />
        /// This is used to enforce uniqueness during registration </remarks> 
        public static bool UserName_Exists(string UserName, string Email, out bool UserNameExists, out bool EmailExists)
        {

            try
            {
                // Pull the database information
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("Check_UserName_Exists", connection)
                    {
                        SelectCommand = { CommandType = CommandType.StoredProcedure }
                    };
                    adapter.SelectCommand.Parameters.AddWithValue("username", UserName );
                    adapter.SelectCommand.Parameters.AddWithValue("email", Email);

                    SqlParameter userExists = adapter.SelectCommand.Parameters.AddWithValue("UserName_Exists", true);
                    userExists.Direction = ParameterDirection.InputOutput;

                    SqlParameter emailExists = adapter.SelectCommand.Parameters.AddWithValue("Email_Exists", true);
                    emailExists.Direction = ParameterDirection.InputOutput;

                    connection.Open();
                    adapter.SelectCommand.ExecuteNonQuery();
                    connection.Close();

                    UserNameExists = Convert.ToBoolean(userExists.Value);
                    EmailExists = Convert.ToBoolean(emailExists.Value);
                    return true;

                }
            }
            catch (Exception ee)
            {
                UserNameExists = true;
                EmailExists = true;
                return false;
            }
        }

        /// <summary> Register a new user in the system </summary>
        /// <param name="UserName"> Username for this new user </param>
        /// <param name="Name"> Name of this user </param>
        /// <param name="Email"> Email address for this new user </param>
        /// <param name="Password"> Password for this new user </param>
        /// <param name="Location"> Location where this user self-identified as working </param>
        /// <returns> TRUE if successful, otherwise FALSE</returns>
        /// <remarks> This calls the 'Register_User' stored procedure</remarks> 
        public static bool Register_User(string UserName, string Password, string Name, string Email, string Location)
        {
            try
            {
                const string SALT = "This is my salt to add to the password";
                string encryptedPassword = SecurityInfo.SHA1_EncryptString(Password + SALT);

                // Pull the database information
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("Register_User", connection)
                    {
                        SelectCommand = { CommandType = CommandType.StoredProcedure }
                    };
                    adapter.SelectCommand.Parameters.AddWithValue("username", UserName);
                    adapter.SelectCommand.Parameters.AddWithValue("password", encryptedPassword);
                    adapter.SelectCommand.Parameters.AddWithValue("name", Name);
                    adapter.SelectCommand.Parameters.AddWithValue("email", Email);
                    adapter.SelectCommand.Parameters.AddWithValue("location", Location);

                    connection.Open();
                    adapter.SelectCommand.ExecuteNonQuery();
                    connection.Close();

                    return true;
                }
            }
            catch (Exception ee)
            {
                return false;
            }
        }

        /// <summary> Save changes to an existing user </summary>
        /// <param name="EditUser"> User object to save </param>
        /// <returns> TRUE if successful, otherwise FALSE</returns>
        /// <remarks> This calls the 'Save_User' stored procedure</remarks> 
        public static bool Save_User(UserInfo EditUser)
        {
            try
            {
                // Pull the database information
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("Save_User", connection)
                    {
                        SelectCommand = { CommandType = CommandType.StoredProcedure }
                    };
                    adapter.SelectCommand.Parameters.AddWithValue("userid", EditUser.PrimaryKey);
                    adapter.SelectCommand.Parameters.AddWithValue("display_name", EditUser.DisplayName);
                    adapter.SelectCommand.Parameters.AddWithValue("email", EditUser.Email);
                    adapter.SelectCommand.Parameters.AddWithValue("location", EditUser.Location.ID);
                    adapter.SelectCommand.Parameters.AddWithValue("active", !EditUser.Disabled);

                    adapter.SelectCommand.Parameters.AddWithValue("@cataloging_specialist", EditUser.Permissions.CatalogingSpecialist);

                    adapter.SelectCommand.Parameters.AddWithValue("can_qc", EditUser.Permissions.CanQC);
                    adapter.SelectCommand.Parameters.AddWithValue("can_run_reports", EditUser.Permissions.CanRunReports);
                    adapter.SelectCommand.Parameters.AddWithValue("can_advanced_search", EditUser.Permissions.CanRunReports || EditUser.Permissions.CanQC);
                    adapter.SelectCommand.Parameters.AddWithValue("can_add_to_pull_list", EditUser.Permissions.CanAddToPullLists);
                    adapter.SelectCommand.Parameters.AddWithValue("is_pull_list_admin", EditUser.Permissions.IsPullListAdmin);
                    adapter.SelectCommand.Parameters.AddWithValue("is_system_admin", EditUser.Permissions.IsSystemAdmin);

                    connection.Open();
                    adapter.SelectCommand.ExecuteNonQuery();
                    connection.Close();

                    return true;

                }
            }
            catch (Exception ee)
            {
                return false;
            }
        }


        /// <summary> Sets a user's password to the newly provided one </summary>
        /// <param name="UserID"> Primary key for this user from the database </param>
        /// <param name="NewPassword"> New password (unencrypted) to set for this user </param>
        /// <param name="IsTemporaryPassword"> Flag indicates if this is a temporary password that must be reset the first time the user logs on</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> TRUE if successful, otherwsie FALSE  </returns>
        /// <remarks> This calls the 'mySobek_Reset_User_Password' stored procedure</remarks> 
        public static bool Reset_User_Password(int UserID, string NewPassword, bool IsTemporaryPassword)
        {
            const string SALT = "This is my salt to add to the password";
            string encryptedPassword = SecurityInfo.SHA1_EncryptString(NewPassword + SALT);

            try
            {
                // Pull the database information
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("Reset_User_Password", connection)
                    {
                        SelectCommand = { CommandType = CommandType.StoredProcedure }
                    };
                    adapter.SelectCommand.Parameters.AddWithValue("userid", UserID);
                    adapter.SelectCommand.Parameters.AddWithValue("password", encryptedPassword);
                    adapter.SelectCommand.Parameters.AddWithValue("is_temporary", IsTemporaryPassword);

                    connection.Open();
                    adapter.SelectCommand.ExecuteNonQuery();
                    connection.Close();

                    return true;
                }
            }
            catch (Exception ee)
            {
                return false;
            }
        }

        public static DataTable Get_All_Users()
        {
            try
            {
                // Pull the database information
                DataSet returnedSet = new DataSet();
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("Get_All_Users", connection)
                    {
                        SelectCommand = { CommandType = CommandType.StoredProcedure }
                    };
                    adapter.Fill(returnedSet);
                }

                // If no row returned, it must not be a valid user
                if ((returnedSet.Tables.Count == 0) || (returnedSet.Tables[0].Rows.Count == 0))
                    return null;

                // Return the fully built user object
                return returnedSet.Tables[0];
            }
            catch (Exception ee)
            {
                return null;
            }
        }

        public static UserInfo Get_User(string UserName, string Password )
        {
            try
            {
                const string SALT = "This is my salt to add to the password";
                string encryptedPassword = SecurityInfo.SHA1_EncryptString(Password + SALT);

                // Pull the database information
                DataSet returnedSet = new DataSet();
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("Get_User_By_Password", connection)
                    {
                        SelectCommand = { CommandType = CommandType.StoredProcedure }
                    };
                    adapter.SelectCommand.Parameters.AddWithValue("username", UserName);
                    adapter.SelectCommand.Parameters.AddWithValue("password", encryptedPassword);

                    adapter.Fill(returnedSet);
                }

                // If no row returned, it must not be a valid user
                if ((returnedSet.Tables.Count == 0) || (returnedSet.Tables[0].Rows.Count == 0))
                    return null;

                // Start to build the return user object
                UserInfo returnValue = build_user_from_dataset(returnedSet);

                // Return the fully built user object
                return returnValue;
            }
            catch (Exception ee)
            {
                return null;
            }
        }

        public static UserInfo Get_User(int UserID)
        {
            try
            {
                // Pull the database information
                DataSet returnedSet = new DataSet();
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("Get_User_By_ID", connection)
                    {
                        SelectCommand = { CommandType = CommandType.StoredProcedure }
                    };
                    adapter.SelectCommand.Parameters.Add("UserID", SqlDbType.Int).Value = UserID;

                    adapter.Fill(returnedSet);
                }

                // If no row returned, it must not be a valid user
                if ((returnedSet.Tables.Count == 0) || (returnedSet.Tables[0].Rows.Count == 0))
                    return null;

                // Start to build the return user object
                UserInfo returnValue = build_user_from_dataset(returnedSet);
                
                // Return the fully built user object
                return returnValue;
            }
            catch (Exception ee)
            {
                return null;
            }
        }

        private static UserInfo build_user_from_dataset(DataSet ReturnedSet)
        {
            // Get the main user table
            DataRow userRow = ReturnedSet.Tables[0].Rows[0];

            // Start to build the return user object
            UserInfo returnValue = new UserInfo();

            // Populate the basic information
            returnValue.PrimaryKey = int.Parse(userRow["UserID"].ToString());
            returnValue.UserName = userRow["UserName"].ToString();
            returnValue.DisplayName = userRow["DisplayName"].ToString();
            returnValue.Email = userRow["Email"].ToString();
            returnValue.DateAdded = DateTime.Parse(userRow["DateAdded"].ToString());
            returnValue.PendingApproval = bool.Parse(userRow["PendingApproval"].ToString());
            returnValue.Disabled = bool.Parse(userRow["Disabled"].ToString());
            returnValue.UserName = userRow["UserName"].ToString();
            returnValue.FullName = userRow["FullName"].ToString();
            returnValue.TemporaryPassword = bool.Parse(userRow["isTemporary_Password"].ToString());


            // Get the location associated with this user
            int locationid = int.Parse(userRow["LocationID"].ToString());
            if (locationid > 0)
            {
                foreach (LocationInfo location in ApplicationCache.Locations.Where(location => location.ID == locationid))
                {
                    returnValue.Location = location;
                    break;
                }
            }

            // Load the permissions
            returnValue.Permissions.CatalogingSpecialist = bool.Parse(userRow["CatalogingSpecialist"].ToString());
            returnValue.Permissions.CanAddToPullLists = bool.Parse(userRow["CanAddToPullLists"].ToString());
            returnValue.Permissions.CanAdvancedSearch = bool.Parse(userRow["CanAdvancedSearch"].ToString());
            returnValue.Permissions.CanQC = bool.Parse(userRow["CanQC"].ToString());
            returnValue.Permissions.CanRunReports = bool.Parse(userRow["CanRunReports"].ToString());
            returnValue.Permissions.IsPullListAdmin = bool.Parse(userRow["IsPullListAdmin"].ToString());
            returnValue.Permissions.IsSystemAdmin = bool.Parse(userRow["IsSystemAdmin"].ToString());

            // Load information on the last added item
            returnValue.Recents.LastAdded.BiblioLevel = userRow["LastAddedBiblioLevel"].ToString();
            returnValue.Recents.LastAdded.CatalogType = userRow["LastAddedCatalogType"].ToString();
            returnValue.Recents.LastAdded.FormType = userRow["LastAddedFormType"].ToString();
            returnValue.Recents.LastAdded.Institution = userRow["LastAddedInstitution"].ToString();
            returnValue.Recents.LastAdded.MaterialType = userRow["LastAddedMaterialType"].ToString();

            // Load information on the last search
            returnValue.Recents.LastSearch.ByCriteria = userRow["LastSearchByCriteria"].ToString();
            returnValue.Recents.LastSearch.FormType = userRow["LastSearchFormType"].ToString();
            returnValue.Recents.LastSearch.Institution = userRow["LastSearchInstitution"].ToString();
            returnValue.Recents.LastSearch.Location = userRow["LastSearchLocation"].ToString();

            returnValue.Recents.LastQcLocationSelected = userRow["LastQcLocationSelected"].ToString();
            returnValue.Recents.LastQcUserSelected = userRow["LastQcUserSelected"].ToString();


            // Return the fully built user object
            return returnValue;
        }

        #endregion

        #region Methods for dealing with authority work at the item work set level

        /// <summary> Clear all the authority work associated with the item work set </summary>
        /// <param name="ItemWorkSetID"> Primary key to the item work set </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Clear_ItemWorkSet_Authority(int ItemWorkSetID )
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("ItemWorkSet_Authority_Clear", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCommand.Parameters.AddWithValue("ItemWorkSetID", ItemWorkSetID);

                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Add_ItemWorkSet_Authority(int ItemWorkSetID, int AuthorityRecordTypeID, bool OriginalWork)
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("ItemWorkSet_Authority_Add", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };


                sqlCommand.Parameters.AddWithValue("ItemWorkSetID", ItemWorkSetID);
                sqlCommand.Parameters.AddWithValue("AuthorityRecordTypeID", AuthorityRecordTypeID);
                sqlCommand.Parameters.AddWithValue("OriginalWork", OriginalWork);
                sqlCommand.Parameters.AddWithValue("DateAdded", DateTime.Now);


                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }


        #endregion

        public static TitleInfo Get_Title(int TitleID, string AlephNum, string OCLC, int UserID)
        {
            try
            {
                // Pull the database information
                DataSet returnedSet = new DataSet();
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("Get_Title", connection)
                    {
                        SelectCommand = { CommandType = CommandType.StoredProcedure }
                    };
                    adapter.SelectCommand.Parameters.Add("TitleID", SqlDbType.Int).Value = TitleID;
                    adapter.SelectCommand.Parameters.Add("AlephNum", SqlDbType.VarChar).Value = AlephNum;
                    adapter.SelectCommand.Parameters.Add("OCLC", SqlDbType.VarChar).Value = OCLC;
                    adapter.SelectCommand.Parameters.Add("UserID", SqlDbType.Int).Value = UserID;

                    adapter.Fill(returnedSet);
                }

                // If no row returned, it must not be a valid user
                if ((returnedSet.Tables.Count == 0) || (returnedSet.Tables[0].Rows.Count == 0))
                    return null;

                // Get the main user table
                DataRow titleRow = returnedSet.Tables[0].Rows[0];

                // Start to build the return user object
                TitleInfo returnValue = new TitleInfo();

                // Populate the basic information
                returnValue.PrimaryKey = int.Parse(titleRow["TitleID"].ToString());
                returnValue.AlephNum = titleRow["AlephNum"].ToString();
                returnValue.OCLC = titleRow["OCLC"].ToString();
                returnValue.ISSN = titleRow["ISSN"].ToString();
                returnValue.Title = titleRow["Title"].ToString();
                returnValue.DateAdded = DateTime.Parse(titleRow["DateAdded"].ToString());
                returnValue.GeneralNotes = titleRow["GeneralNotes"].ToString();
                returnValue.SendToCataloging = bool.Parse(titleRow["SendToCataloging"].ToString());

                // Add the ISSN confirmed information
                if (titleRow["NoIssnConfirmed"] != DBNull.Value)
                {
                    returnValue.NoIssnConfirmed = true;

                    if (titleRow["NoIssnConfirmedDate"] != DBNull.Value)
                        returnValue.NoIssnConfirmedDate = DateTime.Parse(titleRow["NoIssnConfirmedDate"].ToString());
                    returnValue.NoIssnConfirmedUser = titleRow["NoIssnConfirmedUser"].ToString();
                }

                // Add the bibliographic level
                int bibLevelId = int.Parse(titleRow["BibliographicLevelID"].ToString());
                if (bibLevelId > 0)
                {
                    returnValue.BibliographicLevel = new BibliographicLevelInfo(bibLevelId, titleRow["BibliographicLevel"].ToString());
                }

                //// Add the cataloging type
                //int catTypeId = int.Parse(titleRow["CatalogingTypeID"].ToString());
                //if (catTypeId > 0)
                //{
                //    returnValue.CatalogingType = new CatalogingTypeInfo(catTypeId, titleRow["CatalogingType"].ToString());
                //}

                // Add the document type
                int docTypeId = int.Parse(titleRow["DocumentTypeID"].ToString());
                if (docTypeId > 0)
                {
                    returnValue.DocumentType = new DocumentTypeInfo(docTypeId, titleRow["DocumentType"].ToString(), String.Empty);
                }

                // Add the federal agency
                returnValue.FederalAgency = titleRow["FederalAgency"].ToString();

                // Add the record type
                int recordTypeId = int.Parse(titleRow["RecordTypeID"].ToString());
                if (recordTypeId > 0)
                {
                    returnValue.RecordType = new RecordTypeInfo(recordTypeId, titleRow["RecordType"].ToString(), titleRow["RecordType"].ToString());
                }

                // If this was just a way to return the defaults, there won't be the next tables
                if (returnedSet.Tables.Count > 1)
                {
                    // The second table is the Record Cleanup table
                    foreach (DataRow cleanUpRow in returnedSet.Tables[1].Rows)
                    {
                        TitleInfo_CleanUp cleanUp = new TitleInfo_CleanUp();
                        cleanUp.PrimaryKey = int.Parse(cleanUpRow["CleanUpId"].ToString());
                        cleanUp.CleanUpType = cleanUpRow["CleanUpType"].ToString();
                        cleanUp.DateAdded = DateTime.Parse(cleanUpRow["DateAdded"].ToString());
                        cleanUp.AddedByUser = cleanUpRow["UserName"].ToString();

                        if (cleanUpRow["OtherDescription"] != DBNull.Value) cleanUp.OtherDescription = cleanUpRow["OtherDescription"].ToString();

                        returnValue.Tasks.Add(cleanUp);
                        returnValue.OriginalTasks.Add(cleanUp);
                    }


                    // Get the list of item work from the third table 
                    foreach (DataRow thisRow in returnedSet.Tables[2].Rows)
                    {
                        ItemWork itemWork = new ItemWork();
                        itemWork.PrimaryKey = int.Parse(thisRow["ItemWorkID"].ToString());
                        itemWork.ItemsSentToTray = int.Parse(thisRow["ItemsSentToTray"].ToString());
                        itemWork.ItemsWithdrawn = int.Parse(thisRow["ItemsWithdrawnDupes"].ToString());
                        itemWork.ItemsDamaged = int.Parse(thisRow["ItemsDamaged"].ToString());
                        itemWork.LastCopy = bool.Parse(thisRow["LastCopy"].ToString());
                        itemWork.Institution = new InstitutionInfo(int.Parse(thisRow["InstitutionID"].ToString()), thisRow["Institution"].ToString(), thisRow["InstitutionCode"].ToString());
                        itemWork.MaterialType = new MaterialTypeInfo(int.Parse(thisRow["MaterialTypeID"].ToString()), thisRow["MaterialType"].ToString());
                        itemWork.Worker = thisRow["WorkerName"].ToString();
                        itemWork.DateAdded = DateTime.Parse(thisRow["DateAdded"].ToString());

                        if (thisRow["DateSubmitted"] != DBNull.Value) itemWork.DateSubmitted = DateTime.Parse(thisRow["DateSubmitted"].ToString());
                        if (thisRow["DateLastUpdated"] != DBNull.Value) itemWork.DateLastUpdated = DateTime.Parse(thisRow["DateLastUpdated"].ToString());
                        if (thisRow["DateRejected"] != DBNull.Value) itemWork.DateRejected = DateTime.Parse(thisRow["DateRejected"].ToString());
                        if (thisRow["DateApproved"] != DBNull.Value) itemWork.DateApproved = DateTime.Parse(thisRow["DateApproved"].ToString());



                        returnValue.Items.Add(itemWork);
                    }
                }


                // Return the fully built user object
                return returnValue;
            }
            catch (Exception ee)
            {
                LastError = ee.Message;
                return null;
            }
        }

        public static Tuple<int,string> Save_Basic_Title_Info(TitleInfo Info, int UserID)
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return new Tuple<int, string>(-1, "ERROR: Unable to open connection to the database");
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("Save_Basic_Title_Info", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCommand.Parameters.AddWithValue("TitleID", Info.PrimaryKey);
                sqlCommand.Parameters.AddWithValue("AlephNum", Info.AlephNum);
                sqlCommand.Parameters.AddWithValue("OCLC", Info.OCLC);
                sqlCommand.Parameters.AddWithValue("Title", Info.Title ?? String.Empty);
                sqlCommand.Parameters.AddWithValue("ISSN", Info.ISSN ?? String.Empty);
                sqlCommand.Parameters.AddWithValue("BibliographicLevelID", Info.BibliographicLevel.ID);


                if (Info.DocumentType == null)
                    sqlCommand.Parameters.AddWithValue("DocumentTypeID", DBNull.Value);
                else
                    sqlCommand.Parameters.AddWithValue("DocumentTypeID", Info.DocumentType.ID);

                if (String.IsNullOrEmpty(Info.FederalAgency))
                    sqlCommand.Parameters.AddWithValue("FederalAgency", DBNull.Value);
                else
                    sqlCommand.Parameters.AddWithValue("FederalAgency", Info.FederalAgency);
                sqlCommand.Parameters.AddWithValue("RecordTypeID", Info.RecordType.ID);

                sqlCommand.Parameters.AddWithValue("SendToCataloging", Info.SendToCataloging);
                sqlCommand.Parameters.AddWithValue("GeneralNotes", Info.GeneralNotes);
                sqlCommand.Parameters.AddWithValue("UserID", UserID);

                // Add the titleid that returns the titleid for new items
                SqlParameter idParam = sqlCommand.Parameters.AddWithValue("@TitleId_Return", -1);
                idParam.Direction = ParameterDirection.InputOutput;

                // A message parameter returns any error messages
                SqlParameter msgParam = sqlCommand.Parameters.AddWithValue("@Message", String.Empty.PadLeft(255,' '));
                msgParam.Direction = ParameterDirection.InputOutput;

                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return new Tuple<int, string>(-1, "ERROR: Unable to save to the database (" + ex.Message + ")");
                }

                // Get the primery key and message
                int titleId = int.Parse(idParam.Value.ToString());
                string message = msgParam.Value.ToString();

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return new Tuple<int, string>(-1, "ERROR: Unable to close connection to the database");
                }

                return new Tuple<int, string>(titleId, message); 
            }
        }

        public static bool Save_Title_CleanUp(int TitleId, int CleanUpId, string OtherDescription, int UserId)
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("Save_Title_CleanUp", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCommand.Parameters.AddWithValue("TitleID", TitleId);
                sqlCommand.Parameters.AddWithValue("CleanUpID", CleanUpId);

                if ( !String.IsNullOrEmpty(OtherDescription))
                    sqlCommand.Parameters.AddWithValue("OtherDescription", OtherDescription);
                else
                    sqlCommand.Parameters.AddWithValue("OtherDescription", DBNull.Value);

                sqlCommand.Parameters.AddWithValue("UserID", UserId);

                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Remove_Title_CleanUp(int TitleId, int CleanUpId,  int UserId)
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("Remove_Title_CleanUp", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCommand.Parameters.AddWithValue("TitleID", TitleId);
                sqlCommand.Parameters.AddWithValue("CleanUpID", CleanUpId);
                sqlCommand.Parameters.AddWithValue("UserID", UserId);

                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Save_ItemWork_Info(ItemWork Info, int TitleID )
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("Save_ItemWork_Info", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCommand.Parameters.AddWithValue("ItemWorkID", Info.PrimaryKey);
                sqlCommand.Parameters.AddWithValue("TitleID", TitleID);
                sqlCommand.Parameters.AddWithValue("Worker", Info.Worker);
                sqlCommand.Parameters.AddWithValue("ItemsSentToTray", Info.ItemsSentToTray);
                sqlCommand.Parameters.AddWithValue("ItemsWithdrawnDupes", Info.ItemsWithdrawn);
                sqlCommand.Parameters.AddWithValue("ItemsDamaged", Info.ItemsDamaged);

                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Delete_ItemWork_Info(int ItemWorkID )
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("Delete_ItemWork_Info", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCommand.Parameters.AddWithValue("ItemWorkID", ItemWorkID);

                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }



        public static string Submit_For_QC(int TitleID, int WorkerID)
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return "Exception caught: " + ex.Message;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("Submit_For_QC", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCommand.Parameters.AddWithValue("titleid", TitleID);
                sqlCommand.Parameters.AddWithValue("workerid", WorkerID);

                SqlParameter msgParam = sqlCommand.Parameters.AddWithValue("message", String.Empty.PadRight(200, ' '));
                msgParam.Direction = ParameterDirection.InputOutput;
 
                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return "Exception caught: " + ex.Message;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return "Exception caught closing database connection: " + ex.Message;
                }

                return (msgParam.Value == DBNull.Value) ? "Not successful" : msgParam.Value.ToString();
            }

            return "No exception caught";
        }

        /// <summary> Get a workset by primary key of the workset </summary>
        /// <param name="WorkSetID"> Primary key of the item work set from the database </param>
        /// <returns> Fully built WorkSet object </returns>
        public static WorkSet Get_Workset(int WorkSetID)
        {
            try
            {
                // Pull the database information
                DataSet returnedSet = new DataSet();
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("Get_Qc_Workset_Info", connection)
                    {
                        SelectCommand = { CommandType = CommandType.StoredProcedure }
                    };
                    adapter.SelectCommand.Parameters.Add("WorkSetID", SqlDbType.Int).Value = WorkSetID;

                    adapter.Fill(returnedSet);
                }

                // If no row returned, it must not be a valid user
                if ((returnedSet.Tables.Count == 0) || (returnedSet.Tables[0].Rows.Count == 0))
                    return null;

                // Build and return the object
                return build_workset(returnedSet);
            }
            catch (Exception ee)
            {
                LastError = ee.Message;
                return null;
            }
        }

        /// <summary> Get an unsubmitted workset by title and user </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <param name="TitleID">The title identifier.</param>
        /// <returns> Fully built WorkSet object, with -1 as primary key if no workset exists </returns>
        public static WorkSet Get_Workset(int UserID, int TitleID)
        {
            try
            {
                // Pull the database information
                DataSet returnedSet = new DataSet();
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("Get_Unsubmitted_Workset_By_User_Title", connection)
                    {
                        SelectCommand = {CommandType = CommandType.StoredProcedure}
                    };
                    adapter.SelectCommand.Parameters.Add("UserID", SqlDbType.Int).Value = UserID;
                    adapter.SelectCommand.Parameters.Add("TitleID", SqlDbType.Int).Value = TitleID;

                    adapter.Fill(returnedSet);
                }

                // Was this empty ( no unsubmitted )?
                if ((returnedSet.Tables.Count == 0) || (returnedSet.Tables[0].Rows.Count == 0))
                    return new WorkSet { PrimaryKey = -1, TitleID = @TitleID };

                // Build and return the object
                return build_workset(returnedSet);
            }
            catch (Exception ee)
            {
                LastError = ee.Message;
                return null;
            }
        }

        private static WorkSet build_workset(DataSet returnedSet)
        {
            // Get the main user table
            DataRow titleRow = returnedSet.Tables[0].Rows[0];

            // Start to build the return user object
            WorkSet returnValue = new WorkSet();

            // Populate the basic information
            returnValue.PrimaryKey = Int32.Parse(titleRow["ItemWorkSetID"].ToString());
            returnValue.AlephNum = titleRow["AlephNum"].ToString();
            returnValue.TitleID = Int32.Parse(titleRow["TitleID"].ToString());
            returnValue.Worker = titleRow["WorkerName"].ToString();
            returnValue.ProcessorNotes = titleRow["ProcessorNotes"].ToString();
            returnValue.QcNotes = titleRow["QcNotes"].ToString();
            returnValue.QcWorker = titleRow["QcWorker"].ToString();
            returnValue.LastCopy = bool.Parse(titleRow["LastCopy"].ToString());
            returnValue.LastCopyInstitution = titleRow["LastCopyInstitution"].ToString();
            returnValue.Institution = new InstitutionInfo(int.Parse(titleRow["InstitutionID"].ToString()), titleRow["Institution"].ToString(), titleRow["InstitutionCode"].ToString());
            returnValue.MaterialType = new MaterialTypeInfo(int.Parse(titleRow["MaterialTypeID"].ToString()), titleRow["MaterialType"].ToString());
            returnValue.ItemHol_EditCount = Int32.Parse(titleRow["ItemHol_EditCount"].ToString());

            if (titleRow["DateSubmitted"] != DBNull.Value) returnValue.DateSubmitted = DateTime.Parse(titleRow["DateSubmitted"].ToString());
            if (titleRow["DateRejected"] != DBNull.Value) returnValue.DateRejected = DateTime.Parse(titleRow["DateRejected"].ToString());
            if (titleRow["DateApproved"] != DBNull.Value) returnValue.DateApproved = DateTime.Parse(titleRow["DateApproved"].ToString());
            if (titleRow["ItemHolActionType"].ToString().Length > 0)
            {
                returnValue.ItemHolActionType = new ItemHolActionTypeInfo
                {
                    ActionType = titleRow["ItemHolActionType"].ToString(),
                    ID = Int32.Parse(titleRow["ItemHolActionTypeID"].ToString())
                };
            }
            if (titleRow["PccCategory"].ToString().Length > 0)
            {
                returnValue.Pcc_NewAuthentication = false;
                returnValue.Pcc_Maintenance = false;
                string pcc_category = titleRow["PccCategory"].ToString();
                string truncated = pcc_category.Split("|".ToCharArray())[0];
                if (pcc_category.ToLower().IndexOf("|new") > 0 ) returnValue.Pcc_NewAuthentication = true;
                if (pcc_category.ToLower().IndexOf("|maintenance") > 0) returnValue.Pcc_Maintenance =true;
                returnValue.PccCategory = new PccCategoryInfo
                {
                    Category = truncated,
                    ID = Int32.Parse(titleRow["PccCategoryID"].ToString())
                };
            }
            if (titleRow["CatalogingType"].ToString().Length > 0)
            {
                returnValue.CatalogingType = new CatalogingTypeInfo
                {
                    Text = titleRow["CatalogingType"].ToString(),
                    ID = Int32.Parse(titleRow["CatalogingTypeID"].ToString())
                };
            }

            // Get the list of item work from the second table 
            foreach (DataRow thisRow in returnedSet.Tables[1].Rows)
            {
                WorkSetItem itemWork = new WorkSetItem();
                itemWork.PrimaryKey = int.Parse(thisRow["ItemWorkID"].ToString());
                itemWork.ItemsSentToTray = int.Parse(thisRow["ItemsSentToTray"].ToString());
                itemWork.ItemsWithdrawn = int.Parse(thisRow["ItemsWithdrawnDupes"].ToString());
                itemWork.ItemsDamaged = int.Parse(thisRow["ItemsDamaged"].ToString());
                itemWork.DateAdded = DateTime.Parse(thisRow["DateAdded"].ToString());
                if (thisRow["DateLastUpdated"] != DBNull.Value)
                    itemWork.DateLastUpdated = DateTime.Parse(thisRow["DateLastUpdated"].ToString());
                returnValue.Items.Add(itemWork);
            }

            // Get the list of errors from the second table
            foreach (DataRow errorRow in returnedSet.Tables[2].Rows)
            {
                WorkSet_Error error = new WorkSet_Error();
                error.ErrorTypeID = int.Parse(errorRow["ErrorTypeID"].ToString());
                error.ErrorType = errorRow["ErrorType"].ToString();
                error.ErrorDescripton = errorRow["ErrorDescription"].ToString();
                error.DateAdded = DateTime.Parse(errorRow["DateAdded"].ToString());
                error.AddedByUser = errorRow["AddedByUser"].ToString();
                error.OtherDesription = errorRow["OtherDescription"].ToString();
                returnValue.Errors.Add(error);
            }

            // Gets the list of authority stuff from the third table
            foreach( DataRow authRow in returnedSet.Tables[3].Rows )
            {
                int id = int.Parse(authRow["AuthorityRecordTypeID"].ToString());
                string type = authRow["AuthorityRecordType"].ToString();
                bool original = bool.Parse(authRow["OriginalWork"].ToString());

                returnValue.AuthorityWork.Add(new WorkSet_AuthorityWork(id, type, original));
            }

            // Return the fully built user object
            return returnValue;
        }

        /// <summary> Creates a new item work set, or return the primary key to an
        /// existing item workset for the title and user indicated </summary>
        /// <param name="UserID"> Primary key for the worker </param>
        /// <param name="TitleID"> Primary key for the title </param>
        /// <param name="InstitutionID"> Primary key for the institution </param>
        /// <param name="MaterialTypeID"> Primary key for the material type </param>
        /// <returns> New primary key for the item work set from the database </returns>
        public static int Create_ItemWorkSet(int UserID, int TitleID, int InstitutionID, int MaterialTypeID)
        {
            int returnValue = -1;

            try
            {
                // Pull the database information
                DataSet returnedSet = new DataSet();
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Create_ItemWorkSet", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add the values going in
                    cmd.Parameters.Add("UserID", SqlDbType.Int).Value = UserID;
                    cmd.Parameters.Add("TitleID", SqlDbType.Int).Value = TitleID;
                    cmd.Parameters.Add("InstitutionID", SqlDbType.Int).Value = InstitutionID;
                    cmd.Parameters.Add("MaterialTypeID", SqlDbType.Int).Value = MaterialTypeID;

                    // Add the return parameter
                    SqlParameter returnParam = cmd.Parameters.Add("SetID", SqlDbType.Int);
                    returnParam.Value = -1;
                    returnParam.Direction = ParameterDirection.InputOutput;

                    // Run the command
                    connection.Open();
                    cmd.ExecuteNonQuery();

                    // Try to get the return value
                    returnValue = Int32.Parse(returnParam.Value.ToString());

                    connection.Close();
                }
            }
            catch (Exception ee)
            {
                LastError = ee.Message;
                return -1;
            }

            return returnValue;
        }

        /// <summary> Save the item work set </summary>
        /// <param name="SetID"> Primary key to this set </param>
        /// <param name="ItemHolActionTypeID">The item hol action type identifier.</param>
        /// <param name="PccCategoryID">The PCC category identifier.</param>
        /// <param name="CatalogingTypeID">The cataloging type identifier.</param>
        /// <param name="ProcessorNotes">The processor notes.</param>
        /// <param name="LastCopy"> Flag indicates this is the last copy for an institution </param>
        /// <param name="LastCopyInstitution"> Institution for which this is the last copy </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Save_ItemWorkSet(int SetID, int InstitutionID, int MaterialTypeID, int ItemHolActionTypeID, 
            int PccCategoryID, int CatalogingTypeID, string ProcessorNotes, bool LastCopy, string LastCopyInstitution, 
            int EditedCount )
        {
            try
            {
                // Pull the database information
                DataSet returnedSet = new DataSet();
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("Save_ItemWorkSet", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add the values going in
                    cmd.Parameters.Add("setid", SqlDbType.Int).Value = SetID;
                    cmd.Parameters.AddWithValue("institutionid", InstitutionID);
                    cmd.Parameters.AddWithValue("materialtypeid", MaterialTypeID);
                    
                    if (ItemHolActionTypeID > 0)
                        cmd.Parameters.Add("itemHolActionTypeID", SqlDbType.Int).Value = ItemHolActionTypeID;
                    else
                        cmd.Parameters.Add("itemHolActionTypeID", SqlDbType.Int).Value = DBNull.Value;

                    if ( PccCategoryID > 0 )
                        cmd.Parameters.Add("pccCategoryID", SqlDbType.Int).Value = PccCategoryID;
                    else
                        cmd.Parameters.Add("pccCategoryID", SqlDbType.Int).Value = DBNull.Value;

                    if (CatalogingTypeID > 0)
                        cmd.Parameters.Add("catalogingTypeID", SqlDbType.Int).Value = CatalogingTypeID;
                    else
                        cmd.Parameters.Add("catalogingTypeID", SqlDbType.Int).Value = DBNull.Value;

                    cmd.Parameters.AddWithValue("processor_notes", ProcessorNotes);
                    cmd.Parameters.AddWithValue("lastcopy", LastCopy);
                    cmd.Parameters.AddWithValue("lastcopy_institution", LastCopyInstitution);
                    cmd.Parameters.AddWithValue("ItemHol_EditCount", EditedCount);


                    // Run the command
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ee)
            {
                LastError = ee.Message;
                return false;
            }

            return true;
        }

        /// <summary> Reject the workset that was subitted for QC  </summary>
        /// <param name="WorkSetID"> Primary key for this workset </param>
        /// <param name="QcNotes"> QC notes for this workset </param>
        /// <param name="QcWorkerID"> Identifier for the worker doing the QC </param>
        /// <param name="ErrorTypes"> List of error types selected for this </param>
        /// <param name="OtherDescription"> Other description </param>
        /// <returns> TRUE if successful, othewise FALSE </returns>
        public static bool Reject_Workset(int WorkSetID, string QcNotes, int QcWorkerID, List<int> ErrorTypes, string OtherDescription)
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("QC_Reject", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCommand.Parameters.AddWithValue("SetID", WorkSetID);
                sqlCommand.Parameters.AddWithValue("QC_Notes", QcNotes);
                sqlCommand.Parameters.AddWithValue("QC_WorkerID", QcWorkerID);
                sqlCommand.Parameters.AddWithValue("Error01", (ErrorTypes.Count > 0 ? ErrorTypes[0] : -1));
                sqlCommand.Parameters.AddWithValue("Error02", (ErrorTypes.Count > 1 ? ErrorTypes[1] : -1));
                sqlCommand.Parameters.AddWithValue("Error03", (ErrorTypes.Count > 2 ? ErrorTypes[2] : -1));
                sqlCommand.Parameters.AddWithValue("Error04", (ErrorTypes.Count > 3 ? ErrorTypes[3] : -1));
                sqlCommand.Parameters.AddWithValue("Error05", (ErrorTypes.Count > 4 ? ErrorTypes[4] : -1));
                sqlCommand.Parameters.AddWithValue("Error06", (ErrorTypes.Count > 5 ? ErrorTypes[5] : -1));
                sqlCommand.Parameters.AddWithValue("Error07", (ErrorTypes.Count > 6 ? ErrorTypes[6] : -1));
                sqlCommand.Parameters.AddWithValue("Error08", (ErrorTypes.Count > 7 ? ErrorTypes[7] : -1));
                sqlCommand.Parameters.AddWithValue("Error09", (ErrorTypes.Count > 8 ? ErrorTypes[8] : -1));
                sqlCommand.Parameters.AddWithValue("Error10", (ErrorTypes.Count > 9 ? ErrorTypes[9] : -1));
                sqlCommand.Parameters.AddWithValue("Error11", (ErrorTypes.Count > 10 ? ErrorTypes[10] : -1));
                sqlCommand.Parameters.AddWithValue("Error12", (ErrorTypes.Count > 11 ? ErrorTypes[11] : -1));
                sqlCommand.Parameters.AddWithValue("Error13", (ErrorTypes.Count > 12 ? ErrorTypes[12] : -1));
                sqlCommand.Parameters.AddWithValue("Error14", (ErrorTypes.Count > 13 ? ErrorTypes[13] : -1));
                sqlCommand.Parameters.AddWithValue("Error15", (ErrorTypes.Count > 14 ? ErrorTypes[14] : -1));
                sqlCommand.Parameters.AddWithValue("Other_Error_Desc", OtherDescription);

                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary> Saves the QC work related to a workset that was subitted for QC  </summary>
        /// <param name="WorkSetID"> Primary key for this workset </param>
        /// <param name="QcNotes"> QC notes for this workset </param>
        /// <param name="QcWorkerID"> Identifier for the worker doing the QC </param>
        /// <param name="ErrorTypes"> List of error types selected for this </param>
        /// <param name="OtherDescription"> Other description </param>
        /// <returns> TRUE if successful, othewise FALSE </returns>
        public static bool Save_Workset_QC(int WorkSetID, string QcNotes, int QcWorkerID, List<int> ErrorTypes, string OtherDescription)
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("QC_Save", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCommand.Parameters.AddWithValue("SetID", WorkSetID);
                sqlCommand.Parameters.AddWithValue("QC_Notes", QcNotes);
                sqlCommand.Parameters.AddWithValue("QC_WorkerID", QcWorkerID);
                sqlCommand.Parameters.AddWithValue("Error01", (ErrorTypes.Count > 0 ? ErrorTypes[0] : -1));
                sqlCommand.Parameters.AddWithValue("Error02", (ErrorTypes.Count > 1 ? ErrorTypes[1] : -1));
                sqlCommand.Parameters.AddWithValue("Error03", (ErrorTypes.Count > 2 ? ErrorTypes[2] : -1));
                sqlCommand.Parameters.AddWithValue("Error04", (ErrorTypes.Count > 3 ? ErrorTypes[3] : -1));
                sqlCommand.Parameters.AddWithValue("Error05", (ErrorTypes.Count > 4 ? ErrorTypes[4] : -1));
                sqlCommand.Parameters.AddWithValue("Error06", (ErrorTypes.Count > 5 ? ErrorTypes[5] : -1));
                sqlCommand.Parameters.AddWithValue("Error07", (ErrorTypes.Count > 6 ? ErrorTypes[6] : -1));
                sqlCommand.Parameters.AddWithValue("Error08", (ErrorTypes.Count > 7 ? ErrorTypes[7] : -1));
                sqlCommand.Parameters.AddWithValue("Error09", (ErrorTypes.Count > 8 ? ErrorTypes[8] : -1));
                sqlCommand.Parameters.AddWithValue("Error10", (ErrorTypes.Count > 9 ? ErrorTypes[9] : -1));
                sqlCommand.Parameters.AddWithValue("Error11", (ErrorTypes.Count > 10 ? ErrorTypes[10] : -1));
                sqlCommand.Parameters.AddWithValue("Error12", (ErrorTypes.Count > 11 ? ErrorTypes[11] : -1));
                sqlCommand.Parameters.AddWithValue("Error13", (ErrorTypes.Count > 12 ? ErrorTypes[12] : -1));
                sqlCommand.Parameters.AddWithValue("Error14", (ErrorTypes.Count > 13 ? ErrorTypes[13] : -1));
                sqlCommand.Parameters.AddWithValue("Error15", (ErrorTypes.Count > 14 ? ErrorTypes[14] : -1));
                sqlCommand.Parameters.AddWithValue("Other_Error_Desc", OtherDescription);

                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary> Approve the workset that was subitted for QC  </summary>
        /// <param name="WorkSetID"> Primary key for this workset </param>
        /// <param name="QcNotes"> QC notes for this workset </param>
        /// <param name="QcWorkerID"> Identifier for the worker doing the QC </param>
        /// <returns> TRUE if successful, othewise FALSE </returns>
        public static bool Approve_Workset(int WorkSetID, string QcNotes, int QcWorkerID)
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("QC_Approve", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCommand.Parameters.AddWithValue("SetID", WorkSetID);
                sqlCommand.Parameters.AddWithValue("QC_Notes", QcNotes);
                sqlCommand.Parameters.AddWithValue("QC_WorkerID", QcWorkerID);

                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary> Resubmit a workset for QC  </summary>
        /// <param name="WorkSetID"> Primary key for this workset </param>
        /// <param name="ProcessorNotes"> Item processor notes for this workset </param>
        /// <returns> TRUE if successful, othewise FALSE </returns>
        public static bool Resubmit_Workset(int WorkSetID, string ProcessorNotes)
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("Resubmit_ItemWorkSet", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCommand.Parameters.AddWithValue("setid", WorkSetID);
                sqlCommand.Parameters.AddWithValue("processor_notes", ProcessorNotes);

                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary> Resubmit a workset for QC  </summary>
        /// <param name="WorkSetID"> Primary key for this workset </param>
        /// <param name="ProcessorNotes"> Item processor notes for this workset </param>
        /// <returns> TRUE if successful, othewise FALSE </returns>
        public static bool Save_Workset_Notes(int WorkSetID, string ProcessorNotes)
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("Save_ItemWork_Notes", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCommand.Parameters.AddWithValue("setid", WorkSetID);
                sqlCommand.Parameters.AddWithValue("processor_notes", ProcessorNotes);

                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }


        public static DataTable Perform_Search( string Field1, int Id1, string FreeSearch1, string Field2, int Id2, string FreeSearch2, string Field3, int Id3, string FreeSearch3,
            string Field4, int Id4, string FreeSearch4, string Field5, int Id5, string FreeSearch5, string Field6, int Id6, string FreeSearch6,
            string Field7, int Id7, string FreeSearch7, string Field8, int Id8, string FreeSearch8, string Field9, int Id9, string FreeSearch9,
            string Field10, int Id10, string FreeSearch10, string Field11, int Id11, string FreeSearch11, string Field12, int Id12, string FreeSearch12,
            string Field13, int Id13, string FreeSearch13, string Field14, int Id14, string FreeSearch14, string Field15, int Id15, string FreeSearch15, 
            DateTime? DateRange_Start, DateTime? DateRange_End, string Grouping )
        {
            try
            {
                // Pull the database information
                DataSet returnedSet = new DataSet();
                using (SqlConnection connection = new SqlConnection(DbConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("Perform_Search", connection)
                    {
                        SelectCommand = { CommandType = CommandType.StoredProcedure }
                    };
                    adapter.SelectCommand.Parameters.AddWithValue("field1", Field1);
                    adapter.SelectCommand.Parameters.AddWithValue("id1", Id1);
                    adapter.SelectCommand.Parameters.AddWithValue("free1", FreeSearch1);
                    adapter.SelectCommand.Parameters.AddWithValue("field2", Field2);
                    adapter.SelectCommand.Parameters.AddWithValue("id2", Id2);
                    adapter.SelectCommand.Parameters.AddWithValue("free2", FreeSearch2);
                    adapter.SelectCommand.Parameters.AddWithValue("field3", Field3);
                    adapter.SelectCommand.Parameters.AddWithValue("id3", Id3);
                    adapter.SelectCommand.Parameters.AddWithValue("free3", FreeSearch3);
                    adapter.SelectCommand.Parameters.AddWithValue("field4", Field4);
                    adapter.SelectCommand.Parameters.AddWithValue("id4", Id4);
                    adapter.SelectCommand.Parameters.AddWithValue("free4", FreeSearch4);
                    adapter.SelectCommand.Parameters.AddWithValue("field5", Field5);
                    adapter.SelectCommand.Parameters.AddWithValue("id5", Id5);
                    adapter.SelectCommand.Parameters.AddWithValue("free5", FreeSearch5);
                    adapter.SelectCommand.Parameters.AddWithValue("field6", Field6);
                    adapter.SelectCommand.Parameters.AddWithValue("id6", Id6);
                    adapter.SelectCommand.Parameters.AddWithValue("free6", FreeSearch6);
                    adapter.SelectCommand.Parameters.AddWithValue("field7", Field7);
                    adapter.SelectCommand.Parameters.AddWithValue("id7", Id7);
                    adapter.SelectCommand.Parameters.AddWithValue("free7", FreeSearch7);
                    adapter.SelectCommand.Parameters.AddWithValue("field8", Field8);
                    adapter.SelectCommand.Parameters.AddWithValue("id8", Id8);
                    adapter.SelectCommand.Parameters.AddWithValue("free8", FreeSearch8);
                    adapter.SelectCommand.Parameters.AddWithValue("field9", Field9);
                    adapter.SelectCommand.Parameters.AddWithValue("id9", Id9);
                    adapter.SelectCommand.Parameters.AddWithValue("free9", FreeSearch9);
                    adapter.SelectCommand.Parameters.AddWithValue("field10", Field10);
                    adapter.SelectCommand.Parameters.AddWithValue("id10", Id10);
                    adapter.SelectCommand.Parameters.AddWithValue("free10", FreeSearch10);
                    adapter.SelectCommand.Parameters.AddWithValue("field11", Field11);
                    adapter.SelectCommand.Parameters.AddWithValue("id11", Id11);
                    adapter.SelectCommand.Parameters.AddWithValue("free11", FreeSearch11);
                    adapter.SelectCommand.Parameters.AddWithValue("field12", Field12);
                    adapter.SelectCommand.Parameters.AddWithValue("id12", Id12);
                    adapter.SelectCommand.Parameters.AddWithValue("free12", FreeSearch12);
                    adapter.SelectCommand.Parameters.AddWithValue("field13", Field13);
                    adapter.SelectCommand.Parameters.AddWithValue("id13", Id13);
                    adapter.SelectCommand.Parameters.AddWithValue("free13", FreeSearch13);
                    adapter.SelectCommand.Parameters.AddWithValue("field14", Field14);
                    adapter.SelectCommand.Parameters.AddWithValue("id14", Id14);
                    adapter.SelectCommand.Parameters.AddWithValue("free14", FreeSearch14);
                    adapter.SelectCommand.Parameters.AddWithValue("field15", Field15);
                    adapter.SelectCommand.Parameters.AddWithValue("id15", Id15);
                    adapter.SelectCommand.Parameters.AddWithValue("free15", FreeSearch15);

                    if (DateRange_Start.HasValue)
                        adapter.SelectCommand.Parameters.Add("@daterange_start", SqlDbType.Date).Value = DateRange_Start.Value;
                    else
                        adapter.SelectCommand.Parameters.Add("@daterange_start", SqlDbType.Date).Value = DBNull.Value;

                    if (DateRange_End.HasValue)
                        adapter.SelectCommand.Parameters.Add("@daterange_end", SqlDbType.Date).Value = DateRange_End.Value;
                    else
                        adapter.SelectCommand.Parameters.Add("@daterange_end", SqlDbType.Date).Value = DBNull.Value;

                    adapter.SelectCommand.Parameters.AddWithValue("grouping", (Grouping ?? String.Empty));


                    adapter.Fill(returnedSet);
                }

                // If a table was returned, return it
                if (returnedSet.Tables.Count > 0)
                    return returnedSet.Tables[0];
            }
            catch (Exception ee)
            {
                LastError = ee.Message;
            }

            return null;
        }

    }
}
