using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

       
        //GET
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            if (id == 0 || id == null)
            {
                return View(productVM);
            }
            else 
            {
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);
                return View(productVM);
            }

            
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM ProductVM, IFormFile? img)
        {
            Category category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == ProductVM.Product.CategoryId);
            CoverType coverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == ProductVM.Product.CoverTypeId);
            ProductVM.Product.Category = category;
            ProductVM.Product.CoverType = coverType;
            if (ModelState.IsValid)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (img != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extension = Path.GetExtension(img.FileName);
                    if (ProductVM.Product.ImageUrl != null)
                    {
                        var oldIamgePath = Path.Combine(webRootPath, ProductVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldIamgePath))
                        {
                            System.IO.File.Delete(oldIamgePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        img.CopyTo(fileStream);
                    }

                    ProductVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }

                if (ProductVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(ProductVM.Product);
                    TempData["success"] = "Product created successfully.";
                }
                else 
                {
                    _unitOfWork.Product.Update(ProductVM.Product);
                    TempData["success"] = "Product updated successfully.";
                }
                _unitOfWork.Save();
                
                return RedirectToAction("Index");
            }
            return View(ProductVM);

        }

     
        #region API calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties : "Category,CoverType");
            return Json(new { data = products });
        }

        //POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product? Product = _unitOfWork.Product.GetFirstOrDefault(c => c.Id == id);
            if (Product == null)
            {
                return Json(new { success = false, message = "Error while deleteing"});
            }

            var oldIamgePath = Path.Combine(_webHostEnvironment.WebRootPath, Product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldIamgePath))
            {
                System.IO.File.Delete(oldIamgePath);
            }
            _unitOfWork.Product.Remove(Product);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Product deleted successfully."});

        }
        #endregion


    }
}
