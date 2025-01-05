using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.Pools;
using UnityEngine;

namespace Code.Test
{
    [Serializable]
    public class DW_Param
    {
        [SerializeField] private MonoPool<DW_ParamTab> _tabsPool;

        private LiveStateStorage _storage;

        private Dictionary<ELiveStateKey, DW_ParamTab> _paramTabs;
        
        public Task Initialize()
        {
            _storage = Container.Instance.FindStorage<LiveStateStorage>();

            _paramTabs = new Dictionary<ELiveStateKey, DW_ParamTab>();
            
            foreach ((ELiveStateKey key, CharacterLiveState state) in _storage.LiveStates)
            {
                DW_ParamTab tab = _tabsPool.GetNext();
                
                tab.SetLabel(key.ToString());
                tab.SetValue(state.Current.ToString());
                
                tab.OnPressedIncrease += () =>
                {
                    Debug.Log($"+ {key}");
                    _storage.AddPercentageValue(new LiveStatePercentageValue()
                    {
                        Key = key,
                        Value = 5f
                    });
                };
                
                tab.OnPressedDecrease += () =>
                {
                    Debug.Log($"- {key}");

                    _storage.AddPercentageValue(new LiveStatePercentageValue()
                    {
                        Key = key,
                        Value = -5f
                    });
                };
                
                _paramTabs.Add(key, tab);
            }
            
            return Task.CompletedTask;
        }

        public void Refresh()
        {
            foreach ((ELiveStateKey key, CharacterLiveState state) in _storage.LiveStates)
            {
                _paramTabs[key].SetValue(state.Current.ToString());
            }
        }
        
    }
}