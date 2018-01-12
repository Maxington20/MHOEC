using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MHOEC.Models;
using Microsoft.AspNetCore.Http;

namespace MHOEC.Controllers
{
    public class MHTreatmentFertilizersController : Controller
    {
        private readonly OECContext _context;

        public MHTreatmentFertilizersController(OECContext context)
        {
            _context = context;
        }

        // GET: MHTreatmentFertilizers
        public async Task<IActionResult> Index(int? id)
        {
			if (id != null)
			{
				HttpContext.Session.SetInt32("treatmentId", Convert.ToInt32(id));
			}

			else
			{
				if (HttpContext.Session.GetInt32("treatmentId") != null)
				{
					id = HttpContext.Session.GetInt32("treatmentId");
				}

				else
				{
					TempData["Message"] = "Please select a Treatment to see its Fertilizer composition";
					return RedirectToAction("Index", "MHTreatments");
				}
			}

            var oECContext = _context.TreatmentFertilizer
				.Include(t => t.FertilizerNameNavigation)
				.Include(t => t.Treatment)
				.Where(t=> t.TreatmentId == id)
				.OrderBy(t=> t.FertilizerName);

            return View(await oECContext.ToListAsync());
        }

        // GET: MHTreatmentFertilizers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer
                .Include(t => t.FertilizerNameNavigation)
                .Include(t => t.Treatment)
                .SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            if (treatmentFertilizer == null)
            {
                return NotFound();
            }

            return View(treatmentFertilizer);
        }

        // GET: MHTreatmentFertilizers/Create
        public IActionResult Create()
        {
			TempData["treatmentId"] = HttpContext.Session.GetInt32("treatmentId");
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer.OrderBy(t=> t.FertilizerName), "FertilizerName", "FertilizerName");
			//ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "TreatmentId");

			ViewData["ratePerAcre"] = _context.TreatmentFertilizer
				.Where(t => t.TreatmentId == HttpContext.Session.GetInt32("treatmentId"))
				.Select(t => t.RatePerAcre)
				.FirstOrDefault();

			ViewData["rateMetric"] = _context.TreatmentFertilizer
				.Where(t => t.TreatmentId == HttpContext.Session.GetInt32("treatmentId"))
				.Select(t => t.RateMetric)
				.FirstOrDefault();

            return View();
        }

        // POST: MHTreatmentFertilizers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TreatmentFertilizerId,TreatmentId,FertilizerName,RatePerAcre,RateMetric")] TreatmentFertilizer treatmentFertilizer)
        {
			TempData["treatmentId"] = HttpContext.Session.GetInt32("treatmentId");
            if (ModelState.IsValid)
            {
                _context.Add(treatmentFertilizer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer.OrderBy(t=> t.FertilizerName), "FertilizerName", "FertilizerName", treatmentFertilizer.FertilizerName);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "TreatmentId", treatmentFertilizer.TreatmentId);

			ViewData["ratePerAcre"] = _context.TreatmentFertilizer
				.Where(t => t.TreatmentId == HttpContext.Session.GetInt32("treatmentId"))
				.Select(t => t.RatePerAcre)
				.FirstOrDefault();

			ViewData["rateMetric"] = _context.TreatmentFertilizer
				.Where(t => t.TreatmentId == HttpContext.Session.GetInt32("treatmentId"))
				.Select(t => t.RateMetric)
				.FirstOrDefault();

			return View(treatmentFertilizer);
        }

        // GET: MHTreatmentFertilizers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
			TempData["treatmentId"] = HttpContext.Session.GetInt32("treatmentId");
            if (id == null)
            {
                return NotFound();
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            if (treatmentFertilizer == null)
            {
                return NotFound();
            }
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer.OrderBy(t=> t.FertilizerName), "FertilizerName", "FertilizerName", treatmentFertilizer.FertilizerName);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "TreatmentId", treatmentFertilizer.TreatmentId);

            return View(treatmentFertilizer);
        }

        // POST: MHTreatmentFertilizers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TreatmentFertilizerId,TreatmentId,FertilizerName,RatePerAcre,RateMetric")] TreatmentFertilizer treatmentFertilizer)
        {
			TempData["treatmentId"] = HttpContext.Session.GetInt32("treatmentId");
            if (id != treatmentFertilizer.TreatmentFertilizerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatmentFertilizer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentFertilizerExists(treatmentFertilizer.TreatmentFertilizerId))
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
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer.OrderBy(t=> t.FertilizerName), "FertilizerName", "FertilizerName", treatmentFertilizer.FertilizerName);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "TreatmentId", treatmentFertilizer.TreatmentId);
            return View(treatmentFertilizer);
        }

        // GET: MHTreatmentFertilizers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer
                .Include(t => t.FertilizerNameNavigation)
                .Include(t => t.Treatment)
                .SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            if (treatmentFertilizer == null)
            {
                return NotFound();
            }

            return View(treatmentFertilizer);
        }

        // POST: MHTreatmentFertilizers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatmentFertilizer = await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            _context.TreatmentFertilizer.Remove(treatmentFertilizer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentFertilizerExists(int id)
        {
            return _context.TreatmentFertilizer.Any(e => e.TreatmentFertilizerId == id);
        }
    }
}
