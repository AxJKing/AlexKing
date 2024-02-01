using System;
using System.Collections.Generic;
using TaxiManagement;

namespace TaxiManagement
{
    class Program
    {
        private const int TAXI_JOINS_RANK = 1;
        private const int TAXI_LEAVES_RANK = 2;
        private const int TAXI_DROPS_FARE = 3;
        private const int VIEW_FINANCIAL_REPORT = 4;
        private const int VIEW_TRANSACTION_LOG = 5;
        private const int VIEW_TAXI_LOCATIONS = 6;
        private const int EXIT = 7;

        private static UserUI ui;

        static void Main(string[] args)
        {
            ui = new UserUI(new RankManager(), new TaxiManager(), new TransactionManager());

            DisplayMenu();
            int choice = ReadInteger("\nOption:");

            while (choice != EXIT)
            {
                switch (choice)
                {
                    case TAXI_DROPS_FARE:
                        TaxiDropsFare();
                        break;
                    case TAXI_JOINS_RANK:
                        TaxiJoinsRank();
                        break;
                    case TAXI_LEAVES_RANK:
                        TaxiLeavesRank();
                        break;
                    case VIEW_FINANCIAL_REPORT:
                        ViewFinancialReport();
                        break;
                    case VIEW_TAXI_LOCATIONS:
                        ViewTaxiLocations();
                        break;
                    case VIEW_TRANSACTION_LOG:
                        ViewTransactionLog();
                        break;
                    default:
                        Console.WriteLine("\nOption not recogised.");
                        break;
                }

                DisplayMenu();
                choice = ReadInteger("\nOption:");
            }
        }

        private static void DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine(TAXI_JOINS_RANK + ". Taxi joins rank");
            Console.WriteLine(TAXI_LEAVES_RANK + ". Taxi leaves rank");
            Console.WriteLine(TAXI_DROPS_FARE + ". Taxi drops fare");
            Console.WriteLine(VIEW_FINANCIAL_REPORT + ". View financial report");
            Console.WriteLine(VIEW_TRANSACTION_LOG + ". View transaction log");
            Console.WriteLine(VIEW_TAXI_LOCATIONS + ". View taxi locations");
            Console.WriteLine(EXIT + ". Exit");
        }

        private static void DisplayResults(List<string> results)
        {
            Console.WriteLine();
            foreach (string s in results)
            {
                Console.WriteLine(s);
            }
        }

        private static double ReadDouble(string prompt)
        {
            double num = -1;
            bool dataOK = false;

            Console.Write(prompt + " > ");
            do
            {
                try
                {
                    num = Convert.ToDouble(Console.ReadLine());
                    dataOK = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("\nERROR: this should be a decimal-point value. Please try again.");
                    Console.Write(prompt + " > ");
                }
            }
            while (!dataOK);

            return num;
        }

        private static int ReadInteger(string prompt)
        {
            int num = -1;
            bool dataOK = false;

            Console.Write(prompt + " > ");
            do
            {
                try
                {
                    num = Convert.ToInt32(Console.ReadLine());
                    dataOK = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("\nERROR: this should be an integer. Please try again.");
                    Console.Write(prompt + " > ");
                }
            }
            while (!dataOK);

            return num;
        }

        private static string ReadString(string prompt)
        {
            string s;

            Console.Write(prompt + " > ");
            do
            {
                s = Console.ReadLine();
                if (s == "")
                {
                    Console.WriteLine("\nERROR: an answer is required. Please try again.");
                    Console.Write(prompt + " > ");
                }
            }
            while (s == "");

            return s;
        }

        private static void TaxiDropsFare()
        {
            int taxiNum = ReadInteger("Taxi number:");
            bool priceWasPaid = ReadString("Was the agreed price paid? (y/n):").ToLower().StartsWith('y');

            DisplayResults(ui.TaxiDropsFare(taxiNum, priceWasPaid));
        }

        private static void TaxiJoinsRank()
        {
            int taxiNum = ReadInteger("Taxi number:");
            int rankId = ReadInteger("Rank id:");

            DisplayResults(ui.TaxiJoinsRank(taxiNum, rankId));
        }

        private static void TaxiLeavesRank()
        {
            int rankId = ReadInteger("Rank id:");
            string destination = ReadString("Destination:");
            double agreedPrice = ReadDouble("Agreed price:");

            DisplayResults(ui.TaxiLeavesRank(rankId, destination, agreedPrice));
        }

        private static void ViewFinancialReport()
        {
            DisplayResults(ui.ViewFinancialReport());
        }

        private static void ViewTaxiLocations()
        {
            DisplayResults(ui.ViewTaxiLocations());
        }

        private static void ViewTransactionLog()
        {
            DisplayResults(ui.ViewTransactionLog());
        }
    }
}
