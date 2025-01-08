using System;
using System.Collections;
using Code.Data;
using Code.Entities.Items;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Entities.Diva
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
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _animationAnalytic = diva.FindCharacterComponent<DivaAnimationAnalytic>();
            _divaAnimator = diva.FindCharacterComponent<DivaAnimator>();
            _modeAdapter = diva.FindCharacterComponent<DivaModeAdapter>();
        }

        public void StartReactionToObject(ItemEntity item, Action OnEndReaction = null)
        {
            _divaAnimator.StartPlayEat();

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            
            _coroutine = StartCoroutine(Use(item, OnEndReaction));
        }

        private IEnumerator Use(ItemEntity item, Action OnEndReaction = null)
        {
            item.Lock();
            
            WaitForEndOfFrame period = new();
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