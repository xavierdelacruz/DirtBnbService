using System;
using System.Collections.Generic;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.PersistenceServices
{
    [Obsolete("This class only acts a basic template. Do not use it for actual API calls", false)]
    public class UserPersistenceService
    {
        private readonly string PARENT_TABLE = "users";
        private MySqlConnection sqlConnection;

        public UserPersistenceService()
        {
            sqlConnection = SqlServerConnectionManager.Instance.GetSqlConnection();
        }

        // GET Users Call
        public List<User> GetUsers()
        {
            MySqlDataReader mySQLReader = null;
            List<User> users = new List<User>();

            string slqCommandString = "SELECT * FROM " + PARENT_TABLE;
            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    User user = new User
                    {
                        userID = mySQLReader.GetInt32(0),
                        name = mySQLReader.GetString(1),
                        emailAddress = mySQLReader.GetString(2),
                        phoneNumber = mySQLReader.GetString(3),
                        password = mySQLReader.GetString(4)
                    };
                    users.Add(user);

                }
                mySQLReader.Close();
                return users;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Users call in UserPersistenceService: " + ex);
                return null;
            }
        }

        // GET User Call
        public User GetUser(long id)
        {
            MySqlDataReader mySQLReader = null;

            string slqCommandString = "SELECT * FROM " + PARENT_TABLE + " WHERE UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    User user = new User
                    {
                        userID = mySQLReader.GetInt32(0),
                        name = mySQLReader.GetString(1),
                        emailAddress = mySQLReader.GetString(2),
                        phoneNumber = mySQLReader.GetString(3),
                        password = mySQLReader.GetString(4)
                    };

                    mySQLReader.Close();
                    return user;
                }

                mySQLReader.Close();

                return null;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET User call in UserPersistenceService: " + ex);
                return null;
            }
        }

        // POST User Call
        public long SaveUser(User user)
        {
            string sqlCommandString = "INSERT INTO users (UserID, Name, EmailAddress, PhoneNumber, Password) VALUES (" 
                + user.userID + ",'" 
                + user.name + "','" 
                + user.emailAddress + "','" 
                + user.phoneNumber + "','" 
                + user.password + "')";

            MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);
            try
            {
                sqlCommand.ExecuteNonQueryAsync();
                long id = sqlCommand.LastInsertedId;
                return id;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a POST User call in UserPersistenceService: " + ex);
                return -1;
            }
        }

        // DELETE User
        public bool DeleteUser(long id)
        {
            MySqlDataReader mySQLReader = null;
            string slqCommandString = "SELECT * FROM " + PARENT_TABLE + " WHERE UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    mySQLReader.Close();
                    string slqDeleteCommandString = "DELETE FROM " + PARENT_TABLE + " WHERE UserID = " + id.ToString();
                    MySqlCommand sqlDeleteCommand = new MySqlCommand(slqDeleteCommandString, sqlConnection);
                    sqlDeleteCommand.ExecuteNonQuery();
                    return true;
                }
                mySQLReader.Close();
                return false;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a DELETE User call in UserPersistenceService: " + ex);
                return false;
            }
        }

        // PATCH User
        public bool UpdateUser(long id, User user)
        {
            MySqlDataReader mySQLReader = null;

            string slqCommandString = "SELECT * FROM " + PARENT_TABLE + " WHERE UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    if (string.IsNullOrEmpty(user.name))
                    {
                        user.name = mySQLReader.GetString(1);
                    }
                    if (string.IsNullOrEmpty(user.emailAddress))
                    {
                        user.emailAddress = mySQLReader.GetString(2);
                    }
                    if (string.IsNullOrEmpty(user.phoneNumber))
                    {
                        user.phoneNumber = mySQLReader.GetString(3);
                    }
                    if (string.IsNullOrEmpty(user.password))
                    {
                        user.password = mySQLReader.GetString(4);
                    }

                    mySQLReader.Close();

                    string sqlUpdateCommandString = "UPDATE " + PARENT_TABLE +
                        " SET Name='" + user.name 
                        + "', EmailAddress='" + user.emailAddress 
                        + "', " + "PhoneNumber='" + user.phoneNumber 
                        + "', Password='" + user.password 
                        + "' WHERE UserID=" + id.ToString();                    
                    MySqlCommand sqlUpdateCommand = new MySqlCommand(sqlUpdateCommandString, sqlConnection);

                    sqlUpdateCommand.ExecuteNonQuery();
                    return true;
                }

                mySQLReader.Close();
                return false;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a PUT User call in UserPersistenceService: " + ex);
                return false;
            }
        }
    }
}