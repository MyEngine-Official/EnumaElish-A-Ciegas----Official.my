using System.Diagnostics;
using _prueba.gameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary3D.Graphics;
using prueba.gameContent;
using prueba3d.gameContent;

namespace _3d_prueba
{
    public class FuelCellGame : Game
    {
        private GraphicsDeviceManager graphics;
        private GameObject3D ground;
        private Camera3D gameCamera;
        // Game objects
        private FuelCarrier fuelCarrier;
        private FuelCell[] fuelCells;
        private Barrier[] barriers;
        // States to store input values
        private KeyboardState lastKeyboardState = new KeyboardState();
        private KeyboardState currentKeyboardState = new KeyboardState();
        private GamePadState lastGamePadState = new GamePadState();
        private GamePadState currentGamePadState = new GamePadState();
        public FuelCellGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            ground = new GameObject3D();
            gameCamera = new Camera3D();
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            ground.Model = Content.Load<Model>("Models/ground");

            // Initialize and place fuel cell
            fuelCells = new FuelCell[1];
            fuelCells[0] = new FuelCell();
            fuelCells[0].LoadContent(Content, "Models/Sin_nombre2");
            fuelCells[0].Position = new Vector3(0, 0, 50);


            // Initialize and place barriers
            barriers = new Barrier[3];

            barriers[0] = new Barrier();
            barriers[0].LoadContent(Content, "Models/cube10uR");
            barriers[0].Position = new Vector3(0, 0, 30);
            barriers[1] = new Barrier();
            barriers[1].LoadContent(Content, "Models/cylinder10uR");
            barriers[1].Position = new Vector3(15, 0, 30);
            barriers[2] = new Barrier();
            barriers[2].LoadContent(Content, "Models/pyramid10uR");
            barriers[2].Position = new Vector3(-15, 0, 30);

            // Initialize and place fuel carrier
            fuelCarrier = new FuelCarrier();
            fuelCarrier.LoadContent(Content, "Models/KSR-29 sniper rifle old");
            fuelCarrier.Position = new Vector3(0, 10, 0);

            // TODO: use this.Content to load your game content here
            foreach (var mesh in ground.Model.Meshes)
            {

                // muestra el nombre del mesh
                Debug.WriteLine($"Mesh: {mesh.Name}");
                // muestra la traslación del ParentBone (dónde está colocado por el export)
                Matrix m = mesh.ParentBone.Transform;
                Debug.WriteLine($" ParentBone translation: {m.Translation}");
                // si necesita, obtener BoundingSphere aproximado:
                try
                {
                    var bs = mesh.BoundingSphere;
                    Debug.WriteLine($" BoundingSphere center: {bs.Center}, radius: {bs.Radius}");
                }
                catch { /* algunos runtimes no exponen BoundingSphere */ }
            }
            foreach (var mesh in fuelCarrier.Model.Meshes)
            {
                Debug.WriteLine($"Mesh rifle: {mesh.Name}");
                Debug.WriteLine($"  Bone: {mesh.ParentBone.Transform.Translation}");
                Debug.WriteLine($"  Radius: {mesh.BoundingSphere.Radius}");
            }


        }

        float x = 0;
        float y = 0;
        float z = 0;
        float vel = 1;
        float rotation = 0.0f;

        protected override void Update(GameTime gameTime)
        {
            // Update input from sources, Keyboard and GamePad
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            lastGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                x -= vel;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                z += vel;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                z -= vel;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                x += vel;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                y += vel;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                y -= vel;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                rotation += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                rotation -= 0.1f;
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();



            fuelCells[0].Position = new Vector3(0, 0, 0);
            fuelCarrier.Position = new Vector3(x, y + 10, z);
            fuelCarrier.Update(currentGamePadState, currentKeyboardState, barriers);
            gameCamera.Update(fuelCarrier.ForwardDirection, fuelCarrier.Position, graphics.GraphicsDevice.Viewport.AspectRatio);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
                
            DrawTerrain(ground.Model);
            // Draw the fuel cell
            // Draw the fuel cell
            fuelCells[0].Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);

            // Draw the barriers
            foreach (Barrier barrier in barriers)
            {
                barrier.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);
            }

            // Draw the fuel carrier
            fuelCarrier.Draw(gameCamera.ViewMatrix, gameCamera.ProjectionMatrix);

            base.Draw(gameTime);
        }

        private void DrawTerrain(Model model)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {   

                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = Matrix.Identity;

                    // Use the matrices provided by the game camera
                    effect.View = gameCamera.ViewMatrix;
                    effect.Projection = gameCamera.ProjectionMatrix;
                }
                mesh.Draw();
            }
        }
    }
}
