import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { forgotPassword, type ForgotPasswordData } from '../services/AuthService'; // Caminho corrigido

const ForgotPasswordPage: React.FC = () => {
  const [email, setEmail] = useState('');
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setMessage(null);
    setIsLoading(true);

    try {
      const data: ForgotPasswordData = { email };
      const response = await forgotPassword(data);
      setMessage(response.message);
      if (response.developmentOnlyLink) { // Link retornado pelo backend em dev
        console.info("Development Only - Reset Link:", response.developmentOnlyLink);
        // Não precisa adicionar ao message, o backend já tem uma mensagem para dev
      }
      setEmail(''); // Limpar o campo após o sucesso
    } catch (err: any) {
      if (err.response && err.response.data && err.response.data.message) {
        setError(err.response.data.message);
      } else if (err.response && err.response.data && err.response.data.errors) {
        const messages = Object.values(err.response.data.errors).flat();
        setError(messages.join(' '));
      }
      else {
        setError('Falha ao solicitar recuperação de senha. Tente novamente mais tarde.');
      }
      console.error("Forgot Password error:", err);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: '400px', margin: '50px auto', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
      <h2>Esqueceu a Senha?</h2>
      <p style={{ marginBottom: '20px', fontSize: '0.9em', color: '#555' }}>
        Não se preocupe. Informe seu email abaixo e enviaremos um link para você redefinir sua senha.
      </p>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '15px' }}>
          <label htmlFor="email" style={{ display: 'block', marginBottom: '5px' }}>Email:</label>
          <input
            type="email"
            id="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            style={{ width: '100%', padding: '10px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #ddd' }}
          />
        </div>

        {error && <p style={{ color: 'red', marginBottom: '15px', fontSize: '0.9em'  }}>{error}</p>}
        {message && <p style={{ color: 'green', marginBottom: '15px', fontSize: '0.9em'  }}>{message}</p>}

        <button type="submit" disabled={isLoading} style={{ width: '100%', padding: '10px', backgroundColor: '#007bff', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
          {isLoading ? 'Enviando...' : 'Enviar Link de Recuperação'}
        </button>
      </form>
      <p style={{ marginTop: '20px', textAlign: 'center' }}>
        Lembrou a senha? <Link to="/login" style={{ color: '#007bff' }}>Faça login</Link>
      </p>
    </div>
  );
};

export default ForgotPasswordPage;