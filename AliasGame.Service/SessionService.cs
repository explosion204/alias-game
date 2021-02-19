using System;
using AliasGame.Domain;
using AliasGame.Domain.Models;

namespace AliasGame.Service
{
    public class SessionService
    {
        private readonly IRepository<Session> _sessionRepository;

        public SessionService(IRepository<Session> sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public Guid CreateSession(Guid userId)
        {
            throw new NotImplementedException();
        }

        public bool JoinSession(Guid userId, Guid sessionId)
        {
            throw new NotImplementedException();
        }
    }
}