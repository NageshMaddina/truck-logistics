import React, { useCallback, useEffect, useState } from 'react';
import { usersApi, errorMessage } from '../services/api';

function NewUserModal({ onClose, onSaved }) {
  const [form, setForm] = useState({ email: '', password: '', isAdmin: false });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const set = (k, v) => setForm(f => ({ ...f, [k]: v }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true); setError('');
    try {
      await usersApi.create(form);
      onSaved();
    } catch (err) {
      setError(errorMessage(err, 'Could not create user'));
      setSaving(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={e => e.target === e.currentTarget && onClose()}>
      <div className="modal" style={{maxWidth:440}}>
        <div className="modal-header">
          <span className="modal-title">New User</span>
          <button className="modal-close" onClick={onClose}>×</button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="modal-body">
            {error && <div className="error-msg">{error}</div>}
            <div className="form-group">
              <label className="form-label">Email *</label>
              <input className="form-input" type="email" value={form.email} onChange={e => set('email', e.target.value)} required />
            </div>
            <div className="form-group">
              <label className="form-label">Temporary password *</label>
              <input className="form-input" type="text" autoComplete="new-password" value={form.password} onChange={e => set('password', e.target.value)} required />
              <div className="form-hint">At least 6 characters, with upper and lower case letters, a number and a symbol. Share it with the user; they can change it after signing in.</div>
            </div>
            <label className="checkbox-label">
              <input type="checkbox" checked={form.isAdmin} onChange={e => set('isAdmin', e.target.checked)} />
              Administrator (can manage users)
            </label>
          </div>
          <div className="modal-footer">
            <button type="button" className="btn btn-outline" onClick={onClose}>Cancel</button>
            <button type="submit" className="btn btn-primary" disabled={saving}>{saving ? 'Creating...' : 'Create User'}</button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default function Users({ currentEmail }) {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showCreate, setShowCreate] = useState(false);

  const fetchUsers = useCallback(() => {
    setLoading(true);
    usersApi.getAll()
      .then(r => setUsers(r.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => { fetchUsers(); }, [fetchUsers]);

  const handleResetPassword = async (user) => {
    const newPassword = window.prompt(`New temporary password for ${user.email}:`);
    if (!newPassword) return;
    try {
      await usersApi.resetPassword(user.id, newPassword);
      window.alert(`Password reset. Share the new password with ${user.email}.`);
      fetchUsers();
    } catch (err) {
      window.alert(errorMessage(err, 'Could not reset password'));
    }
  };

  const handleDelete = async (user) => {
    if (!window.confirm(`Delete ${user.email}? They will no longer be able to sign in.`)) return;
    try {
      await usersApi.delete(user.id);
      fetchUsers();
    } catch (err) {
      window.alert(errorMessage(err, 'Could not delete user'));
    }
  };

  return (
    <div className="page">
      <div className="page-header">
        <h1 className="page-title">Users</h1>
        <button className="btn btn-primary" onClick={() => setShowCreate(true)}>+ New User</button>
      </div>

      <div className="card">
        <div className="table-wrapper">
          {loading ? (
            <div className="loading">Loading users...</div>
          ) : (
            <table>
              <thead>
                <tr>
                  <th>Email</th>
                  <th>Role</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {users.map(u => (
                  <tr key={u.id}>
                    <td style={{fontWeight:600}}>{u.email}{u.email === currentEmail && <span style={{color:'#64748b', fontWeight:400}}> (you)</span>}</td>
                    <td>{u.isAdmin ? 'Administrator' : 'User'}</td>
                    <td><span className={`badge ${u.isLockedOut ? 'badge-inactive' : 'badge-active'}`}>{u.isLockedOut ? 'Locked' : 'Active'}</span></td>
                    <td style={{display:'flex', gap:8}}>
                      <button className="btn btn-outline btn-sm" onClick={() => handleResetPassword(u)}>Reset password</button>
                      {u.email !== currentEmail && <button className="btn btn-outline btn-sm" onClick={() => handleDelete(u)}>Delete</button>}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>

      {showCreate && (
        <NewUserModal
          onClose={() => setShowCreate(false)}
          onSaved={() => { setShowCreate(false); fetchUsers(); }}
        />
      )}
    </div>
  );
}
