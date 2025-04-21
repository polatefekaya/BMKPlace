import './globals.css';
import { Metadata, Viewport } from 'next';
import { AuthProvider } from '@/contexts/AuthContext';
import TopNav from '@/components/Layout/TopNav';

export const viewport: Viewport = {
  width: 'device-width', 
  initialScale: 1,
  maximumScale: 1,
  userScalable: false,
  // The following helps with mobile performance
  themeColor: '#f0f0f0',
  interactiveWidget: 'resizes-visual',
};

export const metadata: Metadata = {
  title: 'Collaborative Pixel Canvas',
  description: 'A mobile-first collaborative pixel canvas similar to r/place',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>
        <AuthProvider>
          <TopNav />
          <main className="pt-[60px]">
            {children}
          </main>
        </AuthProvider>
      </body>
    </html>
  );
}
