import React from 'react';
import { BrowserRouter, Routes, Route, NavLink } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import Loads from './pages/Loads';
import LoadDetail from './pages/LoadDetail';
import Carriers from './pages/Carriers';
import Drivers from './pages/Drivers';
import './App.css';

function App() {
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
          </ul>
        </nav>
        <main className="main-content">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/loads" element={<Loads />} />
            <Route path="/loads/:id" element={<LoadDetail />} />
            <Route path="/carriers" element={<Carriers />} />
            <Route path="/drivers" element={<Drivers />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;
