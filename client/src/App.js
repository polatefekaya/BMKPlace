import logo from './logo.svg';
import './App.css';
import { Button, Container } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import Canvas from './presentation/pages/canvas';
import Canvases from './presentation/pages/canvases';
import Landing from './presentation/pages/landing';
import TopNavigation from './presentation/prefabs/Navigation';
import { Route, BrowserRouter as Router, Routes } from 'react-router-dom';

function App() {
  return (
    <>
      <Router>
        <TopNavigation />
        <Routes>
          <Route path="/" element={<Landing/>} />
          <Route path="/canvases" element={<Canvases />} />
          <Route path="/canvas/:id" element={<Canvas />} />
        </Routes>
      </Router>
    </>
  );
}

export default App;
