using Microsoft.EntityFrameworkCore;
using PageStatistics.Models;

namespace PageStatistics
{
    public class PageStatisticsDbContext : DbContext, IPageStatisticsDbContext
    {
        public PageStatisticsDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Page> Pages { get; set; }

        public DbSet<Word> Words { get; set; }

        public DbSet<WordFrequency> WordFrequencies { get; set; }
    }
}
