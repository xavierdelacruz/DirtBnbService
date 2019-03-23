namespace DirtBnBWebAPI.Models
{
    public class Complaint
    {
        public long complaintID { get; set; }
        public long guestUserID { get; set; }
        public long hostUserID { get; set; }
        public long csrUserID { get; set; }
        public string content { get; set; }
        public string resolution { get; set; }
    }
}