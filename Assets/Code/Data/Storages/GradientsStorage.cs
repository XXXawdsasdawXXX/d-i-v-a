﻿using System;
using System.Linq;
using UnityEngine;

namespace Code.Data
{
    public class GradientsStorage : MonoBehaviour, IStorage
    {
        [SerializeField] private GradientData[] _gradients;

        public bool TryGetGradient(GradientType gradientType, out Gradient gradient)
        {
            GradientData data = _gradients.FirstOrDefault(g => g.Type == gradientType);
            gradient = data?.Gradient;
            return data != null;
        }

        [ContextMenu("Test")]
        public void Test()
        {
            GradientColorKey[] colors = _gradients[0].Gradient.colorKeys;
            foreach (GradientColorKey colorKey in colors)
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