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
                    Attribute = new Attribute {AirCon = true, Computers = true, Jacks = true, Presenter = true},
                    Building = "A",
                    RoomNr = 100,
                    Size = 30
                },
                new Room
                {
                    Attribute = new Attribute {AirCon = true, Computers = true, Jacks = true, Presenter = true},
                    Building = "A",
                    RoomNr = 101,
                    Size = 45
                },
                new Room
                {
                    Attribute = new Attribute {AirCon = true, Computers = true, Jacks = true, Presenter = true},
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
                    Attribute = new Attribute {AirCon = true, Computers = true, Jacks = true, Presenter = true},
                    Building = "A",
                    RoomNr = 104,
                    Size = 35
                },
                new Room
                {
                    Attribute = new Attribute {AirCon = true, Computers = true, Jacks = true, Presenter = true},
                    Building = "A",
                    RoomNr = 105,
                    Size = 30
                },
                new Room
                {
                    Attribute = new Attribute {AirCon = true, Computers = true, Jacks = true, Presenter = true},
                    Building = "A",
                    RoomNr = 106,
                    Size = 30
                },
                new Room
                {
                    Attribute = new Attribute {AirCon = true, Computers = true, Jacks = true, Presenter = true},
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
            
            //Seed Data into Rights Table 
            var rightsList = new List<Right>
            {
                new Right
                {
                    RightsName = "Verwaltung",
                    UserHasRight = new List<User>
                    {
                        new User
                        {
                            Username = "Verwaltung",Password = "123",Reservations = new List<Reservation>()
                        }
                    }
                },
                new Right
                {
                    RightsName = "Professor",
                    UserHasRight = new List<User>
                    {
                        new User
                        {
                            Username = "Professor",Password = "123",Reservations = new List<Reservation>()
                        }
                    }
                },
                new Right
                {
                    RightsName = "Student",
                    UserHasRight = new List<User>
                    {
                        new User
                        {
                            Username = "Student",Password = "123",Reservations = new List<Reservation>()
                        }
                    }
                },
                new Right
                {
                    RightsName = "Lehrbeauftragter",
                    UserHasRight = new List<User>
                    {
                        new User
                        {
                            Username = "Lehrbeauftragter",Password = "123",Reservations = new List<Reservation>()
                        }
                    }
                },
                new Right
                {
                    RightsName = "Sonstige",
                    UserHasRight = new List<User>
                    {
                        new User
                        {
                            Username = "Sonstige",Password = "123",Reservations = new List<Reservation>()
                        }
                    }
                }
                
            };
            
            

            if (context.Rights != null) context.Rights.AddRange(rightsList);
            if (context.Rooms != null) context.Rooms.AddRange(roomList);


            //Commit changes
            //context.SaveChanges();
        }
    }
}