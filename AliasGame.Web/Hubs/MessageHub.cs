using System.Collections.Generic;
using AutoMapper.Internal;
using Microsoft.AspNetCore.SignalR;

namespace AliasGame.Hubs
{
    public class MessageHub : Hub
    {
        private readonly Dictionary<string, List<string>> _sessionsList;
        private static string _locker = string.Empty;

        public MessageHub()
        {
            _sessionsList = new Dictionary<string, List<string>>();
        }

        public void NotifySession(string sessionId, string message)
        {
            lock (_locker)
            {
                List<string> sessionConnections;
                var hasConnections = _sessionsList.TryGetValue(sessionId, out sessionConnections);

                if (hasConnections)
                {
                    sessionConnections.ForAll(
                        async (uid) => await Clients.Client(uid).SendAsync(message)
                    );
                }
            }
        }

        public void Subscribe(string sessionId, string userId)
        {
            lock (_locker)
            {
                var hasConnections = _sessionsList.TryGetValue(sessionId, out var userConnections);

                if (hasConnections)
                {
                    userConnections.Add(userId);
                    // _sessionsList[sessionId] = userConnections;
                }
                else
                {
                    _sessionsList.Add(sessionId, new List<string> { userId });
                }
            }
        }

        public void Unsubscribe(string sessionId, string userId)
        {
            lock (_locker)
            {
                var hasConnections = _sessionsList.TryGetValue(sessionId, out var userConnections);

                if (hasConnections)
                {
                    userConnections.Remove(userId);
                    // _sessionsList[sessionId] = userConnections;
                }
            }
        }
    }
}