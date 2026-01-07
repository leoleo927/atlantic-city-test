export interface Pedido {
  id: number;
  numeroPedido: string;
  cliente: string;
  fechaCreacion: string;
  fechaModificacion?: string;
  total: number;
  estado: string;
}

export interface PedidoCreate {
  numeroPedido: string;
  cliente: string;
  total: number;
  estado: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  nombre: string;
  rol: string;
}
