import axios from 'axios';

const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:5050/api';

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' },
});

// Auth token (bearer token from POST /auth/login)
const TOKEN_KEY = 'trucklogix.token';
export const getToken = () => localStorage.getItem(TOKEN_KEY);
export const setToken = (token) => localStorage.setItem(TOKEN_KEY, token);
export const clearToken = () => localStorage.removeItem(TOKEN_KEY);

api.interceptors.request.use(config => {
  const token = getToken();
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// An expired or invalid token sends the user back to the login screen
api.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401 && !error.config.url.endsWith('/auth/login') && getToken()) {
      clearToken();
      window.location.assign('/');
    }
    return Promise.reject(error);
  }
);

// Readable message from an API error (ProblemDetails "detail", else a fallback)
export const errorMessage = (err, fallback) => err.response?.data?.detail || err.response?.data?.title || fallback;

// Auth
export const authApi = {
  login: async (email, password) => {
    const r = await api.post('/auth/login', { email, password });
    setToken(r.data.accessToken);
  },
  me: () => api.get('/auth/me'),
  changePassword: (currentPassword, newPassword) => api.post('/auth/change-password', { currentPassword, newPassword }),
};

// Users (admin only)
export const usersApi = {
  getAll: () => api.get('/users'),
  create: (data) => api.post('/users', data),
  resetPassword: (id, newPassword) => api.post(`/users/${id}/reset-password`, { newPassword }),
  delete: (id) => api.delete(`/users/${id}`),
};

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
