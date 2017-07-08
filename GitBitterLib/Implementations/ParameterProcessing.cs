namespace GitBitterLib
{
    using System;

    public enum ParameterCommand
    {
        None = 0,
        Add = 1,
        Del = 2
    }

    public class ParameterProcessing
    {
        public ParameterCommand Command { get; set; }

        public string CommandArg1 { get; set; }

        public string Filepath { get; set; }

        private string commandStr;

        private const string CmdStrAdd = "add";
        private const string CmdStrDel = "del";

        public ParameterProcessing(string[] args)
        {
            Filepath = "gitbitter.json";
            Command = ParameterCommand.None;
            commandStr = string.Empty;

            if (args.Length > 0)
            {
                if ((args.Length > 1) && (args[0].Equals(CmdStrAdd) || args[0].Equals(CmdStrDel)))
                {
                    commandStr = args[0];
                    CommandArg1 = args[1];
                }
                else if ((args.Length > 2) && (args[1].Equals(CmdStrAdd) || args[1].Equals(CmdStrDel)))
                {
                    Filepath = args[0];

                    commandStr = args[1];
                    CommandArg1 = args[2];
                }
                else
                {
                    Filepath = args[0];
                }
            }

            if (commandStr.Equals(CmdStrAdd))
            {
                Command = ParameterCommand.Add;
            }
            else if (commandStr.Equals(CmdStrDel))
            {
                Command = ParameterCommand.Del;
            }
        }
    }
}
