import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import type { MountainBike } from '../types';
import { bikeApi } from '../services/api';
import { MountainBikeCard } from '../components/MountainBikeCard';
import { AccessoryModal } from '../components/AccessoryModal';
import { useToast } from '../hooks/useToast';

export function MountainBikesPage() {
  const [bikes, setBikes] = useState<MountainBike[]>([]);
  const [loading, setLoading] = useState(true);
  const [rentingId, setRentingId] = useState<number | null>(null);
  const [showAccessories, setShowAccessories] = useState(false);
  const { toast, showToast } = useToast();

  useEffect(() => {
    bikeApi
      .getMountainBikes()
      .then((data) => {
        setBikes(data);
      })
      .catch((err) => {
        showToast(err instanceof Error ? err.message : 'Failed to load mountain bikes');
      })
      .finally(() => {
        setLoading(false);
      });
  }, []);

  const handleRent = async (id: number) => {
    setRentingId(id);
    try {
      const result = await bikeApi.rentBike('mountain', id);
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
      <header className="page-header mountain-header">
        <Link to="/" className="back-btn">← Back</Link>
        <h1>⛰️ Mountain Bikes</h1>
      </header>

      <div className="grid">
        {bikes.map((bike) => (
          <MountainBikeCard
            key={bike.id}
            bike={bike}
            onRent={handleRent}
            renting={rentingId === bike.id}
          />
        ))}
      </div>

      {showAccessories && (
        <AccessoryModal
          bikeType="mountain"
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
