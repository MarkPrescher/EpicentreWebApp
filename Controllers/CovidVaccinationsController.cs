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
            return View(await _context.CovidVaccination.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("VACCINATION_ID,VACCINATION_TYPE,VACCINATION_DATE,VACCINATION_NEXT_DATE,VACCINATION_STATUS,USER_ID")] CovidVaccination covidVaccination)
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
        public async Task<IActionResult> Edit(Guid id, [Bind("VACCINATION_ID,VACCINATION_TYPE,VACCINATION_DATE,VACCINATION_NEXT_DATE,VACCINATION_STATUS,USER_ID")] CovidVaccination covidVaccination)
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
    }
}
