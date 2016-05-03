using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using GXPEngine.Core;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GXPEngine
{
    public class FogBox : LevelObject
    {
        private readonly List<Fog> fogList = new List<Fog>();
        private Rectangle bounds;
        private bool debugMode = false;

        public FogBox(string spritePath, World world, Vector2 spawnPosition, float spawnRotation)
            : base(spritePath, world, spawnPosition, spawnRotation, ShapeType.Polygon, BodyType.Static)
        {
            body.IsSensor = true;

            bounds = new Rectangle(-width/2, -height/2, width, height);

            for (int i = 0; i < 40; i++)
            {
                var fog = new Fog(bounds);
                fogList.Add(fog);
                AddChild(fog);
            }

            //Subscribe to collisions
            foreach (Fixture fixture in fixtures)
            {
                fixture.OnCollision += MyOnCollision;
                fixture.OnSeparation += MyOnCollissionEnd;
            }
        }

        public override void Update()
        {
            SyncTransforms();

            Core.Vector2[] bounds = GetExtents();
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float minX = float.MaxValue;
            float minY = float.MaxValue;

            for (int i = 0; i < 4; i++)
            {
                if (bounds[i].x > maxX) maxX = bounds[i].x;
                if (bounds[i].x < minX) minX = bounds[i].x;
                if (bounds[i].y > maxY) maxY = bounds[i].y;
                if (bounds[i].y < minY) minY = bounds[i].y;
            }
            bool test = (maxX < 0) || (maxY < 0) || (minX >= game.width) || (minY >= game.height);
            if (test == false)
            {
                for (int i = 0; i < fogList.Count; i++)
                {
                    fogList[i].Step();
                }
            }
        }


        public bool MyOnCollision(Fixture f1, Fixture f2, Contact contact)
        {
            if (f2.UserData is Player)
            {
                var player = (Player) f2.UserData;

                if (f2 == player.interactor)
                {
//                    Console.WriteLine("player entered gas zone!");
                    player.FogBoxes.Add(this);
                }
            }

            if (f2.UserData is Victim)
            {
                var victim = (Victim) f2.UserData;

                if (f2 == victim.interactor)
                {
//                    Console.WriteLine("victim entered gas zone!");
                    victim.FogBoxes.Add(this);
                }
            }

            //collider return true, trigger return false
            return true;
        }

        public void MyOnCollissionEnd(Fixture f1, Fixture f2)
        {
            if (f2.UserData is Player)
            {
                var player = (Player) f2.UserData;

                if (f2 == player.interactor)
                {
//                    Console.WriteLine("player exited gas zone");
                    player.FogBoxes.Remove(this);
                }
            }

            if (f2.UserData is Victim)
            {
                var victim = (Victim) f2.UserData;

                if (f2 == victim.interactor)
                {
//                    Console.WriteLine("victim exited gas zone");
                    victim.FogBoxes.Remove(this);
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            game.Remove(this);
        }

        protected override void RenderSelf(GLContext glContext)
        {
            if (debugMode)
                base.RenderSelf(glContext);
        }
    }
}