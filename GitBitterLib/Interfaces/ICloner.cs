namespace GitBitterLib
{
    using System;
    using System.Threading.Tasks;

    public class ClonerException : Exception
    {
        public string Activity { get; set; }

        public string Folder { get; set; }

        public ClonerException(Exception exception, string folder, string url, string activity) :
        base("[" + folder + " (" + url + ")] While performing " + activity + ", this except was thrown: " + exception.Message, exception)
        {
            Folder = folder;
            Activity = activity;
        }
    }

    public class BranchException : Exception
    {
        public BranchException(string message) : base(message)
        {
        }
    }

    public interface ICloner
    {
        Task Clone(string repository, string rootdir, string repodir, string branchname);

        Task ResetAndUpdateExisting(string repository, string rootdir, string repodir, string branchname);
    }
}
