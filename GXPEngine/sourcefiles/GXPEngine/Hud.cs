namespace GXPEngine
{
    public class Hud : GameObject
    {
        private GUI gui;

        public Hud()
        {
            game.Add(this);
            HudLayer = new GameObject[6];

            for (int i = 0; i < HudLayer.Length; i++)
            {
                var go = new GameObject();
                HudLayer[i] = go;
                AddChild(go);
            }

            gui = new GUI();
            AddChildOnLayer(gui, 6);
        }

        public GameObject[] HudLayer { get; private set; }

        public void AddChildOnLayer(GameObject childGameObject, int layerNumber)
        {
            if (layerNumber < 0)
                layerNumber = 0;
            if (layerNumber > HudLayer.Length - 1)
                layerNumber = HudLayer.Length - 1;

            HudLayer[layerNumber].AddChild(childGameObject);
        }

        public override void Destroy()
        {
            base.Destroy();
            gui.Destroy();

            gui = null;
            HudLayer = null;
        }

        private void Update()
        {
            if (gui != null)
            {
                gui.DrawGUI();
            }
        }
    }
}