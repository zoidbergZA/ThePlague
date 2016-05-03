using System;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    public class PhysicsHelper
    {
        public static void BuildBody(PhysicalSprite physicalSprite, ShapeType shapeType, BodyType bodyType, World world,
            Vector2 rootPosition, float spawnRotation)
        {
            if (shapeType == ShapeType.Unknown)
            {
                //MakePolyBody(physicalSprite, world, rootPosition, spawnRotation, bodyType);
            }

            else
            {
                physicalSprite.body = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(rootPosition));
                physicalSprite.body.BodyType = bodyType;

                Shape shape = MakeShape(shapeType, physicalSprite.width, physicalSprite.height);

                //Fix the body and shape together using a Fixture object
                Fixture fixture = physicalSprite.body.CreateFixture(shape, physicalSprite);

                physicalSprite.fixtures.Add(fixture);

                physicalSprite.body.Rotation = spawnRotation;
            }
        }

        //public static void MakePolyBody(PhysicalSprite physicalSprite, World world, Vector2 rootPosition, float spawnRotation, BodyType bodyType)
        //{
        //    // make body
        //    physicalSprite.body = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(rootPosition));
        //    physicalSprite.body.BodyType = bodyType;

        //    uint[] data = physicalSprite.texture.GetData();

        //    //Find the vertices that makes up the outline of the shape in the texture
        //    Vertices verts = PolygonTools.CreatePolygon(data, physicalSprite.texture.width, true);

        //    //For now we need to scale the vertices (result is in pixels, we use meters)

        //    Vector2 scale = new Vector2(0.035f, 0.035f);

        //    verts.Scale(ref scale);

        //    var _list = FarseerPhysics.Common.Decomposition.Triangulate.ConvexPartition(verts, FarseerPhysics.Common.Decomposition.TriangulationAlgorithm.Bayazit);

        //    //Create a single body with multiple fixtures

        //    List<Fixture> compound = FixtureFactory.AttachCompoundPolygon(_list, 5f, physicalSprite.body);

        //    foreach (Fixture f in compound)
        //    {
        //        physicalSprite.fixtures.Add(f);
        //    }

        //    compound[0].Body.BodyType = bodyType;

        //}

        public static Shape MakeShape(ShapeType shapeType, float width, float height)
        {
            Shape newShape = null;

            switch (shapeType)
            {
                case ShapeType.Circle:

                    float radius = height;

                    newShape = new CircleShape(ConvertUnits.ToSimUnits(radius*0.5), 5.0f);
                    break;

                case ShapeType.Polygon:
                    // build the polygon shape
                    var spriteRect = new Vector2[4];

                    spriteRect[0] = new Vector2(- (width/2), - (height/2));
                    spriteRect[1] = new Vector2(width/2, - (height/2));
                    spriteRect[2] = new Vector2(width/2, height/2);
                    spriteRect[3] = new Vector2(- (width/2), height/2);

                    var vertices = new Vertices();

                    foreach (Vector2 v in spriteRect)
                    {
                        var vertex = new Vector2(ConvertUnits.ToSimUnits(v.X), ConvertUnits.ToSimUnits(v.Y));
                        //Vector2 vertex = new Vector2(v.X, v.Y);

                        vertices.Add(vertex);
                    }

                    newShape = new PolygonShape(vertices, 5.0f);
                    break;

                default:
                    throw new Exception("Unhandled physics shape!");
            }

            return newShape;
        }
    }
}