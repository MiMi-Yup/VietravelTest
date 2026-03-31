/**
 * @jest-environment jsdom
 */
import { render, screen, fireEvent } from '@testing-library/react';
import Pagination from '../app/components/Pagination';

describe('Pagination', () => {
  const defaultProps = {
    pageNumber: 2,
    totalPages: 5,
    hasPrevious: true,
    hasNext: true,
    onPageChange: jest.fn(),
  };

  beforeEach(() => {
    defaultProps.onPageChange = jest.fn();
  });

  it('renders page buttons', () => {
    render(<Pagination {...defaultProps} />);
    expect(screen.getByText('1')).toBeInTheDocument();
    expect(screen.getByText('2')).toBeInTheDocument();
    expect(screen.getByText('5')).toBeInTheDocument();
  });

  it('calls onPageChange when clicking a page', () => {
    render(<Pagination {...defaultProps} />);
    fireEvent.click(screen.getByText('3'));
    expect(defaultProps.onPageChange).toHaveBeenCalledWith(3);
  });

  it('calls onPageChange for previous button', () => {
    render(<Pagination {...defaultProps} />);
    fireEvent.click(screen.getByText('← Trước'));
    expect(defaultProps.onPageChange).toHaveBeenCalledWith(1);
  });

  it('calls onPageChange for next button', () => {
    render(<Pagination {...defaultProps} />);
    fireEvent.click(screen.getByText('Sau →'));
    expect(defaultProps.onPageChange).toHaveBeenCalledWith(3);
  });

  it('disables previous on first page', () => {
    render(
      <Pagination {...defaultProps} pageNumber={1} hasPrevious={false} />
    );
    const prevButton = screen.getByText('← Trước');
    expect(prevButton).toBeDisabled();
  });

  it('disables next on last page', () => {
    render(
      <Pagination {...defaultProps} pageNumber={5} hasNext={false} />
    );
    const nextButton = screen.getByText('Sau →');
    expect(nextButton).toBeDisabled();
  });

  it('returns null when only one page', () => {
    const { container } = render(
      <Pagination {...defaultProps} totalPages={1} />
    );
    expect(container.innerHTML).toBe('');
  });
});
