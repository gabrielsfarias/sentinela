import { createContext, useContext, useState, type ReactNode, useEffect } from 'react';
import {
    login as apiLogin,
    register as apiRegister,
    type LoginCredentials,
    type RegisterData,
    setAuthToken, // Importado
    clearAuthToken // Importado
} from '../services/AuthService';
import axios from 'axios';

interface AuthContextType {
  isAuthenticated: boolean;
  userEmail: string | null;
  token: string | null; // Pode não ser necessário expor o token diretamente se for gerenciado internamente
  login: (credentials: LoginCredentials) => Promise<void>;
  register: (data: RegisterData) => Promise<void>; // Adicionado para consistência
  logout: (options?: { navigate?: (path: string) => void }) => void;
  isLoading: boolean; // Para feedback de UI durante chamadas de API de auth
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  // Estado inicial tenta carregar do localStorage
  const [tokenState, setTokenState] = useState<string | null>(() => localStorage.getItem('authToken'));
  const [userEmail, setUserEmail] = useState<string | null>(() => localStorage.getItem('userEmail'));
  const [isLoading, setIsLoading] = useState<boolean>(true); // Começa true para verificar o token inicial

  useEffect(() => {
    const responseInterceptor = axios.interceptors.response.use(
      response => response, // Passa a resposta adiante se não houver erro
      error => {
        // Verifica se o erro é de uma chamada à API e se o status é 401
        if (error.response && error.response.status === 401) {
          // Evita loop se o erro 401 for da própria API de login ou endpoints públicos
          const originalRequestUrl = error.config.url;
          if (
            !originalRequestUrl.endsWith('/login') &&
            !originalRequestUrl.endsWith('/cadastro') &&
            !originalRequestUrl.endsWith('/forgot-password') &&
            !originalRequestUrl.endsWith('/reset-password')
          ) {
            console.warn("Interceptor: Erro 401 detectado. Deslogando usuário.", error.config.url);
            // Chama a função de logout interna do contexto.
            // Não é ideal chamar window.location.href diretamente aqui,
            // pois o estado do React pode não ser limpo corretamente.
            // O logout do contexto deve lidar com o redirecionamento.
            // Esta chamada de logout precisa ser síncrona ou o interceptor
            // pode não bloquear corretamente.
            handleLogoutForInterceptor();
          }
        }
        return Promise.reject(error); // Importante repassar o erro
      }
    );
    // Cleanup do interceptor quando o AuthProvider é desmontado
    return () => {
      axios.interceptors.response.eject(responseInterceptor);
    };
  }, []); // Executa uma vez na montagem do AuthProvider

  // Função interna para ser chamada pelo interceptor
  const handleLogoutForInterceptor = () => {
      setTokenState(null);
      setUserEmail(null);
      localStorage.removeItem('authToken');
      localStorage.removeItem('userEmail');
      clearAuthToken(); // Limpa o header do Axios
      // O redirecionamento será tratado pelo PrivateRoute ou pelo componente que detecta isAuthenticated === false
      // Se precisar de redirecionamento imediato aqui:
      // if (window.location.pathname !== '/login') { // Evita loop se já estiver em /login
      //    window.location.href = '/login?sessionExpired=true';
      // }
      // Uma solução melhor é usar o navigate do React Router se ele puder ser acessado aqui.
  };

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