'use client';

import { useState, useEffect, useCallback } from 'react';
import Link from 'next/link';
import { getTours, deleteTour, updateTourStatus } from '../lib/api';
import type { Tour, PagedResponse } from '../lib/types';
import Pagination from '../components/Pagination';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorAlert from '../components/ErrorAlert';

export default function ToursPage() {
  const [data, setData] = useState<PagedResponse<Tour> | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 10;

  const fetchTours = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      const result = await getTours(pageNumber, pageSize);
      setData(result);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Không thể kết nối đến máy chủ.');
    } finally {
      setIsLoading(false);
    }
  }, [pageNumber]);

  useEffect(() => {
    fetchTours();
  }, [fetchTours]);

  const handleDelete = async (id: string, name: string) => {
    if (!confirm(`Bạn có chắc muốn xóa tour "${name}"?`)) return;
    try {
      await deleteTour(id);
      fetchTours();
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : 'Lỗi khi xóa tour.');
    }
  };

  const handleToggleStatus = async (id: string, currentStatus: boolean) => {
    try {
      await updateTourStatus(id, !currentStatus);
      fetchTours();
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : 'Lỗi khi cập nhật trạng thái.');
    }
  };

  if (isLoading) return <LoadingSpinner message="Đang tải danh sách tour..." />;
  if (error) return <ErrorAlert message={error} onRetry={fetchTours} />;

  return (
    <div>
      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-3xl font-bold text-slate-100">Quản lý Tour</h1>
          <p className="mt-1 text-sm text-slate-400">
            Tổng cộng {data?.totalCount || 0} tour trong hệ thống
          </p>
        </div>
        <Link
          href="/tours/create"
          className="inline-flex items-center gap-2 px-5 py-2.5 bg-gradient-to-r from-blue-500 to-cyan-500 text-white font-semibold rounded-xl shadow-lg shadow-blue-500/25 hover:shadow-blue-500/40 hover:scale-[1.02] transition-all duration-200 text-sm"
        >
          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Tạo Tour Mới
        </Link>
      </div>

      {/* Table */}
      {data && data.items.length > 0 ? (
        <div className="bg-slate-900/50 border border-slate-800 rounded-2xl overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b border-slate-800">
                  <th className="text-left px-6 py-4 text-xs font-semibold text-slate-400 uppercase tracking-wider">Tên Tour</th>
                  <th className="text-left px-6 py-4 text-xs font-semibold text-slate-400 uppercase tracking-wider">Thành phố</th>
                  <th className="text-right px-6 py-4 text-xs font-semibold text-slate-400 uppercase tracking-wider">Giá (VNĐ)</th>
                  <th className="text-center px-6 py-4 text-xs font-semibold text-slate-400 uppercase tracking-wider">Trạng thái</th>
                  <th className="text-center px-6 py-4 text-xs font-semibold text-slate-400 uppercase tracking-wider">Hành động</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800/50">
                {data.items.map((tour) => (
                  <tr key={tour.id} className="hover:bg-slate-800/30 transition-colors">
                    <td className="px-6 py-4">
                      <Link
                        href={`/tours/${tour.id}`}
                        className="text-slate-200 font-medium hover:text-blue-400 transition-colors"
                      >
                        {tour.name}
                      </Link>
                    </td>
                    <td className="px-6 py-4 text-slate-400 text-sm">{tour.city}</td>
                    <td className="px-6 py-4 text-right">
                      <span className="text-emerald-400 font-semibold text-sm">
                        {tour.price.toLocaleString('vi-VN')}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-center">
                      <button
                        onClick={() => handleToggleStatus(tour.id, tour.isActive)}
                        className={`inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-medium transition-colors cursor-pointer ${
                          tour.isActive
                            ? 'bg-emerald-500/15 text-emerald-400 hover:bg-emerald-500/25'
                            : 'bg-slate-500/15 text-slate-400 hover:bg-slate-500/25'
                        }`}
                      >
                        <span className={`w-1.5 h-1.5 rounded-full ${tour.isActive ? 'bg-emerald-400' : 'bg-slate-500'}`} />
                        {tour.isActive ? 'Hoạt động' : 'Ẩn'}
                      </button>
                    </td>
                    <td className="px-6 py-4 text-center">
                      <div className="flex items-center justify-center gap-2">
                        <Link
                          href={`/tours/${tour.id}`}
                          className="p-2 rounded-lg text-slate-400 hover:text-blue-400 hover:bg-blue-500/10 transition-all"
                          title="Xem chi tiết"
                        >
                          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                          </svg>
                        </Link>
                        <button
                          onClick={() => handleDelete(tour.id, tour.name)}
                          className="p-2 rounded-lg text-slate-400 hover:text-red-400 hover:bg-red-500/10 transition-all"
                          title="Xóa"
                        >
                          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                          </svg>
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          <div className="px-6 py-4 border-t border-slate-800">
            <Pagination
              pageNumber={data.pageNumber}
              totalPages={data.totalPages}
              hasPrevious={data.hasPrevious}
              hasNext={data.hasNext}
              onPageChange={setPageNumber}
            />
          </div>
        </div>
      ) : (
        <div className="text-center py-20 bg-slate-900/30 rounded-2xl border border-slate-800">
          <p className="text-slate-500 text-lg">Không có tour nào trong hệ thống.</p>
          <Link
            href="/tours/create"
            className="mt-4 inline-flex items-center gap-2 text-blue-400 hover:text-blue-300 text-sm font-medium"
          >
            Tạo tour đầu tiên →
          </Link>
        </div>
      )}
    </div>
  );
}
