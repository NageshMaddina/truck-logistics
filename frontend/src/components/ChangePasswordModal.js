import React, { useState } from 'react';
import { authApi, errorMessage } from '../services/api';

export default function ChangePasswordModal({ onClose }) {
  const [form, setForm] = useState({ current: '', next: '', confirm: '' });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [done, setDone] = useState(false);
  const set = (k, v) => setForm(f => ({ ...f, [k]: v }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (form.next !== form.confirm) { setError('The new passwords do not match.'); return; }
    setSaving(true); setError('');
    try {
      await authApi.changePassword(form.current, form.next);
      setDone(true);
    } catch (err) {
      setError(errorMessage(err, 'Could not change password'));
    } finally { setSaving(false); }
  };

  return (
    <div className="modal-overlay" onClick={e => e.target === e.currentTarget && onClose()}>
      <div className="modal" style={{maxWidth:420}}>
        <div className="modal-header">
          <span className="modal-title">Change Password</span>
          <button className="modal-close" onClick={onClose}>×</button>
        </div>
        {done ? (
          <>
            <div className="modal-body">Your password has been changed.</div>
            <div className="modal-footer"><button className="btn btn-primary" onClick={onClose}>Close</button></div>
          </>
        ) : (
          <form onSubmit={handleSubmit}>
            <div className="modal-body">
              {error && <div className="error-msg">{error}</div>}
              <div className="form-group">
                <label className="form-label">Current password</label>
                <input className="form-input" type="password" autoComplete="current-password" value={form.current} onChange={e => set('current', e.target.value)} required />
              </div>
              <div className="form-group">
                <label className="form-label">New password</label>
                <input className="form-input" type="password" autoComplete="new-password" value={form.next} onChange={e => set('next', e.target.value)} required />
              </div>
              <div className="form-group">
                <label className="form-label">Confirm new password</label>
                <input className="form-input" type="password" autoComplete="new-password" value={form.confirm} onChange={e => set('confirm', e.target.value)} required />
              </div>
            </div>
            <div className="modal-footer">
              <button type="button" className="btn btn-outline" onClick={onClose}>Cancel</button>
              <button type="submit" className="btn btn-primary" disabled={saving}>{saving ? 'Saving...' : 'Change Password'}</button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}
