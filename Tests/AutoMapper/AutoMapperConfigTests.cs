using App.DTO;
using App.Mapper;
using App.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.AutoMapper;

[TestClass]
public abstract class AutoMapperConfigTests
{
    protected readonly IMapper _mapper;
    protected readonly MapperConfiguration _config;

    protected AutoMapperConfigTests()
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
}

