namespace GitBitter
{
    using System;
    using GitBitterLib;
    using Unity;

    public class Program
    {
        public static void Main(string[] args)
        {
            GitBitterContainer.Default.RegisterType<ICloner, GitSharpCloner>();

#if MONO
            GitBitterContainer.Default.RegisterType<ICredentialManager, CredentialManagerPlainText>();
            GitBitterContainer.Default.RegisterType<IIniFile, IniFileMadMilkman>();
            GitBitterContainer.Default.RegisterType<IGitFilesAndFolders, GitFilesAndFoldersMono>();
			GitBitterContainer.Default.RegisterType<IGitBitterLogging, GitBitterLoggingCommandLine>();
#else
            GitBitterContainer.Default.RegisterType<ICredentialManager, CredentialManagerWindows>();
            GitBitterContainer.Default.RegisterType<IIniFile, IniFileWindows>();
            GitBitterContainer.Default.RegisterType<IGitFilesAndFolders, GitFilesAndFoldersWindows>();
            GitBitterContainer.Default.RegisterType<IGitBitterLogging, GitBitterLoggingCommandLine>();
#endif

            try
            {
                var parameters = new ParameterProcessing(args);

                PackageUnwrapper unwrapper = new PackageUnwrapper(parameters.Filepath);

                if (parameters.Command == ParameterCommand.Add)
                {
                    var package = new Package();
                    package.Repository = parameters.CommandArg1;
                    package.SetDefaultFolder();

                    unwrapper.Settings.Packages.Add(package);

                    unwrapper.Save();
                }
                else if (parameters.Command == ParameterCommand.Del)
                {
                    var package = unwrapper.Settings.Packages.Find(p => p.Folder.Equals(parameters.CommandArg1));
                    if (package != null)
                    {
                        unwrapper.Settings.Packages.Remove(package);
                        unwrapper.Save();
                    }
                }

                unwrapper.StartAndWaitForUnwrapping();

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    Console.WriteLine(ex.InnerException.StackTrace);
                }
                else
                {
                    Console.WriteLine(ex.StackTrace);
                }

                Environment.Exit(ex.HResult);
            }
        }
    }
}
