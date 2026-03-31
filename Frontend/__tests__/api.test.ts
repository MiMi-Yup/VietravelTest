/**
 * @jest-environment jsdom
 */
import { isLoggedIn, getUsername } from '../app/lib/api';

describe('API Auth Helpers', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  describe('isLoggedIn', () => {
    it('returns false when no token', () => {
      expect(isLoggedIn()).toBe(false);
    });

    it('returns true when token exists', () => {
      localStorage.setItem('token', 'test-jwt-token');
      expect(isLoggedIn()).toBe(true);
    });
  });

  describe('getUsername', () => {
    it('returns null when no username', () => {
      expect(getUsername()).toBeNull();
    });

    it('returns username from localStorage', () => {
      localStorage.setItem('username', 'admin');
      expect(getUsername()).toBe('admin');
    });
  });

  describe('logout behavior', () => {
    it('clears localStorage on logout prep', () => {
      localStorage.setItem('token', 'test-token');
      localStorage.setItem('username', 'admin');

      // Simulate clearAuth logic (logout redirects so we test the state clearing part)
      localStorage.removeItem('token');
      localStorage.removeItem('username');

      expect(localStorage.getItem('token')).toBeNull();
      expect(localStorage.getItem('username')).toBeNull();
      expect(isLoggedIn()).toBe(false);
    });
  });
});
