using System;

namespace TaxiManagement
{
    public class LeaveTransaction : Transaction
    {// Gets variables
        public int RankId { get; }
        public Taxi Taxi { get; }

        //Method gives leave description
        public LeaveTransaction(DateTime dt, int rankId, Taxi taxi) : base("Leave", dt)
        {
            RankId = rankId;
            Taxi = taxi;
        }

        //Overides transaction
        public override string ToString()
        {
            string destinationInfo = Taxi.HasFare ? $"to {Taxi.Destination} for £{Taxi.AgreedPrice.ToString("F2")}" : "without a fare";
            return $"{TransactionDatetime.ToString("dd/MM/yyyy HH:mm")} Leave     - Taxi {Taxi.Number} from rank {RankId} {destinationInfo}";
        }
    }
}
