using System.Collections.Generic;

namespace Backend.Repositories
{
    public interface IMaquinaCafeRepositorio
    {
        IReadOnlyDictionary<string, int> ObtenerInventarioCafe();
        IReadOnlyDictionary<string, int> ObtenerPreciosCafe();
        IReadOnlyDictionary<int, int> ObtenerInventarioMonedas();
        void RestarCafe(string nombreCafe, int cantidad);
        void RestarMonedas(IReadOnlyDictionary<int, int> monedasAUtilizar);
        void AgregarPlata(IReadOnlyDictionary<int, int> denominaciones);
    }
}
