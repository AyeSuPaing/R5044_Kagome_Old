/*
=========================================================================================================
  Module      : マスタダウンロード系の出力コントローラ(MasterDownLoadPanel.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Manager.Menu;
using w2.Domain.MasterExportSetting;

/// <summary>
/// マスタ出力用パネルコントロール
/// </summary>
public partial class Form_Common_MasterDownLoadPanel : BaseUserControl
{
	#region InnerClass
	/// <summary>
	/// マスタ出力設定のDataRowViewラッパー
	/// </summary>
	/// <remarks>
	/// 各項目へのアクセスをやりやすくしたもの
	/// </remarks>
	private class MasterExportSettingDataRowViewWrapper
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private MasterExportSettingDataRowViewWrapper()
		{

		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dataRowView">マスタ出力設定情報</param>
		public MasterExportSettingDataRowViewWrapper(DataRowView dataRowView)
		{
			this.Data = dataRowView;
		}
		#endregion

		#region プロパティ
		/// <summary>データ</summary>
		public DataRowView Data { get; set; }

		/// <summary>店舗ID</summary>
		public string ShopID
		{
			get { return StringUtility.ToEmpty(Data[Constants.FIELD_MASTEREXPORTSETTING_SHOP_ID]); }
		}
		/// <summary>マスタ種別</summary>
		public string MasterKbn
		{
			get { return StringUtility.ToEmpty(Data[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN]); }
		}
		/// <summary>設定ID</summary>
		public string SettingId
		{
			get { return StringUtility.ToEmpty(Data[Constants.FIELD_MASTEREXPORTSETTING_SETTING_ID]); }
		}
		/// <summary>設定名</summary>
		public string SettingName
		{
			get { return StringUtility.ToEmpty(Data[Constants.FIELD_MASTEREXPORTSETTING_SETTING_NAME]); }
		}
		/// <summary>出力項目</summary>
		public string SettingFields
		{
			get { return StringUtility.ToEmpty(Data[Constants.FIELD_MASTEREXPORTSETTING_FIELDS]); }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return StringUtility.ToEmpty(Data[Constants.FIELD_MASTEREXPORTSETTING_DEL_FLG]); }
		}
		/// <summary>出力ファイル種別</summary>
		public string ExportFileType
		{
			get { return StringUtility.ToEmpty(Data[Constants.FIELD_MASTEREXPORTSETTING_EXPORT_FILE_TYPE]); }
		}
		#endregion
	}
	#endregion

	#region 列挙体
	/// <summary>
	/// マスタダウンロード種別
	/// </summary>
	public enum MasterDownloadType
	{
		/// <summary>アフィリエイト連携ログ</summary>
		AffiliateCoopLog,
		/// <summary>ターゲットリスト情報</summary>
		TargetListData,
		/// <summary>広告コード</summary>
		AdvCode,
		/// <summary>AdvCodeMediaType</summary>
		AdvCodeMediaType,
		/// <summary>ユーザーポイント情報</summary>
		UserPoint,
		/// <summary>ユーザーポイント情報(詳細)</summary>
		UserPointDetail,
		/// <summary>クーポン情報</summary>
		Coupon,
		/// <summary>ユーザクーポン情報</summary>
		UserCoupon,
		/// <summary>クーポン利用ユーザー情報</summary>
		CouponUseUser,
	}
	#endregion

	#region イベント
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// ドロップダウンのバインド
			DdlBind();

			// ドロップダウンに一個もアイテムなければ非表示
			this.tMasterDownload.Visible = ddlExportSetting.Items.Count > 1;

			// ドロップダウンの選択肢が1個だけなら、ブランクを入れない
			if (ddlExportSetting.Items.Count == 2)
			{
				ddlExportSetting.Items.RemoveAt(0);
			}

			this.DataBind();
		}
	}

	/// <summary>
	/// 出力ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnExport_Click(object sender, EventArgs e)
	{
		// 選択してなければエラー画面へ
		if (string.IsNullOrEmpty(ddlExportSetting.SelectedValue))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MASTEREXPORTSETTING_SETTING_ID_NOT_SELECTED);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		Hashtable param = new Hashtable();

		// イベントがある場合のみ実行
		if (OnCreateSearchInputParams != null)
		{
			param = OnCreateSearchInputParams();
		}

		param[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID] = int.Parse(ddlExportSetting.SelectedValue.Split('-')[0]) - 1;
		param[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN] = ddlExportSetting.SelectedValue.Split('-')[1];
		Session[Constants.SESSION_KEY_PARAM] = param;

		// 出力ページへ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MASTEREXPORT);
	}
	#endregion

	#region -DdlBind ドロップダウンバインド
	/// <summary>
	/// ドロップダウンバインド
	/// </summary>
	private void DdlBind()
	{
		this.ddlExportSetting.Items.Add(new ListItem("", ""));
		this.ddlExportSetting.Items.AddRange(this.CreateDdlItems(this.DownloadType));
		this.ddlExportSetting.DataBind();
	}
	#endregion

	#region -CreateDdlItems ドロップダウン用のアイテム生成
	/// <summary>
	/// ドロップダウン用のアイテム生成
	/// </summary>
	/// <param name="type">ダウンロード種別</param>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItems(MasterDownloadType type)
	{
		switch (type)
		{
			case MasterDownloadType.AffiliateCoopLog:
				return CreateDdlItemForAffiliateCoopLog();

			case MasterDownloadType.TargetListData:
				return CreateDdlItemForTargetList();

			case MasterDownloadType.AdvCode:
				return CreateDdlItemForAdvCode();

			case MasterDownloadType.AdvCodeMediaType:
				return CreateDdlItemForAdvCodeMediaType();

			case MasterDownloadType.UserPoint:
				return CreateDdlItemForUserPoint();

			case MasterDownloadType.Coupon:
				return CreateDdlItemForCoupon();

			case MasterDownloadType.UserCoupon:
				return CreateDdlItemForUserCoupon();

			case MasterDownloadType.CouponUseUser:
				return CreateDdlItemForCouponUseUser();
		}

		return new ListItem[] { };
	}

	/// <summary>
	/// ドロップダウン用のアイテム生成
	/// </summary>
	/// <param name="masterKbn">DBではなくSQL振り分けのためのマスター種別</param>
	/// <param name="dbMasterKbn">DB上のマスター種別</param>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItems(string masterKbn, string dbMasterKbn)
	{
		var valueText = ValueText.GetValueText(
			Constants.TABLE_MASTEREXPORTSETTING,
			Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN,
			dbMasterKbn);
		var result = new MasterExportSettingService()
			.GetAllByMaster(this.LoginOperatorShopId, dbMasterKbn)
			.Select(item => CreateDdlItem(item, valueText, masterKbn))
			.ToArray();

		return result;
	}
	#endregion

	#region -CreateDdlItem ドロップダウン用のアイテム生成（単一)
	/// <summary>
	/// ドロップダウン用のアイテム生成（単一)
	/// </summary>
	/// <param name="setting">設定ID</param>
	/// <param name="exportName">出力名</param>
	/// <param name="masterKbn">DBではなくSQL振り分けのためのマスター種別</param>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem CreateDdlItem(MasterExportSettingModel setting, string exportName, string masterKbn)
	{
		return new ListItem(
			(setting.SettingId == Constants.MASTEREXPORTSETTING_MASTER_SETTING_ID) ? string.Format("{0}){0}", exportName) : string.Format("{0}){1}", exportName, setting.SettingName),
			string.Format("{0}-{1}", setting.SettingId, masterKbn));
	}
	#endregion

	#region -CreateDdlItemFor*** ドロップダウン用のアイテム生成（個別)
	/// <summary>
	/// ダウンロード種別がアフィリエイト連携ログの場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForAffiliateCoopLog()
	{
		var result = new List<ListItem>();
		if (Constants.W2MP_MULTIPURPOSE_AFFILIATE_OPTION_ENABLED)
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_AFFILIATECOOPLOG, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_AFFILIATECOOPLOG));
		}
		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別がターゲットリストの場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForTargetList()
	{
		var result = new List<ListItem>();
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, Constants.PATH_ROOT_MP + Constants.PAGE_W2MP_MANAGER_TARGETLIST_LIST, Constants.KBN_MENU_FUNCTION_TARGETLIST_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA));
		}
		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別が広告コードの場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForAdvCode()
	{
		var result = new List<ListItem>();
		if (Constants.W2MP_AFFILIATE_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ADVCODE_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE));
		}

		return result.ToArray();
	}

	/// <summary>
	/// Create dropdown list item For Advertisement Code Media Type
	/// </summary>
	/// <returns>Dropdown list item</returns>
	private ListItem[] CreateDdlItemForAdvCodeMediaType()
	{
		var result = new List<ListItem>();
		if (Constants.W2MP_AFFILIATE_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ADVCODE_MEDIA_TYPE_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE_MEDIA_TYPE, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE_MEDIA_TYPE));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別がユーザーポイント情報の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForUserPoint()
	{
		var result = new List<ListItem>();
		if (Constants.W2MP_POINT_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_USERPOINT_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT));
		}

		if (Constants.W2MP_POINT_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_USERPOINT_DETAIL_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT_DETAIL, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT_DETAIL));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別がクーポン情報の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForCoupon()
	{
		var result = new List<ListItem>();
		if (Constants.W2MP_COUPON_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_COUPON_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別がユーザクーポン情報の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForUserCoupon()
	{
		var result = new List<ListItem>();
		if (Constants.W2MP_COUPON_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_USERCOUPON_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERCOUPON, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERCOUPON));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別がクーポン利用ユーザー情報の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForCouponUseUser()
	{
		var result = Constants.W2MP_COUPON_OPTION_ENABLED
			? CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON_USE_USER, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON_USE_USER)
			: new ListItem[0];
		return result;
	}
	#endregion

	#region プロパティ
	/// <summary>ログインオペレータメニュー</summary>
	protected List<MenuLarge> LoginOperatorMenu
	{
		get { return (List<MenuLarge>)Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU]; }
	}
	/// <summary>ログインオペレータメニュー権限</summary>
	protected string LoginOperatorMenuAccessLevel
	{
		get { return Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU_ACCESS_LEVEL].ToString(); }
	}
	/// <summary>
	/// 検索用ハッシュテーブル作成処理のためのデリゲート
	/// </summary>
	/// <returns></returns>
	public delegate Hashtable CreateSearchParamsHandler();
	/// <summary>
	/// 検索用ハッシュテーブル作成処理イベント
	/// 利用側で処理割り当て
	/// </summary>
	public event CreateSearchParamsHandler OnCreateSearchInputParams;
	/// <summary>マスタダウンロード種別</summary>
	public MasterDownloadType DownloadType { get; set; }
	/// <summary>テーブルサイズ</summary>
	public string TableWidth { get; set; }
	#endregion
}