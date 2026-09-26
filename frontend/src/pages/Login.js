import React, { useState } from 'react';
import { authApi, errorMessage } from '../services/api';

export default function Login({ onLoggedIn }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [signingIn, setSigningIn] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSigningIn(true); setError('');
    try {
      await authApi.login(email, password);
      onLoggedIn();
    } catch (err) {
      setError(errorMessage(err, 'Sign in failed. Is the API running?'));
      setSigningIn(false);
    }
  };

  return (
    <div className="login-page">
      <form className="card login-card" onSubmit={handleSubmit}>
        <div className="login-logo">
          <span className="logo-icon">🚚</span>
          <span className="logo-text">TruckLogix</span>
        </div>
        <p className="login-subtitle">Sign in to continue</p>
        {error && <div className="error-msg">{error}</div>}
        <div className="form-group">
          <label className="form-label" htmlFor="email">Email</label>
          <input id="email" className="form-input" type="email" autoComplete="username" value={email} onChange={e => setEmail(e.target.value)} required autoFocus />
        </div>
        <div className="form-group">
          <label className="form-label" htmlFor="password">Password</label>
          <input id="password" className="form-input" type="password" autoComplete="current-password" value={password} onChange={e => setPassword(e.target.value)} required />
        </div>
        <button type="submit" className="btn btn-primary login-button" disabled={signingIn}>{signingIn ? 'Signing in...' : 'Sign in'}</button>
        <p className="login-hint">Accounts are created by an administrator.</p>
      </form>
    </div>
  );
}
