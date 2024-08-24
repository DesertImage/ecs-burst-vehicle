using DesertImage.ECS;

namespace Game.Vehicle
{
    public struct WheelBrakeApplySystem : IInitialize, IExecute
    {
        private EntitiesGroup _group;

        public void Initialize(in World world)
        {
            _group = Filter.Create(world)
                .With<Wheel>()
                .With<BrakingWheel>()
                .Find();
        }

        public void Execute(ref SystemsContext context)
        {
            var wheels = _group.GetComponents<Wheel>();
            var brakingWheelList = _group.GetComponents<BrakingWheel>();

            foreach (var entityId in _group)
            {
                ref var wheel = ref wheels.Get(entityId);
                var brakes = brakingWheelList.Read(entityId);
                wheel.AngularVelocity += brakes.DeltaAngularVelocity;
            }
        }
    }
}