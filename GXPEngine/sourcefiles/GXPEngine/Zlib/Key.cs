using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;

namespace GXPEngine.Zlib
{
    public enum KeyColor
    {
        Blue,
        Red,
        Green,
        Yellow
    }

    public class Key : LevelObject
    {
//        private static string spritePath = "../Sprites/key1.png";
        private static ShapeType shapeType = ShapeType.Polygon;
        private static BodyType bodyType = BodyType.Dynamic;
        private readonly KeyColor keyColor;

        public Key(string spritePath, World world, Vector2 spawnPosition, float spawnRotation, KeyColor keyColor)
            : base(spritePath, world, spawnPosition, spawnRotation, shapeType, bodyType)
        {
            this.keyColor = keyColor;

            //Subscribe to collisions
            foreach (Fixture fixture in fixtures)
            {
                fixture.OnCollision += MyOnCollision;
            }
        }

        public KeyColor KeyColor
        {
            get { return keyColor; }
        }

        public bool MyOnCollision(Fixture f1, Fixture f2, Contact contact)
        {
            //Console.WriteLine("collision fired!" + "F1: " + f1.UserData);
            //f1.Body.ContactList.

            if (f2.UserData != null && f2.UserData.GetType() == typeof (Player))
            {
                //Console.WriteLine("key picked up!");
                var player = (Player) f2.UserData;

                if (f2 == player.interactor)
                {
                    //add pickup logic here

                    Destroy();
                }
                return false;
            }

            //collider return true, trigger return false
            return true;
        }

        private void SpawnItem()
        {
            var bomb = new Bomb(GameManager.Instance.World, new Vector2(400f, -690f), 0);
            GameManager.Instance.Level.AddChild(bomb);
        }
    }
}