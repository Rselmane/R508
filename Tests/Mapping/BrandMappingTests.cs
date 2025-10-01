using App.DTO;
using App.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.AutoMapper;

namespace Tests.Mapping;

public class BrandMappingTests : AutoMapperConfigTests
{
    [TestMethod]
    public void BrandDTO_To_Brand_Works()
    {
        var dto = new BrandDTO { Id = 2, Name = "Adidas" };

        var brand = _mapper.Map<Brand>(dto);

        Assert.AreEqual(dto.Id, brand.IdBrand);
        Assert.AreEqual(dto.Name, brand.BrandName);
        Assert.IsNull(brand.Products);
    }

    [TestMethod]
    public void Brand_To_BrandDTO_Works()
    {
        var brand = new Brand { IdBrand = 1, BrandName = "Nike" };

        var dto = _mapper.Map<BrandDTO>(brand);

        Assert.AreEqual(brand.IdBrand, dto.Id);
        Assert.AreEqual(brand.BrandName, dto.Name);
    }
}
