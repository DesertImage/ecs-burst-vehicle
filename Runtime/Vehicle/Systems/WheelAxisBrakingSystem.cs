using DesertImage.ECS;

namespace Game.Vehicle
{
    public struct WheelAxisBrakingSystem : IInitialize, IExecute
    {
        private EntitiesGroup _group;
        private EntitiesGroup _nonHandBrakeGroup;
        private EntitiesGroup _toWheelsGroup;

        public void Initialize(in World world)
        {
            _group = Filter.Create(world)
                .With<Vehicle>()
                .With<Brakes>()
                .Find();
            
            _nonHandBrakeGroup = Filter.Create(world)
                .With<WheelAxis>()
                .With<Brakes>()
                .None<HandBrakeWheelAxis>()
                .Find();
            
            _toWheelsGroup = Filter.Create(world)
                .With<WheelAxis>()
                .With<Brakes>()
                .Find();
        }

        public void Execute(ref SystemsContext context)
        {
            var vehicleList = _toWheelsGroup.GetComponents<Vehicle>();
            var wheelAxisList = _toWheelsGroup.GetComponents<WheelAxis>();
            var brakesList = _toWheelsGroup.GetComponents<Brakes>();

            foreach (var entityId in _group)
            {
                var vehicle = vehicleList.Read(entityId);
                var brakes = brakesList.Read(entityId);

                for (var i = 0; i < vehicle.WheelAxis.Count; i++)
                {
                    var axis = vehicle.WheelAxis[i];
                    axis.Replace(brakes);
                }
            }
            
            foreach (var entityId in _nonHandBrakeGroup)
            {
                ref var brakes = ref brakesList.Get(entityId);
                brakes.HandBrakeInput = 0f;
            }
            
            foreach (var entityId in _toWheelsGroup)
            {
                var wheelAxis = wheelAxisList.Read(entityId);
                var brake = brakesList.Read(entityId);

                wheelAxis.Left.Replace(brake);
                wheelAxis.Right.Replace(brake);
            }
        }
    }
}