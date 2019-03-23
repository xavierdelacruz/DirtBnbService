namespace DirtBnBWebAPI.Models
{
    public class Accommodation
    {
        public long accommodationID { get; set; }
        public byte? parking { get; set; }
        public byte? tv { get; set; }
        public byte? wifi { get; set; }
        public byte? airConditioning { get; set; }
        public byte? generalAppliances { get; set; }
        public string bedSize { get; set; }
        public int? pricePerNight { get; set; }
        public string houseNumber { get; set; }
        public int? hostUserID { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string province { get; set; }
    }

    public class AccommodationNoAmenities
    {
        public long accommodationID { get; set; }
        public string bedSize { get; set; }
        public int? pricePerNight { get; set; }
        public string houseNumber { get; set; }
        public int? hostUserID { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string province { get; set; }
    }

    public class AccommodationNoBed
    {
        public long accommodationID { get; set; }
        public byte? parking { get; set; }
        public byte? tv { get; set; }
        public byte? wifi { get; set; }
        public byte? airConditioning { get; set; }
        public byte? generalAppliances { get; set; }
        public int? pricePerNight { get; set; }
        public string houseNumber { get; set; }
        public int? hostUserID { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string province { get; set; }
    }

    public class AccommodationNoPricePerNight
    {
        public long accommodationID { get; set; }
        public byte? parking { get; set; }
        public byte? tv { get; set; }
        public byte? wifi { get; set; }
        public byte? airConditioning { get; set; }
        public byte? generalAppliances { get; set; }
        public string bedSize { get; set; }
        public string houseNumber { get; set; }
        public int? hostUserID { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string province { get; set; }
    }

    public class AccommodationNoAmenitiesNoBedSize
    {
        public long accommodationID { get; set; }
        public int? pricePerNight { get; set; }
        public string houseNumber { get; set; }
        public int? hostUserID { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string province { get; set; }
    }

    public class AccommodationsNoAmenitiesNoPricePerNight
    {
        public long accommodationID { get; set; }
        public string bedSize { get; set; }
        public string houseNumber { get; set; }
        public int? hostUserID { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string province { get; set; }
    }

    public class AccommodationsNoBedSizeNoPricePerNight
    {
        public long accommodationID { get; set; }
        public byte? parking { get; set; }
        public byte? tv { get; set; }
        public byte? wifi { get; set; }
        public byte? airConditioning { get; set; }
        public byte? generalAppliances { get; set; }
        public string houseNumber { get; set; }
        public int? hostUserID { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string province { get; set; }
    }

    public class AccommodationAvg
    {
        public string city { get; set; }
        public string province { get; set; }
        public long avgAccommodationPrice { get; set; }
    }
}