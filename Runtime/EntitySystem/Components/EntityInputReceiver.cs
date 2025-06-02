using UnityEngine;

namespace SpookyCore.Runtime.EntitySystem
{
    public class EntityInputReceiver : EntityComponent
    {
        [field: SerializeField] public Vector2 MoveInput { get; private set; }
        [field: SerializeField] public bool JumpPressed { get; private set; }
        [field: SerializeField] public bool JumpHeld { get; private set; }
        [field: SerializeField] public bool RunHeld { get; private set; }
        [field: SerializeField] public bool DashPressed { get; private set; }
        
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
                ReadInput();
            }
        }

        #endregion

        #region Public Methods

        public virtual void ResetJump() => JumpPressed = false;

        #endregion
        
        #region Private Methods

        protected virtual void ReadInput()
        {
            var x = Input.GetAxisRaw("Horizontal");
            var y = _verticalMovementEnabled ? Input.GetAxisRaw("Vertical") : 0f;
            MoveInput = new Vector2(x, y).normalized;

            JumpPressed = Input.GetButtonDown("Jump");
            JumpHeld = Input.GetButton("Jump"); //For variable jump height
            RunHeld = Input.GetKey(KeyCode.LeftShift);
            DashPressed = Input.GetKeyDown(KeyCode.K);
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