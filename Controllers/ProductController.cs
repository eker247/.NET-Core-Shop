// ProductController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sklep.Models;
using Microsoft.AspNetCore.Http;        // Microsoft.AspNetCore.Http.Internal.FormFile
using System.IO;                        // Path
using Microsoft.AspNetCore.Authorization;

namespace sklep.Controllers
{
    public class ProductController : Controller
    {
        // ścieżka do plików
        private string filePath = System.IO.Directory.GetCurrentDirectory() + 
                                  "/wwwroot/images/product/";
        private readonly ProductContext _context;

        static private Product OldProduct;

        public ProductController(ProductContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            ViewData["path"] = this.filePath;
            return View(await _context.Product.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Name,Img,Category,Price,Number")] Product product,
            IFormFile Img // jeśli mamy wiele plików
            )
        {
            if (ModelState.IsValid)
            {
                string path = this.InsertImage(Img);
                product.Img = Img != null ? path.Split("product/").ElementAt(1) : "";
                _context.Add(product);
                await _context.SaveChangesAsync();

                if (path.Length > 0) {
                    using (var stream = new FileStream(path, FileMode.Create)) {
                        await Img.CopyToAsync(stream);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Edit/5
        [Authorize(Roles = "admin")]        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.SingleOrDefaultAsync(m => m.Id == id);
            OldProduct = product;
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

  
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            bool deleteImg, [Bind("Id,Name,Category,Price,Number")] Product product,
            IFormFile Img)
        {
            // sprawdza spojność danych
            if (id != product.Id)
            {
                return NotFound();
            }

            // jeśli dane są poprawne
            if (ModelState.IsValid)
            {
                // ścieżka do zapisu lub "" jeśli zdjęcie niepoprawne
                string path = this.InsertImage(Img);
                try
                {
                    // jeśli ścieżka jest poprawna i nie jest zaznaczone usuwanie zdjęcia
                    // to product.Img = nazwa pliku a jeśli nie to ""
                    product.Img = (path.Length > 0 && deleteImg == false) ? path.Split("product/").ElementAt(1) : "";
                    Console.WriteLine(DateTime.Now.ToString());
                    // jeśli nie wybrano zdjęcia i nie ma polecenie usunięcia zdjęcia
                    if (product.Img.Length == 0 && deleteImg == false) {
                        // użycie języka TSQL (komunikacja Entity Framework Core z bazą danych)
                        await _context.Database.ExecuteSqlCommandAsync(
                            $"UPDATE Product SET Name = {product.Name}, Category = {product.Category}, Price = {product.Price}, Number = {product.Number} WHERE Id = {id}");
                    }
                    else {
                        // domyślna komunikacja z bazą danych
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                        // zapisanie pliku na dysku serwera
                        DeleteImage(OldProduct.Img);
                        if (product.Img.Length > 0) {
                            using (var stream = new FileStream(path, FileMode.Create)) {
                                await Img.CopyToAsync(stream);
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Product/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .SingleOrDefaultAsync(m => m.Id == id);
            OldProduct = product;
            
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5

        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.SingleOrDefaultAsync(m => m.Id == id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            DeleteImage(OldProduct.Img);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "admin")]
        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        [Authorize(Roles = "admin")]
        private string InsertImage(IFormFile Img) 
        {
            if (Img == null) {
                return "";
            }
            // W razie wystąpienia błędów zwraca null
            // jeśli wszystko ok zwraca ścieżkę do zapisu
            // dopuszczalne typy plików graficznych
            string[] requiredTypes = {"png", "jpg", "jpeg", "gif", "bmp"};
            // rozszerzenie załączonego pliku
            string f_type = Img.ContentType.Split("/").ElementAt(1);
            long f_size = Img.Length;
            // ścieżka to katalog plus nazwa pliku
            string path = this.filePath + DateTime.Now.ToString("MM-dd-yyyy_hh:mm:ss_") + Img.FileName;
            if ( 0 >= f_size || 3000000 < f_size ) {
                // nieprawidłowy rozmiar pliku
                ViewData["err1"] = "Rozmiar zdjęcia powinien być mniejszy niż 3MB";
                path = "";
            }
            if (!requiredTypes.Contains(f_type)) {
                // plik ma nieprawidłowe rozszerzenie
                ViewData["err2"] = "Zdjęcie powinno mieć rozszerzenie: " + 
                    ".png, .jpg, .jpeg, .gif lub .bmp";
                path = "";
            }
            return path;
        }

        // usuwanie pliku o podanej nazwie
        [Authorize(Roles = "admin")]        
        private void DeleteImage(string FileName) {
            // pobranie nazw wszystkich plików z folderu wwwroot/images/product/
            string[] txtList = Directory.GetFiles(filePath, "*");
            // dla każdego pliku
            foreach (var f in txtList) {
                // file to nazwa pliku
                string file = f.Split("product/").ElementAt(1);
                // jeśli nazwa jest taka sama jak podana do usunięcia
                if(file.Equals(FileName)) {
                    // usuwanie pliku
                    System.IO.File.Delete(f);
                }
            }
        }
    }
}
