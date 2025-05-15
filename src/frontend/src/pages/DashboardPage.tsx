import React from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate, Link } from 'react-router-dom';

const DashboardPage: React.FC = () => {
  const { userEmail, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout({ navigate });
  };

  return (
    <div style={{ padding: '20px' }}>
      <h1>Dashboard</h1>
      <p>Esta é a dashboard.</p>
      {userEmail && <p>Bem-vindo, {userEmail}!</p>}
      
      <div style={{ marginTop: '20px', display: 'flex', gap: '10px' }}>
        <button onClick={handleLogout} style={{ padding: '8px 15px' }}>Sair</button>
        <Link to="/account/change-password">
          <button style={{ padding: '8px 15px' }}>Alterar Senha</button>
        </Link>
      </div>
      {/* Aqui você começará a adicionar a lógica de documentos */}
    </div>
  );
};

export default DashboardPage;