using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PageStatistics.Models;

namespace PageStatistics
{
    public interface IPageStatisticsDbContext
    {
        DbSet<Page> Pages { get; set; }

        DbSet<Word> Words { get; set; }

        DbSet<WordFrequency> WordFrequencies { get; set; }

        DatabaseFacade Database { get; }

        int SaveChanges();
    }
}
