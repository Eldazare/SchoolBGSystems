using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TrueWeek1
{
    class Program
    {
        static void Main(string[] args)
        {
            try{
                ICityBikeDataFetcher fetcher;

                Console.WriteLine("Give station name and offline/realtime:");
                string inputLine = Console.ReadLine();
                string[] inputLines = inputLine.Split(" ");
                switch(inputLines[1]){
                    case "offline":
                        fetcher = new OfflineCityBikeFetcher();
                        break;
                    case "realtime":
                        fetcher = new RealTimeCityBikeDataFetcher();
                        break;
                    default:
                        throw new NotFoundException("Secondary command "+inputLines[1]+" not found");
                }
                Task<int> task = fetcher.GetBikeCountInStation(inputLines[0]);
                task.Wait();
                Console.WriteLine("Result is: "+task.Result);
            } catch (AggregateException ae) {
                foreach(Exception theEx in ae.InnerExceptions){
                    if (theEx is ArgumentException){
                        Console.WriteLine(theEx.Message);
                    } else if (theEx is NotFoundException){
                        Console.WriteLine(theEx.Message);
                    } else {
                        Console.WriteLine("something went wrong"+theEx);
                    }
                }
            }
        }
    }
    class NotFoundException : Exception{
        public NotFoundException(string str): base(str){
            
        }
    }
}

