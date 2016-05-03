using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    public abstract class LevelObject : PhysicalSprite
    {
        public LevelObject(string spritePath, World world, Vector2 spawnPosition, float spawnRotation,
            ShapeType shapeType, BodyType bodyType, int spriteCols = 1, int spriteRows = 1)
            : base(spritePath, spawnPosition, spriteCols, spriteRows)
        {
            PhysicsHelper.BuildBody(this, shapeType, bodyType, world, spawnPosition, spawnRotation);
        }

        public virtual void Update()
        {
            SyncTransforms();
        }

        public override void Destroy()
        {
            base.Destroy();
            game.Remove(this);
        }

        public virtual void Interact()
        {
        }

        public struct PrefabFields
        {
            public BodyType bodyType;
            public ShapeType shapeType;
            public string spritePath;
        }
    }
}