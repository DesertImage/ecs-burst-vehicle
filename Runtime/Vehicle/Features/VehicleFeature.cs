using DesertImage.ECS;

namespace Game.Vehicle
{
    public struct VehicleFeature : IFeature
    {
        public void Link(World world)
        {
            new GearboxFeature().Link(world);
            
            world.Add<BrakesInputSystem>(ExecutionOrder.EarlyMainThread);
            world.Add<GasInputSystem>(ExecutionOrder.EarlyMainThread);
            world.Add<SteeringInputSystem>(ExecutionOrder.EarlyMainThread);

            world.Add<WheelDataSystem>(ExecutionOrder.EarlyMainThread);

            world.Add<EngineSystem>();

            world.Add<WheelContactSystem>(ExecutionOrder.Physics);
            world.Add<WheelVelocitySystem>(ExecutionOrder.Physics);
            world.Add<WheelSuspensionSystem>(ExecutionOrder.Physics);
            world.Add<VehicleMaxWheelsAngularVelocitySystem>(ExecutionOrder.Physics);
            world.Add<WheelAxisSteeringSystem>(ExecutionOrder.Physics);
            world.Add<WheelAxisTorqueSystem>(ExecutionOrder.Physics);
            world.Add<WheelAxisBrakingSystem>(ExecutionOrder.Physics);
            world.Add<WheelSteeringSystem>(ExecutionOrder.Physics);
            world.Add<WheelsTorqueSystem>(ExecutionOrder.Physics);

            world.Add<WheelBrakeCalculateSystem>(ExecutionOrder.Physics);
            world.Add<WheelBrakeApplySystem>(ExecutionOrder.Physics);

            world.Add<WheelFrictionSystem>(ExecutionOrder.Physics);

            world.Add<WheelHandBrakeSystem>(ExecutionOrder.Physics);

            world.Add<WheelRotationSystem>(ExecutionOrder.Physics);
            world.Add<WheelVisualizeSystem>(ExecutionOrder.Physics);
            
            world.Add<VehicleBodyShakingSystem>(ExecutionOrder.LateMainThread);
        }
    }
}