using System;
using System.Collections.Generic;

namespace TaxiManagement
{
    public class TaxiManager
    //Creates sorted dictionary for taxis, dictionary for times of taxis
    {
        private SortedDictionary<int, Taxi> taxis;
        private Dictionary<string, DateTime> taxiRecords;

        //Constructor for dictionaries, prepares for storage
        public TaxiManager()
        {
            taxis = new SortedDictionary<int, Taxi>();
            taxiRecords = new Dictionary<string, DateTime>();
        }

        //Searches for taxi in dictionary or returns null
        public Taxi FindTaxi(int taxiNum)
        {
            return taxis.TryGetValue(taxiNum, out Taxi taxi) ? taxi : null;
        }

        //Returns entire taxi collection in dictionary
        public SortedDictionary<int, Taxi> GetAllTaxis()
        {
            return taxis;
        }

        //Returns new taxi, with taxinum, condtion if already exists
        public Taxi CreateTaxi(int taxiNum)
        {
            if (taxis.ContainsKey(taxiNum))
            {
                return taxis[taxiNum]; 
            }
            else
            {
                Taxi newTaxi = new Taxi(taxiNum);
                taxis.Add(taxiNum, newTaxi);
                return newTaxi;
            }
        }


    }
}
