using System.Linq;
using Code.Data.StaticData;
using Code.Data.Value;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "HandConfig", menuName = "Configs/Hand Config")]
    public class HandConfig : ScriptableObject
    {
        public Material LightMaterial;
        public Material DarkMaterial;
        [SerializeField] private InteractionsValueData[] _voidTime;
        [SerializeField] private InteractionsValueData[] _appleDropChance;


        public int GetVoidTime(int dailyInteractionCount)
        {
            return FindIntInArray(_voidTime, dailyInteractionCount);
        }

        public int GetAppleDropChance(int dailyInteractionCount)
        {
            return FindIntInArray(_appleDropChance, dailyInteractionCount);
        }

        private int FindIntInArray(InteractionsValueData[] array, int dailyInteractionCount)
        {
            InteractionsValueData closestData = array.FirstOrDefault(t =>
                t.InteractionsCount.MinValue <= dailyInteractionCount &&
                t.InteractionsCount.MaxValue >= dailyInteractionCount);

            if (closestData == null)
            {
                closestData = array.Aggregate((x, y) =>
                    Mathf.Abs(x.InteractionsCount.MinValue - dailyInteractionCount) <
                    Mathf.Abs(y.InteractionsCount.MinValue - dailyInteractionCount)
                        ? x
                        : y);
            }

            int value = closestData.Value.GetRandomValue();

            return value;
        }

        public int GetLiveTimeTicks()
        {
            return Random.Range(3, 8);
        }
    }
}