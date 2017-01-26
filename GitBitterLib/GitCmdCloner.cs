namespace GitBitterLib
{
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    public class GitCmdCloner : ICloner
    {
        private string gitCommand;

        public GitCmdCloner()
        {
            gitCommand = "git";
        }

        private Process AsyncExec(string filename, string arguments, string workingdirectory)
        {
            var info = new ProcessStartInfo();
            info.FileName = gitCommand;
            info.Arguments = arguments;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = false;
            if (workingdirectory != "")
            {
                info.WorkingDirectory = workingdirectory;
            }

            var p = Process.Start(info);

            p.WaitForExit();

            return p;
        }

        private void SyncExec(string filename, string arguments, string workingdirectory)
        {
            AsyncExec(filename, arguments, workingdirectory).WaitForExit();
        }

        private string GetDestinationFolder(string rootdir, string repodir)
        {
            var path = Path.Combine(rootdir, repodir);
            return path;
        }

        public Task Clone(string repository, string rootdir, string repodir, string branch)
        {
            var task = new Task(() =>
            {
                var destinationdir = GetDestinationFolder(rootdir, repodir);
                SyncExec(gitCommand, "clone \"" + repository + "\" \"" + destinationdir + "\"", rootdir);
                SyncExec(gitCommand, "checkout " + branch, destinationdir);
            });

            task.Start();

            return task;
        }

        public Task ResetAndUpdateExisting(string repository, string rootdir, string repodir, string branch)
        {
            var task = new Task(() => {
                var destinationdir = GetDestinationFolder(rootdir, repodir);
                SyncExec(gitCommand, "reset --hard", destinationdir);
                SyncExec(gitCommand, "pull", destinationdir);
                SyncExec(gitCommand, "checkout " + branch, destinationdir);
            });

            task.Start();

            return task;
        }
    }
}
