using DesertImage.ECS;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Vehicle
{
    public struct WheelFrictionSystem : IInitialize, IExecute
    {
        private EntitiesGroup _group;

        public void Initialize(in World world)
        {
            _group = Filter.Create(world)
                .With<Wheel>()
                .With<WheelVelocity>()
                .With<WheelFriction>()
                .With<Suspension>()
                .With<Rotation>()
                .With<WheelContact>()
                .Find();
        }

        public void Execute(ref SystemsContext context)
        {
            var deltaTime = context.DeltaTime;

            var wheels = _group.GetComponents<Wheel>();
            var wheelVelocitites = _group.GetComponents<WheelVelocity>();
            var wheelFrictions = _group.GetComponents<WheelFriction>();
            var suspensions = _group.GetComponents<Suspension>();
            var rotations = _group.GetComponents<Rotation>();
            var groundContacts = _group.GetComponents<WheelContact>();

            foreach (var entityId in _group)
            {
                var entity = _group.GetEntity(entityId);

                var view = entity.Read<View>().Value.Value;

                ref var wheel = ref wheels.Get(entityId);
                ref var friction = ref wheelFrictions.Get(entityId);

                var velocity = wheelVelocitites.Read(entityId);
                var suspensionForce = suspensions.Read(entityId).Force;
                var rigidbody = wheels.Read(entityId).Rigidbody.Value;
                var hit = groundContacts.Read(entityId).Value;
                var rotation = rotations.Read(entityId).Value;

                if (suspensionForce <= 0f) continue;

                var wheelRadius = wheel.Radius;

                var lateralVelocity = velocity.Side;

                var wheelDelta = wheel.AngularVelocity.AngularToLinear(wheelRadius) - velocity.Forward;
                var longVelocity = wheelDelta;
                var longVelocitySign = math.sign(longVelocity);
                var longVelocityAbs = longVelocitySign * longVelocity;

                friction.Slips = new float2
                (
                    lateralVelocity,
                    longVelocity
                );

                friction.Slips = new float2
                (
                    lateralVelocity * friction.CorneringStiffness,
                    math.clamp(longVelocity * friction.ForwardStiffness, -100f, 100f)
                );

                var frictionRate = friction.FrictionCurve.Evaluate(friction.CombinedSlip);

                var length = math.length(friction.Slips);

                friction.CombinedSlip = length;

                if (length > 1f)
                {
                    friction.Slips = math.normalize(friction.Slips);
                }

                //lateral
                var lateralDirection = math.mul(rotation, math.right());
                var tireForce = frictionRate * friction.Slips * suspensionForce;
                var lateralForce = -tireForce.x * lateralDirection;

                //longitudinal
                var longDirection = math.normalize(math.cross(lateralDirection, hit.normal));
                var longForce = tireForce.y * longDirection;

                if (longVelocityAbs > 1f)
                {
                    wheel.AngularVelocity -=
                        longVelocitySign * (longVelocityAbs * frictionRate).LinearToAngular(wheel.Radius);
                }
                else
                {
                    wheel.AngularVelocity -= (wheelDelta * frictionRate).LinearToAngular(wheel.Radius);
                }

                rigidbody.AddForceAtPosition
                (
                    (longForce + lateralForce) * deltaTime,
                    hit.point,
                    ForceMode.Impulse
                );
            }
        }
    }
}