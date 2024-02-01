using System;
using System.Collections.Generic;

namespace TaxiManagement
{
    public class TransactionManager
    {
        public List<Transaction> Transactions { get; } = new List<Transaction>();

        // Method to record a taxi joining a rank
        public void RecordJoin(int taxiNum, int rankId)
        {
            JoinTransaction transaction = new JoinTransaction(DateTime.Now, taxiNum, rankId);
            Transactions.Add(transaction);
        }

        // Method to record a taxi leaving a rank
        public void RecordLeave(int taxiNum, Taxi taxi)
        {
            LeaveTransaction transaction = new LeaveTransaction(DateTime.Now, taxiNum, taxi);
            Transactions.Add(transaction);
        }

        // Method to record a drop-off by a taxi
        public void RecordDrop(int taxiNum, bool isSuccessful)
        {
            DropTransaction transaction = new DropTransaction(DateTime.Now, taxiNum, isSuccessful);
            Transactions.Add(transaction);
        }

        // Method to manually add a join transaction with a specified timestamp
        public void AddJoinTransaction(int taxiNum, DateTime timestamp)
        {
            JoinTransaction transaction = new JoinTransaction(timestamp, taxiNum, 0);
            Transactions.Add(transaction);
        }

        // Method to manually add a leave transaction with a specified timestamp
        public void AddLeaveTransaction(int taxiNum, DateTime timestamp)
        {
            LeaveTransaction transaction = new LeaveTransaction(timestamp, taxiNum, null);
            Transactions.Add(transaction);
        }

        // Method to manually add a drop transaction 
        public void AddDropTransaction(int taxiNum, DateTime timestamp, bool isSuccessful = false)
        {
            DropTransaction transaction = new DropTransaction(timestamp, taxiNum, isSuccessful);
            Transactions.Add(transaction);
        }
    }
}
