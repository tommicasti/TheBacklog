import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { useSelector } from 'react-redux';
import HomePage from './pages/HomePage';
import NotFound from './pages/NotFound';
import SignInAndSignUp from './pages/SignInAndSignUp';
import SearchPage from './pages/SearchPage';
import MyAccountPage from './pages/MyAccountPage';
import { MobileProvider } from './context/MobileContext';

function App() {
  const isAuthenticated = useSelector((state: any) => state.authToken.isAuthenticated);

  return (
    <MobileProvider>
      <Router>
        <Routes>
          <Route path='*' element={<NotFound />} />
          <Route path='/' element={<HomePage />} />
          <Route path='/signup' element={isAuthenticated ? <MyAccountPage /> : <SignInAndSignUp />} />
          <Route path='/search' element={<SearchPage />} />
          <Route path='/my-account' element={isAuthenticated ? <MyAccountPage /> : <SignInAndSignUp />} />
        </Routes>
      </Router>
    </MobileProvider>
  );
}

export default App;
