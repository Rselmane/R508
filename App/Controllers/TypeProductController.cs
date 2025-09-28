using App.DTO;
using App.Models;
using App.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers;

[Route("api/typeproducts")]
[ApiController]
public class TypeProductController(
    IMapper mapper,
    IDataRepository<TypeProduct> manager,
    AppDbContext context
    ) : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TypeProductDTO?>> Get(int id)
    {
        var result = await manager.GetByIdAsync(id);
        return result == null ? NotFound() : mapper.Map<TypeProductDTO>(result);
    }

    [HttpDelete("{id}")]
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TypeProductDTO>>> GetAll()
    {
        var typesProduct = await manager.GetAllAsync();
        var typeProductDTOs = mapper.Map<IEnumerable<TypeProductDTO>>(typesProduct);
        return new ActionResult<IEnumerable<TypeProductDTO>>(typeProductDTOs);
    }

    // TODO Create TypeProductController
    //
    //

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] TypeProduct typeProduct)
    {
        if (id != typeProduct.IdTypeProduct)
        {
            return BadRequest();
        }
        ActionResult<TypeProduct?> typeProductToUpdate = await manager.GetByIdAsync(id);
        if (typeProductToUpdate.Value == null)
        {
            return NotFound();
        }
        await manager.UpdateAsync(typeProductToUpdate.Value, typeProduct);
        return NoContent();
    }
}
