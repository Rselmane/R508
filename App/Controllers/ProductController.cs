using App.Models;
using App.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[Route("api/produits")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IDataRepository<Produit> _productManager;

    public ProductController(ProductManager manager)
    {
        _productManager = manager;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Produit>> Get(int id)
    {
        var result = await _productManager.GetByIdAsync(id);

        if (result.Value == null)
        {
            return NotFound();
        }

        return result;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produit>>> GetAll()
    {
        return await _productManager.GetAllAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Produit>> Create(Produit produit)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _productManager.AddAsync(produit);
        return CreatedAtAction("Get", new { id = produit.IdProduit }, produit);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update( int id , Produit produit)
    {
        if (id != produit.IdProduit)
        {
            return BadRequest();
        }

        var existingProduit = await _productManager.GetByIdAsync(produit.IdProduit);
        if (existingProduit == null)
        {
            return NotFound();
        }

        await _productManager.UpdateAsync(existingProduit.Value, produit);
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        ActionResult<Produit?> produit = await _productManager.GetByIdAsync(id);

        if (produit.Value == null)
            return NotFound();

        await _productManager.DeleteAsync(produit.Value);
        return NoContent();
    }



}