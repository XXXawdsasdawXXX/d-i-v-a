using Code.Data;
using Code.Entities.Common;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Infrastructure.Services.Interactions
{
    [Preserve]
    public class InteractionObserver_ClickOnCharacter : InteractionObserver, IInitListener, ISubscriber
    {
        [Header("Observer components")] 
        private ColliderButton _characterCollisionButton;
        private DivaLiveStatesAnalytic _divaState;

        [Header("Static data")] 
        private InteractionStorage _interactionStorage;

        public UniTask GameInitialize()
        {
            //Observer components
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _characterCollisionButton = diva.FindCommonComponent<ColliderButton>();
            _divaState = diva.FindCharacterComponent<DivaLiveStatesAnalytic>();

            //Static data
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            
#if DEBUGGING
            Log.Info(this, "[GameInit]", Log.Type.Interaction);
#endif
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _characterCollisionButton.SeriesOfClicksEvent += _onClickSeries;
        }

        public void Unsubscribe()
        {
            _characterCollisionButton.SeriesOfClicksEvent -= _onClickSeries;
        }
     

        private void _onClickSeries(int click)
        {
#if DEBUGGING
            Log.Info(this, $"[_onClickSeries] Click #{click}.", Log.Type.Interaction);
#endif
            if (click == 1)
            {
#if DEBUGGING
                Log.Info(this, "[_onClickSeries] Click good series.", Log.Type.Interaction);
#endif
                _interactionStorage.Add(EInteractionType.Good);
            
                InvokeInteractionEvent();
            }
            else if (click < 3)
            {
                if (_divaState.TryGetLowerSate(out ELiveStateKey lowerKey, out float _) && lowerKey == ELiveStateKey.Sleep)
                {
                    _interactionStorage.Add(EInteractionType.Bad);
#if DEBUGGING
                    Log.Info(this, "[_onClickSeries] Click bad series.", Log.Type.Interaction);
#endif
                }
                else
                {
                    _interactionStorage.Add(EInteractionType.Normal);
#if DEBUGGING
                    Log.Info(this, "[_onClickSeries] Click normal series", Log.Type.Interaction);
#endif
                }

                InvokeInteractionEvent();
            }
            else
            {
#if DEBUGGING
                Log.Info(this, "[_onClickSeries] Click bad series.", Log.Type.Interaction);
#endif
                _interactionStorage.Add(EInteractionType.Bad);
            
                InvokeInteractionEvent();
            }
        }

    }
}