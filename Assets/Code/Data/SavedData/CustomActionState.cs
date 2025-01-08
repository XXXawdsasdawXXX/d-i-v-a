using System;

namespace Code.Data
{
    [Serializable]
    public class CustomActionState
    {
        public ECustomCutsceneActionType Type;
        public bool isActive;
    }
}