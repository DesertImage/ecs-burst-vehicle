using DesertImage.ECS;
using Unity.Mathematics;

namespace Game.Vehicle
{
    public struct WheelVelocitySystem : IInitialize, IExecute
    {
        private EntitiesGroup _group;

        public void Initialize(in World world)
        {
            _group = Filter.Create(world)
                .With<Wheel>()
                .With<WheelVelocity>()
                .With<WheelContact>()
                .With<Position>()
                .With<Rotation>()
                .Find();
        }

        public void Execute(ref SystemsContext context)
        {
            var wheels = _group.GetComponents<Wheel>();
            var wheelVelocities = _group.GetComponents<WheelVelocity>();
            var wheelContacts = _group.GetComponents<WheelContact>();
            var positions = _group.GetComponents<Position>();
            var rotations = _group.GetComponents<Rotation>();

            foreach (var entityId in _group)
            {
                ref var velocity = ref wheelVelocities.Get(entityId);
                var hit = wheelContacts.Read(entityId).Value;
                var rigidbody = wheels.Read(entityId).Rigidbody.Value;

                var position = positions.Read(entityId).Value;
                var rotation = rotations.Get(entityId).Value;

                var worldVelocity = rigidbody.GetPointVelocity(position);

                var right = math.mul(rotation, math.right());
                var forward = math.normalize(math.cross(right, hit.normal));

                velocity.Value = worldVelocity;
                velocity.Forward = math.dot(forward, worldVelocity);
                velocity.Side = math.dot(right, worldVelocity);
            }
        }
    }
}