using System;

namespace TaxiManagement
{  
    public class DropTransaction : Transaction
    {  //Set variables
        public int TaxiId { get; }
        public bool PriceWasPaid { get; }

        //Method whichs adds properties and description
        public DropTransaction(DateTime dt, int taxiId, bool priceWasPaid) : base("Drop fare", dt)
        {
            TaxiId = taxiId;
            PriceWasPaid = priceWasPaid;
        }

        //Overides transaction
        public override string ToString()
        {
            string paymentStatus = PriceWasPaid ? "price was paid" : "price was not paid";
            return $"{TransactionDatetime:dd/MM/yyyy HH:mm} Drop fare - Taxi {TaxiId}, {paymentStatus}";
        }
    }
}
