import type { BeachCruiser, MountainBike, Accessory, RentResponse, OrderResponse } from '../types';

const API_BASE = import.meta.env.VITE_API_URL || 'https://localhost:5001/api';

async function fetchJson<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(url, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });
  if (!res.ok) {
    const error = await res.json().catch(() => ({ error: res.statusText }));
    throw new Error(error.message || error.error || `HTTP ${res.status}`);
  }
  return res.json();
}

export const bikeApi = {
  getBeachCruisers: () => fetchJson<BeachCruiser[]>(`${API_BASE}/bikes/beach`),

  getMountainBikes: () => fetchJson<MountainBike[]>(`${API_BASE}/bikes/mountain`),

  rentBike: (bikeType: string, bikeId: number) =>
    fetchJson<RentResponse>(`${API_BASE}/bikes/rent`, {
      method: 'POST',
      body: JSON.stringify({ bikeType, bikeId }),
    }),

  resetFleet: () =>
    fetchJson<RentResponse>(`${API_BASE}/bikes/reset`, { method: 'POST' }),
};

export const accessoryApi = {
  getAccessories: (bikeType?: string) => {
    const params = bikeType ? `?bikeType=${bikeType}` : '';
    return fetchJson<Accessory[]>(`${API_BASE}/accessories${params}`);
  },

  placeOrder: (items: { accessoryId: number; quantity: number }[]) =>
    fetchJson<OrderResponse>(`${API_BASE}/accessories/order`, {
      method: 'POST',
      body: JSON.stringify(items),
    }),
};
