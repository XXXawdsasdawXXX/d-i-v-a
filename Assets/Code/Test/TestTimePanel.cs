using System;
using System.Collections.Generic;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Test
{
    public class TestTimePanel : MonoBehaviour
    {
        [SerializeField] private Text _paramText;
        private TimeObserver _timer;
        
        private void Start()
        {
            _timer = Container.Instance.FindService<TimeObserver>();
        }

        private void Update()
        {
            KeyValuePair<float, float> time = _timer.GetTimeState();
            string text = $"{Math.Round(time.Key)}/{Math.Round(time.Value)}={Math.Round(time.Key / time.Value, 1)}";
            _paramText.text = text;
        }
    }
}