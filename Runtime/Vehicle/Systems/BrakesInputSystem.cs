using DesertImage.ECS;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Vehicle
{
    public struct BrakesInputSystem : IInitialize, IExecute
    {
        private EntitiesGroup _group;

        public void Initialize(in World world)
        {
            _group = Filter.Create(world)
                .With<Brakes>()
                .With<BrakesInput>()
                .Find();
        }

        public void Execute(ref SystemsContext context)
        {
            var brakesPool = _group.GetComponents<Brakes>();

            foreach (var entityId in _group)
            {
                ref var brakes = ref brakesPool.Get(entityId);

                brakes.Input = -math.min(0f, UnityEngine.Input.GetAxis("Vertical"));
                brakes.HandBrakeInput = UnityEngine.Input.GetKey(KeyCode.Space) ? 1f : 0f;
            }
        }
    }
}