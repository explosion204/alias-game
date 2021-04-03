using System;
using System.Collections.Generic;

namespace AliasGame.Service.Interfaces
{
    public interface ISessionService
    {
        public enum Team
        {
            TeamOne = 1, TeamTwo = 2
        }
        
        string CreateSession(string userId);
        Dictionary<int, string> GetSessionInfo(string sessionId);
        bool JoinSession(string userId, string sessionId, Team team);
        bool LeaveSession(string userId, string sessionId);
    }
}