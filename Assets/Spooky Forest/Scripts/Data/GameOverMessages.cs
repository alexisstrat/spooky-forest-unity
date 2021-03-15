using System;
using System.Collections.Generic;

namespace Spooky_Forest.Scripts.Data
{
    [Serializable]
    public class GameOverMessages
    {
        public List<GameOverMessage> gameOverMessages = new List<GameOverMessage>();
    }

    [Serializable]
    public class GameOverMessage
    {
        public string identifier;
        public List<string> messages;
    }
}