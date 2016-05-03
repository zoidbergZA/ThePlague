using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class CameraController : GameObject
    {
        private readonly Level level;
        public GameObject target;

        public CameraController(Level level, GameObject cameraTarget)
        {
            this.level = level;
            target = cameraTarget;

            level.AddChild(this);
            game.Add(this);
        }

        private void Update()
        {
            float yOffset = 0f;

            if (Input.GetKey(Key.DOWN))
                yOffset = -330;
            if (Input.GetKey(Key.UP))
                yOffset = 300;

            var oldPos = new Vector2(level.x, level.y);
            var targetpos = new Vector2(game.width/2 - target.x, game.height/2 - target.y);
            targetpos.Y += yOffset;

            Vector2 newPos = MyUtils.Lerp(oldPos, targetpos, 0.3f);

            level.x = newPos.X;
            level.y = newPos.Y;

            //level.x = game.width / 2 -target.x;
            //level.y = game.height / 2 - target.y;

            if (GameManager.Instance.Level == null)
            {
                game.Remove(this);
                Destroy();
            }
        }
    }
}