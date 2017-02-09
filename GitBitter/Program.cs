namespace GitBitter
{
    using System;
    using GitBitterLib;
    using Microsoft.Practices.Unity;

    class Program
    {
        static void Main(string[] args)
        {
            GitBitterContainer.Default.RegisterType<ICloner, GitSharpCloner>();

#if MONO
            GitBitterContainer.Default.RegisterType<ICredentialManager, CredentialManagerPlainText>();
            GitBitterContainer.Default.RegisterType<IIniFile, IniFileMadMilkman>();
            GitBitterContainer.Default.RegisterType<IGitFilesAndFolders, GitFilesAndFoldersMono>();
#else
            GitBitterContainer.Default.RegisterType<ICredentialManager, CredentialManagerWindows>();
            GitBitterContainer.Default.RegisterType<IIniFile, IniFileWindows>();
            GitBitterContainer.Default.RegisterType<IGitFilesAndFolders, GitFilesAndFoldersWindows>();
#endif

            try
            {
                if (args.Length > 0)
                {
                    var unwrapper = new PackageUnwrapper(args[0]);
                    unwrapper.StartAndWaitForUnwrapping();
                }
                else
                {
                    var unwrapper = new PackageUnwrapper();
                    unwrapper.StartAndWaitForUnwrapping();
                }

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
