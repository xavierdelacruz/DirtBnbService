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

            string slqCommandString = "SELECT * FROM Payments, Payments_fd WHERE(Payments.CCNumber = Payments_fd.CCNumber)";
            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    Payment Payment = new Payment();
                    Payment.paymentID = mySQLReader.GetInt32(0);
                    Payment.hostUserID = mySQLReader.GetInt32(1);
                    Payment.reservationID = mySQLReader.GetInt32(2);
                    Payment.CCNumber = mySQLReader.GetString(3);
                    Payment.amount = mySQLReader.GetFloat(4);
                    // 5 is CCNumber again
                    Payment.CCType = mySQLReader.GetString(6);
                    Payment.CCVerificationCode = mySQLReader.GetInt32(7);
                    Payment.CCExpiryDate = mySQLReader.GetDateTime(8);
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
            string slqCommandString = "SELECT * FROM Payments, Payments_fd WHERE(" +
                "Payments.CCNumber = Payments_fd.CCNumber) AND " +
                "Payments.PaymentID = " + id.ToString();
            
            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    Payment.paymentID = mySQLReader.GetInt32(0);
                    Payment.hostUserID = mySQLReader.GetInt32(1);
                    Payment.reservationID = mySQLReader.GetInt32(2);
                    Payment.CCNumber = mySQLReader.GetString(3);
                    Payment.amount = mySQLReader.GetFloat(4);
                    // 5 is CCNumber again
                    Payment.CCType = mySQLReader.GetString(6);
                    Payment.CCVerificationCode = mySQLReader.GetInt32(7);
                    Payment.CCExpiryDate = mySQLReader.GetDateTime(8);

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
            string childSqlCommandString = "INSERT INTO Payments_fd VALUES ('"
                + payment.CCNumber.ToString() + "','"
                + payment.CCType + "','" 
                + payment.CCVerificationCode.ToString() + "','"
                + payment.CCExpiryDate.ToString("yyyy'-'MM'-'dd") + "')";
            Debug.WriteLine(childSqlCommandString);

            string sqlCommandString = "INSERT INTO Payments VALUES(null," + payment.hostUserID + "," +  payment.reservationID + "," + 
                payment.CCNumber + "," + payment.amount + ")";
            Debug.WriteLine(sqlCommandString);

            MySqlCommand childSqlCommandPeriod = new MySqlCommand(childSqlCommandString, sqlConnection);
            MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);
            try
            {
                childSqlCommandPeriod.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Child tuple already present due to FD; skipping insertion of child tuple.");
            }
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