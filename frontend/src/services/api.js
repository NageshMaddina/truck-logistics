import axios from 'axios';

const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:5050/api';

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' },
});

// Loads
export const loadsApi = {
  getAll: (params) => api.get('/loads', { params }),
  getById: (id) => api.get(`/loads/${id}`),
  create: (data) => api.post('/loads', data),
  update: (id, data) => api.put(`/loads/${id}`, data),
  delete: (id) => api.delete(`/loads/${id}`),
  addTracking: (id, data) => api.post(`/loads/${id}/tracking`, data),
  getStats: () => api.get('/loads/stats'),
};

// Carriers
export const carriersApi = {
  getAll: (params) => api.get('/carriers', { params }),
  getById: (id) => api.get(`/carriers/${id}`),
  create: (data) => api.post('/carriers', data),
  update: (id, data) => api.put(`/carriers/${id}`, data),
  delete: (id) => api.delete(`/carriers/${id}`),
};

// Drivers
export const driversApi = {
  getAll: (params) => api.get('/drivers', { params }),
  getById: (id) => api.get(`/drivers/${id}`),
  create: (data) => api.post('/drivers', data),
  update: (id, data) => api.put(`/drivers/${id}`, data),
  setAvailability: (id, available) => api.patch(`/drivers/${id}/availability`, available),
};

export default api;
