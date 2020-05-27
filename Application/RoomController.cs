using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationShared;
using Core;
using Microsoft.EntityFrameworkCore;

namespace Application
{
    public class RoomController : IRoom
    {
        /// <summary>
        /// Gets all rooms of a floor in a given Building
        /// </summary>
        /// <param name="building">The building where the rooms should be</param>
        /// <param name="floor">The floor of the rooms</param>
        /// <returns></returns>
        public async Task<List<Room>> GetFloor(int floor, string building)
        {
            await using var context = new ReservationContext();
            
            return await context.Rooms.Where(x => x.Floor == floor)
                .Where(x => x.Building == building).ToListAsync();;
        }

        /// <summary>
        /// Filters a list of Room by the size of the room and various attributes.
        /// </summary>
        /// <param name="rooms">The list of Rooms to be filtered</param>
        /// <param name="size">The minimum Amount of Rooms. If null, no rooms will be filtered out</param>
        /// <param name="attributes">The attributes that the room should have</param>
        /// <returns></returns>
        public async Task<List<Room>> Filter(List<Room> rooms, int? size, Attribute attributes)
        {
            int minSize = size ?? int.MinValue; 

            var query = rooms.Where(x => x.Size >= minSize)
                             .Where(x => !attributes.Computers || x.Attribute.Computers)
                             .Where(x => !attributes.PowerOutlets || x.Attribute.PowerOutlets)
                             .Where(x => !attributes.Presenter || x.Attribute.Presenter)
                             .Where(x => !attributes.AirConditioning || x.Attribute.AirConditioning);

            return query.ToList();
        }

        public async Task<Room> GetCurrentStatus()
        {
            throw new System.NotImplementedException();
        }

        public Task<Room> GetCurrentStatusForFloor(string building, int floor)
        {
            throw new System.NotImplementedException();
        }
    }
}