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
    internal class ExpressionRepository : IRepository<Expression>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ExpressionRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public List<Expression> GetAllEntities()
        {
            var efExpressions = _context.Expressions.ToList();
            return _mapper.Map<List<Expression>>(efExpressions);
        }

        public Expression GetEntity(string id)
        {
            var efExpression = _context.Expressions.FirstOrDefault(x => x.Id == id);
            return efExpression != null ? _mapper.Map<Expression>(efExpression) : null;
        }

        public string SaveEntity(Expression entity)
        {
            var efExpression = _mapper.Map<EfExpression>(entity);
            
            if (efExpression.Id == default)
            {
                efExpression.Id = Guid.NewGuid().ToString();
                _context.Entry(efExpression).State = EntityState.Added;
            }
            else
            {
                _context.Entry(efExpression).State = EntityState.Modified;
            }
            
            _context.SaveChanges();
            return efExpression.Id;
        }

        public void DeleteEntity(string id)
        {
            var efExpression = _context.Expressions.FirstOrDefault(x => x.Id == id);

            if (efExpression != null)
            {
                _context.Expressions.Remove(efExpression);
            }

            _context.SaveChanges();
        }
    }
}