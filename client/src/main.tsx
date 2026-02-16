import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import './index.css'
import App from './App.tsx'
import { AuthProvider } from './context/AuthContext';
import { SseProvider } from './context/SseContext';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AuthProvider>
      <SseProvider>
        <App />
      </SseProvider>
    </AuthProvider>
  </StrictMode>,
)
