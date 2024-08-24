using DesertImage.ECS;
using Unity.Mathematics;

namespace Game.Vehicle
{
    public struct WheelsTorqueSystem : IInitialize, IExecute
    {
        private EntitiesGroup _group;

        public void Initialize(in World world)
        {
            _group = Filter.Create(world)
                .With<Vehicle>()
                .With<Engine>()
                .With<Gearbox>()
                .With<Gas>()
                .Find();
        }

        public void Execute(ref SystemsContext context)
        {
            var vehicles = _group.GetComponents<Vehicle>();
            var allWheels = _group.GetComponents<Wheel>();
            var driveWheels = _group.GetComponents<DriveWheel>();
            var engines = _group.GetComponents<Engine>();
            var gearboxes = _group.GetComponents<Gearbox>();

            var deltaTime = context.DeltaTime;

            foreach (var entityId in _group)
            {
                var vehicle = vehicles.Read(entityId);

                var engine = engines.Read(entityId);
                var gearbox = gearboxes.Read(entityId);
                var totalGearRatio = gearbox.TotalGearRatio;
                var wheels = vehicle.Wheels;

                // Debug.Log($"<color=blue>[WheelTorque]</color> engine torque: {engine.Torque}. Rpm: {engine.Rpm}");
                // Debug.Log($"<color=blue>[WheelTorque]</color> Gearbox total ration: {gearbox.TotalGearRatio}");

                for (var i = 0; i < wheels.Count; i++)
                {
                    var wheelEntity = wheels[i];
                    var wheelEntityId = wheelEntity.Id;

                    if (!wheelEntity.Has<DriveWheel>()) continue;

                    ref var wheel = ref allWheels.Get(wheelEntityId);
                    ref var driveWheel = ref driveWheels.Get(wheelEntityId);

                    // Debug.Log(
                        // $"<color=blue>[WheelTorque]</color> {wheelEntityId} wheelTorque: {driveWheel.Torque}. DriveRation: {driveWheel.DriveRatio}");

                    var angularAcceleration = driveWheel.Torque / wheel.Inertia * deltaTime;

                    var maxWheelSpeedOnCurrentGear =
                        totalGearRatio != 0f ? (engine.AngularVelocity / totalGearRatio) : 9999f;

                    wheel.AngularVelocity += angularAcceleration;

                    var angularVelocityAbs = math.abs(wheel.AngularVelocity);
                    var maxSpeedAbs = math.abs(maxWheelSpeedOnCurrentGear);
                    var maxSpeedSign = math.sign(maxWheelSpeedOnCurrentGear);

                    wheel.AngularVelocity = math.min(angularVelocityAbs, maxSpeedAbs) * maxSpeedSign;

                    // Debug.Log(
                        // $"<color=blue>[WheelTorque]</color> {wheelEntity.Id} wheel angular velocity: {wheel.AngularVelocity}. Rpm: {wheel.AngularVelocity.RadiansPerSecondToRpm()}");
                }
            }
        }
    }
}