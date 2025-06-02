using System;

namespace SpookyCore.Runtime.UI
{
    public class Observable<T>
    {
        private T _value;
        public event Action<T> OnValueChanged;
        public bool HasValue => _value != null;
        private bool _bypassValueChangeCheck;

        public Observable(T value, bool bypassValueChangeCheck = false)
        {
            _value = value;
            _bypassValueChangeCheck = bypassValueChangeCheck;
        }

        public T Value
        {
            get => _value;
            set
            {
                if (!_bypassValueChangeCheck && Equals(_value, value)) return;
                
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }

        /// <summary>
        /// Invoke this callback if the observable has a value. Else add this callback to OnValueChanged.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool TryInvoke(Action<T> callback)
        {
            if (HasValue)
            {
                callback?.Invoke(Value);
                return true;
            }
            Subscribe(callback);
            return false;
        }
        
        public void Subscribe(Action<T> action)
        {
            OnValueChanged += action;
        }

        public void Unsubscribe(Action<T> action)
        {
            OnValueChanged -= action;
        }

        public void UnsubscribeAll()
        {
            //might be wrong
            OnValueChanged = null;
        }
    }
}