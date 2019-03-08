using System;
using System.Collections.Generic;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class GuestPersistenceService
    {
        private readonly string PARENT_TABLE = "guests";
        private readonly string CHILD_TABLE = "guests_fd";
        private MySqlConnection sqlConnection;

        public GuestPersistenceService()
        {
            sqlConnection = SqlServerConnectionManager.Instance.GetSqlConnection();
        }

        // GET Guests Call
        public List<Guest> GetGuests()
        {
            MySqlDataReader mySQLReader = null;
            List<Guest> guests = new List<Guest>();

            string slqCommandString = "SELECT * FROM " + PARENT_TABLE;
            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    Guest guest = new Guest
                    {
                        userID = mySQLReader.GetInt32(0),
                        name = mySQLReader.GetString(1),
                        emailAddress = mySQLReader.GetString(2),
                        phoneNumber = mySQLReader.GetString(3),
                        password = mySQLReader.GetString(4)
                    };
                    guests.Add(guest);
                }
                mySQLReader.Close();
                return guests;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Guests call in GuestPersistenceService: " + ex);
                return null;
            }
        }

        // GET Guest Call
        public Guest GetGuest(long id)
        {
            MySqlDataReader mySQLReader = null;
            string slqCommandString = "SELECT * FROM " + PARENT_TABLE + " WHERE UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    Guest guest = new Guest
                    {
                        userID = mySQLReader.GetInt32(0),
                        name = mySQLReader.GetString(1),
                        emailAddress = mySQLReader.GetString(2),
                        phoneNumber = mySQLReader.GetString(3),
                        password = mySQLReader.GetString(4)
                    };

                    mySQLReader.Close();
                    return guest;
                }

                mySQLReader.Close();

                return null;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Guest call in GuestPersistenceService: " + ex);
                return null;
            }
        }

        // POST Guest Call
        public long SaveGuest(Guest guest)
        {
            /// In POST, the child row has to exist - else, we cannot create a row in the parent table due to the FK constraint.
            string childSqlCommandString = "INSERT INTO " + CHILD_TABLE + " (EmailAddress, Name) VALUES ('"
                + guest.emailAddress + "','"
                + guest.name + "')";

            string sqlCommandString = "INSERT INTO " + PARENT_TABLE + " (UserID, Name, EmailAddress, PhoneNumber, Password) VALUES ("
                + guest.userID + ",'"
                + guest.name + "','"
                + guest.emailAddress + "','"
                + guest.phoneNumber + "','"
                + guest.password + "')";

            MySqlCommand childSqlCommand = new MySqlCommand(childSqlCommandString, sqlConnection);
            MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);
            try
            {
                childSqlCommand.ExecuteNonQuery();
                sqlCommand.ExecuteNonQuery();
                long id = sqlCommand.LastInsertedId;
                return id;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a POST Guest call in GuestPersistenceService: " + ex);
                return -1;
            }
        }

        // DELETE Guest
        public bool DeleteGuest(long id)
        {
            MySqlDataReader mySQLReader = null;
            string slqCommandString = "SELECT * FROM " + PARENT_TABLE + " WHERE UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    // This is the PK of the child table.
                    var email = mySQLReader.GetString(2);

                    mySQLReader.Close();

                    // Use these lines IF and ONLY IF YOU DO NOT HAVE an FK constraint. 
                    // Else, delete the entry from the child table, which will cascade and delete parent entry
                    // NOTE: ON CASCADE DELETE MUST BE ENABLED.
                    // string slqDeleteCommandString = "DELETE FROM " + PARENT_TABLE + " WHERE UserID = " + id.ToString();
                    // MySqlCommand sqlDeleteCommand = new MySqlCommand(slqDeleteCommandString, sqlConnection);

                    string childslqDeleteCommandString = "DELETE FROM " + CHILD_TABLE + " WHERE EmailAddress = '" + email + "'";
                    MySqlCommand sqlDeleteCommand = new MySqlCommand(childslqDeleteCommandString, sqlConnection);

                    sqlDeleteCommand.ExecuteNonQuery();
                    return true;
                }

                mySQLReader.Close();
                return false;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a DELETE Guest call in GuestPersistenceService: " + ex);
                return false;
            }
        }

        // PATCH or PUT Guest
        public bool UpdateGuest(long id, Guest guest)
        {
            MySqlDataReader mySQLReader = null;
            string slqCommandString = "SELECT * FROM " + PARENT_TABLE + " WHERE UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();


                if (mySQLReader.Read())
                {
                    if (string.IsNullOrEmpty(guest.name))
                    {
                        guest.name = mySQLReader.GetString(1);
                    }

                    // If we are to update the email, we will need to reference the old email, as it is the PK in the child table.
                    // AGAIN, updating the PK of a table is a BAD idea, but in our design, we want the user to be able to update Email.
                    var oldEmail = mySQLReader.GetString(2);

                    if (string.IsNullOrEmpty(guest.emailAddress))
                    {
                        guest.emailAddress = mySQLReader.GetString(2);
                    }
                    if (string.IsNullOrEmpty(guest.phoneNumber))
                    {
                        guest.phoneNumber = mySQLReader.GetString(3);
                    }
                    if (string.IsNullOrEmpty(guest.password))
                    {
                        guest.password = mySQLReader.GetString(4);
                    }

                    mySQLReader.Close();

                    // In PUT, if an FK is updated in the child table, it will also get updated on the parent table.
                    // HOWEVER, non-FK attributes will NOT get updated, so you will still need to update it in the parent table in your SQL query.
                    // In this case, Name is not an FK, and will not get updated in the parent table - but if we want integrity, we will need
                    // to turn this into an FK, OR update it manually. Note, we are using the latter.
                    string childSqlUpdateCommandString = "UPDATE " + CHILD_TABLE
                        + " SET EmailAddress='" + guest.emailAddress
                        + "', Name='" + guest.name
                        + "' WHERE EmailAddress = '" + oldEmail
                        + "'";

                    string sqlUpdateCommandString = "UPDATE " + PARENT_TABLE
                        + " SET Name='" + guest.name + "', "
                        + "PhoneNumber='" + guest.phoneNumber
                        + "', Password='" + guest.password
                        + "' WHERE UserID=" + id.ToString();

                    MySqlCommand childMySqlCommand = new MySqlCommand(childSqlUpdateCommandString, sqlConnection);
                    MySqlCommand sqlUpdateCommand = new MySqlCommand(sqlUpdateCommandString, sqlConnection);

                    childMySqlCommand.ExecuteNonQuery();
                    sqlUpdateCommand.ExecuteNonQuery();
                    return true;
                }

                mySQLReader.Close();
                return false;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a PUT Guest call in GuestPersistenceService: " + ex);
                return false;
            }
        }
    }
}