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
    public class MHProvincesController : Controller
    {
        private readonly OECContext _context;

        public MHProvincesController(OECContext context)
        {
            _context = context;
        }

        // GET: MHProvinces
        public async Task<IActionResult> Index()
        {
            var oECContext = _context.Province.Include(p => p.CountryCodeNavigation);
            return View(await oECContext.ToListAsync());
        }

        // GET: MHProvinces/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province
                .Include(p => p.CountryCodeNavigation)
                .SingleOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                return NotFound();
            }

            return View(province);
        }

        // GET: MHProvinces/Create
        public IActionResult Create()
        {
			ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode");
			ViewData["CountryName"] = new SelectList(_context.Country.OrderBy(c=> c.Name), "Name", "Name");
            return View();
        }

        // POST: MHProvinces/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProvinceCode,Name,CountryCode")] Province province)
        {
            if (ModelState.IsValid)
            {
                _context.Add(province);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
			ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
			ViewData["CountryName"] = new SelectList(_context.Country.OrderBy(c => c.Name),"Name","Name");

			return View(province);
        }

        // GET: MHProvinces/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province.SingleOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                return NotFound();
            }
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
			ViewData["CountryName"] = new SelectList(_context.Country.OrderBy(c=> c.Name), "Name", "Name", province.CountryCode);

			return View(province);
        }

        // POST: MHProvinces/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProvinceCode,Name,CountryCode")] Province province)
        {
            if (id != province.ProvinceCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(province);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProvinceExists(province.ProvinceCode))
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
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
			ViewData["CountryName"] = new SelectList(_context.Country, "Name", "Name", province.CountryCode);

			return View(province);
        }

        // GET: MHProvinces/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province
                .Include(p => p.CountryCodeNavigation)
                .SingleOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                return NotFound();
            }

            return View(province);
        }

        // POST: MHProvinces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var province = await _context.Province.SingleOrDefaultAsync(m => m.ProvinceCode == id);
            _context.Province.Remove(province);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProvinceExists(string id)
        {
            return _context.Province.Any(e => e.ProvinceCode == id);
        }
    }
}
