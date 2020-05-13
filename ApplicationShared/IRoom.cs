using System.Collections.Generic;
using Core;

namespace ApplicationShared
{
    public interface IRoom
    {
        List<Room> getFloor();
        List<Room> filter();
        Room getCurrentStatus();
    }
}