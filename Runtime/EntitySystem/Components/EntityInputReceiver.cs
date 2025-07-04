using SpookyCore.Runtime.Systems;
using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public class EntityInputReceiver : EntityComponent
    {
        [field: SerializeField] public Vector2 MoveInput { get; protected set; }
        [field: SerializeField] public bool JumpPressed { get; protected set; }
        [field: SerializeField] public bool JumpHeld { get; protected set; }
        [field: SerializeField] public bool RunHeld { get; protected set; }
        [field: SerializeField] public bool DashPressed { get; protected set; }
        [field: SerializeField] public bool AttackPressed { get; protected set; }

        [Header("Settings")]
        public bool _verticalMovementEnabled;
        public bool _canJump;
        public bool _canDash;
        [SerializeField] protected bool _useNewInputSystem;

        #region Life Cycle
        
        public override void OnStart()
        {
            base.OnStart();
            
            // if (InputManager.Instance)
            // {
            //     InputManager.Instance.OnJumpPressed += OnJumpPressedHandler;
            //     InputManager.Instance.OnMovementPressed += OnMovementPressedHandler;
            //     InputManager.Instance.OnRunPressed += OnRunPressedHandler;
            // }
        }

        private void OnDisable()
        {
            // if (InputManager.Instance)
            // {
            //     InputManager.Instance.OnJumpPressed -= OnJumpPressedHandler;
            //     InputManager.Instance.OnMovementPressed -= OnMovementPressedHandler;
            //     InputManager.Instance.OnRunPressed -= OnRunPressedHandler;
            // }        
        }

        public override void OnUpdate()
        {
            if (!_useNewInputSystem)
            {
                ReadInputOldInputSystem();
            }
        }

        #endregion

        #region Public Methods

        public virtual void ResetJump() => JumpPressed = false;
        
        public virtual Vector2 GetMousePosition(bool worldSpace = true)
        {
            var position = Input.mousePosition;
            return worldSpace
                ? GameManager.Instance.MainCamera.ScreenToWorldPoint(position)
                : position;
        }

        #endregion
        
        #region Private Methods

        protected virtual void ReadInputOldInputSystem()
        {
            var x = Input.GetAxisRaw("Horizontal");
            var y = _verticalMovementEnabled ? Input.GetAxisRaw("Vertical") : 0f;
            MoveInput = new Vector2(x, y).normalized;

            JumpPressed = Input.GetButtonDown("Jump");
            JumpHeld = Input.GetButton("Jump"); //For variable jump height
            RunHeld = Input.GetKey(KeyCode.LeftShift);
            DashPressed = Input.GetKeyDown(KeyCode.K);
            AttackPressed = Input.GetButtonDown("Fire1");
        }
        
        protected virtual void OnMovementPressedHandler(Vector2 movement)
        {
            MoveInput = movement;
        }
        protected virtual void OnRunPressedHandler(bool pressed)
        {
            RunHeld = pressed;
        }
        protected virtual void OnJumpPressedHandler(bool pressed)
        {
            JumpPressed = pressed;
        }
        protected virtual void OnDashPressedHandler(bool pressed)
        {
            DashPressed = pressed;
        }

        #endregion
    }
}