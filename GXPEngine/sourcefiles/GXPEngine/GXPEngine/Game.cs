using System;
using GXPEngine.Core;
using GXPEngine.Managers;

namespace GXPEngine
{
    /// <summary>
    ///     The Game class represents the Game window.
    ///     Only a single instance of this class is allowed.
    /// </summary>
    public class Game : GameObject
    {
        private const int FRAMETIME = 15;
        internal static Game main = null;
        private readonly CollisionManager _collisionManager;

        private readonly GLContext _glContext;
        private readonly UpdateManager _updateManager;
        private int _timeAccumulator;

        //------------------------------------------------------------------------------------------------------------------------
        //														Game()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Initializes a new instance of the <see cref="GXPEngine.Game" /> class.
        ///     This class represents a game window, containing an openGL view.
        /// </summary>
        /// <param name='width'>
        ///     Width of the window in pixels.
        /// </param>
        /// <param name='height'>
        ///     Height of the window in pixels.
        /// </param>
        /// <param name='fullScreen'>
        ///     If set to <c>true</c> the application will run in fullscreen mode.
        /// </param>
        public Game(int width, int height, bool fullScreen)
        {
            if (main != null)
            {
                throw new Exception("Only a single instance of Game is allowed");
            }
            main = this;
            _updateManager = new UpdateManager();
            _collisionManager = new CollisionManager();
            _glContext = new GLContext(this);
            _glContext.CreateWindow(width, height, fullScreen);

            if (main != null) main.Add(this);
        }

        /// <summary>
        ///     Returns the width of the window.
        /// </summary>
        public int width
        {
            get { return _glContext.width; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														height
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Returns the height of the window.
        /// </summary>
        public int height
        {
            get { return _glContext.height; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetViewPort()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Sets the rendering output view port. All rendering will be done within the given rectangle.
        ///     The default setting is {0, 0, game.width, game.height}.
        /// </summary>
        /// <param name='x'>
        ///     The x coordinate.
        /// </param>
        /// <param name='y'>
        ///     The y coordinate.
        /// </param>
        /// <param name='width'>
        ///     The new width of the viewport.
        /// </param>
        /// <param name='height'>
        ///     The new height of the viewport.
        /// </param>
        public void SetViewport(int x, int y, int width, int height)
        {
            _glContext.SetScissor(x, y, width, height);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ShowMouse()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Shows or hides the mouse cursor.
        /// </summary>
        /// <param name='enable'>
        ///     Set this to 'true' to enable the cursor.
        ///     Else, set this to 'false'.
        /// </param>
        public void ShowMouse(bool enable)
        {
            _glContext.ShowCursor(enable);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Start()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Start the game loop. Call this once at the start of your game.
        /// </summary>
        public void Start()
        {
            _glContext.Run();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Step()
        //------------------------------------------------------------------------------------------------------------------------
        internal void Step()
        {
            Sound.Step();
            _timeAccumulator += Time.deltaTime;
            if (_timeAccumulator > 1000)
                _timeAccumulator = 0; //safety
            if (_timeAccumulator >= FRAMETIME)
            {
                _timeAccumulator -= FRAMETIME;
                _updateManager.Step();
                _collisionManager.Step();
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderSelf()
        //------------------------------------------------------------------------------------------------------------------------
        protected override void RenderSelf(GLContext glContext)
        {
            //empty
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Add()
        //------------------------------------------------------------------------------------------------------------------------
        internal void Add(GameObject gameObject)
        {
            _updateManager.Add(gameObject);
            _collisionManager.Add(gameObject);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Remove()
        //------------------------------------------------------------------------------------------------------------------------
        internal void Remove(GameObject gameObject)
        {
            _updateManager.Remove(gameObject);
            _collisionManager.Remove(gameObject);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														width
        //------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------
        //														Destroy()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Destroys the game, and closes the game window.
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();
            _glContext.Close();
        }
    }
}