using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TechTalk.SpecLog.GherkinSynchronization;
using TechTalk.SpecLog.Logging;
using LibGit2Sharp;
using TechTalk.SpecLog.Common.Utilities;

namespace SpecLog.GitPlugin.Server
{
    public class GitRepositoryGherkinFileProvider : IGherkinFileProvider
    {
        public readonly Repository gitRepository;
        public readonly ILogger logger;
        public readonly IGitRepositoryGherkinFileProviderConfiguration configuration;
        public GitRepositoryGherkinFileProvider(ILogger logger, IGitRepositoryGherkinFileProviderConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
            gitRepository = GetRepository();
        }

        public string ProviderType
        {
            get { return GitPlugin.GitGherkinFileProviderType; }
        }

        public bool FileExists(string filePath)
        {
            UpdateRepository();
            return File.Exists(GetFullPath(filePath));
        }

        public DateTime GetLastModifiedDate(string filePath)
        {
            return File.GetLastWriteTime(Path.Combine(configuration.LocalRepository, filePath));
        }

        public string GetContent(string filePath)
        {
            return File.ReadAllText(Path.Combine(configuration.LocalRepository, filePath));
        }

        private string GetFullPath(string filePath)
        {
            var fullPath = Path.GetFullPath(Path.Combine(configuration.LocalRepository, filePath));
            if (!fullPath.StartsWith(configuration.LocalRepository))
                throw new ArgumentException("Parent paths are not allowed.");

            return fullPath;
        }

        private Repository GetRepository()
        {
            try
            {
                logger.Log(TraceEventType.Verbose, "Opening repo {0}", configuration.LocalRepository);
                return new Repository(configuration.LocalRepository);
            }
            catch
            {
                var path = Repository.Clone(configuration.RemoteRepository, configuration.LocalRepository, checkout: false, credentials: GetCredentials());
                return new Repository(path);
            }
        }

        private Credentials GetCredentials()
        {
            if (string.IsNullOrEmpty(configuration.Username)) return null;
            return new Credentials { Username = configuration.Username, Password = configuration.Password };
        }

        private Timeout timeout;
        private readonly object updateSync = new object();
        private void UpdateRepository()
        {
            if (timeout != null && !timeout.Elapsed)
                return;

            lock (updateSync)
            {
                if (timeout == null || timeout.Elapsed)
                {
                    logger.Log(TraceEventType.Verbose, "Updating repo {0} from {1}",
                        configuration.LocalRepository, configuration.RemoteRepository);
                    try
                    {
                        gitRepository.Fetch("origin", credentials: GetCredentials());
                        var branch = string.Format("origin/{0}", configuration.Branch);
                        // NOTE: gitRepository.Checkout(branch) does not work...
                        gitRepository.Branches[branch].Checkout(CheckoutModifiers.Force, null, null);
                    }
                    catch (Exception ex)
                    {
                        logger.Log(TraceEventType.Warning, "Update failed: {0}", ex);
                    }

                    timeout = new Timeout(configuration.UpdateInterval);
                }
            }
        }
    }
}
