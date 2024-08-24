using DesertImage.ECS;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Vehicle
{
    public struct WheelSuspensionSystem : IInitialize, IExecute
    {
        private EntitiesGroup _group;

        public void Initialize(in World world)
        {
            _group = Filter.Create(world)
                .With<Wheel>()
                .With<Suspension>()
                .With<WheelVelocity>()
                .With<Position>()
                .With<Rotation>()
                .Find();
        }

        public void Execute(ref SystemsContext context)
        {
            var wheels = _group.GetComponents<Wheel>();
            var suspensions = _group.GetComponents<Suspension>();
            var wheelVelocitites = _group.GetComponents<WheelVelocity>();
            var positions = _group.GetComponents<Position>();
            var rotations = _group.GetComponents<Rotation>();
            var wheelContacts = _group.GetComponents<WheelContact>();

            foreach (var entityId in _group)
            {
                ref var suspension = ref suspensions.Get(entityId);
                var position = positions.Read(entityId).Value;
                var rotation = rotations.Get(entityId).Value;
                
                var wheelVelocity = wheelVelocitites.Read(entityId);
                var up = math.mul(rotation, math.up());

                var entity = _group.GetEntity(entityId);

                var rigidbody = wheels.Read(entityId).Rigidbody.Value;

                var isInContact = entity.Has<WheelContact>();

                var hit = isInContact ? wheelContacts.Read(entityId).Value : default;

                var offset = suspension.Offset = isInContact ? suspension.Height - hit.distance : 0f;
                var worldVelocity = wheelVelocity.Value;
                var velocity = Vector3.Dot(up, worldVelocity);
                var suspensionForceValue = offset * suspension.Strength - velocity * suspension.Damping;

                suspension.Force = suspensionForceValue;

                if (!isInContact || suspensionForceValue <= 0) continue;

                rigidbody.AddForceAtPosition
                (
                    hit.normal * (suspensionForceValue * context.DeltaTime),
                    position,
                    ForceMode.Impulse
                );
            }
        }
    }
}