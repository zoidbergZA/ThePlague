using System;

//using System.Windows.Forms;

namespace GXPEngine
{
    internal class MainMenu : Sprite
    {
        //private Level level;
        private readonly Sound clickSound = new Sound("../Sounds/slime_jump.wav");
        private readonly Button controlsButton;
        private readonly FogMenu fog;
        private readonly FogMenu fogForeGround;
        private readonly Selecter levelSelecter;
        private readonly Button quitButton;
        private readonly Button startButton;
        private readonly Sprite title;
        private int _state = 1;
        private InstructionPage instructionPage;
        private Level level;
        private Sound music = new Sound("../Sounds/music/menu.mp3", true, true);


        public MainMenu(Level l = null) : base(".../sprites/mainMenuBackground.png")
        {
            startButton = new Button("start", (game.width - 248)/2, game.height/2);
            controlsButton = new Button("control", startButton.x, startButton.y + 50);
            quitButton = new Button("quit", controlsButton.x, controlsButton.y + 50);
            levelSelecter = new Selecter();
            levelSelecter.x = startButton.x - 100;
            fog = new FogMenu(48);

            title = new Sprite(".../sprites/title.png");
            title.x = (game.width - title.width)/2;
            title.y = 200;

            music.Play();

            game.AddChild(this);
            fogForeGround = new FogMenu(5);
            addButtons();
        }

        private void updateState()
        {
            switch (_state)
            {
                case 1:
                    levelSelecter.y = startButton.y;
                    if (Input.GetKeyDown(Key.Z))
                    {
//                    Console.WriteLine("boo");
                        level = new Level("big_level");
                        //Level.LoadLevel("level2");

                        music.Play(true);
                        music = null;

                        levelSelecter.Destroy();
                        game.RemoveChild(this);
                        Destroy();
                    }
                    break;

                case 2:
                    levelSelecter.y = controlsButton.y;
                    if (Input.GetKeyDown(Key.Z))
                    {
                        clickSound.Play();
                        instructionPage = new InstructionPage(this);
                        AddChild(instructionPage);
                    }


                    break;

                case 3:
                    levelSelecter.y = quitButton.y;
                    if (Input.GetKeyDown(Key.Z))
                    {
//					game.Remove(this);
//					this.Destroy();
                        Environment.Exit(1);
                    }
                    break;

                default:
                    _state = 1;
                    break;
            }
        }

        private void Update()
        {
            if (HasChild(instructionPage) == false)
            {
                updateState();
            }

            if (Input.GetKeyDown(Key.UP))
            {
                clickSound.Play();
                _state--;
                if (_state < 1)
                {
                    _state = 3;
                }
            }
            else if (Input.GetKeyDown(Key.DOWN))
            {
                clickSound.Play();
                _state++;
                if (_state > 3)
                {
                    _state = 1;
                }
            }
        }

        private void addButtons()
        {
            AddChild(fog);
            AddChild(title);
            AddChild(levelSelecter);
            AddChild(startButton);
            AddChild(quitButton);
            AddChild(controlsButton);
            AddChild(fogForeGround);
        }
    }
}