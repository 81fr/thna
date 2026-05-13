import React, { useState, useMemo, useEffect } from 'react';
import { 
  LayoutGrid, 
  Box, 
  FileText, 
  Shuffle, 
  PieChart, 
  ClipboardList, 
  Settings, 
  BarChart3, 
  Database,
  TrendingUp,
  Activity,
  ShieldCheck,
  Calendar,
  AlertTriangle,
  CheckCircle,
  Filter,
  FilePlus,
  MoreHorizontal,
  Search,
  Bell,
  ChevronDown,
  Edit,
  Trash2,
  Download
} from 'lucide-react';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  ArcElement,
} from 'chart.js';
import { Bar } from 'react-chartjs-2';
import './App.css';

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend
);

const DEPRECIATION_METHODS = {
  SL: 'القسط الثابت',
  DB: 'القسط المتناقص المضاعف',
  SYD: 'مجموع أرقام السنوات'
};

const App = () => {
  const [view, setView] = useState('dashboard');
  const [assets, setAssets] = useState([
    { id: 'ORG-AST-001', code: 'IT-001', name: 'خوادم البيانات المركزية', category: 'أصول تقنية', cost: 120000, vat: 18000, salvage: 10000, life: 5, date: '2023-01-01', method: 'SL', status: 'يعمل', custody: 'أحمد سالم', source: 'بنك الراجحي' },
    { id: 'ORG-AST-002', code: 'VH-021', name: 'أسطول سيارات التوزيع', category: 'مركبات', cost: 450000, vat: 67500, salvage: 50000, life: 7, date: '2022-06-15', method: 'DB', status: 'يعمل', custody: 'محمد العبدالله', source: 'البلاد' },
    { id: 'ORG-AST-003', code: 'LD-001', name: 'أرض المقر الرئيسي', category: 'أراضي', cost: 1500000, vat: 0, salvage: 1500000, life: 99, date: '2015-01-01', method: 'SL', status: 'يعمل', custody: '-', source: 'تبرعات عينية' },
    { id: 'ORG-AST-004', code: 'WAQ-001', name: 'مبنى الوقف السكني', category: 'أصول أوقاف', cost: 3000000, vat: 0, salvage: 500000, life: 40, date: '2018-05-10', method: 'SL', status: 'يعمل', custody: 'إدارة الأوقاف', source: 'تبرعات عينية' },
    { id: 'ORG-AST-005', code: 'OF-015', name: 'طابعة مكتبية صغيرة', category: 'أثاث', cost: 1500, vat: 225, salvage: 0, life: 3, date: '2024-01-10', method: 'SL', status: 'يعمل', custody: 'سعد فهد', source: 'موردين', isExpense: true },
  ]);

  const [journals, setJournals] = useState([
    { id: 'JV-2024-001', date: '2024-03-01', desc: 'إثبات إهلاك شهر مارس', debit: 15200, credit: null, status: 'مرحل' },
    { id: 'JV-2024-001', date: '2024-03-01', desc: 'مجمع إهلاك الأصول', debit: null, credit: 15200, status: 'مرحل' },
    { id: 'JV-2024-002', date: '2024-04-01', desc: 'إثبات إهلاك شهر أبريل', debit: 15200, credit: null, status: 'مسودة' },
    { id: 'JV-2024-002', date: '2024-04-01', desc: 'مجمع إهلاك الأصول', debit: null, credit: 15200, status: 'مسودة' },
  ]);

  const [transfers, setTransfers] = useState([
    { id: 'TR-092', asset: 'تجهيزات المكتب الرئيسي', from: 'الإدارة', to: 'الموارد البشرية', date: '2024-01-15', status: 'مكتمل' },
    { id: 'TR-093', asset: 'آلة تغليف صناعية', from: 'المستودع', to: 'الإنتاج', date: '2024-02-20', status: 'قيد المراجعة' },
  ]);

  const accountingEngine = useMemo(() => {
    const now = new Date();
    return assets.map(asset => {
      const startDate = new Date(asset.date);
      const monthsElapsed = (now.getFullYear() - startDate.getFullYear()) * 12 + (now.getMonth() - startDate.getMonth());
      const yearsElapsed = Math.max(0, monthsElapsed / 12);
      
      if (asset.isExpense) {
        return {
          ...asset,
          accumulatedDep: asset.cost,
          netBookValue: 0,
        };
      }

      if (asset.category === 'أراضي') {
        return {
          ...asset,
          accumulatedDep: 0,
          netBookValue: asset.cost,
        };
      }

      let accumulatedDep = 0;
      if (asset.method === 'SL') {
        const annualDep = (asset.cost - asset.salvage) / asset.life;
        accumulatedDep = Math.min(asset.cost - asset.salvage, annualDep * yearsElapsed);
      } else if (asset.method === 'DB') {
        const rate = (2 / asset.life);
        let bookValue = asset.cost;
        for(let i=0; i < Math.floor(yearsElapsed); i++) {
          bookValue -= bookValue * rate;
        }
        accumulatedDep = asset.cost - Math.max(asset.salvage, bookValue);
      } else if (asset.method === 'SYD') {
        const sum = (asset.life * (asset.life + 1)) / 2;
        let dep = 0;
        for(let i=0; i < Math.floor(yearsElapsed); i++) {
          dep += (asset.cost - asset.salvage) * ((asset.life - i) / sum);
        }
        accumulatedDep = Math.min(asset.cost - asset.salvage, dep);
      }

      return {
        ...asset,
        accumulatedDep,
        netBookValue: asset.cost - accumulatedDep,
      };
    });
  }, [assets]);

  const totals = useMemo(() => {
    return accountingEngine.reduce((acc, curr) => ({
      cost: acc.cost + curr.cost,
      dep: acc.dep + curr.accumulatedDep,
      nbv: acc.nbv + curr.netBookValue
    }), { cost: 0, dep: 0, nbv: 0 });
  }, [accountingEngine]);

  const chartData = {
    labels: accountingEngine.map(a => a.name.length > 15 ? a.name.substring(0, 15) + '...' : a.name),
    datasets: [
      { label: 'التكلفة التاريخية', data: accountingEngine.map(a => a.cost), backgroundColor: '#0f172a' },
      { label: 'الإهلاك المتراكم', data: accountingEngine.map(a => a.accumulatedDep), backgroundColor: '#0d9488' }
    ]
  };

  const renderDashboard = () => (
    <div className="view-anim">
      <div className="summary-grid">
        <div className="card">
          <div className="stat-icon" style={{background: '#f1f5f9'}}><Database size={20} color="#64748b" /></div>
          <div className="val-sub">إجمالي قيمة المحفظة</div>
          <div className="val-big">{totals.cost.toLocaleString()} ر.س</div>
          <div className="val-sub" style={{color: 'var(--success)'}}><TrendingUp size={14} /> +4.2% نمو سنوي</div>
        </div>
        <div className="card">
          <div className="stat-icon" style={{background: '#fef2f2'}}><Activity size={20} color="#ef4444" /></div>
          <div className="val-sub">الإهلاك المتراكم</div>
          <div className="val-big">{totals.dep.toLocaleString()} ر.س</div>
          <div className="val-sub">يمثل {((totals.dep/totals.cost)*100).toFixed(1)}% من التكلفة</div>
        </div>
        <div className="card">
          <div className="stat-icon" style={{background: '#f0fdf4'}}><ShieldCheck size={20} color="#10b981" /></div>
          <div className="val-sub">صافي القيمة الدفترية</div>
          <div className="val-big">{totals.nbv.toLocaleString()} ر.س</div>
          <div className="val-sub">قيمة الأصول الحالية في الدفاتر</div>
        </div>
        <div className="card">
          <div className="stat-icon" style={{background: '#fffbeb'}}><Calendar size={20} color="#f59e0b" /></div>
          <div className="val-sub">تقدم السنة المالية</div>
          <div className="val-big">الربع الثالث</div>
          <div className="fiscal-progress"><div className="fiscal-bar"></div></div>
          <div className="val-sub" style={{marginTop:'0.5rem'}}>تم إقفال 65% من العمليات</div>
        </div>
      </div>

      <div style={{display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '1.5rem'}}>
        <div className="card">
          <h3 style={{marginBottom:'1rem', fontSize:'1.1rem'}}>تحليل الأصول: التكلفة مقابل الإهلاك</h3>
          <div className="chart-container">
            <Bar data={chartData} options={{ responsive: true, maintainAspectRatio: false }} />
          </div>
        </div>
        <div className="card">
          <h3 style={{marginBottom:'1.5rem', fontSize:'1.1rem'}}>تنبيهات التدقيق</h3>
          <div style={{display: 'flex', flexDirection: 'column', gap: '1rem'}}>
            <div style={{display:'flex', gap:'1rem', padding:'1rem', background:'#fff7ed', borderRadius:'8px', borderRight:'4px solid #f59e0b'}}>
              <AlertTriangle color="#f59e0b" size={20} />
              <div>
                <div style={{fontSize:'0.85rem', fontWeight:600}}>أصول قاربت على النفاد</div>
                <div style={{fontSize:'0.75rem', color:'#9a3412'}}>خوادم البيانات وصلت إلى 90% إهلاك.</div>
              </div>
            </div>
            <div style={{display:'flex', gap:'1rem', padding:'1rem', background:'#f0fdfa', borderRadius:'8px', borderRight:'4px solid #0d9488'}}>
              <CheckCircle color="#0d9488" size={20} />
              <div>
                <div style={{fontSize:'0.85rem', fontWeight:600}}>الجرد السنوي مكتمل</div>
                <div style={{fontSize:'0.75rem', color:'#134e4a'}}>تمت مطابقة 100% من الأصول المادية.</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );

  const renderRegister = () => (
    <div className="view-anim">
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:'2rem'}}>
        <h2 style={{fontSize:'1.25rem'}}>سجل الأصول الثابتة (FAR)</h2>
        <div style={{display:'flex', gap:'0.5rem'}}>
          <button className="btn btn-ghost"><Filter size={18} /> تصفية</button>
          <button className="btn btn-ghost"><Download size={18} /> تصدير Excel</button>
          <button className="btn btn-ghost"><Download size={18} /> تصدير PDF</button>
          <button className="btn btn-primary" onClick={() => setView('new-asset')}><FilePlus size={18} /> تسجيل أصل جديد</button>
        </div>
      </div>
      <div className="table-wrapper">
        <table>
          <thead>
            <tr>
              <th>الرمز الموحد</th>
              <th>بيانات الأصل</th>
              <th>العهدة/المصدر</th>
              <th>التكلفة (مع الضريبة)</th>
              <th>الإهلاك المتراكم</th>
              <th>صافي القيمة</th>
              <th>الحالة</th>
              <th>إجراءات</th>
            </tr>
          </thead>
          <tbody>
            {accountingEngine.map(asset => (
              <tr key={asset.id}>
                <td style={{color:'var(--accent)', fontWeight:600}}>{asset.code}</td>
                <td>
                  <div style={{fontWeight:600}}>{asset.name}</div>
                  <div style={{fontSize:'0.75rem', color:'var(--text-muted)'}}>{asset.category} | {DEPRECIATION_METHODS[asset.method]}</div>
                </td>
                <td>
                  <div style={{fontWeight:600}}>{asset.custody}</div>
                  <div style={{fontSize:'0.75rem', color:'var(--text-muted)'}}>{asset.source}</div>
                </td>
                <td>{(asset.cost + (asset.vat || 0)).toLocaleString()} ر.س</td>
                <td style={{color:'var(--danger)'}}>{asset.category === 'أراضي' ? '-' : `-${asset.accumulatedDep.toLocaleString()}`}</td>
                <td style={{fontWeight:700}}>{asset.netBookValue.toLocaleString()} ر.س</td>
                <td>
                  {asset.isExpense ? 
                    <span className="badge" style={{background:'#fef2f2', color:'#ef4444'}}>مصروف</span> :
                    <span className={`badge ${asset.status === 'يعمل' ? 'b-active' : ''}`} style={asset.status === 'بالمستودع' ? {background:'#fef3c7', color:'#92400e'} : asset.status === 'تالف' ? {background:'#fee2e2', color:'#b91c1c'} : {}}>{asset.status}</span>
                  }
                </td>
                <td>
                  <div style={{display:'flex', gap:'0.5rem'}}>
                    <button className="btn btn-ghost" style={{padding:'0.25rem'}} title="تعديل"><Edit size={16} /></button>
                    <button className="btn btn-ghost" style={{padding:'0.25rem', color:'var(--danger)'}} title="استبعاد/بيع"><Trash2 size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );

  const handleAddAsset = (e) => {
    e.preventDefault();
    const fd = new FormData(e.target);
    const cost = parseFloat(fd.get('cost') || 0);
    const life = parseFloat(fd.get('life') || 0);
    
    if (cost < 3000 || life < 1) {
      alert("تنبيه: سيتم تسجيل هذا البند كمصروف دوري فوراً وفقاً لسياسة الحد الأدنى لرسملة الأصول (أقل من 3000 ريال أو عمر أقل من سنة).");
    } else {
      alert("تم اعتماد الأصل بنجاح وتوليد رمز تتبع موحد له (Barcode/Serial).");
    }
    setView('register');
  };

  const renderNewAsset = () => (
    <div className="view-anim">
      <div style={{marginBottom:'2rem'}}>
        <h2 style={{fontSize:'1.25rem'}}>تسجيل أصل جديد</h2>
        <p style={{color:'var(--text-muted)', fontSize:'0.85rem'}}>أدخل بيانات الأصل الثابت الجديد لإضافته إلى السجل</p>
      </div>
      <div className="card" style={{maxWidth: '800px', background: 'var(--card-bg)'}}>
        <form onSubmit={handleAddAsset} style={{display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem'}}>
          <div style={{display: 'flex', flexDirection: 'column', gap: '0.5rem'}}>
            <label style={{fontSize: '0.85rem', fontWeight: 600}}>اسم الأصل</label>
            <input name="name" type="text" placeholder="مثال: سيارة نقل تويوتا" required style={{padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'transparent', color: 'var(--text)'}} />
          </div>
          <div style={{display: 'flex', flexDirection: 'column', gap: '0.5rem'}}>
            <label style={{fontSize: '0.85rem', fontWeight: 600}}>الفئة التصنيفية</label>
            <select name="category" required style={{padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'transparent', color: 'var(--text)'}}>
              <option value="أراضي">أراضي</option>
              <option value="مباني">مباني</option>
              <option value="مركبات">سيارات ومركبات</option>
              <option value="أصول تقنية">أجهزة تقنية</option>
              <option value="أثاث">أثاث ومعدات</option>
              <option value="أصول أوقاف">أصول أوقاف</option>
            </select>
          </div>
          <div style={{display: 'flex', flexDirection: 'column', gap: '0.5rem'}}>
            <label style={{fontSize: '0.85rem', fontWeight: 600}}>مصدر الأصل / وسيلة الدفع</label>
            <select name="source" required style={{padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'transparent', color: 'var(--text)'}}>
              <option value="بنك الراجحي">بنك الراجحي</option>
              <option value="بنك البلاد">بنك البلاد</option>
              <option value="موردين">موردين (آجل)</option>
              <option value="تبرعات عينية">تبرعات عينية</option>
            </select>
          </div>
          <div style={{display: 'flex', flexDirection: 'column', gap: '0.5rem'}}>
            <label style={{fontSize: '0.85rem', fontWeight: 600}}>الحالة التشغيلية</label>
            <select name="status" required style={{padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'transparent', color: 'var(--text)'}}>
              <option value="يعمل">يعمل</option>
              <option value="بالمستودع">بالمستودع</option>
              <option value="تالف">تالف</option>
            </select>
          </div>
          <div style={{display: 'flex', flexDirection: 'column', gap: '0.5rem'}}>
            <label style={{fontSize: '0.85rem', fontWeight: 600}}>تحديد العهدة (اسم الموظف)</label>
            <input name="custody" type="text" placeholder="مثال: أحمد سالم" style={{padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'transparent', color: 'var(--text)'}} />
          </div>
          <div style={{display: 'flex', flexDirection: 'column', gap: '0.5rem'}}>
            <label style={{fontSize: '0.85rem', fontWeight: 600}}>التكلفة الأساسية (ر.س)</label>
            <input name="cost" type="number" placeholder="0.00" required style={{padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'transparent', color: 'var(--text)'}} />
          </div>
          <div style={{display: 'flex', flexDirection: 'column', gap: '0.5rem'}}>
            <label style={{fontSize: '0.85rem', fontWeight: 600}}>الضريبة المضافة (VAT)</label>
            <input name="vat" type="number" placeholder="0.00" defaultValue="0" style={{padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'transparent', color: 'var(--text)'}} />
          </div>
          <div style={{display: 'flex', flexDirection: 'column', gap: '0.5rem'}}>
            <label style={{fontSize: '0.85rem', fontWeight: 600}}>العمر الإنتاجي (سنوات)</label>
            <input name="life" type="number" placeholder="مثال: 5 (اتركه فارغاً للأراضي)" style={{padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'transparent', color: 'var(--text)'}} />
          </div>
          <div style={{display: 'flex', gap: '1rem', gridColumn: '1 / -1', marginTop: '1rem'}}>
            <button type="submit" className="btn btn-primary" style={{padding: '0.75rem 2rem'}}>حفظ الأصل</button>
            <button type="button" className="btn btn-ghost" onClick={() => setView('register')} style={{padding: '0.75rem 2rem'}}>إلغاء</button>
          </div>
        </form>
      </div>
    </div>
  );

  const renderJournal = () => (
    <div className="view-anim">
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:'2rem'}}>
        <h2 style={{fontSize:'1.25rem'}}>قيود اليومية التلقائية</h2>
        <button className="btn btn-primary" onClick={() => setJournals(journals.map(j => ({...j, status: 'مرحل'})))}><FileText size={18} /> ترحيل القيود</button>
      </div>
      <div className="table-wrapper">
        <table>
          <thead><tr><th>رقم القيد</th><th>التاريخ</th><th>البيان</th><th>مدين</th><th>دائن</th><th>الحالة</th></tr></thead>
          <tbody>
            {journals.map((j, idx) => (
              <tr key={idx}>
                <td style={{fontWeight:600}}>{j.id}</td>
                <td>{j.date}</td>
                <td>{j.desc}</td>
                <td style={{color:'var(--danger)', fontWeight:600}}>{j.debit ? j.debit.toLocaleString() : '-'}</td>
                <td style={{color:'var(--success)', fontWeight:600}}>{j.credit ? j.credit.toLocaleString() : '-'}</td>
                <td>
                  {j.status === 'مرحل' ? 
                    <span className="badge b-active">مرحل</span> : 
                    <span className="badge" style={{background:'#fef3c7', color:'#92400e'}}>مسودة</span>
                  }
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );

  const renderTransfers = () => (
    <div className="view-anim">
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:'2rem'}}>
        <h2 style={{fontSize:'1.25rem'}}>التحويلات العينية للأصول</h2>
        <button className="btn btn-primary" onClick={() => setView('new-transfer')}><Shuffle size={18} /> طلب تحويل جديد</button>
      </div>
      <div className="table-wrapper">
        <table>
          <thead><tr><th>رقم الطلب</th><th>الأصل</th><th>من قسم</th><th>إلى قسم</th><th>التاريخ</th><th>الحالة</th></tr></thead>
          <tbody>
            {transfers.map(t => (
              <tr key={t.id}>
                <td style={{fontWeight:600}}>{t.id}</td>
                <td>{t.asset}</td>
                <td>{t.from}</td>
                <td>{t.to}</td>
                <td>{t.date}</td>
                <td>
                  {t.status === 'مكتمل' ? 
                    <span className="badge b-active">مكتمل</span> : 
                    <span className="badge" style={{background:'#fef3c7', color:'#92400e'}}>قيد المراجعة</span>
                  }
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );

  const renderNewTransfer = () => (
    <div className="view-anim">
      <div style={{marginBottom:'2rem'}}>
        <h2 style={{fontSize:'1.25rem'}}>طلب تحويل أصل جديد</h2>
        <p style={{color:'var(--text-muted)', fontSize:'0.85rem'}}>قم بتحديد الأصل المراد نقله والقسم الوجهة</p>
      </div>
      <div className="card" style={{maxWidth: '800px', background: 'var(--card-bg)'}}>
        <form onSubmit={(e) => { e.preventDefault(); setView('transfers'); }} style={{display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem'}}>
          <div style={{display: 'flex', flexDirection: 'column', gap: '0.5rem', gridColumn: '1 / -1'}}>
            <label style={{fontSize: '0.85rem', fontWeight: 600}}>الأصل المراد تحويله</label>
            <select required style={{padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'transparent', color: 'var(--text)'}}>
              <option value="">-- اختر أصلاً --</option>
              {assets.map(a => <option key={a.id} value={a.id}>{a.name} ({a.id})</option>)}
            </select>
          </div>
          <div style={{display: 'flex', flexDirection: 'column', gap: '0.5rem'}}>
            <label style={{fontSize: '0.85rem', fontWeight: 600}}>إلى قسم</label>
            <select required style={{padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'transparent', color: 'var(--text)'}}>
              <option value="الإنتاج">الإنتاج</option>
              <option value="الموارد البشرية">الموارد البشرية</option>
              <option value="المبيعات">المبيعات</option>
              <option value="الإدارة">الإدارة</option>
            </select>
          </div>
          <div style={{display: 'flex', flexDirection: 'column', gap: '0.5rem'}}>
            <label style={{fontSize: '0.85rem', fontWeight: 600}}>السبب التبريري</label>
            <input type="text" placeholder="مثال: حاجة العمل لمعدات إضافية" required style={{padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'transparent', color: 'var(--text)'}} />
          </div>
          <div style={{display: 'flex', gap: '1rem', gridColumn: '1 / -1', marginTop: '1rem'}}>
            <button type="submit" className="btn btn-primary" style={{padding: '0.75rem 2rem'}}>إرسال الطلب للموافقة</button>
            <button type="button" className="btn btn-ghost" onClick={() => setView('transfers')} style={{padding: '0.75rem 2rem'}}>إلغاء</button>
          </div>
        </form>
      </div>
    </div>
  );

  const renderBudget = () => (
    <div className="view-anim">
      <div style={{marginBottom:'2rem'}}>
        <h2 style={{fontSize:'1.25rem'}}>الميزانية التقديرية الرأسمالية (CAPEX)</h2>
      </div>
      <div className="summary-grid" style={{gridTemplateColumns: 'repeat(3, 1fr)'}}>
        <div className="card">
          <div className="val-sub">الميزانية المعتمدة لعام 2024</div>
          <div className="val-big" style={{color:'var(--success)'}}>1,500,000 ر.س</div>
        </div>
        <div className="card">
          <div className="val-sub">المنصرف الفعلي حتى الآن</div>
          <div className="val-big" style={{color:'var(--danger)'}}>680,000 ر.س</div>
        </div>
        <div className="card">
          <div className="val-sub">المتبقي من الميزانية</div>
          <div className="val-big" style={{color:'var(--accent)'}}>820,000 ر.س</div>
        </div>
      </div>
    </div>
  );

  const renderInventory = () => (
    <div className="view-anim">
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:'2rem'}}>
        <h2 style={{fontSize:'1.25rem'}}>تقارير الجرد الميداني</h2>
        <button className="btn btn-primary"><CheckCircle size={18} /> بدء جرد جديد</button>
      </div>
      <div style={{display: 'grid', gap: '1rem'}}>
        <div className="card" style={{display:'flex', justifyContent:'space-between', alignItems:'center', padding:'1.5rem'}}>
          <div><div style={{fontWeight:600, fontSize:'1.1rem'}}>جرد الربع الأول 2024</div><div style={{fontSize:'0.85rem', color:'var(--text-muted)'}}>تاريخ الإغلاق: 31-03-2024 - شامل جميع الفروع</div></div>
          <div><span className="badge b-active" style={{fontSize:'0.9rem'}}>مكتمل 100%</span></div>
        </div>
        <div className="card" style={{display:'flex', justifyContent:'space-between', alignItems:'center', padding:'1.5rem'}}>
          <div><div style={{fontWeight:600, fontSize:'1.1rem'}}>جرد الربع الثاني 2024</div><div style={{fontSize:'0.85rem', color:'var(--text-muted)'}}>الفرع الرئيسي فقط</div></div>
          <div><span className="badge" style={{background:'#fef3c7', color:'#92400e', fontSize:'0.9rem'}}>قيد التنفيذ 45%</span></div>
        </div>
      </div>
    </div>
  );

  const renderSettings = () => (
    <div className="view-anim">
      <h2 style={{fontSize:'1.25rem', marginBottom:'2rem'}}>إعدادات النظام</h2>
      <div className="card" style={{maxWidth: '600px', display:'flex', flexDirection:'column', gap:'1.5rem'}}>
        <div style={{display:'flex', justifyContent:'space-between', alignItems:'center'}}>
          <div>
            <div style={{fontWeight:600}}>الإشعارات التلقائية</div>
            <div style={{fontSize:'0.85rem', color:'var(--text-muted)'}}>تفعيل إرسال تنبيهات الجرد والإهلاك قبل الموعد</div>
          </div>
          <input type="checkbox" defaultChecked style={{width:'40px', height:'20px', cursor:'pointer'}} />
        </div>
        <div style={{height:'1px', background:'var(--border)'}}></div>
        <div style={{display:'flex', justifyContent:'space-between', alignItems:'center'}}>
          <div>
            <div style={{fontWeight:600}}>ربط النظام المحاسبي (ERP)</div>
            <div style={{fontSize:'0.85rem', color:'var(--text-muted)'}}>مزامنة قيود اليومية مع دفتر الأستاذ العام تلقائياً</div>
          </div>
          <button className="btn btn-ghost" style={{color:'var(--success)'}}><CheckCircle size={16} /> متصل</button>
        </div>
      </div>
    </div>
  );

  return (
    <div className="app-container">
      <aside className="sidebar">
        <div className="logo-area">
          <div className="logo-box"><BarChart3 size={20} color="white" /></div>
          <span style={{fontWeight:800, fontSize:'1.2rem'}}>تراؤف <span style={{color:'var(--accent-light)'}}>V3.0</span></span>
        </div>

        <div className="nav-group">
          <div className="nav-label">الرئيسية</div>
          <div className={`nav-item ${view === 'dashboard' ? 'active' : ''}`} onClick={() => setView('dashboard')}>
            <LayoutGrid size={18} /> لوحة التحكم
          </div>
        </div>

        <div className="nav-group">
          <div className="nav-label">إدارة الأصول</div>
          <div className={`nav-item ${view === 'register' ? 'active' : ''}`} onClick={() => setView('register')}>
            <Box size={18} /> سجل الأصول الثابتة
          </div>
          <div className={`nav-item ${view === 'journal' ? 'active' : ''}`} onClick={() => setView('journal')}><FileText size={18} /> قيود اليومية</div>
          <div className={`nav-item ${view === 'transfers' ? 'active' : ''}`} onClick={() => setView('transfers')}><Shuffle size={18} /> التحويلات العينية</div>
        </div>

        <div className="nav-group">
          <div className="nav-label">التقارير</div>
          <div className={`nav-item ${view === 'budget' ? 'active' : ''}`} onClick={() => setView('budget')}><PieChart size={18} /> الميزانية التقديرية</div>
          <div className={`nav-item ${view === 'inventory' ? 'active' : ''}`} onClick={() => setView('inventory')}><ClipboardList size={18} /> تقارير الجرد</div>
        </div>

        <div style={{marginTop: 'auto'}}>
          <div className={`nav-item ${view === 'settings' ? 'active' : ''}`} onClick={() => setView('settings')}><Settings size={18} /> الإعدادات</div>
          <div style={{display:'flex', alignItems:'center', gap:'0.75rem', padding:'1rem', background:'rgba(255,255,255,0.05)', borderRadius:'12px', marginTop:'1rem'}}>
            <div style={{width:'35px', height:'35px', background:'var(--accent)', borderRadius:'50%', display:'flex', justifyContent:'center', alignItems:'center', fontSize:'0.8rem', fontWeight:700}}>FA</div>
            <div>
              <div style={{fontSize:'0.8rem', fontWeight:600}}>فيصل المحاسب</div>
              <div style={{fontSize:'0.65rem', color:'#94a3b8'}}>المدير التقني والمالي</div>
            </div>
          </div>
        </div>
      </aside>

      <main className="main-content">
        <header className="top-bar">
          <div style={{display:'flex', alignItems:'center', gap:'1rem'}}>
            <div style={{display:'flex', background:'#f1f5f9', padding:'0.5rem 1rem', borderRadius:'20px', gap:'0.5rem', alignItems:'center'}}>
              <Search size={16} color="#64748b" />
              <input type="text" placeholder="بحث سريع..." style={{border:'none', background:'transparent', outline:'none', fontSize:'0.85rem', width:'200px'}} />
            </div>
          </div>
          <div style={{display:'flex', alignItems:'center', gap:'1.5rem'}}>
            <Bell size={20} color="#64748b" style={{cursor:'pointer'}} />
            <div style={{height:'30px', width:'1px', background:'var(--border)'}}></div>
            <div style={{display:'flex', alignItems:'center', gap:'0.5rem', cursor:'pointer'}}>
              <span style={{fontSize:'0.85rem', fontWeight:600}}>2024</span>
              <ChevronDown size={14} />
            </div>
          </div>
        </header>

        <div className="content-area">
          {view === 'dashboard' && renderDashboard()}
          {view === 'register' && renderRegister()}
          {view === 'new-asset' && renderNewAsset()}
          {view === 'journal' && renderJournal()}
          {view === 'transfers' && renderTransfers()}
          {view === 'new-transfer' && renderNewTransfer()}
          {view === 'budget' && renderBudget()}
          {view === 'inventory' && renderInventory()}
          {view === 'settings' && renderSettings()}
        </div>
      </main>
    </div>
  );
};

export default App;
