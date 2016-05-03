using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class Booster : LevelObject
    {
        private static ShapeType shapeType = ShapeType.Polygon;
        private static BodyType bodyType = BodyType.Dynamic;

        private Sound pickupSound = new Sound("../Sounds/interact_1.wav");

        public Booster(string spritePath, World world, Vector2 spawnPosition, float spawnRotation)
            : base(spritePath, world, spawnPosition, spawnRotation, shapeType, bodyType)
        {
            body.Restitution = 0.35f;

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
                //Console.WriteLine("booster picked up!");
                var player = (Player) f2.UserData;

                if (f2 == player.interactor)
                {
//                    GameManager.PlayRandomSound();
                    Destroy();
                }
                return false;
            }

            //collider return true, trigger return false
            return true;
        }
    }
}