namespace DirtBnBWebAPI.Models
{
    public class Accommodation
    {
        public int accommodationID { get; set; }
        public byte parking { get; set; }
        public byte tv { get; set; }
        public byte wifi { get; set; }
        public byte airConditioning { get; set; }
        public byte generalAppliances { get; set; }
        public string bedSize { get; set; }
        public int pricePerNight { get; set; }
        public string houseNumber { get; set; }
        public int hostUserID { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string province { get; set; }
    }
}