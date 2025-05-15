import React, { useState } from 'react';
import { Link, } from 'react-router-dom';
import { changePassword, type ChangePasswordData } from '../services/AuthService'; // Ajuste o caminho se necessário
import { useAuth } from '../contexts/AuthContext'; // Para verificar se está autenticado

const ChangePasswordPage: React.FC = () => {
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmNewPassword, setConfirmNewPassword] = useState('');

  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const { isAuthenticated, } = useAuth(); // Usar logout do contexto

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setMessage(null);

    if (!isAuthenticated) {
        setError("Você precisa estar logado para mudar a senha.");
        // Opcional: redirecionar para login
        // setTimeout(() => navigate('/login'), 2000);
        return;
    }

    if (newPassword !== confirmNewPassword) {
      setError("A nova senha e a senha de confirmação não conferem.");
      return;
    }
    // Adicionar mais validações de frontend para a força da senha se desejar

    setIsLoading(true);
    try {
      const data: ChangePasswordData = { currentPassword, newPassword };
      const response = await changePassword(data);
      setMessage(response.message + " Você pode precisar fazer login novamente com sua nova senha se sua sessão atual expirar.");
      // Limpar campos
      setCurrentPassword('');
      setNewPassword('');
      setConfirmNewPassword('');

      // Opcional: forçar logout para que o usuário precise logar com a nova senha
      // setTimeout(() => {
      //   logout({ navigate }); // Passa o navigate para o logout
      // }, 3000);

    } catch (err: any) {
      if (err.response && err.response.data) {
        if (err.response.data.message) {
            setError(err.response.data.message);
        } else if (err.response.data.errors) {
            const messages = Object.values(err.response.data.errors).flat();
            setError(messages.join(' '));
        } else if (typeof err.response.data === 'string') {
            setError(err.response.data)
        } else {
            setError('Falha ao alterar a senha. Verifique os dados e tente novamente.');
        }
      } else {
        setError('Ocorreu um erro desconhecido. Tente novamente mais tarde.');
      }
      console.error("Change Password error:", err);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: '400px', margin: '50px auto', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
      <h2>Alterar Senha</h2>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '15px' }}>
          <label htmlFor="currentPassword" style={{ display: 'block', marginBottom: '5px' }}>Senha Atual:</label>
          <input
            type="password"
            id="currentPassword"
            value={currentPassword}
            onChange={(e) => setCurrentPassword(e.target.value)}
            required
            style={{ width: '100%', padding: '10px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #ddd' }}
          />
        </div>
        <div style={{ marginBottom: '15px' }}>
          <label htmlFor="newPassword" style={{ display: 'block', marginBottom: '5px' }}>Nova Senha:</label>
          <input
            type="password"
            id="newPassword"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            required
            style={{ width: '100%', padding: '10px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #ddd' }}
          />
        </div>
        <div style={{ marginBottom: '20px' }}>
          <label htmlFor="confirmNewPassword" style={{ display: 'block', marginBottom: '5px' }}>Confirmar Nova Senha:</label>
          <input
            type="password"
            id="confirmNewPassword"
            value={confirmNewPassword}
            onChange={(e) => setConfirmNewPassword(e.target.value)}
            required
            style={{ width: '100%', padding: '10px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #ddd' }}
          />
        </div>

        {error && <p style={{ color: 'red', marginBottom: '15px', fontSize: '0.9em' }}>{error}</p>}
        {message && <p style={{ color: 'green', marginBottom: '15px', fontSize: '0.9em' }}>{message}</p>}

        <button type="submit" disabled={isLoading} style={{ width: '100%', padding: '10px', backgroundColor: '#007bff', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
          {isLoading ? 'Alterando...' : 'Alterar Senha'}
        </button>
      </form>
      <p style={{ marginTop: '20px', textAlign: 'center' }}>
        <Link to="/dashboard" style={{ color: '#007bff' }}>Voltar para Dashboard</Link>
      </p>
    </div>
  );
};

export default ChangePasswordPage;