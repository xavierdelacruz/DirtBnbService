using System;
using System.Collections.Generic;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class GuestPersistenceService
    {
        private MySqlConnection sqlConnection;

        public GuestPersistenceService()
        {
            sqlConnection = SqlServerConnectionManager.Instance.GetSqlConnection();
        }

        // GET Hosts Call
        public List<Host> GetHosts()
        {
            MySqlDataReader mySQLReader = null;
            List<Host> hosts = new List<Host>();

            string slqCommandString = "SELECT * FROM hosts";
            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    Host host = new Host();
                    host.userID = mySQLReader.GetInt32(0);
                    host.name = mySQLReader.GetString(1);
                    host.emailAddress = mySQLReader.GetString(2);
                    host.phoneNumber = mySQLReader.GetString(3);
                    host.password = mySQLReader.GetString(4);
                    hosts.Add(host);

                }
                mySQLReader.Close();
                return hosts;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Hosts call in HostPersistenceService(GetHosts): " + ex);
                return null;
            }
        }

        // GET Host Call
        public Host GetHost(long id)
        {
            Host host = new Host();
            MySqlDataReader mySQLReader = null;

            string slqCommandString = "SELECT * FROM hosts WHERE UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    host.userID = mySQLReader.GetInt32(0);
                    host.name = mySQLReader.GetString(1);
                    host.emailAddress = mySQLReader.GetString(2);
                    host.phoneNumber = mySQLReader.GetString(3);
                    host.password = mySQLReader.GetString(4);

                    mySQLReader.Close();
                    return host;
                }

                mySQLReader.Close();

                return null;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Host call in HostPersistenceService (GetHost): " + ex);
                return null;
            }
        }

        // POST Host Call
        public long SaveHost(Host host)
        {
            string childSqlCommandString = "INSERT INTO hosts_fd (EmailAddress, Name) VALUES ('" + host.emailAddress + "','" +
                host.name + "')";
            string sqlCommandString = "INSERT INTO hosts (UserID, Name, EmailAddress, PhoneNumber, Password) VALUES (" + host.userID + ",'" +
                host.name + "','" + host.emailAddress + "','" + host.phoneNumber + "','" + host.password + "')";

            MySqlCommand childSqlCommand = new MySqlCommand(childSqlCommandString, sqlConnection);
            MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);
            try
            {
                childSqlCommand.ExecuteNonQuery();
                sqlCommand.ExecuteNonQueryAsync();
                long id = sqlCommand.LastInsertedId;
                return id;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a POST Host call in HostPersistenceService (SaveHost): " + ex);
                return -1;
            }
        }

        // DELETE Host
        public bool DeleteHost(long id)
        {
            MySqlDataReader mySQLReader = null;
            string slqCommandString = "SELECT * FROM hosts WHERE UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    var email = mySQLReader.GetString(2);

                    mySQLReader.Close();

                    // Use this line IF and ONLY IF YOU DO NOT HAVE an FK constraint. 
                    // Else, delete the entry from the child table, which will cascade and delete parent entry
                    // NOTE: ON CASCADE DELETE MUST BE ENABLED.
                    // string slqDeleteCommandString = "DELETE FROM hosts_fd WHERE UserID = " + id.ToString();
                    // MySqlCommand sqlDeleteCommand = new MySqlCommand(slqDeleteCommandString, sqlConnection);

                    string childslqDeleteCommandString = "DELETE FROM hosts_fd WHERE EmailAddress = '" + email + "'";
                    MySqlCommand sqlDeleteCommand = new MySqlCommand(childslqDeleteCommandString, sqlConnection);

                    sqlDeleteCommand.ExecuteNonQuery();
                    return true;
                }

                mySQLReader.Close();
                return false;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a DELETE Host call in HostPersistenceService (DeleteHost): " + ex);
                return false;
            }
        }

        // PATCH Host
        public bool UpdateHost(long id, Host host)
        {
            MySqlDataReader mySQLReader = null;

            string slqCommandString = "SELECT * FROM hosts WHERE UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();


                if (mySQLReader.Read())
                {
                    if (string.IsNullOrEmpty(host.name))
                    {
                        host.name = mySQLReader.GetString(1);
                    }
                    var oldEmail = mySQLReader.GetString(2);
                    if (string.IsNullOrEmpty(host.emailAddress))
                    {
                        host.emailAddress = mySQLReader.GetString(2);
                    }
                    if (string.IsNullOrEmpty(host.phoneNumber))
                    {
                        host.phoneNumber = mySQLReader.GetString(3);
                    }
                    if (string.IsNullOrEmpty(host.password))
                    {
                        host.password = mySQLReader.GetString(4);
                    }

                    mySQLReader.Close();

                    string childSqlUpdateCommandString = "UPDATE hosts_fd SET EmailAddress='" + host.emailAddress + "', Name='" + host.name + "' WHERE EmailAddress = '" + oldEmail + "'";
                    string sqlUpdateCommandString = "UPDATE hosts SET Name='" + host.name + "', " +
                        "PhoneNumber='" + host.phoneNumber + "', Password='" + host.password + "' WHERE UserID=" + id.ToString();

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
                Console.WriteLine("Found an error when performing a PUT Host call in HostPersistenceService (UpdateHost): " + ex);
                return false;
            }
        }
    }
}