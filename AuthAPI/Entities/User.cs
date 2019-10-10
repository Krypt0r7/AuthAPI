namespace AuthAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public byte[] PassWordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}