namespace GXPEngine
{
    public class PlayerAnimation : GameObject
    {
        private readonly Player player;
        private float animationSpeed = 0.0005F;
        private AnimationStates animationState;
        private float cFrame;
        private int endFrame;
        public bool gotBitten = false;
        private bool mirrorSprite;
        private bool spaceDown;
        private int startFrame;

        public PlayerAnimation(Player _player)
        {
            player = _player;
            game.Add(this);
            SetAnimationState(AnimationStates.IDLE);
        }

        public void Update()
        {
            HandleAnimation();

            if (!Input.GetKey(Key.Z) && player.IsGrounded)
            {
                spaceDown = false;
            }
        }


        private void HandleAnimation()
        {
            if (gotBitten)
            {
                SetAnimationState(AnimationStates.HIT);
                if (cFrame >= 70.5 && cFrame <= 71)
                {
                    SetAnimationState(AnimationStates.IDLE);
                    gotBitten = false;
                }
            }
            else if (cFrame >= 24.5 && cFrame <= 25)
            {
                SetAnimationState(AnimationStates.GOINGUP);
            }
            else if (cFrame >= 27.5 && cFrame <= 28)
            {
                SetAnimationState(AnimationStates.FALL);
            }
            else if (player.IsGrounded && animationState == AnimationStates.FALL)
            {
                SetAnimationState(AnimationStates.LANDING);
            }
            else if (cFrame >= 34.5 && cFrame <= 35)
            {
                SetAnimationState(AnimationStates.RUN);
            }

            if (animationState != AnimationStates.JUMP && animationState != AnimationStates.GOINGUP &&
                animationState != AnimationStates.LANDING && animationState != AnimationStates.FALL &&
                animationState != AnimationStates.HIT)
            {
                if (player.Hitpoints < 0)
                {
                    SetAnimationState(AnimationStates.DIE);
                }
                else if (Input.GetKey(Key.S) && player.IsGrounded && spaceDown == false)
                {
                    SetAnimationState(AnimationStates.JUMP);
                    spaceDown = true;
                }
                else if (player.IsGrounded == false)
                {
                    SetAnimationState(AnimationStates.FALL);
                }
                else if (Input.GetKey(Key.LEFT) || Input.GetKey(Key.RIGHT))
                {
                    SetAnimationState(AnimationStates.RUN);
                }
                else if (Input.GetKey(Key.X))
                {
                    SetAnimationState(AnimationStates.HEAL);
                }
                else
                {
                    SetAnimationState(AnimationStates.IDLE);
                }
            }
            //}
            if (animationState == AnimationStates.IDLE)
            {
                animationSpeed = 0.15f;
            }
            else if (animationState == AnimationStates.FALL)
            {
                animationSpeed = 0.15f;
            }
            else animationSpeed = 0.5f;

            if (Input.GetKey(Key.LEFT))
            {
                mirrorSprite = true;
            }
            if (Input.GetKey(Key.RIGHT))
            {
                mirrorSprite = false;
            }

            if (cFrame >= endFrame)
            {
                cFrame = startFrame;
                //player.SetFrame(Convert.ToInt16(cFrame));
            }

            player.Mirror(mirrorSprite, false);
            player.SetFrame((int) cFrame);
            player.NextFrame();

            cFrame += animationSpeed;
        }

        private void SetAnimationState(AnimationStates state)
        {
            if (animationState == state)
                return;

            animationState = state;

            switch (animationState)
            {
                case AnimationStates.IDLE:
                    SetAnimationRange(44, 51);
                    break;

                case AnimationStates.GOINGUP:
                    SetAnimationRange(26, 28);
                    break;

                case AnimationStates.RUN:
                    SetAnimationRange(0, 16);
                    mirrorSprite = true;
                    break;

                case AnimationStates.JUMP:
                    SetAnimationRange(22, 26);
                    break;

                case AnimationStates.FALL:
                    SetAnimationRange(28, 30);
                    break;

                case AnimationStates.LANDING:
                    SetAnimationRange(31, 36);
                    break;

                case AnimationStates.HEAL:
                    SetAnimationRange(55, 65);
                    break;

                case AnimationStates.HIT:
                    SetAnimationRange(66, 70);
                    break;
            }

            if (cFrame < startFrame)
                cFrame = startFrame;
        }

        private void SetAnimationRange(int start, int end)
        {
            startFrame = start;
            endFrame = end;
        }

        private enum AnimationStates
        {
            IDLE,
            RUN,
            JUMP,
            FALL,
            HEAL,
            DIE,
            HIT,
            GOINGUP,
            LANDING
        }
    }
}