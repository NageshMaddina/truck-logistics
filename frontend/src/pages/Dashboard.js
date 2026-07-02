import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { loadsApi } from '../services/api';

export default function Dashboard() {
  const [stats, setStats] = useState(null);
  const [recentLoads, setRecentLoads] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      loadsApi.getStats(),
      loadsApi.getAll()
    ]).then(([statsRes, loadsRes]) => {
      setStats(statsRes.data);
      setRecentLoads(loadsRes.data.slice(0, 5));
    }).catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const statusBadge = (status) => {
    const map = { Available: 'badge-available', Booked: 'badge-booked', InTransit: 'badge-intransit', Delivered: 'badge-delivered', Cancelled: 'badge-cancelled' };
    return <span className={`badge ${map[status] || ''}`}>{status}</span>;
  };

  if (loading) return <div className="loading">Loading dashboard...</div>;

  return (
    <div className="page">
      <div className="page-header">
        <h1 className="page-title">Dashboard</h1>
        <Link to="/loads" className="btn btn-primary">+ New Load</Link>
      </div>

      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-label">Total Loads</div>
          <div className="stat-value">{stats?.total ?? 0}</div>
        </div>
        <div className="stat-card">
          <div className="stat-label">Available</div>
          <div className="stat-value success">{stats?.available ?? 0}</div>
        </div>
        <div className="stat-card">
          <div className="stat-label">In Transit</div>
          <div className="stat-value info">{stats?.inTransit ?? 0}</div>
        </div>
        <div className="stat-card">
          <div className="stat-label">Booked</div>
          <div className="stat-value accent">{stats?.booked ?? 0}</div>
        </div>
        <div className="stat-card">
          <div className="stat-label">Delivered</div>
          <div className="stat-value success">{stats?.delivered ?? 0}</div>
        </div>
        <div className="stat-card">
          <div className="stat-label">Total Revenue</div>
          <div className="stat-value" style={{fontSize:'22px'}}>${(stats?.totalRevenue ?? 0).toLocaleString()}</div>
        </div>
      </div>

      <div className="card">
        <div className="card-body">
          <h2 style={{fontSize:'16px', fontWeight:700, marginBottom:'16px'}}>Recent Loads</h2>
          {recentLoads.length === 0 ? (
            <div className="empty-state"><div className="empty-icon">📦</div><p>No loads yet. Create your first load to get started.</p></div>
          ) : (
            <div className="table-wrapper">
              <table>
                <thead>
                  <tr>
                    <th>Load #</th>
                    <th>Route</th>
                    <th>Equipment</th>
                    <th>Carrier</th>
                    <th>Pickup</th>
                    <th>Status</th>
                    <th>Rate</th>
                  </tr>
                </thead>
                <tbody>
                  {recentLoads.map(load => (
                    <tr key={load.id} onClick={() => window.location.href = `/loads/${load.id}`}>
                      <td style={{fontWeight:600, color:'#2563a8'}}>{load.loadNumber}</td>
                      <td>{load.pickupCity}, {load.pickupState} → {load.deliveryCity}, {load.deliveryState}</td>
                      <td>{load.equipmentType}</td>
                      <td>{load.carrierName || <span style={{color:'#94a3b8'}}>Unassigned</span>}</td>
                      <td>{new Date(load.pickupDate).toLocaleDateString()}</td>
                      <td>{statusBadge(load.status)}</td>
                      <td>{load.rate ? `$${load.rate.toLocaleString()}` : '-'}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
          <div style={{marginTop:'16px'}}>
            <Link to="/loads" className="btn btn-outline btn-sm">View All Loads →</Link>
          </div>
        </div>
      </div>
    </div>
  );
}
