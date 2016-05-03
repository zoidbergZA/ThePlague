using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using GXPEngine.Zlib;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class Door : LevelObject
    {
        private static string spritesheet = "../Sprites/door_sheet.png";
        private static ShapeType shapeType = ShapeType.Polygon;
        private static BodyType bodyType = BodyType.Static;

        private readonly bool locked;
        private KeyColor keyColor;

        public Door(World world, Vector2 spawnPosition, float spawnRotation, KeyColor keyColor)
            : base(spritesheet, world, spawnPosition, spawnRotation, ShapeType.Polygon, BodyType.Static, 2, 1)
        {
            this.keyColor = keyColor;
            locked = true;

            //Subscribe to collisions
            foreach (Fixture fixture in fixtures)
            {
                fixture.OnCollision += MyOnCollision;
            }
        }

        public bool MyOnCollision(Fixture f1, Fixture f2, Contact contact)
        {
            if (!locked)
                return false;

            if (f2.UserData is Player)
            {
                var player = (Player) f2.UserData;

                if (f2 == player.interactor)
                {
                    CheckUnlock(player);
                }

                if (!locked)
                    return false;

                return true;
            }

            //collider return true, trigger return false
            return true;
        }

        private void CheckUnlock(Player player)
        {
//            foreach (Player.CollectedKey cKey in player.collectedKeys)
//            {
//                if (cKey.color == keyColor)
//                {
//                    player.collectedKeys.Remove(cKey);
//                    locked = false;
//                    SetFrame(1);
//                    GameManager.PlayRandomSound();
//                    return;
//                }
//            }
        }
    }
}