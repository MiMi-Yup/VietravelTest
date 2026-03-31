'use client';

interface PaginationProps {
  pageNumber: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
  onPageChange: (page: number) => void;
}

export default function Pagination({
  pageNumber,
  totalPages,
  hasPrevious,
  hasNext,
  onPageChange,
}: PaginationProps) {
  if (totalPages <= 1) return null;

  const getPageNumbers = () => {
    const pages: (number | '...')[] = [];
    const delta = 2;

    for (let i = 1; i <= totalPages; i++) {
      if (
        i === 1 ||
        i === totalPages ||
        (i >= pageNumber - delta && i <= pageNumber + delta)
      ) {
        pages.push(i);
      } else if (pages[pages.length - 1] !== '...') {
        pages.push('...');
      }
    }

    return pages;
  };

  return (
    <div className="flex items-center justify-center gap-2 mt-8">
      <button
        onClick={() => onPageChange(pageNumber - 1)}
        disabled={!hasPrevious}
        className="px-3 py-2 rounded-lg text-sm font-medium transition-all duration-200 disabled:opacity-30 disabled:cursor-not-allowed bg-slate-800 text-slate-300 hover:bg-slate-700 border border-slate-700"
      >
        ← Trước
      </button>

      {getPageNumbers().map((page, idx) =>
        page === '...' ? (
          <span key={`dots-${idx}`} className="px-2 text-slate-500">
            …
          </span>
        ) : (
          <button
            key={page}
            onClick={() => onPageChange(page)}
            className={`w-10 h-10 rounded-lg text-sm font-medium transition-all duration-200 ${
              page === pageNumber
                ? 'bg-blue-500 text-white shadow-lg shadow-blue-500/25'
                : 'bg-slate-800 text-slate-400 hover:bg-slate-700 border border-slate-700'
            }`}
          >
            {page}
          </button>
        )
      )}

      <button
        onClick={() => onPageChange(pageNumber + 1)}
        disabled={!hasNext}
        className="px-3 py-2 rounded-lg text-sm font-medium transition-all duration-200 disabled:opacity-30 disabled:cursor-not-allowed bg-slate-800 text-slate-300 hover:bg-slate-700 border border-slate-700"
      >
        Sau →
      </button>
    </div>
  );
}
