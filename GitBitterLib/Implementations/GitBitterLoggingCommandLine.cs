namespace GitBitterLib
{
    using System;

    public class GitBitterLoggingCommandLine : IGitBitterLogging
    {
        public GitBitterLoggingCommandLine()
        {
        }

        public void Add(string AMessage, LoggingLevel ALevel, string AModule)
        {
            if (ALevel != LoggingLevel.Debugging)
            {
                Console.WriteLine("[" + AModule + "] " + AMessage);
            }
        }
    }
}
