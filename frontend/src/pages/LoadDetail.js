import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { loadsApi, carriersApi, driversApi } from '../services/api';

const TRACKING_TYPES = ['PickedUp', 'InTransit', 'Delayed', 'Delivered', 'Exception'];
const STATUSES = ['Available', 'Booked', 'InTransit', 'Delivered', 'Cancelled'];
const trackingIcon = { PickedUp: '🚛', InTransit: '📍', Delayed: '⚠️', Delivered: '✅', Exception: '🔴' };

export default function LoadDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [load, setLoad] = useState(null);
  const [carriers, setCarriers] = useState([]);
  const [drivers, setDrivers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [showTracking, setShowTracking] = useState(false);
  const [trackingForm, setTrackingForm] = useState({ eventType: 'InTransit', city: '', state: '', notes: '', eventTime: new Date().toISOString().slice(0,16) });

  const fetchLoad = () =>
    loadsApi.getById(id).then(r => setLoad(r.data)).catch(() => navigate('/loads'));

  useEffect(() => {
    Promise.all([fetchLoad(), carriersApi.getAll(), driversApi.getAll()])
      .then(([, cRes, dRes]) => { setCarriers(cRes.data); setDrivers(dRes.data); })
      .finally(() => setLoading(false));
  }, [id]);

  const handleAssign = async (field, value) => {
    setSaving(true);
    try {
      await loadsApi.update(id, {
        ...load,
        carrierId: field === 'carrierId' ? (value ? parseInt(value) : null) : load.carrierId,
        driverId: field === 'driverId' ? (value ? parseInt(value) : null) : load.driverId,
        pickupDate: load.pickupDate,
        deliveryDate: load.deliveryDate,
      });
      await fetchLoad();
    } catch (err) {
      alert('Update failed');
    } finally {
      setSaving(false);
    }
  };

  const handleStatusChange = async (status) => {
    setSaving(true);
    try {
      await loadsApi.update(id, { ...load, status, pickupDate: load.pickupDate, deliveryDate: load.deliveryDate });
      await fetchLoad();
    } finally {
      setSaving(false);
    }
  };

  const handleAddTracking = async (e) => {
    e.preventDefault();
    try {
      await loadsApi.addTracking(id, { ...trackingForm, eventTime: new Date(trackingForm.eventTime).toISOString() });
      setShowTracking(false);
      await fetchLoad();
    } catch (err) {
      alert('Failed to add tracking event');
    }
  };

  const handleDelete = async () => {
    if (window.confirm('Delete this load?')) {
      await loadsApi.delete(id);
      navigate('/loads');
    }
  };

  if (loading) return <div className="loading">Loading...</div>;
  if (!load) return null;

  const statusBadgeClass = { Available: 'badge-available', Booked: 'badge-booked', InTransit: 'badge-intransit', Delivered: 'badge-delivered', Cancelled: 'badge-cancelled' };

  return (
    <div className="page">
      <Link to="/loads" className="back-link">← Back to Loads</Link>

      <div className="page-header">
        <div>
          <h1 className="page-title">{load.loadNumber}</h1>
          <span className={`badge ${statusBadgeClass[load.status]}`} style={{marginTop:6, display:'inline-block'}}>{load.status}</span>
        </div>
        <div style={{display:'flex', gap:10}}>
          <button className="btn btn-danger btn-sm" onClick={handleDelete}>Delete</button>
        </div>
      </div>

      {/* Status & Assignment Controls */}
      <div className="card" style={{marginBottom:20}}>
        <div className="card-body">
          <div className="form-grid">
            <div className="form-group" style={{margin:0}}>
              <label className="form-label">Status</label>
              <select className="form-select" value={load.status} onChange={e => handleStatusChange(e.target.value)} disabled={saving}>
                {STATUSES.map(s => <option key={s}>{s}</option>)}
              </select>
            </div>
            <div className="form-group" style={{margin:0}}>
              <label className="form-label">Assigned Carrier</label>
              <select className="form-select" value={load.carrierId || ''} onChange={e => handleAssign('carrierId', e.target.value)} disabled={saving}>
                <option value="">— Unassigned —</option>
                {carriers.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
            </div>
            <div className="form-group" style={{margin:0}}>
              <label className="form-label">Assigned Driver</label>
              <select className="form-select" value={load.driverId || ''} onChange={e => handleAssign('driverId', e.target.value)} disabled={saving}>
                <option value="">— Unassigned —</option>
                {drivers.filter(d => !load.carrierId || d.carrierId === load.carrierId).map(d => (
                  <option key={d.id} value={d.id}>{d.firstName} {d.lastName} {!d.isAvailable ? '(Busy)' : ''}</option>
                ))}
              </select>
            </div>
          </div>
        </div>
      </div>

      {/* Load Details */}
      <div className="detail-grid">
        <div className="card">
          <div className="card-body">
            <div className="detail-section">
              <h3>Pickup</h3>
              <div className="detail-row"><span className="detail-label">Address</span><span className="detail-value">{load.pickupAddress}</span></div>
              <div className="detail-row"><span className="detail-label">City, State</span><span className="detail-value">{load.pickupCity}, {load.pickupState} {load.pickupZip}</span></div>
              <div className="detail-row"><span className="detail-label">Date</span><span className="detail-value">{new Date(load.pickupDate).toLocaleString()}</span></div>
            </div>
          </div>
        </div>
        <div className="card">
          <div className="card-body">
            <div className="detail-section">
              <h3>Delivery</h3>
              <div className="detail-row"><span className="detail-label">Address</span><span className="detail-value">{load.deliveryAddress}</span></div>
              <div className="detail-row"><span className="detail-label">City, State</span><span className="detail-value">{load.deliveryCity}, {load.deliveryState} {load.deliveryZip}</span></div>
              <div className="detail-row"><span className="detail-label">Date</span><span className="detail-value">{new Date(load.deliveryDate).toLocaleString()}</span></div>
            </div>
          </div>
        </div>
        <div className="card">
          <div className="card-body">
            <div className="detail-section">
              <h3>Freight Details</h3>
              <div className="detail-row"><span className="detail-label">Equipment</span><span className="detail-value">{load.equipmentType}</span></div>
              <div className="detail-row"><span className="detail-label">Commodity</span><span className="detail-value">{load.commodity || '—'}</span></div>
              <div className="detail-row"><span className="detail-label">Weight</span><span className="detail-value">{load.weight ? `${load.weight.toLocaleString()} lbs` : '—'}</span></div>
              <div className="detail-row"><span className="detail-label">Miles</span><span className="detail-value">{load.miles ? `${load.miles.toLocaleString()} mi` : '—'}</span></div>
              <div className="detail-row"><span className="detail-label">Rate</span><span className="detail-value">{load.rate ? `$${load.rate.toLocaleString()}` : '—'}</span></div>
            </div>
          </div>
        </div>
        {load.notes && (
          <div className="card">
            <div className="card-body">
              <div className="detail-section">
                <h3>Notes</h3>
                <p style={{fontSize:14, color:'#475569', lineHeight:1.6}}>{load.notes}</p>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Tracking */}
      <div className="card">
        <div className="card-body">
          <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:20}}>
            <h3 style={{fontSize:16, fontWeight:700}}>Tracking Events</h3>
            <button className="btn btn-outline btn-sm" onClick={() => setShowTracking(true)}>+ Add Event</button>
          </div>

          {load.trackingEvents?.length === 0 ? (
            <div className="empty-state" style={{padding:'30px 0'}}>
              <div className="empty-icon" style={{fontSize:32}}>📍</div>
              <p>No tracking events yet.</p>
            </div>
          ) : (
            <ul className="tracking-timeline">
              {load.trackingEvents?.map(evt => (
                <li key={evt.id} className="tracking-item">
                  <div className="tracking-dot">{trackingIcon[evt.eventType] || '📍'}</div>
                  <div className="tracking-content">
                    <div className="tracking-type">{evt.eventType}</div>
                    <div className="tracking-meta">
                      {evt.city && `${evt.city}, ${evt.state} • `}
                      {new Date(evt.eventTime).toLocaleString()}
                    </div>
                    {evt.notes && <div style={{fontSize:13, color:'#475569', marginTop:4}}>{evt.notes}</div>}
                  </div>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>

      {/* Add Tracking Modal */}
      {showTracking && (
        <div className="modal-overlay" onClick={e => e.target === e.currentTarget && setShowTracking(false)}>
          <div className="modal" style={{maxWidth:440}}>
            <div className="modal-header">
              <span className="modal-title">Add Tracking Event</span>
              <button className="modal-close" onClick={() => setShowTracking(false)}>×</button>
            </div>
            <form onSubmit={handleAddTracking}>
              <div className="modal-body">
                <div className="form-group">
                  <label className="form-label">Event Type</label>
                  <select className="form-select" value={trackingForm.eventType} onChange={e => setTrackingForm(f => ({...f, eventType: e.target.value}))}>
                    {TRACKING_TYPES.map(t => <option key={t}>{t}</option>)}
                  </select>
                </div>
                <div className="form-grid">
                  <div className="form-group">
                    <label className="form-label">City</label>
                    <input className="form-input" value={trackingForm.city} onChange={e => setTrackingForm(f => ({...f, city: e.target.value}))} />
                  </div>
                  <div className="form-group">
                    <label className="form-label">State</label>
                    <input className="form-input" maxLength={2} value={trackingForm.state} onChange={e => setTrackingForm(f => ({...f, state: e.target.value}))} />
                  </div>
                </div>
                <div className="form-group">
                  <label className="form-label">Event Time</label>
                  <input className="form-input" type="datetime-local" value={trackingForm.eventTime} onChange={e => setTrackingForm(f => ({...f, eventTime: e.target.value}))} />
                </div>
                <div className="form-group">
                  <label className="form-label">Notes</label>
                  <textarea className="form-textarea" rows={2} value={trackingForm.notes} onChange={e => setTrackingForm(f => ({...f, notes: e.target.value}))} />
                </div>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-outline" onClick={() => setShowTracking(false)}>Cancel</button>
                <button type="submit" className="btn btn-primary">Add Event</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
