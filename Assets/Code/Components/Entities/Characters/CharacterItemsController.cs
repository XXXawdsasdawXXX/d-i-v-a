using System;
using System.Collections;
using Code.Components.Items;
using Code.Data.Enums;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Entities.Characters
{
    public class CharacterItemsController : CharacterComponent,
        IGameInitListener
    {
        [Header("Components")] 
        private CharacterAnimationAnalytic _animationAnalytic;
        private CharacterAnimator _characterAnimator;
        private CharacterModeAdapter _modeAdapter;
        
        private Coroutine _coroutine;
        public event Action<LiveStatePercentageValue[]> OnItemUsed;

        public void GameInit()
        {
            DIVA diva = Container.Instance.FindEntity<DIVA>();
            _animationAnalytic = diva.FindCharacterComponent<CharacterAnimationAnalytic>();
            _characterAnimator = diva.FindCharacterComponent<CharacterAnimator>();
            _modeAdapter = diva.FindCharacterComponent<CharacterModeAdapter>();
        }

        public void StartReactionToObject(Item item, Action OnEndReaction = null)
        {
            _characterAnimator.StartPlayEat();

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            
            _coroutine = StartCoroutine(Use(item, OnEndReaction));
        }

        private IEnumerator Use(Item item, Action OnEndReaction = null)
        {
            item.Lock();
            
            WaitForEndOfFrame period = new WaitForEndOfFrame();
            Vector3 handPosition = _modeAdapter.GetWorldEatPoint();

            while (Vector3.Distance(item.transform.position, handPosition) > 0.05f)
            {
                item.transform.position =Vector3.Lerp(item.transform.position, handPosition, 3 * Time.deltaTime);   
                yield return period;
            }

            yield return new WaitUntil(() => _animationAnalytic.CurrentState == CharacterAnimationState.Eat);
            
            item.Use(onCompleted: () =>
            {
                _characterAnimator.StopPlayEat();
                OnItemUsed?.Invoke(item.Data.BonusValues.Values);
                OnEndReaction?.Invoke();
            });
        }
    }
}