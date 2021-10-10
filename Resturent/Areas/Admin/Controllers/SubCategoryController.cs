using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurent.Models;
using Restaurent.Models.ViewModels;
using Restaurent.Utility;
using Resturent.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurent.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET INDEX
        public async Task<IActionResult> Index()
        {
            var subCategory = await _db.SubCategory.Include(s => s.Category).ToArrayAsync();
            return View(subCategory);
        }

        //GET - CREATE
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                subCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //POST - CREATE 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var IsSubCategoryExists = _db.SubCategory.Include(x => x.Category).Where(x => x.Name == model.subCategory.Name && x.Category.Id == model.subCategory.CategoryId);
                if (IsSubCategoryExists.Count() > 0)
                {
                    StatusMessage = "Error : Sub Category already exsits in "+IsSubCategoryExists.First().Category.Name+" category";
                }
                else
                {
                    _db.SubCategory.Add(model.subCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVm = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                subCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVm);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategory
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        //GET - EDIT
        public async Task<IActionResult> EDIT(int? id)
        {
            if (id == null)
                return NotFound();

            var subcategory = await _db.SubCategory.SingleOrDefaultAsync(x => x.Id == id);

            if (subcategory == null)
                return NotFound();

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                subCategory = subcategory,
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //POST - EDIT 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EDIT(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var IsSubCategoryExists = _db.SubCategory.Include(x => x.Category).Where(x => x.Name == model.subCategory.Name && x.Category.Id == model.subCategory.CategoryId);
                if (IsSubCategoryExists.Count() > 0)
                {
                    StatusMessage = "Error : Sub Category already exsits in " + IsSubCategoryExists.First().Category.Name + " category";
                }
                else
                {
                    var subcatgoryFromDb = await _db.SubCategory.FindAsync(model.subCategory.Id);
                    subcatgoryFromDb.Name = model.subCategory.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVm = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                subCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVm);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var subcategory = await _db.SubCategory.SingleOrDefaultAsync(x => x.Id == id);

            if (subcategory == null)
                return NotFound();

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                subCategory = subcategory,
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var subcategory = await _db.SubCategory.SingleOrDefaultAsync(x => x.Id == id);

            if (subcategory == null)
                return NotFound();

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                subCategory = subcategory,
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var subcategory = await _db.SubCategory.SingleOrDefaultAsync(x => x.Id == id);
            if (subcategory == null)
            {
                return View();
            }
            _db.SubCategory.Remove(subcategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}

