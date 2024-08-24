using DesertImage.ECS;
using Game.Vehicle;
using UnityEngine;

namespace Game
{
    public class VehicleStarter : EcsStarter
    {
        [SerializeField] private EntityView[] wheelViews;
        [SerializeField] private EntityView vehicleView;

        protected override void Initialize()
        {
            base.Initialize();

            for (var i = 0; i < wheelViews.Length; i++)
            {
                var entity = World.GetNewEntity();

                var view = wheelViews[i];

                entity.Replace(new View { Value = view });
                entity.Replace(new Wheel());
                entity.Replace<WheelVelocity>();

                view.Initialize(entity);

                entity.Replace(new ViewAdd { Value = view.transform });
            }

            vehicleView.Initialize(World.GetNewEntity());
        }

        protected override void InitSystems()
        {
            World.AddFeature<VehicleFeature>();
        }
    }
}