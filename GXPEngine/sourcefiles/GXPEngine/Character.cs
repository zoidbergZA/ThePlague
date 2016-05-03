using Microsoft.Xna.Framework;

namespace GXPEngine
{
    public abstract class Character : PhysicalSprite
    {
        public Character(string spritePath, Vector2 position, int cols = 1, int rows = 1)
            : base(spritePath, position, cols, rows)
        {
        }

        public bool Alive { get; protected set; }
        public int Hitpoints { get; protected set; }
        public bool Invulnrable { get; protected set; }
        public Sound DeathSound { get; protected set; }

        public abstract void TakeDamage(int amount);
        public abstract void HandleDeath();
    }
}