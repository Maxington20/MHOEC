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
    public class MHVarietiesController : Controller
    {
        private readonly OECContext _context;

        public MHVarietiesController(OECContext context)
        {
            _context = context;
        }

        // GET: MHVarieties
        public async Task<IActionResult> Index(int? id, string name)
        {
			if (id != null)
			{
				HttpContext.Session.SetInt32("cropId", Convert.ToInt32(id));
			}

			else
			{
				if (HttpContext.Session.GetInt32("cropId") != null)
				{
					id = HttpContext.Session.GetInt32("cropId");
				}

				else
				{
					TempData["Message"] = "Please select a Crop to see its Varieties";
					return RedirectToAction("Index", "MHCrop");
				}
			}

			if (name != null)
			{
				HttpContext.Session.SetString("cropName", name);
			}

			else
			{
				if (HttpContext.Session.GetString("cropName") != null)
				{
					name = HttpContext.Session.GetString("cropName");
				}

				else
				{
					name = Convert.ToString(_context.Variety.Include(v => v.Crop)
						.Where(x => x.CropId == id)
						.Select(x=> x.Crop.Name)
						.FirstOrDefault());
					HttpContext.Session.SetString("cropName", name);
				}
			}

			ViewData["cropId"] = HttpContext.Session.GetInt32("cropId");
			ViewData["cropName"] = HttpContext.Session.GetString("cropName");

			var oECContext = _context.Variety.Include(v => v.Crop)
				.Where(x => x.CropId == id)
				.OrderBy(x => x.Name);
            return View(await oECContext.ToListAsync());
        }

        // GET: MHVarieties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variety = await _context.Variety
                .Include(v => v.Crop)
                .SingleOrDefaultAsync(m => m.VarietyId == id);
            if (variety == null)
            {
                return NotFound();
            }

			ViewData["cropName"] = HttpContext.Session.GetString("cropName");
            return View(variety);
        }

        // GET: MHVarieties/Create
        public IActionResult Create(int? cropId)
        {
			TempData["cropId"] = cropId;

			ViewData["cropName"] = HttpContext.Session.GetString("cropName");
            //ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId");
            return View();
        }

        // POST: MHVarieties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? cropId,[Bind("VarietyId,CropId,Name")] Variety variety)
        {
			TempData["cropId"] = cropId;

            if (ModelState.IsValid)
            {
                _context.Add(variety);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
            return View(variety);
        }

        // GET: MHVarieties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variety = await _context.Variety.SingleOrDefaultAsync(m => m.VarietyId == id);
            if (variety == null)
            {
                return NotFound();
            }
			//ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
			ViewData["cropId"] = variety.CropId;
			ViewData["cropName"] = HttpContext.Session.GetString("cropName");
			return View(variety);
        }

        // POST: MHVarieties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VarietyId,CropId,Name")] Variety variety)
        {
            if (id != variety.VarietyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(variety);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VarietyExists(variety.VarietyId))
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
            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
			ViewData["cropName"] = HttpContext.Session.GetString("cropName");
            return View(variety);
        }

        // GET: MHVarieties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variety = await _context.Variety
                .Include(v => v.Crop)
                .SingleOrDefaultAsync(m => m.VarietyId == id);
            if (variety == null)
            {
                return NotFound();
            }
			ViewData["cropName"] = HttpContext.Session.GetString("cropName");
			return View(variety);
        }

        // POST: MHVarieties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var variety = await _context.Variety.SingleOrDefaultAsync(m => m.VarietyId == id);
            _context.Variety.Remove(variety);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VarietyExists(int id)
        {
            return _context.Variety.Any(e => e.VarietyId == id);
        }
    }
}
