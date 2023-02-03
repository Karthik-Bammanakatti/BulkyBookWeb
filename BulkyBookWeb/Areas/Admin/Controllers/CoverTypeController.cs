using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<CoverType> coverTypes = _unitOfWork.CoverType.GetAll().ToList();
            return View(coverTypes);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType CoverType)
        {
            if (CoverType.Name == CoverType.Id.ToString())
            {
                ModelState.AddModelError("name", "Id and CoverType cannot be same.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(CoverType);
                _unitOfWork.Save();
                TempData["success"] = "CoverType created successfully.";
                return RedirectToAction("Index");
            }
            return View(CoverType);

        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            CoverType? CoverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);

            if (CoverType == null)
            {
                return NotFound();
            }

            return View(CoverType);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType CoverType)
        {
            if (CoverType.Name == CoverType.Id.ToString())
            {
                ModelState.AddModelError("name", "Id and CoverType name cannot be same.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Update(CoverType);
                _unitOfWork.Save();
                TempData["success"] = "CoverType updated successfully.";
                return RedirectToAction("Index");
            }
            return View(CoverType);

        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            CoverType? CoverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);

            if (CoverType == null)
            {
                return NotFound();
            }

            return View(CoverType);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCoverType(int? id)
        {
            CoverType? CoverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            if (CoverType == null)
            {
                return NotFound();
            }

            _unitOfWork.CoverType.Remove(CoverType);
            _unitOfWork.Save();
            TempData["success"] = "CoverType deleted successfully.";
            return RedirectToAction("Index");

        }
    }
}
