'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useState, useEffect } from 'react';
import { isLoggedIn, getUsername, logout } from '../lib/api';

const navItems = [
  { href: '/', label: 'Trang chủ' },
  { href: '/tours', label: 'Quản lý Tour' },
  { href: '/requests', label: 'Phiếu đề nghị' },
];

export default function Navbar() {
  const pathname = usePathname();
  const [username, setUsername] = useState<string | null>(null);
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
    setUsername(getUsername());
  }, [pathname]);

  // Don't show navbar on login page
  if (pathname === '/login') return null;

  return (
    <nav className="sticky top-0 z-50 backdrop-blur-xl bg-slate-900/80 border-b border-slate-700/50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <Link href="/" className="flex items-center gap-3 group">
            <div className="w-9 h-9 rounded-lg bg-gradient-to-br from-blue-500 to-cyan-400 flex items-center justify-center text-white font-bold text-sm shadow-lg shadow-blue-500/25 group-hover:shadow-blue-500/40 transition-shadow">
              VT
            </div>
            <span className="text-lg font-bold bg-gradient-to-r from-blue-400 to-cyan-300 bg-clip-text text-transparent">
              Vietravel
            </span>
          </Link>

          {/* Nav Links */}
          <div className="flex items-center gap-1">
            {navItems.map((item) => {
              const isActive = item.href === '/'
                ? pathname === '/'
                : pathname.startsWith(item.href);

              return (
                <Link
                  key={item.href}
                  href={item.href}
                  className={`px-4 py-2 rounded-lg text-sm font-medium transition-all duration-200 ${
                    isActive
                      ? 'bg-blue-500/15 text-blue-400 shadow-inner'
                      : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/60'
                  }`}
                >
                  {item.label}
                </Link>
              );
            })}
          </div>

          {/* User info */}
          <div className="flex items-center gap-3">
            {mounted && username ? (
              <>
                <span className="text-sm text-slate-400">
                  Xin chào, <span className="text-slate-200 font-medium">{username}</span>
                </span>
                <button
                  onClick={logout}
                  className="px-3 py-1.5 rounded-lg text-xs font-medium text-slate-400 hover:text-red-400 hover:bg-red-500/10 border border-slate-700 hover:border-red-500/30 transition-all"
                >
                  Đăng xuất
                </button>
              </>
            ) : mounted ? (
              <Link
                href="/login"
                className="px-4 py-1.5 rounded-lg text-sm font-medium bg-blue-500/15 text-blue-400 hover:bg-blue-500/25 transition-colors"
              >
                Đăng nhập
              </Link>
            ) : null}
          </div>
        </div>
      </div>
    </nav>
  );
}
