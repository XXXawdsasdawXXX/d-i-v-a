using System;
using Code.Infrastructure.CustomActions;

namespace Code.Data.SavedData
{
    [Serializable]
    public class CustomActionState
    {
        public CustomCutsceneActionType Type;
        public bool isActive;
    }
}