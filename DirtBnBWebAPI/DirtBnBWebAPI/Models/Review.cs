namespace DirtBnBWebAPI.Models
{
    public class Review
    {
        public long reviewID { get; set; }
        public long accommodationID { get; set; }
        public long reservationID { get; set; }
        public string content { get; set; }
        public int rating { get; set; }
    }
}