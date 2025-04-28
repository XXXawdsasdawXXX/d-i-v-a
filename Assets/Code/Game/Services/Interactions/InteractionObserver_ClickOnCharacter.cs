using Code.Data;
using Code.Game.Entities.Common;
using Code.Game.Entities.Diva;
using Code.Game.Services.LiveState;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Game.Services.Interactions
{
    [Preserve]
    public class InteractionObserver_ClickOnCharacter : InteractionObserver, IInitializeListener, ISubscriber
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
            
            Log.Info(this, "[GameInit]", Log.Type.Interaction);

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
            Log.Info(this, $"[_onClickSeries] Click #{click}.", Log.Type.Interaction);
            if (click == 1)
            {
                Log.Info(this, "[_onClickSeries] Click good series.", Log.Type.Interaction);
                _interactionStorage.Add(EInteractionType.Good);
            
                InvokeInteractionEvent();
            }
            else if (click < 3)
            {
                if (_divaState.TryGetLowerSate(out ELiveStateKey lowerKey, out float _) && lowerKey == ELiveStateKey.Sleep)
                {
                    _interactionStorage.Add(EInteractionType.Bad);
                    Log.Info(this, "[_onClickSeries] Click bad series.", Log.Type.Interaction);
                }
                else
                {
                    _interactionStorage.Add(EInteractionType.Normal);
                    Log.Info(this, "[_onClickSeries] Click normal series", Log.Type.Interaction);
                }

                InvokeInteractionEvent();
            }
            else
            {
                Log.Info(this, "[_onClickSeries] Click bad series.", Log.Type.Interaction);
                
                _interactionStorage.Add(EInteractionType.Bad);
            
                InvokeInteractionEvent();
            }
        }

    }
}