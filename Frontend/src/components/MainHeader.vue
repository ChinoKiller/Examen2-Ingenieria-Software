<template>
  <header class="header-cafeteria flex justify-between items-center">
    <router-link to="/" class="header-opcion-cafeteria" active-class="header-opcion-cafeteria-activo">
        Inicio
    </router-link>

    <router-link to="/ordenar" class="header-opcion-cafeteria" active-class="header-opcion-cafeteria-activo">
        Ordenar
    </router-link> 

    <router-link to="/">
        <img src="@/assets/chino_coffee_icon.png" alt="Logo" class="h-[80px] w-[99px]" />
    </router-link>

    <router-link to="/carrito" class="header-opcion-cafeteria" active-class="header-opcion-cafeteria-activo">
        Carrito
    </router-link>

    <router-link to="/monedero" class="header-opcion-cafeteria" active-class="header-opcion-cafeteria-activo">
        Monedero
    </router-link>

</header>
</template>

<script setup>

</script>