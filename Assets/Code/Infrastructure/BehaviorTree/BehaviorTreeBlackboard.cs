using System;
using System.Collections.Generic;
using Code.Components.Character.Params;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree
{
    public class BehaviorTreeBlackboard : MonoBehaviour
    {
        public event Action<BlackboardKey, object> OnVariableChanged;
        public event Action<BlackboardKey, object> OnVariableRemoved;

        private readonly Dictionary<BlackboardKey, object> variables = new();

        public T GetVariable<T>(BlackboardKey key)
        {
            return (T) variables[key];
        }

        public bool TryGetVariable<T>(BlackboardKey key, out T value)
        {
            if (variables.TryGetValue(key, out var result))
            {
                value = (T) result;
                return true;
            }
            
            value = default;
            return false;
        }

        public bool HasVariable(BlackboardKey key)
        {
            return variables.ContainsKey(key);
        }
        
        public void SetVariable(BlackboardKey key, object value)
        {
            variables[key] = value;
            OnVariableChanged?.Invoke(key, value);
        }
        
        
        public void RemoveVariable(BlackboardKey key)
        {
            if (variables.TryGetValue(key, out var value))
            {
                variables.Remove(key);
                OnVariableRemoved?.Invoke(key, value);
            }
        }
    }
}