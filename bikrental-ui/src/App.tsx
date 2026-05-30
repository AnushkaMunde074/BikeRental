import { HomePage } from './pages/HomePage';
import { BeachCruisersPage } from './pages/BeachCruisersPage';
import { MountainBikesPage } from './pages/MountainBikesPage';

function App() {
  const path = window.location.pathname;

  if (path === '/beach-cruisers') return <BeachCruisersPage />;
  if (path === '/mountain-bikes') return <MountainBikesPage />;
  return <HomePage />;
}

export default App;
