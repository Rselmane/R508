namespace Tests.Mapping
{
    using App.DTO;
    using App.Models;
    using global::Tests.AutoMapper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    namespace Tests.Mapping
    {
        [TestClass]
        public class TypeProductMappingTest : AutoMapperConfigTests
        {
            [TestMethod]
            public void TypeProductUpdateDTO_To_TypeProduct_Works()
            {
                TypeProductDTO dto = new TypeProductDTO { Name = "Puma" };

                TypeProduct typeProduct = _mapper.Map<TypeProduct>(dto);

                Assert.AreEqual(dto.Name, typeProduct.TypeProductName);
                Assert.IsNull(typeProduct.Products);
            }

            [TestMethod]
            public void TypeProduct_To_TypeProductDTO_Works()
            {
                TypeProduct typeProduct = new TypeProduct { IdTypeProduct = 1, TypeProductName = "Home" };

                TypeProductDTO dto = _mapper.Map<TypeProductDTO>(typeProduct);

                Assert.AreEqual(typeProduct.TypeProductName, dto.Name);
            }
        }
    }
}
