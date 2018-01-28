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
    public class MHFarmsController : Controller
    {
        private readonly OECContext _context;

        public MHFarmsController(OECContext context)
        {
            _context = context;
        }

        // GET: MHFarms
        public async Task<IActionResult> Index()
        {
            var oECContext = _context.Farm
				.Include(f => f.ProvinceCodeNavigation)
				.OrderBy(f=> f.Name);
            return View(await oECContext.ToListAsync());
        }

        // GET: MHFarms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farm = await _context.Farm
                .Include(f => f.ProvinceCodeNavigation)
                .SingleOrDefaultAsync(m => m.FarmId == id);
            if (farm == null)
            {
                return NotFound();
            }

            return View(farm);
        }

        // GET: MHFarms/Create
        public IActionResult Create()
        {
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode");
            return View();
        }

        // POST: MHFarms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FarmId,Name,Address,Town,County,ProvinceCode,PostalCode,HomePhone,CellPhone,Email,Directions,DateJoined,LastContactDate")] Farm farm)
        {
			try
			{
				if (ModelState.IsValid)
				{
					_context.Add(farm);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
				ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", farm.ProvinceCode);
				TempData["Message"] = "Farm has been added";
				return View(farm);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty,ex.InnerException.Message);
				return View(farm);
			}
        }

        // GET: MHFarms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farm = await _context.Farm.SingleOrDefaultAsync(m => m.FarmId == id);
            if (farm == null)
            {
                return NotFound();
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", farm.ProvinceCode);
			ViewData["ProvinceName"] = new SelectList(_context.Province, "Name", "Name", farm.ProvinceCode);

			return View(farm);
        }

        // POST: MHFarms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FarmId,Name,Address,Town,County,ProvinceCode,PostalCode,HomePhone,CellPhone,Email,Directions,DateJoined,LastContactDate")] Farm farm)
        {
            if (id != farm.FarmId)
            {
                return NotFound();
            }

			try
			{

				if (ModelState.IsValid)
				{
					try
					{
						_context.Update(farm);
						await _context.SaveChangesAsync();
					}
					catch (DbUpdateConcurrencyException)
					{
						if (!FarmExists(farm.FarmId))
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
				ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", farm.ProvinceCode);
				ViewData["ProvinceName"] = new SelectList(_context.Province, "Name", "Name", farm.ProvinceCode);

				TempData["Message"] = "Farm has been edited";

				return View(farm);
			}

			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.InnerException.Message);
				return View(farm);
			}

        }

        // GET: MHFarms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farm = await _context.Farm
                .Include(f => f.ProvinceCodeNavigation)
                .SingleOrDefaultAsync(m => m.FarmId == id);
            if (farm == null)
            {
                return NotFound();
            }

            return View(farm);
        }

        // POST: MHFarms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
			try
			{
				var farm = await _context.Farm.SingleOrDefaultAsync(m => m.FarmId == id);
				_context.Farm.Remove(farm);
				await _context.SaveChangesAsync();
				TempData["Message"] = "Farm has been deleted";
				return RedirectToAction(nameof(Index));
			}

			catch (Exception ex)
			{
				TempData["Message"] = ex.InnerException.Message;
				return RedirectToAction("Delete", "MHFarms");
			}
        }

        private bool FarmExists(int id)
        {
            return _context.Farm.Any(e => e.FarmId == id);
        }
    }
}
