namespace api.Models
{
    public class OtherUser
    {
        public string Id;

        public string Username;

        public OtherUser(User user)
        {
            Id = user.Id;
            Username = user.Username;
        }
    }
}
