'use client';

import { useState, useEffect, useCallback } from 'react';
import Link from 'next/link';
import { getRequests, deleteRequest, approveRequest } from '../lib/api';
import type { BookingRequest, PagedResponse } from '../lib/types';
import Pagination from '../components/Pagination';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorAlert from '../components/ErrorAlert';

const statusConfig: Record<string, { label: string; class: string }> = {
  Received: {
    label: 'Đã tiếp nhận',
    class: 'bg-blue-500/15 text-blue-400',
  },
  PendingApproval: {
    label: 'Chờ duyệt quản lý',
    class: 'bg-amber-500/15 text-amber-400',
  },
  Approved: {
    label: 'Đã phê duyệt',
    class: 'bg-emerald-500/15 text-emerald-400',
  },
};

export default function RequestsPage() {
  const [data, setData] = useState<PagedResponse<BookingRequest> | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 10;

  const fetchRequests = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      const result = await getRequests(pageNumber, pageSize);
      setData(result);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Không thể kết nối đến máy chủ.');
    } finally {
      setIsLoading(false);
    }
  }, [pageNumber]);

  useEffect(() => {
    fetchRequests();
  }, [fetchRequests]);

  const handleDelete = async (id: string) => {
    if (!confirm('Bạn có chắc muốn xóa phiếu đề nghị này?')) return;
    try {
      await deleteRequest(id);
      fetchRequests();
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : 'Lỗi khi xóa phiếu.');
    }
  };

  const handleApprove = async (id: string) => {
    if (!confirm('Xác nhận phê duyệt phiếu này?')) return;
    try {
      await approveRequest(id);
      fetchRequests();
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : 'Lỗi khi phê duyệt.');
    }
  };

  if (isLoading) return <LoadingSpinner message="Đang tải danh sách phiếu đề nghị..." />;
  if (error) return <ErrorAlert message={error} onRetry={fetchRequests} />;

  return (
    <div>
      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-3xl font-bold text-slate-100">Phiếu Đề Nghị Đặt Dịch Vụ</h1>
          <p className="mt-1 text-sm text-slate-400">
            Tổng cộng {data?.totalCount || 0} phiếu trong hệ thống
          </p>
        </div>
        <Link
          href="/requests/create"
          className="inline-flex items-center gap-2 px-5 py-2.5 bg-gradient-to-r from-blue-500 to-cyan-500 text-white font-semibold rounded-xl shadow-lg shadow-blue-500/25 hover:shadow-blue-500/40 hover:scale-[1.02] transition-all duration-200 text-sm"
        >
          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Tạo Phiếu Mới
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
                  <th className="text-center px-6 py-4 text-xs font-semibold text-slate-400 uppercase tracking-wider">Loại</th>
                  <th className="text-center px-6 py-4 text-xs font-semibold text-slate-400 uppercase tracking-wider">Số khách</th>
                  <th className="text-right px-6 py-4 text-xs font-semibold text-slate-400 uppercase tracking-wider">Tổng tiền (VNĐ)</th>
                  <th className="text-center px-6 py-4 text-xs font-semibold text-slate-400 uppercase tracking-wider">Trạng thái</th>
                  <th className="text-center px-6 py-4 text-xs font-semibold text-slate-400 uppercase tracking-wider">Hành động</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800/50">
                {data.items.map((req) => {
                  const status = statusConfig[req.status] || statusConfig.Received;
                  const isApproved = req.status === 'Approved';

                  return (
                    <tr key={req.id} className="hover:bg-slate-800/30 transition-colors">
                      <td className="px-6 py-4">
                        <Link
                          href={`/requests/${req.id}`}
                          className="text-slate-200 font-medium hover:text-blue-400 transition-colors"
                        >
                          {req.tourName}
                        </Link>
                        <div className="text-xs text-slate-500 mt-0.5">
                          {req.personInCharge} • {new Date(req.departureDate).toLocaleDateString('vi-VN')}
                        </div>
                      </td>
                      <td className="px-6 py-4 text-center">
                        <span className="text-xs font-medium text-slate-300 bg-slate-800 px-2.5 py-1 rounded-md">
                          {req.tourType}
                        </span>
                      </td>
                      <td className="px-6 py-4 text-center text-slate-400 text-sm">{req.guestCount}</td>
                      <td className="px-6 py-4 text-right">
                        <span className="text-emerald-400 font-semibold text-sm">
                          {req.totalAmount.toLocaleString('vi-VN')}
                        </span>
                      </td>
                      <td className="px-6 py-4 text-center">
                        <span className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-medium ${status.class}`}>
                          {status.label}
                        </span>
                      </td>
                      <td className="px-6 py-4 text-center">
                        <div className="flex items-center justify-center gap-1">
                          <Link
                            href={`/requests/${req.id}`}
                            className="p-2 rounded-lg text-slate-400 hover:text-blue-400 hover:bg-blue-500/10 transition-all"
                            title="Xem chi tiết"
                          >
                            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                            </svg>
                          </Link>
                          {!isApproved && (
                            <>
                              <button
                                onClick={() => handleApprove(req.id)}
                                className="p-2 rounded-lg text-slate-400 hover:text-emerald-400 hover:bg-emerald-500/10 transition-all"
                                title="Phê duyệt"
                              >
                                <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                </svg>
                              </button>
                              <button
                                onClick={() => handleDelete(req.id)}
                                className="p-2 rounded-lg text-slate-400 hover:text-red-400 hover:bg-red-500/10 transition-all"
                                title="Xóa"
                              >
                                <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                </svg>
                              </button>
                            </>
                          )}
                          {isApproved && (
                            <span className="p-2 text-slate-600 cursor-not-allowed" title="Phiếu đã duyệt - không thể chỉnh sửa">
                              <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                              </svg>
                            </span>
                          )}
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>

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
          <p className="text-slate-500 text-lg">Chưa có phiếu đề nghị nào.</p>
          <Link
            href="/requests/create"
            className="mt-4 inline-flex items-center gap-2 text-blue-400 hover:text-blue-300 text-sm font-medium"
          >
            Tạo phiếu đầu tiên →
          </Link>
        </div>
      )}
    </div>
  );
}
