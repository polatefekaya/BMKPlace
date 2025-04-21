'use client';

import React, { useState } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import AuthModal from '../Auth/AuthModal';

const TopNav: React.FC = () => {
  const { user, logout, isAuthenticated } = useAuth();
  const [isAuthModalOpen, setIsAuthModalOpen] = useState(false);
  const [initialAuthView, setInitialAuthView] = useState<'login' | 'register'>('login');
  
  const openLoginModal = () => {
    setInitialAuthView('login');
    setIsAuthModalOpen(true);
  };
  
  const openRegisterModal = () => {
    setInitialAuthView('register');
    setIsAuthModalOpen(true);
  };
  
  const closeAuthModal = () => {
    setIsAuthModalOpen(false);
  };
  
  const handleLogout = () => {
    logout();
  };
  
  return (
    <>
      <nav className="fixed top-0 left-0 right-0 h-[60px] sm:h-[50px] flex justify-between items-center px-4 sm:px-3 bg-white shadow-md z-10">
        <div className="flex items-center">
          <span className="text-xl sm:text-lg font-semibold text-blue-600">Pixel Canvas</span>
        </div>
        
        <div className="flex items-center">
          {isAuthenticated ? (
            <div className="flex items-center gap-3">
              <span className="font-medium text-gray-600 sm:text-sm sm:max-w-[80px] overflow-hidden text-ellipsis whitespace-nowrap">
                {user?.username || 'User'}
              </span>
              <button 
                onClick={handleLogout}
                className="py-2 px-4 sm:py-1.5 sm:px-3 rounded-md text-sm sm:text-xs font-medium bg-transparent border border-gray-200 text-gray-600 hover:bg-gray-50 transition-all"
              >
                Sign Out
              </button>
            </div>
          ) : (
            <div className="flex gap-2">
              <button 
                onClick={openLoginModal}
                className="py-2 px-4 sm:py-1.5 sm:px-3 rounded-md text-sm sm:text-xs font-medium bg-transparent border border-blue-500 text-blue-500 hover:bg-blue-50 transition-all"
              >
                Sign In
              </button>
              <button 
                onClick={openRegisterModal}
                className="py-2 px-4 sm:py-1.5 sm:px-3 rounded-md text-sm sm:text-xs font-medium bg-blue-500 border border-blue-500 text-white hover:bg-blue-600 hover:border-blue-600 transition-all"
              >
                Register
              </button>
            </div>
          )}
        </div>
      </nav>
      
      <AuthModal
        isOpen={isAuthModalOpen}
        onClose={closeAuthModal}
        initialView={initialAuthView}
      />
    </>
  );
};

export default TopNav;