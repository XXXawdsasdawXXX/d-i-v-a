using System;
using UnityEngine;

namespace Code.Components.Items
{
    public abstract class Item : Entity
    {
        public abstract void Use(Action OnEnd = null);
    }
}