namespace GitBitterLib
{
    using System;

    public enum ParameterCommand
    {
        None = 0,
        Add = 1
    }

    public class ParameterProcessing
    {
        public ParameterCommand Command { get; set; }

        public string CommandArg1 { get; set; }

        public string Filepath { get; set; }

        private string commandStr;

        public ParameterProcessing(string[] args)
        {
            Filepath = "gitbitter.json";
            Command = ParameterCommand.None;

            if (args.Length > 0)
            {
                if ((args.Length > 1) && (args[0].Equals("add")))
                {
                    commandStr = args[0];
                    Command = ParameterCommand.Add;
                    CommandArg1 = args[1];
                }
                else if ((args.Length > 2) && (args[1].Equals("add")))
                {
                    Filepath = args[0];

                    commandStr = args[1];
                    Command = ParameterCommand.Add;
                    CommandArg1 = args[2];
                }
                else
                {
                    Filepath = args[0];
                }
            }
        }
    }
}
