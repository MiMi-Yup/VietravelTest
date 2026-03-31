import Link from 'next/link';

export default function HomePage() {
  return (
    <div className="flex flex-col items-center justify-center min-h-[70vh]">
      {/* Hero */}
      <div className="text-center max-w-3xl">
        <div className="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-blue-500/10 border border-blue-500/20 text-blue-400 text-xs font-medium mb-6">
          <span className="w-2 h-2 rounded-full bg-blue-400 animate-pulse" />
          Hệ thống quản lý nội bộ
        </div>

        <h1 className="text-5xl sm:text-6xl font-bold tracking-tight">
          <span className="bg-gradient-to-r from-blue-400 via-cyan-300 to-teal-400 bg-clip-text text-transparent">
            Vietravel
          </span>
          <br />
          <span className="text-slate-200">Tour Management</span>
        </h1>

        <p className="mt-6 text-lg text-slate-400 leading-relaxed">
          Quản lý tour du lịch và phiếu đề nghị đặt dịch vụ.
          <br />
          Tự động tính toán chi phí và phân luồng phê duyệt.
        </p>

        <div className="mt-10 flex flex-col sm:flex-row items-center justify-center gap-4">
          <Link
            href="/tours"
            className="w-full sm:w-auto inline-flex items-center justify-center gap-2 px-6 py-3 bg-gradient-to-r from-blue-500 to-cyan-500 text-white font-semibold rounded-xl shadow-lg shadow-blue-500/25 hover:shadow-blue-500/40 hover:scale-[1.02] transition-all duration-200"
          >
            Quản lý Tour
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
            </svg>
          </Link>

          <Link
            href="/requests"
            className="w-full sm:w-auto inline-flex items-center justify-center gap-2 px-6 py-3 bg-slate-800 text-slate-300 font-semibold rounded-xl border border-slate-700 hover:bg-slate-700 hover:text-white hover:scale-[1.02] transition-all duration-200"
          >
            Phiếu đề nghị
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
          </Link>
        </div>
      </div>

      {/* Stats */}
      <div className="mt-20 grid grid-cols-1 sm:grid-cols-3 gap-6 w-full max-w-2xl">
        {[
          { label: 'Module Tour', value: 'CRUD', desc: 'Quản lý đầy đủ' },
          { label: 'Module Phiếu', value: 'Auto', desc: 'Phân luồng tự động' },
          { label: 'Ngưỡng duyệt', value: '100tr', desc: 'VNĐ' },
        ].map((stat) => (
          <div
            key={stat.label}
            className="text-center p-5 rounded-2xl bg-slate-900/50 border border-slate-800 hover:border-slate-700 transition-colors"
          >
            <div className="text-2xl font-bold text-blue-400">{stat.value}</div>
            <div className="text-sm text-slate-300 mt-1">{stat.label}</div>
            <div className="text-xs text-slate-500">{stat.desc}</div>
          </div>
        ))}
      </div>
    </div>
  );
}