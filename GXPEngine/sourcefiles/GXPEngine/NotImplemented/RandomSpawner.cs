using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class RandomSpawner : LevelObject
    {
        private float spawnRadius = 256f;
        private float timeLeft;
        private float timer;

        public RandomSpawner(string spritePath, ShapeType shapeType, BodyType bodyType, World world,
            Vector2 spawnPosition, float spawnRotation = 0f)
            : base(spritePath, world, spawnPosition, spawnRotation, shapeType, bodyType)
        {
            game.Add(this);
        }

        public int[] PrefabIds { get; set; }
        public int Amount { get; set; }

        public int Timer
        {
            set
            {
                timer = value;
                timeLeft = value;
            }
        }

        private void Update()
        {
            if (PrefabIds == null || PrefabIds.Length == 0)
                return;

            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0f)
            {
                timeLeft = timer;
                HandleSpawn();
            }
        }

        private void HandleSpawn()
        {
            for (int i = 0; i < Amount; i++)
            {
                int id = (Utils.Random(0, PrefabIds.Length));
                var spawnPosition = new Vector2(Utils.Random(-spawnRadius, spawnRadius),
                    Utils.Random(-spawnRadius, spawnRadius));

                spawnPosition.X += x;
                spawnPosition.Y += y;

                Prefabs.Instantiate(PrefabIds[id], spawnPosition, Utils.Random(0f, 6f));
            }
        }
    }
}