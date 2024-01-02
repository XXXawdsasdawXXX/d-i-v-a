using System.Collections;
using Code.Data.StaticData;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Objects
{
    public class Apple : MonoBehaviour, IGameInitListener
    {
        [Header("Components")] 
        [SerializeField] private AppleAnimator _appleAnimator;
        [SerializeField] private Rigidbody2D _rigidbody2D;

        //serialize for test
        [Header("Private values")] 
        [SerializeField] private ItemData _itemData;

        
        private CharacterLiveStateStorage _liveStateStorage;
        private Coroutine _liveCoroutine;

        public void GameInit()
        {
            _liveStateStorage = Container.Instance.FindStorage<CharacterLiveStateStorage>();
        }
        
        public void InitItem(ItemData itemData)
        {
            _itemData = itemData;
            _liveCoroutine = StartCoroutine(StartLiveTimerRoutine());
        }

        public void Use()
        {
            _liveStateStorage.AddValues(_itemData.InfluentialValues);
            StopCoroutine(_liveCoroutine);
        }

        public void Grow()
        {
            _appleAnimator.PlayEnter();
        }

        public void Fall()
        {
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
        private IEnumerator StartLiveTimerRoutine()
        {
            yield return new WaitForSeconds(_itemData.LiveTime);

            _appleAnimator.PlayExit(() =>
            {
                Debugging.Instance.Log("ITEM EXIT", Debugging.Type.Item);
                gameObject.SetActive(false);
            });
        }
    }
}