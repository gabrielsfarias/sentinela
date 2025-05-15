import { createContext, useContext, useState, type ReactNode, useEffect } from 'react';
import {
    login as apiLogin,
    register as apiRegister,
    type LoginCredentials,
    type RegisterData,
    setAuthToken, // Importado
    clearAuthToken // Importado
} from '../services/AuthService';

interface AuthContextType {
  isAuthenticated: boolean;
  userEmail: string | null;
  token: string | null; // Pode não ser necessário expor o token diretamente se for gerenciado internamente
  login: (credentials: LoginCredentials) => Promise<void>;
  register: (data: RegisterData) => Promise<void>; // Adicionado para consistência
  logout: () => void;
  isLoading: boolean; // Para feedback de UI durante chamadas de API de auth
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  // Estado inicial tenta carregar do localStorage
  const [tokenState, setTokenState] = useState<string | null>(() => localStorage.getItem('authToken'));
  const [userEmail, setUserEmail] = useState<string | null>(() => localStorage.getItem('userEmail'));
  const [isLoading, setIsLoading] = useState<boolean>(true); // Começa true para verificar o token inicial

  useEffect(() => {
    const storedToken = localStorage.getItem('authToken');
    const storedEmail = localStorage.getItem('userEmail');
    if (storedToken) {
      setTokenState(storedToken);
      setUserEmail(storedEmail || null); // Garante que seja null se não houver email
      setAuthToken(storedToken); // Configura o header do Axios
    }
    setIsLoading(false); // Terminou a verificação inicial
  }, []);

  const login = async (credentials: LoginCredentials) => {
    setIsLoading(true);
    try {
      const response = await apiLogin(credentials);
      setTokenState(response.token);
      setUserEmail(response.email);
      localStorage.setItem('authToken', response.token);
      localStorage.setItem('userEmail', response.email);
      // setAuthToken já é chamado dentro de apiLogin se bem sucedido
    } catch (error) {
      clearAuthToken(); // Limpa o token em caso de falha no login
      console.error("Falha no login:", error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const register = async (data: RegisterData) => { // Implementação de register no contexto
    setIsLoading(true);
    try {
      await apiRegister(data);
      // Após o cadastro, o usuário geralmente precisa fazer login.
      // Você pode redirecionar para a página de login ou mostrar uma mensagem.
    } catch (error) {
      console.error("Falha no cadastro:", error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const logout = () => {
    setTokenState(null);
    setUserEmail(null);
    localStorage.removeItem('authToken');
    localStorage.removeItem('userEmail');
    clearAuthToken(); // Limpa o header do Axios
    // O redirecionamento para /login é geralmente feito no componente que chama logout
  };

  return (
    <AuthContext.Provider value={{
        isAuthenticated: !!tokenState,
        userEmail,
        token: tokenState,
        login,
        register,
        logout,
        isLoading
      }}>
      {!isLoading && children} {/* Só renderiza children após a verificação inicial do token */}
      {isLoading && <div>Verificando autenticação...</div>} {/* Ou um spinner global */}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth deve ser usado dentro de um AuthProvider');
  }
  return context;
};