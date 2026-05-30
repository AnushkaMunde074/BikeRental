import type { MountainBike } from '../types';

interface Props {
  bike: MountainBike;
  onRent: (id: number) => void;
  renting: boolean;
}

export function MountainBikeCard({ bike, onRent, renting }: Props) {
  return (
    <div className="card">
      <div className="card-header mountain">
        <h3>{bike.modelName}</h3>
        <span className="color-tag">{bike.brand}</span>
      </div>
      <div className="card-body">
        <div className="meta-row">
          <span className="label">Gears</span>
          <span className="value">{bike.gearCount}-speed</span>
        </div>
        <div className="meta-row">
          <span className="label">Suspension</span>
          <span className="value">{bike.suspensionType}</span>
        </div>
        <div className="meta-row">
          <span className="label">Frame</span>
          <span className="value">{bike.frameMaterial}</span>
        </div>
        <div className="meta-row">
          <span className="label">Terrain</span>
          <span className="value">{bike.terrain}</span>
        </div>
        <div className="meta-row">
          <span className="label">Weight</span>
          <span className="value">{bike.weightKg} kg</span>
        </div>
      </div>
      <div className="card-footer">
        <div className="price">
          ${bike.dailyRate.toFixed(2)} <span>/ day</span>
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
