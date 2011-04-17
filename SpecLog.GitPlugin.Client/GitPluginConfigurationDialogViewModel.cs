using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using TechTalk.SpecLog.Application.Common;
using TechTalk.SpecLog.Application.Common.Dialogs;
using TechTalk.SpecLog.Application.Common.PluginsInfrastructure;

namespace SpecLog.GitPlugin.Client
{
    public class GitPluginConfigurationDialogViewModel : IDialogViewModel
    {
        public readonly GitPluginConfiguration config;
        public readonly Dictionary<string, string> sensitiveConfig = new Dictionary<string, string>();
        public GitPluginConfigurationDialogViewModel(string config, bool enabled)
        {
            this.config = Deserialize(config);
            isEnabled = enabled;

            CloseCommand = new DelegateCommand(Cancel);
            SaveCommand = new DelegateCommand(Save, CanSave);
        }

        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }

        public string Caption
        {
            get { return "Git plugin configuration"; }
        }

        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; SaveCommand.RaiseCanExecuteChanged(); }
        }

        public string RemotePath
        {
            get { return config.RemoteRepository; }
            set { config.RemoteRepository = value; SaveCommand.RaiseCanExecuteChanged(); }
        }

        public string LocalPath
        {
            get { return config.LocalRepository; }
            set { config.LocalRepository = value; SaveCommand.RaiseCanExecuteChanged(); }
        }

        public string Branch
        {
            get { return config.Branch; }
            set { config.Branch = value; SaveCommand.RaiseCanExecuteChanged(); }
        }

        public int UpdateIntervalMinutes
        {
            get { return config.UpdateIntervalMinutes; }
            set { config.UpdateIntervalMinutes = value; SaveCommand.RaiseCanExecuteChanged(); }
        }

        public event EventHandler<EventArgs<bool?>> Close = delegate { };

        public void Cancel()
        {
            Close(this, new EventArgs<bool?>(false));
        }

        public bool CanSave()
        {
            return !IsEnabled
                || (!string.IsNullOrWhiteSpace(RemotePath)
                && !string.IsNullOrWhiteSpace(LocalPath)
                && UpdateIntervalMinutes > 0);
        }

        public void Save()
        {
            Close(this, new EventArgs<bool?>(true));
        }

        public IDialogResult GetDialogResultData()
        {
            return new PluginConfigurationDialogResult(Serialize(config), sensitiveConfig, IsEnabled);
        }

        private GitPluginConfiguration Deserialize(string config)
        {
            if (string.IsNullOrWhiteSpace(config))
                return new GitPluginConfiguration();

            var serializer = new XmlSerializer(typeof(GitPluginConfiguration));
            using (var reader = new StringReader(config))
            {
                return (GitPluginConfiguration)serializer.Deserialize(reader);
            }
        }

        private string Serialize(GitPluginConfiguration config)
        {
            if (config == null)
                return string.Empty;

            var serializer = new XmlSerializer(typeof(GitPluginConfiguration));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, config);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
