import React, { type JSX } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

interface PrivateRouteProps {
  children: JSX.Element;
}

const PrivateRoute: React.FC<PrivateRouteProps> = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return <div>Carregando autenticação...</div>; // Ou um spinner
  }

  if (!isAuthenticated) {
    // Redireciona para a página de login, mas salva a localização atual
    // para que possamos enviar o usuário de volta após o login.
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return children;
};

export default PrivateRoute;