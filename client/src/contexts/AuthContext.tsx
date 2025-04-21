'use client';

import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';

// Define user type
export interface User {
  id: string;
  username: string;
  email: string;
}

// Define auth context type
interface AuthContextType {
  user: User | null;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<boolean>;
  register: (username: string, email: string, password: string) => Promise<boolean>;
  logout: () => void;
  isAuthenticated: boolean;
  error: string | null;
  setError: (error: string | null) => void;
}

// Create the auth context
const AuthContext = createContext<AuthContextType | undefined>(undefined);

// AuthProvider props
interface AuthProviderProps {
  children: ReactNode;
}

// Auth Provider component
export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Check for existing session on initial load
  useEffect(() => {
    const checkAuth = async () => {
      try {
        // In a real app, this would be an API call to check session
        const storedUser = localStorage.getItem('pixel_canvas_user');
        if (storedUser) {
          setUser(JSON.parse(storedUser));
        }
      } catch (err) {
        console.error('Failed to restore authentication', err);
      } finally {
        setIsLoading(false);
      }
    };

    checkAuth();
  }, []);

  // Mock login function
  const login = async (email: string, password: string) => {
    setIsLoading(true);
    setError(null);
    
    try {
      // In a real app, this would be an API call
      // Simulate API delay
      await new Promise(resolve => setTimeout(resolve, 800));
      
      // Simple validation (mocked)
      if (!email || !password) {
        setError('Email and password are required');
        return false;
      }
      
      if (password.length < 6) {
        setError('Password must be at least 6 characters');
        return false;
      }
      
      // Create a mock user (in a real app this would come from the server)
      const mockUser: User = {
        id: Math.random().toString(36).substring(2, 15),
        username: email.split('@')[0],
        email
      };
      
      // Save user to localStorage (for demo purposes)
      localStorage.setItem('pixel_canvas_user', JSON.stringify(mockUser));
      
      // Update state
      setUser(mockUser);
      return true;
    } catch (err) {
      setError('Failed to login. Please try again.');
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  // Mock register function
  const register = async (username: string, email: string, password: string) => {
    setIsLoading(true);
    setError(null);
    
    try {
      // In a real app, this would be an API call
      // Simulate API delay
      await new Promise(resolve => setTimeout(resolve, 800));
      
      // Simple validation (mocked)
      if (!username || !email || !password) {
        setError('All fields are required');
        return false;
      }
      
      if (password.length < 6) {
        setError('Password must be at least 6 characters');
        return false;
      }
      
      // Create a mock user (in a real app this would come from the server)
      const mockUser: User = {
        id: Math.random().toString(36).substring(2, 15),
        username,
        email
      };
      
      // Save user to localStorage (for demo purposes)
      localStorage.setItem('pixel_canvas_user', JSON.stringify(mockUser));
      
      // Update state
      setUser(mockUser);
      return true;
    } catch (err) {
      setError('Failed to register. Please try again.');
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  // Logout function
  const logout = () => {
    localStorage.removeItem('pixel_canvas_user');
    setUser(null);
  };

  const value = {
    user,
    isLoading,
    login,
    register,
    logout,
    isAuthenticated: !!user,
    error,
    setError
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

// Custom hook to use auth context
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};