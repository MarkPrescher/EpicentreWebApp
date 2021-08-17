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
    public class CovidResultsController : Controller
    {
        private readonly EpicentreDataContext _context;

        public CovidResultsController(EpicentreDataContext context)
        {
            _context = context;
        }

        // GET: CovidResults
        public async Task<IActionResult> Index()
        {
            return View(await _context.CovidTest.ToListAsync());
        }

        // GET: CovidResults/Details/5
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

        // GET: CovidResults/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CovidResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: CovidResults/Edit/5
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

        // POST: CovidResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: CovidResults/Delete/5
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

        // POST: CovidResults/Delete/5
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
    }
}
