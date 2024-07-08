/*
=========================================================================================================
  Module      : 入荷通知メール情報一覧画面処理(UserProductArrivalMailList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.UserProductArrivalMail;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Product;

public partial class Form_User_UserProductArrivalMailList : ProductPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

	#region ラップ済みコントロール宣言
	WrappedRepeater WrUserProductArrivalMailList { get { return GetWrappedControl<WrappedRepeater>("rUserProductArrivalMailList"); } }
	WrappedLinkButton WlbUpdate { get { return GetWrappedControl<WrappedLinkButton>("btnUpdate"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエストよりパラメタ取得
			//------------------------------------------------------
			GetParameters();
			// 遷移元の商品詳細URL
			this.BeforeProductUrl = Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_BEFORE_PRODUCT_URL];

			//------------------------------------------------------
			//  入荷通知メール処理区分
			//------------------------------------------------------
			this.blRequestComp = false;
			switch ((string)Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ACTION_KBN])
			{
				// 入荷通知メール情報登録処理
				case Constants.KBN_REQUEST_USERPRODUCTARRIVALMAIL_REGIST:
					UserProductArrivalMailCommon.RegistUserProductArrivalMail(
						StringUtility.ToEmpty(this.LoginUserId),
						StringUtility.ToEmpty(this.ShopId),
						StringUtility.ToEmpty(this.ProductId),
						StringUtility.ToEmpty(this.VariationId),
						Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_PC,
						(string)Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN],
						"");
					this.blRequestComp = true;
					break;

				// 入荷通知メール情報削除処理
				case Constants.KBN_REQUEST_USERPRODUCTARRIVALMAIL_DELETE:
					int iMailNo = 0;
					if (int.TryParse(Request[Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_MAIL_NO],out iMailNo))
					{
						UserProductArrivalMailCommon.DeleteUserProductArrivalMail(this.LoginUserId, iMailNo);
					}
					break;
			}

			// 入荷通知メール情報一覧取得
			this.BindUserProductArrivalMailList();
		}
	}

	/// <summary>
	/// 入荷通知メール情報一覧のバイン
	/// </summary>
	private void BindUserProductArrivalMailList()
	{
		// 入荷通知メール情報一覧
		DataView userProductArrivalMails = UserProductArrivalMailCommon.GetUserProdcutArrivalMailList(this.LoginUserId, this.PageNumber, Constants.CONST_DISP_CONTENTS_DEFAULT_LIST);
		if (userProductArrivalMails.Count != 0)
		{
			int totalCount = (int)userProductArrivalMails[0]["row_count"];

			// 0件でなければ、ページャーを設定
			string strNextUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_LIST;
			this.PagerHtml = WebPager.CreateDefaultListPager(totalCount, this.PageNumber, strNextUrl);
		}
		else
		{
			// 画面制御
			this.WrUserProductArrivalMailList.Visible = false;
			this.WlbUpdate.Visible = false;

			// エラーメッセージ設定
			this.ErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USERPRODUCTARRIVALMAIL_NO_ITEM);
		}

		// 翻訳情報設定
		var products = userProductArrivalMails.Cast<DataRowView>().Select(
			drv => new ProductModel
			{
				ProductId = (string)drv[Constants.FIELD_PRODUCT_PRODUCT_ID]
			}).ToArray();
		userProductArrivalMails = (DataView)NameTranslationCommon.Translate(userProductArrivalMails, products);

		// データバインド
		this.WrUserProductArrivalMailList.DataSource = userProductArrivalMails;
		this.DataBind();
	}

	/// <summary>
	/// ユーザのメール配信フラグチェック
	/// </summary>
	protected bool CheckUserMailFlg()
	{
		return UserProductArrivalMailCommon.CheckUserMailFlg(this.LoginUserId);
	}

	/// <summary>
	/// 商品詳細画面遷移(バリエーションあり)URL作成
	/// </summary>
	/// <param name="objUserProductArrivalMail">入荷通知メール情報</param>
	/// <returns>入荷通知メール情報削除URL</returns>
	protected new string CreateProductDetailVariationUrl(object objUserProductArrivalMail)
	{
		return CreateProductDetailUrl(
			objUserProductArrivalMail,
			(string)GetKeyValue(objUserProductArrivalMail, Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID));
	}

	#region レコナイズ削除予定記述
	/// <summary>
	/// レコナイズ用入荷通知メール登録済み商品ID取得
	/// </summary>
	/// <returns>入荷通知メールを登録した商品の商品ID</returns>
	/// <remarks>
	/// ・商品IDの頭に"P"をつける
	/// ・カンマ区切りで並べる
	/// ・最大10件
	/// </remarks>
	protected string GetArrivalMailRegistedProductsForReconize()
	{
		//return RecommendReconize.GetArrivalMailRegistedProductsForReconize(this.LoginUserId);
		//削除予定処理（マイナーバージョンアップでエラー防止のためメソッドは残す）
		return "";
	}
	#endregion

	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		this.ErrorMessage = null;
		var sbErrorMessages = new StringBuilder();
		foreach (RepeaterItem ri in WrUserProductArrivalMailList.Items)
		{
			// ラップ済みコントロール
			var wtbDateExpiredYearYear = GetWrappedControl<WrappedTextBox>(ri, "tbDateExpiredYearYear");
			var wlProductname = GetWrappedControl<WrappedLiteral>(ri, "lProductname");
			var itemErrorMessage = string.Empty;

			var errorMessageExpiredYear = Validator.Validate(
				"ProductArrivalMail",
				new Hashtable
				{
					{ "date_expired_year_year", CreateDateTimeString(ri) }
				});

			// 入荷メール通知期限年号チェック
			if (string.IsNullOrEmpty(errorMessageExpiredYear))
			{
				var input = new Hashtable
				{
					{ "expired_year", StringUtility.ToHankaku(wtbDateExpiredYearYear.Text.Trim()) }
				};
				var errorMessagesYear = Validator.Validate("ArrivalMailList", input);
				if (string.IsNullOrEmpty(errorMessagesYear) == false)
				{
					itemErrorMessage = string.Format("{0}{1}", wlProductname.Text, errorMessagesYear);
				}
			}
			else
			{
				itemErrorMessage = string.Format(
					"{0}{1}",
					wlProductname.Text.Trim(),
					errorMessageExpiredYear);
			}

			if (string.IsNullOrEmpty(itemErrorMessage) == false)
			{
				sbErrorMessages.Append(itemErrorMessage).Append("<br />");
			}
		}
		if (string.IsNullOrEmpty(sbErrorMessages.ToString()) == false)
		{
			this.ErrorMessage = sbErrorMessages.ToString();
			return;
		}

		//------------------------------------------------------
		// 入荷メール配信期限更新処理
		//------------------------------------------------------
		foreach (RepeaterItem ri in rUserProductArrivalMailList.Items)
		{
			var expiredDateTime =
				((DateTime.Parse(CreateDateTimeString(ri)).Year == DateTime.MaxValue.Year)
					&& (DateTime.Parse(CreateDateTimeString(ri)).Month == DateTime.MaxValue.Month))
					? DateTime.MaxValue.ToString()
					: DateTime.Parse(CreateDateTimeString(ri)).AddMonths(1).AddSeconds(-1).ToString();
			UserProductArrivalMailCommon.UpdateUserProductArrivalMail(
			this.LoginUserId,
			int.Parse(((HiddenField)ri.FindControl("hfArrivalMailNo")).Value),
			expiredDateTime);
		}

		// 入荷通知メール情報一覧取得
		this.BindUserProductArrivalMailList();
	}

	/// <summary>
	/// 入荷通知メール配信期限文字列作成
	/// </summary>
	/// <param name="ri">リピータアイテム</param>
	/// <returns>入荷通知メール配信期限文字列</returns>
	private string CreateDateTimeString(RepeaterItem ri)
	{
		return ((TextBox)ri.FindControl("tbDateExpiredYearYear")).Text + "/" + ((TextBox)ri.FindControl("tbDateExpiredYearMonth")).Text + "/01";
	}

	/// <summary>元の商品詳細URL</summary>
	protected string BeforeProductUrl
	{
		get { return (string)ViewState["BeforeProductUrl"]; }
		private set { ViewState["BeforeProductUrl"] = value; }
	}
	/// <summary>ページャーHTML</summary>
	protected string PagerHtml 
	{
		get { return (string)ViewState["PagerHtml"]; }
		private set { ViewState["PagerHtml"] = value; }
	}
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage 
	{
		get { return (string)ViewState["ErrorMessage"]; }
		private set { ViewState["ErrorMessage"] = value; }
	}
	/// <summary>登録処理完了</summary>
	protected bool blRequestComp
	{
		get { return (bool)ViewState["RequestComp"]; }
		private set { ViewState["RequestComp"] = value; }
	}
}

