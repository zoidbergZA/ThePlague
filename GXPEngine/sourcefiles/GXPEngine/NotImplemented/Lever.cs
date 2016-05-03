using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class Lever : LevelObject
    {
        private static string spritePath = "../Sprites/switch.png";
        private static ShapeType shapeType = ShapeType.Polygon;
        private static BodyType bodyType = BodyType.Static;

        private readonly LevelObject slave;
        private bool triggered;

        public Lever(World world, Vector2 spawnPosition, float spawnRotation, LevelObject slave)
            : base(spritePath, world, spawnPosition, spawnRotation, shapeType, bodyType)
        {
            this.slave = slave;

            //Subscribe to collisions
            foreach (Fixture fixture in fixtures)
            {
                fixture.OnCollision += MyOnCollision;
            }
        }

        public bool MyOnCollision(Fixture f1, Fixture f2, Contact contact)
        {
            //Console.WriteLine("collision fired!" + "F1: " + f1.UserData);

            if (f2.UserData is Player)
            {
                var player = (Player) f2.UserData;

                if (f2 == player.interactor && !triggered)
                {
                    Activate();
                }
            }

            //collider return true, trigger return false
            return false;
        }

        private void Activate()
        {
            triggered = true;
            Mirror(true, false);

            slave.Interact();
        }
    }
}