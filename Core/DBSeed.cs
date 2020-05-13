using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Core
{
    public class DBSeed
    {
        public void SeedDataToDB()
        {
            using var context = new ReservationContext();

            //Seed Rooms with appropriate Attributes
            var roomList = new List<Room>
            {
                new Room
                {
                    Attribute = new Attribute
                    {
                        AirCon = true, Computers = true, Jacks = true, Presenter = true
                    },
                    Building = "A",
                    RoomNr = 100,
                    Size = 30
                },
                new Room
                {
                    Attribute = new Attribute
                    {
                        AirCon = true, Computers = true, Jacks = true, Presenter = true
                    },
                    Building = "A",
                    RoomNr = 101,
                    Size = 45
                },
                new Room
                {
                    Attribute = new Attribute
                    {
                        AirCon = true, Computers = true, Jacks = true, Presenter = true
                    },
                    Building = "A",
                    RoomNr = 102,
                    Size = 30
                },
                new Room
                {
                    Attribute = new Attribute
                    {
                        AirCon = true, Computers = false, Jacks = false, Presenter = true
                    },
                    Building = "A",
                    RoomNr = 103,
                    Size = 70
                },
                new Room
                {
                    Attribute = new Attribute
                    {
                        AirCon = true, Computers = true, Jacks = true, Presenter = true
                    },
                    Building = "A",
                    RoomNr = 104,
                    Size = 35
                },
                new Room
                {
                    Attribute = new Attribute
                    {
                        AirCon = true, Computers = true, Jacks = true, Presenter = true
                    },
                    Building = "A",
                    RoomNr = 105,
                    Size = 30
                },
                new Room
                {
                    Attribute = new Attribute
                    {
                        AirCon = true, Computers = true, Jacks = true, Presenter = true
                    },
                    Building = "A",
                    RoomNr = 106,
                    Size = 30
                },
                new Room
                {
                    Attribute = new Attribute
                    {
                        AirCon = true, Computers = true, Jacks = true, Presenter = true
                    },
                    Building = "A",
                    RoomNr = 107,
                    Size = 30
                },
                new Room
                {
                    Attribute = new Attribute
                    {
                        AirCon = true, Computers = false, Jacks = false, Presenter = true
                    },
                    Building = "A",
                    RoomNr = 108,
                    Size = 70
                },
                new Room
                {
                    Attribute = new Attribute
                    {
                        AirCon = true, Computers = false, Jacks = false, Presenter = true
                    },
                    Building = "A",
                    RoomNr = 109,
                    Size = 50
                }
            };

            var userList = new List<User>
            {
                new User
                {
                    Username = "Peter", Password = "123", Reservations = new List<Reservation>()
                },
                new User
                {
                    Username = "Udo", Password = "456", Reservations = new List<Reservation>()
                },
                new User
                {
                    Username = "Sabine", Password = "789", Reservations = new List<Reservation>()
                }
            };


            //Seed Data into Rights Table 
            var rightsList = new List<Right>
            {
                new Right
                {
                    RightsName = "Verwaltung",
                    UserHasRight = new List<User>
                    {
                    }
                },
                new Right
                {
                    RightsName = "Professor",
                    UserHasRight = new List<User>
                    {
                    }
                },
                new Right
                {
                    RightsName = "Student",
                    UserHasRight = new List<User>
                    {
                    }
                },
                new Right
                {
                    RightsName = "Lehrbeauftragter",
                    UserHasRight = new List<User>
                    {
                    }
                },
                new Right
                {
                    RightsName = "Sonstige",
                    UserHasRight = new List<User>
                    {
                    }
                }
            };


            if (context.Rights != null) context.Rights.AddRange(rightsList);
            if (context.Rooms != null) context.Rooms.AddRange(roomList);
            if (context.Users != null) context.Users.AddRange(userList);

            context.SaveChanges();
            
            var profRight = context.Rights.Where(x => x.RightsName.Equals("Professor")).FirstOrDefault();
            var peter = context.Users.Where(x => x.Username.Equals("Peter")).FirstOrDefault();

            peter.Rights = profRight;
            profRight.UserHasRight.Append(peter);
            
            peter.Reservations.Append(new Reservation
            {
                Start = new DateTime(2020,5,8,12,30,0),
                End = new DateTime(2020,5,8, 14,0,0),
                Room = context.Rooms.FirstOrDefault(x => x.RoomNr == 107)
            });
            
            //Commit changes
            context.SaveChanges();
        }
    }
}