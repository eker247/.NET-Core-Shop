using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sklep.Models;
using Microsoft.AspNetCore.Authorization;


namespace sklep.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductContext _context;
        
        public static List<InCart> ProductsInCart = new List<InCart>();

        public IActionResult AddToCart(int idproduct)
        {
            // temporary InCart having information about buying product
            InCart MyProduct = new InCart();

            MyProduct.IdProduct = idproduct;
            MyProduct.Number = 1;

            // is there the product in the cart
            InCart tmp = ProductsInCart.Find(p => p.IdProduct == idproduct);
            // if not add product with number = 1
            if (tmp == null){
                ProductsInCart.Add(MyProduct);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddToCart(List<InCart> Model, string make)
        {
            if (make == "Opróżnij koszyk") {
                ClearProductsInCart();
            }
            else {
                ProductsInCart = Model;
            }
            if (make == "Kup produkty") {
                return RedirectToAction("BuyProducts");
            }
            else {
                return RedirectToAction("Index");
            }
        }

        public IActionResult Index()
        {
            ViewData["Products"] = _context.Product.ToList();
            ViewData["errors"] = TempData["errors"] as Dictionary<string, int>;

            return View(ProductsInCart);
        }

        // remove all product from 
        static public void ClearProductsInCart()
        {
            ProductsInCart.Clear();
        }

        [Authorize]
        public async Task<IActionResult> BuyProducts()
        {
            var errors = new Dictionary<string, int>();
            foreach (var p in ProductsInCart) {
                Product product = _context.Product.FirstOrDefault(pr => pr.Id == p.IdProduct);
                // return Json(product);
                if (product.Number < p.Number) {
                    // when user try to buy more then products in a store
                    errors.Add(product.Name, product.Number);
                }
            }
            if (errors.Count > 0) {
                TempData["errors"] = errors;
                return RedirectToAction("Index");
            }

            string UserEmail = User.Identity.Name;  // logged user's email
            DateTime date = DateTime.Now;           // actual date
            int IdCart = 1;                         // id from database
            try {
                IdCart = _context.Cart.Max(c => c.Id) + 1;
            }
            catch (System.InvalidOperationException) {
                // kiedy jest pusta baza danych IdCart może być równe 1
            }

            Cart cart = new Cart(IdCart, UserEmail, date);
            _context.Add(cart);

            int IdInCart = 1;                       // id from database
            try {
                IdInCart = _context.InCart.Max(i => i.Id) + 1;
            }
            catch (System.InvalidOperationException) {
                // kiedy jest pusta baza danych IdInCart może być równe 1
            }

            foreach (var p in ProductsInCart) {
                p.IdCart = cart.Id;                 // cart.Id
                p.Id = IdInCart++;                  // IdInCart++
                _context.Add(p);
                Product product = _context.Product.FirstOrDefault(
                    pr => pr.Id == p.IdProduct);
                product.Number -= p.Number;
                _context.Update(product);
            }

            await _context.SaveChangesAsync();
            ClearProductsInCart();
            
            return RedirectToAction("Index", "Product");
        }

        public CartController(ProductContext context)
        {
            _context = context;
        }

        // GET: Cart
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AllCarts()
        {
            ViewData["user"] = User.IsInRole("Admin");
            return View(await _context.Cart.ToListAsync());
        }

        // GET: Cart/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            var pr = _context.InCart.Where(p => p.IdCart == id).ToList();
            var products = new Dictionary<string, int>();
            foreach (var p in pr) {
                products.Add(
                    _context.Product.FirstOrDefault(
                        prod => prod.Id == p.IdProduct).Name,
                    p.Number);
            }
            ViewData["products"] = products;
            return View(cart);
        }

        // GET: Cart/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdUser,Date")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Cart/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.SingleOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }
            return View(cart);
        }

        // POST: Cart/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdUser,Date")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            return View(cart);
        }

        // GET: Cart/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Cart.SingleOrDefaultAsync(m => m.Id == id);
            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.Id == id);
        }
    }
}
