using Code.Game.Services.LiveState;

namespace Code.Infrastructure.Save
{
    public class LiveStateSavedData
    {
        public ELiveStateKey Key;
        public float CurrentValue;
        public bool IsHealing;
    }
}