using System;
using System.Collections.Generic;
using AliasGame.Domain;
using AliasGame.Domain.Models;
using AliasGame.Service.Interfaces;

namespace AliasGame.Service
{
    public class SessionService : ISessionService
    {
        private readonly IRepository<Session> _sessionRepository;

        public SessionService(IRepository<Session> sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public string CreateSession(string userId)
        {
            var session = new Session
            {
                Id = default,
                FirstPlayerId = userId,
                SecondPlayerId = default,
                ThirdPlayerId = default,
                FourthPlayerId = default
            };

            return _sessionRepository.SaveEntity(session);
        }

        public Dictionary<int, string> GetSessionInfo(string sessionId)
        {
            var dict = new Dictionary<int, string>();
            var session = _sessionRepository.GetEntity(sessionId);
            
            if (session != null)
            {
                dict.Add(1, session.FirstPlayerId);
                dict.Add(2, session.SecondPlayerId);
                dict.Add(3, session.ThirdPlayerId);
                dict.Add(4, session.FourthPlayerId);
            }

            return dict;
        }

        public bool JoinSession(string userId, string sessionId, ISessionService.Team team)
        {
            var session = _sessionRepository.GetEntity(sessionId);

            if (session != null) return false;

            switch (team)
            {
                case ISessionService.Team.TeamOne:
                    if (session.SecondPlayerId.Equals(default))
                    {
                        session.SecondPlayerId = userId;
                        _sessionRepository.SaveEntity(session);
                        return true;
                    }

                    return false;
                case ISessionService.Team.TeamTwo:
                    if (session.ThirdPlayerId.Equals(default))
                    {
                        session.ThirdPlayerId = userId;
                        _sessionRepository.SaveEntity(session);
                        return true;
                    }
                    else if (session.FourthPlayerId.Equals(default))
                    {
                        session.FourthPlayerId = userId;
                        _sessionRepository.SaveEntity(session);
                        return true;
                    }

                    return false;
            }

            return false;
        }

        public bool LeaveSession(string userId, string sessionId)
        {
            var session = _sessionRepository.GetEntity(sessionId);

            if (session == null) return false;

            if (session.FirstPlayerId.Equals(userId))
            {
                _sessionRepository.DeleteEntity(sessionId);
            }
            else if (session.SecondPlayerId.Equals(userId))
            {
                session.SecondPlayerId = default;
                _sessionRepository.SaveEntity(session);
            }
            else if (session.ThirdPlayerId.Equals(userId))
            {
                session.ThirdPlayerId = default;
                _sessionRepository.SaveEntity(session);
            }
            else if (session.FourthPlayerId.Equals(userId))
            {
                session.FourthPlayerId = default;
                _sessionRepository.SaveEntity(session);
            }
            else
            {
                return false;
            }
            
            return true;
        }
    }
}