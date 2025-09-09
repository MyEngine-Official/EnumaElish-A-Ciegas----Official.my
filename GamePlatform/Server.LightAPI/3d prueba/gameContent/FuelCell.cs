using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary3D.Graphics;

namespace prueba3d.gameContent
{
    public class FuelCell : GameObject3D
{
    public bool Retrieved { get; set; }

    public FuelCell()
        : base()
    {
        Retrieved = false;
    }


        public void Draw(Matrix view, Matrix projection)
        {
            Matrix translateMatrix = Matrix.CreateTranslation(Position);
            Matrix worldMatrix = translateMatrix;

            if (!Retrieved)
            {
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = worldMatrix;
                        effect.View = view;
                        effect.Projection = projection;

                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                    }
                    mesh.Draw();
                }
            }
        }
        public void LoadContent(ContentManager content, string modelName)
    {
        Model = content.Load<Model>(modelName);
        Position = Vector3.Down;
    }
}
}
