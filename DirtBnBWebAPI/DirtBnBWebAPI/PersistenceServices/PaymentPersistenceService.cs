using System;
using System.Collections.Generic;
using System.Diagnostics;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class PaymentPersistenceService
    {
        private MySqlConnection sqlConnection;

        public PaymentPersistenceService()
        {
            sqlConnection = SqlServerConnectionManager.Instance.GetSqlConnection();
        }

        // GET Payments Call
        public List<Payment> GetPayments()
        {
            MySqlDataReader mySQLReader = null;
            List<Payment> Payments = new List<Payment>();

            string slqCommandString = "SELECT * FROM Payments";
            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    Payment Payment = new Payment();
                    Payment.paymentID = mySQLReader.GetInt32(0);
                    Payment.hostUserID = mySQLReader.GetInt32(1);
                    Payment.CCNumber = mySQLReader.GetString(2);
                    Payment.amount = mySQLReader.GetFloat(3);
                    Payments.Add(Payment);
                }
                mySQLReader.Close();
                return Payments;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine("Found an error when performing a GET Payments call in PaymentPersistenceService(GetPayments): " + ex);
                return null;
            }
        }

        // GET Payment Call
        public Payment GetPayment(long id)
        {
            Payment Payment = new Payment();
            MySqlDataReader mySQLReader = null;

            string slqCommandString = "SELECT * FROM Payments WHERE PaymentID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    Payment.paymentID = mySQLReader.GetInt32(0);
                    Payment.hostUserID = mySQLReader.GetInt32(1);
                    Payment.CCNumber = mySQLReader.GetString(2);
                    Payment.amount = mySQLReader.GetFloat(3);

                    mySQLReader.Close();
                    return Payment;
                }
                mySQLReader.Close();
                return null;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Payment call in PaymentPersistenceService (GetPayment): " + ex);
                return null;
            }
        }

        // POST Payment
        public long SavePayment(Payment payment)
        {


            string sqlCommandString = "INSERT INTO Payments VALUES(null," + payment.hostUserID + "," +
                payment.CCNumber + "," + payment.amount + ")";
            Debug.WriteLine(sqlCommandString);

            MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);
            try
            {
                sqlCommand.ExecuteNonQuery();
                long id = sqlCommand.LastInsertedId;
                return id;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a POST Payment call in PaymentPersistenceService (SavePayment): " + ex);
                return -1;
            }
        }

        // DELETE Payment
        // Intentionally omitted.
    }
}