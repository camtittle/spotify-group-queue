using Api.Domain.Entities;

namespace Api.Domain.DTOs
{
    public class UserPartial
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public UserPartial(User user)
        {
            Id = user.Id;
            Username = user.Username;
        }
    }
}
