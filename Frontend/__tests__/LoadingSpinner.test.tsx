/**
 * @jest-environment jsdom
 */
import { render, screen } from '@testing-library/react';
import LoadingSpinner from '../app/components/LoadingSpinner';

describe('LoadingSpinner', () => {
  it('renders default message', () => {
    render(<LoadingSpinner />);
    expect(screen.getByText('Đang tải dữ liệu...')).toBeInTheDocument();
  });

  it('renders custom message', () => {
    render(<LoadingSpinner message="Loading tours..." />);
    expect(screen.getByText('Loading tours...')).toBeInTheDocument();
  });
});
