using System;
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
    internal class Victim : Character
    {
        private readonly Sprite healIcon;
        private readonly Sound successSound = new Sound("../Sounds/success.wav");
        private bool inFog = true;
        public bool moving = false;
        public Image portrait;
        private bool previousEState;
        private bool saved;
        private VictimController victimAI;
        private VictimAnimator victimAnimator;

        public Victim(World world, Vector2 spawnPosition, float spawnRotation)
            : base(Constants.CHARACTER_VICTIM, spawnPosition, 6, 4)
        {
            DeathSound = new Sound("../Sounds/human_death.wav");
            game.Add(this);

            Alive = true;
            Hitpoints = 100;

            victimAI = new VictimController(this);
            victimAnimator = new VictimAnimator(this, victimAI);
            FogBoxes = new List<FogBox>();
            healIcon = new Sprite("../Sprites/heal_icon.png");
            portrait = new Bitmap("../Sprites/victim_head.png");
            AddChild(healIcon);
            healIcon.alpha = 0f;
            healIcon.SetXY(-healIcon.width/4, -height - (healIcon.height/2));

//            PhysicsHelper.BuildBody(this, ShapeType.Polygon, BodyType.Dynamic, world, spawnPosition, spawnRotation);

            body = BodyFactory.CreateCapsule(world, ConvertUnits.ToSimUnits(height - width),
                ConvertUnits.ToSimUnits(width*0.5f), 5f);
            body.BodyType = BodyType.Dynamic;
            body.UserData = this;
            body.Position = ConvertUnits.ToSimUnits(spawnPosition);
            body.FixedRotation = true;

            SetOrigin(width/2, height/2);

            float interactorSize = width*2;
            var circleShape = new CircleShape(ConvertUnits.ToSimUnits(interactorSize), 0f);
            Fixture fixture = body.CreateFixture(circleShape);
            interactor = fixture;
            interactor.IsSensor = true;

            foreach (Fixture f in body.FixtureList)
            {
                f.UserData = this;

                f.OnCollision += MyOnCollision;
                f.OnSeparation += MyOnSeperation;
            }

            interactor.OnCollision += MyOnCollision;
            interactor.OnSeparation += MyOnSeperation;
        }

        public bool InPlayerRange { get; private set; }
        public List<FogBox> FogBoxes { get; set; }

        private void Update()
        {
            if (body == null || saved)
                return;

            bool keyXDown = Input.GetKey(Key.X);
            bool rescueKey = keyXDown && !previousEState;
            previousEState = keyXDown;

            inFog = ChecFogState();

            if (!inFog && (Time.time - GameManager.Instance.Level.StartedAt) > 4000)
                OnRescued();

            if (GameManager.Instance.Player != null)
            {
                if (rescueKey && victimAI.state == VictimController.States.ASLEEP)
                {
                    //Console.WriteLine (this.HasChild(interactor)+" "+InPlayerRange);
                    if (InPlayerRange)
                    {
                        victimAI.SetTarget(GameManager.Instance.Player);
                        healIcon.alpha = 0f;
                        Toast.NewToast("OK, I will follow you", 4000, portrait);
                    }
                }
            }

            SyncTransforms();
            NextFrame();
        }

        public override void TakeDamage(int amount)
        {
            if (Invulnrable)
                return;

            Hitpoints -= amount;
            Console.WriteLine(Hitpoints);
            if (Hitpoints <= 0 && Alive)
            {
                Hitpoints = 0;
                HandleDeath();
            }
        }

        public override void HandleDeath()
        {
            DeathSound.Play();
            new SpecialEffect("../Sprites/victim_sheet_die.png", 6, 1, x, y, 0.06f);

            GameManager.Instance.Level.OnVictimDeath();

            this.visible = false;
            body.Dispose();
            Destroy();
        }

        private void OnRescued()
        {
            saved = true;
            victimAI.state = VictimController.States.SAVED;
            successSound.Play();
            GameManager.Instance.Level.OnVictimResqued();

            new SpecialEffect("../Sprites/victim_saved.png", 4, 1, x, y, 0.03f);

            Destroy();
        }

        private bool ChecFogState()
        {
            if (FogBoxes.Count > 0)
                return true;
            return false;
        }

        public bool MyOnCollision(Fixture f1, Fixture f2, Contact contact)
        {
            if (f1 != interactor && f2.UserData is Player)
            {
                return false;
            }

            //interact with the player
            if (f1 == interactor && f2.UserData is Player && victimAI.state == VictimController.States.ASLEEP)
            {
                var player = (Player) f2.UserData;
                if (f2 == player.interactor)
                {
//                    Console.WriteLine("hi player");
                    InPlayerRange = true;
                    healIcon.alpha = 1f;
                }
            }

            return true;
        }

        public void MyOnSeperation(Fixture f1, Fixture f2)
        {
            if (f1 == interactor && f2.UserData is Player)
            {
                var player = (Player) f2.UserData;
                if (f2 == player.interactor)
                {
//                    Console.WriteLine("bye player");
                    InPlayerRange = false;
                    healIcon.alpha = 0f;
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            game.Remove(victimAI);
            victimAI.Destroy();
            victimAI = null;

            game.Remove(victimAnimator);
            victimAnimator.Destroy();
            victimAnimator = null;
            portrait.Dispose();

            game.Remove(this);
        }

        public string GetAIState()
        {
            return victimAI.GetState();
        }
    }
}