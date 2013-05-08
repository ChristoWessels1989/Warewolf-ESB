﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Dev2.Runtime.Configuration.ComponentModel;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Dev2.Runtime.Configuration.Settings
{
    public class LoggingSettings : SettingsBase, IDataErrorInfo, ILoggingSettings
    {
        #region Fields

        public new const string SettingName = "Logging";

        private bool _isLoggingEnabled;
        private bool _isVersionLogged;
        private bool _isTypeLogged;
        private bool _isDurationLogged;
        private bool _isDataAndTimeLogged;
        private bool _isInputLogged;
        private bool _isOutputLogged;
        private int _nestedLevelCount;
        private string _logFileDirectory;
        private string _serviceInput;
        private IWorkflowDescriptor _postWorkflow;
        private ObservableCollection<IWorkflowDescriptor> _workflows;
        private bool _logAll;
        private bool _runPostWorkflow;

        #endregion

        #region Properties

        public bool RunPostWorkflow
        {
            get { return _runPostWorkflow; }
            set
            {
                if (_runPostWorkflow == value)
                {
                    return;
                }

                _runPostWorkflow = value;
                NotifyOfPropertyChange(() => RunPostWorkflow);
            }
        }

        public bool IsLoggingEnabled
        {
            get
            {
                return _isLoggingEnabled;
            }
            set
            {
                _isLoggingEnabled = value;
                NotifyOfPropertyChange(() => IsLoggingEnabled);
            }
        }

        public bool IsVersionLogged
        {
            get
            {
                return _isVersionLogged;
            }
            set
            {
                _isVersionLogged = value;
                NotifyOfPropertyChange(() => IsVersionLogged);
            }
        }

        public bool IsTypeLogged
        {
            get
            {
                return _isTypeLogged;
            }
            set
            {
                _isTypeLogged = value;
                NotifyOfPropertyChange(() => IsTypeLogged);
            }
        }

        public bool IsDurationLogged
        {
            get
            {
                return _isDurationLogged;
            }
            set
            {
                _isDurationLogged = value;
                NotifyOfPropertyChange(() => IsDurationLogged);
            }
        }

        public bool IsDataAndTimeLogged
        {
            get
            {
                return _isDataAndTimeLogged;
            }
            set
            {
                _isDataAndTimeLogged = value;
                NotifyOfPropertyChange(() => IsDataAndTimeLogged);
            }
        }

        public bool IsInputLogged
        {
            get
            {
                return _isInputLogged;
            }
            set
            {
                _isInputLogged = value;
                NotifyOfPropertyChange(() => IsInputLogged);
            }
        }

        public bool IsOutputLogged
        {
            get
            {
                return _isOutputLogged;
            }
            set
            {
                _isOutputLogged = value;
                NotifyOfPropertyChange(() => IsOutputLogged);
            }
        }

        public bool LogAll
        {
            get { return _logAll; }
            set
            {
                if (_logAll == value)
                {
                    return;
                }

                _logAll = value;
                NotifyOfPropertyChange(() => LogAll);
            }
        }

        public int NestedLevelCount
        {
            get
            {
                return _nestedLevelCount;
            }
            set
            {
                _nestedLevelCount = value;
                NotifyOfPropertyChange(() => NestedLevelCount);
            }
        }

        public string LogFileDirectory
        {
            get
            {
                return _logFileDirectory;
            }
            set
            {
                _logFileDirectory = value;
                NotifyOfPropertyChange(() => LogFileDirectory);
            }
        }

        public string ServiceInput
        {
            get
            {
                return _serviceInput;
            }
            set
            {
                _serviceInput = value;
                NotifyOfPropertyChange(() => ServiceInput);
            }
        }

        public IWorkflowDescriptor PostWorkflow
        {
            get
            {
                return _postWorkflow;
            }
            set
            {
                if (_postWorkflow == value)
                {
                    return;
                }

                _postWorkflow = value;
                NotifyOfPropertyChange(() => PostWorkflow);
            }
        }

        public ObservableCollection<IWorkflowDescriptor> Workflows
        {
            get
            {
                if (_workflows == null)
                {
                    _workflows = new ObservableCollection<IWorkflowDescriptor>();
                }
                return _workflows;
            }
        }    

        #endregion

        #region CTOR

        public LoggingSettings(string webserverUri)
            : base(SettingName, "Logging", webserverUri)
        {
        }

        public LoggingSettings(XElement xml, string webserverUri)
            : base(xml, webserverUri)
        {
            var postWorkflow = xml.Element("PostWorkflow");
            if (postWorkflow != null)
            {
                PostWorkflow = new WorkflowDescriptor(xml.Element("PostWorkflow"));
            }

            bool boolValue;
            int intValue;
            IsLoggingEnabled = bool.TryParse(xml.AttributeSafe("IsLoggingEnabled"), out boolValue) && boolValue;
            IsVersionLogged = bool.TryParse(xml.AttributeSafe("IsVersionLogged"), out boolValue) && boolValue;
            IsTypeLogged = bool.TryParse(xml.AttributeSafe("IsTypeLogged"), out boolValue) && boolValue;
            IsDurationLogged = bool.TryParse(xml.AttributeSafe("IsDurationLogged"), out boolValue) && boolValue;
            IsDataAndTimeLogged = bool.TryParse(xml.AttributeSafe("IsDataAndTimeLogged"), out boolValue) && boolValue;
            IsInputLogged = bool.TryParse(xml.AttributeSafe("IsInputLogged"), out boolValue) && boolValue;
            IsOutputLogged = bool.TryParse(xml.AttributeSafe("IsOutputLogged"), out boolValue) && boolValue;
            NestedLevelCount = Int32.TryParse(xml.AttributeSafe("NestedLevelCount"), out intValue) ? intValue : 0;
            LogAll = bool.TryParse(xml.AttributeSafe("LogAll"), out boolValue) && boolValue;
            LogFileDirectory = xml.AttributeSafe("LogFileDirectory");
            ServiceInput = xml.AttributeSafe("ServiceInput");

            var workflows = xml.Element("Workflows");
            if (workflows == null)
            {
                return;
            }

            foreach (var workflow in workflows.Elements())
            {
                Workflows.Add(new WorkflowDescriptor(workflow));
            }
        }

        #endregion

        #region ToXml

        public override XElement ToXml()
        {
            XElement postWorkflow = null;
            if (PostWorkflow != null)
            {
                postWorkflow = PostWorkflow.ToXml();
                postWorkflow.Name = "PostWorkflow";
            }

            var workflows = new XElement("Workflows");

            var toPersist = from wf in Workflows
                            where wf.IsSelected
                            select wf;

            foreach (var workflow in toPersist)
            {
                workflows.Add(workflow.ToXml());
            }

            var result = base.ToXml();
            result.Add(
                new XAttribute("IsLoggingEnabled", IsLoggingEnabled),
                new XAttribute("IsVersionLogged", IsVersionLogged),
                new XAttribute("IsTypeLogged", IsTypeLogged),
                new XAttribute("IsDurationLogged", IsDurationLogged),
                new XAttribute("IsDataAndTimeLogged", IsDataAndTimeLogged),
                new XAttribute("IsInputLogged", IsInputLogged),
                new XAttribute("IsOutputLogged", IsOutputLogged),
                new XAttribute("NestedLevelCount", NestedLevelCount),
                new XAttribute("LogAll", LogAll),
                new XAttribute("LogFileDirectory", LogFileDirectory ?? string.Empty),
                new XAttribute("ServiceInput", ServiceInput ?? string.Empty),
                postWorkflow,
                workflows
                );
            return result;
        }

        #endregion

        public string this[string propertyName]
        {
            get
            {
                string result = string.Empty;
                propertyName = propertyName ?? string.Empty;
                if (propertyName == string.Empty || propertyName == "PostWorkflow")
                {
                    if (RunPostWorkflow && !Workflows.Contains(PostWorkflow))
                    {
                        result = "Invalid workflow selected";
                    }
                }
                return result;
            }
        }

        public string Error { get; private set; }
    }
}