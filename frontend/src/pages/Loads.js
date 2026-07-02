import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { loadsApi, carriersApi, driversApi } from '../services/api';

const STATUSES = ['Available', 'Booked', 'InTransit', 'Delivered', 'Cancelled'];
const EQUIPMENT_TYPES = ['Dry Van', 'Reefer', 'Flatbed', 'Step Deck', 'Lowboy', 'Tanker', 'Intermodal'];

const statusBadge = (s) => {
  const map = { Available: 'badge-available', Booked: 'badge-booked', InTransit: 'badge-intransit', Delivered: 'badge-delivered', Cancelled: 'badge-cancelled' };
  return <span className={`badge ${map[s] || ''}`}>{s}</span>;
};

function CreateLoadModal({ carriers, drivers, onClose, onCreated }) {
  const [form, setForm] = useState({
    equipmentType: 'Dry Van', weight: '', commodity: '',
    pickupAddress: '', pickupCity: '', pickupState: '', pickupZip: '', pickupDate: '',
    deliveryAddress: '', deliveryCity: '', deliveryState: '', deliveryZip: '', deliveryDate: '',
    miles: '', rate: '', notes: ''
  });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  const set = (k, v) => setForm(f => ({ ...f, [k]: v }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setError('');
    try {
      await loadsApi.create({
        ...form,
        weight: form.weight ? parseFloat(form.weight) : null,
        miles: form.miles ? parseFloat(form.miles) : null,
        rate: form.rate ? parseFloat(form.rate) : null,
        pickupDate: new Date(form.pickupDate).toISOString(),
        deliveryDate: new Date(form.deliveryDate).toISOString(),
      });
      onCreated();
    } catch (err) {
      setError(err.response?.data?.title || 'Failed to create load');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={e => e.target === e.currentTarget && onClose()}>
      <div className="modal">
        <div className="modal-header">
          <span className="modal-title">New Load</span>
          <button className="modal-close" onClick={onClose}>×</button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="modal-body">
            {error && <div className="error-msg">{error}</div>}
            <div className="form-grid">
              <div className="form-group">
                <label className="form-label">Equipment Type *</label>
                <select className="form-select" value={form.equipmentType} onChange={e => set('equipmentType', e.target.value)} required>
                  {EQUIPMENT_TYPES.map(t => <option key={t}>{t}</option>)}
                </select>
              </div>
              <div className="form-group">
                <label className="form-label">Commodity</label>
                <input className="form-input" value={form.commodity} onChange={e => set('commodity', e.target.value)} placeholder="e.g. Electronics" />
              </div>
              <div className="form-group">
                <label className="form-label">Weight (lbs)</label>
                <input className="form-input" type="number" value={form.weight} onChange={e => set('weight', e.target.value)} />
              </div>
              <div className="form-group">
                <label className="form-label">Miles</label>
                <input className="form-input" type="number" value={form.miles} onChange={e => set('miles', e.target.value)} />
              </div>
              <div className="form-group">
                <label className="form-label">Rate ($)</label>
                <input className="form-input" type="number" step="0.01" value={form.rate} onChange={e => set('rate', e.target.value)} />
              </div>
            </div>
            <hr style={{margin:'16px 0', borderColor:'#e2e8f0'}} />
            <h4 style={{marginBottom:12, fontSize:14, fontWeight:700, color:'#475569'}}>Pickup</h4>
            <div className="form-group">
              <label className="form-label">Address *</label>
              <input className="form-input" value={form.pickupAddress} onChange={e => set('pickupAddress', e.target.value)} required />
            </div>
            <div className="form-grid-3">
              <div className="form-group">
                <label className="form-label">City *</label>
                <input className="form-input" value={form.pickupCity} onChange={e => set('pickupCity', e.target.value)} required />
              </div>
              <div className="form-group">
                <label className="form-label">State *</label>
                <input className="form-input" value={form.pickupState} onChange={e => set('pickupState', e.target.value)} maxLength={2} required />
              </div>
              <div className="form-group">
                <label className="form-label">Date *</label>
                <input className="form-input" type="datetime-local" value={form.pickupDate} onChange={e => set('pickupDate', e.target.value)} required />
              </div>
            </div>
            <hr style={{margin:'16px 0', borderColor:'#e2e8f0'}} />
            <h4 style={{marginBottom:12, fontSize:14, fontWeight:700, color:'#475569'}}>Delivery</h4>
            <div className="form-group">
              <label className="form-label">Address *</label>
              <input className="form-input" value={form.deliveryAddress} onChange={e => set('deliveryAddress', e.target.value)} required />
            </div>
            <div className="form-grid-3">
              <div className="form-group">
                <label className="form-label">City *</label>
                <input className="form-input" value={form.deliveryCity} onChange={e => set('deliveryCity', e.target.value)} required />
              </div>
              <div className="form-group">
                <label className="form-label">State *</label>
                <input className="form-input" value={form.deliveryState} onChange={e => set('deliveryState', e.target.value)} maxLength={2} required />
              </div>
              <div className="form-group">
                <label className="form-label">Date *</label>
                <input className="form-input" type="datetime-local" value={form.deliveryDate} onChange={e => set('deliveryDate', e.target.value)} required />
              </div>
            </div>
            <div className="form-group">
              <label className="form-label">Notes</label>
              <textarea className="form-textarea" rows={3} value={form.notes} onChange={e => set('notes', e.target.value)} />
            </div>
          </div>
          <div className="modal-footer">
            <button type="button" className="btn btn-outline" onClick={onClose}>Cancel</button>
            <button type="submit" className="btn btn-primary" disabled={saving}>{saving ? 'Creating...' : 'Create Load'}</button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default function Loads() {
  const navigate = useNavigate();
  const [loads, setLoads] = useState([]);
  const [carriers, setCarriers] = useState([]);
  const [drivers, setDrivers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [showCreate, setShowCreate] = useState(false);

  const fetchLoads = () => {
    setLoading(true);
    loadsApi.getAll({ search: search || undefined, status: statusFilter || undefined })
      .then(r => setLoads(r.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchLoads();
    carriersApi.getAll().then(r => setCarriers(r.data));
    driversApi.getAll().then(r => setDrivers(r.data));
  }, []);

  useEffect(() => { fetchLoads(); }, [search, statusFilter]);

  return (
    <div className="page">
      <div className="page-header">
        <h1 className="page-title">Loads</h1>
        <button className="btn btn-primary" onClick={() => setShowCreate(true)}>+ New Load</button>
      </div>

      <div className="search-bar">
        <input
          className="search-input"
          placeholder="Search loads, cities, commodity..."
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
        <select className="form-select" style={{width:'auto'}} value={statusFilter} onChange={e => setStatusFilter(e.target.value)}>
          <option value="">All Statuses</option>
          {STATUSES.map(s => <option key={s}>{s}</option>)}
        </select>
      </div>

      <div className="card">
        <div className="table-wrapper">
          {loading ? (
            <div className="loading">Loading loads...</div>
          ) : loads.length === 0 ? (
            <div className="empty-state"><div className="empty-icon">📦</div><p>No loads found.</p></div>
          ) : (
            <table>
              <thead>
                <tr>
                  <th>Load #</th>
                  <th>Route</th>
                  <th>Equipment</th>
                  <th>Carrier</th>
                  <th>Driver</th>
                  <th>Pickup</th>
                  <th>Delivery</th>
                  <th>Rate</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {loads.map(load => (
                  <tr key={load.id} onClick={() => navigate(`/loads/${load.id}`)}>
                    <td style={{fontWeight:600, color:'#2563a8'}}>{load.loadNumber}</td>
                    <td style={{fontSize:13}}>{load.pickupCity}, {load.pickupState} → {load.deliveryCity}, {load.deliveryState}</td>
                    <td>{load.equipmentType}</td>
                    <td>{load.carrierName || <span style={{color:'#94a3b8'}}>—</span>}</td>
                    <td>{load.driverName || <span style={{color:'#94a3b8'}}>—</span>}</td>
                    <td style={{fontSize:13}}>{new Date(load.pickupDate).toLocaleDateString()}</td>
                    <td style={{fontSize:13}}>{new Date(load.deliveryDate).toLocaleDateString()}</td>
                    <td style={{fontWeight:600}}>{load.rate ? `$${load.rate.toLocaleString()}` : '—'}</td>
                    <td>{statusBadge(load.status)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>

      {showCreate && (
        <CreateLoadModal
          carriers={carriers}
          drivers={drivers}
          onClose={() => setShowCreate(false)}
          onCreated={() => { setShowCreate(false); fetchLoads(); }}
        />
      )}
    </div>
  );
}
