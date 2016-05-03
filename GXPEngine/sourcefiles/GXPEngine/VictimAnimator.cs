namespace GXPEngine
{
    internal class VictimAnimator : GameObject
    {
        private readonly Victim victim;
        private readonly VictimController victimController;
        private float animationSpeed = 0.1F;
        private float cFrame;
        private int endFrame;
        public bool gotBitten = false;
        private string lastState;
        private bool mirrorSprite;
        private int startFrame;
        private string state;

        public VictimAnimator(Victim victim, VictimController victimController)
        {
            this.victim = victim;
            this.victimController = victimController;

            SetAnimationState("ASLEEP");
            SetAnimationRange(12, 13);
            game.Add(this);
        }

        private void Update()
        {
            HandleAnimation();
        }


        private void HandleAnimation()
        {
            lastState = state;

            if (victimController == null)
                return;

            state = victimController.GetState();
            if (victim.moving == false && state == "FOLLOW")
            {
                state = "IDLE";
            }
            if (lastState != state)
            {
                SetAnimationState(state);
            }

            if (cFrame >= endFrame)
            {
                cFrame = startFrame;
            }

            victim.SetFrame((int) cFrame);
            victim.NextFrame();

            cFrame += animationSpeed;
        }

        private void SetAnimationState(string state)
        {
            switch (state)
            {
                case "IDLE":
                    SetAnimationRange(5, 7);
                    break;

                case "SAVED":
                    SetAnimationRange(5, 7);
                    break;

                case "FOLLOW":
                    SetAnimationRange(0, 2);
                    break;


                case "GETTINGUP":
                    SetAnimationRange(12, 13);
                    break;


                case "COLAPSING":
                    SetAnimationRange(18, 23);
                    break;

                case "ASLEEP":
                    SetAnimationRange(12, 13);
                    break;
                default:
                    victim.SetFrame(10);
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
    }
}