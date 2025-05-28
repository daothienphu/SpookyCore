using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public class EntityMovement : EntityComponent
    {
        [field: SerializeField] public Vector3 Velocity { get; private set; }
        [field: SerializeField] public bool CanMove { get; private set; } = true;

        public override void OnStart()
        {
            
        }

        public void ToggleMovement(bool isEnabled)
        {
            CanMove = isEnabled;
        }
    }
}