namespace GXPEngine
{
    internal class SpecialEffect : AnimSprite
    {
        private readonly float animationSpeed;
        private float cFrame;

        public SpecialEffect(string spritePath, int cols, int rows, float spawnX, float spawnY, float speed = 1f)
            : base(spritePath, cols, rows)
        {
            animationSpeed = speed;

            GameManager.Instance.Level.AddChild(this);
            game.Add(this);
            SetOrigin(width/2, height/2);

            x = spawnX;
            y = spawnY;
        }

        private void Update()
        {
            SetFrame((int) cFrame);
            NextFrame();

            cFrame += animationSpeed;

            if (currentFrame == 0)
                Destroy();
        }
    }
}