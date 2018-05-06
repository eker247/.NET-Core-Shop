using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sklep.Models;

namespace sklep.Controllers
{
    public class InCartController : Controller
    {
        private readonly ProductContext _context;

        public InCartController(ProductContext context)
        {
            _context = context;
        }

        // GET: InCart
        public async Task<IActionResult> Index()
        {
            return View(await _context.InCart.ToListAsync());
        }

        // GET: InCart/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inCart = await _context.InCart
                .SingleOrDefaultAsync(m => m.Id == id);
            if (inCart == null)
            {
                return NotFound();
            }

            return View(inCart);
        }

        // GET: InCart/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InCart/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdCart,IdProduct,Number")] InCart inCart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inCart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inCart);
        }

        // GET: InCart/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inCart = await _context.InCart.SingleOrDefaultAsync(m => m.Id == id);
            if (inCart == null)
            {
                return NotFound();
            }
            return View(inCart);
        }

        // POST: InCart/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdCart,IdProduct,Number")] InCart inCart)
        {
            if (id != inCart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InCartExists(inCart.Id))
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
            return View(inCart);
        }

        // GET: InCart/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inCart = await _context.InCart
                .SingleOrDefaultAsync(m => m.Id == id);
            if (inCart == null)
            {
                return NotFound();
            }

            return View(inCart);
        }

        // POST: InCart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inCart = await _context.InCart.SingleOrDefaultAsync(m => m.Id == id);
            _context.InCart.Remove(inCart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InCartExists(int id)
        {
            return _context.InCart.Any(e => e.Id == id);
        }
    }
}
