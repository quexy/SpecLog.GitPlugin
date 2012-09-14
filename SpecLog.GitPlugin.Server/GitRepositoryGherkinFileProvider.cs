using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GitSharp;
using GitSharp.Commands;
using GitSharp.Core.Exceptions;
using TechTalk.SpecLog.GherkinSynchronization;
using TechTalk.SpecLog.Logging;

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
            get { return "GitGherkinProvider"; }
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
                logger.Log(TraceEventType.Verbose, "Cloning repo {0} to {1}", configuration.RemoteRepository, configuration.LocalRepository);
                var cloneCmd = new CloneCommand
                {
                    Quiet = true,
                    Bare = false,
                    Source = configuration.RemoteRepository,
                    Directory = configuration.LocalRepository,
                    Branch = configuration.Branch,
                    OutputStream = new StreamWriter(new NullStream())
                };
                cloneCmd.Execute();
                return cloneCmd.Repository;
            }
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
                    var fetchCmd = new FetchCommand
                    {
                        Remote = configuration.RemoteRepository,
                        Repository = gitRepository,
                        OutputStream = new StreamWriter(new NullStream())
                    };
                    try
                    {
                        fetchCmd.Execute();
                    }
                    catch (Exception ex)
                    {
                        if (ex is TransportException && ex.Message == "Nothing to fetch.")
                            logger.Log(TraceEventType.Verbose, "Nothing to fetch.");
                        else
                            logger.Log(TraceEventType.Warning, "Fetch failed: {0}", ex);
                    }

                    logger.Log(TraceEventType.Verbose, "Checkout branch {0}", configuration.Branch);
                    var checkoutCmd = new CheckoutCommand
                    {
                        Quiet = true,
                        Repository = gitRepository,
                        Arguments = new List<string> { configuration.Branch },
                        BranchCreate = String.Empty,
                        OutputStream = new StreamWriter(new NullStream())
                    };
                    try { checkoutCmd.Execute(); }
                    catch (Exception ex) { logger.Log(TraceEventType.Warning, "Checkout failed: {0}", ex); }

                    timeout = new Timeout(configuration.UpdateInterval);
                }
            }
        }

        public class Timeout
        {
            private readonly DateTime endtime;

            public Timeout(TimeSpan duration)
            {
                endtime = DateTime.Now + duration;
            }

            public bool Elapsed
            {
                get { return DateTime.Now > endtime; }
            }
        }
    }
}
