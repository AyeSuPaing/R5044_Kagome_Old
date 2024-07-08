/*
=========================================================================================================
  Module      : リアル店舗情報モデル (RealShopModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.RealShop
{
	/// <summary>
	/// リアル店舗情報モデル
	/// </summary>
	[Serializable]
	public partial class RealShopModel : ModelBase<RealShopModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public RealShopModel()
		{
			// TODO:定数を利用するよう書き換えてください。
			this.RealShopId = "";
			this.Name = "";
			this.NameKana = "";
			this.Desc1KbnPc = "0";
			this.Desc1Pc = "";
			this.Desc2KbnPc = "0";
			this.Desc2Pc = "";
			this.Desc1KbnSp = "0";
			this.Desc1Sp = "";
			this.Desc2KbnSp = "0";
			this.Desc2Sp = "";
			this.Desc1KbnMb = "0";
			this.Desc1Mb = "";
			this.Desc2KbnMb = "0";
			this.Desc2Mb = "";
			this.Zip = "";
			this.Zip1 = "";
			this.Zip2 = "";
			this.Addr = "";
			this.Addr1 = "";
			this.Addr2 = "";
			this.Addr3 = "";
			this.Addr4 = "";
			this.Tel = "";
			this.Tel_1 = "";
			this.Tel_2 = "";
			this.Tel_3 = "";
			this.Fax = "";
			this.Fax_1 = "";
			this.Fax_2 = "";
			this.Fax_3 = "";
			this.Url = "";
			this.MailAddr = "";
			this.OpeningHours = "";
			this.DisplayOrder = 1;
			this.ValidFlg = "1";
			this.DelFlg = "0";
			this.LastChanged = "";
			this.CountryIsoCode = "";
			this.CountryName = "";
			this.Addr5 = "";
			this.AreaId = string.Empty;
			this.BrandId = string.Empty;
			this.Longitude = 0m;
			this.Latitude = 0m;
			this.DateCreated = DateTime.Now;
			this.DateChanged = DateTime.Now;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RealShopModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RealShopModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>リアル店舗ID</summary>
		public string RealShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_REAL_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_REAL_SHOP_ID] = value; }
		}
		/// <summary>店舗名</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_NAME]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_NAME] = value; }
		}
		/// <summary>店舗名かな</summary>
		public string NameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_NAME_KANA] = value; }
		}
		/// <summary>PC説明1HTML区分</summary>
		public string Desc1KbnPc
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DESC1_KBN_PC]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DESC1_KBN_PC] = value; }
		}
		/// <summary>PC説明1</summary>
		public string Desc1Pc
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DESC1_PC]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DESC1_PC] = value; }
		}
		/// <summary>PC説明2HTML区分</summary>
		public string Desc2KbnPc
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DESC2_KBN_PC]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DESC2_KBN_PC] = value; }
		}
		/// <summary>PC説明2</summary>
		public string Desc2Pc
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DESC2_PC]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DESC2_PC] = value; }
		}
		/// <summary>SP説明1HTML区分</summary>
		public string Desc1KbnSp
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DESC1_KBN_SP]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DESC1_KBN_SP] = value; }
		}
		/// <summary>SP説明1</summary>
		public string Desc1Sp
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DESC1_SP]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DESC1_SP] = value; }
		}
		/// <summary>SP説明2HTML区分</summary>
		public string Desc2KbnSp
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DESC2_KBN_SP]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DESC2_KBN_SP] = value; }
		}
		/// <summary>SP説明2</summary>
		public string Desc2Sp
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DESC2_SP]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DESC2_SP] = value; }
		}
		/// <summary>MB説明1HTML区分</summary>
		public string Desc1KbnMb
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DESC1_KBN_MB]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DESC1_KBN_MB] = value; }
		}
		/// <summary>MB説明1</summary>
		public string Desc1Mb
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DESC1_MB]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DESC1_MB] = value; }
		}
		/// <summary>MB説明2HTML区分</summary>
		public string Desc2KbnMb
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DESC2_KBN_MB]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DESC2_KBN_MB] = value; }
		}
		/// <summary>MB説明2</summary>
		public string Desc2Mb
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DESC2_MB]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DESC2_MB] = value; }
		}
		/// <summary>郵便番号</summary>
		public string Zip
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_ZIP]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_ZIP] = value; }
		}
		/// <summary>郵便番号1</summary>
		public string Zip1
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_ZIP1]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_ZIP1] = value; }
		}
		/// <summary>郵便番号2</summary>
		public string Zip2
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_ZIP2]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_ZIP2] = value; }
		}
		/// <summary>住所</summary>
		public string Addr
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_ADDR]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_ADDR] = value; }
		}
		/// <summary>住所1</summary>
		public string Addr1
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_ADDR1]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_ADDR1] = value; }
		}
		/// <summary>住所2</summary>
		public string Addr2
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_ADDR2]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_ADDR2] = value; }
		}
		/// <summary>住所3</summary>
		public string Addr3
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_ADDR3]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_ADDR3] = value; }
		}
		/// <summary>住所4</summary>
		public string Addr4
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_ADDR4]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_ADDR4] = value; }
		}
		/// <summary>電話番号</summary>
		public string Tel
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_TEL]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_TEL] = value; }
		}
		/// <summary>電話番号1</summary>
		public string Tel_1
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_TEL_1]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_TEL_1] = value; }
		}
		/// <summary>電話番号2</summary>
		public string Tel_2
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_TEL_2]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_TEL_2] = value; }
		}
		/// <summary>電話番号3</summary>
		public string Tel_3
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_TEL_3]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_TEL_3] = value; }
		}
		/// <summary>FAX</summary>
		public string Fax
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_FAX]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_FAX] = value; }
		}
		/// <summary>FAX1</summary>
		public string Fax_1
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_FAX_1]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_FAX_1] = value; }
		}
		/// <summary>FAX2</summary>
		public string Fax_2
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_FAX_2]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_FAX_2] = value; }
		}
		/// <summary>FAX3</summary>
		public string Fax_3
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_FAX_3]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_FAX_3] = value; }
		}
		/// <summary>URL</summary>
		public string Url
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_URL]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_URL] = value; }
		}
		/// <summary>メールアドレス</summary>
		public string MailAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_MAIL_ADDR]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_MAIL_ADDR] = value; }
		}
		/// <summary>営業時間</summary>
		public string OpeningHours
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_OPENING_HOURS]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_OPENING_HOURS] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_REALSHOP_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_REALSHOP_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_REALSHOP_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_LAST_CHANGED] = value; }
		}
		/// <summary>国ISOコード</summary>
		public string CountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>国名</summary>
		public string CountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_COUNTRY_NAME]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_COUNTRY_NAME] = value; }
		}
		/// <summary>住所5</summary>
		public string Addr5
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_ADDR5]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_ADDR5] = value; }
		}
		/// <summary>Area id</summary>
		public string AreaId
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_AREA_ID]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_AREA_ID] = value; }
		}
		/// <summary>Brand id</summary>
		public string BrandId
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOP_BRAND_ID]; }
			set { this.DataSource[Constants.FIELD_REALSHOP_BRAND_ID] = value; }
		}
		/// <summary>Longitude</summary>
		public decimal? Longitude
		{
			get
			{
				if (this.DataSource[Constants.FIELD_REALSHOP_LONGITUDE] == DBNull.Value) return null;
				return (decimal)this.DataSource[Constants.FIELD_REALSHOP_LONGITUDE];
			}
			set { this.DataSource[Constants.FIELD_REALSHOP_LONGITUDE] = value; }
		}
		/// <summary>Latitude</summary>
		public decimal? Latitude
		{
			get
			{
				if (this.DataSource[Constants.FIELD_REALSHOP_LATITUDE] == DBNull.Value) return null;
				return (decimal)this.DataSource[Constants.FIELD_REALSHOP_LATITUDE];
			}
			set { this.DataSource[Constants.FIELD_REALSHOP_LATITUDE] = value; }
		}
		#endregion
	}
}
