namespace GitBitter
{
    using System;
    using GitBitterLib;

    class Program
    {
        static void Main(string[] args)
        {
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
                Console.WriteLine(ex.Message + "\n\n" + (ex.InnerException != null ? ex.InnerException.Message + "\n\n" + ex.InnerException.StackTrace : ex.StackTrace));

                Environment.Exit(ex.HResult);
            }
        }
    }
}
