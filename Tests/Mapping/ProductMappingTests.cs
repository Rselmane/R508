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

[TestClass]
public class ProductMappingTests : AutoMapperConfigTests
{
    [TestMethod]
    public void ProductDTO_To_Product_Works()
    {
        var dto = new ProductDTO
        {
            Id = 1,
            Name = "ProductTest",
            Type = "TypeTest",
            Brand = "BrandTest"
        };

        var entiry = _mapper.Map<Product>(dto);

        Assert.AreEqual(dto.Id, entiry.IdProduct);
        Assert.AreEqual(dto.Name, entiry.ProductName);
        Assert.AreEqual(dto.Type, entiry.NavigationTypeProduct?.TypeProductName);
        Assert.AreEqual(dto.Brand, entiry.NavigationBrand?.BrandName);
    }

    [TestMethod]
    public void Product_To_ProductDTO_Works()
    {
        var entity = new Product
        {
            IdProduct = 3,
            ProductName = "ProductEntity",
            NavigationBrand = new Brand { BrandName = "BrandTest" },
            NavigationTypeProduct = new TypeProduct { TypeProductName = "TypeTest" }
        };

        var dto = _mapper.Map<ProductDTO>(entity);

        Assert.AreEqual(entity.IdProduct, dto.Id);
        Assert.AreEqual(entity.ProductName, dto.Name);
        Assert.AreEqual(entity.NavigationTypeProduct?.TypeProductName, dto.Type);
        Assert.AreEqual(entity.NavigationBrand?.BrandName, dto.Brand);
    }
}
