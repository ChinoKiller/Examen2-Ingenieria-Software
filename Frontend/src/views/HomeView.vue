<template>
  <section class="portada-cafeteria">
    <div class="portada-panel">
      <img
        v-for="bean in beanDecoraciones"
        :key="bean"
        :src="coffeeBeanImg"
        alt=""
        aria-hidden="true"
        class="portada-grano"
        :class="`portada-grano--${bean}`"
      />

      <div class="portada-imagen">
        <img :src="fondoCafe" alt="Taza de café" class="portada-figura" />
      </div>

      <div class="portada-contenido space-y-20">

        <h1 class="portada-titulo">
          CAFECITO <span class="portada-titulo-secundario">para</span>
          <span class="portada-titulo-marcado">PROGRAMADORES</span>
        </h1>
        

        <div class="portada-mensajes">
          <p v-if="estadoUi.error" class="portada-alerta portada-alerta-error">
            {{ estadoUi.error }}
          </p>
          <p v-else-if="estaCargando" class="portada-alerta">Cargando catálogo...</p>


            <ul v-else-if="catalogoDestacado.length" class="grid grid-cols-2 gap-20">

                <div
                    v-for="item in catalogoDestacado"
                    :key="item.nombre"
                    class="relative w-[300px] h-[141px]"
                >

                    <img
                    :src="item.imagen"
                    :alt="`Taza de ${item.nombre}`"
                    class="portada-item-imagen absolute -top-3 left-[-60px] z-20"
                    />

                    <div
                    class="bg-[#B0814F] pl-[80px] pr-[46px] py-[21px] 
                            rounded-r-full w-full text-[36px] text-[#F0E2C5] flex items-center z-10"
                    >
                    {{ item.nombre }}
                    </div>
                </div>

            </ul>

          <p v-else class="portada-alerta">Todavía no hay cafés para mostrar.</p>
        </div>

        <div style="display:flex; justify-content:flex-end;">
          <router-link to="/ordenar" class="portada-boton">Ordenar</router-link>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed, onMounted } from 'vue';
import { useCoffeeMachineStore } from '@/stores/useCoffeeMachineStore';
import fondoCafe from '@/assets/background_img.png';
import americanoImg from '@/assets/expresso_item.png';
import capuccinoImg from '@/assets/capuccino_item.png';
import latteImg from '@/assets/latte_item.png';
import moccaImg from '@/assets/mocca_item.png';
import coffeeBeanImg from '@/assets/coffee_bean.png';

const { nombresCafe, estadoUi, estaCargando, cargarDatos } = useCoffeeMachineStore();

const beanDecoraciones = ['uno', 'dos', 'tres', 'cuatro'];

const imagenesPorCafe = {
  Americano: americanoImg,
  Cappuccino: capuccinoImg,
  Latte: latteImg,
  Mocca: moccaImg,
};

const imagenesFallback = [americanoImg, capuccinoImg, latteImg, moccaImg];

const catalogoDestacado = computed(() =>
  nombresCafe.value.map((nombre, index) => ({
    nombre,
    imagen: imagenesPorCafe[nombre] ?? imagenesFallback[index % imagenesFallback.length]
  }))
);

onMounted(() => {
  cargarDatos();
});
</script>
