using UnityEngine;

namespace Code.Components.Objects
{
    public abstract class SpawnService<T> where T : MonoBehaviour
    {
        protected abstract void Spawn();
        protected abstract void DeSpawn();
    }
}