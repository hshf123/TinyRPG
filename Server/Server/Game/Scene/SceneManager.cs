using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class SceneManager
    {
        public static SceneManager Instance { get; } = new SceneManager();

        object _lock = new object();
        Dictionary<int, Scenes> _scenes = new Dictionary<int, Scenes>();
        int _sceneId = 1;

        public Scenes Add(Func<Scenes> sceneFactory)
        {
            Scenes scene = sceneFactory.Invoke();
            scene.Push(scene.Init);

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

        public Scenes Find(int sceneId)
        {
            lock (_lock)
            {
                Scenes scene = null;
                if (_scenes.TryGetValue(sceneId, out scene))
                    return scene;
                return null;
            }
        }

        public void PortalScene(string scene)
        {
            switch(scene)
            {
                case "Lobby":
                    break;
                case "Huntingground":
                    break;
                case "Boss":
                    // TODO
                    break;
            }
        }
    }
}
