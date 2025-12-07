<template>
  <section class="w-full mx-auto px-6">
    <div class="relative rounded-[48px] p-8">
      <img
        v-for="decoracion in beanDecoraciones"
        :key="decoracion.id"
        :src="coffeeBeanImg"
        alt=""
        aria-hidden="true"
        :class="decoracion.clases"
        :style="decoracion.estilo"
      />

      <p
        v-if="estadoUi.error"
        class="mt-6 rounded-2xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
      >
        {{ estadoUi.error }}
      </p>

      <div v-else-if="estaCargando" class="mt-8 text-center text-[#fcebd8]">Cargando catálogo...</div>

      <div v-else class="mt-10 grid gap-20 grid-cols-4">
        <article
          v-for="tarjeta in tarjetasCafe"
          :key="tarjeta.id"
          class="flex h-full flex-col gap-5 rounded-[32px] bg-[#bb8a56]/90 p-5 text-[#F0E2C5] shadow-[0_20px_40px_rgba(0,0,0,0.45)]"
        >
          <div class="overflow-hidden rounded-[24px]">
            <img :src="tarjeta.imagen" :alt="tarjeta.titulo" class="h-40 w-full object-cover" />
          </div>

          <div class="space-y-1">
            <h2 class="text-2xl font-semibold">{{ tarjeta.titulo }}</h2>
            <p class="text-sm text-[#ffe4c7]">{{ formatearMoneda(tarjeta.precio) }} c/u</p>
            <p class="text-sm text-[#ffe4c7]">Disponible: {{ tarjeta.disponible }} unidades</p>
          </div>

          <div class="flex items-center justify-center gap-4">
            <button
              type="button"
              class="flex h-10 w-10 items-center justify-center rounded-full bg-[#f1c27d] text-2xl font-semibold text-[#5b3200] shadow-inner disabled:opacity-40"
              @click="disminuirCafe(tarjeta.id)"
              :disabled="tarjeta.cantidad === 0"
            >
              −
            </button>
            <span class="text-4xl font-semibold">{{ tarjeta.cantidad }}</span>
            <button
              type="button"
              class="flex h-10 w-10 items-center justify-center rounded-full bg-[#f1c27d] text-2xl font-semibold text-[#5b3200] shadow-lg disabled:opacity-40"
              @click="incrementarCafe(tarjeta.id)"
              :disabled="tarjeta.cantidad >= tarjeta.disponible"
            >
              +
            </button>
          </div>

          <div class="text-center text-sm text-[#ffe4c7]">
            Total: {{ formatearMoneda(tarjeta.precio * tarjeta.cantidad) }}
          </div>

        </article>
      </div>

      <footer class="mt-10 flex flex-wrap items-center justify-between gap-4 pt-6 text-[#F0E2C5]">
        <div>
          <p class="text-sm text-[#f9d9b7]">Total actual</p>
          <p class="text-3xl font-semibold">{{ formatearMoneda(totalOrden) }}</p>
        </div>
        <router-link
          to="/carrito"
          class="inline-flex items-center gap-2 rounded-full portada-boton z-10"
        >
          Ir al carrito
          <span aria-hidden="true">→</span>
        </router-link>
      </footer>
    </div>
  </section>
</template>

<script setup>
import { computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useCoffeeMachineStore } from '@/stores/useCoffeeMachineStore';
import americanoCard from '@/assets/americano_tarjeta_img.jpg';
import capuccinoCard from '@/assets/capuccino_tarjeta_img.jpg';
import latteCard from '@/assets/latte_tarjeta_img.webp';
import moccaCard from '@/assets/mocca_tarjeta_img.webp';
import coffeeBeanImg from '@/assets/coffee_bean.png';

const router = useRouter();
const { state, estadoUi, estaCargando, totalOrden, cargarDatos, establecerCantidadCafe } = useCoffeeMachineStore();

const detallesCafe = {
  Americano: { titulo: 'Americano', imagen: americanoCard },
  Cappuccino: { titulo: 'Capuccino', imagen: capuccinoCard },
  Lattee: { titulo: 'Latte', imagen: latteCard },
  Lates: { titulo: 'Latte', imagen: latteCard },
  Latte: { titulo: 'Latte', imagen: latteCard },
  Mocaccino: { titulo: 'Mocca', imagen: moccaCard },
  Mocca: { titulo: 'Mocca', imagen: moccaCard }
};

const beanDecoraciones = [
  { id: 'uno', clases: 'absolute w-24 -top-8 left-6 pointer-events-none', estilo: 'transform: rotate(-18deg);' },
  { id: 'dos', clases: 'absolute w-20 bottom-6 left-52 pointer-events-none', estilo: 'transform: rotate(12deg);' },
  { id: 'tres', clases: 'absolute w-16 top-4 right-56 pointer-events-none', estilo: 'transform: rotate(24deg);' },
  { id: 'cuatro', clases: 'absolute w-20 -bottom-6 right-10 pointer-events-none', estilo: 'transform: rotate(-30deg);' }
];

const tarjetasCafe = computed(() =>
  Object.keys(state.catalogo).map((clave) => {
    const detalle = detallesCafe[clave] ?? { titulo: clave, imagen: americanoCard };
    return {
      id: clave,
      titulo: detalle.titulo,
      imagen: detalle.imagen,
      precio: state.catalogo[clave],
      disponible: state.inventarioCafe?.[clave] ?? 0,
      cantidad: state.orden[clave] ?? 0
    };
  })
);

const formatearMoneda = (valor) =>
  new Intl.NumberFormat('es-CR', {
    style: 'currency',
    currency: 'CRC'
  }).format(Number(valor) || 0);

const disminuirCafe = (nombre) => {
  const actual = Number(state.orden[nombre] ?? 0);
  if (actual <= 0) {
    return;
  }
  establecerCantidadCafe(nombre, actual - 1);
};

const incrementarCafe = (nombre) => {
  const actual = Number(state.orden[nombre] ?? 0);
  const disponible = state.inventarioCafe?.[nombre] ?? 0;
  if (disponible === 0 || actual >= disponible) {
    return;
  }
  establecerCantidadCafe(nombre, actual + 1);
};

const irACarrito = () => {
  if (totalOrden.value > 0) {
    router.push('/carrito');
  }
};

onMounted(() => {
  cargarDatos();
});
</script>
