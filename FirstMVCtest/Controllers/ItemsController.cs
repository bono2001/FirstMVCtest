using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            // Haal alle categorieën op en geef ze door aan de view
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }





        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item item, int[] SelectedCategories)
        {
            if (ModelState.IsValid)
            {
                // Voeg het item toe aan de database
                _context.Add(item);
                await _context.SaveChangesAsync();

                // Voeg de geselecteerde categorieën toe aan het item
                foreach (var categoryId in SelectedCategories)
                {
                    var category = await _context.Categories.FindAsync(categoryId);
                    if (category != null)
                    {
                        item.Categories.Add(category);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Als het formulier niet valide is, laad de categorieën opnieuw
            ViewBag.Categories = _context.Categories.ToList();
            return View(item);
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

            // Haal alle categorieën op en geef ze door aan de view
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.SelectedCategories = item.Categories.Select(c => c.Id).ToArray();  // Haal de geselecteerde categorieën op

            return View(item);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Item item, int[] SelectedCategories)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update het item
                    _context.Update(item);

                    // Haal het bestaande item op inclusief categorieën
                    var existingItem = await _context.Items
                                                     .Include(i => i.Categories)
                                                     .FirstOrDefaultAsync(i => i.Id == id);

                    if (existingItem != null)
                    {
                        // Verwijder alle bestaande categorieën
                        existingItem.Categories.Clear();

                        // Voeg de nieuw geselecteerde categorieën toe
                        foreach (var categoryId in SelectedCategories)
                        {
                            var category = await _context.Categories.FindAsync(categoryId);
                            if (category != null)
                            {
                                existingItem.Categories.Add(category);
                            }
                        }

                        // Sla de wijzigingen op
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
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

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.SelectedCategories = SelectedCategories;
            return View(item);
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



        // Voeg de AddCategoryToItem-methode toe
        public async Task<IActionResult> AddCategoryToItem(int itemId, int categoryId)
        {
            // Haal het item en de categorie uit de database
            var item = await _context.Items.Include(i => i.Categories).FirstOrDefaultAsync(i => i.Id == itemId);
            var category = await _context.Categories.FindAsync(categoryId);

            if (item == null || category == null)
            {
                return NotFound();
            }

            // Voeg de categorie toe aan het item (als deze nog niet is toegevoegd)
            if (!item.Categories.Contains(category))
            {
                item.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // Terug naar de lijst met items
        }

    }
}
