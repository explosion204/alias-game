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
    internal class UserRepository : IRepository<User>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public List<User> GetAllEntities()
        {
            var efUsers = _context.Users.ToList();
            return _mapper.Map<List<User>>(efUsers);
        }

        public User GetEntity(Guid id)
        {
            var efUser = _context.Users.FirstOrDefault(x => x.Id == id);
            return efUser != null ? _mapper.Map<User>(efUser) : null;
        }

        public Guid SaveEntity(User entity)
        {
            var efUser = _mapper.Map<EfUser>(entity);
            
            if (efUser.Id == default)
            {
                _context.Entry(efUser).State = EntityState.Added;
            }
            else
            {
                var entry = _context.Users.First(x => x.Id == efUser.Id);
                _context.Entry(entry).State = EntityState.Detached;
                _context.Entry(efUser).State = EntityState.Modified;
            }
            
            _context.SaveChanges();
            return efUser.Id;
        }

        public void DeleteEntity(Guid id)
        {
            var efUser = _context.Users.FirstOrDefault(x => x.Id == id);

            if (efUser != null)
            {
                _context.Users.Remove(efUser);
            }

            _context.SaveChanges(); 
        }
    }
}