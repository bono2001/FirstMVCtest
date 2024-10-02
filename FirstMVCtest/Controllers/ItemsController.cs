using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirstMVCtest.Data;
using FirstMVCtest.Models;


namespace FirstMVCtest.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ItemDb _context;

        public ItemsController(ItemDb context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index(string sortOrder)
        {
            // Voeg de verschillende sorteermogelijkheden toe aan de ViewBag
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewBag.DateSortParm = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewBag.CategorySortParm = sortOrder == "category_asc" ? "category_desc" : "category_asc";

            // Haal de items op uit de database en include de categorieën
            var items = await _context.Items.Include(i => i.Categories).ToListAsync();

            // Pas sortering toe op basis van de geselecteerde waarde in de dropdown
            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(i => i.Name).ToList();
                    break;
                case "price_asc":
                    items = items.OrderBy(i => i.Price).ToList();
                    break;
                case "price_desc":
                    items = items.OrderByDescending(i => i.Price).ToList();
                    break;
                case "date_asc":
                    items = items.OrderBy(i => i.PurchaseDate).ToList();
                    break;
                case "date_desc":
                    items = items.OrderByDescending(i => i.PurchaseDate).ToList();
                    break;
                case "category_asc":
                    items = items.OrderBy(i => i.Categories.Select(c => c.Name).FirstOrDefault()).ToList(); // Sorteren op eerste categorie
                    break;
                case "category_desc":
                    items = items.OrderByDescending(i => i.Categories.Select(c => c.Name).FirstOrDefault()).ToList(); // Omgekeerde sortering
                    break;
                default:
                    items = items.OrderBy(i => i.Name).ToList(); // Standaard sorteren op naam (A-Z)
                    break;
            }

            return View(items);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Categories) // Zorg ervoor dat categorieën worden ingeladen
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }


        // GET: Items/Create
        public IActionResult Create()
        {
            var viewModel = new ItemEditViewModel
            {
                Item = new Item(),
                Categories = _context.Categories.ToList(),
                SelectedCategoryIds = new List<int>() // Lege lijst voor nieuwe items
            };

            return View(viewModel);
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(viewModel.Item);
                await _context.SaveChangesAsync();

                // Voeg de geselecteerde categorieën toe aan het item
                foreach (var categoryId in viewModel.SelectedCategoryIds)
                {
                    var category = await _context.Categories.FindAsync(categoryId);
                    if (category != null)
                    {
                        viewModel.Item.Categories.Add(category);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Als het formulier niet valide is, laad de categorieën opnieuw
            viewModel.Categories = _context.Categories.ToList();
            return View(viewModel);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                                     .Include(i => i.Categories)
                                     .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var viewModel = new ItemEditViewModel
            {
                Item = item,
                Categories = _context.Categories.ToList(),
                SelectedCategoryIds = item.Categories.Select(c => c.Id).ToList() // Haal de geselecteerde categorieën op
            };

            return View(viewModel);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ItemEditViewModel viewModel)
        {
            if (id != viewModel.Item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update het item in de database
                    _context.Update(viewModel.Item);
                    await _context.SaveChangesAsync();

                    // Haal het bestaande item op inclusief categorieën
                    var existingItem = await _context.Items
                                                     .Include(i => i.Categories)
                                                     .FirstOrDefaultAsync(i => i.Id == id);

                    if (existingItem != null)
                    {
                        // Verwijder de bestaande categorieën
                        existingItem.Categories.Clear();

                        // Voeg de nieuw geselecteerde categorieën toe
                        foreach (var categoryId in viewModel.SelectedCategoryIds)
                        {
                            var category = await _context.Categories.FindAsync(categoryId);
                            if (category != null)
                            {
                                existingItem.Categories.Add(category);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(viewModel.Item.Id))
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

            viewModel.Categories = _context.Categories.ToList();
            return View(viewModel);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}
