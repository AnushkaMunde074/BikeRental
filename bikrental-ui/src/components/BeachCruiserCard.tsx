import type { BeachCruiser } from '../types';

interface Props {
  bike: BeachCruiser;
  onRent: (id: number) => void;
  renting: boolean;
}

export function BeachCruiserCard({ bike, onRent, renting }: Props) {
  return (
    <div className="card">
      <div className="card-header beach">
        <h3>{bike.name}</h3>
        <span className="color-tag">● {bike.color}</span>
      </div>
      <div className="card-body">
        <p>{bike.description}</p>
        <div className="meta-row">
          <span className="label">Frame Size</span>
          <span className="value">{bike.frameSize}</span>
        </div>
      </div>
      <div className="card-footer">
        <div className="price">
          ${bike.pricePerDay.toFixed(2)} <span>/ day</span>
        </div>
        <span className={`badge ${bike.isAvailable ? 'available' : 'rented'}`}>
          {bike.isAvailable ? 'Available' : 'Rented'}
        </span>
      </div>
      <div className="card-action">
        <button
          className="btn-rent"
          disabled={!bike.isAvailable || renting}
          onClick={() => onRent(bike.id)}
        >
          {renting ? 'Processing...' : bike.isAvailable ? 'Rent This Bike' : 'Not Available'}
        </button>
      </div>
    </div>
  );
}
