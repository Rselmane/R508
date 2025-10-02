using App.DTO;
using App.Models;
using App.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;


[Route("api/brand")]
[ApiController]
public class BrandController(
    IMapper mapper,
    IDataRepository<Brand> manager,
    AppDbContext context
    ) : ControllerBase
{
    [HttpGet("brand/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var result = await manager.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(mapper.Map<BrandDTO>(result));
    }

    [HttpDelete("remove/{id}")]
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

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BrandDTO>>> GetAll()
    {
        var brands = await manager.GetAllAsync();
        if (brands == null || !brands.Any())
            return NotFound();

        var brands_dto = mapper.Map<IEnumerable<BrandDTO>>(brands);
        return Ok(brands_dto);
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] BrandUpdateDTO dto)
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
        BrandDTO BrandDetail = mapper.Map<BrandDTO>(brand);
        return CreatedAtAction("Get", new { id = brand.IdBrand }, BrandDetail);
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] BrandUpdateDTO brandDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Récupérer la marque existante
        Brand? brandToUpdate = await manager.GetByIdAsync(id);

        if (brandToUpdate == null)
        {
            return NotFound();
        }

        // Mapper le DTO vers une nouvelle entité Brand
        Brand updatedBrand = mapper.Map<Brand>(brandDto);
        updatedBrand.IdBrand = id; // Conserver l'ID

        await manager.UpdateAsync(brandToUpdate, updatedBrand);

        return NoContent();
    }
}
