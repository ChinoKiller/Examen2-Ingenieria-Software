using System.Collections.Generic;

namespace Backend.Models
{
    public class ResultadoCompra
    {
        public ResultadoCompra(int costoTotal, int montoVuelto, IReadOnlyDictionary<int, int> detalleVuelto, string mensaje)
        {
            CostoTotal = costoTotal;
            MontoVuelto = montoVuelto;
            DetalleVuelto = detalleVuelto;
            Mensaje = mensaje;
        }

        public int CostoTotal { get; }
        public int MontoVuelto { get; }
        public IReadOnlyDictionary<int, int> DetalleVuelto { get; }
        public string Mensaje { get; }
    }
}
