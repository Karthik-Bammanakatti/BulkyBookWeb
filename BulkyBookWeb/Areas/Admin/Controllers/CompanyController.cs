using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

       
        //GET
        public IActionResult Upsert(int? id)
        {
            Company company = new();

            if (id == 0 || id == null)
            {
                return View(company);
            }
            else 
            {
                company = _unitOfWork.Company.GetFirstOrDefault(p => p.Id == id);
                return View(company);
            }

            
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                    TempData["success"] = "Company created successfully.";
                }
                else 
                {
                    _unitOfWork.Company.Update(company);
                    TempData["success"] = "Company updated successfully.";
                }
                _unitOfWork.Save();
                
                return RedirectToAction("Index");
            }
            return View(company);

        }

     
        #region API calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var Companies = _unitOfWork.Company.GetAll();
            return Json(new { data = Companies });
        }

        //POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company? Company = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == id);
            if (Company == null)
            {
                return Json(new { success = false, message = "Error while deleteing"});
            }

            _unitOfWork.Company.Remove(Company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Company deleted successfully."});

        }
        #endregion


    }
}
