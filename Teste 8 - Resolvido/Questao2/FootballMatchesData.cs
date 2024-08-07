using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questao2
{
    public class FootballMatchesData
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public MatchData[] Data { get; set; }
    }
}
