using System;
using System.Collections.Generic;
using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaquinaCafeControlador : ControllerBase
    {
        private readonly IMaquinaCafeServicio _maquinaCafeServicio;

        public MaquinaCafeControlador(IMaquinaCafeServicio maquinaCafeServicio)
        {
            _maquinaCafeServicio = maquinaCafeServicio;
        }

        [HttpGet("cafes")]
        public ActionResult<IReadOnlyDictionary<string, int>> ObtenerInventarioCafe()
        {
            var cafes = _maquinaCafeServicio.ObtenerInventarioCafe();
            return Ok(cafes);
        }

        [HttpGet("precios")]
        public ActionResult<IReadOnlyDictionary<string, int>> ObtenerPreciosCafe()
        {
            var precios = _maquinaCafeServicio.ObtenerPreciosCafe();
            return Ok(precios);
        }

        [HttpGet("monedas")]
        public ActionResult<IReadOnlyDictionary<int, int>> ObtenerInventarioMonedas()
        {
            var monedas = _maquinaCafeServicio.ObtenerInventarioMonedas();
            return Ok(monedas);
        }

        [HttpPost("comprar")]
        public ActionResult<RespuestaCompraDto> ComprarCafe([FromBody] SolicitudCompraCafeDto solicitud)
        {
            try
            {
                var respuesta = _maquinaCafeServicio.ProcesarOrden(solicitud);
                return Ok(respuesta);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
