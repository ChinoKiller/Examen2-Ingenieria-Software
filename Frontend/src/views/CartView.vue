<template>
  <section class="w-full px-6">
    <div
      class="relative mx-auto rounded-[48px] p-10 text-[#F0E2C5] "
    >
      <img
        v-for="decoracion in beanDecoraciones"
        :key="decoracion.id"
        :src="coffeeBeanImg"
        alt=""
        aria-hidden="true"
        :class="decoracion.clases"
        :style="decoracion.estilo"
      />

      <article
        v-if="estaCargando"
        class="mx-auto w-full max-w-2xl rounded-[40px] bg-[#f8e5c8] p-10 text-center text-[#3d1a00] shadow-[0_20px_45px_rgba(0,0,0,0.45)]"
      >
        Cargando carrito...
      </article>

      <article
        v-else
        class="relative mx-auto w-full max-w-2xl rounded-[40px] bg-[#f8e5c8] p-10 text-[#3d1a00] shadow-[0_20px_45px_rgba(0,0,0,0.45)]"
      >
        <template v-if="pasoActivo === 'resumen'">
          <header class="text-center space-y-2">
            <p class="text-sm uppercase tracking-[0.4em] text-[#b27b36]">Carrito</p>
            <h1 class="text-3xl font-semibold text-[#4b1f00]">Factura pendiente</h1>
            <p class="text-sm text-[#8a6230]">Revisa tu orden antes de continuar.</p>
          </header>

          <div class="my-6 h-px bg-[#cfa46f]"></div>

          <ul v-if="tieneOrden" class="space-y-4">
            <li
              v-for="item in resumenOrden"
              :key="item.nombre"
              class="flex items-center justify-between text-lg font-semibold"
            >
              <span>{{ item.cantidad }}x {{ item.nombre }}</span>
              <span>{{ formatearMoneda(item.subtotal) }}</span>
            </li>
          </ul>
          <p v-else class="text-center text-lg leading-relaxed text-[#7a4a1c]">
            Aún no agregas productos a tu carrito. Compra café, mucho café, hoy toca debuggear...
          </p>

          <footer
            v-if="tieneOrden"
            class="mt-10 flex flex-col gap-6 sm:flex-row sm:items-center sm:justify-between"
          >
            <div>
              <p class="text-sm text-[#a07032]">Total</p>
              <p class="text-4xl font-semibold text-[#4b1f00]">{{ formatearMoneda(totalOrden) }}</p>
            </div>
            <div class="flex flex-1 justify-end gap-4">
              <button
                type="button"
                class="h-12 rounded-full bg-[#5a3610] px-8 text-lg font-semibold text-white shadow-inner transition hover:brightness-110"
                @click="cancelarOrden"
              >
                Cancelar
              </button>
              <button
                type="button"
                class="portada-boton disabled:opacity-40"
                :disabled="!tieneOrden"
                @click="irAPago"
              >
                Pagar
              </button>
            </div>
          </footer>
        </template>

        <template v-else-if="pasoActivo === 'pago'">
          <header class="text-center space-y-2">
            <p class="text-sm uppercase tracking-[0.4em] text-[#b27b36]">Ingresar pago</p>
            <h1 class="text-3xl font-semibold text-[#4b1f00]">Ingresa tu pago</h1>
            <p class="text-sm text-[#8a6230]">Registra cuántas monedas entregarás.</p>
          </header>

          <div class="my-6 h-px bg-[#cfa46f]"></div>

          <div class="space-y-4">
            <div
              v-for="moneda in monedas"
              :key="moneda.denominacion"
              class="flex flex-col gap-3 rounded-[28px] bg-[#fdf4e6] px-5 py-4 shadow-inner sm:flex-row sm:items-center sm:justify-between"
            >
              <div class="flex flex-1 items-center gap-4">
                <span class="inline-flex h-10 min-w-[90px] items-center justify-center rounded-full bg-[#f0a637] text-base font-semibold text-white">
                  {{ formatearMoneda(moneda.denominacion) }}
                </span>
              </div>
              <input
                type="number"
                min="0"
                step="1"
                :value="state.pagoMonedas[moneda.denominacion]"
                :disabled="limitePagoAlcanzado"
                @input="establecerMonedas(moneda.denominacion, $event.target.value)"
                class="w-full rounded-2xl border border-[#caa06b] px-4 py-2 text-right text-lg font-semibold text-[#4b1f00] outline-none focus:ring-2 focus:ring-[#f7b34c] disabled:cursor-not-allowed disabled:bg-[#e8d5bb] disabled:text-[#8a6230] sm:w-28"
              />
            </div>
          </div>

          <div class="mt-8 flex flex-col gap-6 text-center text-[#4b1f00]">
            <div class="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <p class="text-sm text-[#a07032]">Total a pagar</p>
                <p class="text-3xl font-semibold">{{ formatearMoneda(totalOrden) }}</p>
              </div>
              <div>
                <p class="text-sm text-[#a07032]">Pago ingresado</p>
                <p
                  class="text-3xl font-semibold"
                  :class="totalPago >= totalOrden ? 'text-[#1f8a5f]' : 'text-[#b02929]'"
                >
                  {{ formatearMoneda(totalPago) }}
                </p>
              </div>
            </div>
            <p
              v-if="limitePagoAlcanzado"
              class="text-sm text-[#1f8a5f]"
            >
              Ya cubriste el total. Ajusta cantidades si deseas usar otras monedas.
            </p>
            <button
              type="button"
              class="h-12 rounded-full bg-gradient-to-b from-[#f7b34c] to-[#d9830f] text-lg font-semibold text-white shadow-[0_6px_18px_rgba(217,131,15,0.5)] transition hover:translate-y-0.5 disabled:opacity-40"
              :disabled="deshabilitarProcesar"
              @click="procesarPago"
            >
              {{ estaEnviando ? 'Procesando...' : 'Procesar' }}
            </button>
          </div>
        </template>

        <template v-else>
          <header class="text-center space-y-2">
            <p class="text-sm uppercase tracking-[0.4em] text-[#b27b36]">Cambio</p>
            <h1 class="text-3xl font-semibold text-[#4b1f00]">Su vuelto</h1>
            <p class="text-sm text-[#8a6230]">Estas son las monedas devueltas por la máquina.</p>
          </header>

          <div class="my-6 h-px bg-[#cfa46f]"></div>

          <div v-if="estadoVuelto.length" class="space-y-4">
            <div
              v-for="moneda in estadoVuelto"
              :key="moneda.denominacion"
              class="flex items-center justify-between rounded-[28px] bg-[#fdf4e6] px-5 py-4 shadow-inner"
            >
              <span class="inline-flex h-10 min-w-[90px] items-center justify-center rounded-full bg-[#f0a637] text-base font-semibold text-white">
                {{ formatearMoneda(moneda.denominacion) }}
              </span>
              <span class="text-lg font-semibold text-[#5a3610]">Cantidad: {{ moneda.cantidad }}</span>
            </div>
          </div>
          <p v-else class="text-center text-lg text-[#7a4a1c]">No hubo monedas de cambio.</p>

          <div class="mt-10 flex flex-col items-center gap-4 text-center">
            <div>
              <p class="text-sm text-[#a07032]">Total devuelto</p>
              <p class="text-4xl font-semibold text-[#4b1f00]">{{ formatearMoneda(montoVuelto) }}</p>
            </div>
            <button
              type="button"
              class="h-12 w-full max-w-xs rounded-full bg-gradient-to-b from-[#f7b34c] to-[#d9830f] text-lg font-semibold text-white shadow-[0_6px_18px_rgba(217,131,15,0.5)] transition hover:translate-y-0.5"
              @click="finalizar"
            >
              Finalizar
            </button>
          </div>
        </template>
      </article>

      <div class="mt-6 space-y-3">
        <p v-if="estadoUi.error" class="rounded-2xl border border-red-200 bg-red-50/90 px-4 py-2 text-sm text-red-800">
          {{ estadoUi.error }}
        </p>
        <p v-else-if="estadoUi.mensaje" class="rounded-2xl border border-emerald-200 bg-emerald-50/90 px-4 py-2 text-sm text-emerald-800">
          {{ estadoUi.mensaje }}
        </p>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useCoffeeMachineStore } from '@/stores/useCoffeeMachineStore';
