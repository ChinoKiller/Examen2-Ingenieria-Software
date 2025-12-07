import { createRouter, createWebHistory } from 'vue-router';
import HomeView from '@/views/HomeView.vue';
import OrderView from '@/views/OrderView.vue';
import CartView from '@/views/CartView.vue';
import WalletView from '@/views/WalletView.vue';

const routes = [
  {
    path: '/',
    name: 'Inicio',
    component: HomeView
  },
  {
    path: '/ordenar',
    name: 'Ordenar',
    component: OrderView
  },
  {
    path: '/carrito',
    name: 'Carrito',
    component: CartView
  },
  {
    path: '/monedero',
    name: 'Monedero',
    component: WalletView
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
});

export default router;
