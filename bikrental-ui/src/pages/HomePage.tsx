import { Link } from 'react-router-dom';
import { useState } from 'react';
import { bikeApi } from '../services/api';
import { useToast } from '../hooks/useToast';

export function HomePage() {
  const [resetting, setResetting] = useState(false);
  const { toast, showToast } = useToast();

  const handleReset = async () => {
    setResetting(true);
    try {
      const result = await bikeApi.resetFleet();
      showToast(result.message);
    } catch {
      showToast('Reset failed');
    } finally {
      setResetting(false);
    }
  };

  return (
    <div className="page">
      <header className="home-header">
        <h1>🚲 PedalPal</h1>
        <p>Modern Bike Rental System</p>
      </header>

      <div className="category-grid">
        <Link to="/beach-cruisers" className="category-card beach-card">
          <div className="category-icon">🏖️</div>
          <h2>Beach Cruisers</h2>
          <p>Smooth rides along the coast. Relaxed geometry, wide tires, pure vibes.</p>
        </Link>

        <Link to="/mountain-bikes" className="category-card mountain-card">
          <div className="category-icon">⛰️</div>
          <h2>Mountain Bikes</h2>
          <p>Built for trails. Full suspension, aggressive gearing, ready for anything.</p>
        </Link>
      </div>

      <div className="admin-section">
        <button className="btn-secondary" onClick={handleReset} disabled={resetting}>
          {resetting ? 'Resetting...' : '🔄 Reset Fleet'}
        </button>
      </div>

      {toast && <div className="toast" role="status">{toast}</div>}
    </div>
  );
}
