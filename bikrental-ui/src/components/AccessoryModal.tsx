import { useState, useEffect } from 'react';
import type { Accessory } from '../types';
import { accessoryApi } from '../services/api';

interface Props {
  bikeType: string;
  onClose: () => void;
  onSuccess: (message: string) => void;
}

export function AccessoryModal({ bikeType, onClose, onSuccess }: Props) {
  const [accessories, setAccessories] = useState<Accessory[]>([]);
  const [quantities, setQuantities] = useState<Record<number, number>>({});
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [loadError, setLoadError] = useState('');

  useEffect(() => {
    let isActive = true;

    accessoryApi
      .getAccessories(bikeType)
      .then((data) => {
        if (!isActive) return;
        setAccessories(data);
        const initial: Record<number, number> = {};
        data.forEach((a) => (initial[a.id] = 0));
        setQuantities(initial);
      })
      .catch((err) => {
        if (!isActive) return;
        setLoadError(err instanceof Error ? err.message : 'Failed to load accessories');
      })
      .finally(() => {
        if (isActive) {
          setLoading(false);
        }
      });

    return () => {
      isActive = false;
    };
  }, [bikeType]);

  const bundleIds = new Set([1, 3]);
  const hasBundleDeal = [...bundleIds].every((id) => (quantities[id] || 0) > 0);

  const subtotal = accessories.reduce(
    (sum, a) => sum + a.unitPrice * (quantities[a.id] || 0),
    0
  );
  const discount = hasBundleDeal ? subtotal * 0.1 : 0;
  const total = subtotal - discount;

  const updateQty = (id: number, delta: number) => {
    const acc = accessories.find((a) => a.id === id);
    if (!acc) return;
    const newQty = Math.max(0, Math.min(acc.stockCount, (quantities[id] || 0) + delta));
    setQuantities((prev) => ({ ...prev, [id]: newQty }));
  };

  const handleSubmit = async () => {
    const items = Object.entries(quantities)
      .filter(([, qty]) => qty > 0)
      .map(([id, qty]) => ({ accessoryId: Number(id), quantity: qty }));

    if (items.length === 0) {
      onClose();
      return;
    }

    setSubmitting(true);
    try {
      const result = await accessoryApi.placeOrder(items);
      let msg = result.message;
      if (result.bundleDiscountApplied) {
        msg += ` (Saved $${result.discountAmount.toFixed(2)})`;
      }
      onSuccess(msg);
    } catch (err) {
      onSuccess(`Order failed: ${err instanceof Error ? err.message : 'Unknown error'}`);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading)
    return (
      <div className="modal-overlay">
        <div className="modal">
          <p style={{ padding: 40, textAlign: 'center' }}>Loading accessories...</p>
        </div>
      </div>
    );

  if (loadError)
    return (
      <div className="modal-overlay" role="dialog" aria-modal="true" aria-label="Accessories unavailable">
        <div className="modal">
          <div className="modal-header">
            <h2>Accessories unavailable</h2>
            <p>{loadError}</p>
          </div>
          <div className="modal-footer">
            <div className="modal-actions">
              <button className="btn-cancel" onClick={onClose}>Continue without accessories</button>
            </div>
          </div>
        </div>
      </div>
    );

  return (
    <div className="modal-overlay" role="dialog" aria-modal="true" aria-label="Add Accessories">
      <div className="modal">
        <div className="modal-header">
          <h2>🛍️ Add Accessories</h2>
          <p>Enhance your ride before you roll out.</p>
        </div>

        {hasBundleDeal && (
          <div className="bundle-banner">
            🎉 Bundle deal! Water Bottle + Bike Light = 10% off your order.
          </div>
        )}

        <div className="modal-body">
          {accessories.map((acc) => (
            <div key={acc.id} className="acc-item">
              <div className="acc-info">
                <div className="acc-name">{acc.name}</div>
                <div className="acc-desc">{acc.description}</div>
                <div className="acc-meta">
                  {acc.category} · Stock: {acc.stockCount}
                </div>
              </div>
              <div className="acc-controls">
                <span className="acc-price">${acc.unitPrice.toFixed(2)}</span>
                {acc.stockCount > 0 ? (
                  <div className="qty-control">
                    <button
                      onClick={() => updateQty(acc.id, -1)}
                      aria-label={`Decrease quantity of ${acc.name}`}
                    >
                      −
                    </button>
                    <span>{quantities[acc.id] || 0}</span>
                    <button
                      onClick={() => updateQty(acc.id, 1)}
                      aria-label={`Increase quantity of ${acc.name}`}
                    >
                      +
                    </button>
                  </div>
                ) : (
                  <span className="out-of-stock">Out of stock</span>
                )}
              </div>
            </div>
          ))}
        </div>

        <div className="modal-footer">
          <div className="order-summary">
            {discount > 0 && (
              <div className="summary-row discount-row">
                <span>Bundle Discount</span>
                <span>-${discount.toFixed(2)}</span>
              </div>
            )}
            <div className="summary-row total">
              <span>Total</span>
              <span>${total.toFixed(2)}</span>
            </div>
          </div>
          <div className="modal-actions">
            <button className="btn-cancel" onClick={onClose}>Skip</button>
            <button className="btn-order" onClick={handleSubmit} disabled={submitting}>
              {submitting ? 'Processing...' : 'Add to Rental'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
