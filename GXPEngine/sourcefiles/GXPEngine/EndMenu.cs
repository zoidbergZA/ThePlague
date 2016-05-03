using System;
using System.Drawing;

namespace GXPEngine
{
    public class EndMenu : Sprite
    {
        private readonly Brush brush = new SolidBrush(Color.White);
        private readonly Canvas canvas;
        private readonly Font font = new Font("Consolas", 20, FontStyle.Regular);
        private Sprite gameOver;
        private string message;

        public EndMenu(string filename, string reason) : base(".../sprites/endScreen.png")
        {
            gameOver = new Sprite(".../sprites/gameOver" + filename + ".png");
            canvas = new Canvas(500, 150);
            canvas.x = (game.width - canvas.width)/2;
            canvas.y = 450;
            message = reason + "\n\nPress z to go back to \nthe main menu.";
            canvas.graphics.DrawString(message, font, brush, 0, 0);
            Console.WriteLine(reason);
//			game.Add (this);
            game.AddChild(this);
            game.Add(canvas);
            //this.AddChild (canvas);
            gameOver.x = (game.width - gameOver.width)/2;
            gameOver.y = (game.height - gameOver.height)/2;
            AddChild(gameOver);
        }


        public void Update()
        {
            if (Input.GetKeyDown(Key.Z))
            {
                game.Remove(this);
                canvas.Destroy();
                Destroy();
                var mainmenu = new MainMenu();
            }
        }
    }
}