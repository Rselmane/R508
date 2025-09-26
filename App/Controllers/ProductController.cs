using App.DTO;
using App.Mapper;
using App.Models;

using App.Models.EntityFramework;
using App.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers;

[Route("api/produits")]
[ApiController]
public class ProductController(
    IMapper<Produit, ProduitDTO> produitMapperDTO,
    IMapper<Produit, ProduitDetailDTO> produitDetailMapper,
    IMapper<Produit, ProductAddDTO> productAddMapper,
    IDataRepository<Produit> manager,
    AppDbContext context
    ) : ControllerBase
{
    private ProductManager manager;

    public ProductController(ProductManager manager)
    {
        this.manager = manager;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProduitDetailDTO?>> Get(int id)
    {
        var result = await manager.GetByIdAsync(id);
        return result == null ? NotFound() : produitDetailMapper.ToDTO(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        ActionResult<Produit?> produit = await manager.GetByIdAsync(id);
        if (produit.Value == null)
            return NotFound();
        await manager.DeleteAsync(produit.Value);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProduitDTO>>> GetAll()
    {
        IEnumerable<ProduitDTO> produits = produitMapperDTO.ToDTOs((await manager.GetAllAsync()));
        return new ActionResult<IEnumerable<ProduitDTO>>(produits);
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProduitDetailDTO>> Create([FromBody] ProductAddDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Mapping DTO → Produit
        var produit = productAddMapper.ToEntity(dto);

        // Gestion de la marque
        if (!string.IsNullOrEmpty(dto.Marque))
        {
            var marque = await context.Marques.FirstOrDefaultAsync(x => x.NomMarque == dto.Marque);
            if (marque == null)
            {
                marque = new Marque { NomMarque = dto.Marque };
                context.Marques.Add(marque);
                await context.SaveChangesAsync();
            }
            produit.IdMarque = marque.IdMarque;
        }

        // Gestion du type produit
        if (!string.IsNullOrEmpty(dto.TypeProduit))
        {
            var typeProduit = await context.TypeProduits.FirstOrDefaultAsync(x => x.NomTypeProduit == dto.TypeProduit);
            if (typeProduit == null)
            {
                typeProduit = new TypeProduit { NomTypeProduit = dto.TypeProduit };
                context.TypeProduits.Add(typeProduit);
                await context.SaveChangesAsync();
            }
            produit.IdTypeProduit = typeProduit.IdTypeProduit;
        }

        // Sauvegarde du produit
        await manager.AddAsync(produit);

        // Retourner le détail du produit créé
        var produitDetail = produitDetailMapper.ToDTO(produit);
        return CreatedAtAction("Get", new { id = produit.IdProduit }, produitDetail);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] Produit produit)
    {
        if (id != produit.IdProduit)
        {
            return BadRequest();
        }
        ActionResult<Produit?> prodToUpdate = await manager.GetByIdAsync(id);
        if (prodToUpdate.Value == null)
        {
            return NotFound();
        }
        await manager.UpdateAsync(prodToUpdate.Value, produit);
        return NoContent();
    }
}