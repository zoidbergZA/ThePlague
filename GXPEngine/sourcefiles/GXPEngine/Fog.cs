using System;
using Glide;
using GXPEngine.Core;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GXPEngine
{
    internal class Fog : Sprite
    {
        private readonly float rotationRate;
        private readonly float transparency = Utils.Random(0.11f, 0.54f); // 0.11, 0.34
        private Rectangle bounds;
        private Vector2 newPosition;
        private bool relocating;
        private Tweener tweener;

        public Fog(Rectangle bounds) : base("../Sprites/smoke.png")
        {
            this.bounds = bounds;

            SetOrigin(width/2, height/2);
            SetXY(
                Utils.Random(bounds.x, bounds.x + bounds.width),
                Utils.Random(bounds.y, bounds.y + bounds.height)
                );
            rotation = Utils.Random(0, 360);
            rotationRate = Utils.Random(-0.25f, 0.25f);
            alpha = transparency;
            float randC = Utils.Random(0.7f, 0.85f);
            SetColor(randC, 1f, randC); // 0.55, 1, 0.55
        }

        public void Step()
        {
            if (tweener != null)
                tweener.Update(Time.deltaTime);

            x += (float) (Math.Sin(y*0.1f)*scaleX*0.3f);
            y -= (float) scaleY*0.12f;

            rotation += rotationRate;

            if (!relocating)
            {
                if (y < bounds.y)
                {
                    relocating = true;
                    newPosition = new Vector2(x, bounds.y + bounds.height);
                    FadeOut();
                }
            }
        }

        private void FadeOut()
        {
            tweener = new Tweener();
            tweener.Tween(this, new {alpha = 0}, 1000f)
                .Ease(Ease.SineOut)
                .OnComplete(FadeIn);
        }

        private void FadeIn()
        {
            SetXY(newPosition.X, newPosition.Y);
            relocating = false;

            tweener = new Tweener();
            tweener.Tween(this, new {alpha = transparency}, 1000f)
                .Ease(Ease.SineOut);
        }
    }
}