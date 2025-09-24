using System;
using MyEngine;
using MyEngine.MyScenes;

namespace MyGame
{
    /// <summary>
    /// Example of how to properly use the MyEngine with integrated scene management
    /// </summary>
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            // Example 1: Basic usage with default scene
            RunBasicExample();

            // Example 2: Custom initial scene
            // RunCustomSceneExample();

            // Example 3: Multiple scenes with transitions
            // RunMultipleSceneExample();
        }

        /// <summary>
        /// Basic example - uses default main menu scene
        /// </summary>
        private static void RunBasicExample()
        {
            using (var game = new MyProgram(
                title: "My Awesome Game",
                width: 1280,
                height: 720,
                fullScreen: false,
                allowUserResizing: true))
            {
                game.Run();
            }
        }

        /// <summary>
        /// Example with custom initial scene
        /// </summary>
        private static void RunCustomSceneExample()
        {
            // Create your custom initial scene
            var customScene = new ExampleGamePlayScene();

            using (var game = new MyProgram(
                title: "Direct to Gameplay",
                width: 1280,
                height: 720,
                fullScreen: false,
                allowUserResizing: false,
                initialScene: customScene))
            {
                game.Run();
            }
        }

        /// <summary>
        /// Example showing how to set up multiple scenes
        /// </summary>
        private static void RunMultipleSceneExample()
        {
            using (var game = new MyProgram(
                title: "Multi-Scene Game",
                width: 1920,
                height: 1080,
                fullScreen: false))
            {
                // The game will start with the default main menu
                // You can register additional scenes after initialization
                // by accessing game.SceneManager from custom code
                
                game.Run();
            }
        }
    }

    /// <summary>
    /// Example of a custom game class that extends functionality
    /// </summary>
    public class CustomGame : MyProgram
    {
        public CustomGame() : base(
            title: "Extended Game",
            width: 1280,
            height: 720,
            fullScreen: false,
            allowUserResizing: true,
            initialScene: new CustomMainMenuScene())
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            // Add custom initialization here
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            // Register additional scenes
            SceneManager.RegisterScene("Gameplay", new ExampleGamePlayScene());
            SceneManager.RegisterScene("Settings", new SettingsScene());
            SceneManager.RegisterScene("Credits", new CreditsScene());
        }
    }

    // Placeholder scenes for example
    public class CustomMainMenuScene : Scene
    {
        public CustomMainMenuScene() : base("CustomMainMenu") 
        { 
            BackgroundColor = Microsoft.Xna.Framework.Color.DarkBlue;
        }

        public override void Initialize() 
        {
            // Initialize your custom menu
            Console.WriteLine("Custom Main Menu Initialized");
        }

        public override void LoadContent() 
        {
            // Load menu content
        }
    }

    public class SettingsScene : Scene
    {
        public SettingsScene() : base("Settings") { }
        public override void Initialize() { }
        public override void LoadContent() { }
    }

    public class CreditsScene : Scene
    {
        public CreditsScene() : base("Credits") { }
        public override void Initialize() { }
        public override void LoadContent() { }
    }
}
