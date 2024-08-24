using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Game.Vehicle
{
    public static class WheelExtensions
    {
        private const double RadToRpmMultiplier = 60 / math.PI_DBL;
        private const double RpmToRadMultiplier = math.PI_DBL * .01666666f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LinearToAngular(this float distance, float radius) => distance / radius;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngularToLinear(this float distance, float radius) => distance * radius;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float TorqueToForce(this float torque, float radius) => torque / radius;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ForceToTorque(this float force, float radius) => force * radius;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RpmToRadiansPerSecond(this float rpm) => (float)(rpm * RpmToRadMultiplier);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadiansPerSecondToRpm(this float radians) => (float)(radians * RadToRpmMultiplier);
    }
}