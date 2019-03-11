using System;

namespace DirtBnBWebAPI.Models
{
    public class Reservation
    {
        public long reservationID { get; set; }
        public long guestUserID { get; set; }
        public long accommodationID { get; set; }
        public long paymentID { get; set; }
        public DateTime startDateTime { get; set; }
        public DateTime endDateTime { get; set; }
        public int reservationLength { get; set; }
    }
}