using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamTwo.Controllers
{
    // TODO: Falta atributo [ApiController] y una ruta base clara; la convención actual viola estándares REST básicos.
    public class CoffeeMachineController : Controller
    {

        private readonly Database _db;

        public CoffeeMachineController(Database db)
        {
            _db = db;
        }

        [HttpGet("getCoffees")]
        public ActionResult<Dictionary<string, int>> GetCoffeePrices()
        {
            // TODO: Exponer directamente los diccionarios del "Database" rompe el encapsulamiento y acopla al cliente a la estructura interna.
            return Ok(_db.keyValues);
        }

        [HttpGet("getCoffeePricesInCents")]
        public ActionResult<Dictionary<string, int>> GetCoffeePricesInCents()
        {
            // TODO: Los nombres keyValues/keyValues2 no son expresivos; Clean Code exige nombres con intención.
            return Ok(_db.keyValues2);
        }

        [HttpGet("getQuantity")]
        public ActionResult<Dictionary<string, int>> GetQuantity()
        {
            // TODO: La API mezcla distintos conceptos (precios y cantidades) en un solo controlador sin separaciones claras de responsabilidad.
            return Ok(_db.keyValues3);
        }

        [HttpPost("buyCoffee")]
        public ActionResult<string> BuyCoffee([FromBody] OrderRequest request)
        {
            // TODO: El controlador implementa toda la lógica de negocio y reglas de la máquina; viola SRP y SOLID (falta de servicios/domains separados).
            if (request.Order == null || request.Order.Count == 0)
                return BadRequest("Ordem vacia.");

            if (request.Payment.TotalAmount <= 0)
                return BadRequest("Dinero insuficiente ");

            try
            {
                // TODO: Se asume que todas las claves existen; falta manejo de errores y domain rules centralizadas.
                var costoTotal = request.Order.Sum(o => _db.keyValues2.First(c => c.Key == o.Key).Value * o.Value);

                if (request.Payment.TotalAmount < costoTotal)
                { 
                    return BadRequest("Dinero insuficiente ");
                }


                foreach (var cafe in request.Order)
                {
                    var selected = _db.keyValues.First(c => c.Key == cafe.Key).Key;
                    // TODO: Buscar elementos con First en cada iteración degrada rendimiento y duplica lógica de acceso a datos.
                    if (cafe.Value > _db.keyValues[selected])
                    {
                        return $"No hay suficientes {selected} en la máquina.";
                    }
                    _db.keyValues[selected] -= cafe.Value;
                }

                var change = request.Payment.TotalAmount - costoTotal;
                String result = $"Su vuelto es de: {change} colones. Desglose:";

                // TODO: Calcular el vuelto en el controlador en lugar de un servicio reutilizable dificulta pruebas unitarias.
                foreach (var coin in _db.keyValues3.Keys.OrderByDescending(c => c))
                {
                    var count = Math.Min(change / coin, _db.keyValues3[coin]);
                    if (count > 0)
                    {
                        result +=  $" {count} moneda de {coin},  ";              
                        change -= coin * count;
                    }
                }


                if (change > 0)
                {
                    return StatusCode(500, "No hay suficiente cambio en la máquina.");
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    // TODO: Las clases DTO deberían vivir en archivos separados para respetar SRP y facilitar pruebas/reutilización.
    public class OrderRequest
    {
        public Dictionary<string, int> Order { get; set; }
        public Payment Payment { get; set; }
    }

    public class Payment
    {
        public int TotalAmount { get; set; }
        // TODO: Estas colecciones nunca se usan; hay datos muertos que confunden a otros desarrolladores.
        public List<int> Coins { get; set; }
        public List<int> Bills { get; set; }
    }
}
