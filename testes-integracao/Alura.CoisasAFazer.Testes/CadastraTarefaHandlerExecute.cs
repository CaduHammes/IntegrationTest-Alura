using System;
using Xunit;
using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Services.Handlers;
using System.Linq;
using Alura.CoisasAFazer.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Alura.CoisasAFazer.Testes
{
    public class CadastraTarefaHandlerExecute
    {

        [Fact]
        public void DadaTerafeComInfoValidasDeveIncluirNoBanco()
        {
            //Arrange
            var comando = new CadastraTarefa("Estudar XUnit", new Categoria("Estudo"), new DateTime(2019, 12, 31));

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;
            var context = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(context);

            var handler = new CadastraTarefaHandler(repo);

            //Act
            handler.Execute(comando);

            //Assert
            var tarefas = repo.ObtemTarefas(t => t.Titulo == "Estudar XUnit").FirstOrDefault();
            Assert.NotNull(tarefas);
        }

        [Fact]
        public void QuandoExceptionForLancadaResultadoIsSuccessDeveSerFalso()
        {
            //Arrange
            var comando = new CadastraTarefa("Estudar XUnit", new Categoria("Estudo"), new DateTime(2019, 12, 31));

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;
            var context = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(context);

            var handler = new CadastraTarefaHandler(repo);

            //Act
            CommandResult resultado = handler.Execute(comando);

            //Assert
            Assert.False(resultado.IsSuccess);

        }
    }
}
