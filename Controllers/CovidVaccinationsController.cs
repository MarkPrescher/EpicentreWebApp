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
using System.Net.Mail;

namespace Epicentre.Controllers
{
    public class CovidVaccinationsController : Controller
    {
        private readonly EpicentreDataContext _context;

        public CovidVaccinationsController(EpicentreDataContext context)
        {
            _context = context;
        }

        // GET: CovidVaccinations
        public async Task<IActionResult> Index()
        {
            var user = await _context.UserDetail.FirstOrDefaultAsync(m => m.EMAIL_ADDRESS == UserActions.UserEmail);
            string id;
            id = user.ID.ToString();
            id = id.ToUpper();
            var covidVaccination = _context.CovidVaccination.Where(m => m.USER_ID == id).ToList();
            return View(covidVaccination);
        }

        // GET: CovidVaccinations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var covidVaccination = await _context.CovidVaccination
                .FirstOrDefaultAsync(m => m.VACCINATION_ID == id);
            if (covidVaccination == null)
            {
                return NotFound();
            }

            return View(covidVaccination);
        }

        // GET: CovidVaccinations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CovidVaccinations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VACCINATION_ID,VACCINATION_TYPE, VACCINATION_LOCATION, VACCINATION_DATE,VACCINATION_NEXT_DATE, VACCINATION_TIME, VACCINATION_STATUS,USER_ID")] CovidVaccination covidVaccination)
        {
            if (ModelState.IsValid)
            {
                covidVaccination.VACCINATION_ID = Guid.NewGuid();
                _context.Add(covidVaccination);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(covidVaccination);
        }

        // GET: CovidVaccinations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var covidVaccination = await _context.CovidVaccination.FindAsync(id);
            if (covidVaccination == null)
            {
                return NotFound();
            }
            return View(covidVaccination);
        }

        // POST: CovidVaccinations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("VACCINATION_ID,VACCINATION_TYPE, VACCINATION_LOCATION, VACCINATION_DATE,VACCINATION_NEXT_DATE, VACCINATION_TIME, VACCINATION_STATUS,USER_ID")] CovidVaccination covidVaccination)
        {
            if (id != covidVaccination.VACCINATION_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(covidVaccination);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CovidVaccinationExists(covidVaccination.VACCINATION_ID))
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
            return View(covidVaccination);
        }

        // GET: CovidVaccinations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var covidVaccination = await _context.CovidVaccination
                .FirstOrDefaultAsync(m => m.VACCINATION_ID == id);
            if (covidVaccination == null)
            {
                return NotFound();
            }

