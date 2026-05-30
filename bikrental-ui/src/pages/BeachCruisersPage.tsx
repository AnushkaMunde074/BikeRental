import { useState, useEffect } from 'react';
import type { BeachCruiser } from '../types';
import { bikeApi } from '../services/api';
import { BeachCruiserCard } from '../components/BeachCruiserCard';
import { AccessoryModal } from '../components/AccessoryModal';

export function BeachCruisersPage() {
  const [bikes, setBikes] = useState<BeachCruiser[]>([]);
  const [loading, setLoading] = useState(true);
  const [rentingId, setRentingId] = useState<number | null>(null);
  const [showAccessories, setShowAccessories] = useState(false);
  const [toast, setToast] = useState('');

  useEffect(() => {
    bikeApi.getBeachCruisers().then((data) => {
      setBikes(data);
      setLoading(false);
    });
  }, []);

  const showToast = (msg: string) => {
    setToast(msg);
    setTimeout(() => setToast(''), 4000);
  };

  const handleRent = async (id: number) => {
    setRentingId(id);
    try {
      const result = await bikeApi.rentBike('beach', id);
      if (result.success) {
        setBikes(bikes.map((b) => (b.id === id ? { ...b, isAvailable: false } : b)));
        setShowAccessories(true);
      } else {
        showToast(result.message);
      }
    } catch (err) {
      showToast(err instanceof Error ? err.message : 'Rental failed');
    }
    setRentingId(null);
  };

  if (loading) return <div className="loading">Loading bikes...</div>;

  return (
    <div className="page">
      <header className="page-header beach-header">
        <a href="/" className="back-btn">← Back</a>
        <h1>🏖️ Beach Cruisers</h1>
      </header>

      <div className="grid">
        {bikes.map((bike) => (
          <BeachCruiserCard
            key={bike.id}
            bike={bike}
            onRent={handleRent}
            renting={rentingId === bike.id}
          />
        ))}
      </div>

      {showAccessories && (
        <AccessoryModal
          bikeType="beach"
          onClose={() => setShowAccessories(false)}
          onSuccess={(msg) => {
            setShowAccessories(false);
            showToast(msg);
          }}
        />
      )}

      {toast && <div className="toast">{toast}</div>}
    </div>
  );
}
