using System;
using System.Collections.Generic;
using TaxiManagement;

namespace TaxiManagement
{
    public class UserUI
    {
        private RankManager rankManager;
        private TaxiManager taxiManager;
        private TransactionManager transactionManager;

        public UserUI(RankManager rankManager, TaxiManager taxiManager, TransactionManager transactionManager)
        {
            this.rankManager = rankManager;
            this.taxiManager = taxiManager;
            this.transactionManager = transactionManager;
        }

        public List<string> TaxiDropsFare(int taxiNum, bool priceWasPaid)
        {
            List<string> results = new List<string>();
            Taxi taxi = taxiManager.FindTaxi(taxiNum);
            if (taxi != null)
            {
                if (taxi.HasFare)
                {
                    taxi.DropFare(priceWasPaid);
                    taxi.Destination = null;
                    transactionManager.AddDropTransaction(taxi.Number, DateTime.Now, priceWasPaid);
                    string priceStatus = priceWasPaid ? "price was paid" : "price was not paid";
                    results.Add($"Taxi {taxiNum} has dropped its fare and the {priceStatus}.");
                }
                else
                {
                    results.Add($"Taxi {taxiNum} has not dropped its fare.");
                }
            }
            else
            {
                results.Add($"Taxi {taxiNum} does not exist.");
            }

            return results;
        }

        public List<string> TaxiJoinsRank(int taxiNum, int rankId)
        {
            List<string> results = new List<string>();

            Taxi taxi = taxiManager.FindTaxi(taxiNum);
            Rank rank = rankManager.FindRank(rankId);

            if (taxi == null)
            {
                taxi = taxiManager.CreateTaxi(taxiNum);
            }

            if (rank != null)
            {
                bool added = rankManager.AddTaxiToRank(taxi, rankId);
                if (added)
                {
                    transactionManager.RecordJoin(taxi.Number, rankId);
                    results.Add($"Taxi {taxiNum} has joined rank {rankId}.");
                }
                else
                {
                    results.Add($"Taxi {taxiNum} has not joined rank {rankId}.");
                }
            }
            else
            {
                results.Add($"Rank {rankId} does not exist.");
            }

            return results;
        }

        public List<string> TaxiLeavesRank(int rankId, string destination, double agreedPrice)
        {
            List<string> results = new List<string>();

            Rank rank = rankManager.FindRank(rankId);

            if (rank != null)
            {
                Taxi taxi = rank.FrontTaxiInRankTakesFare(destination, agreedPrice);
                if (taxi != null)
                {
                    taxi.Destination = destination;
                    taxi.AgreedPrice = agreedPrice; 
                    transactionManager.RecordLeave(taxi.Number, taxi);
                    results.Add(
                        $"Taxi {taxi.Number} has left rank {rankId} to take a fare to {destination} for £{agreedPrice:0.00}.");
                }
                else
                {
                    results.Add($"Taxi has not left rank {rankId}.");
                }
            }
            else
            {
                results.Add($"Rank {rankId} does not exist.");
            }

            return results;
        }

        public List<string> ViewFinancialReport()
        {
            List<string> results = new List<string>();
            results.Add("Financial report");
            results.Add("================");

            var allTaxis = taxiManager.GetAllTaxis().Values;

            if (allTaxis.Count == 0)
            {
                results.Add("No taxis, so no money taken");
            }
            else
            {
                double total = 0;

                
                var sortedTaxis = allTaxis.OrderBy(taxi => taxi.Number).ToList();

                foreach (Taxi taxi in sortedTaxis)
                {
                    string taxiLine = $"Taxi {taxi.Number.ToString().PadLeft(2)} {taxi.TotalMoneyPaid.ToString("F2").PadLeft(10)}";
                    results.Add(taxiLine);
                    total += taxi.TotalMoneyPaid;
                }

                results.Add("           =======");
                results.Add($"Total:{total.ToString("F2").PadLeft(12)}");
                results.Add("           =======");
            }

            return results;
        }




        public List<string> ViewTaxiLocations()
        {
            List<string> results = new List<string>();
            results.Add("Taxi locations");
            results.Add("==============");

            var allTaxis = taxiManager.GetAllTaxis();
            bool hasTaxis = false;

            foreach (KeyValuePair<int, Taxi> taxiPair in allTaxis)
            {
                Taxi taxi = taxiPair.Value; 
                hasTaxis = true;

                if (taxi.Location == Taxi.ON_ROAD)
                {
                    if (string.IsNullOrEmpty(taxi.Destination))
                    {
                        results.Add($"Taxi {taxi.Number} is on the road");
                    }
                    else
                    {
                        results.Add($"Taxi {taxi.Number} is on the road to {taxi.Destination}");
                    }
                }
                else if (taxi.Location == Taxi.IN_RANK)
                {
                    results.Add($"Taxi {taxi.Number} is in rank {taxi.RankId}");
                }
            }

            if (!hasTaxis)
            {
                results.Add("No taxis");
            }

            return results;
        }





        public List<string> ViewTransactionLog()
        {
            List<string> results = new List<string>();
            results.Add("Transaction report");
            results.Add("==================");

            if (transactionManager.Transactions.Count == 0)
            {
                results.Add("No transactions");
            }
            else
            {
                foreach (Transaction transaction in transactionManager.Transactions)
                {
                    results.Add(transaction.ToString());
                }
            }

            return results;
        }

    }
}
