using System.Linq;
using Code.Components.Character;
using Code.Data.Enums;
using Code.Data.StaticData;
using Code.Data.Value;
using Code.Data.Value.RangeFloat;
using Code.Utils;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "Configs/Character config")]
    public class CharacterConfig : ScriptableObject
    {

        [SerializeField] private LiveStateStaticParam[] _liveStateStaticParams;


        
        public LiveStateStaticParam GetStaticParam(LiveStateKey key)
        {
            return _liveStateStaticParams.FirstOrDefault(d => d.Key == key);
        }
        
        public float GetDecreasingValue(LiveStateKey key)
        {
            var data = _liveStateStaticParams.FirstOrDefault(d => d.Key == key);
            if (data == null)
            {
                Debugging.Instance.ErrorLog($"");
            }
            return data?.DecreasingValue ?? 0;
        }

  
    }
}