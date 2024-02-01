using System;
using System.Collections.Generic;
using System.Linq;

namespace TaxiManagement
{
    public class Rank
    {
        //Declares variable properties and list
        public int Id { get; }
        private int numberOfTaxiSpaces;
        private List<Taxi> taxis;

        //Method takes 2 parameters and assigns ID, adds to list
        public Rank(int id, int numberOfTaxiSpaces)
        {
            Id = id;
            this.numberOfTaxiSpaces = numberOfTaxiSpaces;
            taxis = new List<Taxi>(numberOfTaxiSpaces);
        }

        //Allows the list to be accessed
        public List<Taxi> Taxis
        {
            get { return taxis; }
        }


        // Method adds Taxi object to taxi list of the Rank object at the back
        public bool AddTaxi(Taxi t)
        {
            if (taxis.Contains(t))
            {
                return false;
            }
            if (taxis.Count < numberOfTaxiSpaces)
            {
                taxis.Add(t);
                t.Rank = this;
                return true;
            }
            return false;
        }


        //Method condition if no taxis return null
        public Taxi FrontTaxiTakesFare(string destination, double agreedPrice)
        {
            if (taxis.Count > 0)
            {
                Taxi taxi = taxis[0];
                taxi.AddFare(destination, agreedPrice);
                taxis.RemoveAt(0);
                return taxi;
            }
            return null;
        }

        //Method if taxi list empty return null
        public Taxi FrontTaxiInRankTakesFare(string destination, double agreedPrice)
        {
            if (taxis.Count > 0)
            {
                Taxi taxi = taxis[0];
                taxi.AddFare(destination, agreedPrice);
                taxis.RemoveAt(0);
                return taxi;
            }
            return null;
        }

    }
}
