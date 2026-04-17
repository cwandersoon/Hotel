
SELECT * FROM Customers 
SELECT * FROM Rooms 
SELECT * FROM Invoices 
SELECT * FROM Bookings 


INSERT INTO Rooms(
    RoomNumber,
    [Type],
    [Size],
    ExtraBedCapacity,
    PricePerNight,
    IsDeleted)
VALUES(
    '301',
    'Double',
    'Large',
    2,
    2000,
    0);


SELECT
    Id,
    CustomerId,
    ArrivalDate,
    DepartureDate 
FROM Bookings 
WHERE IsDeleted = 0
ORDER BY ArrivalDate ASC;


UPDATE Invoices 
SET IsPaid = 1 
WHERE Id = 1;


UPDATE Rooms
SET IsDeleted = 1
WHERE Id = 1


DELETE FROM Rooms
WHERE Id = 1


-- Visar bokningslista med kundnamn och rumsnummer
SELECT 
    B.Id AS BookingId,
    C.FirstName + ' ' + C.LastName AS CustomerName,
    R.RoomNumber,
    B.ArrivalDate,
    B.DepartureDate,
    B.TotalPrice
FROM Bookings B
JOIN Customers C ON B.CustomerId = C.Id
JOIN Rooms R ON B.RoomId = R.Id

-- Visar antal betalda/obetalda fakturor och totalsumma av de båda
SELECT 
    IsPaid, 
    SUM(TotalAmount) AS TotalSum,
    COUNT(*) AS NumberOfInvoices
FROM Invoices
GROUP BY IsPaid;

-- Visar kunder som har obetalda fakturor
SELECT 
    Id,
    FirstName,
    LastName
FROM Customers
WHERE Id IN (
    SELECT B.CustomerId 
    FROM Bookings B
    JOIN Invoices I ON B.Id = I.BookingId
    WHERE I.IsPaid = 0
);