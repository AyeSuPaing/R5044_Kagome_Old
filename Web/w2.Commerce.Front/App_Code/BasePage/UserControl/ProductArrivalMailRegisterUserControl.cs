/*
=========================================================================================================
  Module      : 入荷通知メール登録フォーム処理(BodyProductArrivalMailRegisterUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Web.WrappedContols;
using w2.App.Common.UserProductArrivalMail;

/// <summary>
/// 入荷通知メール登録フォームユーザーコントロール
/// </summary>
public partial class ProductArrivalMailRegisterUserControl : BaseUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.IsRegisterSucceeded = false;
		this.IsAlreadyVisible = this.Visible;
	}

	/// <summary>
	/// ページレンダー
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_PreRender(object sender, EventArgs e)
	{
		// バリエーションがある場合、バリエーションIDが渡されていない要素は無視する。
		if (string.IsNullOrEmpty(this.VariationId) && this.HasVariation && string.IsNullOrEmpty(this.VariationId)) return;

		// Check Arrival Mail Kbn
		this.ArrivalManager.CheckArrivalMailKbn();

		// 商品情報セット
		this.ArrivalManager.SetProductInfo();

		// 入荷通知情報セット
		this.ArrivalManager.SetArrivalInfo();

		// 加工データをセット
		this.IsPcAddrRegistered = this.ArrivalManager.IsPcAddrRegistered;
		this.IsMbAddrRegistered = this.ArrivalManager.IsMbAddrRegistered;
		this.IsOtherAddrRegistered = this.ArrivalManager.IsOtherAddrRegistered;

		// チェックボックスのEnable制御
		this.WcbUserPcAddr.Enabled = (this.IsPcAddrRegistered == false);
		this.WcbUserMobileAddr.Enabled = (this.IsMbAddrRegistered == false);

		// 加工データをセット
		this.ProductName = this.ArrivalManager.ProductName;
		this.HasVariation = this.ArrivalManager.HasVariation;
		this.VariationName1 = this.ArrivalManager.VariationName1;
		this.VariationName2 = this.ArrivalManager.VariationName2;

		if (this.Visible)
		{
			// 「その他」のチェックボックスを可視化
			this.WcbMailAddr.Visible = this.IsLoggedIn;
		}
	}

	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRegister_Click(object sender, System.EventArgs e)
	{
		// チェックボックスのEnable制御
		this.WcbUserPcAddr.Enabled = (this.IsPcAddrRegistered == false);
		this.WcbUserMobileAddr.Enabled = (this.IsMbAddrRegistered == false);

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

		//結果データのセット
		this.IsRegisterSucceeded = true;
		this.HasPcMailKbnResult = this.ArrivalManager.HasPcMailKbnResult;
		this.HasMbMailKbnResult = this.ArrivalManager.HasMbMailKbnResult;
		this.HasOtherMailKbnResult = this.ArrivalManager.HasOtherMailKbnResult;
	}

	/// <summary>登録したものにPCメール区分は含まれているか</summary>
	public bool HasPcMailKbnResult
	{
		get { return (bool)(ViewState["HasPcMailKbnResult"] ?? false); }
		set { ViewState["HasPcMailKbnResult"] = value; }
	}
	/// <summary>登録したものにMBメール区分は含まれているか</summary>
	public bool HasMbMailKbnResult
	{
		get { return (bool)(ViewState["HasMbMailKbnResult"] ?? false); }
		set { ViewState["HasMbMailKbnResult"] = value; }
	}
	/// <summary>登録したものにその他メール区分は含まれているか</summary>
	public bool HasOtherMailKbnResult
	{
		get { return (bool)(ViewState["HasOtherMailKbnResult"] ?? false); }
		set { ViewState["HasOtherMailKbnResult"] = value; }
	}

	/// <summary>
	/// 通知用パラメーターセット
	/// </summary>
	/// <returns>通知メール機能用マネージャー</returns>
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
			IsPcAddrRegistered = this.IsPcAddrRegistered,
			IsMbAddrRegistered = this.IsMbAddrRegistered,
			IsOtherAddrRegistered = this.IsOtherAddrRegistered
		};
		return arrivalManager;
	}

	/// <summary>商品名</summary>
	protected string ProductName { get; set; }
	/// <summary>バリエーションを持っているか</summary>
	public bool HasVariation
	{
		get { return (bool)(ViewState["HasVariation"] ?? true); }
		set { ViewState["HasVariation"] = value; }
	}
	/// <summary>バリエーション表示名1</summary>
	protected string VariationName1 { get; set; }
	/// <summary>バリエーション表示名2</summary>
	protected string VariationName2 { get; set; }

	/// <summary>PCメールアドレス</summary>
	protected string PcAddr { get { return this.LoginUserMail; } }
	/// <summary>MBメールアドレス</summary>
	protected string MbAddr { get { return this.LoginUserMail2; } }
	/// <summary>PCメールアドレスの有無</summary>
	protected bool HasPcAddr { get { return (this.PcAddr != ""); } }
	/// <summary>MBメールアドレスの有無</summary>
	protected bool HasMbAddr { get { return (this.MbAddr != ""); } }

	/// <summary>入荷通知メールの有効期限</summary>
	protected DateTime ExpiredDate
	{
		get { return UserProductArrivalMailCommon.GetExpiredDate(this.ArrivalMailKbn); }
	}

	/// <summary>PCメールアドレスが入荷通知登録済みかどうか</summary>
	protected bool IsPcAddrRegistered {
		get { return (bool)(ViewState["IsPcAddrRegistered"] ?? false); }
		set { ViewState["IsPcAddrRegistered"] = value; }
	}
	/// <summary>MBメールアドレスが入荷通知登録済みかどうか</summary>
	protected bool IsMbAddrRegistered {
		get { return (bool)(ViewState["IsMbAddrRegistered"] ?? false); }
		set { ViewState["IsMbAddrRegistered"] = value; }
	}
	/// <summary>その他メールアドレスが入荷通知登録済みかどうか</summary>
	protected bool IsOtherAddrRegistered
	{
		get { return (bool)(ViewState["IsOtherAddrRegistered"] ?? false); }
		set { ViewState["IsOtherAddrRegistered"] = value; }
	}
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage { get; private set; }

	/// <summary> 登録が完了したかどうか </summary>
	protected bool IsRegisterSucceeded
	{
		get { return (bool)ViewState["IsRegisterSucceeded"]; }
		set { ViewState["IsRegisterSucceeded"] = value; }
	}

	/// <summary>商品ID（外部から設定可能）</summary>
	public string ProductId
	{
		get { return (string)ViewState["ProductId"]; }
		set { ViewState["ProductId"] = value; }
	}
	/// <summary>バリエーションID（外部から設定可能）</summary>
	public string VariationId
	{
		get { return (string)ViewState["VariationId"]; }
		set { ViewState["VariationId"] = value; }
	}

	/// <summary>入荷通知区分（外部から設定可能）</summary>
	public string ArrivalMailKbn
	{
		get { return (string)ViewState["ArrivalMailKbn"]; }
		set { ViewState["ArrivalMailKbn"] = value; }
	}

	/// <summary> メール通知登録機能用マネージャー </summary>
	private ProductArrivalMailManager ArrivalManager
	{
		get { return m_arrivalManager ?? (m_arrivalManager = CreateArrivalParameter()); }
	}
	private ProductArrivalMailManager m_arrivalManager;

	/// <summary> 既に表示されていたか T/F </summary>
	private bool IsAlreadyVisible
	{
		get { return (bool)(ViewState["IsAlreadyVisible"] ?? false); }
		set { ViewState["IsAlreadyVisible"] = value; }
	}

	private WrappedCheckBox WcbUserPcAddr { get { return GetWrappedControl<WrappedCheckBox>("cbUserPcAddr"); } }
	private WrappedCheckBox WcbUserMobileAddr { get { return GetWrappedControl<WrappedCheckBox>("cbUserMobileAddr"); } }
	private WrappedCheckBox WcbMailAddr { get { return GetWrappedControl<WrappedCheckBox>("cbMailAddr"); } }
	private WrappedTextBox WtbMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbMailAddr"); } }
}