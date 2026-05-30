import { bikeApi } from '../services/api';
import { useState } from 'react';

export function HomePage() {
  const [resetting, setResetting] = useState(false);
  const [toast, setToast] = useState('');

  const handleReset = async () => {
    setResetting(true);
    try {
      const result = await bikeApi.resetFleet();
      setToast(result.message);
      setTimeout(() => setToast(''), 4000);
    } catch {
      setToast('Reset failed');
    }
    setResetting(false);
  };

  return (
    <div className="page">
      <header className="home-header">
        <h1>🚲 PedalPal</h1>
        <p>Modern Bike Rental System</p>
      </header>

      <div className="category-grid">
        <a href="/beach-cruisers" className="category-card beach-card">
          <div className="category-icon">🏖️</div>
          <h2>Beach Cruisers</h2>
          <p>Smooth rides along the coast. Relaxed geometry, wide tires, pure vibes.</p>
        </a>

        <a href="/mountain-bikes" className="category-card mountain-card">
          <div className="category-icon">⛰️</div>
          <h2>Mountain Bikes</h2>
          <p>Built for trails. Full suspension, aggressive gearing, ready for anything.</p>
        </a>
      </div>

      <div className="admin-section">
        <button className="btn-secondary" onClick={handleReset} disabled={resetting}>
          {resetting ? 'Resetting...' : '🔄 Reset Fleet'}
        </button>
      </div>

      {toast && <div className="toast">{toast}</div>}
    </div>
  );
}
