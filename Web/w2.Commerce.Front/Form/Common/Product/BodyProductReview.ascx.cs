/*
=========================================================================================================
  Module      : 商品レビュー出力コントローラ処理(BodyProductReview.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using AmazonPay.StandardPaymentRequests;
using w2.App.Common.Option;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Logger;
using w2.Domain.ProductReview;

public partial class Form_Common_Product_BodyProductReview : ProductUserControl
{
	#region ラップ済みコントロール宣言
	WrappedHtmlGenericControl WdvProductReviewComments { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvProductReviewComments"); } }
	WrappedHtmlGenericControl WdvProductReviewInput { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvProductReviewInput"); } }
	WrappedHtmlGenericControl WdvProductReviewConfirm { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvProductReviewConfirm"); } }
	WrappedHtmlGenericControl WdvProductReviewComplete { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvProductReviewComplete"); } }
	WrappedHtmlGenericControl WsIconImageRating1 { get { return GetWrappedControl<WrappedHtmlGenericControl>("sIconImageRating1"); } }
	WrappedHtmlGenericControl WsIconImageRating2 { get { return GetWrappedControl<WrappedHtmlGenericControl>("sIconImageRating2"); } }
	WrappedHtmlGenericControl WsIconImageRating3 { get { return GetWrappedControl<WrappedHtmlGenericControl>("sIconImageRating3"); } }
	WrappedHtmlGenericControl WsIconImageRating4 { get { return GetWrappedControl<WrappedHtmlGenericControl>("sIconImageRating4"); } }
	WrappedHtmlGenericControl WsIconImageRating5 { get { return GetWrappedControl<WrappedHtmlGenericControl>("sIconImageRating5"); } }
	WrappedRepeater WrProductReview { get { return GetWrappedControl<WrappedRepeater>("rProductReview"); } }
	WrappedTextBox WtbNickName { get { return GetWrappedControl<WrappedTextBox>("tbNickName"); } }
	WrappedDropDownList WddlReviewRating { get { return GetWrappedControl<WrappedDropDownList>("ddlReviewRating"); } }
	WrappedTextBox WtbReviewTitle { get { return GetWrappedControl<WrappedTextBox>("tbReviewTitle"); } }
	WrappedTextBox WtbReviewComment { get { return GetWrappedControl<WrappedTextBox>("tbReviewComment"); } }
	#endregion

	/// <summary>ロック専用オブジェクト</summary>
	private static readonly object m_lockObject = new object();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		SetProductReviewDataForDisplay();
	}

	/// <summary>
	/// 商品レビューデータ設定
	/// </summary>
	private void SetProductReviewDataForDisplay()
	{
		// 商品レビューページ番号（ページャ動作時のみもちまわる）
		int iCurrentReviewPageNumber;
		bool blDispCommontsWithPager;
		if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out iCurrentReviewPageNumber) == false)
		{
			iCurrentReviewPageNumber = 1;
			blDispCommontsWithPager = false;
		}
		else
		{
			blDispCommontsWithPager = true;
		}

		//------------------------------------------------------
		// コンポーネント初期化
		//------------------------------------------------------
		// コメント一覧・コメント入力フォーム表示ボタンのみ表示
		lbDispCommentForm.Visible = true;
		this.WdvProductReviewComments.Visible = true;
		this.WdvProductReviewInput.Visible = false;
		this.WdvProductReviewConfirm.Visible = false;
		this.WdvProductReviewComplete.Visible = false;

		// 評価の値を設定
		if (this.WddlReviewRating.Items.Count == 0)
		{
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PRODUCTREVIEW, Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING))
			{
				this.WddlReviewRating.AddItem(li);
			}
		}

		//------------------------------------------------------
		// 商品レビューコメント一覧取得
		//------------------------------------------------------
		DataView dvProductReview = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductReview", "GetProductReview"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCTREVIEW_SHOP_ID, this.ShopId);
			htInput.Add(Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID, this.ProductId);
			htInput.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_PRODUCTREVIEW_LIST * (iCurrentReviewPageNumber - 1) + 1);
			htInput.Add("end_row_num", (blDispCommontsWithPager) ? (Constants.CONST_DISP_CONTENTS_PRODUCTREVIEW_LIST * iCurrentReviewPageNumber) : this.ProductReviewCount);

			dvProductReview = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		// 商品レビューの総計取得
		int iTotalProductCounts = (dvProductReview.Count != 0) ? (int)dvProductReview[0].Row["row_count"] : 0;

		//------------------------------------------------------
		// コメントあり？
		//------------------------------------------------------
		if (dvProductReview.Count != 0)
		{
			// コメント一覧表示
			this.WdvProductReviewComments.Visible = true;

			//------------------------------------------------------
			// コメント＆ページャ全件表示？
			//------------------------------------------------------
			if (blDispCommontsWithPager)
			{
				lbReviewDispPager.Visible = false;

				// ページャ表示
				string strNextUrl = CreateProductDetailUrl(this.ShopId, this.CategoryId, this.BrandId, this.SearchWord, this.ProductId, this.VariationId, this.ProductName, this.BrandName);
				this.PagerHtml = WebPager.CreateProductReviewPager(iTotalProductCounts, iCurrentReviewPageNumber, strNextUrl);
			}
			//------------------------------------------------------
			// 先頭コメントのみ表示？
			//------------------------------------------------------
			else
			{
				// コメント総合計が初期表示数以下の場合、ページャ表示の文言なし
				if (iTotalProductCounts <= this.ProductReviewCount)
				{
					lbReviewDispPager.Visible = false;
				}
				else
				{
					this.TotalComment = iTotalProductCounts + "";
					lbReviewDispPager.Visible = true;
				}
			}

			//------------------------------------------------------
			// コメント一覧データバインド
			//------------------------------------------------------
			this.WrProductReview.DataSource = dvProductReview;
			this.WrProductReview.DataBind();
		}
		//------------------------------------------------------
		// コメントなしのとき
		//------------------------------------------------------
		else
		{
			// コメント一覧非表示
			this.WdvProductReviewComments.Visible = false;

			// ページャ表示制御
			lbReviewDispPager.Visible = false;
		}

		// Enter押下でサブミットさせない ※FireFoxでは関数内からevent.keyCodeが呼べないらしい
		this.WtbReviewTitle.Attributes["onkeypress"]
				= this.WtbNickName.Attributes["onkeypress"]
				= "if (event.keyCode==13){ return false; }";
	}

	/// <summary>
	/// 商品詳細画面の表示へ戻る
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBackToDetail_Click(object sender, EventArgs e)
	{
		// コメント一覧表示
		this.WdvProductReviewComments.Visible = true;
		lbDispCommentForm.Visible = true;

		// コメント入力欄非表示
		this.WdvProductReviewInput.Visible = false;
	}

	/// <summary>
	/// 商品レビュープレビュー表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReviewConfirm_Click(object sender, EventArgs e)
	{
		lbDispCommentForm.Visible = false;
		this.WdvProductReviewComments.Visible = false;
		this.WdvProductReviewInput.Visible = true;

		//------------------------------------------------------
		// 入力チェック＆重複チェック
		//------------------------------------------------------
		Hashtable htParam = new Hashtable();
		htParam.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_RATING, this.WddlReviewRating.Text);		// 評価
		htParam.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_TITLE, StringUtility.RemoveUnavailableControlCode(this.WtbReviewTitle.Text));		// タイトル
		htParam.Add(Constants.FIELD_PRODUCTREVIEW_NICK_NAME, StringUtility.RemoveUnavailableControlCode(this.WtbNickName.Text));				// ニックネーム
		htParam.Add(Constants.FIELD_PRODUCTREVIEW_REVIEW_COMMENT, StringUtility.RemoveUnavailableControlCode(this.WtbReviewComment.Text));	// コメント

		//------------------------------------------------------
		// エラーチェック＆カスタムバリデータへセット
		//------------------------------------------------------
		Dictionary<string, string> dictionaryErrorMessages = Validator.ValidateAndGetErrorContainer("ProductReview", htParam);
		if (dictionaryErrorMessages.Count != 0)
		{
			// カスタムバリデータ取得
			List<CustomValidator> customValidatorList = new List<CustomValidator>();
			CreateCustomValidators(this, customValidatorList);

			// エラーをカスタムバリデータへ
			SetControlViewsForError("ProductReview", dictionaryErrorMessages, customValidatorList);

			return;
		}

		//------------------------------------------------------
		// プレビュー表示
		//------------------------------------------------------
		this.WdvProductReviewInput.Visible = false;
		this.WdvProductReviewConfirm.Visible = true;

		this.WsIconImageRating1.Visible = (this.WddlReviewRating.SelectedValue == "1");
		this.WsIconImageRating2.Visible = (this.WddlReviewRating.SelectedValue == "2");
		this.WsIconImageRating3.Visible = (this.WddlReviewRating.SelectedValue == "3");
		this.WsIconImageRating4.Visible = (this.WddlReviewRating.SelectedValue == "4");
		this.WsIconImageRating5.Visible = (this.WddlReviewRating.SelectedValue == "5");

		this.ReviewTitle = WebSanitizer.HtmlEncode(StringUtility.RemoveUnavailableControlCode(this.WtbReviewTitle.Text));
		this.NickName = WebSanitizer.HtmlEncode(StringUtility.RemoveUnavailableControlCode(this.WtbNickName.Text));
		this.ReviewComment = WebSanitizer.HtmlEncodeChangeToBr(StringUtility.RemoveUnavailableControlCode(this.WtbReviewComment.Text));
	}

	/// <summary>
	/// レビュー入力画面の表示へ戻る
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBackToInput_Click(object sender, EventArgs e)
	{
		this.WdvProductReviewComments.Visible = false;
		this.WdvProductReviewInput.Visible = true;
		this.WdvProductReviewConfirm.Visible = false;
	}

	/// <summary>
	/// 商品レビューインサート処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbProductReviewRegist_Click(object sender, EventArgs e)
	{
		var sqlAccessor = new SqlAccessor();
		try
		{
			// トランザクション開始
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();
			
			// レビュー投稿
			var model = RegisterProductReview(sqlAccessor);

			// レビュー投稿ポイント付与
			var errorMessages = IssueReviewPoint(model, sqlAccessor);
			if (string.IsNullOrEmpty(errorMessages) == false)
			{
				// トランザクションコミット
				sqlAccessor.RollbackTransaction();

				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// トランザクションコミット
			sqlAccessor.CommitTransaction();
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(ex);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// 即時反映の場合は画面遷移
		if (Constants.PRODUCTREVIEW_AUTOOPEN_ENABLED)
		{
			SetProductReviewDataForDisplay();

			if (string.IsNullOrEmpty(this.LoginUserId) == false)
			{
				if (Constants.CROSS_POINT_OPTION_ENABLED)
				{
					UserUtility.AdjustPointByCrossPointApi(this.LoginUserId);
				}
				this.Process.LoginUserPoint = PointOptionUtility.GetUserPoint(this.LoginUserId);
			}

		}
		// 即時反映でない場合は非表示
		else
		{
			this.WdvProductReviewComments.Visible = false;
			this.WdvProductReviewConfirm.Visible = false;
			this.WdvProductReviewComplete.Visible = true;
		}
	}

	/// <summary>
	/// 商品レビュー投稿登録
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>商品レビューモデル</returns>
	private ProductReviewModel RegisterProductReview(SqlAccessor accessor)
	{
		var service = new ProductReviewService();

		// モデル作成
		var model = new ProductReviewModel
		{
			ShopId = this.ShopId,
			ProductId = this.ProductId,
			ReivewNo = service.GetNewReviewNo(this.ShopId, this.ProductId, accessor),
			UserId = StringUtility.ToEmpty(this.LoginUserId),
			NickName = StringUtility.RemoveUnavailableControlCode(this.WtbNickName.Text),
			ReviewRating = this.WddlReviewRating.SelectedValue,
			ReviewTitle = StringUtility.RemoveUnavailableControlCode(this.WtbReviewTitle.Text),
			ReviewComment = StringUtility.RemoveUnavailableControlCode(this.WtbReviewComment.Text),
		};
		if (Constants.PRODUCTREVIEW_AUTOOPEN_ENABLED)
		{
			model.OpenFlg = Constants.FLG_PRODUCTREVIEW_OPEN_FLG_VALID;
			model.DateOpened = DateTime.Now;
		}

		lock (m_lockObject)
		{
			service.Insert(model, accessor);
		}

		return model;
	}

	/// <summary>
	/// レビューポイント発行
	/// <param name="model">商品レビューモデル</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// </summary>
	private string IssueReviewPoint(ProductReviewModel model, SqlAccessor accessor)
	{
		if((this.IsIssuanceReviewPoint == false) || string.IsNullOrEmpty(this.LoginUserId)) return string.Empty;
		var result = PointOptionUtility.AddReviewPoint(model, Constants.FLG_LASTCHANGED_USER, accessor);
		return result;
	}

	/// <summary>
	/// 商品レビューコメント入力欄表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDispCommentForm_Click(object sender, EventArgs e)
	{
		// コメント一覧・文言削除（ページングで入力内容が保持できないため入力時は一覧を表示させない）
		this.WdvProductReviewComments.Visible = false;
		lbDispCommentForm.Visible = false;

		// コメント入力欄表示
		this.WdvProductReviewInput.Visible = true;

		// デフォルト値設定
		this.WtbNickName.Text = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_LOGIN_USER_NICK_NAME]);
		this.WddlReviewRating.SelectedIndex = 0;
		this.WtbReviewTitle.Text = "";
		this.WtbReviewComment.Text = "";
	}

	/// <summary>
	/// 商品レビュー全件表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReviewDispPager_Click(object sender, EventArgs e)
	{
		Response.Redirect(
			CreateProductDetailUrl(
				this.ShopId,
				this.CategoryId,
				this.BrandId,
				this.SearchWord,
				this.ProductId,
				this.VariationId,
				this.ProductName,
				this.BrandName,
				"1") + "#aProductReview");
	}

	/// <summary>
	/// レビュー一覧に戻るクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
	{
		this.WdvProductReviewComments.Visible = true;
		this.WdvProductReviewComplete.Visible = false;
	}

	/// <summary>商品名（外部設定）</summary>
	public string ProductName
	{
		get { return (string)ViewState["ProductName"]; }
		set { ViewState["ProductName"] = value; }
	}
	/// <summary>商品レビュー表示件数（外部から設定可能）</summary>
	public int ProductReviewCount
	{
		get { return m_ProductReviewCount; }
		set { m_ProductReviewCount = value; }
	}
	int m_ProductReviewCount = 5;

	/// <summary>ページャーHTML</summary>
	protected string PagerHtml
	{
		get { return (string)ViewState["PagerHtml"]; }
		private set { ViewState["PagerHtml"] = value; }
	}
	/// <summary>レビュータイトル</summary>
	protected string ReviewTitle
	{
		get { return (string)ViewState["ReviewTitle"]; }
		private set { ViewState["ReviewTitle"] = value; }
	}
	/// <summary>ニックネーム</summary>
	protected string NickName
	{
		get { return (string)ViewState["NickName"]; }
		private set { ViewState["NickName"] = value; }
	}
	/// <summary>レビューコメント</summary>
	protected string ReviewComment
	{
		get { return (string)ViewState["ReviewComment"]; }
		private set { ViewState["ReviewComment"] = value; }
	}
	/// <summary>コメント件数</summary>
	protected string TotalComment
	{
		get { return (string)ViewState["TotalComment"]; }
		private set { ViewState["TotalComment"] = value; }
	}
	/// <summary>レビュー投稿ポイント付与判定</summary>
	protected bool IsIssuanceReviewPoint
	{
		get
		{
			var result = Constants.W2MP_POINT_OPTION_ENABLED
				&& Constants.REVIEW_REWARD_POINT_ENABLED
				&& Constants.PRODUCTREVIEW_AUTOOPEN_ENABLED;
			return result;
		}
	}

}
