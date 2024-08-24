using DesertImage.ECS;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Vehicle
{
    public class WheelView : EntityView
    {
        [SerializeField] private Rigidbody rigidbody;

        [SerializeField] [Space] [Header("Suspension")]
        private float height = .55f;

        [SerializeField] private float strength = 40000f;
        [SerializeField] private float damping = 4000f;

        [SerializeField] [Space] [Header("Wheel")]
        private float radius = .5f;

        [SerializeField] private Transform view;

        [FormerlySerializedAs("Friction")] [FormerlySerializedAs("Side")] [Space] [Header("Friction")] [SerializeField]
        private AnimationCurve friction;

        public override void Initialize(in Entity entity)
        {
            base.Initialize(in entity);

            entity.Replace
            (
                new WheelFriction
                {
                    FrictionCurve = new Curve(entity.CreateBufferList<float, WheelFriction>(128), friction, 128),
                    CorneringStiffness = 1f,
                    ForwardStiffness = 1f,
                }
            );

            const float mass = 20f;

            entity.Replace
            (
                new Wheel
                {
                    Mass = mass,
                    Radius = radius,
                    Inertia = mass * radius * radius * .5f,
                    View = view,
                    AngularVelocity = 1f,
                    Rigidbody = rigidbody
                }
            );

            entity.Replace
            (
                new Suspension
                {
                    Height = height,
                    Strength = strength,
                    Damping = damping
                }
            );
            
            entity.Replace<BrakingWheel>();
        }

        private void OnDrawGizmos()
        {
            if (!Entity.IsAlive()) return;

            var transf = transform;
            var contactPosition = transf.position + -transf.up * Entity.Read<Wheel>().Radius;

            var friction = Entity.Read<WheelFriction>();
            var lateralFriction = friction.Slips.x;
            var longFriction = friction.Slips.y;

            Gizmos.color = Color.red;
            Gizmos.DrawLine
            (
                contactPosition,
                contactPosition + transform.right * lateralFriction
            );

            Gizmos.color = Color.blue;
            Gizmos.DrawLine
            (
                contactPosition,
                contactPosition + transform.forward * longFriction
            );
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!view)
            {
                view = transform.GetChild(0);
            }
        }
    }
}