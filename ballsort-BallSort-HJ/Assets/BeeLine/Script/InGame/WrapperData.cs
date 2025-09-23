using System;

namespace Lei31.Utils
{
    public class WrapperData<T>
    {
        public event Action<T, T> OnValueChange = delegate { };

        public T Value
        {
            get => _value;
            set
            {
                if (!_value.Equals(value))
                {
                    T oldValue = _value;
                    _value = value;
                    OnValueChange.Invoke(oldValue, _value);
                }
            }
        }

        public void ResetValue(T value)
        {
            _value = value;
        }

        public WrapperData(T defaultValue)
        {
            _value = defaultValue;
        }

        private T _value;
    }
}