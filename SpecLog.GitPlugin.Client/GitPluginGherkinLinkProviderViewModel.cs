using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecLog.Application.Common.PluginsInfrastructure;
using System.ComponentModel;

namespace SpecLog.GitPlugin.Client
{
    public class GitPluginGherkinLinkProviderViewModel : IGherkinLinkProviderViewModel
    {
        public bool AllowBrowseGherkinFile
        {
            get { return false; }
        }

        public bool CanLink
        {
            get { return !string.IsNullOrEmpty(FilePath); }
        }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; OnPropertyChanged("CanLink"); }
        }

        public string GetTransformedFilePath(string repositoryPath)
        {
            return FilePath;
        }

        public string ProviderDisplayName
        {
            get { return "Git repository"; }
        }

        public string ProviderType
        {
            get { return "GitGherkinProvider"; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
