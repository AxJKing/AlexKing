using System;

namespace TaxiManagement
{
    public abstract class Transaction
    {
        //Set variables
        public DateTime TransactionDatetime { get; }
        public string TransactionType { get; }
        public string TaxiNumber { get; }
        public DateTime Timestamp { get; set; }

        //Method takes transaction type and time
        public Transaction(string type, DateTime dt)
        {
            TransactionType = type;
            TransactionDatetime = dt;
        }

        //Related transaction classes can override
        public abstract override string ToString();
    }
}
