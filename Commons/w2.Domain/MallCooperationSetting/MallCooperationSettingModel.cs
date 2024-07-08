/*
=========================================================================================================
  Module      : モール連携設定モデル (MallCooperationSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MallCooperationSetting
{
	/// <summary>
	/// モール連携設定モデル
	/// </summary>
	public partial class MallCooperationSettingModel : ModelBase<MallCooperationSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MallCooperationSettingModel()
		{
			this.PopPort = Constants.FLG_MALLCOOPERATIONSETTING_POP_PORT_DEFAULT;
			this.PopApopFlg = Constants.FLG_MALLCOOPERATIONSETTING_POP_APOP_FLG_APOP;
			this.CnvidRtnNUpdItemcsv = null;
			this.CnvidRtnNStkItemcsv = null;
			this.CnvidRtnVInsItemcsv = null;
			this.CnvidRtnVInsSelectcsv = null;
			this.CnvidRtnVUpdItemcsv = null;
			this.CnvidRtnVStkSelectcsv = null;
			this.CnvidRtnItemcatcsv = null;
			this.CnvidYhoNInsDatacsv = null;
			this.CnvidYhoNInsStockcsv = null;
			this.CnvidYhoNUpdDatacsv = null;
			this.CnvidYhoNStkDatacsv = null;
			this.CnvidYhoVInsDatacsv = null;
			this.CnvidYhoVInsStockcsv = null;
			this.CnvidYhoVUpdDatacsv = null;
			this.CnvidYhoVStkDatacsv = null;
			this.MaintenanceDateFrom = null;
			this.MaintenanceDateTo = null;
			this.ValidFlg = Constants.FLG_MALLCOOPERATIONSETTING_VALID_FLG_VALID;
			this.DelFlg = "0";
			this.LastProductLogNo = null;
			this.LastProductvariationLogNo = null;
			this.LastProductstockLogNo = null;
			this.StockUpdateUseFlg = Constants.FLG_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG_INVALID;
			this.LohacoPrivateKey = string.Empty;
			this.SftpPort = 22;
			this.NextEngineStockStoreAccount = string.Empty;
			this.NextEngineStockAuthKey = string.Empty;
			this.YahooApiClientId = string.Empty;
			this.YahooApiClientSecret = string.Empty;
			this.YahooApiAccessToken = string.Empty;
			this.YahooApiRefreshToken = string.Empty;
			this.YahooApiSellerId = string.Empty;
			this.YahooApiPublicKey = string.Empty;
			this.YahooApiPublicKeyVersion = string.Empty;
			this.YahooApiLastPublicKeyAuthorizedAt = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MallCooperationSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MallCooperationSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID] = value; }
		}
		/// <summary>モールID</summary>
		public string MallId
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] = value; }
		}
		/// <summary>モール区分</summary>
		public string MallKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN] = value; }
		}
		/// <summary>モール名</summary>
		public string MallName
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME] = value; }
		}
		/// <summary>モール出品設定</summary>
		public string MallExhibitsConfig
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG] = value; }
		}
		/// <summary>対象メールアドレス</summary>
		public string TgtMailAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_TGT_MAIL_ADDR]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_TGT_MAIL_ADDR] = value; }
		}
		/// <summary>受信POPサーバ</summary>
		public string PopServer
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_POP_SERVER]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_POP_SERVER] = value; }
		}
		/// <summary>受信POPポート</summary>
		public int PopPort
		{
			get { return (int)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_POP_PORT]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_POP_PORT] = value; }
		}
		/// <summary>受信POPユーザ名</summary>
		public string PopUserName
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_POP_USER_NAME]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_POP_USER_NAME] = value; }
		}
		/// <summary>受信POPパスワード</summary>
		public string PopPassword
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_POP_PASSWORD]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_POP_PASSWORD] = value; }
		}
		/// <summary>受信APOPフラグ</summary>
		public string PopApopFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_POP_APOP_FLG]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_POP_APOP_FLG] = value; }
		}
		/// <summary>FTPホスト名（アドレス）</summary>
		public string FtpHost
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_HOST]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_HOST] = value; }
		}
		/// <summary>FTPユーザー名</summary>
		public string FtpUserName
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_USER_NAME]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_USER_NAME] = value; }
		}
		/// <summary>FTPパスワード</summary>
		public string FtpPassword
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_PASSWORD]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_PASSWORD] = value; }
		}
		/// <summary>FTPアップロード先ディレクトリ</summary>
		public string FtpUploadDir
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_UPLOAD_DIR]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_UPLOAD_DIR] = value; }
		}
		/// <summary>受注情報取込設定</summary>
		public string OrderImportSetting
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ORDER_IMPORT_SETTING]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ORDER_IMPORT_SETTING] = value; }
		}
		/// <summary>その他設定</summary>
		public string OtherSetting
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_OTHER_SETTING]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_OTHER_SETTING] = value; }
		}
		/// <summary>商品コンバータID：楽天2</summary>
		public int? CnvidRtnNUpdItemcsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_UPD_ITEMCSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_UPD_ITEMCSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_UPD_ITEMCSV] = value; }
		}
		/// <summary>商品コンバータID：楽天3</summary>
		public int? CnvidRtnNStkItemcsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_STK_ITEMCSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_STK_ITEMCSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_STK_ITEMCSV] = value; }
		}
		/// <summary>商品コンバータID：楽天4</summary>
		public int? CnvidRtnVInsItemcsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_ITEMCSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_ITEMCSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_ITEMCSV] = value; }
		}
		/// <summary>商品コンバータID：楽天5</summary>
		public int? CnvidRtnVInsSelectcsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_SELECTCSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_SELECTCSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_SELECTCSV] = value; }
		}
		/// <summary>商品コンバータID：楽天6</summary>
		public int? CnvidRtnVUpdItemcsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_UPD_ITEMCSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_UPD_ITEMCSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_UPD_ITEMCSV] = value; }
		}
		/// <summary>商品コンバータID：楽天8</summary>
		public int? CnvidRtnVStkSelectcsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_STK_SELECTCSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_STK_SELECTCSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_STK_SELECTCSV] = value; }
		}
		/// <summary>商品コンバータID：楽天9</summary>
		public int? CnvidRtnItemcatcsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_ITEMCATCSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_ITEMCATCSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_ITEMCATCSV] = value; }
		}
		/// <summary>商品コンバータID：Yahoo!1</summary>
		public int? CnvidYhoNInsDatacsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_DATACSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_DATACSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_DATACSV] = value; }
		}
		/// <summary>商品コンバータID：Yahoo!2</summary>
		public int? CnvidYhoNInsStockcsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_STOCKCSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_STOCKCSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_STOCKCSV] = value; }
		}
		/// <summary>商品コンバータID：Yahoo!3</summary>
		public int? CnvidYhoNUpdDatacsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_UPD_DATACSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_UPD_DATACSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_UPD_DATACSV] = value; }
		}
		/// <summary>商品コンバータID：Yahoo!4</summary>
		public int? CnvidYhoNStkDatacsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_STK_DATACSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_STK_DATACSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_STK_DATACSV] = value; }
		}
		/// <summary>商品コンバータID：Yahoo!5</summary>
		public int? CnvidYhoVInsDatacsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_DATACSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_DATACSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_DATACSV] = value; }
		}
		/// <summary>商品コンバータID：Yahoo!6</summary>
		public int? CnvidYhoVInsStockcsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_STOCKCSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_STOCKCSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_STOCKCSV] = value; }
		}
		/// <summary>商品コンバータID：Yahoo!7</summary>
		public int? CnvidYhoVUpdDatacsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_UPD_DATACSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_UPD_DATACSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_UPD_DATACSV] = value; }
		}
		/// <summary>商品コンバータID：Yahoo!8</summary>
		public int? CnvidYhoVStkDatacsv
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_STK_DATACSV] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_STK_DATACSV];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_STK_DATACSV] = value; }
		}
		/// <summary>メンテナンス開始日</summary>
		public DateTime? MaintenanceDateFrom
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_FROM] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_FROM];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_FROM] = value; }
		}
		/// <summary>メンテナンス終了日</summary>
		public DateTime? MaintenanceDateTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_TO] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_TO];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_TO] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_DEL_FLG] = value; }
		}
		/// <summary>最終商品マスタログNO</summary>
		public long? LastProductLogNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_PRODUCT_LOG_NO] == DBNull.Value) return null;
				return (long?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_PRODUCT_LOG_NO];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_PRODUCT_LOG_NO] = value; }
		}
		/// <summary>最終商品バリエーションマスタログNO</summary>
		public long? LastProductvariationLogNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_PRODUCTVARIATION_LOG_NO] == DBNull.Value) return null;
				return (long?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_PRODUCTVARIATION_LOG_NO];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_PRODUCTVARIATION_LOG_NO] = value; }
		}
		/// <summary>最終商品在庫マスタログNO</summary>
		public long? LastProductstockLogNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_PRODUCTSTOCK_LOG_NO] == DBNull.Value) return null;
				return (long?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_PRODUCTSTOCK_LOG_NO];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_PRODUCTSTOCK_LOG_NO] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_CHANGED] = value; }
		}
		/// <summary>楽天APIのユーザ名</summary>
		public string RakutenApiUserName
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_USER_NAME]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_USER_NAME] = value; }
		}
		/// <summary>楽天APIの店舗URL</summary>
		public string RakutenApiShopUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SHOP_URL]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SHOP_URL] = value; }
		}
		/// <summary>楽天APIのサービスシークレット</summary>
		public string RakutenApiServiceSecret
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SERVICE_SECRET]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SERVICE_SECRET] = value; }
		}
		/// <summary>楽天APIのライセンスキー</summary>
		public string RakutenApiLicenseKey
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_LICENSE_KEY]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_LICENSE_KEY] = value; }
		}
		/// <summary>Amazon出品者ID</summary>
		public string AmazonMerchantId
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MERCHANT_ID]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MERCHANT_ID] = value; }
		}
		/// <summary>AmazonマーケットプレイスID</summary>
		public string AmazonMarketplaceId
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MARKETPLACE_ID]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MARKETPLACE_ID] = value; }
		}
		/// <summary>AmazonAWSアクセスキーID</summary>
		public string AmazonAwsAccesskeyId
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_AWS_ACCESSKEY_ID]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_AWS_ACCESSKEY_ID] = value; }
		}
		/// <summary>Amazon秘密キー</summary>
		public string AmazonSecretKey
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_SECRET_KEY]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_SECRET_KEY] = value; }
		}
		/// <summary>MWS認証トークン</summary>
		public string AmazonMwsAuthtoken
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MWS_AUTHTOKEN]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MWS_AUTHTOKEN] = value; }
		}
		/// <summary>在庫連携利用フラグ</summary>
		public string StockUpdateUseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG] = value; }
		}
		/// <summary>SFTPホスト名（アドレス）</summary>
		public string SftpHost
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_HOST]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_HOST] = value; }
		}
		/// <summary>SFTPユーザー名</summary>
		public string SftpUserName
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_USER_NAME]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_USER_NAME] = value; }
		}
		/// <summary>SFTPパスフレーズ</summary>
		public string SftpPassPhrase
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PASS_PHRASE]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PASS_PHRASE] = value; }
		}
		/// <summary>SFTPポート番号</summary>
		public int SftpPort
		{
			get { return (int)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PORT]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PORT] = value; }
		}
		/// <summary>SFTP秘密鍵ファイルパス</summary>
		public string SftpPrivateKeyFilePath
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PRIVATE_KEY_FILE_PATH]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PRIVATE_KEY_FILE_PATH] = value; }
		}
		/// <summary>＆mallのテナントコード</summary>
		public string AndmallTenantCode
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_TENANT_CODE]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_TENANT_CODE] = value; }
		}
		/// <summary>＆mallのショップID</summary>
		public string AndmallBaseStoreCode
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_BASE_STORE_CODE]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_BASE_STORE_CODE] = value; }
		}
		/// <summary>＆mallの店番</summary>
		public string AndmallShopNo
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SHOP_NO]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SHOP_NO] = value; }
		}
		/// <summary>＆mallの商品連携カラム</summary>
		public string AndmallCooperation
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_COOPERATION]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_COOPERATION] = value; }
		}
		/// <summary>＆mallの商品バリエーション連携カラム</summary>
		public string AndmallVariationCooperation
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_VARIATION_COOPERATION]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_VARIATION_COOPERATION] = value; }
		}
		/// <summary>＆mallのサイトコード</summary>
		public string AndmallSiteCode
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SITE_CODE]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SITE_CODE] = value; }
		}
		/// <summary>＆mallのサイト認証キー</summary>
		public string AndmallSignatureKey
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SIGNATURE_KEY]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SIGNATURE_KEY] = value; }
		}
		/// <summary>ロハコ秘密鍵</summary>
		public string LohacoPrivateKey
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LOHACO_API_PRIVATE_KEY];}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_LOHACO_API_PRIVATE_KEY] = value; }
		}
		/// <summary>ネクストエンジン_在庫連携_ストアアカウント</summary>
		public string NextEngineStockStoreAccount
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_NEXT_ENGINE_STOCK_STORE_ACCOUNT]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_NEXT_ENGINE_STOCK_STORE_ACCOUNT] = value; }
		}
		/// <summary>ネクストエンジン_在庫連携_認証キー</summary>
		public string NextEngineStockAuthKey
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_NEXT_ENGINE_STOCK_AUTH_KEY]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_NEXT_ENGINE_STOCK_AUTH_KEY] = value; }
		}
		/// <summary>Facebook catalog id</summary>
		public string FacebookCatalogId
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_FACEBOOK_CATALOG_ID]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_FACEBOOK_CATALOG_ID] = value; }
		}
		/// <summary>Facebook access token</summary>
		public string FacebookAccessToken
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_FACEBOOK_ACCESS_TOKEN]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_FACEBOOK_ACCESS_TOKEN] = value; }
		}
		/// <summary>Yahoo API Client ID (アプリケーションID)</summary>
		public string YahooApiClientId
		{
			get { return (string)this.DataSource["yahoo_api_client_id"]; }
			set { this.DataSource["yahoo_api_access_token"] = value; }
		}
		/// <summary>Yahoo API Client シークレット</summary>
		public string YahooApiClientSecret
		{
			get { return (string)this.DataSource["yahoo_api_client_secret"]; }
			set { this.DataSource["yahoo_api_client_secret"] = value; }
		}
		/// <summary>Yahoo API Access Token</summary>
		public string YahooApiAccessToken
		{
			get { return (string)this.DataSource["yahoo_api_access_token"]; }
			set { this.DataSource["yahoo_api_access_token"] = value; }
		}
		/// <summary>Yahoo API Access Token有効期限</summary>
		public DateTime? YahooApiAccessTokenExpirationDatetime
		{
			get
			{
				if (this.DataSource["yahoo_api_access_token_expiration_datetime"] == DBNull.Value) return null;
				return (DateTime?)this.DataSource["yahoo_api_access_token_expiration_datetime"];
			}
			set { this.DataSource["yahoo_api_access_token_expiration_datetime"] = value; }
		}
		/// <summary>Yahoo API Refresh Token</summary>
		public string YahooApiRefreshToken
		{
			get { return (string)this.DataSource["yahoo_api_refresh_token"]; }
			set { this.DataSource["yahoo_api_refresh_token"] = value; }
		}
		/// <summary>Yahoo API Refresh Token 有効期限</summary>
		public DateTime? YahooApiRefreshTokenExpirationDatetime
		{
			get
			{
				if (this.DataSource["yahoo_api_refresh_token_expiration_datetime"] == DBNull.Value) return null; 
				return (DateTime?)this.DataSource["yahoo_api_refresh_token_expiration_datetime"];
			}
			set { this.DataSource["yahoo_api_refresh_token_expiration_datetime"] = value; }
		}
		/// <summary>Yahoo API Seller ID</summary>
		public string YahooApiSellerId
		{
			get { return (string)this.DataSource["yahoo_api_seller_id"]; }
			set { this.DataSource["yahoo_api_seller_id"] = value; }
		}
		/// <summary>Yahoo API公開鍵</summary>
		public string YahooApiPublicKey
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_PUBLIC_KEY]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_PUBLIC_KEY] = value; }
		}
		/// <summary>Yahoo API公開鍵バージョン</summary>
		public string YahooApiPublicKeyVersion
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_PUBLIC_KEY_VERSION]; }
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_PUBLIC_KEY_VERSION] = value; }
		}
		/// <summary>Yahoo API公開鍵最終認証日時</summary>
		public DateTime? YahooApiLastPublicKeyAuthorizedAt
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_LAST_PUBLIC_KEY_AUTHORIZED_AT] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_LAST_PUBLIC_KEY_AUTHORIZED_AT];
			}
			set { this.DataSource[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_LAST_PUBLIC_KEY_AUTHORIZED_AT] = value; }
		}
		#endregion
	}
}
