using System;
using FarseerPhysics;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class RatController : GameObject
    {
        public enum States
        {
            PATROL,
            CHASE
        }

        private readonly Vector2[] waypoints;

        private float chaseSpeed = 2f;
        private int currentWaypoint;
        private float movementForce = 150f;
        private float patrolSpeed = 0.35f;
        private Rat rat;

        public RatController(Rat rat)
        {
            this.rat = rat;
            State = States.PATROL;

            waypoints = new[]
            {
                new Vector2(rat.x - 6*GameManager.Instance.Level.TileSize, rat.y),
                new Vector2(rat.x + 6*GameManager.Instance.Level.TileSize, rat.y)
            };

            game.Add(this);
        }

        public States State { get; set; }
        public Character Target { get; set; }

        public void SetTarget(Character target)
        {
            Target = target;
            State = States.CHASE;
        }

        private void Update()
        {
            if (rat.body == null)
            {
                rat = null;
                Destroy();
                return;
            }

            switch (State)
            {
                case States.PATROL:
                    Patrol();
                    break;

                case States.CHASE:
                    Chase();
                    break;
            }
        }

        private void Patrol()
        {
            var myPosition = new Vector2(rat.x, rat.y);

            if (Vector2.Distance(myPosition, waypoints[currentWaypoint]) < GameManager.Instance.Level.TileSize*0.5f)
            {
                currentWaypoint++;

                if (currentWaypoint >= waypoints.Length)
                    currentWaypoint = 0;
            }

            Move(ConvertUnits.ToSimUnits(waypoints[currentWaypoint]), patrolSpeed);
        }

        private void Chase()
        {
            if (Target == null)
            {
                State = States.PATROL;
                return;
            }

            if (Target.body == null)
            {
                Target = null;
                State = States.PATROL;
                return;
            }

            float distanceToTarget =
                ConvertUnits.ToDisplayUnits(Vector2.Distance(Target.body.Position, Target.body.Position));

//            if (distanceToTarget < 10f)
//                return;

            Move(Target.body.Position, chaseSpeed);
        }

        private void Move(Vector2 moveTo, float speed)
        {
            if (Math.Abs(rat.body.LinearVelocity.X) < speed)
            {
                if (moveTo.X < rat.body.Position.X)
                {
                    rat.body.ApplyForce(new Vector2(-movementForce, 0f));
                }
                else
                {
                    rat.body.ApplyForce(new Vector2(movementForce, 0f));
                }
            }
        }

        public string GetState()
        {
            return State.ToString();
        }
    }
}