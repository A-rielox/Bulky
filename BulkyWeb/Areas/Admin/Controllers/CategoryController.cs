using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }




    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    public IActionResult Index()
    {
        List<Category> categories = _unitOfWork.Category.GetAll().ToList();

        return View(categories);
    }


    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category obj)
    {
        // p' una validacion custom ( en este caso p'q no sean iguales )
        if (obj.Name == obj.DisplayOrder.ToString())
        {
            // la key ( el 1er valor es p' q se agrege a la validacion de ese field en el modelo
            // en este caso va a aparecer como error en el Name 
            ModelState.AddModelError("name", "The display order cannot be equal to the name");
        }
        if (obj.Name != null && obj.Name.ToLower() == "test")
        {
            // en este caso (con la key vacia) se va a agregar a la validacion del modelo
            // y va a aparecer en la validacion gral <div asp-validation-summary="All"></div> 
            ModelState.AddModelError("", "el nombre no puede ser test");
        }

        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Add(obj);
            _unitOfWork.Save();

            TempData["success"] = "Category created successfully.";
            // accedo a esta con el volor de la key "success"

            return RedirectToAction("Index");

        }

        return View(); // si no es valido el modelo me deja en el mismo lado


        // si quiera ir a otro controller seria:
        // RedirectToAction("nombre_accion","nombre_controller");
    }


    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0) return NotFound(); // aca podria enviar a una error page p.ej.

        var category = _unitOfWork.Category.Get(c => c.Id == id);

        if (category == null) return NotFound();

        return View(category);
    }

    [HttpPost]
    public IActionResult Edit(Category obj)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Update(obj);
            _unitOfWork.Save();

            TempData["success"] = "Category edited successfully.";

            return RedirectToAction("Index");
        }

        return View();
    }


    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0) return NotFound(); // aca podria enviar a una error page p.ej.

        var category = _unitOfWork.Category.Get(c => c.Id == id);

        if (category == null) return NotFound();

        return View(category);
    }

    // podria pasar todo el obj y no tendria que cambiar el nombre, ya q tendria distinta signature
    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? id)
    {
        if (id == null || id == 0) return NotFound();

        var category = _unitOfWork.Category.Get(c => c.Id == id);

        if (category == null) return NotFound();

        _unitOfWork.Category.Remove(category);
        _unitOfWork.Save();

        TempData["success"] = "Category deleted successfully.";

        return RedirectToAction("Index");
    }
}
