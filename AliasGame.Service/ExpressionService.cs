using System.Collections.Generic;
using System.Linq;
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

        public List<string> GetExpressions(int count)
        {
            return _expressionRepository.GetEntities(count).Select(x => x.Text).ToList();
        }
    }
}