using App.DTO;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers;

[Route("api/product")]
[ApiController]
public class ProductController(
    IMapper mapper,
    IDataRepository<Product> manager,
    AppDbContext context
    ) : ControllerBase
{

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailDTO?>> Get(int id)
    {
        var result = await manager.GetByIdAsync(id);
        return result == null ? NotFound() : mapper.Map<ProductDetailDTO>(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        ActionResult<Product?> produit = await manager.GetByIdAsync(id);
        if (produit.Value == null)
            return NotFound();
        await manager.DeleteAsync(produit.Value);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll()
    {
        var produits = await manager.GetAllAsync();
        var produitDTOs = mapper.Map<IEnumerable<ProductDTO>>(produits);
        return new ActionResult<IEnumerable<ProductDTO>>(produitDTOs);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDetailDTO>> Create([FromBody] ProductAddDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Mapping DTO → Produit
        var produit = mapper.Map<Product>(dto);

        // Gestion de la marque
        if (!string.IsNullOrEmpty(dto.Marque))
        {
            var marque = await context.Marques.FirstOrDefaultAsync(x => x.BrandName == dto.Marque);
            if (marque == null)
            {
                marque = new Brand { BrandName = dto.Marque };
                context.Marques.Add(marque);
                await context.SaveChangesAsync();
            }
            produit.IdBrand = marque.IdBrand;
        }

        // Gestion du type produit
        if (!string.IsNullOrEmpty(dto.TypeProduit))
        {
            var typeProduit = await context.TypeProduits.FirstOrDefaultAsync(x => x.TypeProductName == dto.TypeProduit);
            if (typeProduit == null)
            {
                typeProduit = new TypeProduct { TypeProductName = dto.TypeProduit };
                context.TypeProduits.Add(typeProduit);
                await context.SaveChangesAsync();
            }
            produit.IdTypeProduct = typeProduit.IdTypeProduct;
        }

        // Sauvegarde du produit
        await manager.AddAsync(produit);

        // Retourner le détail du produit créé
        var produitDetail = mapper.Map<ProductDetailDTO>(produit);
        return CreatedAtAction("Get", new { id = produit.IdProduct }, produitDetail);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] Product produit)
    {
        if (id != produit.IdProduct)
        {
            return BadRequest();
        }
        ActionResult<Product?> prodToUpdate = await manager.GetByIdAsync(id);
        if (prodToUpdate.Value == null)
        {
            return NotFound();
        }
        await manager.UpdateAsync(prodToUpdate.Value, produit);
        return NoContent();
    }
}