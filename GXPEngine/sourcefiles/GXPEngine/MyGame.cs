using GXPEngine;

public class MyGame : Game
{
    public static int ycount = 0;

    public MyGame() : base(1280, 960, true)
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