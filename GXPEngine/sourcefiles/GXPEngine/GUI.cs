using System.Drawing;

namespace GXPEngine
{
    internal class GUI : Canvas
    {
        private readonly Brush brush;
        private readonly Font font;
        private readonly PointF position;

        public GUI() : base(800, 80)
        {
            font = new Font("Arial", 20, FontStyle.Regular);
            brush = new SolidBrush(Color.Green);
            position = new PointF(128, 0);
        }

        public void DrawGUI()
        {
            // Show Toast messages
            Toast.ShowToast();

//            String guiString = "";
//
//            if (GameManager.Instance.Player != null)
//            {
//                guiString += "breath: " + GameManager.Instance.Player.Breath;
//            }
//
//            graphics.Clear(Color.Empty);
//            graphics.DrawString(guiString, font, brush, position);
        }
    }
}