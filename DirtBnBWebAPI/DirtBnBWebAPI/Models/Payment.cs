namespace DirtBnBWebAPI.Models
{
    public class Payment
    {
        public long paymentID { get; set; }
        public long hostUserID { get; set; }
        public string CCNumber { get; set; }
        public float amount { get; set; }
    }
}