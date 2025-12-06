using System.Collections.Generic;

namespace Backend.Dtos
{
    public class SolicitudCompraCafeDto
    {
        public Dictionary<string, int> Orden { get; set; } = new();
        public PagoDto Pago { get; set; } = new();
    }

    public class PagoDto
    {
        public Dictionary<int, int> Monedas { get; set; } = new();
    }
}
