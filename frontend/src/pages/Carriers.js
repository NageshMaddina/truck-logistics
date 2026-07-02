import React, { useEffect, useState } from 'react';
import { carriersApi } from '../services/api';

function CarrierModal({ carrier, onClose, onSaved }) {
  const [form, setForm] = useState(carrier || { name:'', mcNumber:'', dotNumber:'', contactName:'', contactEmail:'', contactPhone:'', address:'', city:'', state:'', zipCode:'', rating:'' });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const set = (k,v) => setForm(f => ({...f, [k]:v}));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true); setError('');
    try {
      const payload = { ...form, rating: form.rating ? parseFloat(form.rating) : null };
      if (carrier) await carriersApi.update(carrier.id, payload);
      else await carriersApi.create(payload);
      onSaved();
    } catch (err) {
      setError(err.response?.data?.title || 'Save failed');
    } finally { setSaving(false); }
  };

  return (
    <div className="modal-overlay" onClick={e => e.target === e.currentTarget && onClose()}>
      <div className="modal">
        <div className="modal-header">
          <span className="modal-title">{carrier ? 'Edit Carrier' : 'New Carrier'}</span>
          <button className="modal-close" onClick={onClose}>×</button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="modal-body">
            {error && <div className="error-msg">{error}</div>}
            <div className="form-grid">
              <div className="form-group">
                <label className="form-label">Carrier Name *</label>
                <input className="form-input" value={form.name} onChange={e => set('name', e.target.value)} required />
              </div>
              <div className="form-group">
                <label className="form-label">MC Number *</label>
                <input className="form-input" value={form.mcNumber} onChange={e => set('mcNumber', e.target.value)} required />
              </div>
              <div className="form-group">
                <label className="form-label">DOT Number</label>
                <input className="form-input" value={form.dotNumber} onChange={e => set('dotNumber', e.target.value)} />
              </div>
              <div className="form-group">
                <label className="form-label">Rating (0–5)</label>
                <input className="form-input" type="number" step="0.1" min="0" max="5" value={form.rating} onChange={e => set('rating', e.target.value)} />
              </div>
            </div>
            <hr style={{margin:'12px 0', borderColor:'#e2e8f0'}} />
            <h4 style={{marginBottom:12, fontSize:13, fontWeight:700, color:'#475569'}}>Contact</h4>
            <div className="form-grid">
              <div className="form-group">
                <label className="form-label">Contact Name</label>
                <input className="form-input" value={form.contactName} onChange={e => set('contactName', e.target.value)} />
              </div>
              <div className="form-group">
                <label className="form-label">Email</label>
                <input className="form-input" type="email" value={form.contactEmail} onChange={e => set('contactEmail', e.target.value)} />
              </div>
              <div className="form-group">
                <label className="form-label">Phone</label>
                <input className="form-input" value={form.contactPhone} onChange={e => set('contactPhone', e.target.value)} />
              </div>
              <div className="form-group">
                <label className="form-label">City</label>
                <input className="form-input" value={form.city} onChange={e => set('city', e.target.value)} />
              </div>
              <div className="form-group">
                <label className="form-label">State</label>
                <input className="form-input" maxLength={2} value={form.state} onChange={e => set('state', e.target.value)} />
              </div>
            </div>
          </div>
          <div className="modal-footer">
            <button type="button" className="btn btn-outline" onClick={onClose}>Cancel</button>
            <button type="submit" className="btn btn-primary" disabled={saving}>{saving ? 'Saving...' : 'Save Carrier'}</button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default function Carriers() {
  const [carriers, setCarriers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [modal, setModal] = useState(null); // null | 'create' | carrier obj

  const fetch = () => {
    setLoading(true);
    carriersApi.getAll({ search: search || undefined })
      .then(r => setCarriers(r.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  };

  useEffect(() => { fetch(); }, [search]);

  const handleDelete = async (id) => {
    if (window.confirm('Deactivate this carrier?')) {
      await carriersApi.delete(id);
      fetch();
    }
  };

  const renderStars = (rating) => {
    if (!rating) return '—';
    return '★'.repeat(Math.round(rating)) + '☆'.repeat(5 - Math.round(rating)) + ` (${rating})`;
  };

  return (
    <div className="page">
      <div className="page-header">
        <h1 className="page-title">Carriers</h1>
        <button className="btn btn-primary" onClick={() => setModal('create')}>+ New Carrier</button>
      </div>

      <div className="search-bar">
        <input className="search-input" placeholder="Search carriers, MC#, contact..." value={search} onChange={e => setSearch(e.target.value)} />
      </div>

      <div className="card">
        <div className="table-wrapper">
          {loading ? (
            <div className="loading">Loading carriers...</div>
          ) : carriers.length === 0 ? (
            <div className="empty-state"><div className="empty-icon">🏢</div><p>No carriers found.</p></div>
          ) : (
            <table>
              <thead>
                <tr>
                  <th>Carrier Name</th>
                  <th>MC Number</th>
                  <th>Contact</th>
                  <th>Location</th>
                  <th>Drivers</th>
                  <th>Active Loads</th>
                  <th>Rating</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {carriers.map(c => (
                  <tr key={c.id} onClick={() => setModal(c)}>
                    <td style={{fontWeight:600}}>{c.name}</td>
                    <td style={{fontFamily:'monospace', fontSize:13}}>{c.mcNumber}</td>
                    <td>
                      <div style={{fontSize:13}}>{c.contactName}</div>
                      {c.contactPhone && <div style={{fontSize:11, color:'#64748b'}}>{c.contactPhone}</div>}
                    </td>
                    <td style={{fontSize:13}}>{c.city && `${c.city}, ${c.state}`}</td>
                    <td style={{textAlign:'center'}}>{c.totalDrivers}</td>
                    <td style={{textAlign:'center'}}>{c.activeLoads}</td>
                    <td style={{fontSize:13, color:'#d97706'}}>{renderStars(c.rating)}</td>
                    <td><span className={`badge ${c.isActive ? 'badge-active' : 'badge-inactive'}`}>{c.isActive ? 'Active' : 'Inactive'}</span></td>
                    <td onClick={e => e.stopPropagation()}>
                      <button className="btn btn-outline btn-sm" onClick={() => handleDelete(c.id)}>Deactivate</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>

      {modal && (
        <CarrierModal
          carrier={modal === 'create' ? null : modal}
          onClose={() => setModal(null)}
          onSaved={() => { setModal(null); fetch(); }}
        />
      )}
    </div>
  );
}
