using System;
using System.Collections.Generic;
using System.Xml;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    public class Level : GameObject
    {
        //private Sound music;
        private Sprite bg;
        public Hud hud;
        private XmlDocument levelXmlDocument;
        private MainMenu mainmenu;
        public Mask mask;
        private Sound music;
        public List<Vector2> playerPlacers;
        private int resqueGoal = 5;
        private bool restartCalled;
        private int restartTimer;
        private int tileCounter;
        public Tile[] tileset;
        private int victimCount = 7;
        public List<Vector2> victimPlacers;
        private World world;

        public Level(string levelName)
        {
            GameManager.Instance.Level = this;
            StartedAt = Time.time;

            playerPlacers = new List<Vector2>();
            victimPlacers = new List<Vector2>();

            RenderLayer = new GameObject[6];

            for (int i = 0; i < RenderLayer.Length; i++)
            {
                var go = new GameObject();
                RenderLayer[i] = go;
                AddChild(go);
            }

            music = new Sound("../Sounds/music/plague_ingame.mp3", true, true);
            music.Play();

            // create a world with normal gravity
            world = new World(new Vector2(0.0f, 9.82f));
            GameManager.Instance.World = world;

            //TODO: move this operation somewhere better(perhaps game)
            ConvertUnits.SetDisplayUnitToSimUnitRatio(Constants.WORLD_TO_PIXEL_RATIO);

            LoadLevelData(levelName);

            //Place player and victims
            PlacePlayer();
            PlaceVictims();

            game.Add(this);
            game.AddChild(this);

            hud = new Hud();
            game.AddChild(hud);

            mask = new Mask(GameManager.Instance.Player);
            Toast.NewToast("Find and Rescue " + resqueGoal + " of " + victimCount + " victims!");
        }

        public int StartedAt { get; private set; }
        public GameObject[] RenderLayer { get; private set; }
        public int TileSize { get; private set; }
        public int VictimsResqued { get; set; }

        public int VictimCount
        {
            get { return victimCount; }
        }

        public void AddChildOnLayer(GameObject childGameObject, int layerNumber)
        {
            if (layerNumber < 0)
                layerNumber = 0;
            if (layerNumber > RenderLayer.Length - 1)
                layerNumber = RenderLayer.Length - 1;

            RenderLayer[layerNumber].AddChild(childGameObject);
        }

        public void EndLevelTimer(int timer)
        {
            restartTimer = timer;
            restartCalled = true;
        }

        private void LoadLevelData(string levelName)
        {
            string levelPath = "../Levels/" + levelName + ".tmx";

            // Load a background
            bg = new Sprite("../Sprites/background.png");
            game.AddChild(bg);

            levelXmlDocument = new XmlDocument();
            levelXmlDocument.Load(levelPath);

            XmlElement root = levelXmlDocument.DocumentElement;

            //Get the map metadata
            string version = root.GetAttribute("version");
            TileSize = int.Parse(root.GetAttribute("tilewidth")); // the width/height of the individual tiles

            //Get the tileset
            XmlNode tilesetNode = levelXmlDocument.SelectSingleNode("map/tileset");
            string tilesetSource = tilesetNode.Attributes["source"].Value;

            LoadTileset(tilesetSource);

            //For each object layer, place the tiles in that layer
            XmlNodeList objectGroups = levelXmlDocument.SelectNodes("map/objectgroup");

            foreach (XmlElement objectGroup in objectGroups)
            {
                XmlNodeList objectPlacers = objectGroup.SelectNodes("object");

                foreach (XmlNode objectNode in objectPlacers)
                {
                    var position = new Vector2(int.Parse(objectNode.Attributes["x"].Value),
                        int.Parse(objectNode.Attributes["y"].Value));

                    PlaceTile(position, int.Parse(objectNode.Attributes["gid"].Value));
                }
            }

            levelXmlDocument = null;
        }

        private void LoadTileset(string tilesetName)
        {
            string tilesetPath = "../Levels/" + tilesetName;
            var tilesetXmlDocument = new XmlDocument();

            tilesetXmlDocument.Load(tilesetPath);

            XmlNodeList tileNodes = tilesetXmlDocument.SelectNodes("tileset/tile");

            tileset = new Tile[tileNodes.Count];

            tileCounter = 0;

            foreach (XmlElement tileElement in tileNodes)
            {
                var tile = new Tile();

                tile.id = int.Parse(tileElement.GetAttribute("id"));
                XmlNode imageNode = tileElement.SelectSingleNode("image");
                tile.width = int.Parse(imageNode.Attributes["width"].Value);
                tile.height = int.Parse(imageNode.Attributes["height"].Value);
                tile.source = imageNode.Attributes["source"].Value;

                //Get tile properties
                tile.properties = new Dictionary<string, string>();

                XmlNodeList propertyNodes = tileElement.SelectNodes("properties/property");

                if (propertyNodes.Count > 0)
                {
                    foreach (XmlNode propNode in propertyNodes)
                    {
                        string propKey = propNode.Attributes["name"].Value;
                        string propVal = propNode.Attributes["value"].Value;

                        tile.properties.Add(propKey, propVal);
                    }
                }

                tileset[tileCounter] = tile;
                tileCounter++;
            }
        }

        private void PlaceTile(Vector2 position, int tileGid)
        {
            if (tileGid == 0)
                return;

            Tile tile = tileset[tileGid - 1];

            var offset = new Vector2(tile.width/2, -tile.height/2);

            Vector2 placementPosition = position;
            placementPosition += offset;

            Prefabs.Instantiate(tileGid, placementPosition);
        }

        private void PlacePlayer()
        {
            if (playerPlacers.Count == 0)
                throw new Exception("no player placers found in level");

            int rand = Utils.Random(0, playerPlacers.Count);

            var player1 = new Player(world, playerPlacers[rand]);
            AddChildOnLayer(player1, 1);
            GameManager.Instance.Player = player1;
        }

        private void PlaceVictims()
        {
            if (victimPlacers.Count == 0 || victimPlacers.Count < victimCount)
                throw new Exception("not enough victims in the level");
//            Console.WriteLine(victimPlacers.Count);
            while (victimPlacers.Count > victimCount)
            {
                int rand = Utils.Random(0, victimPlacers.Count);

                victimPlacers.RemoveAt(rand);
            }

            for (int i = 0; i < victimPlacers.Count; i++)
            {
                var victim = new Victim(world, victimPlacers[i], 0);
                AddChildOnLayer(victim, 0);
            }
        }

        private void Update()
        {
            if (restartCalled && restartTimer > 0)
            {
                Console.WriteLine(restartTimer);
                restartTimer -= Time.deltaTime;
            }

            if (restartCalled && restartTimer <= 0)
            {
                HandleGameOver(false, "you died!");
            }

            //Reset key for testing
            if (Input.GetKeyDown(Key.Q))
                ResetLevel();

            if (Input.GetKeyDown(Key.Z) && GameManager.Instance.Player == null)
            {
                ResetLevel();
            }

            if (Time.deltaTime != 0)
            {
                if (world != null)
                {
                    float dt = 0.05f; // normal: 0.05f
//                Console.WriteLine((float)Time.deltaTime / 1000);
                    world.Step((float) Time.deltaTime/1000*2.85f);
                }
            }
        }

        public void OnVictimResqued()
        {
            VictimsResqued++;
            Console.WriteLine(VictimsResqued + " | " + resqueGoal);
            if (VictimsResqued == resqueGoal)
            {
                HandleGameOver(true, "you successfully resqued enough victims!");
                return;
            }

            Toast.NewToast("you saved a victim! "
                           + (VictimCount - VictimsResqued) + "/"
                           + VictimCount + " victims remaining..."
                );
        }

        public void OnVictimDeath()
        {
            victimCount--;

            Toast.NewToast("Victim killed!, " + victimCount + " victims remaining");

            if (VictimsResqued + VictimCount < resqueGoal)
            {
                HandleGameOver(false, "too few victims left");
            }
        }

        public void ResetLevel()
        {
            if (GameManager.Instance.Level != null)
            {
                game.Remove(this);
                game.RemoveChild(this);
                game.RemoveChild(bg);
                game.Remove(hud);
                game.Remove(bg);
                bg.Destroy();
                hud.Destroy();
                game.Remove(GameManager.Instance.Player);
                GameManager.Instance.Player.Destroy();

                levelXmlDocument = null;
//				GameManager.Instance.Player.Destroy();

                music.Play(true);
                music = null;

                world.Clear();
                world = null;
                GameManager.Instance.World = null;

//                GameManager.Instance.Level = null;
                GameManager.Instance.Player = null;


                MyGame.ycount++;
                Destroy();
                Console.WriteLine(this + "" + MyGame.ycount);

                //Level.LoadLevel ("level2");
            }
        }

        public void HandleGameOver(bool success, string reason)
        {
            if (success)
            {
                new EndMenu("win", reason);
            }
            else
            {
                new EndMenu("lose", reason);
            }

            ResetLevel();
            GameManager.Instance.PlayerScore = 0;
            GameManager.Instance.Clear();
        }

        public struct Tile
        {
            public int height;
            public int id;
            public Dictionary<string, string> properties;
            public string source;
            public int width;
        }
    }
}