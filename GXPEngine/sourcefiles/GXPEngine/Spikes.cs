using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class Spikes : LevelObject
    {
        public Spikes(string spritePath, ShapeType shapeType, BodyType bodyType, World world, Vector2 spawnPosition,
            float spawnRotation = 0f)
            : base(spritePath, world, spawnPosition, spawnRotation, shapeType, bodyType)
        {
            //Subscribe to collisions
            foreach (Fixture fixture in fixtures)
            {
                fixture.OnCollision += MyOnCollision;
            }
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