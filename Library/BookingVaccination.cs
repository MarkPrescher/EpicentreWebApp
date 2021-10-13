using Epicentre.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epicentre.Library
{
    public class BookingVaccination
    {
        private readonly EpicentreDataContext _context;
        public readonly static string AVAILABLE = "Available";
        public readonly static string FULLY_BOOKED = "Fully Booked!";

        public BookingVaccination(EpicentreDataContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckBookingAvailability(string timeSlot)
        {
            bool isFullyBooked = false;

            if (await CheckNumberOfBookings(timeSlot) >= 4)
                isFullyBooked = true;

            return isFullyBooked;
        }

        private async Task<int> CheckNumberOfBookings(string timeSlot)
        {
            int numberOfBookings = await _context.CovidVaccination.Where(b => b.VACCINATION_LOCATION == CovidVaccinationDetails.VaccinationLocation && b.VACCINATION_DATE == CovidVaccinationDetails.VaccinationDate.ToString() && b.VACCINATION_TIME == timeSlot).CountAsync();
  
            return numberOfBookings;
        }
    }

}
