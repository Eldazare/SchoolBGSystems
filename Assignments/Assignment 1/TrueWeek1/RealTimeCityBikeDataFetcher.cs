using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TrueWeek1
{
    public class RealTimeCityBikeDataFetcher : ICityBikeDataFetcher
    {
        public async Task<int> GetBikeCountInStation(string stationName){
            foreach (char ch in stationName){
                if (char.IsDigit(ch)){
                    throw new ArgumentException("There is a number in the name");
                }
            }
            HttpClient client = new HttpClient();
            var respons = await client.GetAsync(new Uri("http://api.digitransit.fi/routing/v1/routers/hsl/bike_rental"));
            BlueprintClass theObject = JsonConvert.DeserializeObject(await respons.Content.ReadAsStringAsync(),typeof(BlueprintClass)) as BlueprintClass;
            foreach (BlueprintMinor minor in theObject.stations){
                //Console.WriteLine(minor.bikesAvailable);
                if (minor.name.Contains(stationName)){
                    return minor.bikesAvailable;
                }
            }
            throw new NotFoundException(stationName+ " not found.");
        }
    }
}