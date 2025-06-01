using System;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public interface IInputManager
    {
        event Action<bool> OnJumpPressed;
        event Action<Vector2> OnMovementPressed;
        event Action<bool> OnRunPressed;
    }
}