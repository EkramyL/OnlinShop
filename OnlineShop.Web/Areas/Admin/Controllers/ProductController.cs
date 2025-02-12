using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Core.Models;
using OnlineShop.DataAccess.Data;

namespace OnlineShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Product
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        public IActionResult DeleteGallery(int id)
        {
            var productGallery = _context.ProductGalleries.FirstOrDefault(x => x.Id == id);
            if (productGallery == null)
            {
                return NotFound();
            }
            var uploaderFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "galleries");
            var imagePath = Path.Combine(uploaderFolder, productGallery.ImageName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
                
            }
            _context.ProductGalleries.Remove(productGallery);
            _context.SaveChanges();
            return Redirect($"edit/{productGallery.ProductId}");
        }
            
        
        // GET: Admin/Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,FullDesc,Price,Discount,ImageName,Qty,Tags,VideoUrl")] Product product, IFormFile? mainImage, IFormFile[]? galleryImages)
        {
            if (ModelState.IsValid)
            {
                
                // ****** Saving main Image ***** //
                if (mainImage != null)
                {
                    product.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(mainImage.FileName);
                    var uploaderFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                    var imagePath = Path.Combine(uploaderFolder, product.ImageName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await mainImage.CopyToAsync(stream);
                    }
                }
                _context.Products.Add(product);
               await _context.SaveChangesAsync();
                // =================================== //
                
                // **************** Saving Gallery Images ******* //
                
                if (galleryImages != null)
                {
                    foreach (var item in galleryImages)
                    {
                        var newGallery = new ProductGallery();
                        newGallery.ProductId = product.Id;
                        
                        // ------------- save each image ------------ //
                        newGallery.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        var uploaderFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "galleries");
                        var imagePath = Path.Combine(uploaderFolder, newGallery.ImageName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await item.CopyToAsync(stream);
                        }
                        _context.ProductGalleries.Add(newGallery);
                    }
                }
                
                // ====================================== //
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["gallery"]= _context.ProductGalleries.Where(x => x.ProductId == product.Id).ToList();
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,FullDesc,Price,Discount,ImageName,Qty,Tags,VideoUrl")] Product product, IFormFile? mainImage, IFormFile[]? galleryImages)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                
                try
                {
                    // ****** Saving main Image ***** //
                    if (mainImage != null)
                    {
                        product.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(mainImage.FileName);
                        var uploaderFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                        var imagePath = Path.Combine(uploaderFolder, product.ImageName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await mainImage.CopyToAsync(stream);
                        }
                   
                    }
                
                
                    // **************** Saving Gallery Images ******* //
                
                    if (galleryImages != null)
                    {
                        foreach (var item in galleryImages)
                        {
                            var newGallery = new ProductGallery();
                            newGallery.ProductId = product.Id;
                        
                            // ------------- save each image ------------ //
                            newGallery.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                            var uploaderFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "galleries");
                            var imagePath = Path.Combine(uploaderFolder, newGallery.ImageName);
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await item.CopyToAsync(stream);
                            }
                            _context.ProductGalleries.Add(newGallery);
                      
                        }
                       
                    }
                
                    // ====================================== //
                    _context.Update(product);
                    await _context.SaveChangesAsync();
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

        // GET: Admin/Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                var uploaderFolderMain = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                var imagePathMain = Path.Combine(uploaderFolderMain, product.ImageName);
                if (System.IO.File.Exists(imagePathMain))
                {
                    System.IO.File.Delete(imagePathMain);
                
                }
                
                var galleries = _context.ProductGalleries.Where(x => x.ProductId == id).ToList();
                if (galleries != null)
                {
                    foreach (var item in galleries)
                    {
                        var uploaderFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "galleries");
                        var imagePath = Path.Combine(uploaderFolder, item.ImageName);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                
                        }
                        _context.ProductGalleries.Remove(item);
                        
                    }  
                }
                 
                
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
