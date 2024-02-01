using System;

namespace TaxiManagement
{
    public class Taxi
    {//Retrieve and define properties and fields
        public int Number { get; } 
        public double CurrentFare { get; private set; } 
        public string Destination { get; set; } 
        public string Location { get; private set; } 
        public double TotalMoneyPaid { get; private set; } 
        public double AgreedPrice { get; set; } 
        public int? RankId { get; private set; } 
        private Rank rank; 

        public Rank Rank //Get Rank Property, checks conditions
        {
            get { return rank; }
            set
            {
                if (value == null)
                {
                    throw new Exception("Rank cannot be null");
                }
                if (!string.IsNullOrEmpty(Destination))
                {
                    throw new Exception("Cannot join rank if fare has not been dropped");
                }

                rank = value;
                RankId = value.Id;
                if (string.IsNullOrEmpty(Destination))
                {
                    Location = IN_RANK;
                }
            }
        }

        //Defines constants
        public const string ON_ROAD = "on the road"; 
        public const string IN_RANK = "in rank"; 
        private string driverName; 


        //Checks whether ongoing fare
        public bool HasFare { get { return CurrentFare > 0; } } 

        //Provides taxi with Taxi number, defines properties related 
        public Taxi(int number)
        {
            Number = number;
            CurrentFare = 0;
            Destination = string.Empty;
            Location = ON_ROAD;
            TotalMoneyPaid = 0;
            RankId = null;
        }

        //Provides taxi with Driver name, defines properties related 
        public Taxi(string driverName)
        {
            this.driverName = driverName;
            Number = new Random().Next(1, 1000); 
            CurrentFare = 0;
            Destination = string.Empty;
            Location = ON_ROAD;
            TotalMoneyPaid = 0;
            RankId = null;
        }

        //Method for add fare, condition, sets properties
        public void AddFare(string destination, double agreedPrice)
        {
            if (agreedPrice < 0)
            {
                throw new ArgumentException("Agreed price cannot be negative");
            }

            Destination = destination;
            AgreedPrice = agreedPrice;
            CurrentFare += agreedPrice;
            Location = ON_ROAD;
            rank = null;
            RankId = null;
        }

        //Method for Drop fare, condition, sets properties
        public bool DropFare(bool isPricePaid)
        {
            Console.WriteLine("Dropping off passengers.");
            if (isPricePaid)
            {
                TotalMoneyPaid += CurrentFare;
            }
            CurrentFare = 0;
            Destination = string.Empty;

            return true;
        }

    }
}
