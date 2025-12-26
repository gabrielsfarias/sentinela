import { Routes, Route } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import DashboardPage from './pages/DashboardPage';
import ForgotPasswordPage from './pages/ForgotPasswordPage';
import ResetPasswordPage from './pages/ResetPasswordPage';
import ChangePasswordPage from './pages/ChangePasswordPage'; // --- NOVA IMPORTAÇÃO ---
import PrivateRoute from './components/PrivateRoute';

function App() {
  return (
    <Routes>
      {/* Rotas Públicas */}
      <Route path="/login" element={<LoginPage />} />
      <Route path="/cadastro" element={<RegisterPage />} />
      <Route path="/forgot-password" element={<ForgotPasswordPage />} />
      <Route path="/reset-password" element={<ResetPasswordPage />} />

      {/* Rotas Privadas */}
      <Route path="/dashboard" element={
        <PrivateRoute>
          <DashboardPage />
        </PrivateRoute>
      } />
      <Route path="/account/change-password" element={
        <PrivateRoute>
          <ChangePasswordPage />
        </PrivateRoute>
      } />

      {/* Rota padrão pode ser o login ou um redirect para dashboard se logado */}
      <Route path="*" element={<LoginPage />} />
    </Routes>
  );
}
export default App;