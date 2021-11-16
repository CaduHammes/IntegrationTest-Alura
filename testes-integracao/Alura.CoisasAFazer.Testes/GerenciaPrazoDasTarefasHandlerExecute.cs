using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Alura.CoisasAFazer.Testes
{
    public class GerenciaPrazoDasTarefasHandlerExecute
    {
        [Fact]
        public void QuandoTarefasEstiveremAtrasadasDeveMudarSeuStatus()
        {
            //Arrange
            var compCateg = new Categoria(1, "Compras");
            var casaCateg = new Categoria(2, "Casa");
            var trabCateg = new Categoria(3, "Trabalho");
            var saudCateg = new Categoria(4, "Saude");
            var higiCateg = new Categoria(5, "Higiene");

            var tarefas = new List<Tarefa> {
            //Atrasadas a partir de 1/1/2019
            new Tarefa(1, "Tirar o lixo", casaCateg, new DateTime(2018, 12,31), null, StatusTarefa.Criada),
            new Tarefa(4, "Fazer o almoço", casaCateg, new DateTime(2017, 12, 1), null, StatusTarefa.Criada),
            new Tarefa(9, "Ir a academia", saudCateg, new DateTime(2018,12,31), null, StatusTarefa.Criada),
            new Tarefa(7, "Conlcuir o relatorio", trabCateg, new DateTime(2018,5,7), null, StatusTarefa.Pendente),
            new Tarefa(10, "Beber agua", saudCateg, new DateTime(2018,12,31), null,StatusTarefa.Criada),
            //Dentro do prazo em 1/1/2019
            new Tarefa(8, "Comparecer a reuniao", trabCateg, new DateTime(2018,11,12), new DateTime(2018,11,30), StatusTarefa.Concluida),
            new Tarefa(2, "Arrumar a casa", casaCateg, new DateTime(2019,4,5), null, StatusTarefa.Criada),
            new Tarefa(3, "Escovar os dentes", higiCateg, new DateTime(2019,1,2), null, StatusTarefa.Criada),
            new Tarefa(5, "Comprar presente pro Joao", compCateg, new DateTime(2019,10,8), null, StatusTarefa.Criada),
            new Tarefa(6, "Comprar raçao", compCateg, new DateTime(2019,11,20), null, StatusTarefa.Criada)
            };

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;
            var context = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(context);

            repo.IncluirTarefas(tarefas.ToArray());

            var comando = new GerenciaPrazoDasTarefas(new DateTime(2019, 1, 1));
            var handler = new GerenciaPrazoDasTarefasHandler(repo);

            //Act
            handler.Execute(comando);

            //Assert
            var tarefasEmAtraso = repo.ObtemTarefas(t => t.Status == StatusTarefa.EmAtraso);
            Assert.Equal(5, tarefasEmAtraso.Count());
        }

        [Fact]
        public void QuandoExecuteForInvocadoDeveChamarAtualizarTarefasNaQtDeVezesTotalDeTarefasAtrasadas()
        {
            //Arrange
            var tarefas = new List<Tarefa>
            {
                //Atrasadas
                new Tarefa(1, "Tirar o lixo", new Categoria("Dummy"), new DateTime(2018, 12,31), null, StatusTarefa.Criada),
                new Tarefa(4, "Fazer o almoço", new Categoria("Dummy"), new DateTime(2017, 12, 1), null, StatusTarefa.Criada),
                new Tarefa(9, "Ir a academia", new Categoria("Dummy"), new DateTime(2018,12,31), null, StatusTarefa.Criada)
                //Dentro do prazo
            };
            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.ObtemTarefas(It.IsAny<Func<Tarefa, bool>>()))
                .Returns(tarefas);

            var repo = mock.Object;

            var comando = new GerenciaPrazoDasTarefas(new DateTime(2019, 1, 1));
            var handler = new GerenciaPrazoDasTarefasHandler(repo);

            //Act
            handler.Execute(comando);

            //Assert
            mock.Verify(r => r.AtualizarTarefas(It.IsAny<Tarefa[]>()), Times.Exactly(1));
        }
    }
}
