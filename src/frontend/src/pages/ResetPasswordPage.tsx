import React, { useState, useEffect } from 'react';
import { useSearchParams, Link, useNavigate } from 'react-router-dom';
import { resetPassword, type ResetPasswordData } from '../services/AuthService'; // Caminho corrigido

const ResetPasswordPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  const [token, setToken] = useState<string | null>(null);
  const [email, setEmail] = useState<string | null>(null); // Email virá da URL

  const [newPassword, setNewPassword] = useState('');
  const [confirmNewPassword, setConfirmNewPassword] = useState('');

  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isLinkInvalid, setIsLinkInvalid] = useState(false);

  useEffect(() => {
    const urlToken = searchParams.get('token');
    const urlEmail = searchParams.get('email');

    if (urlToken && urlEmail) {
      setToken(urlToken);
      setEmail(urlEmail);
      setIsLinkInvalid(false);
    } else {
      setError("Link de recuperação inválido ou incompleto. Por favor, solicite um novo link.");
      setIsLinkInvalid(true);
    }
  }, [searchParams]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setMessage(null);

    if (isLinkInvalid) {
        setError("Não é possível prosseguir, o link é inválido.");
        return;
    }

    if (newPassword !== confirmNewPassword) {
      setError("As novas senhas não conferem.");
      return;
    }
    if (!token || !email) { // Checagem extra, embora isLinkInvalid deva cobrir
      setError("Token ou email ausentes. Não é possível resetar a senha.");
      return;
    }

    setIsLoading(true);

    try {
      const data: ResetPasswordData = {
        email: email!, // Sabemos que o email existe por causa do useEffect
        token: token!, // Sabemos que o token existe
        newPassword,
      };
      const response = await resetPassword(data);
      setMessage(response.message + " Você será redirecionado para o login em breve.");
      setNewPassword('');
      setConfirmNewPassword('');
      setTimeout(() => navigate('/login'), 4000); // Redireciona após mostrar a mensagem
    } catch (err: any) {
      if (err.response && err.response.data && err.response.data.message) {
        setError(err.response.data.message);
      } else if (err.response && err.response.data && err.response.data.errors) {
        const messages = Object.values(err.response.data.errors).flat();
        setError(messages.join(' '));
      }
       else {
        setError('Falha ao redefinir a senha. O link pode ter expirado ou a senha não atende aos critérios.');
      }
      console.error("Reset Password error:", err);
    } finally {
      setIsLoading(false);
    }
  };

  if (isLinkInvalid && error) {
      return (
        <div style={{ maxWidth: '400px', margin: '50px auto', padding: '20px', border: '1px solid #ccc', borderRadius: '8px', textAlign: 'center' }}>
            <h2 style={{color: 'red'}}>Link Inválido</h2>
            <p style={{ color: 'red', marginBottom: '15px' }}>{error}</p>
            <Link to="/forgot-password" style={{ color: '#007bff' }}>Solicitar novo link</Link> <br/>
            <Link to="/login" style={{ color: '#007bff', marginTop: '10px', display: 'inline-block' }}>Voltar para Login</Link>
        </div>
      );
  }

  return (
    <div style={{ maxWidth: '400px', margin: '50px auto', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
      <h2>Redefinir Senha</h2>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '15px' }}>
          <label htmlFor="email-display" style={{ display: 'block', marginBottom: '5px' }}>Email:</label>
          <input
            type="email"
            id="email-display"
            value={email || ''} // Preenchido da URL
            readOnly // Usuário não deve editar
            style={{ width: '100%', padding: '10px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #ddd', backgroundColor: '#f9f9f9' }}
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

        <button type="submit" disabled={isLoading || isLinkInvalid} style={{ width: '100%', padding: '10px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
          {isLoading ? 'Redefinindo...' : 'Redefinir Senha'}
        </button>
      </form>
      {!message && ( // Não mostrar o link de login se uma mensagem de sucesso já estiver sendo exibida
        <p style={{ marginTop: '20px', textAlign: 'center' }}>
            <Link to="/login" style={{ color: '#007bff' }}>Voltar para Login</Link>
        </p>
      )}
    </div>
  );
};

export default ResetPasswordPage;