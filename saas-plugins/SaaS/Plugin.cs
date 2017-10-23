using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saas_plugins.SaaS
{
    public class Plugin
    {
        //RunExpression("ad2csv.dll", "ad2csv.SaaS.CompilerRunner", "MyDomain", "code goes here", "ad2csv.SaaS.CompilerRunner.CSCodeEvaler", "EvalCode", new object[0]);

        private string _instanceDomainName = "";
        private string _compilerRunnerNamespace = "";
        
        private string _dllFileDir = "";
        private string _dllFileName = "";
        private List<string> _dllFileNameReferenceSet = null;
        private string _classNamespacePath = "";
        private string[] _code = null;

        //private string functionCall
        //private object[] functionArgs

        public Plugin(string dllFileDir, string dllFileName, List<string> dllFileNameReferenceSet, 
            string compilerRunnerNamespace, string instanceDomainName, 
            string classNamespacePath, string[] code) {

            this._dllFileDir = dllFileDir;
            this._dllFileName = dllFileName;
            this._dllFileNameReferenceSet = dllFileNameReferenceSet;
            this._compilerRunnerNamespace = compilerRunnerNamespace;
            this._instanceDomainName = instanceDomainName;
            this._classNamespacePath = classNamespacePath;
            this._code = code;
        }

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

        public string CompilerRunnerNamespace {
            get {return this._compilerRunnerNamespace;}
            set {this._compilerRunnerNamespace = value;}
        }

        public string InstanceDomainName {
            get {return this._instanceDomainName;}
            set {this._instanceDomainName = value;}
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
