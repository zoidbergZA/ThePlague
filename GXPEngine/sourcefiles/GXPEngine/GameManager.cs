using FarseerPhysics.Dynamics;

namespace GXPEngine
{
    public class GameManager
    {
        #region Singleton

        private static GameManager instance;

        private GameManager()
        {
        }

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }

        #endregion

        public Player Player { get; set; }
        public Level Level { get; set; }
        public World World { get; set; }
        public int PlayerScore { get; set; }

        public void Clear()
        {
            Player = null;
            Level = null;
            World = null;
            Player = null;

            instance = null;
        }

//        public static Player player;
//        public static Level level;
//        public static World world;
//        public static int playerScore;

//        private static readonly string[] randomSounds =
//        {
//            "../Sounds/fart_1.wav",
//            "../Sounds/fart_2.wav",
//            "../Sounds/fart_3.wav",
//            "../Sounds/fart_4.wav",
//            "../Sounds/fart_5.wav",
//            "../Sounds/fart_6.wav"
//        };
//
//        public static void PlayRandomSound()
//        {
//            int rnd = Utils.Random(0, randomSounds.Length);
//
//            var fart = new Sound(randomSounds[rnd]);
//
//            fart.Play();
//        }
    }
}