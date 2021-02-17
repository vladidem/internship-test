using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace PageStatistics
{
    public interface IPageStatisticsDbContext
    {
        public DatabaseFacade Database { get; }
    }
}
