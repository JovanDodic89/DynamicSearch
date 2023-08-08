namespace DynamicSearch.Application.Exceptions
{
    public class RequestNotValidException : Exception
    {
        public RequestNotValidException(string field, string message) : base(message) 
        {
            Field = field;
        }

        public string Field { get; private set; }
    }
}
