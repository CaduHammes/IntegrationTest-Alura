using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Alura.CoisasAFazer.WebApp.Controllers;
using Alura.CoisasAFazer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Alura.CoisasAFazer.Core.Models;

namespace Alura.CoisasAFazer.Testes
{
    public class TarefasControllerEndPoitCadastraTarefa
    {
        [Fact]
        public void DadaTarefaComInfoValidasDeveRetornar200()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;
            var contexto = new DbTarefasContext(options);
            contexto.Categorias.Add(new Categoria(20, "Estudo"));
            contexto.SaveChanges();
            var repo = new RepositorioTarefa(contexto);

            var controlador = new TarefasController(repo, mockLogger.Object);

            var model = new CadastraTarefaVM();

            model.IdCategoria = 20;
            model.Titulo = "Estudar XUnit";
            model.Prazo = new DateTime(2019, 12, 31);

            //Act
            var retorno = controlador.EndpointCadastraTarefa(model);

            //Assert
            Assert.IsType<OkResult>(retorno);
        }

        [Fact]
        public void QuandoExcecaoForLancaedaDeveRetornar500()
        {
            //Arrange
            var logger = new Mock<ILogger<CadastraTarefaHandler>>();

            var repo = new Mock<IRepositorioTarefas>();
            repo.Setup(r => r.ObtemCategoriaPorId(20)).Returns(new Categoria(20, "Estudo"));
            repo.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(new Exception("Houve um erro"));

            var controlador = new TarefasController(repo.Object, logger.Object);

            var model = new CadastraTarefaVM();

            model.IdCategoria = 20;
            model.Titulo = "Estudar XUnit";
            model.Prazo = new DateTime(2019, 12, 31);

            //Act
            var retorno = controlador.EndpointCadastraTarefa(model);

            //Assert
            Assert.IsType<StatusCodeResult>(retorno);
            var statusCodeRetornado = (retorno as StatusCodeResult).StatusCode;
            Assert.Equal(500, statusCodeRetornado);
        }
    }
}
