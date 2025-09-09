using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLibrary.ServerSideRendering
{
    public class PlayerPositionDto
    {
        public string PlayerId { get; set; } = "";
        public float X { get; set; }
        public float Y { get; set; }
    }

}
