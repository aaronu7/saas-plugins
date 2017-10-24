using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saas_plugins.SaaS
{
    public class Plugin
    {
        private string _name = "";
        private string _description = "";        
        private string _dllFileDir = "";
        private string _dllFileName = "";
        private List<string> _dllFileNameReferenceSet = null;
        private string _classNamespacePath = "";
        private string[] _code = null;

        // MetaData (ex. Function calls and parameters) 

        public Plugin(string name, string description, 
            string dllFileDir, string dllFileName, List<string> dllFileNameReferenceSet, 
            string classNamespacePath, string[] code) {

            this._name = name;
            this._description = description;
            this._dllFileDir = dllFileDir;
            this._dllFileName = dllFileName;
            this._dllFileNameReferenceSet = dllFileNameReferenceSet;
            this._classNamespacePath = classNamespacePath;
            this._code = code;
        }

        public override string ToString() {return this.Name;}

        #region " Properties "

        public string DllFileDir {
            get {return this._dllFileDir;}
            set {this._dllFileDir = value;}
        }
        public string DllFileName {
            get {return this._dllFileName;}
            set {this._dllFileName = value;}
        }
        public List<string> DllFileNameReferenceSet {
            get {return this._dllFileNameReferenceSet;}
            set {this._dllFileNameReferenceSet = value;}
        }

        public string Name {
            get {return this._name;}
            set {this._name = value;}
        }

        public string Description {
            get {return this._description;}
            set {this._description = value;}
        }

        public string ClassNamespacePath {
            get {return this._classNamespacePath;}
            set {this._classNamespacePath = value;}
        }

        public string[] Code {
            get {return this._code;}
            set {this._code = value;}
        }
        #endregion
    }
}
