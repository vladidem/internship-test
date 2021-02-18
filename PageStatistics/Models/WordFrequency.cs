namespace PageStatistics.Models
{
    public class WordFrequency
    {
        public int Id { get; set; }

        public int Frequency { get; set; }

        public Word Word { get; set; }

        public int WordId { get; set; }

        public Page Page { get; set; }

        public int PageId { get; set; }
    }
}
