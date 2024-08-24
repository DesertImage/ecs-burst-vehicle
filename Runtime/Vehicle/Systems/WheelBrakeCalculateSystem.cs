using DesertImage.ECS;
using Unity.Mathematics;

namespace Game.Vehicle
{
    public struct WheelBrakeCalculateSystem : IInitialize, IExecute
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

            var deltaTime = context.DeltaTime;

            foreach (var entityId in _group)
            {
                ref var wheel = ref wheels.Get(entityId);
                var brakes = brakeList.Read(entityId);

                var brakesTorque = brakes.Torque * brakes.Input;
                var handBrakesTorque = brakes.HandBrakeTorque * brakes.HandBrakeInput;
                
                var brakeTorque = brakesTorque + handBrakesTorque;

                if (brakeTorque <= 0) continue;

                var wheelInertia = wheel.Inertia;
                var toZeroTorque = -wheel.AngularVelocity * wheelInertia / deltaTime;
                var toZeroTorqueAbs = math.abs(toZeroTorque);
                var usedTorque = toZeroTorqueAbs < brakeTorque ? toZeroTorqueAbs : brakeTorque;

                wheel.AngularVelocity += math.sign(toZeroTorque) * usedTorque / wheelInertia * deltaTime;
            }
        }
    }
}