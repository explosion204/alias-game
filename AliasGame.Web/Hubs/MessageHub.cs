using System.Collections.Generic;
using System.Linq;
using AutoMapper.Internal;
using Microsoft.AspNetCore.SignalR;

namespace AliasGame.Hubs
{
    public class MessageHub : Hub
    {
        private static readonly Dictionary<string, Dictionary<string, List<string>>> _sessionsList = 
            new Dictionary<string, Dictionary<string, List<string>>>();
        // _sessionList[sessionId] < userList[userId] < list of connection ids
        
        private static string _locker = string.Empty;
        
        public void NotifySession(string sessionId, string message)
        {
            lock (_locker)
            {
                var hasSession = _sessionsList.TryGetValue(sessionId, out var sessionUsers);
                
                if (hasSession)
                {
                    foreach (var (_, connections) in sessionUsers)
                    {
                        connections.ForEach(
                            async (connId) => await Clients.Client(connId).SendAsync("receive_message", message)
                        );
                    }
                }
            }
        }

        public void Subscribe(string sessionId, string userId)
        {
            lock (_locker)
            {
                var hasSession = _sessionsList.TryGetValue(sessionId, out var sessionUsers);
                
                if (hasSession)
                {
                    var hasUser = sessionUsers.TryGetValue(userId, out var userConnections);

                    if (hasUser)
                    {
                        userConnections.Add(Context.ConnectionId);
                    }
                    else
                    {
                        sessionUsers.Add(userId, new List<string> { Context.ConnectionId });
                    }
                }
                else
                {
                    var users = new Dictionary<string, List<string>>();
                    users.Add(userId, new List<string> { Context.ConnectionId });
                    _sessionsList.Add(sessionId, users);
                }
            }
        }

        public void Unsubscribe(string sessionId, string userId)
        {
            lock (_locker)
            {
                var hasSession = _sessionsList.TryGetValue(sessionId, out var sessionUsers);
                
                if (hasSession)
                {
                    sessionUsers.Remove(userId);

                    if (sessionUsers.Count == 0)
                    {
                        // session clean up
                        _sessionsList.Remove(sessionId);
                    }
                }
            }
        }
    }
}