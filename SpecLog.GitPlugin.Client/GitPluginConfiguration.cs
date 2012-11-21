using System;

namespace SpecLog.GitPlugin.Client
{
    [Serializable]
    public class GitPluginConfiguration
    {
        public GitPluginConfiguration()
        {
            UpdateIntervalMinutes = 5;
            Branch = "master";
        }

        public string RemoteRepository { get; set; }
        public string LocalRepository { get; set; }
        public string Branch { get; set; }
        public int UpdateIntervalMinutes { get; set; }
    }

    public static class PluginCommands
    {
        public const string SynchronizeGherkinFilesVerb = "SynchronizeGherkinFiles";
    }
}
