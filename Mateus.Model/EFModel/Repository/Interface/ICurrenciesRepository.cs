﻿using System.Linq;
using PITFramework.Repository;

namespace Mateus.Model.EFModel.Repository.Interface
{
    public interface ICurrenciesRepository : IRepository<Currency>
    {
        Currency GetCurrencyByPK(int currenciesPK);

        IQueryable<Currency> GetValid();
    }
}
