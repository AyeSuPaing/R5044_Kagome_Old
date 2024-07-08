/*
=========================================================================================================
  Module      : 問合せ入力画面処理(InquiryInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Product;
using w2.Domain.User;

public partial class Form_Inquiry_InquiryInput : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	# region ラップ済コントロール宣言
	protected WrappedDropDownList WddlInquiryTitle { get { return GetWrappedControl<WrappedDropDownList>("ddlInquiryTitle"); } }
	protected WrappedDropDownList WddlProductVariation { get { return GetWrappedControl<WrappedDropDownList>("ddlProductVariation"); } }
	protected WrappedHiddenField WhfProductTitlePrefix { get { return GetWrappedControl<WrappedHiddenField>("hfProductTitlePrefix", ""); } }
	protected WrappedTextBox WtbInquiryText { get { return GetWrappedControl<WrappedTextBox>("tbInquiryText"); } }
	protected WrappedTextBox WtbUserName1 { get { return GetWrappedControl<WrappedTextBox>("tbUserName1"); } }
	protected WrappedTextBox WtbUserName2 { get { return GetWrappedControl<WrappedTextBox>("tbUserName2"); } }
	protected WrappedTextBox WtbUserNameKana1 { get { return GetWrappedControl<WrappedTextBox>("tbUserNameKana1"); } }
	protected WrappedTextBox WtbUserNameKana2 { get { return GetWrappedControl<WrappedTextBox>("tbUserNameKana2"); } }
	protected WrappedTextBox WtbUserMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbUserMailAddr"); } }
	protected WrappedTextBox WtbUserMailAddrConf { get { return GetWrappedControl<WrappedTextBox>("tbUserMailAddrConf"); } }
	protected WrappedTextBox WtbUserTel1 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel1"); } }
	protected WrappedTextBox WtbUserTel2 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2"); } }
	protected WrappedTextBox WtbUserTel3 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel3"); } }
	protected WrappedHtmlGenericControl WspBack { get { return GetWrappedControl<WrappedHtmlGenericControl>("spBack"); } }
	protected WrappedTextBox WtbTel1 { get { return GetWrappedControl<WrappedTextBox>("tbTel1"); } }
	# endregion

	private const string VARIATION_NAME = "@@ variation_name @@";
	private const string VARIATION_ID = "@@ variation_id @@";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// HTTPS通信チェック（HTTPの場合、HTTPSで再読込）
		//------------------------------------------------------
		CheckHttps(this.SecurePageProtocolAndHost + Request.Url.PathAndQuery);

		this.ProductMaster = (this.IsProductInquiry) ? ProductCommon.GetProductInfo(this.ShopId, this.ProductId, this.MemberRankId, this.UserFixedPurchaseMemberFlg) : null;

		// 翻訳情報設定
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			SetProductTranslationData();
		}

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 特定商品に関する問合わせを初期化
			//------------------------------------------------------
			InitializeProductInquiry();

			//------------------------------------------------------
			// 確認ページからの遷移で入力情報の復元
			//------------------------------------------------------
			if (this.SessionParamTargetPage == Constants.PAGE_FRONT_INQUIRY_INPUT)
			{
				InformationRestoration();

				// 確認ページからの遷移後にリセットすると入力情報で補完されてしまうため、ここでセッションを初期化
				this.SessionParamTargetPage = null;
			}
			//------------------------------------------------------
			// ログインしていればユーザ情報取得して初期化
			//------------------------------------------------------
			else if (this.IsLoggedIn)
			{
				SetInitializeUserInformation();
			}
		}
	}

	/// <summary>
	/// 特定商品に関する問合わせを初期化
	/// </summary>
	private void InitializeProductInquiry()
	{
		//------------------------------------------------------
		// 商品情報あり
		//------------------------------------------------------
		if ((this.ProductMaster != null) && (this.ProductMaster.Count > 0))
		{
			// 問合せ件名の作成
			string inquiryTitle = GetInquiryTitle();
			// バリエーションのDDL作成(複数件ある場合のみ)
			CreateVariationDropDownList(this.ProductMaster);
			// 表示切替
			this.ProductInquiry = this.WhfProductTitlePrefix.Value + inquiryTitle;
			this.ProductPageURL = ProductCommon.CreateProductDetailUrl(this.ProductMaster[0], "", "", "", "", ""); // 商品ページへのURL
			this.WspBack.Visible = true;
			this.WddlInquiryTitle.Visible = false; // ドロップダウン非表示
			this.WddlProductVariation.Visible = ProductCommon.HasVariation(this.ProductMaster[0]);
		}
		//------------------------------------------------------
		// 商品情報なし
		//------------------------------------------------------
		else
		{
			this.ProductInquiry = "";
			this.ProductPageURL = "";
			this.WspBack.Visible = false;
			this.WddlInquiryTitle.Visible = true;
			this.WddlProductVariation.Visible = false;
		}
	}

	/// <summary>
	/// 問合せ件名を取得
	/// </summary>
	/// <returns></returns>
	private string GetInquiryTitle()
	{
		string inquiryTitle = CreateInquiryTitleTemplate(this.ProductMaster[0]);
		return CreateInquiryTitle(this.ProductMaster, inquiryTitle);
	}

	/// <summary>
	/// 問合せ件名の作成のテンプレートを取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns></returns>
	private string CreateInquiryTitleTemplate(DataRowView product)
	{
		StringBuilder inquiryTitle = new StringBuilder();
		inquiryTitle.Append(product[Constants.FIELD_PRODUCT_NAME]).Append(VARIATION_NAME);
		inquiryTitle.Append(" [").Append(product[Constants.FIELD_PRODUCT_PRODUCT_ID]).Append(VARIATION_ID).Append("]");

		return inquiryTitle.ToString();
	}

	/// <summary>
	/// 問合せ件名の作成
	/// </summary>
	/// <param name="dataViewProduct">商品情報</param>
	/// <param name="inquiryTitle">問合せ件名(置換前)</param>
	/// <returns>問合せ件名(置換後)</returns>
	private string CreateInquiryTitle(DataView dataViewProduct, string inquiryTitle)
	{
		// バリエーション管理してない
		if (ProductCommon.HasVariation(dataViewProduct[0]) == false)
		{
			inquiryTitle = inquiryTitle.Replace(VARIATION_NAME, "").Replace(VARIATION_ID, "");
		}
		// バリエーション管理してる
		else
		{
			if (IsPostBack)
			{
				foreach (DataRowView product in dataViewProduct)
				{
					if ((string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] == this.WddlProductVariation.SelectedValue)
					{
						inquiryTitle = inquiryTitle.Replace(VARIATION_NAME, ProductCommon.CreateVariationName(product))
							.Replace(VARIATION_ID, ((string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]).Substring(((string)product[Constants.FIELD_PRODUCT_PRODUCT_ID]).Length));
						break;
					}
				}
			}
			inquiryTitle = inquiryTitle.Replace(VARIATION_NAME, "").Replace(VARIATION_ID, "");
		}
		return inquiryTitle;
	}

	/// <summary>
	/// バリエーションのDDL作成(複数件ある場合のみ)
	/// </summary>
	/// <param name="dataViewProduct">商品情報</param>
	private void CreateVariationDropDownList(DataView dataViewProduct)
	{
		if ((this.WddlProductVariation.InnerControl != null)
			&& ProductCommon.HasVariation(dataViewProduct[0]))
		{
			this.WddlProductVariation.Items.Add(
				new ListItem(ReplaceTag("@@DispText.variation_name_list.unselected@@"), ""));
			foreach (DataRowView product in dataViewProduct)
			{
				ListItem listItem = new ListItem(
					 ProductCommon.CreateVariationName(product)
					+ " [" + ((string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]).Replace((string)product[Constants.FIELD_PRODUCT_PRODUCT_ID], "") + "]",
					(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);

				this.WddlProductVariation.Items.Add(listItem);
			}
			this.WddlProductVariation.SelectItemByValue(this.VariationId);
		}
	}

	/// <summary>
	/// 確認画面から遷移して来た際の情報の復元
	/// </summary>
	private void InformationRestoration()
	{
		Hashtable htInput = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		// ドロップダウンの中に存在する問い合わせタイトルの場合
		if (this.WddlInquiryTitle.Items.Contains(new ListItem((string)htInput[Constants.INQUIRY_KEY_INQUIRY_TITLE])))
		{
			this.WddlInquiryTitle.SelectedValue = (string)htInput[Constants.INQUIRY_KEY_INQUIRY_TITLE];
			this.WddlInquiryTitle.Visible = true;
			this.ProductInquiry = "";
			this.ProductPageURL = "";
			this.WspBack.Visible = false;
			this.WddlProductVariation.Visible = false;
		}

		this.WtbInquiryText.Text = (string)htInput[Constants.INQUIRY_KEY_INQUIRY_TEXT];
		this.WtbUserName1.Text = (string)htInput[Constants.FIELD_USER_NAME1];
		this.WtbUserName2.Text = (string)htInput[Constants.FIELD_USER_NAME2];
		this.WtbUserNameKana1.Text = (string)htInput[Constants.FIELD_USER_NAME_KANA1];
		this.WtbUserNameKana2.Text = (string)htInput[Constants.FIELD_USER_NAME_KANA2];
		this.WtbUserMailAddr.Text = (string)htInput[Constants.FIELD_USER_MAIL_ADDR];
		this.WtbUserMailAddrConf.Text = (string)htInput[Constants.FIELD_USER_MAIL_ADDR + Constants.FIELD_COMMON_CONF];

		// Set value for telephone
		SetTelTextbox(
			this.WtbTel1,
			this.WtbUserTel1,
			this.WtbUserTel2,
			this.WtbUserTel3,
			StringUtility.ToEmpty(htInput[Constants.FIELD_USER_TEL1]));
	}

	/// <summary>
	/// ユーザ情報取得して初期化
	/// </summary>
	private void SetInitializeUserInformation()
	{
		var user = new UserService().Get(this.LoginUserId);
		if (user != null)
		{
			this.WtbUserName1.Text = user.Name1;
			this.WtbUserName2.Text = user.Name2;
			this.WtbUserNameKana1.Text = user.NameKana1;
			this.WtbUserNameKana2.Text = user.NameKana2;
			this.WtbUserMailAddr.Text = user.MailAddr;
			this.WtbUserMailAddrConf.Text = user.MailAddr;

			// Set value for telephone
			SetTelTextbox(
				this.WtbTel1,
				this.WtbUserTel1,
				this.WtbUserTel2,
				this.WtbUserTel3,
				user.Tel1);
		}
	}

	/// <summary>
	/// 確認するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbConfirm_Click(object sender, EventArgs e)
	{
		// 「国」がなくて、グローバル対応しない場合は日本住所とする
		// ※グローバル対応の場合は入力する値のままで、全角に変換しない
		var isJpAddress = (Constants.GLOBAL_OPTION_ENABLE == false);

		//------------------------------------------------------
		// 問合せ情報取得
		//------------------------------------------------------
		Hashtable htInput = new Hashtable();
		if (StringUtility.ToEmpty(this.ProductInquiry) != "")
		{
			// 問合せ件名の作成
			string inquiryTitle = ((this.ProductMaster != null) && (this.ProductMaster.Count > 0)) ? GetInquiryTitle() : "";
			htInput.Add(Constants.INQUIRY_KEY_INQUIRY_TITLE, this.WhfProductTitlePrefix.Value + inquiryTitle);
		}
		else
		{
			htInput.Add(Constants.INQUIRY_KEY_INQUIRY_TITLE, this.WddlInquiryTitle.SelectedValue);
		}
		htInput.Add(Constants.INQUIRY_KEY_INQUIRY_TEXT, this.WtbInquiryText.Text);
		htInput.Add(Constants.INQUIRY_KEY_PRODUCT_URL, this.ProductPageURL);
		htInput.Add(Constants.FIELD_USER_NAME1, DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserName1.Text, isJpAddress));
		htInput.Add(Constants.FIELD_USER_NAME2, DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserName2.Text, isJpAddress));

		if (Constants.GLOBAL_OPTION_ENABLE == false)
		{
			htInput.Add(Constants.FIELD_USER_NAME_KANA1, StringUtility.ToZenkaku(this.WtbUserNameKana1.Text));
			htInput.Add(Constants.FIELD_USER_NAME_KANA2, StringUtility.ToZenkaku(this.WtbUserNameKana2.Text));
		}
		htInput.Add(Constants.FIELD_USER_MAIL_ADDR, StringUtility.ToHankaku(this.WtbUserMailAddr.Text));
		htInput.Add(Constants.FIELD_USER_MAIL_ADDR + Constants.FIELD_COMMON_CONF, StringUtility.ToHankaku(this.WtbUserMailAddrConf.Text));

		// Set value for telephone
		var inputTel = (this.WtbUserTel1.HasInnerControl)
			? StringUtility.ToHankaku(this.WtbUserTel1.Text.Trim())
			: StringUtility.ToHankaku(this.WtbTel1.Text.Trim());
		if (this.WtbUserTel1.HasInnerControl)
		{
			inputTel = UserService.CreatePhoneNo(
				inputTel,
				StringUtility.ToHankaku(this.WtbUserTel2.Text.Trim()),
				StringUtility.ToHankaku(this.WtbUserTel3.Text.Trim()));
		}
		var userTel = new Tel(inputTel);
		htInput.Add(Constants.FIELD_USER_TEL1_1, userTel.Tel1);
		htInput.Add(Constants.FIELD_USER_TEL1_2, userTel.Tel2);
		htInput.Add(Constants.FIELD_USER_TEL1_3, userTel.Tel3);
		if (this.WtbUserTel1.HasInnerControl == false)
		{
			htInput[Constants.FIELD_USER_TEL1] = (string.IsNullOrEmpty(userTel.TelNo) == false)
				? userTel.TelNo
				: inputTel;
		}

		htInput.Add(Constants.FIELD_PRODUCT_SHOP_ID, StringUtility.ToEmpty(this.ShopId));
		htInput.Add(Constants.FIELD_PRODUCT_PRODUCT_ID, StringUtility.ToEmpty(this.ProductId));
		htInput.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, this.WddlProductVariation.SelectedValue);
		//------------------------------------------------------
		// エラーチェック＆カスタムバリデータへセット
		//------------------------------------------------------
		Dictionary<string, string> dicErrorMessages = Validator.ValidateAndGetErrorContainer("Inquiry", htInput);
		if (dicErrorMessages.Count != 0)
		{
			// カスタムバリデータ取得
			List<CustomValidator> lCustomValidators = new List<CustomValidator>();
			CreateCustomValidators(this, lCustomValidators);

			// エラーをカスタムバリデータへ
			SetControlViewsForError("Inquiry", dicErrorMessages, lCustomValidators);

			return;
		}

		// Telを分割して入力した場合、連結後の値を入力チェックしないため、後から詰める
		if (this.WtbUserTel1.HasInnerControl)
		{
			htInput[Constants.FIELD_USER_TEL1] = inputTel;
		}

		// グローバルOPがONの場合は「かな」の入力チェックをしないため、後から詰める
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			htInput.Add(Constants.FIELD_USER_NAME_KANA1, StringUtility.ToZenkaku(this.WtbUserNameKana1.Text));
			htInput.Add(Constants.FIELD_USER_NAME_KANA2, StringUtility.ToZenkaku(this.WtbUserNameKana2.Text));
		}

		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		// パラメタセット
		Session[Constants.SESSION_KEY_PARAM] = htInput;

		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_INQUIRY_CONFIRM;

		// 画面遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_INQUIRY_CONFIRM);
	}

	/// <summary>
	/// 商品翻訳情報設定
	/// </summary>
	private void SetProductTranslationData()
	{
		// 通常の問い合わせページにアクセスした場合は何もしない
		if (this.ProductMaster == null) return;

		var products = this.ProductMaster.Cast<DataRowView>().Select(
			drv => new ProductModel
			{
				ProductId = (string)drv[Constants.FIELD_PRODUCT_PRODUCT_ID]
			}).ToArray();

		var translationSettings = NameTranslationCommon.GetProductAndVariationTranslationSettings(
			products,
			RegionManager.GetInstance().Region.LanguageCode,
			RegionManager.GetInstance().Region.LanguageLocaleId);
		this.ProductMaster = NameTranslationCommon.SetProductAndVariationTranslationDataToDataView(this.ProductMaster, translationSettings);
	}

	#region プロパティ
	/// <summary>特定商品に対する問い合わせ</summary>
	protected string ProductInquiry
	{
		get { return (string)ViewState["ProductInquiry"]; }
		private set { ViewState["ProductInquiry"] = value; }
	}
	/// <summary>商品ページのURL</summary>
	protected string ProductPageURL
	{
		get { return (string)ViewState["ProductPageURL"]; }
		private set { ViewState["ProductPageURL"] = value; }
	}
	/// <summary>店舗ID</summary>
	protected string ShopId { get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHOP_ID]); } }
	/// <summary>商品ID</summary>
	protected string ProductId { get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ID]); } }
	/// <summary>商品バリエーションID</summary>
	protected string VariationId { get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_VARIATION_ID]); } }
	/// <summary>商品情報</summary>
	protected DataView ProductMaster { get; private set; }
	/// <summary>特定商品に対する問い合わせか</summary>
	protected bool IsProductInquiry
	{
		get { return (this.ShopId != "") && (this.ProductId != ""); }
	}
	/// <summary>リセット時のパラメータ</summary>
	protected string QueryParams
	{
		get
		{
			StringBuilder queryParams = new StringBuilder();
			if ((this.ShopId != "") && (this.ProductId != ""))
			{
				queryParams.Append("?").Append(Constants.REQUEST_KEY_SHOP_ID).Append("=").Append(this.ShopId);
				queryParams.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(this.ProductId);
				queryParams.Append("&").Append(Constants.REQUEST_KEY_VARIATION_ID).Append("=").Append(this.VariationId);
			}
			return queryParams.ToString();
		}
	}
	#endregion
}
