namespace GitBitterLib
{
    using System;

    public class GitBitterLoggingCommandLine : IGitBitterLogging
    {
        public GitBitterLoggingCommandLine()
        {
        }

        public void Add(string message, LoggingLevel level, string module)
        {
            if (level != LoggingLevel.Debugging)
            {
                Console.WriteLine("[" + module + "] " + message);
            }
        }
    }
}
