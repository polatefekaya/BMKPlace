'use client';

import React, { useState } from 'react';
import Modal from '../UI/Modal';
import { LoginForm, RegisterForm } from './AuthForms';

type AuthModalView = 'login' | 'register';

interface AuthModalProps {
  isOpen: boolean;
  onClose: () => void;
  initialView?: AuthModalView;
}

const AuthModal: React.FC<AuthModalProps> = ({
  isOpen,
  onClose,
  initialView = 'login'
}) => {
  const [currentView, setCurrentView] = useState<AuthModalView>(initialView);
  
  const handleSuccess = () => {
    onClose();
  };
  
  const switchToLogin = () => setCurrentView('login');
  const switchToRegister = () => setCurrentView('register');
  
  const title = currentView === 'login' ? 'Sign In' : 'Create Account';
  
  return (
    <Modal isOpen={isOpen} onClose={onClose} title={title}>
      {currentView === 'login' ? (
        <LoginForm onSuccess={handleSuccess} switchToRegister={switchToRegister} />
      ) : (
        <RegisterForm onSuccess={handleSuccess} switchToLogin={switchToLogin} />
      )}
    </Modal>
  );
};

export default AuthModal;