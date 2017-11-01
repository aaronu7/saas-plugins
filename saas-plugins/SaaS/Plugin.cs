/****************************** Plugin ******************************\
This class provides the entity model of a "Plugin".

Copyright (c) Aaron Ulrich
This source is subject to the Apache License Version 2.0, January 2004
See http://www.apache.org/licenses/.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;

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

        private Int32 _compileOrder = 0;

        private bool _isCompiled = false;

        // MetaData (ex. Function calls and parameters) 

        public Plugin(string name, string description, 
            string dllFileDir, string dllFileName, List<string> dllFileNameReferenceSet, 
            string classNamespacePath, string[] code, Int32 compileOrder) {

            this._name = name;
            this._description = description;
            this._dllFileDir = dllFileDir;
            this._dllFileName = dllFileName;
            this._dllFileNameReferenceSet = dllFileNameReferenceSet;
            this._classNamespacePath = classNamespacePath;
            this._code = code;
            this._compileOrder = compileOrder;
        }

        public override string ToString() {return this.PluginID;}
        
        #region " Properties "
        
        /// <summary>
        /// An identifier used to uniquely ID this plugin.
        /// </summary>
        public string PluginID {get {return this._dllFileName; }}

        /// <summary>
        /// Get/Set in where this compilation should occur in a sequence of plugins
        /// </summary>
        public Int32 CompileOrder {
            get {return this._compileOrder;}
            set {this._compileOrder = value;}
        }        

        /// <summary>
        /// Get/Set if the current plugin code matches the existing DLL. If false, the DLL should be recompiled.
        /// </summary>
        public bool IsCompiled {
            get {return this._isCompiled;}
            set {this._isCompiled = value;}
        }

        /// <summary>
        /// The full file path to the plugins DLL.
        /// </summary>
        public string DllFilePath {
            get {return  this._dllFileDir + this._dllFileName;}
        }

        /// <summary>
        /// The directory path to the plugins DLL.
        /// </summary>
        public string DllFileDir {
            get {return this._dllFileDir;}
            set {this._dllFileDir = value;}
        }

        /// <summary>
        /// The file name for this DLL.
        /// </summary>
        public string DllFileName {
            get {return this._dllFileName;}
            set {this._dllFileName = value;}
        }
        
        /// <summary>
        /// A set of references to add when compiling this DLL.
        /// </summary>
        public List<string> DllFileNameReferenceSet {
            get {return this._dllFileNameReferenceSet;}
            set {this._dllFileNameReferenceSet = value;}
        }

        /// <summary>
        /// The friendly name for this plugin.
        /// </summary>
        public string Name {
            get {return this._name;}
            set {this._name = value;}
        }

        /// <summary>
        /// A description of this plugin.
        /// </summary>
        public string Description {
            get {return this._description;}
            set {this._description = value;}
        }

        /// <summary>
        /// A classnamespace path for this plugin.
        /// </summary>
        public string ClassNamespacePath {
            get {return this._classNamespacePath;}
            set {this._classNamespacePath = value;}
        }

        /// <summary>
        /// A set of source code files used in compiling this plugin.
        /// </summary>
        public string[] Code {
            get {return this._code;}
            set {this._code = value;}
        }
        #endregion
    }
}
