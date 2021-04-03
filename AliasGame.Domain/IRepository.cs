using System;
using System.Collections.Generic;
using AliasGame.Domain.Models;

namespace AliasGame.Domain
{
    public interface IRepository<T> where T : Entity
    {
        List<T> GetAllEntities();
        T GetEntity(string id);
        string SaveEntity(T entity);
        void DeleteEntity(string id);
    }
}