import coffeeBeanImg from '@/assets/coffee_bean.png';

const router = useRouter();

const {
  state,
  estadoUi,
  resumenOrden,
  totalOrden,
  totalPago,
  establecerMonedas,
  procesarCompra,
  cargarDatos,
  estaEnviando,
  estaCargando
} = useCoffeeMachineStore();

const pasoActivo = ref('resumen');

const beanDecoraciones = [
  { id: 'uno', clases: 'pointer-events-none absolute w-24 -top-6 left-12', estilo: 'transform: rotate(-18deg);' },
  { id: 'dos', clases: 'pointer-events-none absolute w-16 top-24 right-20', estilo: 'transform: rotate(24deg);' },
  { id: 'tres', clases: 'pointer-events-none absolute w-14 bottom-10 left-16', estilo: 'transform: rotate(-12deg);' },
  { id: 'cuatro', clases: 'pointer-events-none absolute w-20 -bottom-8 right-10', estilo: 'transform: rotate(-30deg);' }
];

const monedas = computed(() =>
  Object.entries(state.inventarioMonedas)
    .sort((a, b) => Number(b[0]) - Number(a[0]))
    .map(([denominacion, cantidadSistema]) => ({ denominacion: Number(denominacion), cantidadSistema }))
);

const estadoVuelto = computed(() =>
  state.ultimoResultado?.detalleVuelto
    ? Object.entries(state.ultimoResultado.detalleVuelto).map(([denominacion, cantidad]) => ({
        denominacion: Number(denominacion),
        cantidad
      }))
    : []
);

