using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class Prefabs
    {
        public enum PrefabsList
        {
            //--------------------------------------------------------//
            //   These id's match the tile [id - 1] in the .TML file  //
            //--------------------------------------------------------//

            blank,
            Ball,
            Crate,
            Rat,
            Doctor,
            Spikes,
            GasBox,
            Victim,
            stone_middle,
            moss_corner_left,
            moss_middle,
            moss_corner_right,
            stone_corner_left,
            stone_corner_right,
            pillar,
            spiked_wall,
            platform1,
            platform3,
            platform2,
            broken_wall,
            capitel,
            column,
            crate,
            fence,
            gargola,
            moss,
            rail_vertical,
            rail_horizontal,
            torch,
            wall,
            wall_hole,
            broken_wall_2,
            stones
        }

        public static void Instantiate(int id, Vector2 position, float rotation = 0)
        {
            World world = GameManager.Instance.World;
            Level level = GameManager.Instance.Level;
            int layer = 0;
            LevelObject levelObject = null;
            Level.Tile tile = GameManager.Instance.Level.tileset[id - 1];

            string spritePath = tile.source;

            var prefabsList = (PrefabsList) id;

            #region Prefab selector

            switch (prefabsList)
            {
                case PrefabsList.blank:
                    return;
                    break;

                case PrefabsList.Ball:
                    levelObject = new BasicObject(spritePath, ShapeType.Circle, BodyType.Dynamic, world, position,
                        rotation);
                    levelObject.body.Restitution = 0.2f;
                    levelObject.body.Mass *= 25;
                    levelObject.body.AngularDamping = 0.2f;
                    break;

                case PrefabsList.Crate:
                    levelObject = new BasicObject(spritePath, ShapeType.Polygon, BodyType.Dynamic, world, position,
                        rotation);

                    levelObject.body.Mass *= 2.4f;
                    break;

                case PrefabsList.Rat:
                    var monster = new Rat(world, position, rotation);
                    level.AddChildOnLayer(monster, layer);
                    break;

                case PrefabsList.Doctor:
                    Vector2 playerPos = position;
                    level.playerPlacers.Add(playerPos);
                    break;

                case PrefabsList.Spikes:
                    levelObject = new Spikes(spritePath, ShapeType.Polygon, BodyType.Static, world, position, rotation);
                    break;

                case PrefabsList.GasBox:
                    levelObject = new FogBox(spritePath, world, position, rotation);
                    layer = 2;
                    break;

                case PrefabsList.Victim:
//                    Victim victim = new Victim(world, position, rotation);
//                    level.AddChildOnLayer(victim, layer);
                    Vector2 vicPos = position;
                    level.victimPlacers.Add(vicPos);
                    break;

                case PrefabsList.stone_middle:
                    levelObject = new BasicObject(spritePath, ShapeType.Polygon, BodyType.Static, world, position,
                        rotation);
                    break;

                case PrefabsList.moss_corner_left:
                    levelObject = new BasicObject(spritePath, ShapeType.Polygon, BodyType.Static, world, position,
                        rotation);
                    break;

                case PrefabsList.moss_middle:
                    levelObject = new BasicObject(spritePath, ShapeType.Polygon, BodyType.Static, world, position,
                        rotation);
                    break;

                case PrefabsList.moss_corner_right:
                    levelObject = new BasicObject(spritePath, ShapeType.Polygon, BodyType.Static, world, position,
                        rotation);
                    break;

                case PrefabsList.stone_corner_left:
                    levelObject = new BasicObject(spritePath, ShapeType.Polygon, BodyType.Static, world, position,
                        rotation);
                    break;

                case PrefabsList.stone_corner_right:
                    levelObject = new BasicObject(spritePath, ShapeType.Polygon, BodyType.Static, world, position,
                        rotation);
                    break;

                case PrefabsList.pillar:
                    levelObject = new BasicObject(spritePath, ShapeType.Polygon, BodyType.Static, world, position,
                        rotation);
                    break;

                case PrefabsList.spiked_wall:
                    levelObject = new Spikes(spritePath, ShapeType.Polygon, BodyType.Static, world, position,
                        rotation);
                    break;

                case PrefabsList.platform1:
                    levelObject = new MovingPlatform(spritePath, GetMovementRange(tile), ShapeType.Polygon, world,
                        position, rotation);
                    break;

                case PrefabsList.platform3:
                    levelObject = new MovingPlatform(spritePath, GetMovementRange(tile), ShapeType.Polygon, world,
                        position, rotation);
                    break;

                case PrefabsList.platform2:
                    levelObject = new MovingPlatform(spritePath, GetMovementRange(tile), ShapeType.Polygon, world,
                        position, rotation, true);
                    break;

                case PrefabsList.broken_wall:
                    CreateProp(spritePath, position);
                    break;

                case PrefabsList.capitel:
                    CreateProp(spritePath, position, 2);
                    break;

                case PrefabsList.column:
                    CreateProp(spritePath, position, 2);
                    break;

                case PrefabsList.crate:
                    CreateProp(spritePath, position);
                    break;

                case PrefabsList.fence:
                    CreateProp(spritePath, position);
                    break;

                case PrefabsList.gargola:
                    CreateProp(spritePath, position, 2);
                    break;

                case PrefabsList.moss:
                    CreateProp(spritePath, position);
                    break;

                case PrefabsList.rail_vertical:
                    CreateProp(spritePath, position);
                    break;

                case PrefabsList.rail_horizontal:
                    CreateProp(spritePath, position);
                    break;

                case PrefabsList.torch:
                    CreateProp(spritePath, position);
                    break;

                case PrefabsList.wall:
                    CreateProp(spritePath, position);
                    break;

                case PrefabsList.wall_hole:
                    CreateProp(spritePath, position);
                    break;

                case PrefabsList.broken_wall_2:
                    CreateProp(spritePath, position, 2);
                    break;

                case PrefabsList.stones:
                    levelObject = new BasicObject(spritePath, ShapeType.Polygon, BodyType.Static, world, position,
                        rotation);
                    break;
            }

            #endregion

            if (levelObject != null)
            {
                if (tile.properties.Count > 0)
                {
                    //Add tile properties
                    if (tile.properties.ContainsKey("Friction"))
                    {
                        try
                        {
                            levelObject.body.Friction = float.Parse(tile.properties["Friction"]);
                        }
                        catch
                        {
                            levelObject.body.Friction = 0.55f;
                        }
                    }
                    else
                    {
                        levelObject.body.Friction = 0.55f;
                    }
                }

                level.AddChildOnLayer(levelObject, layer);
            }
//            return levelObject;
        }

        private static void CreateProp(string spritePath, Vector2 position, int onLayer = 0)
        {
            var sprite = new Sprite(spritePath);
            sprite.SetXY(position.X, position.Y);
            sprite.SetOrigin(sprite.width/2, sprite.height/2);
            GameManager.Instance.Level.AddChildOnLayer(sprite, onLayer);
        }

        private static MovementRange GetMovementRange(Level.Tile platformTile)
        {
            var range = new MovementRange();

            if (platformTile.properties.ContainsKey("Down"))
                range.down = float.Parse(platformTile.properties["Down"]);
            if (platformTile.properties.ContainsKey("Right"))
                range.right = float.Parse(platformTile.properties["Right"]);

            return range;
        }

        public struct MovementRange
        {
            public float down;
            public float right;
        }
    }
}