using System;
using System.Collections.Generic;
using Backend.Dtos;
using Backend.Repositories;
using Backend.Services;
using Moq;
using Xunit;

namespace Backend.Tests
{
    public class MaquinaCafeServicioTests
    {
        private static (MaquinaCafeServicio Sut, Mock<IMaquinaCafeRepositorio> RepoMock) CrearServicio()
        {
            var repoMock = new Mock<IMaquinaCafeRepositorio>(MockBehavior.Strict);
            var servicio = new MaquinaCafeServicio(repoMock.Object);
            return (servicio, repoMock);
        }

        [Fact]
        public void ObtenerInventarioCafe_RetornaDatosDelRepositorio()
        {
            var (sut, repoMock) = CrearServicio();
            var esperado = new Dictionary<string, int> { { "Americano", 5 } };
            repoMock.Setup(r => r.ObtenerInventarioCafe()).Returns(esperado);

            var resultado = sut.ObtenerInventarioCafe();

            Assert.Equal(esperado, resultado);
            repoMock.Verify(r => r.ObtenerInventarioCafe(), Times.Once);
        }

        [Fact]
        public void ProcesarOrden_NullSolicitud_LanzaArgumentNullException()
        {
            var (sut, _) = CrearServicio();

            Assert.Throws<ArgumentNullException>(() => sut.ProcesarOrden(null!));
        }

        [Fact]
        public void ProcesarOrden_OrdenVacia_LanzaArgumentException()
        {
            var (sut, _) = CrearServicio();
            var solicitud = new SolicitudCompraCafeDto
            {
                Orden = new Dictionary<string, int>(),
                Pago = new PagoDto { Monedas = new Dictionary<int, int> { { 500, 1 } } }
            };

            var ex = Assert.Throws<ArgumentException>(() => sut.ProcesarOrden(solicitud));
            Assert.Contains("orden", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ProcesarOrden_PagoInsuficiente_LanzaArgumentException()
        {
            var (sut, repoMock) = CrearServicio();
            var precios = new Dictionary<string, int> { { "Americano", 1000 } };
            var inventario = new Dictionary<string, int> { { "Americano", 3 } };

            repoMock.Setup(r => r.ObtenerPreciosCafe()).Returns(precios);
            repoMock.Setup(r => r.ObtenerInventarioCafe()).Returns(inventario);

            var solicitud = new SolicitudCompraCafeDto
            {
                Orden = new Dictionary<string, int> { { "Americano", 2 } },
                Pago = new PagoDto { Monedas = new Dictionary<int, int> { { 500, 3 } } }
            };

            var ex = Assert.Throws<ArgumentException>(() => sut.ProcesarOrden(solicitud));
            Assert.Contains("insuficiente", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ProcesarOrden_SinMonedasParaVuelto_LanzaInvalidOperationException()
        {
            var (sut, repoMock) = CrearServicio();
            var precios = new Dictionary<string, int> { { "Americano", 950 } };
            var inventario = new Dictionary<string, int> { { "Americano", 3 } };
            var monedasDisponibles = new Dictionary<int, int> { { 25, 1 } }; // no alcanza para 50 de vuelto

            repoMock.Setup(r => r.ObtenerPreciosCafe()).Returns(precios);
            repoMock.Setup(r => r.ObtenerInventarioCafe()).Returns(inventario);
            repoMock.Setup(r => r.AgregarPlata(It.IsAny<IReadOnlyDictionary<int, int>>()));
            repoMock.Setup(r => r.ObtenerInventarioMonedas()).Returns(monedasDisponibles);

            var solicitud = new SolicitudCompraCafeDto
            {
                Orden = new Dictionary<string, int> { { "Americano", 1 } },
                Pago = new PagoDto { Monedas = new Dictionary<int, int> { { 1000, 1 } } }
            };

            Assert.Throws<InvalidOperationException>(() => sut.ProcesarOrden(solicitud));
            repoMock.Verify(r => r.RestarCafe(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            repoMock.Verify(r => r.RestarMonedas(It.IsAny<IReadOnlyDictionary<int, int>>()), Times.Never);
        }

        [Fact]
        public void ProcesarOrden_Exitoso_RestaInventarioYGeneraDetalleVuelto()
        {
            var (sut, repoMock) = CrearServicio();
            var precios = new Dictionary<string, int> { { "Americano", 1000 } };
            var inventarioCafe = new Dictionary<string, int> { { "Americano", 4 } };
            var inventarioMonedas = new Dictionary<int, int> { { 1000, 2 }, { 500, 5 }, { 100, 10 } };

            repoMock.Setup(r => r.ObtenerPreciosCafe()).Returns(precios);
            repoMock.Setup(r => r.ObtenerInventarioCafe()).Returns(inventarioCafe);
            repoMock.Setup(r => r.AgregarPlata(It.Is<IReadOnlyDictionary<int, int>>(d => d.ContainsKey(1000) && d[1000] == 3)));
            repoMock.Setup(r => r.ObtenerInventarioMonedas()).Returns(inventarioMonedas);
            repoMock.Setup(r => r.RestarCafe("Americano", 2));
            repoMock.Setup(r => r.RestarMonedas(It.Is<IReadOnlyDictionary<int, int>>(d => d.Count == 1 && d[1000] == 1)));

            var solicitud = new SolicitudCompraCafeDto
            {
                Orden = new Dictionary<string, int> { { "Americano", 2 } },
                Pago = new PagoDto { Monedas = new Dictionary<int, int> { { 1000, 3 } } }
            };

            var respuesta = sut.ProcesarOrden(solicitud);

            Assert.Equal(2000, respuesta.CostoTotal);
            Assert.Equal(1000, respuesta.MontoVuelto);
            Assert.Single(respuesta.DetalleVuelto);
            Assert.Equal(1, respuesta.DetalleVuelto[1000]);
            Assert.Contains("vuelto", respuesta.Mensaje, StringComparison.OrdinalIgnoreCase);

            repoMock.VerifyAll();
        }
    }
}
