using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

namespace ApplicationShared
{
    public interface IRoom
    {
        Task<List<Room>> getFloor(int floor, string building);
        Task<List<Room>> filter(List<Room> rooms, int? size, Attribute attributes);
        Task<Room> getCurrentStatus();
    }
}