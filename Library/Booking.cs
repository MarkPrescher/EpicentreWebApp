using Epicentre.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epicentre.Library
{
    public class Booking
    {
        private readonly EpicentreDataContext _context;
        public readonly static string AVAILABLE = "Available";
        public readonly static string FULLY_BOOKED = "Fully Booked!";

        public Booking(EpicentreDataContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckBookingAvailability(string timeSlot)
        {
            bool isFullyBooked = false;

            if (await CheckNumberOfBookings(timeSlot) >= 2)
                isFullyBooked = true;

            return isFullyBooked;
        }

        private async Task<int> CheckNumberOfBookings(string timeSlot)
        {
            int numberOfBookings = await _context.CovidTest.Where(b => b.TEST_LOCATION == CovidTestDetails.TestLocation && b.TEST_DATE == CovidTestDetails.TestDate.ToString() && b.TEST_TIME == timeSlot).CountAsync();

            return numberOfBookings;
        }
    }
}