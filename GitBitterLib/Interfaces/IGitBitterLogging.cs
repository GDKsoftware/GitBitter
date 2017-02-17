namespace GitBitterLib
{
    using System;

    public enum LoggingLevel
    {
        Error = 1,
        Info = 2,
        Debugging = 3
    };

    public interface IGitBitterLogging
    {
        void Add(string AMessage, LoggingLevel ALevel);
    }
}
