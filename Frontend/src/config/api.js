const API_BASE_URL = process.env.VUE_APP_API_BASE_URL || 'http://localhost:5059/api/MaquinaCafeControlador';

export const apiConfig = {
  baseURL: API_BASE_URL,
  endpoints: {
    cafes: `${API_BASE_URL}/cafes`,
    precios: `${API_BASE_URL}/precios`,
    monedas: `${API_BASE_URL}/monedas`,
    comprar: `${API_BASE_URL}/comprar`
  }
};

export default apiConfig;
