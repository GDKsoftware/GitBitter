namespace GitBitterLib
{
    using System;

    public class GitBitterLoggingCommandLine : IGitBitterLogging
    {
        public GitBitterLoggingCommandLine()
        {
        }

        public void Add(string AMessage, LoggingLevel ALevel)
        {
            if (ALevel != LoggingLevel.Debugging)
            {
                Console.WriteLine(AMessage);
            }
        }
    }
}
