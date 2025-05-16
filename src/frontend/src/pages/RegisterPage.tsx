import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { register as apiRegister } from '../services/AuthService';


const RegisterPage: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccessMessage(null);

    console.log("Password:", password);
    console.log("Confirm Password:", confirmPassword);

    if (password !== confirmPassword) {
      setError("As senhas não conferem.");
      return;
    }
    setIsLoading(true);
    try {
      // Usando diretamente o service, pois o AuthContext.register pode não logar automaticamente
      const response = await apiRegister({ email, password, confirmPassword });
      setSuccessMessage(response.message || "Cadastro realizado! Você pode fazer login agora.");
      // Opcional: redirecionar para login após um tempo ou limpar campos
      // navigate('/login');
    } catch (err: any) {
        if (err.response && err.response.data) {
            if (typeof err.response.data === 'string') {
                setError(err.response.data);
            } else if (err.response.data.errors) { // Para erros de ModelState
                const messages = Object.values(err.response.data.errors).flat();
                setError(messages.join(' '));
            } else if(err.response.data.message) {
                 setError(err.response.data.message);
            } else {
                setError('Falha no cadastro. Verifique os dados.');
            }
        } else {
            setError(err.message || 'Falha no cadastro.');
        }
    } finally {
        setIsLoading(false);
    }
  };

  return (
    <div>
      <h2>Cadastro</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="email">Email:</label>
          <input type="email" id="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        </div>
        <div>
          <label htmlFor="password">Senha:</label>
          <input type="password" id="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
        </div>
        <div>
          <label htmlFor="confirmPassword">Confirmar Senha:</label>
          <input type="password" id="confirmPassword" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} required />
        </div>
        {error && <p style={{ color: 'red' }}>{error}</p>}
        {successMessage && <p style={{ color: 'green' }}>{successMessage}</p>}
        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Cadastrando...' : 'Cadastrar'}
        </button>
      </form>
      <p>
        Já tem conta? <Link to="/login">Faça login</Link>
      </p>
    </div>
  );
};
export default RegisterPage;