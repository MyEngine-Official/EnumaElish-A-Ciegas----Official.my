using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEngine.MyCore.MyComponents
{
    public class ScriptComponent
    {
        public string FilePath { get; set; }
        public ScriptLanguaje Languaje { get; set; }
    }

    public enum ScriptLanguaje
    {
        CSharp,
        FSharp,
        Python,
        JavaScript,
        Lua,
        Ruby
    }
}
