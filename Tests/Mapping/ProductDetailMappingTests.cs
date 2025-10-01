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

public class ProductDetailMappingTests : AutoMapperConfigTests
{
    [TestMethod]
    public void ProductDetailDTO_To_Product_Works()
    {
        var dto = new ProductDetailDTO
        {
            Id = 5,
            Name = "DetailDto",
            Brand = "BrandDetail",
            Type = "TypeDetail",
            PhotoName = "pic.png",
            PhotoUri = "http://example.com/pic.png",
            Description = "A description",
            Stock = 7
        };

        var product = _mapper.Map<Product>(dto);

        Assert.AreEqual(dto.Id, product.IdProduct);
        Assert.AreEqual(dto.Name, product.ProductName);
        Assert.AreEqual(dto.Brand, product.NavigationBrand?.BrandName);
        Assert.AreEqual(dto.Type, product.NavigationTypeProduct?.TypeProductName);
        Assert.AreEqual(dto.PhotoName, product.PhotoName);
        Assert.AreEqual(dto.PhotoUri, product.PhotoUri);
        Assert.AreEqual(dto.Description, product.Description);
        Assert.AreEqual(dto.Stock ?? 0, product.ActualStock);
    }

    [TestMethod]
    public void Product_To_ProductDetailDto_Works()
    {
        var entity = new Product
        {
            IdProduct = 2,
            ProductName = "ProduitEntity",
            ActualStock = 5,
            MinStock = 10,
            NavigationBrand = new Brand { BrandName = "TestBrand" },
            NavigationTypeProduct = new TypeProduct { TypeProductName = "TestType" }
        };

        var dto = _mapper.Map<ProductDetailDTO>(entity);

        Assert.AreEqual(entity.ProductName, dto.Name);
        Assert.AreEqual(entity.NavigationBrand.BrandName, dto.Brand);
        Assert.AreEqual(entity.NavigationTypeProduct.TypeProductName, dto.Type);
        Assert.AreEqual(5, dto.Stock);
        Assert.IsTrue(dto.InRestocking);
    }
}
