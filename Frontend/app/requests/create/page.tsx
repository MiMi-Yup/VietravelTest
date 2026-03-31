'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { createRequest } from '../../lib/api';
import type { CreateRequestDetailDto } from '../../lib/types';

const emptyDetail = (): CreateRequestDetailDto => ({
  serviceType: '',
  serviceName: '',
  supplier: '',
  quantity: 1,
  unitPrice: 0,
  note: '',
});

export default function CreateRequestPage() {
  const router = useRouter();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [form, setForm] = useState({
    tourName: '',
    departureDate: '',
    personInCharge: '',
    tourType: 'FIT' as 'FIT' | 'GIT' | 'MICE',
    guestCount: 1,
  });

  const [details, setDetails] = useState<CreateRequestDetailDto[]>([emptyDetail()]);

  // MICE warning logic
  const showMiceWarning = form.tourType === 'MICE' && form.guestCount < 10;

  // Calculations
  const lineTotals = details.map((d) => d.quantity * d.unitPrice);
  const grandTotal = lineTotals.reduce((sum, lt) => sum + lt, 0);

  const addDetail = () => setDetails([...details, emptyDetail()]);

  const removeDetail = (index: number) => {
    if (details.length <= 1) return;
    setDetails(details.filter((_, i) => i !== index));
  };

  const updateDetail = (index: number, field: keyof CreateRequestDetailDto, value: string | number) => {
    setDetails(details.map((d, i) => i === index ? { ...d, [field]: value } : d));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    // Client-side validation
    if (!form.tourName.trim() || !form.departureDate || !form.personInCharge.trim()) {
      setError('Vui lòng điền đầy đủ thông tin chung.');
      return;
    }
    if (form.guestCount < 1) {
      setError('Số lượng khách phải lớn hơn 0.');
      return;
    }

    const invalidDetail = details.find(
      (d) => !d.serviceType.trim() || !d.serviceName.trim() || !d.supplier.trim() || d.quantity < 1 || d.unitPrice <= 0
    );
    if (invalidDetail) {
      setError('Vui lòng điền đầy đủ và hợp lệ thông tin cho tất cả dịch vụ.');
      return;
    }

    try {
      setIsSubmitting(true);
      await createRequest({
        tourName: form.tourName.trim(),
        departureDate: new Date(form.departureDate).toISOString(),
        personInCharge: form.personInCharge.trim(),
        tourType: form.tourType,
        guestCount: form.guestCount,
        details: details.map((d) => ({
          ...d,
          serviceType: d.serviceType.trim(),
          serviceName: d.serviceName.trim(),
          supplier: d.supplier.trim(),
          note: d.note?.trim() || undefined,
        })),
      });
      router.push('/requests');
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Lỗi khi tạo phiếu.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="max-w-4xl mx-auto">
      {/* Breadcrumb */}
      <nav className="flex items-center gap-2 text-sm text-slate-500 mb-6">
        <Link href="/requests" className="hover:text-slate-300 transition-colors">Phiếu đề nghị</Link>
        <span>→</span>
        <span className="text-slate-300">Tạo mới</span>
      </nav>

      <form onSubmit={handleSubmit}>
        {/* Error */}
        {error && (
          <div className="mb-6 p-4 bg-red-500/10 border border-red-500/20 rounded-xl text-red-400 text-sm">
            {error}
          </div>
        )}

        {/* MICE Warning */}
        {showMiceWarning && (
          <div className="mb-6 p-4 bg-amber-500/10 border border-amber-500/20 rounded-xl flex items-start gap-3">
            <svg className="w-5 h-5 text-amber-400 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.082 16.5c-.77.833.192 2.5 1.732 2.5z" />
            </svg>
            <div>
              <span className="font-semibold text-amber-400 text-sm">Cảnh báo MICE!</span>
              <p className="text-amber-300/80 text-sm mt-0.5">
                Tour loại MICE thường yêu cầu ít nhất 10 khách. Số khách hiện tại ({form.guestCount}) thấp hơn mức khuyến nghị.
              </p>
            </div>
          </div>
        )}

        {/* Section 1: General Info */}
        <div className="bg-slate-900/50 border border-slate-800 rounded-2xl overflow-hidden mb-6">
          <div className="px-8 py-5 border-b border-slate-800">
            <h2 className="text-lg font-bold text-slate-100">Thông tin chung</h2>
          </div>
          <div className="px-8 py-6 space-y-5">
            <div className="grid grid-cols-2 gap-4">
              <div className="col-span-2">
                <label className="block text-sm font-medium text-slate-300 mb-1.5">
                  Tên tour <span className="text-red-400">*</span>
                </label>
                <input
                  type="text"
                  value={form.tourName}
                  onChange={(e) => setForm({ ...form, tourName: e.target.value })}
                  className="w-full px-4 py-2.5 bg-slate-800 border border-slate-700 rounded-xl text-slate-200 placeholder-slate-500 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                  placeholder="VD: Tour MICE Đà Nẵng"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1.5">
                  Ngày khởi hành <span className="text-red-400">*</span>
                </label>
                <input
                  type="date"
                  value={form.departureDate}
                  onChange={(e) => setForm({ ...form, departureDate: e.target.value })}
                  className="w-full px-4 py-2.5 bg-slate-800 border border-slate-700 rounded-xl text-slate-200 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1.5">
                  Người phụ trách <span className="text-red-400">*</span>
                </label>
                <input
                  type="text"
                  value={form.personInCharge}
                  onChange={(e) => setForm({ ...form, personInCharge: e.target.value })}
                  className="w-full px-4 py-2.5 bg-slate-800 border border-slate-700 rounded-xl text-slate-200 placeholder-slate-500 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                  placeholder="Họ và tên"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1.5">
                  Loại tour <span className="text-red-400">*</span>
                </label>
                <select
                  value={form.tourType}
                  onChange={(e) => setForm({ ...form, tourType: e.target.value as 'FIT' | 'GIT' | 'MICE' })}
                  className="w-full px-4 py-2.5 bg-slate-800 border border-slate-700 rounded-xl text-slate-200 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                >
                  <option value="FIT">FIT — Khách lẻ</option>
                  <option value="GIT">GIT — Đoàn</option>
                  <option value="MICE">MICE — Hội nghị</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1.5">
                  Số lượng khách <span className="text-red-400">*</span>
                </label>
                <input
                  type="number"
                  min="1"
                  value={form.guestCount}
                  onChange={(e) => setForm({ ...form, guestCount: Number(e.target.value) })}
                  className="w-full px-4 py-2.5 bg-slate-800 border border-slate-700 rounded-xl text-slate-200 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                />
              </div>
            </div>
          </div>
        </div>

        {/* Section 2: Service Details */}
        <div className="bg-slate-900/50 border border-slate-800 rounded-2xl overflow-hidden mb-6">
          <div className="px-8 py-5 border-b border-slate-800 flex items-center justify-between">
            <h2 className="text-lg font-bold text-slate-100">Danh sách dịch vụ</h2>
            <button
              type="button"
              onClick={addDetail}
              className="inline-flex items-center gap-1.5 px-4 py-2 bg-blue-500/10 text-blue-400 hover:bg-blue-500/20 rounded-lg text-sm font-medium transition-colors border border-blue-500/20"
            >
              <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
              </svg>
              Thêm dịch vụ
            </button>
          </div>

          <div className="px-8 py-6 space-y-4">
            {details.map((detail, index) => (
              <div
                key={index}
                className="p-5 bg-slate-800/50 border border-slate-700/50 rounded-xl space-y-4"
              >
                <div className="flex items-center justify-between">
                  <span className="text-sm font-semibold text-slate-400">Dịch vụ #{index + 1}</span>
                  {details.length > 1 && (
                    <button
                      type="button"
                      onClick={() => removeDetail(index)}
                      className="text-red-400 hover:text-red-300 text-sm font-medium transition-colors"
                    >
                      Xóa
                    </button>
                  )}
                </div>

                <div className="grid grid-cols-3 gap-3">
                  <div>
                    <label className="block text-xs font-medium text-slate-400 mb-1">Loại dịch vụ *</label>
                    <input
                      type="text"
                      value={detail.serviceType}
                      onChange={(e) => updateDetail(index, 'serviceType', e.target.value)}
                      className="w-full px-3 py-2 bg-slate-800 border border-slate-700 rounded-lg text-sm text-slate-200 placeholder-slate-500 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                      placeholder="VD: Vận chuyển"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-slate-400 mb-1">Tên dịch vụ *</label>
                    <input
                      type="text"
                      value={detail.serviceName}
                      onChange={(e) => updateDetail(index, 'serviceName', e.target.value)}
                      className="w-full px-3 py-2 bg-slate-800 border border-slate-700 rounded-lg text-sm text-slate-200 placeholder-slate-500 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                      placeholder="VD: Xe 45 chỗ"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-slate-400 mb-1">Nhà cung cấp *</label>
                    <input
                      type="text"
                      value={detail.supplier}
                      onChange={(e) => updateDetail(index, 'supplier', e.target.value)}
                      className="w-full px-3 py-2 bg-slate-800 border border-slate-700 rounded-lg text-sm text-slate-200 placeholder-slate-500 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                      placeholder="VD: Công ty ABC"
                    />
                  </div>
                </div>

                <div className="grid grid-cols-4 gap-3">
                  <div>
                    <label className="block text-xs font-medium text-slate-400 mb-1">Số lượng *</label>
                    <input
                      type="number"
                      min="1"
                      value={detail.quantity}
                      onChange={(e) => updateDetail(index, 'quantity', Number(e.target.value))}
                      className="w-full px-3 py-2 bg-slate-800 border border-slate-700 rounded-lg text-sm text-slate-200 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-slate-400 mb-1">Đơn giá (VNĐ) *</label>
                    <input
                      type="number"
                      min="1"
                      value={detail.unitPrice || ''}
                      onChange={(e) => updateDetail(index, 'unitPrice', Number(e.target.value))}
                      className="w-full px-3 py-2 bg-slate-800 border border-slate-700 rounded-lg text-sm text-slate-200 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                      placeholder="0"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-slate-400 mb-1">Thành tiền</label>
                    <div className="px-3 py-2 bg-slate-900 border border-slate-700 rounded-lg text-sm text-emerald-400 font-semibold">
                      {lineTotals[index].toLocaleString('vi-VN')}
                    </div>
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-slate-400 mb-1">Ghi chú</label>
                    <input
                      type="text"
                      value={detail.note || ''}
                      onChange={(e) => updateDetail(index, 'note', e.target.value)}
                      className="w-full px-3 py-2 bg-slate-800 border border-slate-700 rounded-lg text-sm text-slate-200 placeholder-slate-500 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                      placeholder="Tùy chọn"
                    />
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Summary & Submit */}
        <div className="bg-slate-900/50 border border-slate-800 rounded-2xl overflow-hidden">
          <div className="px-8 py-6 flex items-center justify-between">
            <div>
              <p className="text-sm text-slate-400">Tổng chi phí phiếu</p>
              <p className="text-3xl font-bold text-emerald-400 mt-1">
                {grandTotal.toLocaleString('vi-VN')} <span className="text-lg text-slate-500">VNĐ</span>
              </p>
              {grandTotal > 100_000_000 && (
                <p className="text-xs text-amber-400 mt-1">⚠ Vượt ngưỡng 100 triệu → Cần phê duyệt quản lý</p>
              )}
            </div>
            <div className="flex items-center gap-3">
              <Link
                href="/requests"
                className="px-5 py-2.5 rounded-xl text-sm font-medium text-slate-400 hover:text-slate-200 hover:bg-slate-800 transition-all"
              >
                Hủy
              </Link>
              <button
                type="submit"
                disabled={isSubmitting}
                className="px-6 py-2.5 bg-gradient-to-r from-blue-500 to-cyan-500 text-white font-semibold rounded-xl shadow-lg shadow-blue-500/25 hover:shadow-blue-500/40 disabled:opacity-50 disabled:cursor-not-allowed transition-all text-sm"
              >
                {isSubmitting ? 'Đang tạo...' : 'Tạo Phiếu Đề Nghị'}
              </button>
            </div>
          </div>
        </div>
      </form>
    </div>
  );
}
