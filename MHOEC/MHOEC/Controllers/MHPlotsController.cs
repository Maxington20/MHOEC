using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MHOEC.Models;

namespace MHOEC.Controllers
{
    public class MHPlotsController : Controller
    {
        private readonly OECContext _context;

        public MHPlotsController(OECContext context)
        {
            _context = context;
        }

        // GET: MHPlots
        public async Task<IActionResult> Index()
        {
            var oECContext = _context.Plot.Include(p => p.Farm).Include(p => p.Variety);
            return View(await oECContext.ToListAsync());
        }

        // GET: MHPlots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot
                .Include(p => p.Farm)
                .Include(p => p.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }

            return View(plot);
        }

        // GET: MHPlots/Create
        public IActionResult Create()
        {
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "ProvinceCode");
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId");
            return View();
        }

        // POST: MHPlots/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlotId,FarmId,VarietyId,DatePlanted,DateHarvested,PlantingRate,PlantingRateByPounds,RowWidth,PatternRepeats,OrganicMatter,BicarbP,Potassium,Magnesium,Calcium,PHsoil,PHbuffer,Cec,PercentBaseSaturationK,PercentBaseSaturationMg,PercentBaseSaturationCa,PercentBaseSaturationH,Comments")] Plot plot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "ProvinceCode", plot.FarmId);
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId", plot.VarietyId);
            return View(plot);
        }

        // GET: MHPlots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot.SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "ProvinceCode", plot.FarmId);
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId", plot.VarietyId);
            return View(plot);
        }

        // POST: MHPlots/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlotId,FarmId,VarietyId,DatePlanted,DateHarvested,PlantingRate,PlantingRateByPounds,RowWidth,PatternRepeats,OrganicMatter,BicarbP,Potassium,Magnesium,Calcium,PHsoil,PHbuffer,Cec,PercentBaseSaturationK,PercentBaseSaturationMg,PercentBaseSaturationCa,PercentBaseSaturationH,Comments")] Plot plot)
        {
            if (id != plot.PlotId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlotExists(plot.PlotId))
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
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "ProvinceCode", plot.FarmId);
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId", plot.VarietyId);
            return View(plot);
        }

        // GET: MHPlots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot
                .Include(p => p.Farm)
                .Include(p => p.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }

            return View(plot);
        }

        // POST: MHPlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plot = await _context.Plot.SingleOrDefaultAsync(m => m.PlotId == id);
            _context.Plot.Remove(plot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlotExists(int id)
        {
            return _context.Plot.Any(e => e.PlotId == id);
        }
    }
}
