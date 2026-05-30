export interface BeachCruiser {
  id: number;
  name: string;
  color: string;
  frameSize: string;
  description: string;
  pricePerDay: number;
  isAvailable: boolean;
}

export interface MountainBike {
  id: number;
  modelName: string;
  brand: string;
  gearCount: number;
  suspensionType: string;
  frameMaterial: string;
  terrain: string;
  dailyRate: number;
  weightKg: number;
  isAvailable: boolean;
}

export interface Accessory {
  id: number;
  name: string;
  category: string;
  description: string;
  unitPrice: number;
  stockCount: number;
  compatibleWith: string;
}

export interface RentResponse {
  success: boolean;
  message: string;
}

export interface OrderResponse {
  success: boolean;
  message: string;
  subtotal: number;
  discountAmount: number;
  total: number;
  bundleDiscountApplied: boolean;
}