            return View(covidVaccination);
        }

        // POST: CovidVaccinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var covidVaccination = await _context.CovidVaccination.FindAsync(id);
            _context.CovidVaccination.Remove(covidVaccination);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CovidVaccinationExists(Guid id)
        {
            return _context.CovidVaccination.Any(e => e.VACCINATION_ID == id);
        }


        // The original "Create" method - not inserting into database though - only inserts in the Book() method.
        public async Task<IActionResult> RegisterCovidVaccination()
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

            UserInformationDetails.FirstName = details.FIRST_NAME;
            UserInformationDetails.LastName = details.LAST_NAME;
            return View();
        }



        public async Task<IActionResult> TimeBookingVaccination(string vaccinationLocation, string vaccinationDate)
        {
            // If there are no params, we need to throw not found, otherwise user can bypass booking stage
            if (vaccinationLocation == null || vaccinationDate == null)
            {
                return NotFound();
            }

            // Converting test date to correct format
            var date = Convert.ToDateTime(vaccinationDate);
            vaccinationDate = date.ToString("MM/dd/yyyy");

            // Checking to see what the parameter text is equal to
            ViewBag.VaccinationDate = vaccinationDate;
            ViewBag.VaccinationLocation = UrlParams.GetVaccinationLocation(vaccinationLocation);

            // Assigning to static variables so that these values are retained and can be called to be placed in the model when inserting into DB
            CovidVaccinationDetails.VaccinationLocation = ViewBag.VaccinationLocation;
            CovidVaccinationDetails.VaccinationDate = vaccinationDate; // we can use the parameter because its not jumbled

            // Checks to see if it is weekday or weekend for the test date to filter time slots
            bool isWeekDay = TimeSlots.CheckIfWeekDay(date);
            BookingVaccination booking = new BookingVaccination(_context);
            int timeSlotCounter = TimeSlots.timeSlotCounter;

            if (isWeekDay)
            {
                //Used to determine what timeslots are shown to user
                ViewBag.Day = "Weekday";

                ViewBag.TS0800 = BookingVaccination.AVAILABLE;
                ViewBag.TS0815 = BookingVaccination.AVAILABLE;
                ViewBag.TS0830 = BookingVaccination.AVAILABLE;
                ViewBag.TS0845 = BookingVaccination.AVAILABLE;

                ViewBag.TS0900 = BookingVaccination.AVAILABLE;
                ViewBag.TS0915 = BookingVaccination.AVAILABLE;
                ViewBag.TS0930 = BookingVaccination.AVAILABLE;
                ViewBag.TS0945 = BookingVaccination.AVAILABLE;

                ViewBag.TS1000 = BookingVaccination.AVAILABLE;
                ViewBag.TS1015 = BookingVaccination.AVAILABLE;
                ViewBag.TS1030 = BookingVaccination.AVAILABLE;
                ViewBag.TS1045 = BookingVaccination.AVAILABLE;

                ViewBag.TS1100 = BookingVaccination.AVAILABLE;
                ViewBag.TS1115 = BookingVaccination.AVAILABLE;
                ViewBag.TS1130 = BookingVaccination.AVAILABLE;
                ViewBag.TS1145 = BookingVaccination.AVAILABLE;

                ViewBag.TS1200 = BookingVaccination.AVAILABLE;
                ViewBag.TS1215 = BookingVaccination.AVAILABLE;
                ViewBag.TS1230 = BookingVaccination.AVAILABLE;
                ViewBag.TS1245 = BookingVaccination.AVAILABLE;

                ViewBag.TS1300 = BookingVaccination.AVAILABLE;
                ViewBag.TS1315 = BookingVaccination.AVAILABLE;
                ViewBag.TS1330 = BookingVaccination.AVAILABLE;
                ViewBag.TS1345 = BookingVaccination.AVAILABLE;

                ViewBag.TS1400 = BookingVaccination.AVAILABLE;
                ViewBag.TS1415 = BookingVaccination.AVAILABLE;
                ViewBag.TS1430 = BookingVaccination.AVAILABLE;
                ViewBag.TS1445 = BookingVaccination.AVAILABLE;

                ViewBag.TS1500 = BookingVaccination.AVAILABLE;
                ViewBag.TS1515 = BookingVaccination.AVAILABLE;
                ViewBag.TS1530 = BookingVaccination.AVAILABLE;
                ViewBag.TS1545 = BookingVaccination.AVAILABLE;

                ViewBag.TS1600 = BookingVaccination.AVAILABLE;




                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0800 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0815 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0830 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0845 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0900 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0915 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0930 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0945 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1000 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1015 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1030 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1045 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1100 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1115 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1130 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1145 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;


                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1200 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1215 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1230 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1245 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1300 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1315 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1330 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1345 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1400 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1415 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1430 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1445 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1500 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1515 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1530 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1545 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKDAY_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1600 = BookingVaccination.FULLY_BOOKED;

            }
            else
            {
                //Used to determine what timeslots are shown to user
                ViewBag.Day = "Weekend";


                ViewBag.TS0800 = BookingVaccination.AVAILABLE;
                ViewBag.TS0815 = BookingVaccination.AVAILABLE;
                ViewBag.TS0830 = BookingVaccination.AVAILABLE;
                ViewBag.TS0845 = BookingVaccination.AVAILABLE;

                ViewBag.TS0900 = BookingVaccination.AVAILABLE;
                ViewBag.TS0915 = BookingVaccination.AVAILABLE;
                ViewBag.TS0930 = BookingVaccination.AVAILABLE;
                ViewBag.TS0945 = BookingVaccination.AVAILABLE;

                ViewBag.TS1000 = BookingVaccination.AVAILABLE;
                ViewBag.TS1015 = BookingVaccination.AVAILABLE;
                ViewBag.TS1030 = BookingVaccination.AVAILABLE;
                ViewBag.TS1045 = BookingVaccination.AVAILABLE;

                ViewBag.TS1100 = BookingVaccination.AVAILABLE;
                ViewBag.TS1115 = BookingVaccination.AVAILABLE;
                ViewBag.TS1130 = BookingVaccination.AVAILABLE;
                ViewBag.TS1145 = BookingVaccination.AVAILABLE;

                ViewBag.TS1200 = BookingVaccination.AVAILABLE;
                ViewBag.TS1215 = BookingVaccination.AVAILABLE;
                ViewBag.TS1230 = BookingVaccination.AVAILABLE;
                ViewBag.TS1245 = BookingVaccination.AVAILABLE;

                ViewBag.TS1300 = BookingVaccination.AVAILABLE;



                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0800 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0815 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0830 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0845 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0900 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0915 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0930 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS0945 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1000 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1015 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1030 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1045 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1100 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1115 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1130 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1145 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;


                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1200 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1215 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1230 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;
                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1245 = BookingVaccination.FULLY_BOOKED;
                timeSlotCounter++;

                if (await booking.CheckBookingAvailability(TimeSlots.WEEKEND_TIME_SLOTS[timeSlotCounter]))
                    ViewBag.TS1300 = BookingVaccination.FULLY_BOOKED;

            }

            return View();
        }


        // Displaying all the final details. Insertion to the database does NOT happen here, this action method will call the Book() method which will run the code to insert into database
        public IActionResult ConfirmBookingVaccination(string time)
        {
            if (time == null)
            {
                return NotFound();
            }
            CovidVaccinationDetails.VaccinationTime = time;

            ViewBag.VaccinationLocation = CovidVaccinationDetails.VaccinationLocation;
            ViewBag.VaccinationDate = CovidVaccinationDetails.VaccinationDate;
            ViewBag.VaccinationTime = time;

            return View();
        }

        public async Task<IActionResult> Book()
        {
            CovidVaccination covidVaccination = new CovidVaccination();
            covidVaccination.VACCINATION_ID = Guid.NewGuid();
            covidVaccination.VACCINATION_DATE = CovidVaccinationDetails.VaccinationDate;
            covidVaccination.VACCINATION_TIME = CovidVaccinationDetails.VaccinationTime;
            covidVaccination.VACCINATION_LOCATION = CovidVaccinationDetails.VaccinationLocation;
            
            //this must be changed later
            covidVaccination.VACCINATION_NEXT_DATE = "Temp";
            covidVaccination.VACCINATION_STATUS = "Temp";
            covidVaccination.VACCINATION_TYPE = "Unknown";
            covidVaccination.USER_ID = "1"; // must eventually get user id


            try
            {
                await _context.CovidVaccination.AddAsync(covidVaccination);
                await _context.SaveChangesAsync();

                string Data;
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new System.Net.NetworkCredential("noreplyepicentertest@gmail.com", "TestingPassword1");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress("noreplyepicentertest@gmail.com", "Epicentre");
                mail.To.Add(new MailAddress(UserActions.UserEmail));
                mail.Subject = "COVID-19 Vaccination Details";
                Data = "First Name: " + UserInformationDetails.FirstName + "\n" + "Last Name: " + UserInformationDetails.LastName + "\n";

                switch (covidVaccination.VACCINATION_LOCATION)
                {
                    case "Stellenbosch, Western Cape":
                        Data = Data + "Location : " + covidVaccination.VACCINATION_LOCATION + "(2 Groeneweide Rd, Stellenbosch, Cape Town, 7800)";
                        break;
                    case "Bellville, Western Cape":
                        Data = Data + "Location : " + covidVaccination.VACCINATION_LOCATION+ "(8 Zinnia Rd, Bloemhof, Cape Town, 7530)";
                        break;

                    case "Rondebosch, Western Cape":
                        Data = Data + "Location : " + covidVaccination.VACCINATION_LOCATION + "(5 Duke Avenue, Rondebosch, Cape Town, Western Cape)";
                        break;

                    case "Hillcrest, KwaZulu-Natal":
                        Data = Data + "Location : " + covidVaccination.VACCINATION_LOCATION + "(43a Old Main Rd, Hillcrest)";
                        break;
                    case "Pietermaritzburg, KwaZulu-Natal":
                        Data = Data + "Location : " + covidVaccination.VACCINATION_LOCATION+ "(Temp)";
                        break;
                    case "Durban Central, KwaZulu-Natal":
                        Data = Data + "Location : " + covidVaccination.VACCINATION_LOCATION + "(Temp)";
                        break;
                    case "Randburg, Gauteng":
                        Data = Data + "Location : " + covidVaccination.VACCINATION_LOCATION+ "(67 Dundalk Avenue, Parkview, Randburg, Gauteng)";
                        break;

                    case "Midrand, Gauteng":
                        Data = Data + "Location : " + covidVaccination.VACCINATION_LOCATION + "(Temp)";
                        break;
                }
                Data = Data + "\n" + "Type: " + covidVaccination.VACCINATION_TYPE+ "\n" + "Date: " + covidVaccination.VACCINATION_DATE+ "\n"
                    + "Time: " + covidVaccination.VACCINATION_TIME;
                mail.Body = Data;
                smtpClient.Send(mail);
                return RedirectToAction(nameof(SuccessfulBookingVaccination));
            }
            catch (Exception exception)
            {
                return RedirectToAction(nameof(FailedBookingVaccination));
            }
        }

        public IActionResult SuccessfulBookingVaccination()
        {
            ViewBag.VaccinationLocation = CovidVaccinationDetails.VaccinationLocation;
            ViewBag.VaccinationDate = CovidVaccinationDetails.VaccinationDate;
            ViewBag.VaccinationTime = CovidVaccinationDetails.VaccinationTime;

            return View();
        }

        public IActionResult FailedBookingVaccination()
        {
            return View();
        }


    }
}

