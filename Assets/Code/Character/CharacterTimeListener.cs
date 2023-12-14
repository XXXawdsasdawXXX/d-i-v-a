﻿using System;
using Code.Character.AnimationReader.Mode;
using UnityEngine;

namespace Code.Character
{
    public class CharacterTimeListener : MonoBehaviour
    {
        [SerializeField] private CharacterAnimator _characterAnimator;

        /*private void Update()
        {
            if (IsNightTime() && _characterAnimator.Mode != CharacterAnimationMode.Sleep)
            {
                _characterAnimator.SetSleepMode();
            }
        }*/

        private bool IsNightTime()
        {
            DateTime currentTime = DateTime.Now;
            int hour = currentTime.Hour;


            return hour >= 21 || hour < 6;
        }
    }
}