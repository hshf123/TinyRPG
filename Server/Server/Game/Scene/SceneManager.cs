using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class SceneManager<T> where T : Scenes, new()
    {
        public static SceneManager<T> Instance { get; } = new SceneManager<T>();

        object _lock = new object();
        Dictionary<int, T> _scenes = new Dictionary<int, T>();
        int _sceneId = 1;

        public T Add(string mapName)
        {
            T scene = new T();
            scene.Init(mapName);

            lock(_lock)
            {
                scene.SceneId = _sceneId;
                _scenes.Add(_sceneId, scene);
                _sceneId++;
            }
            return scene;
        }

        public bool Remove(int sceneId)
        {
            lock(_lock)
            {
                return _scenes.Remove(sceneId);
            }
        }

        public T Find(int sceneId)
        {
            lock (_lock)
            {
                T scene = null;
                if (_scenes.TryGetValue(sceneId, out scene))
                    return scene;
                return null;
            }
        }
    }
}
