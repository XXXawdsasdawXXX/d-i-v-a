using System;
using System.Linq;
using Code.Data.Interfaces;
using UnityEngine;

namespace Code.Data.Storages
{
    public class GradientsDictionary : MonoBehaviour, IService
    {
        [SerializeField] private GradientData[] _gradients;

        public bool TryGetGradient(GradientType gradientType, out Gradient gradient)
        {
            var data = _gradients.FirstOrDefault(g => g.Type == gradientType);
            gradient = data?.Gradient;
            return data != null;
        }

        [ContextMenu("Test")]
        public void Test()
        {
            var colors = _gradients[0].Gradient.colorKeys;
            foreach (var colorKey in colors)
            {
                Debug.Log(colorKey.color.ToString());
            }
        }
    }

    [Serializable]
    public class GradientData
    {
        public GradientType Type;
        public Gradient Gradient;
    }

    public enum GradientType
    {
        None,
        SoftBlue,
        WhiteGhost,
    }
}