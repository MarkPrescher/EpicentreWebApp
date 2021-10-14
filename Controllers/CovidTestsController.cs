using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Epicentre.Data;
using Epicentre.Models;
using Epicentre.Library;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;

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

        public async Task<IActionResult> Index()
        {
            return View(await _context.CovidTest.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var covidTest = await _context.CovidTest
                .FirstOrDefaultAsync(m => m.TEST_ID == id);
            if (covidTest == null)
            {
                return NotFound();
            }

            return View(covidTest);
        }

        // !!! *** OBSOLETE *** !!!
        public IActionResult Create()
        {
            return View();
        }

        // !!! *** OBSOLETE *** !!!
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TEST_ID,TEST_TYPE,TEST_DATE,TEST_STATUS,TEST_RESULT,USER_ID")] CovidTest covidTest)
        {
            if (ModelState.IsValid)
            {
                covidTest.TEST_ID = Guid.NewGuid();
                _context.Add(covidTest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(covidTest);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var covidTest = await _context.CovidTest.FindAsync(id);
            if (covidTest == null)
            {
                return NotFound();
            }
            return View(covidTest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("TEST_ID,TEST_TYPE,TEST_DATE,TEST_STATUS,TEST_RESULT,USER_ID")] CovidTest covidTest)
        {
            if (id != covidTest.TEST_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(covidTest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CovidTestExists(covidTest.TEST_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(covidTest);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var covidTest = await _context.CovidTest
                .FirstOrDefaultAsync(m => m.TEST_ID == id);
            if (covidTest == null)
            {
                return NotFound();
            }

            return View(covidTest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var covidTest = await _context.CovidTest.FindAsync(id);
            _context.CovidTest.Remove(covidTest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CovidTestExists(Guid id)
        {
            return _context.CovidTest.Any(e => e.TEST_ID == id);
        }

        // The original "Create" method - not inserting into database though - only inserts in the Book() method.
        public async Task<IActionResult> RegisterCovidTest()
        {
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
            return View();
        }

        // !!!!!!!! Check to see if these can be passed as hidden parameter using POST, instead of JavaScript !!!!!!!!
        public async Task<IActionResult> TimeBooking(string testType, string testLocation, string testDate)
        {
            // If there are no params, we need to throw not found, otherwise user can bypass booking stage
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
            bool isWeekDay = TimeSlots.CheckIfWeekDay(date);
            Booking booking = new Booking(_context);
            int timeSlotCounter = TimeSlots.timeSlotCounter;

            if (isWeekDay)
            {
                //Used to determine what timeslots are shown to user
                ViewBag.Day = "Weekday";
                // Time slots - these need to be finished! 
                // DONE!!!
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
              

                // times finish at 16:00 (ViewBag.TS1600)

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0800 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0815 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0830 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0845 = Booking.FULLY_BOOKED;
                timeSlotCounter++;

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

                // finish as well please
                // DONE!!!
            }
            else
            {
                //Used to determine what timeslots are shown to user
                ViewBag.Day = "Weekend";

                // Time slots - these need to be finished!
                // DONE!!!
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
                ViewBag.TS1245 = Booking.AVAILABLE;

                ViewBag.TS1300 = Booking.AVAILABLE;

                // times finish at 13:00 (ViewBag.TS1300)

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0800 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0815 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0830 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0845 = Booking.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0900 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0915 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0930 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0945 = Booking.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1000 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1015 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1030 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1045 = Booking.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1100 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1115 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1130 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1145 = Booking.FULLY_BOOKED;
                timeSlotCounter++;


                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1200 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1215 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1230 = Booking.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1245 = Booking.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1300 = Booking.FULLY_BOOKED;
                // finish as well please
                // DONE!!!
            }

            return View();
        }


        // Displaying all the final details. Insertion to the database does NOT happen here, this action method will call the Book() method which will run the code to insert into database
        public IActionResult ConfirmBooking(string time)
        {
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

        public async Task<IActionResult> Book()
        {
            CovidTest covidTest = new CovidTest();
            covidTest.TEST_ID = Guid.NewGuid();
            covidTest.TEST_TYPE = CovidTestDetails.TestType;
            covidTest.TEST_DATE = CovidTestDetails.TestDate;
            covidTest.TEST_TIME = CovidTestDetails.TestTime;
            covidTest.TEST_LOCATION = CovidTestDetails.TestLocation;
            covidTest.TEST_STATUS = "Awaiting...";
            covidTest.TEST_RESULT = "Awaiting...";
            covidTest.USER_ID = "1"; // must eventually get user id

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

                mail.From = new MailAddress("noreplyepicentertest@gmail.com", "Epicenter");
                mail.To.Add(new MailAddress(UserActions.UserEmail));
                mail.Subject = "Covid Test Details";
                Data = "First Name: " + UserInformationDetails.FirstName+ "\n" + "Last Name: " + UserInformationDetails.LastName +"\n" +  
                    "Location : " + covidTest.TEST_LOCATION + "\n" + "Type: " + covidTest.TEST_TYPE + "\n" + "Date: " + covidTest.TEST_DATE + "\n"
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

        public IActionResult SuccessfulBooking()
        {
            ViewBag.TestType = CovidTestDetails.TestType;
            ViewBag.TestLocation = CovidTestDetails.TestLocation;
            ViewBag.TestDate = CovidTestDetails.TestDate;
            ViewBag.TestTime = CovidTestDetails.TestTime;

            return View();
        }

        public IActionResult FailedBooking()
        {
            return View();
        }
    }
}
