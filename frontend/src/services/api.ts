import axios from 'axios';
import type { LoginRequest, LoginResponse, Pedido, PedidoCreate } from '../types/index';

const API_URL = 'http://localhost:5145';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>('/api/auth/login', credentials);
    return response.data;
  },
};

export const pedidosService = {
  getAll: async (): Promise<Pedido[]> => {
    const response = await api.get<Pedido[]>('/api/pedidos');
    return response.data;
  },

  getById: async (id: number): Promise<Pedido> => {
    const response = await api.get<Pedido>(`/api/pedidos/${id}`);
    return response.data;
  },

  create: async (pedido: PedidoCreate): Promise<Pedido> => {
    const response = await api.post<Pedido>('/api/pedidos', pedido);
    return response.data;
  },

  update: async (id: number, pedido: PedidoCreate): Promise<Pedido> => {
    const response = await api.put<Pedido>(`/api/pedidos/${id}`, pedido);
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await api.delete(`/api/pedidos/${id}`);
  },
};
