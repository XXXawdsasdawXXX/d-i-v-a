using System;
using System.Collections;
using Code.Components.Items;
using Code.Data.Enums;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Entities
{
    public class DivaItemsController : DivaComponent,
        IGameInitListener
    {
        [Header("Components")] 
        private DivaAnimationAnalytic _animationAnalytic;
        private DivaAnimator _divaAnimator;
        private DivaModeAdapter _modeAdapter;
        
        private Coroutine _coroutine;
        public event Action<LiveStatePercentageValue[]> OnItemUsed;

        public void GameInit()
        {
            Diva diva = Container.Instance.FindEntity<Diva>();
            _animationAnalytic = diva.FindCharacterComponent<DivaAnimationAnalytic>();
            _divaAnimator = diva.FindCharacterComponent<DivaAnimator>();
            _modeAdapter = diva.FindCharacterComponent<DivaModeAdapter>();
        }

        public void StartReactionToObject(Item item, Action OnEndReaction = null)
        {
            _divaAnimator.StartPlayEat();

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

            yield return new WaitUntil(() => _animationAnalytic.CurrentState == EDivaAnimationState.Eat);
            
            item.Use(onCompleted: () =>
            {
                _divaAnimator.StopPlayEat();
                OnItemUsed?.Invoke(item.Data.BonusValues.Values);
                OnEndReaction?.Invoke();
            });
        }
    }
}