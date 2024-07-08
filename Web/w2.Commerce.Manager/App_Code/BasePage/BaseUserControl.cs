/*
=========================================================================================================
  Module      : 基底ユーザーコントロール(BaseUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Global;
using w2.App.Common.Order;
using w2.App.Common.Global.Config;
using w2.App.Common.Option;
using w2.App.Common.Web.Page;
using w2.Domain.FieldMemoSetting;
using w2.Domain.ShopOperator;

/// <summary>
/// BaseUserControl の概要の説明です
/// </summary>
public class BaseUserControl : System.Web.UI.UserControl
{
	/// <summary>
	/// タグ置換
	/// </summary>
	/// <param name="targetString">置換対象文字列</param>
	/// <param name="countyIsoCode">国ISOコード（配送先の国によって切り替わるときに利用）</param>
	/// <returns>置換後文字列</returns>
	public string ReplaceTag(string targetString, string countyIsoCode = "")
	{
		return CommonPage.ReplaceTag(targetString, countyIsoCode);
	}

	/// <summary>
	/// グローバルタグ名作成
	/// </summary>
	/// <param name="targetString">置換対象文字列リスト</param>
	/// <param name="globalTagPart">グローバル部分タグ</param>
	/// <returns>グローバルタグ名</returns>
	private string CreateGlobalTagName(string targetString, string globalTagPart)
	{
		if (targetString.Contains(string.Format(".{0}.", globalTagPart))) return targetString;

		var lastIndexOfDot = targetString.LastIndexOf('.');
		var targetStringGlobal = targetString.Substring(0, lastIndexOfDot) + "." + globalTagPart
			+ targetString.Substring(lastIndexOfDot);
		return targetStringGlobal;
	}

	/// <summary>
	/// 国が日本かどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>日本か</returns>
	protected static bool IsCountryJp(string countryIsoCode)
	{
		return GlobalAddressUtil.IsCountryJp(countryIsoCode);
	}

	/// <summary>
	/// 郵便番号が必須かどうか
	/// </summary>
	/// <param name="countryIsoCode">国ISOコード</param>
	/// <returns>必須か</returns>
	protected static bool IsAddrZipcodeNecessary(string countryIsoCode)
	{
		return GlobalAddressUtil.IsAddrZipcodeNecessary(countryIsoCode);
	}

	/// <summary>商品IDを含まないバリエーションID</summary>
	protected const string FIELD_PRODUCTVARIATION_V_ID = "v_id";

	/// <summary>RawUrl（IISのバージョンによる機能の違いを吸収）</summary>
	public string RawUrl
	{
		get { return w2.Common.Web.WebUtility.GetRawUrl(Request); }
	}
	/// <summary>ログイン店舗オペレータ</summary>
	protected ShopOperatorModel LoginShopOperator
	{
		get { return (ShopOperatorModel)Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR] ?? new ShopOperatorModel(); }
	}
	/// <summary>ログインオペレータ店舗ID</summary>
	protected string LoginOperatorShopId
	{
		get { return this.LoginShopOperator.ShopId; }
	}
	/// <summary>ログインオペレータ識別ID</summary>
	protected string LoginOperatorDeptId
	{
		get { return this.LoginShopOperator.ShopId; }
	}
	/// <summary>ログインオペレータID</summary>
	protected string LoginOperatorId
	{
		get { return this.LoginShopOperator.OperatorId; }
	}
	/// <summary>ログインオペレータ名</summary>
	protected string LoginOperatorName
	{
		get { return this.LoginShopOperator.Name; }
	}
	/// <summary>アクションステータス</summary>
	protected string ActionStatus
	{
		get { return (string)Request[Constants.REQUEST_KEY_ACTION_STATUS]; }
	}
	/// <summary>ポップアップか</summary>
	protected bool IsPopUp
	{
		get { return (Request[Constants.REQUEST_KEY_WINDOW_KBN] == Constants.KBN_WINDOW_POPUP); }
	}
	/// <summary>クレジットカード入力可能か</summary>
	protected bool CanCreditCardInput
	{
		get { return BasePageHelper.CanUseCreditCardNoForm; }
	}
	/// <summary>クレジットカード仮登録が必要なカード区分か(ZEUS以外) ※仮クレカを利用するかはこの条件に加え自社サイト注文かつ新規クレカか等の条件が必要</summary>
	public bool NeedsRegisterProvisionalCreditCardCardKbnExceptZeus
	{
		get { return OrderCommon.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus; }
	}
	/// <summary>商品税込み表示フラグ</summary>
	public bool ProductIncludedTaxFlg
	{
		get { return Constants.MANAGEMENT_INCLUDED_TAX_FLAG; }
	}
	/// <summary>商品価格区分表示文言</summary>
	public string ProductPriceTextPrefix
	{
		get { return TaxCalculationUtility.GetTaxTypeText(); }
	}

	/// <summary>項目メモ一覧</summary>
	protected FieldMemoSettingModel[] FieldMemoSettingData { get; set; }
	/// <summary>リードタイム設定利用可否</summary>
	public bool UseLeadTime
	{
		get { return GlobalConfigUtil.UseLeadTime(); }
	}
	/// <summary>日本に配送可能か</summary>
	protected bool IsShippingCountryAvailableJp
	{
		get
		{
			return ((Constants.GLOBAL_OPTION_ENABLE == false)
				|| (ShippingCountryUtil.GetShippingCountryAvailableListAndCheck(Constants.COUNTRY_ISO_CODE_JP)));
		}
	}
}
