using System;
using System.Collections.Generic;

namespace Backend.Repositories
{
    public class MaquinaCafeRepositorio : IMaquinaCafeRepositorio
    {
        private readonly Dictionary<string, int> _inventarioCafe = new()
        {
            { "Americano", 10 },
            { "Cappuccino", 8 },
            { "Lates", 10 },
            { "Mocaccino", 15 }
        };

        private readonly Dictionary<string, int> _preciosCafe = new()
        {
            { "Americano", 950 },
            { "Cappuccino", 1200 },
            { "Lates", 1350 },
            { "Mocaccino", 1500 }
        };

        private readonly Dictionary<int, int> _inventarioMonedas = new()
        {
            { 1000, 0 },
            { 500, 20 },
            { 100, 30 },
            { 50, 50 },
            { 25, 25 }
        };
        

        public IReadOnlyDictionary<string, int> ObtenerInventarioCafe()
        {
            return new Dictionary<string, int>(_inventarioCafe);
        }

        public IReadOnlyDictionary<string, int> ObtenerPreciosCafe()
        {
            return new Dictionary<string, int>(_preciosCafe);
        }

        public IReadOnlyDictionary<int, int> ObtenerInventarioMonedas()
        {
            return new Dictionary<int, int>(_inventarioMonedas);
        }

        public void RestarCafe(string nombreCafe, int cantidad)
        {
            if (string.IsNullOrWhiteSpace(nombreCafe))
            {
                throw new ArgumentException("El nombre del cafe es requerido.", nameof(nombreCafe));
            }

            if (!_inventarioCafe.ContainsKey(nombreCafe))
            {
                throw new ArgumentException($"El cafe {nombreCafe} no existe en inventario.", nameof(nombreCafe));
            }

            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(cantidad));
            }

            if (cantidad > _inventarioCafe[nombreCafe])
            {
                throw new ArgumentException("No hay suficientes cafes en el inventario.", nameof(cantidad));
            }

            _inventarioCafe[nombreCafe] -= cantidad;
        }

        public void RestarMonedas(IReadOnlyDictionary<int, int> monedasAUtilizar)
        {
            if (monedasAUtilizar == null || monedasAUtilizar.Count == 0)
            {
                return;
            }

            foreach (var moneda in monedasAUtilizar)
            {
                if (!_inventarioMonedas.TryGetValue(moneda.Key, out var disponibles) || disponibles < moneda.Value)
                {
                    throw new InvalidOperationException($"No hay suficientes monedas de {moneda.Key} colones.");
                }
            }

            foreach (var moneda in monedasAUtilizar)
            {
                _inventarioMonedas[moneda.Key] -= moneda.Value;
            }
        }
    
        public void AgregarPlata(IReadOnlyDictionary<int, int> denominaciones)
        {
            if (denominaciones == null || denominaciones.Count == 0)
            {
                return;
            }

            foreach (var denominacion in denominaciones)
            {
                if (denominacion.Value <= 0)
                {
                    continue;
                }

                if (!_inventarioMonedas.ContainsKey(denominacion.Key))
                {
                    throw new Exception($"La denominacion {denominacion.Key} no existe en el inventario.");
                }

                _inventarioMonedas[denominacion.Key] += denominacion.Value;
            }
        }
    }
}
