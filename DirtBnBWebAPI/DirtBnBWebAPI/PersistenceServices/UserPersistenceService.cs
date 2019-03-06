using System.Data.SqlClient;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;
using System;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class UserPersistenceService
    {
        private MySqlConnection sqlConnection;

        public UserPersistenceService()
        {
            sqlConnection = SqlServerConnectionManager.Instance.GetSqlConnection();
        }

        // POST CALL
        public long SaveUser(User user)
        {
            string sqlCommandString = "INSERT INTO users (UserID, Name, EmailAddress, PhoneNumber, Password) VALUES (" + user.userID + ",'" +
                user.name + "','" + user.emailAddress + "','" + user.phoneNumber + "','"  + user.password + "')";

            MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);
            try
            {
                sqlCommand.ExecuteNonQuery();
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