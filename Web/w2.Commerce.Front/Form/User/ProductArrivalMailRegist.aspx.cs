/*
=========================================================================================================
  Module      : 入荷通知メール登録画面処理(ProductArrivalMailRegist.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.App.Common.Web.WrappedContols;

public partial class Form_User_ProductArrivalMailRegist : ProductPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
    {
		this.ProductId = Request[Constants.REQUEST_KEY_PRODUCT_ID];
		this.VariationId = Request[Constants.REQUEST_KEY_VARIATION_ID];
		this.ArrivalMailKbn = Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN];

		// 登録完了のときは画面を切り替えて、以降の処理は実行しない
		if (Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ACTION_KBN] == "COMPLETE")
		{
			this.WdivInput.Visible = false;
			this.WdivComplete.Visible = true;
			return;
		}

		// Check Arrival Mail Kbn
		this.ArrivalManager.CheckArrivalMailKbn();

		// 商品情報セット
		this.ArrivalManager.SetProductInfo();

		// 入荷通知情報セット
		this.ArrivalManager.SetArrivalInfo();

		if (!IsPostBack)
		{
			this.WcbMailAddr.Visible = this.IsLoggedIn;
		}

		// 加工データをセット
		this.IsPcAddrRegistered = this.ArrivalManager.IsPcAddrRegistered;
		this.IsMbAddrRegistered = this.ArrivalManager.IsMbAddrRegistered;
		this.ProductName = this.ArrivalManager.ProductName;
		this.HasVariation = this.ArrivalManager.HasVariation;
		this.VariationName1 = this.ArrivalManager.VariationName1;
		this.VariationName2 = this.ArrivalManager.VariationName2;
		this.VariationName3 = this.ArrivalManager.VariationName3;

		if (!IsPostBack)
		{
			// 画面を初期化
			this.WdivComplete.Visible = false;
			DataBind();
		}
	}

	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRegister_Click(object sender, System.EventArgs e)
	{
		// 通知登録&メール送信のデータ準備
		this.ErrorMessage = this.ArrivalManager.StanbyMailInfo(
			this.WcbUserPcAddr.Checked,
			this.WcbUserMobileAddr.Checked,
			this.WcbMailAddr.Checked,
			Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_PC,
			StringUtility.ToHankaku(this.WtbMailAddr.Text.Trim()));
		if (string.IsNullOrEmpty(this.ErrorMessage) == false) return;

		// 通知登録＆メール送信
		this.ArrivalManager.SendMailAndRegisterArival();

		//------------------------------------------------------
		// 画面を登録完了に切り替え
		// HTTPS→HTTPに切り替える目的で、Ridirectさせる
		//------------------------------------------------------
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Uri.UriSchemeHttp).Append(Uri.SchemeDelimiter).Append(Constants.SITE_DOMAIN).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_REGIST);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ACTION_KBN).Append("=").Append("COMPLETE");
		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 通知用パラメーターセット
	/// </summary>
	private ProductArrivalMailManager CreateArrivalParameter()
	{
		var arrivalManager = new ProductArrivalMailManager
		{
			IsLoggedIn = this.IsLoggedIn,
			LoginUserId = this.LoginUserId,
			PcAddr = this.PcAddr,
			MbAddr = this.MbAddr,
			ProductId = this.ProductId,
			VariationId = string.IsNullOrEmpty(this.VariationId) ? this.ProductId : this.VariationId,
			ArrivalMailKbn = this.ArrivalMailKbn,
		};
		return arrivalManager;
	}

	/// <summary>商品ID</summary>
	new protected string ProductId { get; private set; }
	/// <summary>バリエーションID</summary>
	new protected string VariationId { get; private set; }
	/// <summary>入荷通知区分</summary>
	protected string ArrivalMailKbn { get; private set; }

	/// <summary>商品名</summary>
	protected string ProductName { get; private set; }
	/// <summary>バリエーション有無フラグ</summary>
	protected new bool HasVariation { get; private set; }
	/// <summary>バリエーション表示名1</summary>
	protected string VariationName1 { get; private set; }
	/// <summary>バリエーション表示名2</summary>
	protected string VariationName2 { get; private set; }
	/// <summary>バリエーション表示名3</summary>
	protected string VariationName3 { get; private set; }

	/// <summary>PCメールアドレス</summary>
	protected string PcAddr { get { return this.LoginUserMail; } }
	/// <summary>MBメールアドレス</summary>
	protected string MbAddr { get { return this.LoginUserMail2; } }
	/// <summary>PCメールアドレスの有無</summary>
	protected bool HasPcAddr { get { return (this.PcAddr != ""); } }
	/// <summary>MBメールアドレスの有無</summary>
	protected bool HasMbAddr { get { return (this.MbAddr != ""); } }

	/// <summary>入荷通知メールの有効期限</summary>
	protected DateTime ExpiredDate { get; private set; }

	/// <summary>PCメールアドレスが入荷通知登録済みかどうか</summary>
	protected bool IsPcAddrRegistered
	{
		get { return (bool)ViewState["IsPcAddrRegistered"]; }
		private set { ViewState["IsPcAddrRegistered"] = value; }
	}
	/// <summary>MBメールアドレスが入荷通知登録済みかどうか</summary>
	protected bool IsMbAddrRegistered
	{
		get { return (bool)ViewState["IsMbAddrRegistered"]; }
		private set { ViewState["IsMbAddrRegistered"] = value; }
	}

	/// <summary> メール通知登録機能用マネージャー </summary>
	private ProductArrivalMailManager ArrivalManager
	{
		get { return m_arrivalManager ?? (m_arrivalManager = CreateArrivalParameter()); }
	}
	private ProductArrivalMailManager m_arrivalManager;

	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage { get; private set; }

	private WrappedHtmlGenericControl WdivInput { get { return GetWrappedControl<WrappedHtmlGenericControl>("divInput"); } }
	private WrappedHtmlGenericControl WdivComplete { get { return GetWrappedControl<WrappedHtmlGenericControl>("divComplete"); } }
	private WrappedCheckBox WcbUserPcAddr { get { return GetWrappedControl<WrappedCheckBox>("cbUserPcAddr"); } }
	private WrappedCheckBox WcbUserMobileAddr { get { return GetWrappedControl<WrappedCheckBox>("cbUserMobileAddr"); } }
	private WrappedCheckBox WcbMailAddr { get { return GetWrappedControl<WrappedCheckBox>("cbMailAddr"); } }
	private WrappedTextBox WtbMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbMailAddr"); } }
}
