using System.Collections.Generic;
using System.Linq;

namespace TaxiManagement
{
    public class RankManager
    {
        //Rank Variables can be read from list
        private readonly List<Rank> ranks;

        //Constructor initializes  the ranks with three objects
        public RankManager()
        {
            ranks = new List<Rank>()
            {
                new Rank(1, 5),
                new Rank(2, 2),
                new Rank(3, 4)
            };
        }

        //Method iterates over ranks list and returns first rank
        public Rank FindRank(int rankId)
        {
            return ranks.FirstOrDefault(r => r.Id == rankId);
        }

        //Method checks if the specific rank already exists, meets conditions
        public bool AddTaxiToRank(Taxi taxi, int rankId)
        {
            var rank = FindRank(rankId);

            if (rank == null || taxi.HasFare)
            {
                return false;
            }

            var existingTaxi = ranks.SelectMany(r => r.Taxis).FirstOrDefault(t => t.Number == taxi.Number);

            if (existingTaxi != null)
            {
                return false;
            }

            return rank.AddTaxi(taxi);
        }

        //Method checks if specific rank exists returns null, otherwise calls front taxi method
        public Taxi FrontTaxiInRankTakesFare(int rankId, string destination, double agreedPrice)
        {
            var rank = FindRank(rankId);

            if (rank == null || rank.Taxis.Count == 0)
            {
                return null;
            }

            return rank.FrontTaxiTakesFare(destination, agreedPrice);
        }

    }
}
