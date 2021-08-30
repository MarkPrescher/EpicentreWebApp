using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Epicentre.Data;
using Epicentre.Models;

namespace Epicentre.Controllers
{
    public class CovidTestsController : Controller
    {
        private readonly EpicentreDataContext _context;

        CovidTest covidTest = new CovidTest();

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
        public IActionResult RegisterCovidTest()
        {
            return View();
        }

        // Query a table regarding time slots and availability???
        public IActionResult TimeBooking(string testType, string testLocation)
        {
            ViewBag.CovidTestType = "";
            ViewBag.TestingLocation = "";

            if (testType == null || testLocation == null)
            {
                return NotFound();
            }
            if (testType.Equals("dZr9RcABjq"))
            {
                ViewBag.CovidTestType = "PCR Swab Test";
            }
            if (testType.Equals("SkwFaATkYv"))
            {
                ViewBag.CovidTestType = "Rapid Antigen Test";
            }
            if (testType.Equals("hcYPmydyWZ"))
            {
                ViewBag.CovidTestType = "Antibody Test";
            }
            if (testLocation.Equals("Hillcrest"))
            {
                ViewBag.TestingLocation = "Hillcrest, KwaZulu-Natal";
            }
            if (testLocation.Equals("Durban Central"))
            {
                ViewBag.TestingLocation = "Durban Central, KwaZulu-Natal";
            }
            covidTest.TEST_TYPE = ViewBag.CovidTestType; // populating the model here?
            covidTest.TEST_LOCATION = ViewBag.TestingLocation; // populating the model here?
            return View();
        }

        // Displaying all the final details. Insertion to the database does NOT happen here, this action method will call the Book() method which will run the code to insert into database
        public IActionResult ConfirmBooking()
        {
            ViewBag.CovidTestType = covidTest.TEST_TYPE;
            return View();
        }

        // This is NOT a method that is related to a view! A normal method
        [HttpPost]
        public void Book()
        {
            // Logic to insert into database using EF (Entity Framework) Core, such as _context.Add(covidTest); await _context.SaveChangesAsync(); etc.
            // Depending on the results (if statement to check successful insertion), it will return to a view (action method) that will either display a success message or a failure message
            // Populate the model here? Such that covidTest.TEST_TYPE = "XXX";
        }

        public IActionResult SuccessfulBooking()
        {
            return View(); // Not created
        }

        public IActionResult FailedBooking()
        {
            return View(); // Not created
        }
    }
}
