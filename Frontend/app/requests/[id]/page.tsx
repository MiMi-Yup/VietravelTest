'use client';

import { useState, useEffect } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';
import { getRequestById, deleteRequest, approveRequest } from '../../lib/api';
import type { BookingRequest } from '../../lib/types';
import LoadingSpinner from '../../components/LoadingSpinner';
import ErrorAlert from '../../components/ErrorAlert';

const statusConfig: Record<string, { label: string; class: string; dotClass: string }> = {
  Received: {
    label: 'Đã tiếp nhận',
    class: 'bg-blue-500/15 text-blue-400 border-blue-500/20',
    dotClass: 'bg-blue-400',
  },
  PendingApproval: {
    label: 'Chờ duyệt quản lý',
    class: 'bg-amber-500/15 text-amber-400 border-amber-500/20',
    dotClass: 'bg-amber-400',
  },
  Approved: {
    label: 'Đã phê duyệt',
    class: 'bg-emerald-500/15 text-emerald-400 border-emerald-500/20',
    dotClass: 'bg-emerald-400',
  },
};

export default function RequestDetailPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id as string;

  const [request, setRequest] = useState<BookingRequest | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetch = async () => {
      try {
        setIsLoading(true);
        const data = await getRequestById(id);
        setRequest(data);
      } catch (err: unknown) {
        setError(err instanceof Error ? err.message : 'Không thể tải phiếu đề nghị.');
      } finally {
        setIsLoading(false);
      }
    };
    fetch();
  }, [id]);

  const handleDelete = async () => {
    if (!confirm('Xóa phiếu đề nghị này?')) return;
    try {
      await deleteRequest(id);
      router.push('/requests');
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : 'Lỗi khi xóa.');
    }
  };

  const handleApprove = async () => {
    if (!confirm('Xác nhận phê duyệt phiếu?')) return;
    try {
      const approved = await approveRequest(id);
      setRequest(approved);
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : 'Lỗi khi phê duyệt.');
    }
  };

  if (isLoading) return <LoadingSpinner message="Đang tải phiếu đề nghị..." />;
  if (error) return <ErrorAlert message={error} />;
  if (!request) return <ErrorAlert message="Không tìm thấy phiếu đề nghị." />;

  const status = statusConfig[request.status] || statusConfig.Received;
  const isApproved = request.status === 'Approved';
  const showMiceWarning = request.tourType === 'MICE' && request.guestCount < 10;

  return (
    <div className="max-w-4xl mx-auto">
      {/* Breadcrumb */}
      <nav className="flex items-center gap-2 text-sm text-slate-500 mb-6">
        <Link href="/requests" className="hover:text-slate-300 transition-colors">Phiếu đề nghị</Link>
        <span>→</span>
        <span className="text-slate-300">Chi tiết</span>
      </nav>

      {/* MICE Warning */}
      {showMiceWarning && (
        <div className="mb-6 p-4 bg-amber-500/10 border border-amber-500/20 rounded-xl flex items-start gap-3">
          <svg className="w-5 h-5 text-amber-400 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.082 16.5c-.77.833.192 2.5 1.732 2.5z" />
          </svg>
          <div>
            <span className="font-semibold text-amber-400 text-sm">Cảnh báo MICE!</span>
            <p className="text-amber-300/80 text-sm mt-0.5">
              Tour MICE với chỉ {request.guestCount} khách (khuyến nghị tối thiểu 10 khách).
            </p>
          </div>
        </div>
      )}

      {/* Header Card */}
      <div className="bg-slate-900/50 border border-slate-800 rounded-2xl overflow-hidden mb-6">
        <div className="px-8 py-6 border-b border-slate-800">
          <div className="flex items-start justify-between">
            <div>
              <h1 className="text-2xl font-bold text-slate-100">{request.tourName}</h1>
              <div className="mt-3 flex items-center gap-3">
                <span className={`inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-medium border ${status.class}`}>
                  <span className={`w-1.5 h-1.5 rounded-full ${status.dotClass}`} />
                  {status.label}
                </span>
                <span className="text-xs font-medium text-slate-300 bg-slate-800 px-2.5 py-1 rounded-md">
                  {request.tourType}
                </span>
              </div>
            </div>
            <div className="flex items-center gap-2">
              {!isApproved && (
                <>
                  <button
                    onClick={handleApprove}
                    className="px-4 py-2 rounded-lg text-sm font-medium bg-emerald-500/10 text-emerald-400 hover:bg-emerald-500/20 border border-emerald-500/20 transition-colors"
                  >
                    Phê duyệt
                  </button>
                  <button
                    onClick={handleDelete}
                    className="px-4 py-2 rounded-lg text-sm font-medium bg-red-500/10 text-red-400 hover:bg-red-500/20 border border-red-500/20 transition-colors"
                  >
                    Xóa
                  </button>
                </>
              )}
              {isApproved && (
                <div className="flex items-center gap-2 px-4 py-2 bg-slate-800 rounded-lg text-slate-500 text-sm">
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                  </svg>
                  Đã khóa
                </div>
              )}
            </div>
          </div>
        </div>

        {/* General Info */}
        <div className="px-8 py-6">
          <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
            <div>
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Ngày khởi hành</label>
              <p className="mt-1 text-slate-200 font-medium">{new Date(request.departureDate).toLocaleDateString('vi-VN')}</p>
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Người phụ trách</label>
              <p className="mt-1 text-slate-200 font-medium">{request.personInCharge}</p>
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Số khách</label>
              <p className="mt-1 text-slate-200 font-medium">{request.guestCount}</p>
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Tổng chi phí</label>
              <p className="mt-1 text-emerald-400 text-xl font-bold">{request.totalAmount.toLocaleString('vi-VN')} <span className="text-sm text-slate-500">VNĐ</span></p>
            </div>
          </div>
        </div>
      </div>

      {/* Service Details Table */}
      <div className="bg-slate-900/50 border border-slate-800 rounded-2xl overflow-hidden">
        <div className="px-8 py-5 border-b border-slate-800">
          <h2 className="text-lg font-bold text-slate-100">Chi tiết dịch vụ ({request.details.length})</h2>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-slate-800">
                <th className="text-left px-6 py-3 text-xs font-semibold text-slate-400 uppercase tracking-wider">#</th>
                <th className="text-left px-6 py-3 text-xs font-semibold text-slate-400 uppercase tracking-wider">Loại DV</th>
                <th className="text-left px-6 py-3 text-xs font-semibold text-slate-400 uppercase tracking-wider">Tên DV</th>
                <th className="text-left px-6 py-3 text-xs font-semibold text-slate-400 uppercase tracking-wider">NCC</th>
                <th className="text-right px-6 py-3 text-xs font-semibold text-slate-400 uppercase tracking-wider">SL</th>
                <th className="text-right px-6 py-3 text-xs font-semibold text-slate-400 uppercase tracking-wider">Đơn giá</th>
                <th className="text-right px-6 py-3 text-xs font-semibold text-slate-400 uppercase tracking-wider">Thành tiền</th>
                <th className="text-left px-6 py-3 text-xs font-semibold text-slate-400 uppercase tracking-wider">Ghi chú</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-800/50">
              {request.details.map((d, i) => (
                <tr key={d.id} className="hover:bg-slate-800/30 transition-colors">
                  <td className="px-6 py-3 text-sm text-slate-500">{i + 1}</td>
                  <td className="px-6 py-3 text-sm text-slate-300">{d.serviceType}</td>
                  <td className="px-6 py-3 text-sm text-slate-200 font-medium">{d.serviceName}</td>
                  <td className="px-6 py-3 text-sm text-slate-400">{d.supplier}</td>
                  <td className="px-6 py-3 text-sm text-slate-300 text-right">{d.quantity}</td>
                  <td className="px-6 py-3 text-sm text-slate-300 text-right">{d.unitPrice.toLocaleString('vi-VN')}</td>
                  <td className="px-6 py-3 text-sm text-emerald-400 font-semibold text-right">{d.lineTotal.toLocaleString('vi-VN')}</td>
                  <td className="px-6 py-3 text-sm text-slate-500">{d.note || '—'}</td>
                </tr>
              ))}
            </tbody>
            <tfoot>
              <tr className="border-t border-slate-700">
                <td colSpan={6} className="px-6 py-4 text-right text-sm font-semibold text-slate-300 uppercase">Tổng cộng</td>
                <td className="px-6 py-4 text-right text-lg font-bold text-emerald-400">{request.totalAmount.toLocaleString('vi-VN')}</td>
                <td></td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>
    </div>
  );
}
