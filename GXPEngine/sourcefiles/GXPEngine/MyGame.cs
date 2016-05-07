using GXPEngine;

public class MyGame : Game
{
    public static int ycount = 0;

    public MyGame() : base(1280, 960, false) // resolution, fullscreen?
    {
        var mainMenu = new MainMenu();
    }

    private void Update()
    {
    }

    private static void Main()
    {
        new MyGame().Start();
    }
}