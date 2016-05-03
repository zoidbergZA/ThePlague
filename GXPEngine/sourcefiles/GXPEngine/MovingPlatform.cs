using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Glide;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class MovingPlatform : LevelObject
    {
        private readonly float rangeDown;
        private readonly float rangeRight;
        private readonly Tweener tweener;
        private readonly float xMax;
        private readonly float xMin;
        private readonly float xPos;
        private readonly float yMax;
        private readonly float yMin;
        private readonly float yPos;
        private bool spiked;

        public MovingPlatform(string spritePath, Prefabs.MovementRange range, ShapeType shapeType, World world,
            Vector2 spawnPosition,
            float spawnRotation = 0f, bool spiked = false)
            : base(spritePath, world, spawnPosition, spawnRotation, shapeType, BodyType.Kinematic)
        {
            this.spiked = spiked;
            tweener = new Tweener();
            int tileSize = GameManager.Instance.Level.TileSize;

            yPos = body.Position.Y;
            xPos = body.Position.X;

            rangeDown = range.down;
            range.down = ConvertUnits.ToSimUnits(range.down*tileSize);
            rangeRight = range.right;
            range.right = ConvertUnits.ToSimUnits(range.right*tileSize);

            yMax = body.Position.Y;
            yMin = body.Position.Y + range.down;
            xMax = body.Position.X;
            xMin = body.Position.X + range.right;

            if (spiked)
            {
                //Subscribe to collisions
                foreach (Fixture fixture in fixtures)
                {
                    fixture.OnCollision += MyOnCollision;
                }
            }

//            Console.WriteLine("bodyPos = {2} yMin = {0} yMax = {1}", yMin, yMax, body.Position.Y);
            TweenToMin();
        }

        private void TweenToMin()
        {
            tweener.Tween(this, new {yPos = yMin, xPos = xMin}, 6000f, 5000f)
                .Ease(Ease.SineInOut).OnComplete(TweenToMax);
        }

        private void TweenToMax()
        {
            tweener.Tween(this, new {yPos = yMax, xPos = xMax}, 6000f, 5000f)
                .Ease(Ease.SineInOut).OnComplete(TweenToMin);
        }

        private void Update()
        {
            tweener.Update(Time.deltaTime);

            if (body == null)
            {
                game.Remove(this);
                return;
            }

//            body.SetTransform(new Vector2(body.Position.X, yPos), 0);
            if (rangeDown > 0)
                body.LinearVelocity = new Vector2(0, yPos - body.Position.Y);
            if (rangeRight > 0)
                body.LinearVelocity = new Vector2(xPos - body.Position.X, 0);

            SyncTransforms();
//            Console.WriteLine(yPos - body.Position.Y);
        }

        public bool MyOnCollision(Fixture f1, Fixture f2, Contact contact)
        {
            if (f2.UserData is Character && !f2.IsSensor)
            {
                var character = (Character) f2.UserData;

                character.TakeDamage(1000);
            }

            //collider return true, trigger return false
            return true;
        }
    }
}