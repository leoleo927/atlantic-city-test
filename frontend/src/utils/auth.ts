import type { LoginResponse } from '../types/index';

export const saveAuthData = (data: LoginResponse): void => {
  localStorage.setItem('token', data.token);
  localStorage.setItem('user', JSON.stringify({
    email: data.email,
    nombre: data.nombre,
    rol: data.rol,
  }));
};

export const getToken = (): string | null => {
  return localStorage.getItem('token');
};

export const getUser = (): { email: string; nombre: string; rol: string } | null => {
  const user = localStorage.getItem('user');
  return user ? JSON.parse(user) : null;
};

export const clearAuthData = (): void => {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
};

export const isAuthenticated = (): boolean => {
  return !!getToken();
};
