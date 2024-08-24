using DesertImage.ECS;

namespace Game.Vehicle
{
    public struct WheelHandBrakeSystem : IInitialize, IExecute
    {
        private EntitiesGroup _group;

        public void Initialize(in World world)
        {
            _group = Filter.Create(world)
                .With<Wheel>()
                .With<Brakes>()
                .Find();
        }

        public void Execute(ref SystemsContext context)
        {
            var wheels = _group.GetComponents<Wheel>();
            var brakeList = _group.GetComponents<Brakes>();

            foreach (var entityId in _group)
            {
                ref var wheel = ref wheels.Get(entityId);
                var brakes = brakeList.Read(entityId);

                var handBrakeTorque = brakes.HandBrakeTorque * brakes.HandBrakeInput;

                if (handBrakeTorque <= 0) continue;

                wheel.AngularVelocity = 0f;
            }
        }
    }
}