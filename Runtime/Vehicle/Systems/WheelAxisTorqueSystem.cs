using DesertImage.ECS;

namespace Game.Vehicle
{
    public struct WheelAxisTorqueSystem : IInitialize, IExecute
    {
        private EntitiesGroup _group;
        private EntitiesGroup _toWheelsGroup;

        public void Initialize(in World world)
        {
            _group = Filter.Create(world)
                .With<Vehicle>()
                .With<Engine>()
                .With<Gearbox>()
                .Find();

            _toWheelsGroup = Filter.Create(world)
                .With<WheelAxis>()
                .Find();
        }

        public void Execute(ref SystemsContext context)
        {
            var vehicleList = _toWheelsGroup.GetComponents<Vehicle>();
            var engineList = _toWheelsGroup.GetComponents<Engine>();
            var gearboxList = _toWheelsGroup.GetComponents<Gearbox>();
            var gasList = _toWheelsGroup.GetComponents<Gas>();
            var wheelAxisList = _toWheelsGroup.GetComponents<WheelAxis>();

            foreach (var entityId in _group)
            {
                var vehicle = vehicleList.Read(entityId);
                var engine = engineList.Read(entityId);
                var gas = gasList.Read(entityId).Value;
                var totalGearRatio = gearboxList.Read(entityId).TotalGearRatio;

                for (var i = 0; i < vehicle.WheelAxis.Count; i++)
                {
                    var axisEntity = vehicle.WheelAxis[i];

                    ref var axis = ref wheelAxisList.Get(axisEntity.Id);
                    axis.Torque = gas * engine.Torque * totalGearRatio * axis.DriveRatio;
                }
            }

            foreach (var entityId in _toWheelsGroup)
            {
                var wheelAxis = wheelAxisList.Read(entityId);

                // if (wheelAxis.Torque == 0f) continue;

                var torque = wheelAxis.Torque * .5f; //divided by 2 because split between wheels

                wheelAxis.Left.Replace(new DriveWheel { Torque = torque });
                wheelAxis.Right.Replace(new DriveWheel { Torque = torque });
            }
        }
    }
}