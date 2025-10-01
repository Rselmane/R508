using App.Controllers;
using App.DTO;
using App.Mapper;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Controllers
{

    [TestClass]
    [TestSubject(typeof(TypeProductController))]
    [TestCategory("integration")]
    public class TypeProductControllerTest
    {
        private readonly AppDbContext _context;
        private readonly TypeProductController _typeProductController;
        private readonly MapperConfiguration _config;

        public TypeProductControllerTest()
        {
            _context = new AppDbContext();

            _config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TypeProductMapper>();
            }, new LoggerFactory());

            _config.AssertConfigurationIsValid();
            IMapper mapper = _config.CreateMapper();

            IDataRepository<TypeProduct> manager = new TypeProductManager(_context);
            _typeProductController = new TypeProductController(mapper, manager, _context);
        }

        [TestMethod]
        public void ShouldGetTypeProduct()
        {
            // Given : un produit en base de donnée
            TypeProduct typeProductInDb = new TypeProduct()
            {
                TypeProductName = "School"
            };
            _context.TypeProducts.Add(typeProductInDb);
            _context.SaveChanges();

            // When : J'appelle la méthode get de mon api pour récupérer le produit
            ActionResult<TypeProductDTO> action = _typeProductController.Get(typeProductInDb.IdTypeProduct).GetAwaiter().GetResult();
            TypeProductDTO returnTypeProduct = action.Value;

            // Then : On récupère le produit et le code de retour est 200
            Assert.IsNotNull(action);
            Assert.IsInstanceOfType(action.Value, typeof(TypeProductDTO));
            Assert.AreEqual(typeProductInDb.TypeProductName, returnTypeProduct.Name);
        }

    }
}
