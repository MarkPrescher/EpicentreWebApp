using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Epicentre.Data;
using Epicentre.Models;
using Epicentre.Library;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Epicentre.Controllers
{
    [Authorize]
    public class CovidTestsController : Controller
    {
        private readonly EpicentreDataContext _context;

        public CovidTestsController(EpicentreDataContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index()
        {
            if (!UserActions.UserExists(_context) && !User.IsInRole("Nurse"))
            {
                return RedirectToAction("Index", "UserDetails");
            }
           
            var covidTest = await _context.CovidTest.Where(c => c.USER_EMAIL == UserActions.UserEmail).ToListAsync();
            return View(covidTest);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> RegisterCovidTest()
        {
            if (!UserActions.UserExists(_context) && !User.IsInRole("Nurse"))
            {
                return RedirectToAction("Index", "UserDetails");
            }
            var details = await _context.UserDetail.FirstOrDefaultAsync(m => m.EMAIL_ADDRESS == UserActions.UserEmail);

            ViewBag.FirstName = details.FIRST_NAME;
            ViewBag.LastName = details.LAST_NAME;
            ViewBag.ID = details.ID_NUMBER;
            ViewBag.Contact = details.CONTACT_NUMBER;
            ViewBag.Email = details.EMAIL_ADDRESS;
            ViewBag.Gender = details.GENDER;
            ViewBag.Medical = details.MEDICAL_AID;
            ViewBag.Membership = details.MEMBERSHIP_NUMBER;
            ViewBag.Auth = details.AUTH_NUMBER;

            UserInformationDetails.FirstName = details.FIRST_NAME;
            UserInformationDetails.LastName = details.LAST_NAME;
            return View();
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> TimeBooking(string testType, string testLocation, string testDate)
        {
            if (!UserActions.UserExists(_context) && !User.IsInRole("Nurse"))
            {
                return RedirectToAction("Index", "UserDetails");
            }

            if (testType == null || testLocation == null || testDate == null)
            {
                return NotFound();
            }

            // Converting test date to correct format
            var date = Convert.ToDateTime(testDate);
            testDate = date.ToString("MM/dd/yyyy");

            // Checking to see what the parameter text is equal to
            ViewBag.CovidTestType = UrlParams.GetTestType(testType);
            ViewBag.TestDate = testDate; // Do not jumble like test type, and testing location!
            ViewBag.TestingLocation = UrlParams.GetTestLocation(testLocation); // Read below comment

            // Assigning to static variables so that these values are retained and can be called to be placed in the model when inserting into DB
            CovidTestDetails.TestType = ViewBag.CovidTestType;
            CovidTestDetails.TestLocation = ViewBag.TestingLocation;
            CovidTestDetails.TestDate = testDate; // we can use the parameter because its not jumbled

            // Checks to see if it is weekday or weekend for the test date to filter time slots
            string day = TimeSlots.CheckDay(date);
            Booking booking = new Booking(_context);
            int timeSlotCounter = TimeSlots.timeSlotCounter;

            if (day.Equals("Weekday"))
            {
                // Used to determine what timeslots are shown to user
                ViewBag.Day = "Weekday";
                ViewBag.TS0900 = Booking.AVAILABLE;
                ViewBag.TS0915 = Booking.AVAILABLE;
                ViewBag.TS0930 = Booking.AVAILABLE;
                ViewBag.TS0945 = Booking.AVAILABLE;

                ViewBag.TS1000 = Booking.AVAILABLE;
                ViewBag.TS1015 = Booking.AVAILABLE;
                ViewBag.TS1030 = Booking.AVAILABLE;
                ViewBag.TS1045 = Booking.AVAILABLE;

                ViewBag.TS1100 = Booking.AVAILABLE;
                ViewBag.TS1115 = Booking.AVAILABLE;
                ViewBag.TS1130 = Booking.AVAILABLE;
                ViewBag.TS1145 = Booking.AVAILABLE;

                ViewBag.TS1200 = Booking.AVAILABLE;
                ViewBag.TS1215 = Booking.AVAILABLE;
                ViewBag.TS1230 = Booking.AVAILABLE;
                ViewBag.TS1245 = Booking.AVAILABLE;

                ViewBag.TS1300 = Booking.AVAILABLE;
                ViewBag.TS1315 = Booking.AVAILABLE;
                ViewBag.TS1330 = Booking.AVAILABLE;
                ViewBag.TS1345 = Booking.AVAILABLE;

                ViewBag.TS1400 = Booking.AVAILABLE;
                ViewBag.TS1415 = Booking.AVAILABLE;
                ViewBag.TS1430 = Booking.AVAILABLE;
                ViewBag.TS1445 = Booking.AVAILABLE;

                ViewBag.TS1500 = Booking.AVAILABLE;
                ViewBag.TS1515 = Booking.AVAILABLE;
                ViewBag.TS1530 = Booking.AVAILABLE;
                ViewBag.TS1545 = Booking.AVAILABLE;

                ViewBag.TS1600 = Booking.AVAILABLE;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0900 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0915 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0930 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0945 = Booking.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1000 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1015 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1030 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1045 = Booking.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1100 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1115 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1130 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1145 = Booking.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1200 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1215 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1230 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1245 = Booking.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1300 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1315 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1330 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1345 = Booking.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1400 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1415 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1430 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1445 = Booking.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1500 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1515 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1530 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1545 = Booking.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1600 = Booking.FULLY_BOOKED;
            }
            else if (day.Equals("Saturday"))
            {
                ViewBag.Day = "Saturday";
                ViewBag.TestLocation = CovidTestDetails.TestLocation;

                if (CovidTestDetails.TestLocation.Equals("Hillcrest, KwaZulu-Natal"))
                {
                    ViewBag.TS0800 = Booking.AVAILABLE;
                    ViewBag.TS0815 = Booking.AVAILABLE;
                    ViewBag.TS0830 = Booking.AVAILABLE;
                    ViewBag.TS0845 = Booking.AVAILABLE;

                    ViewBag.TS0900 = Booking.AVAILABLE;
                    ViewBag.TS0915 = Booking.AVAILABLE;
                    ViewBag.TS0930 = Booking.AVAILABLE;
                    ViewBag.TS0945 = Booking.AVAILABLE;

                    ViewBag.TS1000 = Booking.AVAILABLE;
                    ViewBag.TS1015 = Booking.AVAILABLE;
                    ViewBag.TS1030 = Booking.AVAILABLE;
                    ViewBag.TS1045 = Booking.AVAILABLE;

                    ViewBag.TS1100 = Booking.AVAILABLE;
                    ViewBag.TS1115 = Booking.AVAILABLE;
                    ViewBag.TS1130 = Booking.AVAILABLE;
                    ViewBag.TS1145 = Booking.AVAILABLE;

                    ViewBag.TS1200 = Booking.AVAILABLE;
                    ViewBag.TS1215 = Booking.AVAILABLE;
                    ViewBag.TS1230 = Booking.AVAILABLE;

                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0800 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0815 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0830 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0845 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;

                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0900 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0915 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0930 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0945 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;

                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1000 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1015 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1030 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1045 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;

                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1100 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1115 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1130 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1145 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;

                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1200 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1215 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.HILLCREST_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1230 = Booking.FULLY_BOOKED;
                }

                else if (CovidTestDetails.TestLocation.Equals("Randburg, Gauteng"))
                {
                    ViewBag.TS0900 = Booking.AVAILABLE;
                    ViewBag.TS0915 = Booking.AVAILABLE;
                    ViewBag.TS0930 = Booking.AVAILABLE;
                    ViewBag.TS0945 = Booking.AVAILABLE;

                    ViewBag.TS1000 = Booking.AVAILABLE;
                    ViewBag.TS1015 = Booking.AVAILABLE;
                    ViewBag.TS1030 = Booking.AVAILABLE;
                    ViewBag.TS1045 = Booking.AVAILABLE;

                    ViewBag.TS1100 = Booking.AVAILABLE;
                    ViewBag.TS1115 = Booking.AVAILABLE;
                    ViewBag.TS1130 = Booking.AVAILABLE;
                    ViewBag.TS1145 = Booking.AVAILABLE;

                    ViewBag.TS1200 = Booking.AVAILABLE;
                    ViewBag.TS1215 = Booking.AVAILABLE;
                    ViewBag.TS1230 = Booking.AVAILABLE;
                    ViewBag.TS1245 = Booking.AVAILABLE;

                    ViewBag.TS1300 = Booking.AVAILABLE;
                    ViewBag.TS1315 = Booking.AVAILABLE;
                    ViewBag.TS1330 = Booking.AVAILABLE;
                    ViewBag.TS1345 = Booking.AVAILABLE;

                    ViewBag.TS1400 = Booking.AVAILABLE;
                    ViewBag.TS1415 = Booking.AVAILABLE;
                    ViewBag.TS1430 = Booking.AVAILABLE;

                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0900 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0915 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0930 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0945 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;

                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1000 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1015 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1030 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1045 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;

                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1100 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1115 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1130 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1145 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;

                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1200 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1215 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1230 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1245 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;

                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1300 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1315 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1330 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1345 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;

                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1400 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1415 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RANDBURG_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1430 = Booking.FULLY_BOOKED;
                }

                else if (CovidTestDetails.TestLocation.Equals("Rondebosch, Western Cape"))
                {
                    ViewBag.TS0830 = Booking.AVAILABLE;
                    ViewBag.TS0845 = Booking.AVAILABLE;

                    ViewBag.TS0900 = Booking.AVAILABLE;
                    ViewBag.TS0915 = Booking.AVAILABLE;
                    ViewBag.TS0930 = Booking.AVAILABLE;
                    ViewBag.TS0945 = Booking.AVAILABLE;

                    ViewBag.TS1000 = Booking.AVAILABLE;
                    ViewBag.TS1015 = Booking.AVAILABLE;
                    ViewBag.TS1030 = Booking.AVAILABLE;
                    ViewBag.TS1045 = Booking.AVAILABLE;

                    ViewBag.TS1100 = Booking.AVAILABLE;
                    ViewBag.TS1115 = Booking.AVAILABLE;
                    ViewBag.TS1130 = Booking.AVAILABLE;

                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0830 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0845 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;

                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0900 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0915 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0930 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS0945 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;

                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1000 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1015 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1030 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1045 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;

                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1100 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1115 = Booking.FULLY_BOOKED;
                    timeSlotCounter++;
                    if (await booking.CheckBookingAvailability(TimeSlots.RONDEBOSCH_SATURDAY_TIME_SLOTS[timeSlotCounter]))
                        ViewBag.TS1130 = Booking.FULLY_BOOKED;
                }
            }
            else
            {
                ViewBag.Day = "Sunday";
            }

            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult ConfirmBooking(string time)
        {
            if (!UserActions.UserExists(_context) && !User.IsInRole("Nurse"))
            {
                return RedirectToAction("Index", "UserDetails");
            }
            if (time == null)
            {
                return NotFound();
            }
            CovidTestDetails.TestTime = time;

            ViewBag.TestType = CovidTestDetails.TestType;
            ViewBag.TestLocation = CovidTestDetails.TestLocation;
            ViewBag.TestDate = CovidTestDetails.TestDate;
            ViewBag.TestTime = time;

            return View();
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Book()
        {
            if (!UserActions.UserExists(_context) && !User.IsInRole("Nurse"))
            {
                return RedirectToAction("Index", "UserDetails");
            }
            CovidTest covidTest = new CovidTest();
            covidTest.TEST_ID = Guid.NewGuid();
            covidTest.TEST_TYPE = CovidTestDetails.TestType;
            covidTest.TEST_DATE = CovidTestDetails.TestDate;
            covidTest.TEST_TIME = CovidTestDetails.TestTime;
            covidTest.TEST_LOCATION = CovidTestDetails.TestLocation;
            covidTest.TEST_STATUS = "Waiting for test...";
            covidTest.TEST_RESULT = "Awaiting...";
            covidTest.USER_EMAIL = UserActions.UserEmail;

            try
            {
                await _context.CovidTest.AddAsync(covidTest);
                await _context.SaveChangesAsync();

                string Data;
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new System.Net.NetworkCredential("noreplyepicentertest@gmail.com", "TestingPassword1");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress("noreplyepicentertest@gmail.com", "Epicentre");
                mail.To.Add(new MailAddress(UserActions.UserEmail));
                mail.Subject = "COVID-19 Test Details";
                Data = "First Name: " + UserInformationDetails.FirstName + "\n" + "Last Name: " + UserInformationDetails.LastName + "\n";

                switch (covidTest.TEST_LOCATION)
                {
                    case "Stellenbosch, Western Cape":
                        Data = Data + "Location : " + covidTest.TEST_LOCATION + "(2 Groeneweide Rd, Stellenbosch, Cape Town, 7800)";
                        break;
                    case "Bellville, Western Cape":
                        Data = Data + "Location : " + covidTest.TEST_LOCATION + "(8 Zinnia Rd, Bloemhof, Cape Town, 7530)";
                        break;

                    case "Rondebosch, Western Cape":
                        Data = Data + "Location : " + covidTest.TEST_LOCATION + "(5 Duke Avenue, Rondebosch, Cape Town, Western Cape)";
                        break;

                    case "Hillcrest, KwaZulu-Natal":
                        Data = Data + "Location : " + covidTest.TEST_LOCATION + "(43a Old Main Rd, Hillcrest)";
                        break;
                    case "Randburg, Gauteng":
                        Data = Data + "Location : " + covidTest.TEST_LOCATION + "(67 Dundalk Avenue, Parkview, Randburg, Gauteng)";
                        break;
                }
                Data = Data + "\n" + "Type: " + covidTest.TEST_TYPE + "\n" + "Date: " + covidTest.TEST_DATE + "\n"
                    + "Time: " + covidTest.TEST_TIME;
                mail.Body = Data;
                smtpClient.Send(mail);
                return RedirectToAction(nameof(SuccessfulBooking));
            }
            catch (Exception exception)
            {
                return RedirectToAction(nameof(FailedBooking));
            }
        }

        [Authorize(Roles = "User")]
        public IActionResult SuccessfulBooking()
        {
            ViewBag.TestType = CovidTestDetails.TestType;
            ViewBag.TestLocation = CovidTestDetails.TestLocation;
            ViewBag.TestDate = CovidTestDetails.TestDate;
            ViewBag.TestTime = CovidTestDetails.TestTime;

            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult FailedBooking()
        {
            return View();
        }

        [Authorize(Roles = "Nurse, Admin")]
        public async Task<IActionResult> SearchForPatient()
        {
            return View(await _context.CovidTest.ToListAsync());
        }

        [Authorize(Roles = "Nurse, Admin")]
        public async Task<IActionResult> Patients(string idNumber)
        {
            if (idNumber == null)
            {
                return NotFound();
            }

            ViewBag.ID = idNumber;
            var userDetails = await _context.UserDetail.Where(u => u.ID_NUMBER == idNumber).FirstOrDefaultAsync();
            var covidTest = await _context.CovidTest.Where(c => c.USER_EMAIL == userDetails.EMAIL_ADDRESS).ToListAsync();
            return View(covidTest);
        }

        [Authorize(Roles = "Nurse, Admin")]
        public async Task<IActionResult> UpdateStatus(string testId)
        {
            if (testId == null)
            {
                return NotFound();
            }

            var covidTest = await _context.CovidTest.Where(c => c.TEST_ID == Guid.Parse(testId)).FirstOrDefaultAsync();
            var userDetails = await _context.UserDetail.Where(u => u.EMAIL_ADDRESS == covidTest.USER_EMAIL).FirstOrDefaultAsync();

            string testStatus = covidTest.TEST_STATUS;
            switch (testStatus)
            {
                case "Waiting for test...":
                    covidTest.TEST_STATUS = "Sample taken";
                    break;
                case "Sample taken":
                    covidTest.TEST_STATUS = "Sample collected";
                    break;
                case "Sample collected":
                    covidTest.TEST_STATUS = "Analyzing sample...";
                    break;
                case "Analyzing sample...":
                    covidTest.TEST_STATUS = "Completed";
                    break;
                default: return NotFound();
            }

            _context.Update(covidTest);
            await _context.SaveChangesAsync();

            return RedirectToAction("Patients", "CovidTests", new { idNumber = userDetails.ID_NUMBER });
        }

        [Authorize(Roles = "Nurse, Admin")]
        public async Task<IActionResult> UpdateResult(string testId, string result)
        {
            if (testId == null || result == null)
            {
                return NotFound();
            }

            var covidTest = await _context.CovidTest.Where(c => c.TEST_ID == Guid.Parse(testId)).FirstOrDefaultAsync();
            var userDetails = await _context.UserDetail.Where(u => u.EMAIL_ADDRESS == covidTest.USER_EMAIL).FirstOrDefaultAsync();

            if (result == "Positive")
            {
                covidTest.TEST_RESULT = "Positive";
            }
            else
            {
                covidTest.TEST_RESULT = "Negative";
            }

            _context.Update(covidTest);
            await _context.SaveChangesAsync();

            var userEmail = await _context.CovidTest.Where(c => c.TEST_ID == Guid.Parse(testId)).Select(c => c.USER_EMAIL).FirstOrDefaultAsync();
            MailMessage mailMessage = new MailMessage("noreplyepicentertest@gmail.com", userEmail/*Recipient email*/, "COVID-19 Test Result", $"Your test result has been received.\n\nYou have tested {result} for COVID-19.");
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("noreplyepicentertest@gmail.com", "TestingPassword1");
            await smtpClient.SendMailAsync(mailMessage);

            return RedirectToAction("Patients", "CovidTests", new { idNumber = userDetails.ID_NUMBER });
        }

        [HttpPost]
        public async Task<IActionResult> SendFile(string testId, IFormFile file)
        {
            if (testId == null || file == null)
            {
                return NotFound();
            }

            var covidTest = await _context.CovidTest.Where(c => c.TEST_ID == Guid.Parse(testId)).FirstOrDefaultAsync();
            var userDetails = await _context.UserDetail.Where(u => u.EMAIL_ADDRESS == covidTest.USER_EMAIL).FirstOrDefaultAsync();

            MailMessage mailMessage = new MailMessage("noreplyepicentertest@gmail.com", userDetails.EMAIL_ADDRESS/*Recipient email*/, "COVID-19 Report", $"Please kindly see the attached document in this email.");

            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            var fileBytes = memoryStream.ToArray();
            Attachment attachment = new Attachment(new MemoryStream(fileBytes), file.FileName);
            mailMessage.Attachments.Add(attachment);
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("noreplyepicentertest@gmail.com", "TestingPassword1");
            await smtpClient.SendMailAsync(mailMessage);

            return RedirectToAction("Patients", "CovidTests", new { idNumber = userDetails.ID_NUMBER });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.CompletedCovidTests = await _context.CovidTest.CountAsync();
            ViewBag.OutstandingTests = await _context.CovidTest.Where(c => c.TEST_RESULT == "Awaiting...").CountAsync();
            ViewBag.PositiveTests = await _context.CovidTest.Where(c => c.TEST_RESULT == "Positive").CountAsync();
            ViewBag.NegativeTests = await _context.CovidTest.Where(c => c.TEST_RESULT == "Negative").CountAsync();
            string tomorrowDate = DateTime.Today.AddDays(1).ToString("MM/dd/yyyy");
            ViewBag.TestsTomorrow = await _context.CovidTest.Where(c => c.TEST_DATE == tomorrowDate).CountAsync();
            var pcr = await _context.CovidTest.Where(c => c.TEST_TYPE == "PCR Swab Test").CountAsync();
            var rapid = await _context.CovidTest.Where(c => c.TEST_TYPE == "Rapid Antigen Test").CountAsync();
            var antibody = await _context.CovidTest.Where(c => c.TEST_TYPE == "Antibody Test").CountAsync();
            if (pcr > rapid && pcr > antibody)
            {
                ViewBag.MostPopularTest = "PCR Swab Test";
            }
            else if (rapid > pcr && rapid > antibody)
            {
                ViewBag.MostPopularTest = "Rapid Antigen Test";
            }
            else
            {
                ViewBag.MostPopularTest = "Antibody Test";
            }
            List<int> counts = new List<int>();
            List<string> names = new List<string>();
            counts.Add(await _context.CovidTest.Where(c => c.TEST_LOCATION == "Hillcrest, KwaZulu-Natal").CountAsync());
            names.Add("Hillcrest, KwaZulu-Natal");
            counts.Add(await _context.CovidTest.Where(c => c.TEST_LOCATION == "Stellenbosch, Western Cape").CountAsync());
            names.Add("Stellenbosch, Western Cape");
            counts.Add(await _context.CovidTest.Where(c => c.TEST_LOCATION == "Bellville, Western Cape").CountAsync());
            names.Add("Bellville, Western Cape");
            counts.Add(await _context.CovidTest.Where(c => c.TEST_LOCATION == "Rondebosch, Western Cape").CountAsync());
            names.Add("Rondebosch, Western Cape");
            counts.Add(await _context.CovidTest.Where(c => c.TEST_LOCATION == "Randburg, Gauteng").CountAsync());
            names.Add("Randburg, Gauteng");
            var mostPopularSite = names[counts.IndexOf(counts.Max())];
            ViewBag.MostPopularSite = mostPopularSite;

            return View();
        }
    }
}