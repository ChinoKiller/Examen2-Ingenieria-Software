using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Backend.Dtos;
using Backend.Repositories;

namespace Backend.Services
{
    public class MaquinaCafeServicio : IMaquinaCafeServicio
    {
        private readonly IMaquinaCafeRepositorio _repositorio;

        public MaquinaCafeServicio(IMaquinaCafeRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public IReadOnlyDictionary<string, int> ObtenerInventarioCafe() 
        { 
            return _repositorio.ObtenerInventarioCafe();
        }

        public IReadOnlyDictionary<string, int> ObtenerPreciosCafe()
        { 
            return _repositorio.ObtenerPreciosCafe();
        }

        public IReadOnlyDictionary<int, int> ObtenerInventarioMonedas()
        { 
            return _repositorio.ObtenerInventarioMonedas();
        }

        public RespuestaCompraDto ProcesarOrden(SolicitudCompraCafeDto solicitud)
        {
            if (solicitud == null)
            {
                throw new ArgumentNullException(nameof(solicitud));
            }

            if (solicitud.Orden == null || solicitud.Orden.Count == 0)
            {
                throw new ArgumentException("La orden no puede estar vacia.", nameof(solicitud.Orden));
            }

            if (solicitud.Pago == null)
            {
                throw new ArgumentException("El pago es requerido.", nameof(solicitud.Pago));
            }

            var precios = _repositorio.ObtenerPreciosCafe();
            var inventario = _repositorio.ObtenerInventarioCafe();

            var costoTotal = CalcularCostoTotal(solicitud.Orden, precios, inventario);
            var montoPago = CalcularPagoTotal(solicitud.Pago);

            if (montoPago <= 0)
            {
                throw new ArgumentException("El monto pagado debe ser mayor a cero.", nameof(montoPago));
            }

            if (montoPago < costoTotal)
            {
                throw new ArgumentException("Dinero insuficiente para completar la compra.");
            }

            var montoVuelto = montoPago - costoTotal;

            AgregarPagoAlInventario(solicitud.Pago);

            var inventarioMonedas = _repositorio.ObtenerInventarioMonedas();
            var detalleVuelto = CalcularVuelto(montoVuelto, inventarioMonedas);

            if (detalleVuelto == null)
            {
                throw new InvalidOperationException("No hay suficientes monedas para su vuelto en la máquina.");
            }

            foreach (var cafe in solicitud.Orden)
            {
                _repositorio.RestarCafe(cafe.Key, cafe.Value);
            }

            _repositorio.RestarMonedas(detalleVuelto);

            var mensaje = ConstruirMensaje(montoVuelto, detalleVuelto);
            return new RespuestaCompraDto
            {
                CostoTotal = costoTotal,
                MontoVuelto = montoVuelto,
                DetalleVuelto = detalleVuelto.ToDictionary(par => par.Key, par => par.Value),
                Mensaje = mensaje
            };
        }

        private void AgregarPagoAlInventario(PagoDto pago)
        {
            if (pago == null)
            {
                return;
            }

            if (pago.Monedas != null && pago.Monedas.Count > 0)
            {
                _repositorio.AgregarPlata(pago.Monedas);
            }
        }

        private static int CalcularCostoTotal(Dictionary<string, int> orden, IReadOnlyDictionary<string, int> precios, IReadOnlyDictionary<string, int> inventario)
        {
            var total = 0;
            foreach (var cafe in orden)
            {
                if (cafe.Value <= 0)
                {
                    throw new ArgumentException("Las cantidades deben ser mayores a cero.");
                }

                if (!precios.TryGetValue(cafe.Key, out var precio))
                {
                    throw new ArgumentException($"El cafe {cafe.Key} no existe en la lista de precios.");
                }

                if (!inventario.TryGetValue(cafe.Key, out var existencias))
                {
                    throw new ArgumentException($"El cafe {cafe.Key} no existe en el inventario.");
                }

                if (cafe.Value > existencias)
                {
                    throw new InvalidOperationException($"No hay suficientes {cafe.Key} en la maquina.");
                }

                total += precio * cafe.Value;
            }

            return total;
        }

        private int CalcularPagoTotal(PagoDto pago)
        {
            int montoTotal = 0;

            if (pago == null || pago.Monedas == null || pago.Monedas.Count == 0)
            {
                return montoTotal;
            }

            if (pago.Monedas != null && pago.Monedas.Count > 0)
            {
                foreach (var moneda in pago.Monedas)
                {
                    montoTotal += moneda.Key * moneda.Value;
                }

            }

            return montoTotal;
        }

        private static IReadOnlyDictionary<int, int>? CalcularVuelto(int montoVuelto, IReadOnlyDictionary<int, int> inventarioMonedas)
        {
            if (montoVuelto == 0)
            {
                return new Dictionary<int, int>();
            }

            var restante = montoVuelto;
            var resultado = new Dictionary<int, int>();

            foreach (var denominacion in inventarioMonedas.Keys.OrderByDescending(valor => valor))
            {
                var disponibles = inventarioMonedas[denominacion];

                var monedasNecesarias = Math.Min(restante / denominacion, disponibles); 

                if (monedasNecesarias > 0)
                {
                    resultado[denominacion] = monedasNecesarias;
                    restante -= denominacion * monedasNecesarias;
                }

                if (restante == 0)
                {
                    break;
                }
            }

            return restante == 0 ? resultado : null;
        }

        private static string ConstruirMensaje(int montoVuelto, IReadOnlyDictionary<int, int> detalleVuelto)
        {
            if (montoVuelto == 0)
            {
                return "Compra completada. No hay vuelto.";
            }

            var builder = new StringBuilder();
            builder.Append($"Su vuelto es de: {montoVuelto} colones. Desglose:");

            foreach (var entrada in detalleVuelto.OrderByDescending(par => par.Key))
            {
                builder.Append($" {entrada.Value} moneda(s) de {entrada.Key}");
            }

            return builder.ToString();
        }
    }
}