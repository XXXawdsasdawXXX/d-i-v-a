using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Code.Infrastructure.DI;
using UnityEngine;

namespace Code.Infrastructure.GameLoop
{
    public class GameEventDispatcher : MonoBehaviour
    {
        private readonly List<IGameStartListener> _startListeners = new List<IGameStartListener>();
        private readonly List<IGameTickListener> _tickListeners = new List<IGameTickListener>();
        private readonly List<IGameExitListener> _exitListeners = new List<IGameExitListener>();

        public void Awake()
        {
            InitializeListeners();
        }

        private void Start()
        {
            NotifyGameStart();
        }

        private void Update()
        {
            NotifyGameTick();
        }

        private void OnApplicationQuit()
        {
            NotifyGameExit();
        }

        private void InitializeListeners()
        {
            var gameListeners = Container.Instance.GetGameListeners();


            /*// Получаем все типы в текущей сборке
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            // Фильтруем только те, которые реализуют интерфейс IGameListeners
            IEnumerable<Type> listenerTypes = types.Where(t => typeof(IGameListeners).IsAssignableFrom(t) && !t.IsInterface);
*/
            foreach (var listener in gameListeners)
            {
                // Инициализируем объект

                // Добавляем в соответствующий список в зависимости от интерфейса
                if (listener is IGameStartListener)
                    _startListeners.Add(listener as IGameStartListener);
                if (listener is IGameTickListener)
                    _tickListeners.Add(listener as IGameTickListener);
                if (listener is IGameExitListener)
                    _exitListeners.Add(listener as IGameExitListener);
            }
        }

        private void NotifyGameStart()
        {
            foreach (var listener in _startListeners)
            {
                listener.GameStart();
            }
        }

        private void NotifyGameTick()
        {
            foreach (var listener in _tickListeners)
            {
                listener.GameTick();
            }
        }

        private void NotifyGameExit()
        {
            foreach (var listener in _exitListeners)
            {
                listener.GameExit();
            }
        }
    }
}