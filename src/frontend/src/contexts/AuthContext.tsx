import { createContext, useContext, useState, type ReactNode, useEffect } from 'react';
import {
  login as apiLogin,
  register as apiRegister,
  type LoginCredentials,
  type RegisterData,
  setAuthToken,
  clearAuthToken
} from '../services/AuthService'; // Verifique o nome do arquivo: AuthService ou authService
import axios from 'axios';

interface AuthContextType {
  isAuthenticated: boolean;
  userEmail: string | null;
  token: string | null;
  login: (credentials: LoginCredentials) => Promise<void>;
  register: (data: RegisterData) => Promise<void>;
  logout: (options?: { navigate?: (path: string, navigateOptions?: { replace?: boolean }) => void }) => void;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [tokenState, setTokenState] = useState<string | null>(() => localStorage.getItem('authToken'));
  const [userEmail, setUserEmail] = useState<string | null>(() => localStorage.getItem('userEmail'));
  const [isLoading, setIsLoading] = useState<boolean>(true);

  const handleLogoutForInterceptor = (isErrorFromLogin = false) => { // Adicionado parâmetro opcional
    if (isErrorFromLogin && tokenState === null && !localStorage.getItem('authToken')) {
      console.log("Interceptor: Erro 401 da API de login, mas usuário já parece deslogado. Nenhuma ação de logout forçado.");
      return;
    }
    console.warn("Interceptor: Erro 401 detectado ou logout forçado. Deslogando usuário.");
    setTokenState(null);
    setUserEmail(null);
    localStorage.removeItem('authToken');
    localStorage.removeItem('userEmail');
    clearAuthToken();
    // O redirecionamento é melhor tratado pelo componente que detecta isAuthenticated: false
    // ou pelo componente que explicitamente chama logout com a função navigate.
    // Forçar window.location.href aqui pode ser abrupto.
    // Se for realmente necessário um redirecionamento global a partir daqui:
    // if (window.location.pathname !== '/login' && !window.location.pathname.startsWith('/reset-password')) {
    //    window.location.href = '/login?sessionExpired=true';
    // }
  };

  useEffect(() => {
    const responseInterceptor = axios.interceptors.response.use(
      response => response,
      error => {
        if (error.response && error.response.status === 401) {
          const originalRequestUrl = error.config.url?.toString() || ''; // Garante que é string
          const isLoginAttempt = originalRequestUrl.endsWith('/login');

          if (!isLoginAttempt && // Não deslogue em falha de login (o login handler trata isso)
            !originalRequestUrl.endsWith('/cadastro') &&
            !originalRequestUrl.endsWith('/forgot-password') &&
            !originalRequestUrl.endsWith('/reset-password')
          ) {
            handleLogoutForInterceptor();
          } else if (isLoginAttempt) {
            // Se foi uma tentativa de login que falhou com 401, o login handler já trata.
            // Não precisamos fazer um logout forçado aqui, só garantir que o token seja limpo
            // se por acaso existisse um inválido. O login handler já faz clearAuthToken().
            console.warn("Interceptor: Erro 401 na tentativa de login. O componente de login deve tratar.");
          }
        }
        return Promise.reject(error);
      }
    );
    return () => {
      axios.interceptors.response.eject(responseInterceptor);
    };
  }, [tokenState]);

  useEffect(() => {
    const storedToken = localStorage.getItem('authToken');
    const storedEmail = localStorage.getItem('userEmail');
    if (storedToken) {
      setTokenState(storedToken);
      setUserEmail(storedEmail || null);
      setAuthToken(storedToken);
    }
    setIsLoading(false);
  }, []);

  const login = async (credentials: LoginCredentials) => {
    setIsLoading(true);
    try {
      const response = await apiLogin(credentials);
      setTokenState(response.token);
      setUserEmail(response.email);
      localStorage.setItem('authToken', response.token);
      localStorage.setItem('userEmail', response.email);
      // setAuthToken é chamado dentro de apiLogin
    } catch (error) {
      clearAuthToken(); // Importante limpar o token (especialmente o header do axios) em caso de falha
      console.error("Falha no login:", error);
      throw error; // Re-throw para o componente de UI tratar (ex: mostrar mensagem de erro)
    } finally {
      setIsLoading(false);
    }
  };

  const register = async (data: RegisterData) => {
    setIsLoading(true);
    try {
      await apiRegister(data);
    } catch (error) {
      console.error("Falha no cadastro:", error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const logout = (options?: { navigate?: (path: string, navigateOptions?: { replace?: boolean }) => void }) => {
    console.log("AuthContext: Logout chamado");
    setTokenState(null);
    setUserEmail(null);
    localStorage.removeItem('authToken');
    localStorage.removeItem('userEmail');
    clearAuthToken();

    if (options?.navigate) {
      console.log("AuthContext: Navegando para /login usando options.navigate");
      options.navigate('/login', { replace: true }); // Usa replace para não manter a página anterior no histórico
    } else {
      // Fallback se navigate não for passado. Isso é menos ideal porque
      // causa um refresh completo da página e perde o estado do React.
      // É melhor garantir que `navigate` seja sempre passado.
      console.warn("AuthContext: Função navigate não fornecida para logout. Usando window.location.href como fallback.");
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
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
      {!isLoading ? children : <div>Verificando autenticação...</div>}
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