using System;
using System.Linq;


namespace WeatherLab
{
    class Program
    {
        static string dbfile = @".\data\climate.db";

        public static void Main(string[] args)
        {
            var measurements = new WeatherSqliteContext(dbfile).Weather;

            // total precipitation
            var total_2020_precipitation = measurements.Where(
                    row => row.year == 2020
                ).Select(
                    row => row.precipitation
                ).Sum();

            Console.WriteLine($"Total precipitation in 2020: {total_2020_precipitation} mm\n");

            // hdd and cdd
            var groupByYear = measurements
                .GroupBy(row => row.year)
                .Select(
                    groupByYear => new {
                        Year = groupByYear.Key,
                        Hdd = groupByYear.Where(row => row.meantemp < 18).Count(),
                        Cdd = groupByYear.Where(row => row.meantemp >= 18).Count()
                    }
                );

            Console.WriteLine("Year\tHDD\tCDD");
            foreach (var i in groupByYear)
            {
                Console.WriteLine($"{i.Year}\t{i.Hdd}\t{i.Cdd}");
            }

            // most variable days
            var mvd = measurements
                .Select(
                    r => new { 
                        Date = $"{r.year}-{r.month:d2}-{r.day:d2}",
                        Delta = r.maxtemp - r.mintemp
                    }
                ).OrderByDescending(
                    data => data.Delta
                ).Take(5);

            Console.WriteLine("\nTop 5 Most Variable Days");
            Console.WriteLine("YYYY-MM-DD\tDelta");
            foreach (var i in mvd)
            {
                Console.WriteLine($"{i.Date}\t{Math.Round((Double)i.Delta, 2)}");
            }
        }
    }
}
