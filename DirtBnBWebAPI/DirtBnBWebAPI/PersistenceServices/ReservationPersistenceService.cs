using System;
using System.Collections.Generic;
using System.Diagnostics;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class ReservationPersistenceService
    {
        private readonly string PARENT_TABLE = "reservations";
        private readonly string CHILD_TABLE = "reservations_fd_reservation_period";

        private MySqlConnection sqlConnection;

        public ReservationPersistenceService()
        {
            sqlConnection = SqlServerConnectionManager.Instance.GetSqlConnection();
        }

        // GET Reservations Call
        public List<Reservation> GetReservations()
        {
            MySqlDataReader mySQLReader = null;
            List<Reservation> reservations = new List<Reservation>();

            string sqlCommandString = "SELECT * FROM " + PARENT_TABLE + "," + CHILD_TABLE + 
                " WHERE(" + PARENT_TABLE + ".startDateTime = " + CHILD_TABLE + ".startDateTime) AND(" +
                PARENT_TABLE + ".endDateTime = " + CHILD_TABLE + ".endDateTime)";
            MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);
            Debug.WriteLine(sqlCommandString);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    Reservation reservation = new Reservation
                    {
                        reservationID = mySQLReader.GetInt32(0),
                        guestUserID = mySQLReader.GetInt32(1),
                        accommodationID = mySQLReader.GetInt32(2),
                        startDateTime = mySQLReader.GetDateTime(3),
                        endDateTime = mySQLReader.GetDateTime(4),
                        reservationLength = (int) mySQLReader["reservationLength"]
                    };
                    reservations.Add(reservation);
                }
                mySQLReader.Close();
                return reservations;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Reservation call in ReservationPersistenceService: " + ex);
                return null;
            }
        }

        // GET Reservation Call
        public Reservation GetReservation(long id)
        {

            MySqlDataReader mySQLReader = null;
            string slqCommandString = "SELECT * FROM " + PARENT_TABLE + "," + CHILD_TABLE +
                " WHERE(" + PARENT_TABLE + ".startDateTime = " + CHILD_TABLE + ".startDateTime) AND(" +
                PARENT_TABLE + ".endDateTime = " + CHILD_TABLE + ".endDateTime) AND(" +
                PARENT_TABLE + ".ReservationID = " + id.ToString() + ")";
            Debug.WriteLine(slqCommandString);
            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    Reservation reservation = new Reservation
                    {
                        reservationID = mySQLReader.GetInt32(0),
                        guestUserID = mySQLReader.GetInt32(1),
                        accommodationID = mySQLReader.GetInt32(2),
                        startDateTime = mySQLReader.GetDateTime(3),
                        endDateTime = mySQLReader.GetDateTime(4),
                        reservationLength = (int)mySQLReader["reservationLength"]
                    };
                    mySQLReader.Close();
                    return reservation;
                }
                mySQLReader.Close();
                return null;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Reservation call in ReservationPersistenceService: " + ex);
                return null;
            }
        }

        // POST Reservation Call
        public long SaveReservation(Reservation reservation)
        {
            TimeSpan diff = reservation.endDateTime - reservation.startDateTime;
            int reservationLength = diff.Days + 1;
            Debug.WriteLine("RESERVATION LENGTH IS: " + reservationLength);

            string childSqlCommandStringPeriod = "INSERT INTO " + CHILD_TABLE + " VALUES ('"
                + reservation.startDateTime.ToString("yyyy'-'MM'-'dd") + "','"
                + reservation.endDateTime.ToString("yyyy'-'MM'-'dd") + "','"
                // TODO: add logic to calculate reservation length rather than requiring it be specified
                + reservationLength + "')";
            Debug.WriteLine(childSqlCommandStringPeriod);

            string sqlCommandString = "INSERT INTO " + PARENT_TABLE + " VALUES ("
                + reservation.reservationID + ",'"
                + reservation.guestUserID + "','"
                + reservation.accommodationID + "','"
                + reservation.startDateTime.ToString("yyyy'-'MM'-'dd") + "','"
                + reservation.endDateTime.ToString("yyyy'-'MM'-'dd") + "')";
            Debug.WriteLine(sqlCommandString);

            MySqlCommand childSqlCommandPeriod = new MySqlCommand(childSqlCommandStringPeriod, sqlConnection);
            MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);

            try
            {
                childSqlCommandPeriod.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Child tuple already present due to FD; skipping insertion of child tuple.");
            }

            try
            {
                sqlCommand.ExecuteNonQuery();
                long id = sqlCommand.LastInsertedId;
                return id;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine("Found an error when performing a POST Reservation call in ReservationPersistenceService: " + ex);
                return -1;
            }
        }

        // DELETE Reservation
        public bool DeleteReservation(long id)
        {
            MySqlDataReader mySQLReader = null;
            string slqCommandString = "SELECT * FROM " + PARENT_TABLE + " WHERE ReservationID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    mySQLReader.Close();
                    string slqDeleteCommandString = "DELETE FROM " + PARENT_TABLE + " WHERE ReservationID = " + id.ToString();
                    MySqlCommand sqlDeleteCommand = new MySqlCommand(slqDeleteCommandString, sqlConnection);
                    sqlDeleteCommand.ExecuteNonQuery();
                    return true;
                }
                mySQLReader.Close();
                return false;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a DELETE Reservation call in ReservationPersistenceService: " + ex);
                return false;
            }
        }
    }
}