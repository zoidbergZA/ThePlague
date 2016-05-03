using System;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    public class PlayerController : GameObject
    {
        private readonly Sound jumpSound = new Sound("../Sounds/jump2.mp3"); //todo: change to proper jump sound wav
        private readonly Player player;
//		private readonly Sound landSound = new Sound("../Sounds/footstep_01.wav");

        private bool debugMode = false;
        private float jumpForce = 1600;
        private float moveForce = 800f;

        public PlayerController(Player _player)
        {
            player = _player;
            game.Add(this);
        }

        public void Update()
        {
            if (player == null || !player.Alive || player.body == null)
                return;

            HandleControl();
        }

        private void HandleControl()
        {
            float lateralInput = LateralInput();

            //clamp x velocity
            Vector2 vel = player.body.LinearVelocity;

            if (Math.Abs(vel.X) > 3f)
                player.body.LinearVelocity = new Vector2(Math.Sign(vel.X)*3f, vel.Y);

            if (Input.GetKeyDown(Key.Z) && player.IsGrounded)
            {
                player.ResetFeet();
                player.body.ApplyLinearImpulse(new Vector2(0.0f, -jumpForce));
                jumpSound.Play();
            }

            //x force
            if (player.body.LinearVelocity.X < 3.0f && lateralInput > 0f)
                player.body.ApplyForce(new Vector2(lateralInput*moveForce, 0.0f));

            if (player.body.LinearVelocity.X > -3.0f && lateralInput < 0f)
                player.body.ApplyForce(new Vector2(lateralInput*moveForce, 0.0f));
        }

        private float LateralInput()
        {
            float xInput = 0;

            if (Input.GetKey(Key.LEFT))
                xInput -= 1;
            if (Input.GetKey(Key.RIGHT))
                xInput += 1;

            return xInput;
        }
    }
}