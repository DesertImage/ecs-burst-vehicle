using DesertImage.ECS;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Vehicle
{
    public struct WheelContactSystem : IInitialize, IExecute, IDrawGizmos
    {
        private EntitiesGroup _group;

        public void Initialize(in World world)
        {
            _group = Filter.Create(world)
                .With<Position>()
                .With<Rotation>()
                .With<Suspension>()
                .Find();
        }

        public void Execute(ref SystemsContext context)
        {
            var positions = _group.GetComponents<Position>();
            var rotations = _group.GetComponents<Rotation>();
            var suspensions = _group.GetComponents<Suspension>();

            foreach (var entityId in _group)
            {
                var suspension = suspensions.Read(entityId);
                var position = positions.Read(entityId).Value;
                var rotation = rotations.Read(entityId).Value;

                var entity = _group.GetEntity(entityId);

                var up = math.mul(rotation, math.up());

                if (!UnityEngine.Physics.Raycast(position, -up, out var hit, suspension.Height))
                {
                    if (entity.Has<WheelContact>())
                    {
                        entity.Remove<WheelContact>();
                    }

                    continue;
                }

                entity.Replace(new WheelContact { Value = hit });
            }
        }

        public void DrawGizmos(in World world)
        {
            var suspensions = _group.GetComponents<Suspension>();
            var positions = _group.GetComponents<Position>();
            var rotations = _group.GetComponents<Rotation>();

            foreach (var entityId in _group)
            {
                var suspension = suspensions.Read(entityId);

                var originPosition = positions.Read(entityId).Value;
                var rotation = rotations.Read(entityId).Value;
                var up = math.mul(rotation, math.up());

                var hasContact = world.GetEntity(entityId).Has<WheelContact>();

                Debug.DrawLine
                (
                    originPosition,
                    originPosition + -up * suspension.Height,
                    hasContact ? Color.green : Color.red,
                .25f
                );
            }
        }
    }
}