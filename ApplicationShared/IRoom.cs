using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

namespace ApplicationShared
{
    public interface IRoom
    {
        Task<List<Room>> GetFloor(int floor, string building);
        Task<List<Room>> Filter(List<Room> rooms, int? size, Attribute attributes);
        Task<Room> GetCurrentStatus();
        Task<Room> GetCurrentStatusForFloor(string building, int floor);
    }
}