import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import type { BeachCruiser } from '../types';
import { bikeApi } from '../services/api';
import { BeachCruiserCard } from '../components/BeachCruiserCard';
import { AccessoryModal } from '../components/AccessoryModal';
import { useToast } from '../hooks/useToast';

export function BeachCruisersPage() {
  const [bikes, setBikes] = useState<BeachCruiser[]>([]);
  const [loading, setLoading] = useState(true);
  const [rentingId, setRentingId] = useState<number | null>(null);
  const [showAccessories, setShowAccessories] = useState(false);
  const { toast, showToast } = useToast();

  useEffect(() => {
    bikeApi
      .getBeachCruisers()
      .then((data) => {
        setBikes(data);
      })
      .catch((err) => {
        showToast(err instanceof Error ? err.message : 'Failed to load beach cruisers');
      })
      .finally(() => {
        setLoading(false);
      });
  }, []);

  const handleRent = async (id: number) => {
    setRentingId(id);
    try {
      const result = await bikeApi.rentBike('beach', id);
      if (result.success) {
        setBikes((prev) => prev.map((b) => (b.id === id ? { ...b, isAvailable: false } : b)));
        setShowAccessories(true);
      } else {
        showToast(result.message);
      }
    } catch (err) {
      showToast(err instanceof Error ? err.message : 'Rental failed');
    } finally {
      setRentingId(null);
    }
  };

  if (loading) return <div className="loading">Loading bikes...</div>;

  return (
    <div className="page">
      <header className="page-header beach-header">
        <Link to="/" className="back-btn">← Back</Link>
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

      {toast && <div className="toast" role="status">{toast}</div>}
    </div>
  );
}
