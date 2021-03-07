using AliasGame.Domain;
using AliasGame.Domain.Models;
using AliasGame.Service.Interfaces;

namespace AliasGame.Service
{
    public class DataManager : IDataManager
    {
        public IRepository<Expression> ExpressionRepository { get; set; }
        public IRepository<Session> SessionRepository { get; set; }
        public IRepository<User> UserRepository { get; set; }

        public DataManager(
            IRepository<Expression> expressionRepository, 
            IRepository<Session> sessionRepository, 
            IRepository<User> userRepository
        )
        {
            ExpressionRepository = expressionRepository;
            SessionRepository = sessionRepository;
            UserRepository = userRepository;
        }
    }
}