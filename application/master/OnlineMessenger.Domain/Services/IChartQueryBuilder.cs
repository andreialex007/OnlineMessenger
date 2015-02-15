using System;
using System.Collections.Generic;

namespace OnlineMessenger.Domain.Services
{
    public interface IChartQueryBuilder
    {
        Dictionary<DateTime, int> UsersPerDay();
        Dictionary<DateTime, int> MessagesPerDay();
        Dictionary<string, int> UsersPerOperator();
    }
}