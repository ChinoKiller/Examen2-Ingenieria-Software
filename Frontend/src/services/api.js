import { apiConfig } from '@/config/api';

async function request(url, options = {}) {
  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers ?? {})
    },
    ...options
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || 'Error desconocido en la API');
  }

  return response.json();
}

export function obtenerCatalogo() {
  return request(apiConfig.endpoints.cafes);
}

export function obtenerPrecios() {
  return request(apiConfig.endpoints.precios);
}

export function obtenerMonedas() {
  return request(apiConfig.endpoints.monedas);
}

export function enviarOrden(payload) {
  return request(apiConfig.endpoints.comprar, {
    method: 'POST',
    body: JSON.stringify(payload)
  });
}
