using App.DTO;
using App.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.AutoMapper;

namespace Tests.Mapping
{
    [TestClass]
    public class BrandUpdateMappingTest : AutoMapperConfigTests
    {
        [TestMethod]
        public void BrandUpdateDTO_To_Brand_Works()
        {
            var dto = new BrandUpdateDTO { Name = "Puma" };

            var brand = _mapper.Map<Brand>(dto);

            Assert.AreEqual(dto.Name, brand.BrandName);
            Assert.IsNull(brand.Products);
        }

        [TestMethod]
        public void Brand_To_BrandUpdateDTO_Works()
        {
            var brand = new Brand { IdBrand = 1, BrandName = "Reebok" };

            var dto = _mapper.Map<BrandUpdateDTO>(brand);

            Assert.AreEqual(brand.BrandName, dto.Name);
        }
    }
}