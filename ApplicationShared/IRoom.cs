using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

namespace ApplicationShared
{
    public interface IRoom
    {
        Task<List<Room>> getFloor();
        Task<List<Room>> filter();
        Task<Room> getCurrentStatus();
    }
}