using App.DTO;
using App.Models;
using App.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers;

[Route("api/typeproduct")]
[ApiController]
public class TypeProductController(
    IMapper mapper,
    IDataRepository<TypeProduct> manager,
    AppDbContext context
    ) : ControllerBase
{
    [HttpGet("type_product/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var result = await manager.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(mapper.Map<TypeProductDTO>(result));
    }

    [HttpDelete("remove/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        ActionResult<TypeProduct?> typeProduct = await manager.GetByIdAsync(id);
        if (typeProduct.Value == null)
            return NotFound();
        await manager.DeleteAsync(typeProduct.Value);
        return NoContent();
    }

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TypeProductDTO>>> GetAll()
    {
        var typesProduct = await manager.GetAllAsync();
        if (typesProduct == null || !typesProduct.Any())
            return NotFound();

        var typesProduct_dto = mapper.Map<IEnumerable<TypeProductDTO>>(typesProduct);
        return Ok(typesProduct_dto);
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] TypeProductDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Mapping DTO → Marque
        TypeProduct typeProduct = mapper.Map<TypeProduct>(dto);


        // Sauvegarde de la  marque
        await manager.AddAsync(typeProduct);

        // Retourner le détail de la marque  créé
        TypeProductDTO typeProductDetail = mapper.Map<TypeProductDTO>(typeProduct);
        return CreatedAtAction("Get", new { id = typeProduct.IdTypeProduct }, typeProductDetail);
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] TypeProductUpdateDTO typeProductDTO)
    { // Récupérer le type de produit  existante

        TypeProduct? TypeProductToUpdate = await manager.GetByIdAsync(id);

        if (TypeProductToUpdate == null)
        {
            return NotFound();
        }

        // Mapper le DTO vers une nouvelle entité TypeProduct
        TypeProduct updatedTypeProduct = mapper.Map<TypeProduct>(typeProductDTO);
        updatedTypeProduct.IdTypeProduct = id; // Conserver l'ID

        await manager.UpdateAsync(TypeProductToUpdate, updatedTypeProduct);

        return NoContent();

    }
}
