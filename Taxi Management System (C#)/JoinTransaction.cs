using System;

namespace TaxiManagement
{
    public class JoinTransaction : Transaction
    {//Retrieves variables
        public int TaxiId { get; }
        public int RankId { get; }

        //Method adds description to transaction
        public JoinTransaction(DateTime dt, int taxiId, int rankId) : base("Join", dt)
        {
            TaxiId = taxiId;
            RankId = rankId;
        }

        //Overides transaction
        public override string ToString()
        {
            return $"{TransactionDatetime:dd/MM/yyyy HH:mm} Join      - Taxi {TaxiId} in rank {RankId}";
        }
    }
}
