using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEngineAPI
{
    public class GlobalConfig
    {
        private string _editorXMLPath;

        public GlobalConfig(string editorXMLPath)
        {
            _editorXMLPath = editorXMLPath;
        }
    }
}
