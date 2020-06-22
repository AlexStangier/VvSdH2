using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

namespace ApplicationShared
{
    public interface IRoom
    {
        Task<List<Room>> GetFloor(int floor, string building);
        Task<List<Room>> Filter(List<Room> rooms, int? size, Core.Attribute attributes);
        Task<Room> GetCurrentStatusForRoom(int roomnumber, string building);
        Task<List<Room>> GetCurrentStatusForFloor(string building, int floor);
        Task<(string, string)> GetReservationForRoom(Room room, DateTime timestamp);
    }
}