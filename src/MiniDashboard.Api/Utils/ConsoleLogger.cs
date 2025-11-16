namespace MiniDashboard.Api.Utils
{
    public class ConsoleLogger : Common.ILogger
    {
        public void Debug(string? msg, params object[] args)
        => Write("DEBUG", ConsoleColor.Cyan, msg, args);

        public void Info(string? msg, params object[] args)
            => Write("INFO", ConsoleColor.Green, msg, args);

        public void Verbose(string? msg, params object[] args)
            => Write("VERBOSE", ConsoleColor.Gray, msg, args);

        public void Error(string? msg, params object[] args)
            => Write("ERROR", ConsoleColor.Red, msg, args);


        private void Write(string level, ConsoleColor color, string? msg, params object[] args)
        {
            Console.ForegroundColor = color;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string text = msg ?? "<null>";

            if (args != null && args.Length > 0)
            {
                try
                {
                    text = string.Format(text, args);
                }
                catch
                {
                    text += " | Args: " + string.Join(", ", args.Select(a => a?.ToString() ?? "<null>"));
                }
            }

            Console.WriteLine($"[{timestamp}] [{level}] {text}");

            Console.ResetColor();
        }
    }
}
