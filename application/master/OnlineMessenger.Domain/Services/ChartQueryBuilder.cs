using System;
using System.Collections.Generic;
using System.Linq;
using OnlineMessenger.Domain.Infrastructure;


namespace OnlineMessenger.Domain.Services
{
    public class ChartQueryBuilder : IChartQueryBuilder
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChartQueryBuilder(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Dictionary<DateTime, int> UsersPerDay()
        {
            return _unitOfWork.UserRepository.UsersPerDay();
        }

        public Dictionary<DateTime, int> MessagesPerDay()
        {
            return _unitOfWork.MessageRepository.MessagesPerDay();
        }

        public Dictionary<string, int> UsersPerOperator()
        {
            var items = _unitOfWork.UserRepository.Query(x => x.Roles, x => x.Clients)
                .Where(x => x.Roles.Any())
                .Select(x => new { x.Name, ClientsCount = x.Clients.Count() })
                .ToDictionary(x => x.Name, x => x.ClientsCount);
            return items;
        }

    }
}
