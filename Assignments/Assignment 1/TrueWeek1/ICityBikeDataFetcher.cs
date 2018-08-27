using System.Threading.Tasks;

namespace TrueWeek1
{
    public interface ICityBikeDataFetcher
    {
        Task<int> GetBikeCountInStation(string stationName);
    }
}