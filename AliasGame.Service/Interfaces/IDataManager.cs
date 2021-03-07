using AliasGame.Domain;
using AliasGame.Domain.Models;

namespace AliasGame.Service.Interfaces
{
    public interface IDataManager
    {
        IRepository<Expression> ExpressionRepository { get; set; }
        IRepository<Session> SessionRepository { get; set; }
        IRepository<User> UserRepository { get; set; }
    }
}