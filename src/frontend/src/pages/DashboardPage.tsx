import React from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';

const DashboardPage: React.FC = () => {
  const { userEmail, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div>
      <h1>Dashboard</h1>
      <p>Esta é a dashboard.</p>
      {userEmail && <p>Bem-vindo, {userEmail}!</p>}
      <button onClick={handleLogout}>Sair</button>
    </div>
  );
};
export default DashboardPage;