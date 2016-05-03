using System.Collections.Generic;

namespace GXPEngine
{
    public class FogMenu : GameObject
    {
        private readonly List<Sprite> fogList = new List<Sprite>();
        private Sprite fog;

        public FogMenu(int clouds)
        {
            game.Add(this);
            for (int i = 0; i < clouds; i++)
            {
                fog = new Sprite(".../sprites/fogmenu.png");
                fog.SetScaleXY(0.5, 0.5);
                fog.x = ((game.width*2)/100)*(Utils.Random(0, 100)) - game.width;
                fog.y = (game.height/100)*Utils.Random(10, 90);
                fog.SetColor(0.55f, 1, 0.55f);
                AddChild(fog);
                fogList.Add(fog);
            }
        }

        public void Update()
        {
            foreach (Sprite fog in fogList)
            {
                fog.x += 0.2f;
                if (fog.x > game.width)
                {
                    fog.x = -game.width;
                }
            }
        }
    }
}