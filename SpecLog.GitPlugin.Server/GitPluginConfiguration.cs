using System;
using System.Xml.Serialization;
using TechTalk.SpecLog.GherkinSynchronization;

namespace SpecLog.GitPlugin.Server
{
    [Serializable]
    public class GitPluginConfiguration : IGherkinFilePollerConfiguration, IGitRepositoryGherkinFileProviderConfiguration
    {
        public string RemoteRepository { get; set; }
        public string LocalRepository { get; set; }
        public string Branch { get; set; }
        public int UpdateIntervalMinutes { get; set; }

        [XmlIgnore]
        public TimeSpan UpdateInterval
        {
            get { return TimeSpan.FromMinutes(UpdateIntervalMinutes); }
        }
    }

    public static class PluginCommands
    {
        public const string SynchronizeGherkinFilesVerb = "SynchronizeGherkinFiles";
    }
}
