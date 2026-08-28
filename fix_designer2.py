import re

with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\App_Code\db.designer.cs', 'r', encoding='utf-8') as f:
    cs = f.read()

props = '''
		private string _chucdanh;
		private System.Nullable<decimal> _PhuCap_RnD;
		private System.Nullable<decimal> _PhuCap_TrucHotline;
		private System.Nullable<decimal> _Thuong_DuAn_Max;
		private System.Nullable<decimal> _LuongDongBH;
		private System.Nullable<decimal> _NganSach_Max;

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_chucdanh", DbType="NVarChar(MAX)")]
		public string chucdanh
		{
			get { return this._chucdanh; }
			set { if ((this._chucdanh != value)) { this.OnchucdanhChanging(value); this.SendPropertyChanging(); this._chucdanh = value; this.SendPropertyChanged("chucdanh"); this.OnchucdanhChanged(); } }
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PhuCap_RnD", DbType="Decimal(18,0)")]
		public System.Nullable<decimal> PhuCap_RnD
		{
			get { return this._PhuCap_RnD; }
			set { if ((this._PhuCap_RnD != value)) { this.OnPhuCap_RnDChanging(value); this.SendPropertyChanging(); this._PhuCap_RnD = value; this.SendPropertyChanged("PhuCap_RnD"); this.OnPhuCap_RnDChanged(); } }
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PhuCap_TrucHotline", DbType="Decimal(18,0)")]
		public System.Nullable<decimal> PhuCap_TrucHotline
		{
			get { return this._PhuCap_TrucHotline; }
			set { if ((this._PhuCap_TrucHotline != value)) { this.OnPhuCap_TrucHotlineChanging(value); this.SendPropertyChanging(); this._PhuCap_TrucHotline = value; this.SendPropertyChanged("PhuCap_TrucHotline"); this.OnPhuCap_TrucHotlineChanged(); } }
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Thuong_DuAn_Max", DbType="Decimal(18,0)")]
		public System.Nullable<decimal> Thuong_DuAn_Max
		{
			get { return this._Thuong_DuAn_Max; }
			set { if ((this._Thuong_DuAn_Max != value)) { this.OnThuong_DuAn_MaxChanging(value); this.SendPropertyChanging(); this._Thuong_DuAn_Max = value; this.SendPropertyChanged("Thuong_DuAn_Max"); this.OnThuong_DuAn_MaxChanged(); } }
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LuongDongBH", DbType="Decimal(18,0)")]
		public System.Nullable<decimal> LuongDongBH
		{
			get { return this._LuongDongBH; }
			set { if ((this._LuongDongBH != value)) { this.OnLuongDongBHChanging(value); this.SendPropertyChanging(); this._LuongDongBH = value; this.SendPropertyChanged("LuongDongBH"); this.OnLuongDongBHChanged(); } }
		}

		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NganSach_Max", DbType="Decimal(18,0)")]
		public System.Nullable<decimal> NganSach_Max
		{
			get { return this._NganSach_Max; }
			set { if ((this._NganSach_Max != value)) { this.OnNganSach_MaxChanging(value); this.SendPropertyChanging(); this._NganSach_Max = value; this.SendPropertyChanged("NganSach_Max"); this.OnNganSach_MaxChanged(); } }
		}
'''

events = '''
    partial void OnchucdanhChanging(string value);
    partial void OnchucdanhChanged();
    partial void OnPhuCap_RnDChanging(System.Nullable<decimal> value);
    partial void OnPhuCap_RnDChanged();
    partial void OnPhuCap_TrucHotlineChanging(System.Nullable<decimal> value);
    partial void OnPhuCap_TrucHotlineChanged();
    partial void OnThuong_DuAn_MaxChanging(System.Nullable<decimal> value);
    partial void OnThuong_DuAn_MaxChanged();
    partial void OnLuongDongBHChanging(System.Nullable<decimal> value);
    partial void OnLuongDongBHChanged();
    partial void OnNganSach_MaxChanging(System.Nullable<decimal> value);
    partial void OnNganSach_MaxChanged();
'''

if 'PhuCap_RnD' not in cs:
    cs = cs.replace('private string _taikhoan;', 'private string _taikhoan;\n' + props + events)
    with open(r'd:\ADMIN\Documents\FreeLancer\QuangAnhMT.com\QuangAnhMT.com\App_Code\db.designer.cs', 'w', encoding='utf-8') as f:
        f.write(cs)

