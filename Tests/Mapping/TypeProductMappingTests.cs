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
public class TypeProductMappingTests : AutoMapperConfigTests
{
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
