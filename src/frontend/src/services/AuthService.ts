import axios from 'axios';

// --- Tipos de Dados para as Requisições e Respostas ---

export interface LoginCredentials {
  email?: string;
  password?: string;
}

export interface ChangePasswordData {
  currentPassword?: string;
  newPassword?: string;
  confirmNewPassword?: string;
}

export interface RegisterData {
  email?: string;
  password?: string;
  confirmPassword?: string; // Usado apenas no frontend para validação
}

export interface ForgotPasswordData {
  email: string;
}

export interface ResetPasswordData {
  email: string;
  token: string;
  newPassword?: string;
  // confirmNewPassword não é enviado para a API, usado apenas no frontend
}

interface AuthResponse {
  token: string;
  expiration: string; // ISO string date
  email: string;
}

interface RegisterResponse {
  message: string;
  // Pode incluir outros dados se o backend retornar
}

interface SimpleMessageResponse {
  message: string;
  developmentOnlyLink?: string; // Para o link de dev no forgot password
  developmentOnlyToken?: string; // Se o backend retornasse o token diretamente (não é o caso aqui)
  errors?: string[]; // Para erros de validação do backend
}

// --- Configuração do Axios ---

// O proxy do vite.config.ts cuidará do prefixo do host para /login, /cadastro etc., em desenvolvimento.
// Em produção, o frontend será servido pelo mesmo host da API ou você configurará um proxy reverso.
const API_URL_PREFIX = ''; // Não precisa de prefixo aqui por causa do proxy do Vite

// Função para configurar o token JWT nos headers default do Axios
// Isso é chamado no AuthContext quando o token é obtido/carregado.
export const setAuthToken = (token: string | null) => {
  if (token) {
    axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  } else {
    delete axios.defaults.headers.common['Authorization'];
  }
};


// --- Funções de Serviço ---

export const login = async (credentials: LoginCredentials): Promise<AuthResponse> => {
  const response = await axios.post<AuthResponse>(`${API_URL_PREFIX}/login`, credentials);
  if (response.data.token) {
    setAuthToken(response.data.token); // Configura o token globalmente após o login
  }
  return response.data;
};

export const register = async (data: RegisterData): Promise<RegisterResponse> => {
  // O backend não precisa de confirmPassword
  const payload = { email: data.email, password: data.password };
  const response = await axios.post<RegisterResponse>(`${API_URL_PREFIX}/cadastro`, payload);
  return response.data;
};

export const forgotPassword = async (data: ForgotPasswordData): Promise<SimpleMessageResponse> => {
  const response = await axios.post<SimpleMessageResponse>(`${API_URL_PREFIX}/forgot-password`, data);
  return response.data;
};

export const resetPassword = async (data: ResetPasswordData): Promise<SimpleMessageResponse> => {
  // O backend não espera confirmNewPassword
  const payload = { email: data.email, token: data.token, newPassword: data.newPassword };
  const response = await axios.post<SimpleMessageResponse>(`${API_URL_PREFIX}/reset-password`, payload);
  return response.data;
};

// Chamado no logout no AuthContext
export const clearAuthToken = () => {
  setAuthToken(null);
};

export const changePassword = async (data: ChangePasswordData): Promise<SimpleMessageResponse> => {
  // O backend não espera confirmNewPassword
  const payload = { currentPassword: data.currentPassword, newPassword: data.newPassword };
  // O token JWT já deve estar nos headers do axios (configurado pelo setAuthToken)
  const response = await axios.post<SimpleMessageResponse>(`${API_URL_PREFIX}/change-password`, payload);
  return response.data;
};