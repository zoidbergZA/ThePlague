using System.Collections.Generic;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    public abstract class PhysicalSprite : AnimSprite
    {
        public Body body;
        public List<Fixture> fixtures;
        public Fixture interactor;

        public PhysicalSprite(string spritePath, Vector2 spawnPosition, int cols = 1, int rows = 1)
            : base(spritePath, cols, rows)
        {
            fixtures = new List<Fixture>();

            x = spawnPosition.X;
            y = spawnPosition.Y;

            SetOrigin(width/2, height/2);

            game.Add(this);
        }

        private void Update()
        {
            if (body == null)
                return;

            SyncTransforms();
        }

        public void SyncTransforms()
        {
            if (body == null)
                return;

            var spritePos = new Vector2(x, y);
            Vector2 PhysPos = ConvertUnits.ToDisplayUnits(body.Position);
            float syncRot = body.Rotation;

            Vector2 newPos = MyUtils.Lerp(spritePos, PhysPos, 0.9f);

            x = newPos.X;
            y = newPos.Y;
            rotation = (float) MyUtils.RadianToDegree(syncRot); // TODO: fix rotation bug
        }

        public override void Destroy()
        {
            //destroy sprite
            base.Destroy();
            game.Remove(this);
            //destroy physics object
            if (body != null)
                body.Dispose();

            body = null;
            fixtures = null;
            interactor = null;
        }
    }
}