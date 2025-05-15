import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/login': {
        target: 'https://localhost:5177', // SUA PORTA DA API .NET (HTTPS)
        changeOrigin: true,
        secure: false, // Necessário se a API usa certificado autoassinado em dev
      },
      '/cadastro': {
        target: 'https://localhost:5177',
        changeOrigin: true,
        secure: false,
      },
      '/forgot-password': {
        target: 'https://localhost:5177',
        changeOrigin: true,
        secure: false,
      },
      '/reset-password': {
        target: 'https://localhost:5177',
        changeOrigin: true,
        secure: false,
      },
      // Adicione aqui outras rotas de API que você criar no futuro
      // Ex: '/documents'
    }
  }
})