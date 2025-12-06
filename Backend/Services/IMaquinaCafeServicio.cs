using System.Collections.Generic;
using Backend.Dtos;

namespace Backend.Services
{
    public interface IMaquinaCafeServicio
    {
        IReadOnlyDictionary<string, int> ObtenerInventarioCafe();
        IReadOnlyDictionary<string, int> ObtenerPreciosCafe();
        IReadOnlyDictionary<int, int> ObtenerInventarioMonedas();
        RespuestaCompraDto ProcesarOrden(SolicitudCompraCafeDto solicitud);
    }
}
