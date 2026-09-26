import React, { useCallback, useEffect, useState } from 'react';
import { BrowserRouter, Routes, Route, NavLink, Navigate } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import Loads from './pages/Loads';
import LoadDetail from './pages/LoadDetail';
import Carriers from './pages/Carriers';
import Drivers from './pages/Drivers';
import Users from './pages/Users';
import Login from './pages/Login';
import ChangePasswordModal from './components/ChangePasswordModal';
import { authApi, getToken, clearToken } from './services/api';
import './App.css';

function App() {
  // undefined = still checking the saved token, null = signed out
  const [user, setUser] = useState(undefined);
  const [showChangePassword, setShowChangePassword] = useState(false);

  const loadUser = useCallback(() => {
    if (!getToken()) { setUser(null); return; }
    authApi.me()
      .then(r => setUser(r.data))
      .catch(() => { clearToken(); setUser(null); });
  }, []);

  useEffect(() => { loadUser(); }, [loadUser]);

  const handleSignOut = () => {
    clearToken();
    setUser(null);
  };

  if (user === undefined) return <div className="loading">Loading...</div>;
  if (user === null) return <Login onLoggedIn={loadUser} />;

  return (
    <BrowserRouter>
      <div className="app">
        <nav className="sidebar">
          <div className="sidebar-logo">
            <span className="logo-icon">🚚</span>
            <span className="logo-text">TruckLogix</span>
          </div>
          <ul className="nav-links">
            <li><NavLink to="/" end className={({isActive}) => isActive ? 'active' : ''}>📊 Dashboard</NavLink></li>
            <li><NavLink to="/loads" className={({isActive}) => isActive ? 'active' : ''}>📦 Loads</NavLink></li>
            <li><NavLink to="/carriers" className={({isActive}) => isActive ? 'active' : ''}>🏢 Carriers</NavLink></li>
            <li><NavLink to="/drivers" className={({isActive}) => isActive ? 'active' : ''}>👤 Drivers</NavLink></li>
            {user.isAdmin && <li><NavLink to="/users" className={({isActive}) => isActive ? 'active' : ''}>🔑 Users</NavLink></li>}
          </ul>
          <div className="sidebar-user">
            <div className="sidebar-user-email" title={user.email}>{user.email}</div>
            <div className="sidebar-user-actions">
              <button onClick={() => setShowChangePassword(true)}>Change password</button>
              <button onClick={handleSignOut}>Sign out</button>
            </div>
          </div>
        </nav>
        <main className="main-content">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/loads" element={<Loads />} />
            <Route path="/loads/:id" element={<LoadDetail />} />
            <Route path="/carriers" element={<Carriers />} />
            <Route path="/drivers" element={<Drivers />} />
            <Route path="/users" element={user.isAdmin ? <Users currentEmail={user.email} /> : <Navigate to="/" replace />} />
          </Routes>
        </main>
        {showChangePassword && <ChangePasswordModal onClose={() => setShowChangePassword(false)} />}
      </div>
    </BrowserRouter>
  );
}

export default App;
