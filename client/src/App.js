import logo from './logo.svg';
import './App.css';
import { Button } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import Landing from './presentation/pages/landing';
import TopNavigation from './presentation/prefabs/Navigation';

function App() {
  return (
    <>
      <TopNavigation></TopNavigation>
      <Landing></Landing>
    </>
  );
}

export default App;
