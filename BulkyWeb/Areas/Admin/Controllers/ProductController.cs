using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
	private readonly IUnitOfWork _unitOfWork;

	// p' acceder al root, simplemente lo pongo aca y se inyecta solo NO lo
	// tengo q dar de alta en program.cs ya viene xdefault
	private readonly IWebHostEnvironment _webHostEnvironment;

	public ProductController(IUnitOfWork unitOfWork,
							 IWebHostEnvironment webHostEnvironment)
	{
		_unitOfWork = unitOfWork;
		_webHostEnvironment = webHostEnvironment;
	}




	////////////////////////////////////////////////
	////////////////////////////////////////////////
	public IActionResult Index()
	{
		List<Product> products = _unitOfWork.Product.GetAll().ToList();

		return View(products);
	}

	////////////////////////////////////////////////
	////////////////////////////////////////////////
	public IActionResult Upsert(int? id)
	{
		IEnumerable<SelectListItem> categoryList = _unitOfWork.Category
								.GetAll().Select(c => new SelectListItem
								{
									Text = c.Name,
									Value = c.Id.ToString(),
								});

		// como en la view ya tengo "@model Product" no puedo llegar y pasar el categoryList
		// a la view, xeso ocupo un ViewBag
		// la key p'acceder a este valor va a ser "CategoryList"
		// ViewBag.CategoryList = categoryList;

		// ViewData["CategoryList"] = categoryList;

		// p' cuando hay que pasar muchos object distintos mejor crear un ViewModel
		ProductVM productVM = new()
		{
			CategoryList = categoryList,
			Product = new Product()
		};

		if (id == null || id == 0)
		{
			// CREATE
			return View(productVM);
		}
		else
		{
			// UPDATE
			productVM.Product = _unitOfWork.Product.Get(p => p.Id == id);

			return View(productVM);
		}
	}

	[HttpPost]
	public IActionResult Upsert(ProductVM productVM, IFormFile? file)
	{
		// p'q no me salte el error de validacion ( xq al intentar crear la product aca
		// viene todo el ProductVM y no va a traer el IEnumrable del select, ni todo el
		// Category, solo el CategoryId ) tengo q poner la anotacion [ValidateNever]
		// en las q no quiero q valide ( en la entity )
		if (ModelState.IsValid)
		{
			// WebRootPath me manda al wwwRoot folder
			string wwwRootPath = _webHostEnvironment.WebRootPath;

			if (file != null)
			{
				// p'darle nombre random
				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
				string productPath = Path.Combine(wwwRootPath, @"images\product");

				// si se esta actualizando => ya hay una imagen => la borro y ya subo la nueva
				if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
				{
					// borrar la anterior
					var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
				{
					file.CopyTo(fileStream);
				}

				productVM.Product.ImageUrl = @"\images\product\" + fileName;
			}

			if (productVM.Product.Id == 0)
			{
				_unitOfWork.Product.Add(productVM.Product);
			}
			else
			{
				_unitOfWork.Product.Update(productVM.Product);
			}

			_unitOfWork.Save();

			TempData["success"] = "Product created successfully.";

			return RedirectToAction("Index");
		}

		// si no pongo los [ValidateNever] p'q no salga el error xno tener el IEnumrable,
		// es basicamente hacer el populate del dropdown
		//else {
		//	IEnumerable<SelectListItem> categoryList = _unitOfWork.Category
		//						.GetAll().Select(c => new SelectListItem
		//						{
		//							Text = c.Name,
		//							Value = c.Id.ToString(),
		//						});

		//	productVM.CategoryList = categoryList;

		//	return View(productVM);
		//}

		return View();
		// si no es valido el modelo me deja en el mismo lado
		// si quiera ir a otro controller seria:
		// RedirectToAction("nombre_accion","nombre_controller");
	}

	////////////////////////////////////////////////
	////////////////////////////////////////////////	AHORA CON UPSERT
	//public IActionResult Edit(int? id)
	//{
	//	if (id == null || id == 0) return NotFound();

	//	var product = _unitOfWork.Product.Get(p => p.Id == id);

	//	if (product == null) return NotFound();

	//	return View(product);
	//}

	//[HttpPost]
	//public IActionResult Edit(Product obj)
	//{
	//	if (ModelState.IsValid)
	//	{
	//		_unitOfWork.Product.Update(obj);
	//		_unitOfWork.Save();

	//		TempData["success"] = "Product edited successfully.";

	//		return RedirectToAction("Index");
	//	}

	//	return View();
	//}


	////////////////////////////////////////////////
	////////////////////////////////////////////////
	public IActionResult Delete(int? id)
	{
		if (id == null || id == 0) return NotFound();

		var product = _unitOfWork.Product.Get(p => p.Id == id);

		if (product == null) return NotFound();

		return View(product);
	}

	[HttpPost, ActionName("Delete")]
	public IActionResult DeletePOST(int? id)
	{
		if (id == null || id == 0) return NotFound();

		var product = _unitOfWork.Product.Get(p => p.Id == id);

		if (product == null) return NotFound();

		_unitOfWork.Product.Remove(product);
		_unitOfWork.Save();

		TempData["success"] = "Product deleted successfully.";

		return RedirectToAction("Index");
	}
}



/*					EL CREATE ( previo Upsert )

	////////////////////////////////////////////////
	////////////////////////////////////////////////
	public IActionResult Create()
	{
		IEnumerable<SelectListItem> categoryList = _unitOfWork.Category
								.GetAll().Select(c => new SelectListItem
								{
									Text = c.Name,
									Value = c.Id.ToString(),
								});

		// como en la view ya tengo "@model Product" no puedo llegar y pasar el categoryList
		// a la view, xeso ocupo un ViewBag
		// la key p'acceder a este valor va a ser "CategoryList"
		// ViewBag.CategoryList = categoryList;

		// ViewData["CategoryList"] = categoryList;

		// p' cuando hay que pasar muchos object distintos mejor crear un ViewModel
		ProductVM productVM = new()
		{
			CategoryList = categoryList,
			Product = new Product()
		};

		return View(productVM);
	}

	[HttpPost]
	public IActionResult Create(ProductVM productVM)
	{
		// p'q no me salte el error de validacion ( xq al intentar crear la product aca
		// viene todo el ProductVM y no va a traer el IEnumrable del select, ni todo el
		// Category, solo el CategoryId ) tengo q poner la anotacion [ValidateNever]
		// en las q no quiero q valide ( en la entity )
		if (ModelState.IsValid)
		{
			_unitOfWork.Product.Add(productVM.Product);
			_unitOfWork.Save();

			TempData["success"] = "Product created successfully.";

			return RedirectToAction("Index");
		}

		// si no pongo los [ValidateNever] p'q no salga el error xno tener el IEnumrable
		// es basicamente hacer el populate del dropdown
		//else {
		//	IEnumerable<SelectListItem> categoryList = _unitOfWork.Category
		//						.GetAll().Select(c => new SelectListItem
		//						{
		//							Text = c.Name,
		//							Value = c.Id.ToString(),
		//						});

		//	productVM.CategoryList = categoryList;

		//	return View(productVM);
		//}

		return View();
		// si no es valido el modelo me deja en el mismo lado
		// si quiera ir a otro controller seria:
		// RedirectToAction("nombre_accion","nombre_controller");
	}

*/