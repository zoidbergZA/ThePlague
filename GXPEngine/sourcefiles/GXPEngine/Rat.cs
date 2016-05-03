using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class Rat : Character
    {
        private float attackCooldown;

        private int attackDamage = 20;
        private float attackRate = 0.4f; // attacks per second
        private Sound attackSound = new Sound("../Sounds/rat_attack.wav");
        private Fixture hearingCircle;
        private float hearingDistance = 192f;
        private RatAnimator ratAnimator;
        private RatController ratController;

        public Rat(World world, Vector2 spawnPosition, float spawnRotation)
            : base(Constants.RAT, spawnPosition, 8, 4) //change rows/cols to match spritesheet
        {
            DeathSound = new Sound("../Sounds/rat_death.wav");
            ratController = new RatController(this);
            ratAnimator = new RatAnimator(this);
            Alive = true;
            Hitpoints = 100;

            PhysicsHelper.BuildBody(this, ShapeType.Circle, BodyType.Dynamic, world, spawnPosition, spawnRotation);

            game.Add(this);
            SetOrigin(width/2, height/2);

            var hearingCircleShape = new CircleShape(ConvertUnits.ToSimUnits(hearingDistance), 0f);

            var offset = new Vector2(0f, 0f);

            hearingCircleShape.Position = offset;
            hearingCircle = body.CreateFixture(hearingCircleShape);
            fixtures.Add(hearingCircle);

            body.FixedRotation = true;


            //Subscribe to collisions
            foreach (Fixture fixture in fixtures)
            {
                fixture.OnCollision += MyOnCollision;
            }
        }

        private void Update()
        {
            if (!Alive)
                return;

            if (body == null)
                return;

            SyncTransforms();

            if (attackCooldown > 0)
                attackCooldown -= Time.deltaTime;

            if (attackCooldown <= 0 && ratController.Target != null && ratController.Target.body != null)
            {
                float dist =
                    ConvertUnits.ToDisplayUnits(Vector2.Distance(body.Position, ratController.Target.body.Position));

                if (dist < GameManager.Instance.Level.TileSize*3)
                    Attack(ratController.Target);
            }

            if (body != null && body.LinearVelocity.X >= 0.1f)
                Mirror(false, false);
            else
                Mirror(true, false);
        }

        public override void TakeDamage(int amount)
        {
            if (Invulnrable)
                return;

            Hitpoints -= amount;

            if (Hitpoints <= 0)
            {
                Hitpoints = 0;
                HandleDeath();
            }
        }

        public override void HandleDeath()
        {
            Alive = false;

            new SpecialEffect("../Sprites/rat_sheet_die.png", 5, 1, x, y, 0.03f);

            if (DeathSound != null)
                DeathSound.Play();
            body.Dispose();
            body = null;
            Destroy();

            game.Remove(ratController);
            ratController.Destroy();
            ratController = null;

            game.Remove(ratAnimator);
            ratAnimator.Destroy();
            ratAnimator = null;

            game.Remove(this);
            hearingCircle = null;
            attackSound = null;
            DeathSound = null;
        }

        private void Attack(Character attackTarget)
        {
            if (attackCooldown > 0)
                return;

            if (ratController.Target == null)
                ratController.Target = attackTarget;

            if (ratController.Target.body == null)
            {
                ratController.Target = null;
                return;
            }

            attackCooldown = (1/attackRate)*1000;

            if (attackTarget is Victim)
            {
                attackTarget.TakeDamage(200);
            }
            if (attackTarget is Player)
            {
                var p = (Player) attackTarget;
                p.LoseBreath(19);
            }
            attackSound.Play();

            BumpTargetBack(attackTarget);
        }

        public bool MyOnCollision(Fixture f1, Fixture f2, Contact contact)
        {
            if (f1 == hearingCircle && f2.UserData is Player)
            {
                var player = (Player) f2.UserData;

                if (f2 == player.interactor)
                {
                    ratController.SetTarget(player);
                }
            }

            if (f1 != hearingCircle && f2.UserData is Player)
            {
                var player = (Player) f2.UserData;

                Attack(player);
            }

            if (f1 != hearingCircle && f2.UserData is Victim)
            {
                var victim = (Victim) f2.UserData;

                if (!f2.IsSensor)
                {
                    Attack(victim);
                }
            }

            //collider return true, trigger return false
            if (f1 == hearingCircle)
                return false;
            return true;
        }

        public string GetRatAIState()
        {
            return ratController.GetState();
        }

        private void BumpTargetBack(Character attackTarget)
        {
            if (attackTarget.body == null || body == null)
                return;
            Vector2 relPos = attackTarget.body.Position - body.Position;

            relPos.Normalize();

            attackTarget.body.ApplyLinearImpulse(relPos*300f);
        }
    }
}