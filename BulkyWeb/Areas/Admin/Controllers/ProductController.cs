using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
	private readonly IUnitOfWork _unitOfWork;

	public ProductController(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
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

	////////////////////////////////////////////////
	////////////////////////////////////////////////
	public IActionResult Edit(int? id)
	{
		if (id == null || id == 0) return NotFound();

		var product = _unitOfWork.Product.Get(p => p.Id == id);

		if(product == null) return NotFound();

		return View(product);
	}

	[HttpPost]
	public IActionResult Edit(Product obj)
	{
		if(ModelState.IsValid)
		{
			_unitOfWork.Product.Update(obj);
			_unitOfWork.Save();

			TempData["success"] = "Product edited successfully.";

			return RedirectToAction("Index");
		}

		return View();
	}


	////////////////////////////////////////////////
	////////////////////////////////////////////////
	public IActionResult Delete(int? id)
	{
		if (id == null || id == 0) return NotFound();

		var product = _unitOfWork.Product.Get(p => p.Id == id);

		if(product == null) return NotFound();

		return View(product);
	}

	[HttpPost, ActionName("Delete")]
	public IActionResult DeletePOST(int? id)
	{
		if (id == null || id == 0) return NotFound();

		var product = _unitOfWork.Product.Get(p => p.Id == id);

		if(product == null) return NotFound();

		_unitOfWork.Product.Remove(product);
		_unitOfWork.Save();

		TempData["success"] = "Product deleted successfully.";

		return RedirectToAction("Index");
	}
}
