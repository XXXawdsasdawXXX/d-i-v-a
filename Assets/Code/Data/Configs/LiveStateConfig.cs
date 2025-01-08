using System.Linq;
using UnityEngine;

namespace Code.Data
{
    [CreateAssetMenu(fileName = "LiveStateConfig", menuName = "Configs/Live State config")]
    public class LiveStateConfig : ScriptableObject
    {
        public LiveStateRangePercentageValue Awakening;

        [SerializeField] private LiveStateStaticParam[] _liveStateStaticParams;

        public LiveStateStaticParam GetStaticParam(ELiveStateKey key)
        {
            return _liveStateStaticParams.FirstOrDefault(d => d.Key == key);
        }
    }
}