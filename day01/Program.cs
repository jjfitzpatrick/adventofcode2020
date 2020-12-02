using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace day01
{
    class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<int>(
                    aliases: new string[] { "--sum" },
                    getDefaultValue: () => 2020,
                    description: "Two numbers from the list of values should sum to this."),
                new Option<FileInfo>(
                    aliases: new string[] { "--file" },
                    getDefaultValue: () => new FileInfo("input.txt"),
                    description: "File to use as input. One integer per line.")
            };

            rootCommand.Description = "Day 1 challenge for Advent of Code."
                + "\nSee https://adventofcode.com/2020/day/1 for more information.";

            rootCommand.Handler = CommandHandler.Create<int, FileInfo>((sum, file) =>
                {
                    Console.WriteLine($"The value for --sum is: {sum}");
                    Console.WriteLine($"The value for --file is: {file.Name}");

                    var values = GetValues(file?.Name);
                    var solution = new List<List<int>>();

                    Console.WriteLine($"");
                    Console.WriteLine($"Integers discovered in {file.Name}: {values.Count}");

                    // Naive solution
                    Console.WriteLine($"");
                    Console.WriteLine("Executing NaiveSolution");
                    using (new ConsoleStopWatch())
                    {
                        solution = NaiveSolution(values, sum);
                    }

                    foreach (var pair in solution)
                    {
                        var subset = String.Join(", ", pair);
                        var multiply = pair[0] * pair[1];
                        Console.WriteLine($"{subset}, multiplying to {multiply}");
                    }

                    // Hash Table solution
                    Console.WriteLine($"");
                    Console.WriteLine("Executing HashTableSolution");
                    using (new ConsoleStopWatch())
                    {
                        solution = HashTableSolution(values, sum);
                    }

                    foreach (var pair in solution)
                    {
                        var subset = String.Join(", ", pair);
                        var multiply = pair[0] * pair[1];
                        Console.WriteLine($"{subset}, multiplying to {multiply}");
                    }
                });

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }

        public static List<int> GetValues(string fileName)
        {
            var result = new List<int>();

            string[] input = File.ReadAllLines(fileName);

            foreach (var str in input)
            {
                result.Add(Int32.Parse(str));
            }

            return result;
        }

        public static List<List<int>> NaiveSolution(List<int> values, int sum)
        {
            var result = new List<List<int>>();
            for (int i = 0; i < values.Count; i++)
            {
                for (int j = 0; j < values.Count; j++)
                {
                    if (i == j) continue;

                    if (values.ElementAt(i) + values.ElementAt(j) == sum)
                    {
                        result.Add(new List<int>
                        {
                            values.ElementAt(i),
                            values.ElementAt(j),
                        });
                    }
                }
            }

            return result;
        }

        public static List<List<int>> HashTableSolution(List<int> values, int sum)
        {
            var result = new List<List<int>>();
            var hashTable = new Hashtable();

            for (int i = 0; i < values.Count; i++)
            {
                var sumDifference = sum - values.ElementAt(i);
                if (hashTable.ContainsValue(sumDifference))
                {
                    result.Add(new List<int>
                    {
                        values.ElementAt(i),
                        sumDifference
                    });
                }

                hashTable.Add(i, values.ElementAt(i));
            }

            return result;
        }

        public class ConsoleStopWatch : IDisposable
        {
            private readonly Stopwatch _stopWatch;
            public ConsoleStopWatch()
            {
                _stopWatch = new Stopwatch();
                _stopWatch.Start();
            }

            public void Dispose()
            {
                _stopWatch.Stop();
                TimeSpan ts = _stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000000000}",
                                                    ts.Hours, ts.Minutes, ts.Seconds,
                                                    ts.Milliseconds);

                var ticks = ts.Ticks;

                Console.WriteLine($"Method executed in {elapsedTime} time and used {ticks} ticks.");
            }
        }
    }
}
