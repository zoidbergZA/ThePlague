using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class BasicObject : LevelObject
    {
        public BasicObject(string spritePath, ShapeType shapeType, BodyType bodyType, World world, Vector2 spawnPosition,
            float spawnRotation = 0f)
            : base(spritePath, world, spawnPosition, spawnRotation, shapeType, bodyType)
        {
        }
    }
}