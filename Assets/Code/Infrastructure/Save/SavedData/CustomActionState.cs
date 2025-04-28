using System;
using Code.Game.CustomActions;

namespace Code.Infrastructure.Save
{
    [Serializable]
    public class CustomActionState
    {
        public ECustomCutsceneActionType Type;
        public bool isActive;
    }
}