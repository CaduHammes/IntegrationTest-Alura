using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.Infrastructure;
namespace Alura.CoisasAFazer.Testes
{
    public class ObtemCategoriaPorIdExecute
    {
        [Fact]
        public void QuandoOIdForExistenteDeveChamarObtemCategoriaPorIdUmaUnicaVez()
        {
            //Arrange
            var idCategoria = 20;
            var comando = new ObtemCategoriaPorId(idCategoria);

            var mock = new Mock<IRepositorioTarefas>();

            var repo = mock.Object;

            var handler = new ObtemCategoriaPorIdHandler(repo);

            //Act
            handler.Execute(comando);

            //Assert
            mock.Verify(r => r.ObtemCategoriaPorId(idCategoria), Times.Once());

        }
    }
}
