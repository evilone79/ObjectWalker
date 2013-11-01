using System;
using System.Collections.Generic;

namespace WalkerTester
{
    public class Entity
    {
        public Entity()
        {
            Dict = new Dictionary<int, string>();
        }
        public string AnotherStuff { get; set; }
        public DateTime Date { get; set; }
        public bool Result { get; set; }
        public Dictionary<int, string> Dict { get; set; }
        public Dictionary<int, List<string>> DictComplex { get; set; }
    }
    public class Configuration
    {
        public LinkedList<string> LS { get; set; } 
        public HashSet<Entity> HS { get; set; } 
        public Dictionary<int, Entity> DictEnt { get; set; }
        public Entity TheEntity { get; set; }

        protected string _url = string.Empty;
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
            }
        }
        protected string _fileHash = string.Empty;
        public string FileHash
        {
            get { return _fileHash; }
            set { _fileHash = value; }
        }

        protected bool _isMsi = false;
        public bool IsMsi
        {
            get { return _isMsi; }
            set { _isMsi = value; }
        }

        protected bool _uacRequired = false;
        public bool UacRequired
        {
            get { return _uacRequired; }
            set { _uacRequired = value; }
        }

        protected bool _silent = false;
        public bool Silent
        {
            get { return _silent; }
            set { _silent = value; }
        }

        protected string _paramSource = string.Empty;
        public string ParamSource
        {
            get { return _paramSource; }
            set { _paramSource = value; }
        }

        protected string _exitValueLocation = string.Empty;
        public string ExitValueLocation
        {
            get { return _exitValueLocation; }
            set { _exitValueLocation = value; }
        }

        protected string _onInstallFail = string.Empty;
        public string OnInstallFail
        {
            get { return _onInstallFail; }
            set { _onInstallFail = value; }
        }

        protected bool _delayedBilling = false;
        public bool DelayedBilling
        {
            get { return _delayedBilling; }
            set { _delayedBilling = value; }
        }

        protected List<Entity> _defaultInstallerParams = null;
        public List<Entity> DefaultInstallerParams
        {
            get { return _defaultInstallerParams; }
            set { _defaultInstallerParams = value; }
        }

        public List<string> Listing { get; set; }

        protected string _preRequisites = string.Empty;
        public string PreRequisites
        {
            get { return _preRequisites; }
            set { _preRequisites = value; }
        }
    }
}
