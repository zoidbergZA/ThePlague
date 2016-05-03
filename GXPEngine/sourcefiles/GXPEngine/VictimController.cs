using System;
using FarseerPhysics;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class VictimController : GameObject
    {
        public enum States
        {
            ASLEEP,
            IDLE,
            FOLLOW,
            SAVED
        }

        private readonly Victim victim;

        private float followRange = 800f;
        private float maxSpeed = 1.4f;
        private float moveForce = 400f;
        public States state;

        public VictimController(Victim victim)
        {
            this.victim = victim;
            game.Add(this);
            state = States.ASLEEP;
        }

        public Player Target { get; set; }

        public void SetTarget(Player targetObject)
        {
            Target = targetObject;
            state = States.FOLLOW;
        }

        private void Update()
        {
            switch (state)
            {
                case States.IDLE:
                    WaitForTarget();
                    break;

                case States.FOLLOW:
                    FollowTarget();
                    break;
            }
        }

        private void WaitForTarget()
        {
            if (Target.body == null)
            {
                Target = null;
                state = States.ASLEEP;
                return;
            }

            float distanceToTarget =
                ConvertUnits.ToDisplayUnits(Vector2.Distance(Target.body.Position, victim.body.Position));
            if (distanceToTarget < followRange)
            {
                state = States.FOLLOW;
                Toast.NewToast("I found you!", 3000, victim.portrait);
            }
        }

        private void FollowTarget()
        {
            if (Target.body == null)
            {
                Target = null;
                state = States.ASLEEP;
                return;
            }

            float distanceToTarget =
                ConvertUnits.ToDisplayUnits(Vector2.Distance(Target.body.Position, victim.body.Position));
            if (distanceToTarget > followRange)
            {
                state = States.IDLE;
                Toast.NewToast("I lost you...", 3000, victim.portrait);
            }

            if (distanceToTarget < 100f)
            {
                victim.moving = false;
                return;
            }
            if (Math.Abs(victim.body.LinearVelocity.X) > maxSpeed)
                return;

            if (Target.body.Position.X < victim.body.Position.X)
            {
                victim.moving = true;
                victim.body.ApplyForce(new Vector2(-moveForce, 0f));
                victim.Mirror(true, false);
            }
            else
            {
                victim.moving = true;
                victim.body.ApplyForce(new Vector2(moveForce, 0f));
                victim.Mirror(false, false);
            }
        }

        public String GetState()
        {
            return state.ToString();
        }
    }
}