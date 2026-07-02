import React, { useEffect, useState } from 'react';
import { driversApi, carriersApi } from '../services/api';

function DriverModal({ driver, carriers, onClose, onSaved }) {
  const [form, setForm] = useState(driver || { carrierId:'', firstName:'', lastName:'', licenseNumber:'', phone:'', email:'' });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const set = (k,v) => setForm(f => ({...f, [k]:v}));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true); setError('');
    try {
      const payload = { ...form, carrierId: parseInt(form.carrierId) };
      if (driver) await driversApi.update(driver.id, payload);
      else await driversApi.create(payload);
      onSaved();
    } catch (err) {
      setError(err.response?.data?.title || 'Save failed');
    } finally { setSaving(false); }
  };

  return (
    <div className="modal-overlay" onClick={e => e.target === e.currentTarget && onClose()}>
      <div className="modal" style={{maxWidth:500}}>
        <div className="modal-header">
          <span className="modal-title">{driver ? 'Edit Driver' : 'New Driver'}</span>
          <button className="modal-close" onClick={onClose}>×</button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="modal-body">
            {error && <div className="error-msg">{error}</div>}
            <div className="form-group">
              <label className="form-label">Carrier *</label>
              <select className="form-select" value={form.carrierId} onChange={e => set('carrierId', e.target.value)} required>
                <option value="">Select carrier...</option>
                {carriers.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
            </div>
            <div className="form-grid">
              <div className="form-group">
                <label className="form-label">First Name *</label>
                <input className="form-input" value={form.firstName} onChange={e => set('firstName', e.target.value)} required />
              </div>
              <div className="form-group">
                <label className="form-label">Last Name *</label>
                <input className="form-input" value={form.lastName} onChange={e => set('lastName', e.target.value)} required />
              </div>
              <div className="form-group">
                <label className="form-label">CDL License # *</label>
                <input className="form-input" value={form.licenseNumber} onChange={e => set('licenseNumber', e.target.value)} required />
              </div>
              <div className="form-group">
                <label className="form-label">Phone</label>
                <input className="form-input" value={form.phone} onChange={e => set('phone', e.target.value)} />
              </div>
              <div className="form-group">
                <label className="form-label">Email</label>
                <input className="form-input" type="email" value={form.email} onChange={e => set('email', e.target.value)} />
              </div>
            </div>
          </div>
          <div className="modal-footer">
            <button type="button" className="btn btn-outline" onClick={onClose}>Cancel</button>
            <button type="submit" className="btn btn-primary" disabled={saving}>{saving ? 'Saving...' : 'Save Driver'}</button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default function Drivers() {
  const [drivers, setDrivers] = useState([]);
  const [carriers, setCarriers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filterCarrier, setFilterCarrier] = useState('');
  const [filterAvailable, setFilterAvailable] = useState('');
  const [modal, setModal] = useState(null);

  const fetchDrivers = () => {
    setLoading(true);
    driversApi.getAll({
      carrierId: filterCarrier || undefined,
      available: filterAvailable !== '' ? filterAvailable === 'true' : undefined
    })
      .then(r => setDrivers(r.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    carriersApi.getAll().then(r => setCarriers(r.data));
  }, []);

  useEffect(() => { fetchDrivers(); }, [filterCarrier, filterAvailable]);

  const toggleAvailability = async (driver, e) => {
    e.stopPropagation();
    await driversApi.setAvailability(driver.id, !driver.isAvailable);
    fetchDrivers();
  };

  return (
    <div className="page">
      <div className="page-header">
        <h1 className="page-title">Drivers</h1>
        <button className="btn btn-primary" onClick={() => setModal('create')}>+ New Driver</button>
      </div>

      <div className="search-bar">
        <select className="form-select" style={{width:'auto'}} value={filterCarrier} onChange={e => setFilterCarrier(e.target.value)}>
          <option value="">All Carriers</option>
          {carriers.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
        </select>
        <select className="form-select" style={{width:'auto'}} value={filterAvailable} onChange={e => setFilterAvailable(e.target.value)}>
          <option value="">All Availability</option>
          <option value="true">Available</option>
          <option value="false">Busy</option>
        </select>
      </div>

      <div className="card">
        <div className="table-wrapper">
          {loading ? (
            <div className="loading">Loading drivers...</div>
          ) : drivers.length === 0 ? (
            <div className="empty-state"><div className="empty-icon">👤</div><p>No drivers found.</p></div>
          ) : (
            <table>
              <thead>
                <tr>
                  <th>Name</th>
                  <th>Carrier</th>
                  <th>CDL License</th>
                  <th>Phone</th>
                  <th>Email</th>
                  <th>Availability</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {drivers.map(d => (
                  <tr key={d.id} onClick={() => setModal(d)}>
                    <td style={{fontWeight:600}}>{d.firstName} {d.lastName}</td>
                    <td>{d.carrierName}</td>
                    <td style={{fontFamily:'monospace', fontSize:13}}>{d.licenseNumber}</td>
                    <td style={{fontSize:13}}>{d.phone || '—'}</td>
                    <td style={{fontSize:13}}>{d.email || '—'}</td>
                    <td>
                      <span className={`badge ${d.isAvailable ? 'badge-available' : 'badge-intransit'}`}>
                        {d.isAvailable ? 'Available' : 'Busy'}
                      </span>
                    </td>
                    <td onClick={e => e.stopPropagation()} style={{display:'flex', gap:8}}>
                      <button className="btn btn-outline btn-sm" onClick={(e) => toggleAvailability(d, e)}>
                        {d.isAvailable ? 'Mark Busy' : 'Mark Free'}
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>

      {modal && (
        <DriverModal
          driver={modal === 'create' ? null : modal}
          carriers={carriers}
          onClose={() => setModal(null)}
          onSaved={() => { setModal(null); fetchDrivers(); }}
        />
      )}
    </div>
  );
}
