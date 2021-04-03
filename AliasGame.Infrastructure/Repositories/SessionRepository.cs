using System;
using System.Collections.Generic;
using System.Linq;
using AliasGame.Domain;
using AliasGame.Domain.Models;
using AliasGame.Infrastructure.Database;
using AliasGame.Infrastructure.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AliasGame.Infrastructure
{
    internal class SessionRepository : IRepository<Session>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SessionRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public List<Session> GetAllEntities()
        {
            var efSessions = _context.Sessions.ToList();
            return _mapper.Map<List<Session>>(efSessions);
        }

        public Session GetEntity(string id)
        {
            var efSession = _context.Sessions.FirstOrDefault(x => x.Id == id);
            return efSession != null ? _mapper.Map<Session>(efSession) : null;
        }

        public string SaveEntity(Session entity)
        {
            var efSession = _mapper.Map<EfSession>(entity);
            
            if (efSession.Id == default)
            {
                efSession.Id = Guid.NewGuid().ToString();
                _context.Entry(efSession).State = EntityState.Added;
            }
            else
            {
                var entry = _context.Sessions.First(x => x.Id == efSession.Id);
                _context.Entry(entry).State = EntityState.Detached;
                _context.Entry(efSession).State = EntityState.Modified;
            }
            
            _context.SaveChanges();
            return efSession.Id;
        }

        public void DeleteEntity(string id)
        {
            var efSession = _context.Sessions.FirstOrDefault(x => x.Id == id);

            if (efSession != null)
            {
                _context.Sessions.Remove(efSession);
            }

            _context.SaveChanges();        
        }
    }
}