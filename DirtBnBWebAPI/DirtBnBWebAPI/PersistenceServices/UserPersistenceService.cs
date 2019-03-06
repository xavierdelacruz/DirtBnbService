using System;
using System.Collections.Generic;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class UserPersistenceService
    {
        private MySqlConnection sqlConnection;

        public UserPersistenceService()
        {
            sqlConnection = SqlServerConnectionManager.Instance.GetSqlConnection();
        }

        // GET Users Call
        public IEnumerable<User> GetUsers()
        {
            MySqlDataReader mySQLReader = null;
            List<User> users = new List<User>();

            string slqCommandString = "SELECT * FROM users";
            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    User user = new User();
                    user.userID = mySQLReader.GetInt32(0);
                    user.name = mySQLReader.GetString(1);
                    user.emailAddress = mySQLReader.GetString(2);
                    user.phoneNumber = mySQLReader.GetString(3);
                    user.password = mySQLReader.GetString(4);
                    users.Add(user);

                }
                mySQLReader.Close();
                return users;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a POST User call in UserPersistenceService(SaveUser): " + ex);
                return null;
            }
        }

        // GET User Call
        public User GetUser(long id)
        {
            User user = new User();
            MySqlDataReader mySQLReader = null;

            string slqCommandString = "SELECT * FROM users WHERE UserID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    user.userID = mySQLReader.GetInt32(0);
                    user.name = mySQLReader.GetString(1);
                    user.emailAddress = mySQLReader.GetString(2);
                    user.phoneNumber = mySQLReader.GetString(3);
                    user.password = mySQLReader.GetString(4);

                    mySQLReader.Close();

                    return user;
                }

                mySQLReader.Close();

                return null;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a POST User call in UserPersistenceService (SaveUser): " + ex);
                return null;
            }
        }

        // POST User Call
        public long SaveUser(User user)
        {
            string sqlCommandString = "INSERT INTO users (UserID, Name, EmailAddress, PhoneNumber, Password) VALUES (" + user.userID + ",'" +
                user.name + "','" + user.emailAddress + "','" + user.phoneNumber + "','" + user.password + "')";

            MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);
            try
            {
                sqlCommand.ExecuteNonQueryAsync();
                long id = sqlCommand.LastInsertedId;
                return id;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a POST User call in UserPersistenceService (SaveUser): " + ex);
                throw new Exception(ex.ToString());
            }
        }
    }
}