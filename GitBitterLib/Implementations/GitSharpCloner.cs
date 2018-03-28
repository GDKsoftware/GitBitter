namespace GitBitterLib
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using LibGit2Sharp;
    using LibGit2Sharp.Handlers;
    using Microsoft.Practices.Unity;

    public class HarmlessException : Exception
    {
        public HarmlessException() : base("HarmlessException")
        {
        }
    }

    public class GitSharpCloner : ICloner
    {
        private const string AppNameBitbucket = "gitbitter:bitbucket";
        private const string AppNameGithub = "gitbitter:github";
        private Identity identity;
        private GitConfig gitConfig;

        public GitSharpCloner()
        {
            gitConfig = new GitConfig();

            LoadSettings();
        }

        private void LoadSettings()
        {
            identity = new Identity(gitConfig.UserName, gitConfig.UserEmail);
        }
        
        private string GetUrlForRepository(string repository)
        {
            if (gitConfig.UseSSH)
            {
#if MONO
                // todo: find out why we need to do this on OSX/mono
                return repository.Replace("https://", "git://git@");
#else
                return repository.Replace("https://", "ssh://git@");
#endif
            }
            else
            {
                return repository;
            }
        }

        public Task Clone(string repository, string rootdir, string repodir, string branch)
        {
            var task = new Task(() =>
            {
                var logging = GitBitterContainer.Default.Resolve<IGitBitterLogging>();

                string url = GetUrlForRepository(repository);

                string stage = "initialization";
                logging.Add(stage, LoggingLevel.Info, repodir);
                try
                {
                    var options = GetCloneOptions(url, branch);
                    var fullRepoPath = Path.Combine(rootdir, repodir);

                    stage = "cloning";
                    logging.Add(stage, LoggingLevel.Info, repodir);

                    options.OnProgress = (logmessage) =>
                    {
                        logging.Add(logmessage, LoggingLevel.Info, repodir);
                        return true;
                    };
                    Repository.Clone(url, fullRepoPath, options);

                    logging.Add("Finished Cloning", LoggingLevel.Info, repodir);
                }
                catch (Exception ex)
                {
                    throw new ClonerException(ex, repodir, url, stage);
                }
            });
            task.Start();
            return task;
        }

        public Task ResetAndUpdateExisting(string repository, string rootdir, string repodir, string branchname)
        {
            var task = new Task(() =>
            {
                var logging = GitBitterContainer.Default.Resolve<IGitBitterLogging>();

                string url = GetUrlForRepository(repository);

                var fullRepoPath = Path.Combine(rootdir, repodir);
                Repository repo = null;

                string stage = "initialization";
                logging.Add(stage, LoggingLevel.Info, repodir);
                try
                {
                    var options = new PullOptions();
                    options.FetchOptions = new FetchOptions();
                    options.FetchOptions.OnProgress = (logmessage) =>
                    {
                        logging.Add(logmessage, LoggingLevel.Info, repodir);
                        return true;
                    };

                    SetCredentialsProvider(options, repository);

                    var sig = new Signature(identity, DateTime.Now);

                    repo = new Repository(fullRepoPath);

                    if (gitConfig.UseResetHard)
                    {
                        stage = "reset";
                        logging.Add(stage, LoggingLevel.Info, repodir);

                        PerformRepoReset(repository, rootdir, repodir, branchname, fullRepoPath, repo);
                    }

                    stage = "pull";
                    logging.Add(stage, LoggingLevel.Info, repodir);

                    Commands.Pull(repo, sig, options);

                    var branch = GetOrCreateLocalBranch(repo, branchname);

                    stage = "checkout " + branchname;
                    logging.Add(stage, LoggingLevel.Info, repodir);
                    Commands.Checkout(repo, branch);

                    logging.Add("Finished updating", LoggingLevel.Info, repodir);
                }
                catch (RepositoryNotFoundException)
                {
                    repo.Dispose();
                    MoveAndReClone(repository, rootdir, repodir, branchname, fullRepoPath);
                }
                catch (LibGit2SharpException ex)
                {
                    repo.Dispose();
                    if (ex.Message.Contains("There is no tracking information for the current branch"))
                    {
                        MoveAndReClone(repository, rootdir, repodir, branchname, fullRepoPath);
                    }
                }
                catch (HarmlessException)
                {
                    // do nothing
                }
                catch (Exception ex)
                {
                    repo.Dispose();
                    throw new ClonerException(ex, repodir, url, stage);
                }
            });
            task.Start();
            return task;
        }

        private void MoveToOldFolder(string fullRepoPath)
        {
            var numberOfOldFolders = 1;
            while (Directory.Exists(fullRepoPath + "." + numberOfOldFolders + ".old"))
            {
                numberOfOldFolders++;
            }

            Directory.Move(fullRepoPath, fullRepoPath + "." + numberOfOldFolders + ".old");
        }

        private void SetCredentialsProvider(PullOptions options, string repository)
        {
            if (gitConfig.UseSSH)
            {
                options.FetchOptions.CredentialsProvider = GetCredentialHandlerSSH(repository);
            }
            else
            {
                options.FetchOptions.CredentialsProvider = GetCredentialHandler(repository);
            }
        }

        private void PerformRepoReset(string repository, string rootdir, string repodir, string branchname, string fullRepoPath, Repository repo)
        {
            try
            {
                repo.Reset(ResetMode.Hard);
            }
            catch (Exception e)
            {
                repo.Dispose();
                if (e.Message.Contains("No valid git object identified by 'HEAD' exists in the repository"))
                {
                    MoveAndReClone(repository, rootdir, repodir, branchname, fullRepoPath);

                    throw new HarmlessException();
                }
                else
                {
                    throw;
                }
            }
        }

        private void MoveAndReClone(string repository, string rootdir, string repodir, string branchname, string fullRepoPath)
        {
            MoveToOldFolder(fullRepoPath);

            Clone(repository, rootdir, repodir, branchname).Wait();
        }

        private Branch GetOrCreateLocalBranch(IRepository repo, string branchname)
        {
            var localBranch = repo.Branches[branchname];

            if (localBranch == null)
            {
                var remoteBranch = GetRemoteBranch(repo, branchname);
                if (remoteBranch != null)
                {
                    return CreateLocalBranchFromRemote(repo, remoteBranch, branchname);
                }
                else
                {
                    throw new BranchException("Branch " + branchname + " does not exist.");
                }
            }

            return localBranch;
        }

        private Branch CreateLocalBranchFromRemote(IRepository repo, Branch remotebranch, string branchname)
        {
            var localbranch = repo.CreateBranch(branchname, remotebranch.Tip);

            repo.Branches.Update(localbranch, b => b.TrackedBranch = remotebranch.CanonicalName);

            return localbranch;
        }

        private Branch GetRemoteBranch(IRepository repo, string branchname)
        {
            foreach (var branch in repo.Branches)
            {
                if ((branch.FriendlyName == branch.RemoteName + "/" + branchname))
                {
                    return branch;
                }
            }

            return null;
        }

        private CredentialsHandler GetCredentialHandlerSSH(string repository)
        {
            var filesAndFolders = GitBitterContainer.Default.Resolve<IGitFilesAndFolders>();

            var privkey = filesAndFolders.SSHPrivateKeyFile();
            var pubkey = filesAndFolders.SSHPublicKeyFile();

            if (File.Exists(privkey) && File.Exists(pubkey))
            {
                var credentials = new SshUserKeyCredentials();
                credentials.Username = "git";
                credentials.Passphrase = string.Empty;
                credentials.PrivateKey = privkey;
                credentials.PublicKey = pubkey;

                return (_url, _user, _cred) => credentials;
            }
            else
            {
                var credentials = new SshAgentCredentials();
                credentials.Username = "git";

                return (_url, _user, _cred) => credentials;
            }
        }

        private CredentialsHandler GetCredentialHandler(string repository)
        {
            ICredentialManager credmanager = GitBitterContainer.Default.Resolve<ICredentialManager>();
            var credentials = new SecureUsernamePasswordCredentials();
            if (repository.Contains("bitbucket"))
            {
                var cred = credmanager.ReadCredential(AppNameBitbucket);
                credentials.Username = cred.UserName;
                credentials.Password = cred.Password;
            }
            else if (repository.Contains("github"))
            {
                var cred = credmanager.ReadCredential(AppNameGithub);
                credentials.Username = cred.UserName;
                credentials.Password = cred.Password;
            }

            return (_url, _user, _cred) => credentials;
        }

        private CloneOptions GetCloneOptions(string repository, string branch)
        {
            var options = new CloneOptions();
            options.BranchName = branch;
            options.Checkout = true;

            if (gitConfig.UseSSH)
            {
                options.CredentialsProvider = GetCredentialHandlerSSH(repository);
            }
            else
            {
                options.CredentialsProvider = GetCredentialHandler(repository);
            }

            return options;
        }
    }
}
