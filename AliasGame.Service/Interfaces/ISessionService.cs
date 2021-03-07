using System;

namespace AliasGame.Service.Interfaces
{
    public interface ISessionService
    {
        Guid CreateSession(Guid userId);
        bool JoinSession(Guid userId, Guid sessionId);
    }
}