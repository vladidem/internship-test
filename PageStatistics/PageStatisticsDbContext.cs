using Microsoft.EntityFrameworkCore;

namespace PageStatistics
{
    public class PageStatisticsDbContext : DbContext, IPageStatisticsDbContext
    {
        public PageStatisticsDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
