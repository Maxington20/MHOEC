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
    public class MHPlotsController : Controller
    {
        private readonly OECContext _context;

        public MHPlotsController(OECContext context)
        {
            _context = context;
        }

        // GET: MHPlots
        public async Task<IActionResult> Index(string sortOrder, string name, int? id, string pageName)
        {
			IQueryable<Plot> oECContext;


			//I really need to figure out how the sorting will work with all of the different parameters I
			//am currently using. I could do it if all of the other crap was not involved, but with all these
			//other things happening, I am not sure what to do
			if (sortOrder != null)
			{
				switch (sortOrder)
				{
					case "Farm":
						sortOrder = "p=> p.Farm.Name";
						break;

					case "Variety":
						sortOrder = "p=> p.Variety.Name";
						break;

					default:
						sortOrder = "p=> p.Cec";
						break;
				}
			}

			if (id != null)
			{
				if (pageName == "Crop")
				{
					oECContext = _context.Plot
						.Include(p => p.Farm)
						.Include(p => p.Variety)
						.Include(p => p.Variety.Crop)
						.Include(p => p.Treatment)
						.Where(p => p.Variety.CropId == id)
						.OrderByDescending(p => p.DatePlanted);
					ViewData["varietyName"] = name + " " + "Crop";
					HttpContext.Session.SetInt32("varietyID", Convert.ToInt32(null));
					HttpContext.Session.SetString("cropName", name);
				}

				else if (pageName == "Variety")
				{
					oECContext = _context.Plot
					   .Include(p => p.Farm)
					   .Include(p => p.Variety)
					   .Include(p => p.Variety.Crop)
					   .Include(p => p.Treatment)
					   .Where(p => p.Variety.VarietyId == id)
					   .OrderByDescending(p => p.DatePlanted);

					HttpContext.Session.SetInt32("varietyID", Convert.ToInt32(id));
					HttpContext.Session.SetString("varietyName", name);
					ViewData["varietyID"] = HttpContext.Session.GetInt32("varietyID");
					ViewData["varietyName"] = name + " " + "Variety";
					HttpContext.Session.SetString("varietyName", name);
				}

				else
				{
					HttpContext.Session.SetInt32("plotId", Convert.ToInt32(id));
					oECContext = _context.Plot
					   .Include(p => p.Farm)
					   .Include(p => p.Variety)
					   .Include(p => p.Variety.Crop)
					   .Include(p => p.Treatment)
					   .Where(p => p.PlotId == id)
					   .OrderByDescending(p => p.DatePlanted);

					//HttpContext.Session.SetInt32("varietyID", Convert.ToInt32(id));
					//HttpContext.Session.SetString("varietyName", name);
					//ViewData["varietyID"] = HttpContext.Session.GetInt32("varietyID");
					ViewData["varietyName"] = name + " " + "Variety";
					//HttpContext.Session.SetString("varietyName", name);
				}

				//else
				//{
				//	oECContext = _context.Plot
				//	   .Include(p => p.Farm)
				//	   .Include(p => p.Variety)
				//	   .Include(p => p.Variety.Crop)
				//	   .Include(p => p.Treatment).Where(p => p.PlotId == id)
				//	   .OrderByDescending(p => p.DatePlanted);

					
				//}
			}
			else
			{
				oECContext = _context.Plot.Include(p => p.Farm)
					.Include(p => p.Variety)
					.Include(p => p.Variety.Crop)
					.Include(p => p.Treatment)
					.OrderByDescending(p => p.DatePlanted);
			}

			ViewData["cropName"] = HttpContext.Session.GetString("cropName");
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
				.Include(p=> p.Variety.Crop)
				.Include(p=> p.Treatment)
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
			ViewData["sessionVarietyID"] = HttpContext.Session.GetInt32("varietyID");
			ViewData["FarmId"] = new SelectList(_context.Farm.OrderBy(f => f.Name), "FarmId", "Name");
			if (HttpContext.Session.GetInt32("varietyID") != null && HttpContext.Session.GetInt32("varietyID") != 0)
			{
				ViewData["VarietyId"] = new SelectList(_context.Variety.Where(v => v.VarietyId == HttpContext.Session.GetInt32("varietyID")).OrderBy(v => v.Name), "VarietyId", "Name");
			}
			else
			{
				ViewData["VarietyId"] = new SelectList(_context.Variety.OrderBy(v => v.Name), "VarietyId", "Name");
			}

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
            ViewData["FarmId"] = new SelectList(_context.Farm.OrderBy(x=>x.Name), "FarmId", "ProvinceCode", plot.FarmId);
            ViewData["VarietyId"] = new SelectList(_context.Variety.OrderBy(x=>x.Name), "VarietyId", "VarietyId", plot.VarietyId);
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
