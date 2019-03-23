using System;

namespace DirtBnBWebAPI.Models
{
    public class Payment
    {
        public long paymentID { get; set; }
        public long hostUserID { get; set; }
        public long reservationID { get; set; }
        public string CCNumber { get; set; }
        public float amount { get; set; }
        public string CCType { get; set; }
        public int CCVerificationCode { get; set; }
        public DateTime CCExpiryDate { get; set; }
    }
}