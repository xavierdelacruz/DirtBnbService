using System;
using System.Collections.Generic;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class HostPersistenceService
    {
        private readonly string PARENT_TABLE = "hosts";
        private readonly string CHILD_TABLE = "hosts_fd";
        private MySqlConnection sqlConnection;

        public HostPersistenceService()
        {
            sqlConnection = SqlServerConnectionManager.Instance.GetSqlConnection();
        }

        // GET Hosts Call
        public List<Host> GetHosts()
        {
            MySqlDataReader mySQLReader = null;
            List<Host> hosts = new List<Host>();

            string slqCommandString = "SELECT p.*, c.Name FROM " + PARENT_TABLE + " p , " + CHILD_TABLE + " c WHERE p.EmailAddress = c.EmailAddress";
            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    Host host = new Host
                    {
                        userID = mySQLReader.GetInt32(0),
                        emailAddress = mySQLReader.GetString(1),
                        phoneNumber = mySQLReader.GetString(2),
                        password = mySQLReader.GetString(3),
                        name = mySQLReader.GetString(4),
                    };
                    hosts.Add(host);
                }
                mySQLReader.Close();
                return hosts;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Hosts call in HostPersistenceService: " + ex);
                return null;
            }
        }

        // GET Host Call
        public Host GetHost(long id)
        {

            MySqlDataReader mySQLReader = null;
            string slqCommandString = "SELECT p.*, c.Name FROM " + PARENT_TABLE + " p , " + CHILD_TABLE + " c WHERE p.EmailAddress = c.EmailAddress AND UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    Host host = new Host
                    {
                        userID = mySQLReader.GetInt32(0),
                        emailAddress = mySQLReader.GetString(1),
                        phoneNumber = mySQLReader.GetString(2),
                        password = mySQLReader.GetString(3),
                        name = mySQLReader.GetString(4),
                    };
                    mySQLReader.Close();
                    return host;
                }
                mySQLReader.Close();
                return null;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Host call in HostPersistenceService: " + ex);
                return null;
            }
        }

        // POST Host Call
        public long SaveHost(Host host)
        {
            /// In POST, the child row has to exist - else, we cannot create a row in the parent table due to the FK constraint.
            string childSqlCommandString = "INSERT INTO " + CHILD_TABLE + " (EmailAddress, Name) VALUES ('"
                + host.emailAddress + "','"
                + host.name + "')";
            Debug.WriteLine(childSqlCommandString);

            string sqlCommandString = "INSERT INTO " + PARENT_TABLE + " (UserID, EmailAddress, PhoneNumber, Password) VALUES ("
                + host.userID + ",'"
                + host.emailAddress + "','"
                + host.phoneNumber + "','"
                + host.password + "')";
            Debug.WriteLine(sqlCommandString);


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
                Console.WriteLine("Found an error when performing a POST Host call in HostPersistenceService: " + ex);
                return -1;
            }
        }

        // DELETE Host
        public bool DeleteHost(long id)
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
                Console.WriteLine("Found an error when performing a DELETE Host call in HostPersistenceService: " + ex);
                return false;
            }
        }

        // PATCH or PUT Host
        public bool UpdateHost(long id, Host host)
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

                    if (string.IsNullOrEmpty(host.emailAddress))
                    {
                        host.emailAddress = mySQLReader.GetString(1);
                    }
                    if (string.IsNullOrEmpty(host.phoneNumber))
                    {
                        host.phoneNumber = mySQLReader.GetString(2);
                    }
                    if (string.IsNullOrEmpty(host.password))
                    {
                        host.password = mySQLReader.GetString(3);
                    }
                    if (string.IsNullOrEmpty(host.name))
                    {
                        host.name = mySQLReader.GetString(4);
                    }

                    mySQLReader.Close();

                    // In PUT, if an FK is updated in the child table, it will also get updated on the parent table.
                    // HOWEVER, non-FK attributes will NOT get updated, so you will still need to update it in the parent table in your SQL query.
                    // In this case, Name is not an FK, and will not get updated in the parent table - but if we want integrity, we will need
                    // to turn this into an FK, OR update it manually. Note, we are using the latter.
                    string childSqlUpdateCommandString = "UPDATE " + CHILD_TABLE
                        + " SET EmailAddress='" + host.emailAddress
                        + "', Name='" + host.name
                        + "' WHERE EmailAddress = '" + oldEmail
                        + "'";

                    string sqlUpdateCommandString = "UPDATE " + PARENT_TABLE
                        + " SET PhoneNumber='" + host.phoneNumber
                        + "', Password='" + host.password
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
                Console.WriteLine("Found an error when performing a PUT Host call in HostPersistenceService: " + ex);
                return false;
            }
        }
    }
}