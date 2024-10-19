import logo from './logo.svg';
import './App.css';
import { Button, Container } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import Landing from './presentation/pages/Landing';
import Canvas from './presentation/pages/Canvas';
import Canvases from './presentation/pages/Canvases';
import NewCanvas from './presentation/pages/NewCanvas';
import TopNavigation from './presentation/prefabs/Navigation';
import { Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import RootContextProvider from './domain/context/RootContextProvider';
import PerfCanvas from './presentation/pages/PerfCanvas';
import RTCanvas from './presentation/pages/RTCanvas';

function App() {
  return (
    <>
    <RootContextProvider>
      <Router>
        <TopNavigation />
        <Routes>
          <Route path="/" element={<Landing/>} />
          <Route path="/canvases" element={<Canvases />} />
          <Route path="/canvas/:id" element={<RTCanvas />} />
        </Routes>
      </Router>
    </RootContextProvider>
    </>
  );
}

export default App;
