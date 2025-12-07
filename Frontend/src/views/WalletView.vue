<template>
  <section class="w-full max-w-6xl mx-auto px-6 py-12">
    <div class="relative overflow-hidden rounded-[48px] bg-gradient-to-br from-[#2c1200] to-[#0b0400] p-10 text-[#F0E2C5] shadow-[0_25px_60px_rgba(0,0,0,0.75)]">
      <img
        v-for="decoracion in beanDecoraciones"
        :key="decoracion.id"
        :src="coffeeBeanImg"
        alt=""
        aria-hidden="true"
        :class="decoracion.clases"
        :style="decoracion.estilo"
      />

      <header class="space-y-2">
        <p class="text-sm uppercase tracking-[0.4em] text-[#ffba63]">Monedero</p>
        <h1 class="text-4xl font-semibold">Inventario de monedas</h1>
        <p class="text-[#f8ddbc]">Consulta cuántas monedas tiene disponible la máquina en cada denominación.</p>
      </header>

      <p
        v-if="estadoUi.error"
        class="mt-6 rounded-2xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
      >
        {{ estadoUi.error }}
      </p>

      <div v-else-if="estaCargando" class="mt-10 text-center text-[#fcebd8]">
        Cargando inventario...
      </div>

      <div v-else class="mt-12">
        <div v-if="tarjetasMonedas.length" class="flex flex-wrap items-end justify-between gap-8">
          <div
            v-for="tarjeta in tarjetasMonedas"
            :key="tarjeta.denominacion"
            class="flex min-w-[140px] flex-1 flex-col items-center gap-4 text-center"
          >
            <div
              :class="['flex items-center justify-center shadow-[0_10px_25px_rgba(0,0,0,0.45)]', tarjeta.forma]"
              :style="tarjeta.estilo"
            >
              <span :class="['text-2xl font-semibold', tarjeta.textoClase]">{{ tarjeta.etiqueta }}</span>
            </div>
            <p class="text-4xl font-semibold text-white">{{ tarjeta.cantidad }}</p>
            <p class="text-lg uppercase tracking-[0.4em] text-[#f8dfc0]">Unidades</p>
          </div>
        </div>
        <p v-else class="text-center text-sm text-[#fcebd8]">No hay monedas registradas en el sistema.</p>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed, onMounted } from 'vue';
import { useCoffeeMachineStore } from '@/stores/useCoffeeMachineStore';
import coffeeBeanImg from '@/assets/coffee_bean.png';

const { state, estadoUi, estaCargando, cargarDatos } = useCoffeeMachineStore();

const beanDecoraciones = [
  { id: 'uno', clases: 'absolute w-20 -top-6 left-10 pointer-events-none', estilo: 'transform: rotate(-18deg);' },
  { id: 'dos', clases: 'absolute w-16 top-24 right-16 pointer-events-none', estilo: 'transform: rotate(24deg);' },
  { id: 'tres', clases: 'absolute w-14 bottom-8 left-1/3 pointer-events-none', estilo: 'transform: rotate(-30deg);' }
];

const tarjetasMonedas = computed(() =>
  Object.entries(state.inventarioMonedas)
    .sort((a, b) => Number(b[0]) - Number(a[0]))
    .map(([denominacion, cantidad]) => {
      const config = estilosMoneda(Number(denominacion));
      return {
        denominacion: Number(denominacion),
        cantidad,
        etiqueta: `₡ ${Number(denominacion).toLocaleString('es-HN')}`,
        ...config
      };
    })
);

const estilosMoneda = (valor) => {
  if (valor >= 1000) {
    return {
      forma: 'h-20 w-40 rounded-[18px]',
      estilo: 'background:#ffb7b7;',
      textoClase: 'text-[#7a1d1d]'
    };
  }
  return {
    forma: 'h-24 w-24 rounded-full',
    estilo: 'background:#f6a531;',
    textoClase: 'text-[#4b1f00]'
  };
};

onMounted(() => {
  cargarDatos();
});
</script>
