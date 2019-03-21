using System;
using System.Collections.Generic;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class AccommodationPersistenceService
    {
        private readonly string PARENT_TABLE = "accommodations";
        private readonly string CHILD_TABLE = "accommodations_fd";

        private MySqlConnection sqlConnection;

        public AccommodationPersistenceService()
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
                        accommodationID = mySQLReader.GetInt32(0),
                        parking = mySQLReader.GetByte(1),
                        wifi = mySQLReader.GetByte(2),
                        tv = mySQLReader.GetByte(3),
                        airConditioning = mySQLReader.GetByte(4),
                        generalAppliances = mySQLReader.GetByte(5),
                        bedSize = mySQLReader.GetString(6),
                        pricePerNight = mySQLReader.GetInt32(7),
                        houseNumber = mySQLReader.GetString(8),
                        hostUserID = mySQLReader.GetInt32(9),
                        postalCode = mySQLReader.GetString(10),
                        city = mySQLReader.GetString(11),
                        street = mySQLReader.GetString(12),
                        province = mySQLReader.GetString(13)
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
                        accommodationID = mySQLReader.GetInt32(0),
                        parking = mySQLReader.GetByte(1),
                        wifi = mySQLReader.GetByte(2),
                        tv = mySQLReader.GetByte(3),
                        airConditioning = mySQLReader.GetByte(4),
                        generalAppliances = mySQLReader.GetByte(5),
                        bedSize = mySQLReader.GetString(6),
                        pricePerNight = mySQLReader.GetInt32(7),
                        houseNumber = mySQLReader.GetString(8),
                        hostUserID = mySQLReader.GetInt32(9),
                        postalCode = mySQLReader.GetString(10),
                        city = mySQLReader.GetString(11),
                        street = mySQLReader.GetString(12),
                        province = mySQLReader.GetString(13)
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
        public long SaveAccommodation(Accommodation accomodation)
        {
            // In POST, the child row has to exist - else, we cannot create a row in the parent table due to the FK constraint.
            string childSqlCommandStringPostalCode = "INSERT IGNORE INTO " + CHILD_TABLE + " (PostalCode, City, Street, Province) VALUES ('"
                + accomodation.postalCode + "','"
                + accomodation.city + "','"
                + accomodation.street + "','"
                + accomodation.province + "')"; ;

            string sqlCommandString = "INSERT INTO " + PARENT_TABLE + " (AccommodationID, Parking, Wifi, TV, AirConditioning, " +
                "GeneralAppliances, BedSize, PricePerNight, HouseNumber, HostUserID, PostalCode, City, Street, Province) VALUES ("
                + accomodation.accommodationID + ","
                + accomodation.parking + ","
                + accomodation.wifi + ","
                + accomodation.tv + ","
                + accomodation.airConditioning + ","
                + accomodation.generalAppliances + ",'"
                + accomodation.bedSize + "',"
                + accomodation.pricePerNight + ",'"
                + accomodation.houseNumber + "',"
                + accomodation.hostUserID + ",'"
                + accomodation.postalCode + "','"
                + accomodation.city + "','"
                + accomodation.street + "','"
                + accomodation.province + "')";

            MySqlCommand childSqlCommandPostalCode = new MySqlCommand(childSqlCommandStringPostalCode, sqlConnection);
            MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);
            try
            {
                childSqlCommandPostalCode.ExecuteNonQuery();
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
                    // var postalCode = mySQLReader.GetString(10);

                    mySQLReader.Close();

                    // Use these lines IF and ONLY IF YOU DO HAVE an FK constraint and DO NOT NEED TO DELETE tuples from the child table(s). 
                    // Else, delete the entry from the child table, which will cascade and delete parent entry
                    // NOTE: ON CASCADE DELETE MUST BE ENABLED.
                    string slqDeleteCommandString = "DELETE FROM " + PARENT_TABLE + " WHERE AccommodationID = " + id.ToString();
                    MySqlCommand sqlDeleteCommand = new MySqlCommand(slqDeleteCommandString, sqlConnection);

                    //string childslqDeleteCommandStringCity = "DELETE FROM " + CHILD_TABLE_CITY + " WHERE PostalCode = '" + postalCode + "'";
                    //string childslqDeleteCommandStringStreet = "DELETE FROM " + CHILD_TABLE_STREET + " WHERE PostalCode = '" + postalCode + "'";
                    //string childslqDeleteCommandStringProvince = "DELETE FROM " + CHILD_TABLE_PROVINCE + " WHERE PostalCode = '" + postalCode + "'";
                    //MySqlCommand sqlDeleteCommandCity = new MySqlCommand(childslqDeleteCommandStringCity, sqlConnection);
                    //MySqlCommand sqlDeleteCommandStreet = new MySqlCommand(childslqDeleteCommandStringStreet, sqlConnection);
                    //MySqlCommand sqlDeleteCommandProvince = new MySqlCommand(childslqDeleteCommandStringProvince, sqlConnection);

                    sqlDeleteCommand.ExecuteNonQuery();
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
                    if (accommodation.parking == null)
                    {
                        accommodation.parking = mySQLReader.GetByte(1);
                    }
                    if (accommodation.wifi == null)
                    {
                        accommodation.wifi = mySQLReader.GetByte(2);
                    }
                    if (accommodation.tv == null)
                    {
                        accommodation.tv = mySQLReader.GetByte(3);
                    }
                    if (accommodation.airConditioning == null)
                    {
                        accommodation.airConditioning = mySQLReader.GetByte(4);
                    }
                    if (accommodation.generalAppliances == null)
                    {
                        accommodation.generalAppliances = mySQLReader.GetByte(5);
                    }
                    if (string.IsNullOrEmpty(accommodation.bedSize))
                    {
                        accommodation.bedSize = mySQLReader.GetString(6);
                    }
                    if (accommodation.pricePerNight == null)
                    {
                        accommodation.pricePerNight = mySQLReader.GetInt32(7);
                    }
                    if (string.IsNullOrEmpty(accommodation.houseNumber))
                    {
                        accommodation.houseNumber = mySQLReader.GetString(8);
                    }
                    if (accommodation.hostUserID == null)
                    {
                        accommodation.hostUserID = mySQLReader.GetInt32(9);
                    }

                    var oldPostcalCode = mySQLReader.GetString(10);

                    if (string.IsNullOrEmpty(accommodation.postalCode))
                    {
                        accommodation.postalCode = mySQLReader.GetString(10);
                    }
                    if (string.IsNullOrEmpty(accommodation.city))
                    {
                        accommodation.city = mySQLReader.GetString(11);
                    }
                    if (string.IsNullOrEmpty(accommodation.street))
                    {
                        accommodation.street = mySQLReader.GetString(12);
                    }
                    if (string.IsNullOrEmpty(accommodation.province))
                    {
                        accommodation.province = mySQLReader.GetString(13);
                    }

                    // If we are to update the email, we will need to reference the old email, as it is the PK in the child table.
                    // AGAIN, updating the PK of a table is a BAD idea, but in our design, we want the user to be able to update Email.
                    

                    mySQLReader.Close();

                    // In PUT, if an FK is updated in the child table, it will also get updated on the parent table.
                    // HOWEVER, non-FK attributes will NOT get updated, so you will still need to update it in the parent table in your SQL query.
                    // In this case, Name is not an FK, and will not get updated in the parent table - but if we want integrity, we will need
                    // to turn this into an FK, OR update it manually. Note, we are using the latter.

                    string childSqlUpdateCommandStringPostalCode = "UPDATE " + CHILD_TABLE
                        + " SET PostalCode='" + accommodation.postalCode
                        + "', City='" + accommodation.city
                        + "', Street='" + accommodation.street
                        + "', Province='" + accommodation.province
                        + "' WHERE PostalCode = '" + oldPostcalCode
                        + "'";

                    string sqlUpdateCommandString = "UPDATE " + PARENT_TABLE
                        + " SET Parking='" + accommodation.parking
                        + "', Wifi='" + accommodation.wifi
                        + "', TV='" + accommodation.tv
                        + "', AirConditioning='" + accommodation.airConditioning
                        + "', GeneralAppliances='" + accommodation.generalAppliances
                        + "', BedSize='" + accommodation.bedSize
                        + "', PricePerNight='" + accommodation.pricePerNight
                        + "', HouseNumber='" + accommodation.houseNumber
                        + "', City='" + accommodation.city
                        + "', Street='" + accommodation.street
                        + "', Province='" + accommodation.province
                        + "' WHERE AccommodationID=" + id.ToString();

                    MySqlCommand childMySqlCommandPostalCode = new MySqlCommand(childSqlUpdateCommandStringPostalCode, sqlConnection);
                    MySqlCommand sqlUpdateCommand = new MySqlCommand(sqlUpdateCommandString, sqlConnection);

                    childMySqlCommandPostalCode.ExecuteNonQuery();
                    sqlUpdateCommand.ExecuteNonQuery();
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