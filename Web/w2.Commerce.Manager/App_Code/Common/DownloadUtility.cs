/*
=========================================================================================================
  Module      : Download utility(DownloadUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Manager.Menu;
using w2.App.Common.Order.Workflow;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.MasterExportSetting;
using w2.Domain.ShopOperator;

/// <summary>
/// Download utility
/// </summary>
public class DownloadUtility
{
	#region 列挙体
	/// <summary>
	/// マスタダウンロード種別
	/// </summary>
	public enum MasterDownloadType
	{
		/// <summary>商品</summary>
		Product,
		/// <summary>商品バリエーション</summary>
		//ProductVariation,
		/// <summary>商品タグ</summary>
		//ProductTag,
		/// <summary>商品拡張項目</summary>
		//ProductExtend,
		/// <summary>商品在庫</summary>
		ProductStock,
		/// <summary>商品カテゴリー</summary>
		ProductCategory,
		/// <summary>商品価格</summary>
		//ProductPrice,
		/// <summary>注文</summary>
		Order,
		/// <summary>注文商品</summary>
		//OrderItem,
		/// <summary>注文セットプロモーション</summary>
		//OrderSetPromotion,
		/// <summary>注文ワークフロー</summary>
		OrderWorkflow,
		/// <summary>注文商品ワークフロー</summary>
		//OrderItemWorkflow,
		/// <summary>注文セットプロモーションワークフロー</summary>
		//OrderSetPromotionWorkflow,
		/// <summary>ユーザー</summary>
		User,
		/// <summary>発注情報</summary>
		StockOrder,
		/// <summary>発注商品情報</summary>
		//StockOrderItem,
		/// <summary>商品+バリエーション</summary>
		//ProductView,
		/// <summary>モール出品設定情報</summary>
		//MallExhibitsConfig,
		/// <summary>入荷通知メール情報</summary>
		UserProductArrivalMail,
		/// <summary>商品レビュー</summary>
		ProductReview,
		/// <summary>商品セール価格</summary>
		ProductSalePrice,
		/// <summary>ショートURL</summary>
		ShortUrl,
		/// <summary>定期購入マスタ</summary>
		FixedPurchase,
		/// <summary>定期購入ワークフロー</summary>
		FixedPurchaseWorkflow,
		/// <summary>定期購入商品マスタ</summary>
		//FixedPurchaseItem,
		/// <summary>シリアルキーマスタ</summary>
		SerialKey,
		/// <summary>メッセージマスタ</summary>
		//CsMessage,
		/// <summary>リアル店舗マスタ</summary>
		RealShop,
		/// <summary>リアル店舗商品在庫マスタ</summary>
		RealShopProductStock,
	}
	#endregion

	/// <summary>
	/// Create items
	/// </summary>
	/// <param name="type">Workflow type</param>
	/// <returns>List items</returns>
	public Dictionary<string, string> CreateItems(string type)
	{
		var workflowType = (type == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
			? MasterDownloadType.OrderWorkflow
			: MasterDownloadType.FixedPurchaseWorkflow;
		var results = CreateDdlItems(workflowType);
		return results.ToDictionary(item => item.Value, item => item.Text);
	}

	/// <summary>
	/// ドロップダウン用のアイテム生成
	/// </summary>
	/// <param name="type">ダウンロード種別</param>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItems(MasterDownloadType type)
	{
		switch (type)
		{
			case MasterDownloadType.OrderWorkflow:
				return CreateDdlItemForOrderWorkFlow();

			case MasterDownloadType.FixedPurchaseWorkflow:
				return CreateDdlItemFixedPurchaseWorkFlow();

			default:
				return new ListItem[]
				{
					new ListItem(string.Empty, string.Empty)
				};
		}
	}

	/// <summary>
	/// ダウンロード種別が注文の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForOrderWorkFlow()
	{
		var result = new List<ListItem>();

		// 注文
		if (MenuUtility.HasAuthority(
			this.LoginOperatorMenu,
			this.RawUrl,
			Constants.KBN_MENU_FUNCTION_ORDERWF_DL))
		{
			result.AddRange(CreateDdlItems(
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER_WORKFLOW,
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER));
		}

		// 注文商品
		if (MenuUtility.HasAuthority(
			this.LoginOperatorMenu,
			this.RawUrl,
			Constants.KBN_MENU_FUNCTION_ORDERWF_ITEM_DL))
		{
			result.AddRange(CreateDdlItems(
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM_WORKFLOW,
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM));
		}

		// 注文セットプロモーション
		if (Constants.SETPROMOTION_OPTION_ENABLED
			&& MenuUtility.HasAuthority(
				this.LoginOperatorMenu,
				this.RawUrl,
				Constants.KBN_MENU_FUNCTION_ORDERWF_SETPROMOTION_DL))
		{
			result.AddRange(CreateDdlItems(
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION_WORKFLOW,
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION));
		}

		// データ結合マスタ
		if (Constants.MASTEREXPORT_DATABINDING_OPTION_ENABLE
			&& MenuUtility.HasAuthority(
				this.LoginOperatorMenu,
				this.RawUrl,
				Constants.KBN_MENU_FUNCTION_DATABINDING_DL))
		{
			result.AddRange(CreateDdlItems(
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING_WORKFLOW,
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING));
		}
		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別が定期の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemFixedPurchaseWorkFlow()
	{
		var result = new List<ListItem>();

		// 定期
		if (MenuUtility.HasAuthority(
			this.LoginOperatorMenu,
			this.RawUrl,
			Constants.KBN_MENU_FUNCTION_FIXEDPURCHASEWF_DL))
		{
			result.AddRange(CreateDdlItems(
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE_WORKFLOW,
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE));
		}

		// 定期詳細
		if (MenuUtility.HasAuthority(
			this.LoginOperatorMenu,
			this.RawUrl,
			Constants.KBN_MENU_FUNCTION_FIXEDPURCHASEWF_ITEM_DL))
		{
			result.AddRange(CreateDdlItems(
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM_WORKFLOW,
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ドロップダウン用のアイテム生成
	/// </summary>
	/// <param name="masterKbn">DBではなくSQL振り分けのためのマスター種別</param>
	/// <param name="dbMasterKbn">DB上のマスター種別</param>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItems(string masterKbn, string dbMasterKbn)
	{
		var setting = DomainFacade.Instance.MasterExportSettingService.GetAllByMaster(
			this.LoginOperatorShopId,
			dbMasterKbn);

		var valueText = ValueText.GetValueText(
			Constants.TABLE_MASTEREXPORTSETTING,
			Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN,
			dbMasterKbn);

		var dropdownItem = setting
			.Select(item => CreateDdlItem(item, valueText, masterKbn))
			.ToArray();
		return dropdownItem;
	}

	/// <summary>
	/// ドロップダウン用のアイテム生成（単一)
	/// </summary>
	/// <param name="setting">設定ID</param>
	/// <param name="exportName">出力名</param>
	/// <param name="masterKbn">DBではなくSQL振り分けのためのマスター種別</param>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem CreateDdlItem(
		MasterExportSettingModel setting,
		string exportName,
		string masterKbn)
	{
		return new ListItem(
			(setting.SettingId == Constants.MASTEREXPORTSETTING_MASTER_SETTING_ID)
				? string.Format("{0}){0}", exportName)
				: string.Format("{0}){1}", exportName, setting.SettingName),
			string.Format("{0}-{1}", setting.SettingId, masterKbn));
	}

	/// <summary>
	/// Create search input params
	/// </summary>
	/// <param name="workflowType">Workflow type</param>
	/// <param name="workflowUtility">Workflow utility</param>
	public void CreateSearchInputParams(string workflowType, WorkflowUtility workflowUtility)
	{
		if (workflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
		{
			this.OnCreateSearchInputParams += workflowUtility.GetSearchParamsOrder;
		}
		else
		{
			this.OnCreateSearchInputParams += workflowUtility.GetSearchParamsFixedPurchase;
		}
	}

	/// <summary>
	/// Execute export
	/// </summary>
	/// <param name="key">Key export setting selected</param>
	/// <returns>A hashtable of param</returns>
	public Hashtable ExecuteExport(string key)
	{
		// 選択してなければエラー画面へ
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}

		var param = new Hashtable();

		// イベントがある場合のみ実行
		if (OnCreateSearchInputParams != null)
		{
			param = OnCreateSearchInputParams();
		}

		param[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID] = (int.Parse(key.Split('-')[0]) - 1);
		param[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN] = key.Split('-')[1];
		return param;
	}

	#region プロパティ
	/// <summary>Login operator menu</summary>
	public List<MenuLarge> LoginOperatorMenu
	{
		get
		{
			return (List<MenuLarge>)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU];
		}
	}
	/// <summary>RawUrl（IISのバージョンによる機能の違いを吸収）</summary>
	public string RawUrl
	{
		get { return WebUtility.GetRawUrl(Request); }
	}
	/// <summary>ログインオペレータ店舗ID</summary>
	public string LoginOperatorShopId
	{
		get { return this.LoginShopOperator.ShopId; }
	}
	/// <summary>ログイン店舗オペレータ</summary>
	public ShopOperatorModel LoginShopOperator
	{
		get
		{
			var shopOperator = (ShopOperatorModel)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR]
				?? new ShopOperatorModel();
			return shopOperator;
		}
	}
	/// <summary>Request</summary>
	public HttpRequest Request
	{
		get { return HttpContext.Current.Request; }
	}

	/// <summary>
	/// 検索用ハッシュテーブル作成処理のためのデリゲート
	/// </summary>
	/// <returns></returns>
	public delegate Hashtable CreateSearchParamsHandler(string masterKbn = null);

	/// <summary>
	/// 検索用ハッシュテーブル作成処理イベント
	/// 利用側で処理割り当て
	/// </summary>
	public event CreateSearchParamsHandler OnCreateSearchInputParams;
	#endregion
}
