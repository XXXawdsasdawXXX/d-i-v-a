using System.Collections.Generic;
using Code.Infrastructure.ServiceLocator;

namespace Code.Data
{
    public class WhiteBoard_Hand : IStorage
    {
        public enum Type
        {
            None,
            IsHidden,

            //IsHoldingObject,
            IsShowApple
        }

        private readonly Dictionary<Type, object> _dataDictionary = new();

        public bool TryGetData<T>(Type type, out T data)
        {
            if (_dataDictionary.TryGetValue(type, out object result) && result is T value)
            {
                data = value;
                return true;
            }

            data = default;
            return false;
        }

        public void SetData(Type type, object data)
        {
            if (_dataDictionary.ContainsKey(type))
            {
                _dataDictionary[type] = data;
            }
            else
            {
                _dataDictionary.Add(type, data);
            }
        }
    }
}