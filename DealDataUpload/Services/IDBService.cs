using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealDataUpload.Models;

namespace DealDataUpload.Services
{
    public interface IDBService
    {
        List<Deal> FetchAllDeals();

        Deal GetDealByDealNumber(int dealNumber);
        void AddDeal(Deal newDeal);

        void AddMultipleDeals(List<Deal> deals);

        string[] FetchMostPopularVehicles();

    }
}
