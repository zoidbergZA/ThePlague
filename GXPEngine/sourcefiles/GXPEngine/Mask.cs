using Glide;

namespace GXPEngine
{
    public class Mask : Sprite
    {
        private readonly Sprite lungsFill;
        private readonly Sprite lungsOutline;
        public bool MaskOn;
        private float cooldownTimer;
        private float damageCooldown = 1000f;
        public bool inhale;
        private float lungScaler = 1f;

        private float targetAlpha = 0.95f;
        private float targetScale = 1f;
        private Tweener tweener;

        public Mask(Player player)
            : base("../Sprites/mask.png")
        {
            Player = player;

            lungsOutline = new Sprite("../Sprites/lungs_outline.png");
            lungsFill = new Sprite("../Sprites/lungs_fill.png");

            AddChild(lungsFill);
            AddChild(lungsOutline);

            lungsFill.SetXY(25, 0);
            lungsOutline.SetXY(25, 0);

            width = game.width;
            height = game.height;
            alpha = 0f;

            game.Add(this);
            GameManager.Instance.Level.hud.AddChildOnLayer(this, 0);
        }

        public Player Player { get; set; }

        public void DeathTween()
        {
            tweener = new Tweener();

            tweener.Tween(this, new {alpha = 1F}, 8000f)
                .OnComplete(OnDeathTweenComplete);

            RemoveChild(lungsOutline);
            RemoveChild(lungsFill);
        }

        private void OnDeathTweenComplete()
        {
            Destroy();
        }

        private void Update()
        {
//            SetLungScale();
            ShowLungs();

            if (tweener != null)
                tweener.Update(Time.deltaTime);

            if (!Player.Alive)
                return;

            if (MaskOn != ChecMaskState())
            {
                if (!MaskOn)
                {
//                    Console.WriteLine("put on mask!");
                    CreateTween(targetAlpha, targetScale);
                    Player.PlayCough();
                }
                else
                {
//                    Console.WriteLine("remove mask");
                    MaskOffTween();
                }

                MaskOn = ChecMaskState();
            }

            if (MaskOn)
            {
                HandleFogDamage();
            }
            else
            {
                HandleRegen();
            }
        }

        private void SetLungScale()
        {
            lungsOutline.SetScaleXY(lungScaler, lungScaler);
            lungsFill.SetScaleXY(lungScaler, lungScaler);
        }

        private void ShowLungs()
        {
            float breathFraction = Player.Breath/Player.MaxBreath;

            float a = breathFraction*1.15f;
            if (a > 1f)
                a = 1f;

            lungsFill.alpha = (a);
            lungsFill.SetColor(breathFraction, 1 - breathFraction, 0);
        }

        private void HandleRegen()
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0)
            {
                Player.RegenerateBreath(4.1f);
                cooldownTimer = damageCooldown;
            }
        }

        private void HandleFogDamage()
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0)
            {
                Player.LoseBreath(1.2f);
                cooldownTimer = damageCooldown;
            }
        }

        private bool ChecMaskState()
        {
            if (Player == null)
                return false;

            if (Player.FogBoxes.Count > 0)
                return true;
            return false;
        }

        private void MaskOffTween()
        {
            tweener = new Tweener();

            tweener.Tween(this, new {alpha = 0f, targetScale = 1f}, 3000f)
                .Ease(Ease.CubeInOut);
        }

        private void CreateTween(float transparency, float scale)
        {
            tweener = new Tweener();

            tweener.Tween(this, new {alpha = transparency, lungScaler = scale}, 3000f, 700f)
                .Ease(Ease.CubeInOut)
                .OnComplete(OnTweenComplete);
        }

        private void OnTweenComplete()
        {
            if (inhale)
            {
                inhale = false;
                targetAlpha = 0.61f;
                targetScale = 0.8f;
            }
            else
            {
                inhale = true;
                targetAlpha = 0.26f;
                targetScale = 1f;
            }

            CreateTween(targetAlpha, targetScale);
        }

//        protected override void RenderSelf(Core.GLContext glContext)
//        {
//            GL.Enable(GL.BLEND);
//            GL.BlendFunc(GL.DST_COLOR, GL.SRC_COLOR);
//            base.RenderSelf(glContext);
//
//            GL.BlendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
//        }
    }
}