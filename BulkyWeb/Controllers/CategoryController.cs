using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers;

public class CategoryController : Controller
{
	private readonly ApplicationDbContext _db;

	public CategoryController(ApplicationDbContext db)
	{
		_db = db;
	}




	////////////////////////////////////////////////
	////////////////////////////////////////////////
	public IActionResult Index()
	{
		List<Category> categories = _db.Categories.ToList();

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
			_db.Categories.Add(obj);
			_db.SaveChanges();

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

		var category = _db.Categories.Find(id);

		if (category == null) return NotFound();

		return View(category);
	}

	[HttpPost]
	public IActionResult Edit(Category obj)
	{
		if (ModelState.IsValid)
		{
			_db.Categories.Add(obj);
			_db.SaveChanges();

			return RedirectToAction("Index");

		}

		return View();
	}









}
