using System;
using System.Xml.Serialization;
using TechTalk.SpecLog.GherkinSynchronization;

namespace SpecLog.GitPlugin.Server
{
    [Serializable]
    public class GitPluginConfiguration : IGherkinFilePollerConfiguration, IGitRepositoryGherkinFileProviderConfiguration
    {
        public const string PasswordKey = "GitAuthPassword";

        public string RemoteRepository { get; set; }
        public string LocalRepository { get; set; }
        public string Branch { get; set; }

        public int UpdateIntervalMinutes { get; set; }

        [XmlIgnore]
        public TimeSpan UpdateInterval
        {
            get { return TimeSpan.FromMinutes(UpdateIntervalMinutes); }
        }

        public string Username { get; set; }

        [XmlIgnore]
        public string Password { get; set; }
    }

    public static class PluginCommands
    {
        public const string SynchronizeGherkinFilesVerb = "SynchronizeGherkinFiles";
    }
}
