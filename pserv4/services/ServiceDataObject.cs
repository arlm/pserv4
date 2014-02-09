﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pserv4.services
{
    public class ServiceDataObject : DataObject
    {
        public string DisplayName { get; private set; }
        public string ServiceType { get; private set; }
        
        public string StartType { get; private set; }
        public string BinaryPathName { get; private set; }
        public string LoadOrderGroup { get; private set; }
        public string ErrorControl { get; private set; }
        public string TagId { get; private set; }
        public string Description { get; private set; }
        public string User { get; private set; }

        public string ControlsAcceptedString
        {
            get
            {
                return ServicesLocalisation.Localized(ControlsAccepted);
            }
        }

        public string Win32ExitCode { get; private set; }
        public string ServiceSpecificExitCode { get; private set; }
        public string CheckPoint { get; private set; }
        public string WaitHint { get; private set; }
        public string ServiceFlags { get; private set; }

        public string CurrentStateString
        { 
            get
            {
                return ServicesLocalisation.Localized(CurrentState);
            }
        }

        private SC_RUNTIME_STATUS _CurrentState;
        public SC_RUNTIME_STATUS CurrentState
        {
            get
            {
                return _CurrentState;
            }
            private set
            {
                if( _CurrentState != value )
                {
                    _CurrentState = value;
                    NotifyPropertyChanged("CurrentStateString");

                    bool isRunning = (_CurrentState == SC_RUNTIME_STATUS.SERVICE_RUNNING);
                    if( isRunning != IsRunning )
                    {
                        IsRunning = isRunning;
                        NotifyPropertyChanged("IsRunning");
                    }
                }
            }
        }

        private SC_CONTROLS_ACCEPTED _ControlsAccepted;
        public SC_CONTROLS_ACCEPTED ControlsAccepted
        {
            get
            {
                return _ControlsAccepted;
            }
            private set
            {
                if (_ControlsAccepted != value)
                {
                    _ControlsAccepted = value;
                    NotifyPropertyChanged("ControlsAcceptedString");
                }
            }
        }

        private string _pid;
        public string PID
        {
            get
            {
                return _pid;
            }
            private set
            {
                string v = value;
                if (v.Equals("0"))
                    v = "";

                if (_pid == null)
                    _pid = "";

                if( !_pid.Equals(v))
                {
                    _pid = v;
                    NotifyPropertyChanged("PID");
                }
            }
        }

        public void UpdateFrom(ENUM_SERVICE_STATUS_PROCESS essp)
        {
            CurrentState = essp.CurrentState;
            ControlsAccepted = essp.ControlsAccepted;
            PID = essp.ProcessID.ToString();
        }

        public void UpdateFrom(SERVICE_STATUS_PROCESS ssp)
        {
            CurrentState = ssp.CurrentState;
            ControlsAccepted = ssp.ControlsAccepted;
            PID = ssp.ProcessID.ToString();
        }

        public ServiceDataObject(NativeService service, ENUM_SERVICE_STATUS_PROCESS essp)
            :   base(essp.ServiceName)
        {
            DisplayName = essp.DisplayName;
            CurrentState = essp.CurrentState;
            ControlsAccepted = essp.ControlsAccepted;

            Win32ExitCode = essp.Win32ExitCode.ToString();
            ServiceSpecificExitCode = essp.ServiceSpecificExitCode.ToString();
            CheckPoint = essp.CheckPoint.ToString();
            WaitHint = essp.WaitHint.ToString();
            ServiceFlags = essp.ServiceFlags.ToString();

            ServiceType = ServicesLocalisation.Localized(essp.ServiceType);
            if (essp.ProcessID != 0)
                PID = essp.ProcessID.ToString();
            else
                PID = "";
            QUERY_SERVICE_CONFIG config = service.ServiceConfig;

            if (essp.CurrentState == SC_RUNTIME_STATUS.SERVICE_RUNNING)
            {
                IsRunning = true;
            }

            if (config != null)
            {
                if (config.StartType == SC_START_TYPE.SERVICE_DISABLED)
                {
                    IsDisabled = true;
                }
                StartType = ServicesLocalisation.Localized(config.StartType);
                BinaryPathName = config.BinaryPathName;
                LoadOrderGroup = config.LoadOrderGroup;
                ErrorControl = ServicesLocalisation.Localized(config.ErrorControl);
                TagId = config.TagId.ToString();
                User = config.ServiceStartName;

            }
            ToolTip = Description = service.Description;
            ToolTipCaption = DisplayName;
        }
    }

}
