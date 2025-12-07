<template>
  <div class="contenedor-pagina-cafeteria">
    <MainHeader />
    <main class="contenedor-cuerpo-cafeteria">
      <router-view />
    </main>

    <div
      v-if="mostrarFueraDeServicio"
      class="fixed inset-0 z-[999] flex items-center justify-center bg-black/70 backdrop-blur-sm"
    >
      <div class="rounded-[36px] bg-gradient-to-r from-[#4b2500] via-[#7a3b00] to-[#b36300] px-16 py-10 text-center text-white shadow-[0_30px_60px_rgba(0,0,0,0.6)]">
        <p class="text-xs uppercase tracking-[0.6em] text-[#f6c88f]">Fuera de servicio</p>
        <p class="mt-3 text-3xl font-semibold tracking-[0.2em]">FUERA DE SERVICIO</p>
        <p class="mt-4 text-base text-[#f6e6d1]">La máquina no cuenta con monedas para dar cambio en este momento.</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import MainHeader from '@/components/MainHeader.vue';
import { useCoffeeMachineStore } from '@/stores/useCoffeeMachineStore';

const { state, datosCargados } = useCoffeeMachineStore();

const mostrarFueraDeServicio = computed(() => {
  if (!datosCargados.value) {
    return false;
  }
  const valores = Object.values(state.inventarioMonedas);
  if (!valores.length) {
    return false;
  }
  return valores.every((cantidad) => Number(cantidad) === 0);
});
</script>
