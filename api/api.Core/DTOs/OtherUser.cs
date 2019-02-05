using api.Domain.Entities;

namespace api.Domain.DTOs
{
    public class OtherUser
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public OtherUser(User user)
        {
            Id = user.Id;
            Username = user.Username;
        }
    }
}
