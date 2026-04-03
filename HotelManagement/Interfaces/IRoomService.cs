using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Interfaces
{
    public interface IRoomService
    {
        List<RoomDTO> GetAllRooms();
        RoomDTO? GetRoomByNumber(int number);
        List<RoomDTO> GetAvailableRooms(DateTime arrival, DateTime departure);
    }
}
