namespace DirtBnBWebAPI.Models
{
    public class User
    {
        public long userID { get; set; }
        public string name { get; set; }
        public string emailAddress { get; set; }
        public string phoneNumber { get; set; }
        public string password { get; set; }
    }
}