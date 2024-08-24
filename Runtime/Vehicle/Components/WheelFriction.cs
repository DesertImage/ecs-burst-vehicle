using Unity.Mathematics;

namespace Game.Vehicle
{
    public struct WheelFriction
    {
        public float2 Slips;
        public float CombinedSlip;
        
        public Curve FrictionCurve;

        public float CorneringStiffness;
        public float ForwardStiffness;
    }
}