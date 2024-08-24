using DesertImage.ECS;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Vehicle
{
    public struct WheelSteeringSystem : IInitialize, IExecute
    {
        private EntitiesGroup _group;

        public void Initialize(in World world)
        {
            _group = Filter.Create(world)
                .With<Wheel>()
                .With<Steering>()
                // .With<SteeringWheel>()
                .With<View>()
                .Find();
        }

        public void Execute(ref SystemsContext context)
        {
            var steerings = _group.GetComponents<Steering>();
            var steeringWheels = _group.GetComponents<SteeringWheel>();
            var views = _group.GetComponents<View>();

            foreach (var entityId in _group)
            {
                var view = views.Read(entityId);
                var steering = steerings.Read(entityId).Value;
                var steeringWheel = steeringWheels.Read(entityId);

                var transform = view.Value.Value.transform;

                var localRotation = transform.localRotation.eulerAngles;
                localRotation.y = math.lerp(0f, steeringWheel.MaxAngle, steering);
                transform.localRotation = Quaternion.Euler(localRotation);
            }
        }
    }
}