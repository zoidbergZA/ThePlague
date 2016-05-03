namespace GXPEngine
{
    internal class RatAnimator : GameObject
    {
        private readonly Rat rat;
        private float animationSpeed = 0.3F;
        private float cFrame;
        private int endFrame;
        private string lastState;
        private int startFrame;
        private string state;


        public RatAnimator(Rat rat)
        {
            this.rat = rat;

            game.Add(this);
        }


        private void Update()
        {
            HandleAnimation();
        }


        private void HandleAnimation()
        {
            lastState = state;
            state = rat.GetRatAIState();


            if (lastState != state)
            {
                SetAnimationState(state);
            }

            if (cFrame >= endFrame)
            {
                cFrame = startFrame;
            }

            rat.SetFrame((int) cFrame);
            rat.NextFrame();

            cFrame += animationSpeed;
        }

        private void SetAnimationState(string state)
        {
            switch (state)
            {
                case "CHASE":
                    SetAnimationRange(0, 5);
                    break;

                case "PATROL":
                    SetAnimationRange(0, 5);
                    break;

                case "BITE":
                    SetAnimationRange(7, 9);
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