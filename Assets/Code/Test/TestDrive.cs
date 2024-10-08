using System;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Test
{
    public class TestDrive : MonoBehaviour, IGameStartListener
    {
        [SerializeField] private float _timeScale;

        public void GameStart()
        {
            SetTimeScale();
        }

        public void SetTimeScale()
        {
            Time.timeScale = _timeScale;
        }
    }
}