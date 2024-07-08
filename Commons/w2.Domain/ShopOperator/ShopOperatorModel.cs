/*
=========================================================================================================
  Module      : 店舗管理者マスタモデル (ShopOperatorModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ShopOperator
{
	/// <summary>
	/// 店舗管理者マスタモデル
	/// </summary>
	[Serializable]
	public partial class ShopOperatorModel : ModelBase<ShopOperatorModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ShopOperatorModel()
		{
			this.ShopId = "";
			this.OperatorId = "";
			this.Name = "";
			this.MailAddr = "";
			this.MenuAccessLevel1 = null;
			this.MenuAccessLevel2 = null;
			this.MenuAccessLevel3 = null;
			this.MenuAccessLevel4 = null;
			this.MenuAccessLevel5 = null;
			this.MenuAccessLevel6 = null;
			this.MenuAccessLevel7 = null;
			this.MenuAccessLevel8 = null;
			this.MenuAccessLevel9 = null;
			this.MenuAccessLevel10 = null;
			this.LoginId = "";
			this.Password = "";
			this.OdbcUserName = "";
			this.OdbcPassword = "";
			this.ValidFlg = Constants.FLG_ON;
			this.DelFlg = Constants.FLG_OFF;
			this.LastChanged = "";
			this.UsableAffiliateTagIdsInReport = "";
			this.UsableAdvcodeMediaTypeIds = string.Empty;
			this.UsableOutputLocations = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShopOperatorModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShopOperatorModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_SHOP_ID] = value; }
		}
		/// <summary>オペレータID</summary>
		public string OperatorId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID] = value; }
		}
		/// <summary>オペレータ名</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME] = value; }
		}
		/// <summary>メールアドレス</summary>
		public string MailAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_MAIL_ADDR]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MAIL_ADDR] = value; }
		}
		/// <summary>メニューアクセスレベル1</summary>
		public int? MenuAccessLevel1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL1] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL1];
			}
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL1] = value; }
		}
		/// <summary>メニューアクセスレベル2</summary>
		public int? MenuAccessLevel2
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL2] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL2];
			}
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL2] = value; }
		}
		/// <summary>メニューアクセスレベル3</summary>
		public int? MenuAccessLevel3
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL3] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL3];
			}
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL3] = value; }
		}
		/// <summary>メニューアクセスレベル4</summary>
		public int? MenuAccessLevel4
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL4] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL4];
			}
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL4] = value; }
		}
		/// <summary>メニューアクセスレベル5</summary>
		public int? MenuAccessLevel5
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL5] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL5];
			}
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL5] = value; }
		}
		/// <summary>メニューアクセスレベル6</summary>
		public int? MenuAccessLevel6
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL6] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL6];
			}
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL6] = value; }
		}
		/// <summary>メニューアクセスレベル7</summary>
		public int? MenuAccessLevel7
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL7] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL7];
			}
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL7] = value; }
		}
		/// <summary>メニューアクセスレベル8</summary>
		public int? MenuAccessLevel8
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL8] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL8];
			}
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL8] = value; }
		}
		/// <summary>メニューアクセスレベル9</summary>
		public int? MenuAccessLevel9
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL9] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL9];
			}
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL9] = value; }
		}
		/// <summary>メニューアクセスレベル10</summary>
		public int? MenuAccessLevel10
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL10] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL10];
			}
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL10] = value; }
		}
		/// <summary>ログインID</summary>
		public string LoginId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_LOGIN_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_LOGIN_ID] = value; }
		}
		/// <summary>パスワード</summary>
		public string Password
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_PASSWORD]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_PASSWORD] = value; }
		}
		/// <summary>ODBCユーザ名</summary>
		public string OdbcUserName
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_ODBC_USER_NAME]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_ODBC_USER_NAME] = value; }
		}
		/// <summary>ODBCパスワード</summary>
		public string OdbcPassword
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_ODBC_PASSWORD]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_ODBC_PASSWORD] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHOPOPERATOR_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHOPOPERATOR_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_LAST_CHANGED] = value; }
		}
		/// <summary>閲覧可能な広告コード</summary>
		public string UsableAdvcodeNosInReport
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_NOS_IN_REPORT]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_NOS_IN_REPORT] = value; }
		}
		/// <summary>閲覧可能なタグID</summary>
		public string UsableAffiliateTagIdsInReport
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_USABLE_AFFILIATE_TAG_IDS_IN_REPORT]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_USABLE_AFFILIATE_TAG_IDS_IN_REPORT] = value; }
		}
		/// <summary>閲覧可能な広告媒体区分ID</summary>
		public string UsableAdvcodeMediaTypeIds
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_MEDIA_TYPE_IDS]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_MEDIA_TYPE_IDS] = value; }
		}
		/// <summary>閲覧可能な設置箇所</summary>
		public string UsableOutputLocations
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_USABLE_OUTPUT_LOCATIONS]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_USABLE_OUTPUT_LOCATIONS] = value; }
		}
		/// <summary>最終ログイン日時</summary>
		public DateTime DateLastLoggedIn
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHOPOPERATOR_DATE_LAST_LOGGEDIN]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_DATE_LAST_LOGGEDIN] = value; }
		}
		/// <summary>リモートIPアドレス</summary>
		public string RemoteAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_REMOTE_ADDR]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_REMOTE_ADDR] = value; }
		}
		/// <summary>認証コード</summary>
		public string AuthenticationCode
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPOPERATOR_AUTHENTICATION_CODE]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_AUTHENTICATION_CODE] = value; }
		}
		/// <summary>認証コード送信日時</summary>
		public DateTime DateCodeSend
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHOPOPERATOR_DATE_CODE_SEND]; }
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_DATE_CODE_SEND] = value; }
		}
		#endregion
	}
}
