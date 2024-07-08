/*
=========================================================================================================
  Module      : 会員ランク設定確認ページ処理(MemberRankConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.RefreshFileManager;
using w2.Domain.MemberRank;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

public partial class Form_MemberRank_MemberRankConfirm : MemberRankPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			// ランクID
			string strRankId = Request[Constants.REQUEST_KEY_MEMBERRANK_ID];
			ViewState.Add(Constants.REQUEST_KEY_MEMBERRANK_ID, strRankId);
			// アクションステータス
			string strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, strActionStatus);

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(strActionStatus);
			this.MemberRankTranslationData = new NameTranslationSettingModel[0];

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			// 新規登録・更新
			if (strActionStatus == Constants.ACTION_STATUS_INSERT ||
				strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				// 処理区分チェック
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

				// 登録・編集画面のパラメタ取得
				Hashtable htParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_MEMBERRANK_INFO];

				lRankId.Text = WebSanitizer.HtmlEncode(htParam[Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID]);			// ランクID
				lRankName.Text = WebSanitizer.HtmlEncode(htParam[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME]);		// ランク名
				lRankOrder.Text = WebSanitizer.HtmlEncode(htParam[Constants.FIELD_MEMBERRANK_MEMBER_RANK_ORDER]);	// ランク順位
				
				// 注文割引方法
				lOrderDiscountType.Text = WebSanitizer.HtmlEncode(
					GetOrderDiscountType(
						htParam[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_VALUE].ToPriceDecimal(),
						(string)htParam[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_TYPE]));

				// 注文金額割引き閾値
				lOrderDiscountThresholdPrice.Text = WebSanitizer.HtmlEncode(
					StringUtility.ToPrice((htParam[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_THRESHOLD_PRICE])));

				// ポイント加算方法
				lPointAddType.Text = WebSanitizer.HtmlEncode(
											htParam[Constants.FIELD_MEMBERRANK_POINT_ADD_VALUE]
											+ " "
											+ ValueText.GetValueText(Constants.TABLE_MEMBERRANK, Constants.FIELD_MEMBERRANK_POINT_ADD_TYPE, htParam[Constants.FIELD_MEMBERRANK_POINT_ADD_TYPE]));

				// 配送料割引方法
				lShippingDiscountType.Text = WebSanitizer.HtmlEncode(
					GetShippingDiscountType(
						htParam[Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_VALUE].ToPriceDecimal(),
						(string)htParam[Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_TYPE]));

				// 定期会員割引率
				if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
				{
					lFixedPurchaseDiscountRate.Text = string.Format("{0}%", WebSanitizer.HtmlEncode(htParam[Constants.FIELD_MEMBERRANK_FIXED_PURCHASE_DISCOUNT_RATE]));
				}
				// ランクメモ
				lRankMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(htParam[Constants.FIELD_MEMBERRANK_MEMBER_RANK_MEMO]);

				// 有効フラグ
				lValidFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MEMBERRANK, Constants.FIELD_MEMBERRANK_VALID_FLG, htParam[Constants.FIELD_MEMBERRANK_VALID_FLG]));
			}
			// 詳細表示
			else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				var memberRank = MemberRankService.Get(strRankId);

				// 該当データが有る場合
				if (memberRank != null)
				{
					lRankId.Text = WebSanitizer.HtmlEncode(memberRank.MemberRankId);		// ランクID
					lRankName.Text = WebSanitizer.HtmlEncode(memberRank.MemberRankName);	// ランク名
					lRankOrder.Text = WebSanitizer.HtmlEncode(memberRank.MemberRankOrder);	// ランク順位

					// 注文割引方法
					lOrderDiscountType.Text = WebSanitizer.HtmlEncode(
						GetOrderDiscountType(memberRank.OrderDiscountValue, memberRank.OrderDiscountType));

					// 注文金額割引き閾値
					lOrderDiscountThresholdPrice.Text = WebSanitizer.HtmlEncode(StringUtility.ToPrice(memberRank.OrderDiscountThresholdPrice));

					// ポイント加算方法
					lPointAddType.Text = WebSanitizer.HtmlEncode(
					memberRank.PointAddValue
												+ " "
					+ ValueText.GetValueText(Constants.TABLE_MEMBERRANK, Constants.FIELD_MEMBERRANK_POINT_ADD_TYPE, memberRank.PointAddType));

					// 配送料割引方法
					lShippingDiscountType.Text = WebSanitizer.HtmlEncode(
						GetShippingDiscountType(memberRank.ShippingDiscountValue, memberRank.ShippingDiscountType));

					// 定期会員割引率
					if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
					{
						lFixedPurchaseDiscountRate.Text = string.Format("{0}%", WebSanitizer.HtmlEncode(memberRank.FixedPurchaseDiscountRate));
					}
					// ランクメモ
					lRankMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(memberRank.MemberRankMemo);

					// 有効フラグ
					lValidFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MEMBERRANK, Constants.FIELD_MEMBERRANK_VALID_FLG, memberRank.ValidFlg));

					// 翻訳設定情報取得
					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						this.MemberRankTranslationData = GetMemberRankTranslationData(memberRank.MemberRankId);
						rTranslationRankName.DataBind();
					}
				}
				// 該当データが無い場合
				else
				{
					// エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
				}
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <param name="strActionStatus"></param>
	private void InitializeComponents(string strActionStatus)
	{
		// 新規登録
		if (strActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			btnInsertTop.Visible = true;
			btnInsertBottom.Visible = true;
			trConfirm.Visible = true;
		}
		// 更新
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trConfirm.Visible = true;
		}
		// 詳細
		else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnEditTop.Visible = true;
			btnEditBottom.Visible = true;
			btnDeleteTop.Visible = true;
			btnDeleteBottom.Visible = true;
			trDetail.Visible = true;
		}
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_LIST);
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へのURLを作成
		StringBuilder sbUrl = new StringBuilder(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_REGISTER);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_MEMBERRANK_ID).Append("=").Append(HttpUtility.UrlEncode((string)ViewState[Constants.REQUEST_KEY_MEMBERRANK_ID]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_UPDATE));

		// 編集画面へ遷移
		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>削除対象のランクが、ユーザー、または商品に割当てられていた場合は削除不可能</remarks>
	protected void btnDeleteTop_Click(object sender, System.EventArgs e)
	{
		string strRankId = (string)ViewState[Constants.REQUEST_KEY_MEMBERRANK_ID];

		// ユーザー、または商品に割当てられているかチェック
		var canDelete = MemberRankService.CheckMemberRankDelete(strRankId);

		// 削除対象のランクが、ユーザーにも商品にも割当てられていない場合
		if (canDelete)
		{
			// ランク情報を削除
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MemberRank", "DeleteMemberRank"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, strRankId);

				int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}

			// 各サイトの会員ランク情報更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.MemberRank).CreateUpdateRefreshFile();

			// 一覧画面へ戻る
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_LIST);
		}
		else
		{
			// エラーページで、会員情報が削除できない旨を通知
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MEMBER_RANK_DELETE_IMPOSSIBLE_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 更新ボタンクリック処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MemberRank", "UpdateMemberRank"))
		{
			Hashtable htInput = (Hashtable)Session[Constants.SESSIONPARAM_KEY_MEMBERRANK_INFO];
			htInput[Constants.FIELD_MEMBERRANK_LAST_CHANGED] = this.LoginOperatorName;	// 最終更新者
			int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}

		// 各サイトの会員ランク情報更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.MemberRank).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_LIST);
	}

	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 会員ランク情報存在チェック
		// (※会員ランク情報が存在しない場合はデフォルト会員ランクとして設定するため)
		//------------------------------------------------------
		DataView dvMemberRank = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MemberRank", "GetMemberRankListAllIncludeValidInvalid"))
		{
			dvMemberRank = sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
		}

		// デフォルト会員ランクとして登録するかを判定
		string strDefaultMemberRank = (dvMemberRank.Count == 0) ?
			Constants.FLG_MEMBERRANK_DEFAULT_MEMBER_RANK_SETTING_FLG_ON : Constants.FLG_MEMBERRANK_DEFAULT_MEMBER_RANK_SETTING_FLG_OFF;

		//------------------------------------------------------
		// 会員ランク情報登録
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MemberRank", "InsertMemberRank"))
		{
			Hashtable htInput = (Hashtable)Session[Constants.SESSIONPARAM_KEY_MEMBERRANK_INFO];
			htInput.Add(Constants.FIELD_MEMBERRANK_DEFAULT_MEMBER_RANK_SETTING_FLG, strDefaultMemberRank);
			htInput[Constants.FIELD_MEMBERRANK_LAST_CHANGED] = this.LoginOperatorName;	// 最終更新者
			int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}

		// 各サイトの会員ランク情報更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.MemberRank).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_LIST);
	}

	#region -GetMemberRankTranslationData 会員ランク設定翻訳情報取得
	/// <summary>
	/// 会員ランク設定翻訳情報取得
	/// </summary>
	/// <param name="memberRankId">会員ランクID</param>
	/// <returns>会員ランク設定翻訳情報</returns>
	private NameTranslationSettingModel[] GetMemberRankTranslationData(string memberRankId)
	{
		var searchCondition = new NameTranslationSettingSearchCondition
		{
			DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK,
			MasterId1 = memberRankId,
			MasterId2 = string.Empty,
			MasterId3 = string.Empty,
		};
		var translationData = new NameTranslationSettingService().GetTranslationSettingsByMasterId(searchCondition);
		return translationData;
	}
	#endregion

	/// <summary>会員ランク翻訳設定情報</summary>
	protected NameTranslationSettingModel[] MemberRankTranslationData
	{
		get { return (NameTranslationSettingModel[])ViewState["memberrank_translation_data"]; }
		set { ViewState["memberrank_translation_data"] = value; }
	}
}