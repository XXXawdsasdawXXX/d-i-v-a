using System.Collections.Generic;
using Code.Data.Enums;
using Code.Test;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_Nimbus : CustomAction_AudioParticle
    {
        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.Nimbus;
        }


        protected override ParticleType GetParticleType()
        {
            return ParticleType.Nimbus;
        }


        protected override void UpdateParticles()
        {
            if (_isNotUsed || _particlesSystems == null)
            {
                Debugging.Instance.Log("=(((((");
                return;
            }

            foreach (var particle in _particlesSystems)
            {
                if (!particle.IsPlay) continue;
                particle.transform.position = GetParticlePosition();
            }
        }

        private Vector3 GetParticlePosition()
        {
            return _characterModeAdapter.GetWorldEatPoint() + Vector3.up * 0.5f;
        }
    }
}