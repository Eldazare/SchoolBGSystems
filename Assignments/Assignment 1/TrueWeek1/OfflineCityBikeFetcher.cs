using System;
using System.Threading.Tasks;

namespace TrueWeek1
{
    public class OfflineCityBikeFetcher : ICityBikeDataFetcher
    {
        public async Task<int> GetBikeCountInStation(string stationName)
        {
            var text = System.IO.File.ReadAllText("bikedata.txt");
            var lines = text.Split("\n");
            //Console.WriteLine(lines[0]);
            foreach(string str in lines){
                string[] lineSplit = str.Split(" : ");
                if (lineSplit[0] == stationName){
                    //Console.WriteLine(lineSplit[0]);
                    //Console.WriteLine(lineSplit[1]);
                    return int.Parse(lineSplit[1]);
                }
            }
            throw new NotFoundException(stationName + " not found.");
        }
    }
}