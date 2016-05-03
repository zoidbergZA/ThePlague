using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class Bomb : LevelObject
    {
        private static string spritesheet = "../Sprites/tnt_sheet.png";
        private static ShapeType shapeType = ShapeType.Polygon;
        private static BodyType bodyType = BodyType.Dynamic;

        private readonly Sound activateSound = new Sound("../Sounds/beep_1.wav");
        private readonly Sound explodeSound = new Sound("../Sounds/firework_explosion.wav");
        private int fuseTimer = 3000;

        public Bomb(World world, Vector2 spawnPosition, float spawnRotation)
            : base(spritesheet, world, spawnPosition, spawnRotation, shapeType, bodyType, 2, 1)
        {
            game.Add(this);

            //Subscribe to collisions
            foreach (Fixture fixture in fixtures)
            {
                fixture.OnCollision += MyOnCollision;
            }

            body.Mass *= 5;
            body.Restitution = 0.25f;
        }

        public bool Activated { get; protected set; }
        public bool Detonated { get; protected set; }
        public int MyTimer { get; protected set; }

        public bool MyOnCollision(Fixture f1, Fixture f2, Contact contact)
        {
            //Console.WriteLine("collision fired!" + "F1: " + f1.UserData);

            if (f2.UserData is Player)
            {
                //Console.WriteLine("booster picked up!");
                var player = (Player) f2.UserData;

                if (f2 == player.interactor && !Activated)
                {
                    Activate();
                }
            }

            if (f2.UserData is Rat)
            {
                if (Activated)
                    Deactivate();
            }

            //collider return true, trigger return false
            return true;
        }

        private void Activate()
        {
            Activated = true;
            MyTimer = fuseTimer;
            activateSound.Play();
            SetFrame(1);
        }

        private void Deactivate()
        {
            Activated = false;
            activateSound.Play();
            SetFrame(0);
        }

        private void Update()
        {
            base.Update();

            if (!Activated)
                return;

            MyTimer -= Time.deltaTime;

            if (MyTimer <= 0)
                Detonate();
        }

        private void Detonate()
        {
            if (Detonated)
                return;

            Detonated = true;
            new SpecialEffect("../Sprites/explotion_1.png", 40, 1, x, y);
            explodeSound.Play();
            Destroy();
        }
    }
}