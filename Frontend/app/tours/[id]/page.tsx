'use client';

import { useState, useEffect } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';
import { getTourById, deleteTour, updateTourStatus } from '../../lib/api';
import type { Tour } from '../../lib/types';
import LoadingSpinner from '../../components/LoadingSpinner';
import ErrorAlert from '../../components/ErrorAlert';

export default function TourDetailPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id as string;

  const [tour, setTour] = useState<Tour | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchTour = async () => {
      try {
        setIsLoading(true);
        const data = await getTourById(id);
        setTour(data);
      } catch (err: unknown) {
        setError(err instanceof Error ? err.message : 'Không thể tải thông tin tour.');
      } finally {
        setIsLoading(false);
      }
    };
    fetchTour();
  }, [id]);

  const handleDelete = async () => {
    if (!tour || !confirm(`Xóa tour "${tour.name}"?`)) return;
    try {
      await deleteTour(id);
      router.push('/tours');
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : 'Lỗi khi xóa tour.');
    }
  };

  const handleToggleStatus = async () => {
    if (!tour) return;
    try {
      await updateTourStatus(id, !tour.isActive);
      setTour({ ...tour, isActive: !tour.isActive });
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : 'Lỗi khi cập nhật trạng thái.');
    }
  };

  if (isLoading) return <LoadingSpinner message="Đang tải thông tin tour..." />;
  if (error) return <ErrorAlert message={error} />;
  if (!tour) return <ErrorAlert message="Không tìm thấy tour." />;

  return (
    <div className="max-w-3xl mx-auto">
      {/* Breadcrumb */}
      <nav className="flex items-center gap-2 text-sm text-slate-500 mb-6">
        <Link href="/tours" className="hover:text-slate-300 transition-colors">Quản lý Tour</Link>
        <span>→</span>
        <span className="text-slate-300">Chi tiết</span>
      </nav>

      <div className="bg-slate-900/50 border border-slate-800 rounded-2xl overflow-hidden">
        {/* Header */}
        <div className="px-8 py-6 border-b border-slate-800">
          <div className="flex items-start justify-between">
            <div>
              <h1 className="text-2xl font-bold text-slate-100">{tour.name}</h1>
              <div className="mt-2 flex items-center gap-3">
                <span className={`inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-medium ${
                  tour.isActive
                    ? 'bg-emerald-500/15 text-emerald-400'
                    : 'bg-slate-500/15 text-slate-400'
                }`}>
                  <span className={`w-1.5 h-1.5 rounded-full ${tour.isActive ? 'bg-emerald-400' : 'bg-slate-500'}`} />
                  {tour.isActive ? 'Hoạt động' : 'Ẩn'}
                </span>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <button
                onClick={handleToggleStatus}
                className="px-4 py-2 rounded-lg text-sm font-medium bg-slate-800 text-slate-300 hover:bg-slate-700 border border-slate-700 transition-colors"
              >
                {tour.isActive ? 'Ẩn Tour' : 'Kích hoạt'}
              </button>
              <button
                onClick={handleDelete}
                className="px-4 py-2 rounded-lg text-sm font-medium bg-red-500/10 text-red-400 hover:bg-red-500/20 border border-red-500/20 transition-colors"
              >
                Xóa
              </button>
            </div>
          </div>
        </div>

        {/* Info */}
        <div className="px-8 py-6 space-y-5">
          {tour.description && (
            <div>
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Mô tả</label>
              <p className="mt-1 text-slate-300">{tour.description}</p>
            </div>
          )}

          <div className="grid grid-cols-2 gap-6">
            <div>
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Thành phố</label>
              <p className="mt-1 text-slate-200 font-medium">{tour.city}</p>
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Giá tour</label>
              <p className="mt-1 text-emerald-400 text-xl font-bold">{tour.price.toLocaleString('vi-VN')} VNĐ</p>
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Ngày tạo</label>
              <p className="mt-1 text-slate-400 text-sm">{new Date(tour.createdAt).toLocaleDateString('vi-VN')}</p>
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Cập nhật lần cuối</label>
              <p className="mt-1 text-slate-400 text-sm">{new Date(tour.updatedAt).toLocaleDateString('vi-VN')}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
