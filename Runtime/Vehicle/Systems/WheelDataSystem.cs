using DesertImage.ECS;

namespace Game.Vehicle
{
    public struct WheelDataSystem : IInitialize, IExecute
    {
        private EntitiesGroup _group;

        public void Initialize(in World world)
        {
            _group = Filter.Create(world)
                .With<Wheel>()
                .With<View>()
                .With<Position>()
                .With<Rotation>()
                .Find();
        }

        public void Execute(ref SystemsContext context)
        {
            var viewList = _group.GetComponents<View>();
            var positionList = _group.GetComponents<Position>();
            var rotationList = _group.GetComponents<Rotation>();

            foreach (var entityId in _group)
            {
                var transform = viewList.Read(entityId).Value.Value.transform;
                ref var position = ref positionList.Get(entityId);
                ref var rotation = ref rotationList.Get(entityId);

                position.Value = transform.position;
                rotation.Value = transform.rotation;
            }
        }
    }
}