const montoVuelto = computed(() => Number(state.ultimoResultado?.montoVuelto ?? 0));
const tieneOrden = computed(() => resumenOrden.value.length > 0);
const deshabilitarProcesar = computed(() => totalOrden.value === 0 || estaEnviando.value);
const limitePagoAlcanzado = computed(() => totalOrden.value > 0 && totalPago.value >= totalOrden.value);

const formatearMoneda = (valor) =>
  new Intl.NumberFormat('es-CR', {
    style: 'currency',
    currency: 'CRC'
  }).format(Number(valor) || 0);

const irAPago = () => {
  if (!tieneOrden.value) {
    return;
  }
  estadoUi.error = '';
  estadoUi.mensaje = '';
  pasoActivo.value = 'pago';
};

const cancelarOrden = () => {
  Object.keys(state.orden).forEach((clave) => {
    state.orden[clave] = 0;
  });
  Object.keys(state.pagoMonedas).forEach((clave) => {
    state.pagoMonedas[clave] = 0;
  });
  state.ultimoResultado = null;
  estadoUi.mensaje = '';
  estadoUi.error = '';
  pasoActivo.value = 'resumen';
};

const procesarPago = async () => {
  if (deshabilitarProcesar.value) {
    return;
  }
  await procesarCompra();
  if (!estadoUi.error && state.ultimoResultado) {
    pasoActivo.value = 'vuelto';
  }
};

const finalizar = () => {
  cancelarOrden();
  router.push('/ordenar');
};

watch(
  () => state.ultimoResultado,
  (nuevo) => {
    if (nuevo) {
      pasoActivo.value = 'vuelto';
    }
  }
);

watch(
  () => resumenOrden.value.length,
  (cantidad) => {
    if (cantidad === 0 && pasoActivo.value === 'pago' && !state.ultimoResultado) {
      pasoActivo.value = 'resumen';
    }
  }
);

onMounted(() => {
  cargarDatos();
});
</script>
