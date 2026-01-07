import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { pedidosService } from '../services/api';
import type { Pedido, PedidoCreate } from '../types/index';
import { clearAuthData, getUser } from '../utils/auth';

export default function Pedidos() {
  const [pedidos, setPedidos] = useState<Pedido[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingPedido, setEditingPedido] = useState<Pedido | null>(null);
  const [error, setError] = useState('');
  const [filtroNumero, setFiltroNumero] = useState('');
  const [filtroCliente, setFiltroCliente] = useState('');
  const [ordenFecha, setOrdenFecha] = useState<'asc' | 'desc'>('asc');
  const navigate = useNavigate();
  const user = getUser();

  const [formData, setFormData] = useState<PedidoCreate>({
    numeroPedido: '',
    cliente: '',
    total: 0,
    estado: 'Registrado',
  });

  useEffect(() => {
    loadPedidos();
  }, []);

  const loadPedidos = async () => {
    try {
      const data = await pedidosService.getAll();
      setPedidos(data);
    } catch (err) {
      setError('Error al cargar los pedidos');
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    clearAuthData();
    navigate('/');
  };

  const openCreateModal = () => {
    setEditingPedido(null);
    setFormData({
      numeroPedido: '',
      cliente: '',
      total: 0,
      estado: 'Registrado',
    });
    setError('');
    setShowModal(true);
  };

  const openEditModal = (pedido: Pedido) => {
    setEditingPedido(pedido);
    setFormData({
      numeroPedido: pedido.numeroPedido,
      cliente: pedido.cliente,
      total: pedido.total,
      estado: pedido.estado,
    });
    setError('');
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    // Validación del lado del cliente
    if (formData.total <= 0) {
      setError('El total debe ser mayor que 0');
      return;
    }

    try {
      if (editingPedido) {
        await pedidosService.update(editingPedido.id, formData);
      } else {
        await pedidosService.create(formData);
      }
      setError('');
      setShowModal(false);
      loadPedidos();
    } catch (err: any) {
      setError(err.response?.data?.message || err.response?.data?.error || 'Error al guardar el pedido');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('¿Está seguro de eliminar este pedido?')) return;

    try {
      await pedidosService.delete(id);
      loadPedidos();
    } catch (err) {
      setError('Error al eliminar el pedido');
    }
  };

  const getEstadoColor = (estado: string) => {
    switch (estado) {
      case 'Registrado':
        return 'bg-blue-100 text-blue-800';
      case 'En Proceso':
        return 'bg-yellow-100 text-yellow-800';
      case 'Completado':
        return 'bg-green-100 text-green-800';
      case 'Eliminado':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  // Filtrar y ordenar pedidos
  const pedidosFiltrados = pedidos
    .filter((pedido) => {
      const matchNumero = pedido.numeroPedido.toLowerCase().includes(filtroNumero.toLowerCase());
      const matchCliente = pedido.cliente.toLowerCase().includes(filtroCliente.toLowerCase());
      return matchNumero && matchCliente;
    })
    .sort((a, b) => {
      const dateA = new Date(a.fechaCreacion).getTime();
      const dateB = new Date(b.fechaCreacion).getTime();
      return ordenFecha === 'asc' ? dateA - dateB : dateB - dateA;
    });

  const toggleOrdenFecha = () => {
    setOrdenFecha(ordenFecha === 'asc' ? 'desc' : 'asc');
  };

  if (loading) {
    return <div className="flex justify-center items-center min-h-screen">Cargando...</div>;
  }

  return (
    <div className="min-h-screen bg-gray-100">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16 items-center">
            <h1 className="text-xl font-bold">Atlantic City</h1>
            <div className="flex items-center gap-4">
              <span className="text-sm text-gray-600">
                {user?.nombre} ({user?.rol})
              </span>
              <button
                onClick={handleLogout}
                className="bg-red-600 text-white px-4 py-2 rounded-md hover:bg-red-700"
              >
                Cerrar Sesión
              </button>
            </div>
          </div>
        </div>
      </nav>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold">Gestión de Pedidos</h2>
          <button
            onClick={openCreateModal}
            className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700"
          >
            Nuevo Pedido
          </button>
        </div>

        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {error}
          </div>
        )}

        <div className="bg-white p-4 rounded-lg shadow-sm mb-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Filtrar por Número de Pedido
              </label>
              <input
                type="text"
                value={filtroNumero}
                onChange={(e) => setFiltroNumero(e.target.value)}
                placeholder="Ej: PED-001"
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Filtrar por Cliente
              </label>
              <input
                type="text"
                value={filtroCliente}
                onChange={(e) => setFiltroCliente(e.target.value)}
                placeholder="Ej: Juan Pérez"
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
        </div>

        <div className="bg-white shadow-md rounded-lg overflow-hidden">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Número
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Cliente
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  <button
                    onClick={toggleOrdenFecha}
                    className="flex items-center gap-1 hover:text-gray-700"
                  >
                    FECHA
                    <span className="text-sm">
                      {ordenFecha === 'asc' ? '↑' : '↓'}
                    </span>
                  </button>
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Total
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Estado
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Acciones
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {pedidosFiltrados.map((pedido) => (
                <tr key={pedido.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {pedido.numeroPedido}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {pedido.cliente}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {pedido.fechaCreacion}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    ${pedido.total.toFixed(2)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className={`px-2 py-1 text-xs font-semibold rounded-full ${getEstadoColor(pedido.estado)}`}>
                      {pedido.estado}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    {pedido.estado !== 'Eliminado' ? (
                      <>
                        <button
                          onClick={() => openEditModal(pedido)}
                          className="text-blue-600 hover:text-blue-900 mr-4"
                        >
                          Editar
                        </button>
                        <button
                          onClick={() => handleDelete(pedido.id)}
                          className="text-red-600 hover:text-red-900"
                        >
                          Eliminar
                        </button>
                      </>
                    ) : (
                      <span className="text-gray-400 text-sm">-</span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {showModal && (
        <div className="fixed inset-0 bg-gray-900/70 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-8 max-w-md w-full">
            <h3 className="text-xl font-bold mb-4">
              {editingPedido ? 'Editar Pedido' : 'Nuevo Pedido'}
            </h3>

            <form onSubmit={handleSubmit} className="space-y-4">
              {editingPedido && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Número de Pedido
                  </label>
                  <input
                    type="text"
                    value={formData.numeroPedido}
                    readOnly
                    disabled
                    className="w-full px-3 py-2 border border-gray-300 rounded-md bg-gray-100 text-gray-600 cursor-not-allowed"
                  />
                  <p className="text-xs text-gray-500 mt-1">El número de pedido no se puede modificar</p>
                </div>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Cliente
                </label>
                <input
                  type="text"
                  value={formData.cliente}
                  onChange={(e) => setFormData({ ...formData, cliente: e.target.value })}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Total
                </label>
                <input
                  type="number"
                  step="0.01"
                  min="0.01"
                  value={formData.total}
                  onChange={(e) => setFormData({ ...formData, total: parseFloat(e.target.value) })}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Estado
                </label>
                <select
                  value={formData.estado}
                  onChange={(e) => setFormData({ ...formData, estado: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="Registrado">Registrado</option>
                  <option value="En Proceso">En Proceso</option>
                  <option value="Completado">Completado</option>
                </select>
              </div>

              {error && (
                <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded text-sm">
                  {error}
                </div>
              )}

              <div className="flex gap-2 pt-4">
                <button
                  type="submit"
                  className="flex-1 bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700"
                >
                  Guardar
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setShowModal(false);
                    setError('');
                  }}
                  className="flex-1 bg-gray-300 text-gray-700 py-2 px-4 rounded-md hover:bg-gray-400"
                >
                  Cancelar
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
