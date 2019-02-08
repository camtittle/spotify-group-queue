namespace Api.Business.Exceptions
{
    public class ResourceNotFoundException : ApiException
    {
        public ResourceNotFoundException(string message) : base(message)
        {
        }
    }
}
