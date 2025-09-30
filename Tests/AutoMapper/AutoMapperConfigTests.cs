using App.DTO;
using App.Mapper;
using App.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.AutoMapper;

[TestClass]
public class AutoMapperConfigTests
{
    private readonly IMapper _mapper;
    private readonly MapperConfiguration _config;

    public AutoMapperConfigTests()
    {
        _config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductMapper>();
            cfg.AddProfile<ProductDetailMapper>();
            cfg.AddProfile<BrandMapper>();
            cfg.AddProfile<TypeProductMapper>();
        }, new LoggerFactory());

        _config.AssertConfigurationIsValid();
        _mapper = _config.CreateMapper();
    }

    [TestMethod]
    public void ConfigurationAutoMapper_IsValid()
    {
        Assert.IsNotNull(_mapper);
    }

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

    [TestMethod]
    public void TypeProductDTO_To_TypeProduct_Should_Map_Correctly()
    {
        var dto = new TypeProductDTO { Id = 20, Name = "T-shirt" };

        var type = _mapper.Map<TypeProduct>(dto);

        Assert.AreEqual(dto.Id, type.IdTypeProduct);
        Assert.AreEqual(dto.Name, type.TypeProductName);
        Assert.IsNull(type.Products);
    }

    [TestMethod]
    public void TypeProduct_To_TypeProductDTO_Should_Map_Correctly()
    {
        var type = new TypeProduct { IdTypeProduct = 10, TypeProductName = "Chaussure" };

        var dto = _mapper.Map<TypeProductDTO>(type);

        Assert.AreEqual(type.IdTypeProduct, dto.Id);
        Assert.AreEqual(type.TypeProductName, dto.Name);
    }
}

