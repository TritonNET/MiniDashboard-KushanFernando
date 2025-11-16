namespace MiniDashboard.Common
{
    public interface ILogger
    {
        void Error(string? msg, params object[] args);

        void Info(string? msg, params object[] args);

        void Debug(string? msg, params object[] args);

        void Verbose(string? msg, params object[] args);
    }
}
