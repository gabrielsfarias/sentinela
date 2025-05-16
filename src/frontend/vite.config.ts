import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/login': {
        target: 'https://localhost:7292',
        changeOrigin: true,
        secure: false,
      },
      '/cadastro': {
        target: 'https://localhost:7292',
        changeOrigin: true,
        secure: false,
      },
      '/forgot-password': {
        target: 'https://localhost:7292',
        changeOrigin: true,
        secure: false,
      },
      '/reset-password': {
        target: 'https://localhost:7292',
        changeOrigin: true,
        secure: false,
      },
      '/change-password': {
        target: 'https://localhost:7292',
        changeOrigin: true,
        secure: false,
      }
      // Adicione aqui outras rotas de API que você criar no futuro
    }
  }
})