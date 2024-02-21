using System.Linq;
using Code.Data.Enums;
using Code.Data.StaticData;
using Code.Data.Value;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "LiveStateConfig", menuName = "Configs/Live State config")]
    public class LiveStateConfig : ScriptableObject
    {
        public LiveStateRangePercentageValue Awakening;

        [SerializeField] private LiveStateStaticParam[] _liveStateStaticParams;

        public LiveStateStaticParam GetStaticParam(LiveStateKey key)
        {
            return _liveStateStaticParams.FirstOrDefault(d => d.Key == key);
        }
    }
}