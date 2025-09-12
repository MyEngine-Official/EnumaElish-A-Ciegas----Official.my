using System;
using MyEngineDemoGameImplementation;

public static class Program
{
    [STAThread]
    static void Main()
    {
        using (var game = new MyGame1())
        {
            game.Run();
        }
    }
}