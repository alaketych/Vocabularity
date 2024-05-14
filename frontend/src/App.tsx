import Menu from './components/Menu'
import { Subscription } from './pages/_index'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import './App.css'


function App() {
  return (
    <BrowserRouter>
      <Menu />
      <Routes>
          <Route path="/subscription" element={<Subscription />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
