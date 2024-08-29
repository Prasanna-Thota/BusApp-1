using BusApp.Models.Dto;
using BusApplication.Data;
using BusApplication.Models;
using BusApplication.Models.Dto;
using BusApplication.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusApplication.Controllers
{
    [Route("api/busDetails")]
    [ApiController]
    public class BusDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public BusDetailsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost("create")]
        public ActionResult<BusDetailsEntity> addBusDetails([FromBody] BusDetails busDetails)
        {
            if (busDetails == null)
            {
                return BadRequest("Bus details are empty.");
            }

            var existingBus = _db.Bus_Details.FirstOrDefault(x => x.BusNumber == busDetails.BusNumber);
            if (existingBus != null)
            {
                return BadRequest("Bus is already entered.");
            }

            BusDetailsEntity bus = new BusDetailsEntity
            {
                BusNumber = busDetails.BusNumber,
                BusName = busDetails.BusName,
                From = busDetails.From,
                To = busDetails.To,
                BoardingPoints = busDetails.BoardingPoints,
                DroppingPoints = busDetails.DroppingPoints,
                OperatorName = busDetails.OperatorName,
                OperatorNumber = busDetails.OperatorNumber,
                SleeperNonSleeper = busDetails.SleeperNonSleeper,
                AcNonAc = busDetails.AcNonAc,
                DepartureTime = busDetails.DepartureTime,
                ArrivalTime = busDetails.ArrivalTime,
                Duration = busDetails.Duration,
                LowerBerthPrice = busDetails.LowerBerthPrice,
                UpperBerthPrice = busDetails.UpperBerthPrice,
                TotalSeats = busDetails.TotalSeats,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                AvailableDates = busDetails.AvailableDates ?? new List<DateTime>(), // Adjust type if necessary
            };

            try
            {
                _db.Bus_Details.Add(bus);
                _db.SaveChanges();
                Console.WriteLine("Bus details saved successfully.");
                return Ok(bus);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error saving bus details: " + e.Message);
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpGet("get")]
        public ActionResult<List<BusDetailsDto>> getData()
        {
            var busDetails = _db.Bus_Details.Select(b => new BusDetailsDto
            {
                Id = b.Id,
                BusName = b.BusName,
                BusNumber = b.BusNumber,
                From = b.From,
                BoardingPoints = b.BoardingPoints,
                To = b.To,
                DroppingPoints = b.DroppingPoints,
                SleeperNonSleeper = b.SleeperNonSleeper,
                AcNonAc = b.AcNonAc,
                DepartureTime = b.DepartureTime,
                ArrivalTime = b.ArrivalTime,
                Duration = b.Duration,
                LowerBerthPrice = b.LowerBerthPrice,
                UpperBerthPrice = b.UpperBerthPrice,
                TotalSeats = b.TotalSeats,
                OperatorName = b.OperatorName,
                OperatorNumber = b.OperatorNumber,
                AvailableDates = b.AvailableDates,

            }).ToList();

            return Ok(busDetails);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBusDetails(int id, [FromBody] BusDetails updatedBusDetail)
        {

            var existingBusDetail = _db.Bus_Details.FirstOrDefault(b => b.Id == id);
            if (existingBusDetail == null)
            {
                return NotFound();
            }


            existingBusDetail.Id = id;
            existingBusDetail.BusName = updatedBusDetail.BusName;
            existingBusDetail.BusNumber = updatedBusDetail.BusNumber;
            existingBusDetail.From = updatedBusDetail.From;
            existingBusDetail.To = updatedBusDetail.To;
            existingBusDetail.OperatorName = updatedBusDetail.OperatorName;
            existingBusDetail.OperatorNumber = updatedBusDetail.OperatorNumber;
            existingBusDetail.DepartureTime = updatedBusDetail.DepartureTime;
            existingBusDetail.ArrivalTime = updatedBusDetail.ArrivalTime;
            existingBusDetail.AcNonAc = updatedBusDetail.AcNonAc;
            existingBusDetail.SleeperNonSleeper = updatedBusDetail.SleeperNonSleeper;
            existingBusDetail.BoardingPoints = updatedBusDetail.BoardingPoints;
            existingBusDetail.DroppingPoints = updatedBusDetail.DroppingPoints;
            existingBusDetail.LowerBerthPrice = updatedBusDetail.LowerBerthPrice;
            existingBusDetail.UpperBerthPrice = updatedBusDetail.UpperBerthPrice;
            existingBusDetail.TotalSeats = updatedBusDetail.TotalSeats;

            existingBusDetail.CreatedDate = existingBusDetail.CreatedDate; // Assuming CreatedDate is set during creation and shouldn't change
            existingBusDetail.UpdatedDate = DateTime.Now; // Update UpdatedDate to current time
            existingBusDetail.AvailableDates = updatedBusDetail.AvailableDates ?? new List<DateTime>();

            try
            {
                _db.SaveChanges();
                return Ok("Updated Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating bus details: {ex.Message}");
            }
        }

        [HttpDelete("Delete/{id}")]
        public ActionResult DeleteBusDetails(int id)
        {
            var bus = _db.Bus_Details.Find(id);
            if (bus == null)
            {
                return NotFound();
            }

            _db.Bus_Details.Remove(bus);
            _db.SaveChanges();

            return Ok("Bus Deleted Successfully");
        }


        [HttpGet("fromandto")]
        public ActionResult<List<BustFromandToDto>> GetFromAndToDetails()
        {
            var details = _db.Bus_Details
                     .Select(b => new { b.From, b.To })
                     .Distinct()
                     .Select(b => new BustFromandToDto
                     {
                         From = b.From,
                         To = b.To
                     })
                     .ToList();

            return Ok(details);
        }



        [HttpGet("busdata/{from}/{to}/{date}")]
        public ActionResult<List<BusDetailsDto>> GetBusDetails(string from, string to, DateTime date)
        {
            try
            {
                // Normalize the date to ignore time part for comparison
                var normalizedDate = date.Date;

                var bus = _db.Bus_Details
                             .Where(b => b.From == from && b.To == to &&
                                         b.AvailableDates.Any(d => d.Date == normalizedDate))
                             .Select(b => new BusDetailsDto
                             {
                                 Id = b.Id,
                                 BusName = b.BusName,
                                 BusNumber = b.BusNumber,
                                 From = b.From,
                                 BoardingPoints = b.BoardingPoints,
                                 To = b.To,
                                 DroppingPoints = b.DroppingPoints,
                                 SleeperNonSleeper = b.SleeperNonSleeper,
                                 AcNonAc = b.AcNonAc,
                                 DepartureTime = b.DepartureTime,
                                 ArrivalTime = b.ArrivalTime,
                                 Duration = b.Duration,
                                 LowerBerthPrice = b.LowerBerthPrice,
                                 UpperBerthPrice = b.UpperBerthPrice,
                                 TotalSeats = b.TotalSeats,
                                 OperatorName = b.OperatorName,
                                 OperatorNumber = b.OperatorNumber,
                             })
                             .ToList();
                return Ok(bus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("bookingDetails")]
        public ActionResult CreateBookingDetails([FromBody] BookingDetails model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Map BookingDetails to BookingDetailsEntity
                var entity = new BookingDetailsEntity
                {
                    UserName = model.UserName,
                    BusNumber = model.BusNumber,
                    Id = model.Id,
                    BusName = model.BusName,
                    Date = model.Date,
                    From = model.From,
                    To = model.To,
                    Contact = new Models.Entity.ContactDetails
                    {
                        Email = model.Contact.Email,
                        Phone = model.Contact.Phone
                    },
                    Passengers = new List<Models.Entity.Passenger>(),
                    TotalAmount = model.TotalAmount,
                    TotalWithGst = model.TotalWithGst,
                    GstAmount = model.GstAmount,
                    GstNumber = model.GstNumber,
                    SelectedSeats = model.SelectedSeats,
                    CreatedDate = DateTime.Now
                };

                foreach (var passenger in model.Passengers)
                {
                    entity.Passengers.Add(new Models.Entity.Passenger
                    {
                        Name = passenger.Name,
                        Gender = passenger.Gender,
                        Age = passenger.Age
                    });
                }

                // Add entity to context and save changes
                _db.Booking_Details.Add(entity);
                _db.SaveChanges();

                // Simulated success response
                return Ok("Booking details created successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception here or handle it based on your application's error handling strategy
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getSeats")]
        public ActionResult GetBookedSeats(int busId, string busNumber, DateTime date)
        {
            var bookingDetails = _db.Booking_Details
                .Where(b => b.Id == busId && b.BusNumber == busNumber && b.Date == date) // Filtering by both busId and busNumber
                .ToList(); // Materialize the query to fetch BookingDetails entities

            // Extract selected seats from each BookingDetails entity
            var bookedSeats = bookingDetails
                .SelectMany(b => b.SelectedSeats) // Flatten the selected seats from all rows
                .ToList(); // Materialize the query to fetch selected seats

            return Ok(bookedSeats);
        }

        [HttpGet("getSeatsCount/{busId}/{busNumber}/{date}")]
        public ActionResult<int> GetBookedSeatsCount(int busId, string busNumber, DateTime date)
        {
            try
            {
                var bookedSeatsCount = _db.Booking_Details
                        .Where(b => b.Id == busId && b.BusNumber == busNumber && b.Date == date)
                        .ToList()  // Materialize the query here
                        .SelectMany(b => b.SelectedSeats)
                        .Count();

                return Ok(bookedSeatsCount);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving booked seats count: {ex.Message}");
            }
        }


        [HttpPost("getDates")]
        public ActionResult<List<OrderCountResponse>> GetOrdersByDates(List<DateTime> dates)
        {
            if (dates == null || dates.Count == 0)
            {
                return BadRequest("No dates provided.");
            }

            var result = new List<OrderCountResponse>();

            try
            {
                foreach (var date in dates)
                {
                    // Calculate order count for the given date
                    var orderCount = _db.Booking_Details.Count(order => order.Date == date);
                    result.Add(new OrderCountResponse { Date = date, OrderCount = orderCount });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                Console.WriteLine($"Error fetching order data: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}
