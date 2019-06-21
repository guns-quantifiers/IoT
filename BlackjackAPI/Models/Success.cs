namespace BlackjackAPI.Models
{
    public class Success
    {
        public Success(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
