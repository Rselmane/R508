using App.DTO;
using App.Models;
using App.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers;


[Route("api/brands")]
[ApiController]
public class BrandController(
    IMapper mapper,
    IDataRepository<Brand> manager,
    AppDbContext context
    ) : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BrandDTO?>> Get(int id)
    {
        var result = await manager.GetByIdAsync(id);
        return result == null ? NotFound() : mapper.Map<BrandDTO>(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        ActionResult<Brand?> brand = await manager.GetByIdAsync(id);
        if (brand.Value == null)
            return NotFound();
        await manager.DeleteAsync(brand.Value);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BrandDTO>>> GetAll()
    {
        var brands = await manager.GetAllAsync();
        var brandDTOs = mapper.Map<IEnumerable<BrandDTO>>(brands);
        return new ActionResult<IEnumerable<BrandDTO>>(brandDTOs);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BrandDTO>> Create([FromBody] BrandDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Mapping DTO → Marque
        Brand brand = mapper.Map<Brand>(dto);


        // Sauvegarde de la  marque
        await manager.AddAsync(brand);

        // Retourner le détail de la marque  créé
        ProductDetailDTO BrandDetail = mapper.Map<ProductDetailDTO>(brand);
        return CreatedAtAction("Get", new { id = brand.IdBrand }, BrandDetail);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] Brand brand)
    {
        if (id != brand.IdBrand)
        {
            return BadRequest();
        }
        ActionResult<Brand?> brandToUpdate = await manager.GetByIdAsync(id);
        if (brandToUpdate.Value == null)
        {
            return NotFound();
        }
        await manager.UpdateAsync(brandToUpdate.Value, brand);
        return NoContent();
    }
}
