using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Interfaces
{
    public interface IRoomService
    {
        int AddRoom(RoomDTO roomDto);
        List<RoomDTO> GetAllRooms();
        RoomDTO? GetRoomByNumber(int roomNumber);
        RoomDTO? GetRoomById(int roomId);
        List<RoomDTO> GetAvailableRooms(DateTime arrival, DateTime departure);
        bool UpdateRoom(RoomDTO roomDto);
        bool DeleteRoom(int roomId);

        bool IsRoomNumberUnique(int roomNumber);
    }
}
