// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
// using sklep.Models;
// using Microsoft.AspNetCore.Authorization;

// namespace sklep.Controllers
// {
//     public class CartController : Controller
//     {
//         private readonly ProductContext _context;

//         public CartController(ProductContext context)
//         {
//             _context = context;
//         }

//         // GET: Cart
//         public async Task<IActionResult> Index()
//         {
//             ViewData["user"] = User.IsInRole("Admin");
//             return View(await _context.Cart.ToListAsync());
//         }

//         // GET: Cart/Details/5
//         public async Task<IActionResult> Details(int? id)
//         {
//             if (id == null)
//             {
//                 return NotFound();
//             }

//             var cart = await _context.Cart
//                 .SingleOrDefaultAsync(m => m.Id == id);
//             if (cart == null)
//             {
//                 return NotFound();
//             }

//             return View(cart);
//         }

//         // GET: Cart/Create
//         public IActionResult Create()
//         {
//             return View();
//         }

//         // POST: Cart/Create
//         // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//         // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Create([Bind("Id,IdUser,Date")] Cart cart)
//         {
//             if (ModelState.IsValid)
//             {
//                 _context.Add(cart);
//                 await _context.SaveChangesAsync();
//                 return RedirectToAction(nameof(Index));
//             }
//             return View(cart);
//         }

//         // GET: Cart/Edit/5
//         public async Task<IActionResult> Edit(int? id)
//         {
//             if (id == null)
//             {
//                 return NotFound();
//             }

//             var cart = await _context.Cart.SingleOrDefaultAsync(m => m.Id == id);
//             if (cart == null)
//             {
//                 return NotFound();
//             }
//             return View(cart);
//         }

//         // POST: Cart/Edit/5
//         // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//         // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Edit(int id, [Bind("Id,IdUser,Date")] Cart cart)
//         {
//             if (id != cart.Id)
//             {
//                 return NotFound();
//             }

//             if (ModelState.IsValid)
//             {
//                 try
//                 {
//                     _context.Update(cart);
//                     await _context.SaveChangesAsync();
//                 }
//                 catch (DbUpdateConcurrencyException)
//                 {
//                     if (!CartExists(cart.Id))
//                     {
//                         return NotFound();
//                     }
//                     else
//                     {
//                         throw;
//                     }
//                 }
//                 return RedirectToAction(nameof(Index));
//             }
//             return View(cart);
//         }

//         // GET: Cart/Delete/5
//         public async Task<IActionResult> Delete(int? id)
//         {
//             if (id == null)
//             {
//                 return NotFound();
//             }

//             var cart = await _context.Cart
//                 .SingleOrDefaultAsync(m => m.Id == id);
//             if (cart == null)
//             {
//                 return NotFound();
//             }

//             return View(cart);
//         }

//         // POST: Cart/Delete/5
//         [HttpPost, ActionName("Delete")]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> DeleteConfirmed(int id)
//         {
//             var cart = await _context.Cart.SingleOrDefaultAsync(m => m.Id == id);
//             _context.Cart.Remove(cart);
//             await _context.SaveChangesAsync();
//             return RedirectToAction(nameof(Index));
//         }

//         private bool CartExists(int id)
//         {
//             return _context.Cart.Any(e => e.Id == id);
//         }
//     }
// }
