import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { HomePage } from './pages/HomePage';
import { BeachCruisersPage } from './pages/BeachCruisersPage';
import { MountainBikesPage } from './pages/MountainBikesPage';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/beach-cruisers" element={<BeachCruisersPage />} />
        <Route path="/mountain-bikes" element={<MountainBikesPage />} />
      </Routes>
    </BrowserRouter>
  );
}
