import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { BrowserRouter } from 'react-router-dom'
import { MasgedBrandingProvider } from '@/contexts/MasgedBrandingContext'
import App from './App'
import './styles/index.css'

const queryClient = new QueryClient()

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <MasgedBrandingProvider>
        <BrowserRouter>
          <App />
        </BrowserRouter>
      </MasgedBrandingProvider>
    </QueryClientProvider>
  </StrictMode>,
)
