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
    IProductRepository manager,
    AppDbContext context
    ) : ControllerBase
{

    [HttpGet("details/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var result = await manager.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(mapper.Map<ProductDetailDTO>(result));
    }

    [HttpDelete("remove/{id}")]
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

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll()
    {
        var products = await manager.GetAllWithRelationsAsync();
        if (products == null || !products.Any())
            return NotFound();

        var products_dto = mapper.Map<IEnumerable<ProductDTO>>(products);
        return Ok(products_dto);
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ProductAddDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Mapping DTO → Produit
        var produit = mapper.Map<Product>(dto);

        // Gestion de la marque
        if (!string.IsNullOrEmpty(dto.Brand))
        {
            var marque = await context.Brands.FirstOrDefaultAsync(x => x.BrandName == dto.Brand);
            if (marque == null)
            {
                marque = new Brand { BrandName = dto.Brand };
                context.Brands.Add(marque);
                await context.SaveChangesAsync();
            }
            produit.IdBrand = marque.IdBrand;
        }

        // Gestion du type produit
        if (!string.IsNullOrEmpty(dto.TypeProduct))
        {
            var typeProduit = await context.TypeProducts.FirstOrDefaultAsync(x => x.TypeProductName == dto.TypeProduct);
            if (typeProduit == null)
            {
                typeProduit = new TypeProduct { TypeProductName = dto.TypeProduct };
                context.TypeProducts.Add(typeProduit);
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

    [HttpPut("update/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] ProductAddDTO dto)
    {
        // Récupérer le produit existant
        Product? produitToUpdate = await manager.GetByIdAsync(id);
        if (produitToUpdate == null)
        {
            return NotFound();
        }

        // Mapper le DTO vers une nouvelle entité Product
        var updatedProduct = mapper.Map<Product>(dto);
        updatedProduct.IdProduct = id; // Conserver l'ID

        // Gestion de la marque
        if (!string.IsNullOrEmpty(dto.Brand))
        {
            var marque = await context.Brands.FirstOrDefaultAsync(x => x.BrandName == dto.Brand);
            if (marque == null)
            {
                marque = new Brand { BrandName = dto.Brand };
                context.Brands.Add(marque);
                await context.SaveChangesAsync();
            }
            updatedProduct.IdBrand = marque.IdBrand;
        }

        // Gestion du type produit
        if (!string.IsNullOrEmpty(dto.TypeProduct))
        {
            var typeProduit = await context.TypeProducts.FirstOrDefaultAsync(x => x.TypeProductName == dto.TypeProduct);
            if (typeProduit == null)
            {
                typeProduit = new TypeProduct { TypeProductName = dto.TypeProduct };
                context.TypeProducts.Add(typeProduit);
                await context.SaveChangesAsync();
            }
            updatedProduct.IdTypeProduct = typeProduit.IdTypeProduct;
        }

        // Mettre à jour le produit
        await manager.UpdateAsync(produitToUpdate, updatedProduct);

        return NoContent();
    }
}