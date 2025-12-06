using System.Collections.Generic;

namespace Backend.Dtos
{
    public class RespuestaCompraDto
    {
        public int CostoTotal { get; init; }
        public int MontoVuelto { get; init; }
        public Dictionary<int, int> DetalleVuelto { get; init; } = new();
        public string Mensaje { get; init; } = string.Empty;
    }
}
