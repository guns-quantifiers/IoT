namespace Core.Components
{
    public interface ILogger
    {
        void Warning(string message);
        void Debug(string message);
    }
}
