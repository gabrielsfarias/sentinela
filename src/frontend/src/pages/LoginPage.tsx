import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate, Link, useLocation } from 'react-router-dom'; // Adicionar useLocation

const LoginPage: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const { login, isLoading } = useAuth();
  const navigate = useNavigate();
  const location = useLocation(); // Para redirecionar de volta após login, se aplicável

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      await login({ email, password });

      const from = location.state?.from?.pathname || "/dashboard";
      navigate(from, { replace: true });

    } catch (err: any) {
      if (err.response && err.response.data && err.response.data.message) {
        setError(err.response.data.message);
      } else {
        setError(err.message || 'Falha no login. Verifique suas credenciais.');
      }
      console.error("Login error:", err);
    }
  };

  return (
    <div style={{ maxWidth: '400px', margin: '50px auto', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
      <h2>Login</h2>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '15px' }}>
          <label htmlFor="email" style={{ display: 'block', marginBottom: '5px' }}>Email:</label>
          <input
            type="email"
            id="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            autoComplete="email"
            style={{ width: '100%', padding: '10px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #ddd' }}
          />
        </div>
        <div style={{ marginBottom: '20px' }}>
          <label htmlFor="password" style={{ display: 'block', marginBottom: '5px' }}>Senha:</label>
          <input
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            autoComplete="current-password"
            style={{ width: '100%', padding: '10px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #ddd' }}
          />
        </div>
        {error && <p style={{ color: 'red', marginBottom: '15px', fontSize: '0.9em' }}>{error}</p>}
        <button type="submit" disabled={isLoading} style={{ width: '100%', padding: '10px', backgroundColor: '#007bff', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
          {isLoading ? 'Entrando...' : 'Entrar'}
        </button>
      </form>
      <div style={{ marginTop: '20px', textAlign: 'center', fontSize: '0.9em' }}>
        <p style={{ marginBottom: '10px' }}>
          <Link to="/forgot-password" style={{ color: '#007bff' }}>Esqueceu a senha?</Link>
        </p>
        <p>
          Não tem conta? <Link to="/cadastro" style={{ color: '#007bff' }}>Cadastre-se</Link>
        </p>
      </div>
    </div>
  );
};
export default LoginPage;