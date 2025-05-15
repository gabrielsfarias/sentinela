import { Routes, Route } from 'react-router-dom'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import DashboardPage from './pages/DashboardPage'
import ForgotPasswordPage from './pages/ForgotPasswordPage'
import ResetPasswordPage from './pages/ResetPasswordPage'
import PrivateRoute from './components/PrivateRoute'
// import AuthLayout from './components/AuthLayout'; // Opcional

function App() {
  return (
    // <AuthLayout> // Se for usar um layout comum para login/register
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/cadastro" element={<RegisterPage />} />
      <Route path="/forgot-password" element={<ForgotPasswordPage />} />
      <Route path="/reset-password" element={<ResetPasswordPage />} /> {/* Pode precisar de :token/:email na rota */}

      <Route path="/dashboard" element={
        <PrivateRoute>
          <DashboardPage />
        </PrivateRoute>
      } />
      {/* Rota padrão pode ser o login ou um redirect */}
      <Route path="*" element={<LoginPage />} />
    </Routes>
    // </AuthLayout>
  )
}
export default App