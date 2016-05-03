using System.Collections.Generic;
using System.Drawing;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    public class Player : Character
    {
        private readonly Sound coughBad = new Sound("../Sounds/cough_bad.wav");
        private readonly Sound coughMedium = new Sound("../Sounds/cough_medium.wav");
        private readonly Sound damageSound = new Sound("../Sounds/cough.wav");
        private readonly List<Body> feetList;
        private readonly Sound landSound = new Sound("../Sounds/jump.wav");
        public readonly World myWorld;
        private readonly Image portrait = new Bitmap("../Sprites/doctor_head.png");
        private Sound currentCough;
        private bool debugMode = false;
        public Fixture feet;
        private float lastCough;
        private int lastDamage;
        private bool masked = false;
        private PlayerAnimation playerAnimation;
        private PlayerController playerControl;
        private int regenCooldown = 3000;
        private int restartTimer = 4000;

        public Player(World world, Vector2 spawnPosition)
            : base(Constants.CHARACTER_DOCTOR, spawnPosition, 11, 7) //change rows/cols to match spritesheet
        {
            DeathSound = new Sound("../Sounds/human_death.wav");

            SetOrigin(width/2, height/2);

            FogBoxes = new List<FogBox>();
            feetList = new List<Body>();

            Alive = true;
            Hitpoints = 100;
            MaxBreath = 100f;
            Breath = MaxBreath;
            lastCough = Breath;
            currentCough = coughMedium;

            if (debugMode)
                Invulnrable = true;

            myWorld = world;
            body = BodyFactory.CreateCapsule(world, ConvertUnits.ToSimUnits(height - width),
                ConvertUnits.ToSimUnits(width*0.5f), 5f);
            body.BodyType = BodyType.Dynamic;
            body.UserData = this;
            body.Position = ConvertUnits.ToSimUnits(spawnPosition);

            var circleShape = new CircleShape(ConvertUnits.ToSimUnits(width/2 + 4), 0f);
            Fixture fixture = body.CreateFixture(circleShape);
            interactor = fixture;
            interactor.IsSensor = true;

            var feetShape = new CircleShape(ConvertUnits.ToSimUnits(width/6), 0f);
            feetShape.Position = new Vector2(0, ConvertUnits.ToSimUnits(height/2 - 10f));
            feet = body.CreateFixture(feetShape);
            feet.IsSensor = true;

            foreach (Fixture f in body.FixtureList)
            {
                f.UserData = this;
            }

            body.FixedRotation = true;
            body.Friction = 0.06f;
            body.LinearDamping = 0.98f;

            playerAnimation = new PlayerAnimation(this);
            playerControl = new PlayerController(this);
            new CameraController(GameManager.Instance.Level, this);

            feet.OnCollision += MyOnCollision;
            feet.OnSeparation += MyOnSeperation;

            game.Add(this);
        }

        public float MaxBreath { get; private set; }
        public float Breath { get; private set; }
        public List<FogBox> FogBoxes { get; set; }
        public bool InFog { get; private set; }

        public bool IsGrounded
        {
            get
            {
//                Console.WriteLine(feetList.Count);
//                foreach (Fixture f in feetList)
//                {
//                    Console.WriteLine(f.Body.FixtureList.Count);
//                }
                if (feetList.Count > 0)
                    return true;
                return false;
            }
        }

        private void Update()
        {
//            if (!Alive && restartTimer >= 0)
//            {
//                restartTimer -= Time.deltaTime;
//
//                if (restartTimer <= 0)
//                    OnDeathCompleted();
//            }

            if (!Alive || body == null)
                return;

            SyncTransforms();
        }

        public override void TakeDamage(int amount)
        {
            if (Invulnrable)
                return;

            Hitpoints -= amount;
            lastDamage = Time.time;

            if (Hitpoints <= 0 && Alive)
            {
                Hitpoints = 0;
                HandleDeath();
            }
        }

        public void RegenerateBreath(float amount)
        {
            if (lastDamage + regenCooldown > Time.time)
                return;

            Breath += amount;

            if (Breath > MaxBreath)
            {
                Breath = MaxBreath;
            }
        }

        public void LoseBreath(float amount)
        {
            Breath -= amount;
            lastDamage = Time.time;

            if (Breath <= 0 && Alive)
            {
                Breath = 0;
                HandleDeath();
                return;
            }

            if (amount > 10)
                damageSound.Play();

            else
            {
                float breathFraction = Breath/MaxBreath;
                float coughFraction = MaxBreath/4;

                currentCough = coughMedium;

                if (breathFraction < 0.5f)
                {
                    coughFraction *= 0.5f;
                }
                if (breathFraction < 0.25f)
                {
                    coughFraction *= 0.8f;
                    currentCough = coughBad;
                }
                if (Breath <= lastCough - coughFraction)
                {
                    lastCough = Breath;
                    currentCough.Play();
                }
            }
        }

        public override void HandleDeath()
        {
            Alive = false;

            if (body != null)
            {
                body.Dispose();
                body = null;
            }

            GameManager.Instance.Level.EndLevelTimer(restartTimer);
            game.Remove(playerControl);
            playerControl.Destroy();
            playerControl = null;
            game.Remove(playerAnimation);
            playerAnimation.Destroy();
            playerAnimation = null;
            if (GameManager.Instance.Level.mask != null)
            {
                GameManager.Instance.Level.mask.DeathTween();
            }

            DeathSound.Play();
            Toast.NewToast("Aaargh..", 3000, portrait);
            new SpecialEffect("../Sprites/doctor_sheet_die.png", 7, 1, x, y, 0.1f);

            feet = null;
            alpha = 0f; //todo: override renderSelf to stop render
            Destroy();
        }

        public void ResetFeet()
        {
            feetList.Clear();
//            Console.WriteLine(feetList.Count);
        }

        public void PlayCough()
        {
            coughMedium.Play();
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        private void OnDeathCompleted()
        {
            GameManager.Instance.Player = null;

            game.Remove(this);
            Destroy();
        }

        private bool MyOnCollision(Fixture f1, Fixture f2, Contact contact)
        {
            if (feetList.Contains(f2.Body))
                return false;

            if (f2.UserData is Player || f2.UserData is Victim || f2.UserData is FogBox || f2.IsSensor)
                return false;

            feetList.Add(f2.Body);

            if (feetList.Count == 1)
                landSound.Play();

//            Console.WriteLine("feet:" + feetList.Count);
//            foreach (Fixture f in feetList)
//            {
//                Console.WriteLine(f.UserData);
//            }

            return true;
        }

        private void MyOnSeperation(Fixture f1, Fixture f2)
        {
            if (!feetList.Contains(f2.Body))
                return;

            feetList.Remove(f2.Body);
        }

//        private float LateralInput()
//        {
//            float xInput = 0;
//
//            if (Input.GetKey(Key.LEFT))
//                xInput -= 1;
//            if (Input.GetKey(Key.RIGHT))
//                xInput += 1;
//
//            return xInput;
//        }
    }
}