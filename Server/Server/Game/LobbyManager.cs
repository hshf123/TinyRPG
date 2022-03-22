using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class LobbyManager
    {
        public static LobbyManager Instance { get; } = new LobbyManager();

        object _lock = new object();
        Dictionary<int, Lobby> _lobbys = new Dictionary<int, Lobby>();
        int _sceneId = 1;

        public Lobby Add()
        {
            Lobby lobby = new Lobby();
            lock(_lock)
            {
                lobby.SceneId = _sceneId;
                _lobbys.Add(_sceneId, lobby);
                _sceneId++;
            }
            return lobby;
        }

        public bool Remove(int sceneId)
        {
            lock(_lock)
            {
                return _lobbys.Remove(sceneId);
            }
        }

        public Lobby Find(int sceneId)
        {
            lock (_lock)
            {
                Lobby lobby = null;
                if (_lobbys.TryGetValue(sceneId, out lobby))
                    return lobby;
                return null;
            }
        }
    }
}
