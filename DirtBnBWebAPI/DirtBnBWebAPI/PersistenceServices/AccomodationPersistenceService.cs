using System;
using System.Collections.Generic;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class AccomodationPersistenceService
    {
        private readonly string PARENT_TABLE = "accommodations";
        private readonly string CHILD_TABLE_CITY = "accommodations_fd_city";
        private readonly string CHILD_TABLE_STREET = "accommodations_fd_street";
        private readonly string CHILD_TABLE_PROVINCE = "accommodations_fd_province";

        private MySqlConnection sqlConnection;

        public AccomodationPersistenceService()
        {
            sqlConnection = SqlServerConnectionManager.Instance.GetSqlConnection();
        }

        // GET Accommodations Call
        public List<Accommodation> GetAccommodations()
        {
            MySqlDataReader mySQLReader = null;
            List<Accommodation> accommodations = new List<Accommodation>();

            string slqCommandString = "SELECT * FROM " + PARENT_TABLE;
            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    Accommodation accommodation = new Accommodation
                    {
                        //userID = mySQLReader.GetInt32(0),
                        //name = mySQLReader.GetString(1),
                        //emailAddress = mySQLReader.GetString(2),
                        //phoneNumber = mySQLReader.GetString(3),
                        //password = mySQLReader.GetString(4)
                    };
                    accommodations.Add(accommodation);
                }
                mySQLReader.Close();
                return accommodations;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Accommodation call in AccomodationPersistenceService: " + ex);
                return null;
            }
        }

        // GET Accommodation Call
        public Accommodation GetAccommodation(long id)
        {

            MySqlDataReader mySQLReader = null;

            string slqCommandString = "SELECT * FROM " + PARENT_TABLE + " WHERE AccommodationID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    Accommodation accommodation = new Accommodation
                    {
                        //userID = mySQLReader.GetInt32(0),
                        //name = mySQLReader.GetString(1),
                        //emailAddress = mySQLReader.GetString(2),
                        //phoneNumber = mySQLReader.GetString(3),
                        //password = mySQLReader.GetString(4)
                    };
                    mySQLReader.Close();
                    return accommodation;
                }
                mySQLReader.Close();
                return null;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Accomodation call in AccomodationPersistenceService: " + ex);
                return null;
            }
        }

        // POST Accommodation Call
        public long SaveHost(Accommodation accomodation)
        {
            /// In POST, the child row has to exist - else, we cannot create a row in the parent table due to the FK constraint.
            string childSqlCommandStringCity = "INSERT INTO " + CHILD_TABLE_CITY + " (EmailAddress, Name) VALUES ('"
                + accomodation.postalCode + "','"
                + accomodation.city + "')";

            string childSqlCommandStringStreet = "INSERT INTO " + CHILD_TABLE_CITY + " (EmailAddress, Name) VALUES ('"
                + accomodation.postalCode + "','"
                + accomodation.street + "')";

            string childSqlCommandStringProvince = "INSERT INTO " + CHILD_TABLE_CITY + " (EmailAddress, Name) VALUES ('"
                + accomodation.postalCode + "','"
                + accomodation.province + "')";

            string sqlCommandString = "INSERT INTO " + PARENT_TABLE + " (AccommodationID, Parking, Wifi, TV, AirConditioning, " +
                "GeneralAppliances, BedSize, PricePerNight, HouseNumber, HostUserID, " +
                "PostalCode, City, Street, Province) VALUES ("
                + accomodation.accommodationID + ",'"
                + accomodation.parking + "','"
                + accomodation.wifi + "','"
                + accomodation.tv + "','"
                + accomodation.airConditioning + "','"
                + accomodation.generalAppliances + "','"
                + accomodation.bedSize + "','"
                + accomodation.pricePerNight + "','"
                + accomodation.houseNumber + "','"
                + accomodation.hostUserID + "','"
                + accomodation.postalCode + "','"
                + accomodation.city + "','"
                + accomodation.street + "','"
                + accomodation.province + "')";

            MySqlCommand childSqlCommandCity = new MySqlCommand(childSqlCommandStringCity, sqlConnection);
            MySqlCommand childSqlCommandStreet = new MySqlCommand(childSqlCommandStringStreet, sqlConnection);
            MySqlCommand childSqlCommandProvince = new MySqlCommand(childSqlCommandStringProvince, sqlConnection);
            MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);
            try
            {
                childSqlCommandCity.ExecuteNonQuery();
                childSqlCommandStreet.ExecuteNonQuery();
                childSqlCommandProvince.ExecuteNonQuery();
                sqlCommand.ExecuteNonQuery();
                long id = sqlCommand.LastInsertedId;
                return id;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a POST Accomodation call in AccomodationPersistenceService: " + ex);
                return -1;
            }
        }

        // DELETE Accommodation
        public bool DeleteAccommodation(long id)
        {
            MySqlDataReader mySQLReader = null;
            string slqCommandString = "SELECT * FROM " + PARENT_TABLE + " WHERE AccommodationID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    // This is the PK of the child table.
                    var postalCode = mySQLReader.GetString(10);

                    mySQLReader.Close();

                    // Use these lines IF and ONLY IF YOU DO NOT HAVE an FK constraint. 
                    // Else, delete the entry from the child table, which will cascade and delete parent entry
                    // NOTE: ON CASCADE DELETE MUST BE ENABLED.
                    // string slqDeleteCommandString = "DELETE FROM " + PARENT_TABLE + " WHERE UserID = " + id.ToString();
                    // MySqlCommand sqlDeleteCommand = new MySqlCommand(slqDeleteCommandString, sqlConnection);

                    string childslqDeleteCommandStringCity = "DELETE FROM " + CHILD_TABLE_CITY + " WHERE PostalCode = '" + postalCode + "'";
                    string childslqDeleteCommandStringStreet = "DELETE FROM " + CHILD_TABLE_STREET + " WHERE PostalCode = '" + postalCode + "'";
                    string childslqDeleteCommandStringProvince = "DELETE FROM " + CHILD_TABLE_PROVINCE + " WHERE PostalCode = '" + postalCode + "'";
                    MySqlCommand sqlDeleteCommandCity = new MySqlCommand(childslqDeleteCommandStringCity, sqlConnection);
                    MySqlCommand sqlDeleteCommandStreet = new MySqlCommand(childslqDeleteCommandStringStreet, sqlConnection);
                    MySqlCommand sqlDeleteCommandProvince = new MySqlCommand(childslqDeleteCommandStringProvince, sqlConnection);

                    sqlDeleteCommandCity.ExecuteNonQuery();
                    sqlDeleteCommandStreet.ExecuteNonQuery();
                    sqlDeleteCommandProvince.ExecuteNonQuery();
                    return true;
                }

                mySQLReader.Close();
                return false;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a DELETE Accommodation call in AccomodationPersistenceService: " + ex);
                return false;
            }
        }

        // PATCH or PUT Accommodation
        public bool UpdateAccommodation(long id, Accommodation accommodation)
        {
            MySqlDataReader mySQLReader = null;

            string slqCommandString = "SELECT * FROM " + PARENT_TABLE + " WHERE AccommodationID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();


                if (mySQLReader.Read())
                {
                    //if (string.IsNullOrEmpty(accommodation.name))
                    //{
                    //    accommodation.name = mySQLReader.GetString(1);
                    //}

                    // If we are to update the email, we will need to reference the old email, as it is the PK in the child table.
                    // AGAIN, updating the PK of a table is a BAD idea, but in our design, we want the user to be able to update Email.
                    var oldEmail = mySQLReader.GetString(2);

                    //if (string.IsNullOrEmpty(host.emailAddress))
                    //{
                    //    host.emailAddress = mySQLReader.GetString(2);
                    //}
                    //if (string.IsNullOrEmpty(host.phoneNumber))
                    //{
                    //    host.phoneNumber = mySQLReader.GetString(3);
                    //}
                    //if (string.IsNullOrEmpty(host.password))
                    //{
                    //    host.password = mySQLReader.GetString(4);
                    //}

                    mySQLReader.Close();

                    // In PUT, if an FK is updated in the child table, it will also get updated on the parent table.
                    // HOWEVER, non-FK attributes will NOT get updated, so you will still need to update it in the parent table in your SQL query.
                    // In this case, Name is not an FK, and will not get updated in the parent table - but if we want integrity, we will need
                    // to turn this into an FK, OR update it manually. Note, we are using the latter.

                    //string childSqlUpdateCommandString = "UPDATE " + CHILD_TABLE
                    //    + " SET EmailAddress='" + accommodation.emailAddress
                    //    + "', Name='" + accommodation.name
                    //    + "' WHERE EmailAddress = '" + oldEmail
                    //    + "'";

                    //string sqlUpdateCommandString = "UPDATE " + PARENT_TABLE
                    //    + " SET Name='" + host.name + "', "
                    //    + "PhoneNumber='" + host.phoneNumber
                    //    + "', Password='" + host.password
                    //    + "' WHERE UserID=" + id.ToString();

                    //MySqlCommand childMySqlCommand = new MySqlCommand(childSqlUpdateCommandString, sqlConnection);
                    //MySqlCommand sqlUpdateCommand = new MySqlCommand(sqlUpdateCommandString, sqlConnection);

                    //childMySqlCommand.ExecuteNonQuery();
                    //sqlUpdateCommand.ExecuteNonQuery();
                    return true;
                }

                mySQLReader.Close();
                return false;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a PUT Accommodation call in AccomodationPersistenceService: " + ex);
                return false;
            }
        }
    }
}