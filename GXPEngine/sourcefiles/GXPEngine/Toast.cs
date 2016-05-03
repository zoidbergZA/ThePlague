using System.Drawing;

namespace GXPEngine
{
    internal class Toast
    {
        private static readonly PointF displayPosition = new PointF(75, 0);
        private static ToastMessage currentMessage;
        private static readonly Color bgColor = Color.FromArgb(40, 0, 0, 0);

        /// <summary>
        ///     Create a new toast message to display for a set time
        /// </summary>
        /// <param name="message">The text string to display</param>
        /// <param name="displayTime">Time to display in miliseconds</param>
        public static void NewToast(string message, int displayTime = 4000, Image portrait = null)
        {
            var newMessage = new ToastMessage(message, displayTime, portrait);
            currentMessage = newMessage;
//            currentMessage.graphics.Clear(bgColor);
        }

        public static void ShowToast()
        {
            if (currentMessage != null)
            {
                if (currentMessage.enabled)
                    currentMessage.graphics.Clear(bgColor);

                if (currentMessage.portrait != null)
                {
                    var p = new PointF(0, 0);
                    if (currentMessage.enabled)
                        currentMessage.graphics.DrawImage(currentMessage.portrait, p);
                }

                if (currentMessage.enabled)
                    currentMessage.graphics.DrawString(currentMessage.message, currentMessage.font, currentMessage.brush,
                        displayPosition);
            }
        }

        public static void DestroyToastMessage(ToastMessage msg)
        {
//            if (msg == null)
//                return;

            if (msg.enabled)
                msg.graphics.Clear(Color.Empty);
            if (msg.portrait != null)
            {
//                msg.portrait.Dispose();
                msg.enabled = false;
            }
            msg.Destroy();
            currentMessage = null;
        }

        public class ToastMessage : Canvas
        {
            public Brush brush = new SolidBrush(Color.White);
            public bool enabled = true;
            public Font font = new Font("Garamond", 18, FontStyle.Bold);
            public string message;
            public Image portrait;
            private int timer;

            public ToastMessage(string message, int displayTime, Image portrait)
                : base(564, 75)
            {
                this.message = message;
                timer = displayTime;
                this.portrait = portrait;

                x = (game.width - width)/2;
                y = 10f;

                Game.main.AddChild(this);
            }

            public override void Destroy()
            {
                base.Destroy();
                enabled = false;
            }

            private void Update()
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                    DestroyToastMessage(this);
            }
        }
    }
}