import { reactive, ref, computed, watch } from 'vue';
import { obtenerCatalogo, obtenerPrecios, obtenerMonedas, enviarOrden } from '@/services/api';

const state = reactive({
  catalogo: {},
  inventarioCafe: {},
  inventarioMonedas: {},
  orden: {},
  pagoMonedas: {},
  ultimoResultado: null
});

const estadoUi = reactive({
  mensaje: '',
  error: ''
});

const estaCargando = ref(false);
const estaEnviando = ref(false);
const datosCargados = ref(false);
let mensajeTimeout = null;
let errorTimeout = null;

watch(
  () => estadoUi.mensaje,
  (nuevo) => {
    if (mensajeTimeout) {
      clearTimeout(mensajeTimeout);
      mensajeTimeout = null;
    }
    if (nuevo) {
      mensajeTimeout = setTimeout(() => {
        estadoUi.mensaje = '';
        mensajeTimeout = null;
      }, 5000);
    }
  }
);

watch(
  () => estadoUi.error,
  (nuevo) => {
    if (errorTimeout) {
      clearTimeout(errorTimeout);
      errorTimeout = null;
    }
    if (nuevo) {
      errorTimeout = setTimeout(() => {
        estadoUi.error = '';
        errorTimeout = null;
      }, 5000);
    }
  }
);

function limpiarDiccionario(diccionario) {
  return Object.fromEntries(
    Object.entries(diccionario)
      .filter(([, valor]) => Number(valor) > 0)
      .map(([clave, valor]) => [clave, Number(valor)])
  );
}

function obtenerTotalOrden() {
  return Object.entries(state.orden).reduce((total, [nombre, cantidad]) => {
    if (!state.catalogo[nombre]) {
      return total;
    }
    return total + state.catalogo[nombre] * Number(cantidad ?? 0);
  }, 0);
}

function obtenerMontoPago() {
  return Object.entries(state.pagoMonedas).reduce((total, [denominacion, cantidad]) => {
    return total + Number(denominacion) * Number(cantidad ?? 0);
  }, 0);
}

export function useCoffeeMachineStore() {
  async function cargarDatos(force = false) {
    if (datosCargados.value && !force) {
      return;
    }

    try {
      estaCargando.value = true;
      estadoUi.error = '';
      const [inventario, precios, monedas] = await Promise.all([
        obtenerCatalogo(),
        obtenerPrecios(),
        obtenerMonedas()
      ]);

      Object.keys(state.catalogo).forEach((clave) => delete state.catalogo[clave]);
      Object.entries(precios).forEach(([clave, valor]) => {
        state.catalogo[clave] = valor;
        if (!(clave in state.orden)) {
          state.orden[clave] = 0;
        }
      });

      Object.keys(state.inventarioCafe).forEach((clave) => delete state.inventarioCafe[clave]);
      Object.entries(inventario).forEach(([clave, valor]) => {
        state.inventarioCafe[clave] = valor;
        if (!(clave in state.orden)) {
          state.orden[clave] = 0;
        }
      });

      Object.keys(state.inventarioMonedas).forEach((clave) => delete state.inventarioMonedas[clave]);
      Object.entries(monedas)
        .sort((a, b) => Number(b[0]) - Number(a[0]))
        .forEach(([clave, valor]) => {
          state.inventarioMonedas[clave] = valor;
          if (!(clave in state.pagoMonedas)) {
            state.pagoMonedas[clave] = 0;
          }
        });

      datosCargados.value = true;
    } catch (error) {
      estadoUi.error = error.message ?? 'No se pudieron cargar los datos.';
    } finally {
      estaCargando.value = false;
    }
  }

  function establecerCantidadCafe(nombre, cantidad) {
    if (!(nombre in state.catalogo)) {
      return;
    }
    state.orden[nombre] = Math.max(0, Number(cantidad) || 0);
  }

  function establecerMonedas(denominacion, cantidad) {
    if (!(denominacion in state.inventarioMonedas)) {
      return;
    }
    state.pagoMonedas[denominacion] = Math.max(0, Number(cantidad) || 0);
  }

  async function procesarCompra() {
    try {
      estaEnviando.value = true;
      estadoUi.error = '';
      estadoUi.mensaje = '';
      state.ultimoResultado = null;

      const payload = {
        orden: limpiarDiccionario(state.orden),
        pago: {
          montoTotal: obtenerMontoPago(),
          monedas: limpiarDiccionario(state.pagoMonedas)
        }
      };

      const respuesta = await enviarOrden(payload);
      estadoUi.mensaje = respuesta.mensaje ?? 'Compra realizada correctamente.';
      state.ultimoResultado = respuesta;
      await cargarDatos(true);
      limpiarOrdenYPago();
    } catch (error) {
      estadoUi.error = error.message ?? 'No se pudo procesar la compra.';
    } finally {
      estaEnviando.value = false;
    }
  }

  function limpiarOrdenYPago() {
    Object.keys(state.orden).forEach((clave) => {
      state.orden[clave] = 0;
    });
    Object.keys(state.pagoMonedas).forEach((clave) => {
      state.pagoMonedas[clave] = 0;
    });
  }

  const nombresCafe = computed(() => Object.keys(state.catalogo));

  const resumenOrden = computed(() =>
    Object.entries(state.orden)
      .filter(([, cantidad]) => Number(cantidad) > 0)
      .map(([nombre, cantidad]) => ({
        nombre,
        cantidad: Number(cantidad),
        subtotal: state.catalogo[nombre] * Number(cantidad)
      }))
  );

  const totalOrden = computed(() => obtenerTotalOrden());
  const totalPago = computed(() => obtenerMontoPago());

  return {
    state,
    estadoUi,
    datosCargados,
    estaCargando,
    estaEnviando,
    nombresCafe,
    resumenOrden,
    totalOrden,
    totalPago,
    cargarDatos,
    establecerCantidadCafe,
    establecerMonedas,
    procesarCompra
  };
}
