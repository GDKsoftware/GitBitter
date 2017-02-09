namespace GitBitterLib
{
    using System.Threading.Tasks;

    public interface ICloner
    {
        Task Clone(string repository, string rootdir, string repodir, string branch);

        Task ResetAndUpdateExisting(string repository, string rootdir, string repodir, string branch);
    }
}
