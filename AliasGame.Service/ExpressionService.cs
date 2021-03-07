using System;
using System.Collections.Generic;
using AliasGame.Domain;
using AliasGame.Domain.Models;
using AliasGame.Service.Interfaces;

namespace AliasGame.Service
{
    public class ExpressionService : IExpressionService
    {
        private readonly IRepository<Expression> _expressionRepository;

        public ExpressionService(IRepository<Expression> expressionRepository)
        {
            _expressionRepository = expressionRepository;
        }

        public List<Expression> GetExpressions(int count)
        {
            var allExpressions = _expressionRepository.GetAllEntities();
            var targetExpressions = new List<Expression>();
            var random = new Random(DateTime.Now.Second);

            for (var i = 0; i < count; i++)
            {
                var expression = allExpressions[random.Next(count)];
                targetExpressions.Add(expression);
            }

            return targetExpressions;
        }
    }
}