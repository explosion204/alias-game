using System.Collections.Generic;
using AliasGame.Domain.Models;

namespace AliasGame.Service.Interfaces
{
    public interface IExpressionService
    {
        List<Expression> GetExpressions(int count);
        
    }
}