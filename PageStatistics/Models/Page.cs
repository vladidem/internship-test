using System;
using System.Collections.Generic;

namespace PageStatistics.Models
{
    public class Page
    {
        public int Id { get; set; }

        public string Address { get; set; }

        public string FileName { get; set; }

        public DateTime LoadedAt { get; set; }

        public List<WordFrequency> WordFrequencies { get; set; } = new List<WordFrequency>();
    }
}
