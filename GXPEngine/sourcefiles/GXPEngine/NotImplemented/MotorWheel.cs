using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace GXPEngine
{
    internal class MotorWheel : LevelObject
    {
        private static string spritePath = "../Sprites/wheel.png";
        private static ShapeType shapeType = ShapeType.Circle;
        private static BodyType bodyType = BodyType.Dynamic;

        public MotorWheel(World world, Vector2 spawnPosition, float spawnRotation)
            : base(spritePath, world, spawnPosition, spawnRotation, shapeType, bodyType)
        {
            Body motorPaddleAxle = BodyFactory.CreateCircle(world, 0.1f, 1f);

            RevoluteJoint j = JointFactory.CreateRevoluteJoint
                (
                    world,
                    body,
                    new Vector2(0.0f, 0.0f),
                    motorPaddleAxle,
                    body.Position
                );

            // set speed and torque
            j.MotorSpeed = MathHelper.Pi;
            j.MotorImpulse = 20;
            j.MotorEnabled = true;
            j.MaxMotorTorque = 20;
        }
    }
}