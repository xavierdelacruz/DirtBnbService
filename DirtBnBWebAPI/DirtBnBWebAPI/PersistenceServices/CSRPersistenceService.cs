using System;
using System.Collections.Generic;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class CSRPersistenceService
    {
        private readonly string PARENT_TABLE = "csrs";
        private readonly string CHILD_TABLE = "csrs_fd";
        private MySqlConnection sqlConnection;

        public CSRPersistenceService()
        {
            sqlConnection = SqlServerConnectionManager.Instance.GetSqlConnection();
        }

        // GET CSRs Call
        public List<CSR> GetCSRS()
        {
            MySqlDataReader mySQLReader = null;
            List<CSR> csrs = new List<CSR>();

            string slqCommandString = "SELECT p.*, c.Name FROM " + PARENT_TABLE + " p , " + CHILD_TABLE + " c WHERE p.EmailAddress = c.EmailAddress";
            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    CSR csr = new CSR
                    {
                        userID = mySQLReader.GetInt32(0),
                        emailAddress = mySQLReader.GetString(1),
                        phoneNumber = mySQLReader.GetString(2),
                        password = mySQLReader.GetString(3),
                        name = mySQLReader.GetString(4),
                    };
                    csrs.Add(csr);
                }
                mySQLReader.Close();
                return csrs;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET CSRs call in CSRPersistenceService: " + ex);
                return null;
            }
        }

        // GET CSR Call
        public CSR GetCSR(long id)
        {

            MySqlDataReader mySQLReader = null;
            string slqCommandString = "SELECT p.*, c.Name FROM " + PARENT_TABLE + " p , " + CHILD_TABLE + " c WHERE p.EmailAddress = c.EmailAddress AND UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    CSR csr = new CSR
                    {
                        userID = mySQLReader.GetInt32(0),
                        emailAddress = mySQLReader.GetString(1),
                        phoneNumber = mySQLReader.GetString(2),
                        password = mySQLReader.GetString(3),
                        name = mySQLReader.GetString(4),
                    };

                    mySQLReader.Close();
                    return csr;
                }

                mySQLReader.Close();

                return null;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET CSR call in CSRPersistenceService: " + ex);
                return null;
            }
        }

        // POST CSR Call
        public long SaveCSR(CSR csr)
        {
            /// In POST, the child row has to exist - else, we cannot create a row in the parent table due to the FK constraint.
            string childSqlCommandString = "INSERT INTO " + CHILD_TABLE + " (EmailAddress, Name) VALUES ('"
                + csr.emailAddress + "','"
                + csr.name + "')";

            string sqlCommandString = "INSERT INTO " + PARENT_TABLE + " (UserID, EmailAddress, PhoneNumber, Password) VALUES ("
                + csr.userID + ",'"
                + csr.emailAddress + "','"
                + csr.phoneNumber + "','"
                + csr.password + "')";

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
                Console.WriteLine("Found an error when performing a POST CSR call in CSRPersistenceService: " + ex);
                return -1;
            }
        }

        // DELETE CSR
        public bool DeleteCSR(long id)
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
                    var email = mySQLReader.GetString(1);

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
                Console.WriteLine("Found an error when performing a DELETE CSR call in CSRPersistenceService: " + ex);
                return false;
            }
        }

        // PATCH or PUT CSR
        public bool UpdateCSR(long id, CSR csr)
        {
            MySqlDataReader mySQLReader = null;
            string slqCommandString = "SELECT p.*, c.Name FROM " + PARENT_TABLE + " p , " + CHILD_TABLE + " c WHERE p.EmailAddress = c.EmailAddress AND p.UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();


                if (mySQLReader.Read())
                {
                    // If we are to update the email, we will need to reference the old email, as it is the PK in the child table.
                    // AGAIN, updating the PK of a table is a BAD idea, but in our design, we want the user to be able to update Email.
                    var oldEmail = mySQLReader.GetString(1);

                    if (string.IsNullOrEmpty(csr.emailAddress))
                    {
                        csr.emailAddress = mySQLReader.GetString(1);
                    }
                    if (string.IsNullOrEmpty(csr.phoneNumber))
                    {
                        csr.phoneNumber = mySQLReader.GetString(2);
                    }
                    if (string.IsNullOrEmpty(csr.password))
                    {
                        csr.password = mySQLReader.GetString(3);
                    }
                    if (string.IsNullOrEmpty(csr.name))
                    {
                        csr.name = mySQLReader.GetString(4);
                    }

                    mySQLReader.Close();

                    // In PUT, if an FK is updated in the child table, it will also get updated on the parent table.
                    // HOWEVER, non-FK attributes will NOT get updated, so you will still need to update it in the parent table in your SQL query.
                    // In this case, Name is not an FK, and will not get updated in the parent table - but if we want integrity, we will need
                    // to turn this into an FK, OR update it manually. Note, we are using the latter.
                    string childSqlUpdateCommandString = "UPDATE " + CHILD_TABLE
                        + " SET EmailAddress='" + csr.emailAddress
                        + "', Name='" + csr.name
                        + "' WHERE EmailAddress = '" + oldEmail
                        + "'";

                    string sqlUpdateCommandString = "UPDATE " + PARENT_TABLE
                        + " SET PhoneNumber='" + csr.phoneNumber
                        + "', Password='" + csr.password
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
                Console.WriteLine("Found an error when performing a PUT CSR call in CSRPersistenceService: " + ex);
                return false;
            }
        }
    }
}