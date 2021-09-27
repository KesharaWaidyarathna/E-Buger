using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Restaurent.Models.ViewModels;
using Restaurent.Utility;
using Resturent.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurent.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _webHostEnvironment = hostEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _db.Category,
                MenuItem = new Models.MenuItem()
            };
        }

        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItems.Include(x=>x.Category).Include(x=>x.SubCategory).ToListAsync();
            return View(menuItems);
        }

        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        //POST - CREATE 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST() {

            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (!ModelState.IsValid)
                return View(MenuItemVM);

            _db.MenuItems.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            string webRootpath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItems.FindAsync(MenuItemVM.MenuItem.Id);
            if (files.Count() > 0)
            {
                var uploads = Path.Combine(webRootpath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileSteam = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileSteam);
                }

                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webRootpath, @"images\"+SD.DefualtFoodImage);
                System.IO.File.Copy(uploads, webRootpath + @"\images\" + MenuItemVM.MenuItem.Id + ".jpg");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".jpg";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
