/*
 =========================================================================================================
  Module      : ノベルティ情報登録ページ処理(NoveltyRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Input;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.RefreshFileManager;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.Domain.MemberRank;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.Novelty;

public partial class Form_Novelty_NoveltyRegister : NoveltyPage
{
	/// <summary>登録・更新完了メッセージ表示用パラメータ名</summary>
	private string REQUEST_KEY_SUCCESS = "success";
	/// <summary>対象商品入力テキストボックスのサイズ（通常）</summary>
	protected const int TEXT_BOX_ROWS_NORMAL = 3;
	/// <summary>対象商品入力テキストボックスのサイズ（拡大）</summary>
	protected const int TEXT_BOX_ROWS_LARGE = 10;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 表示コンポーネント初期化
			InitializeComponents();

			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_COPY_INSERT:
					// DBからデータを取得し画面にセット
					SetValues(GetNoveltyInfo());
					break;

				case Constants.ACTION_STATUS_INSERT:
					// 空データを画面にセット
					btnAddTargetItem_Click(null, null);
					btnAddItem_Click(null, null);
					break;
				default:
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					break;
			}

			// 対象アイテム表示切替
			rblTarget_SelectedIndexChanged(null, null);
		}
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnToList_Click(object sender, EventArgs e)
	{
		var url = CreateNoveltyListUrl(this.SearchInfo.NoveltyId, this.SearchInfo.NoveltyDisplayName, this.SearchInfo.NoveltyName, this.SearchInfo.NoveltyStatus, this.SearchInfo.SortKbn, this.SearchInfo.PageNum);
		Response.Redirect(url);
	}

	/// <summary>
	/// コピー新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateNoveltyRegisterUrl(this.RequestNoveltyId, Constants.ACTION_STATUS_COPY_INSERT));
	}

	/// <summary>
	/// 登録・更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertUpdate_Click(object sender, EventArgs e)
	{
		// 登録?
		var isInsert = (this.ActionStatus != Constants.ACTION_STATUS_UPDATE);

		// 入力情報取得
		var input = GetInputData();

		// 入力チェック
		var success = input.Validate(isInsert);

		// 画面にエラーメッセージをセット
		trNoveltyErrorMessagesTitle.Visible =
			trNoveltyErrorMessages.Visible = (input.ErrorMessage.Length != 0); ;
		lbNoveltyErrorMessages.Text = input.ErrorMessage;
		SetTargetItemInfo(input.TargetItemList);
		SetGrantConditionsInfo(input.GrantConditionsList);

		// エラーの場合、処理を抜ける
		if (success == false) return;

		// DB登録・更新
		var model = input.CreateModel();
		if (isInsert)
		{
			new NoveltyService().Insert(model);
		}
		else
		{
			new NoveltyService().Update(model);
		}

		// 各サイトの情報を最新状態にする
		RefreshFileManagerProvider.GetInstance(RefreshFileType.Novelty).CreateUpdateRefreshFile();

		// 登録・更新画面へ
		var url = CreateNoveltyRegisterUrl(model.NoveltyId, Constants.ACTION_STATUS_UPDATE) + "&" + REQUEST_KEY_SUCCESS + "=1";
		Response.Redirect(url);
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		// DBから削除
		new NoveltyService().Delete(this.LoginOperatorShopId, this.RequestNoveltyId);

		// 各サイトの情報を最新状態にする
		RefreshFileManagerProvider.GetInstance(RefreshFileType.Novelty).CreateUpdateRefreshFile();

		// ノベルティ設定一覧へ遷移
		var url = CreateNoveltyListUrl(this.SearchInfo.NoveltyId, this.SearchInfo.NoveltyDisplayName, this.SearchInfo.NoveltyName, this.SearchInfo.NoveltyStatus, this.SearchInfo.SortKbn);
		Response.Redirect(url);
	}

	/// <summary>
	/// 対象アイテム表示切替
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblTarget_SelectedIndexChanged(object sender, EventArgs e)
	{
		foreach (RepeaterItem item in rTargetItemList.Items)
		{
			// アイテム入力欄の表示制御
			item.FindControl("trBrandId").Visible
				= item.FindControl("trCategoryId").Visible
				= item.FindControl("trProductId").Visible
				= item.FindControl("trVariationId").Visible
				= (((RadioButtonList)item.FindControl("rblTarget")).SelectedValue != Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_ALL);
		}
	}

	/// <summary>
	/// 対象アイテム追加
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddTargetItem_Click(object sender, EventArgs e)
	{
		// 対象アイテム追加
		var inputTargetItemList = GetTargetItemInput().ToList();
		var inputTargetItem = new NoveltyTargetItemInput();
		inputTargetItem.NoveltyTargetItemNo = inputTargetItemList.Count + 1;
		inputTargetItemList.Add(inputTargetItem);

		// 画面にセット
		SetTargetItemInfo(inputTargetItemList.ToArray());
	}

	/// <summary>
	/// 対象アイテム削除
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rTargetItemList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// 対象アイテム削除?
		if (e.CommandName == "delete")
		{
			// 1件の場合は削除不可とする
			var inputTargetItemList = GetTargetItemInput().ToList();
			if (inputTargetItemList.Count == 1) return;

			// 対象アイテム削除
			inputTargetItemList.Remove(inputTargetItemList[int.Parse(e.CommandArgument.ToString())]);

			// 画面にセット
			SetTargetItemInfo(inputTargetItemList.ToArray());
		}
	}

	/// <summary>
	/// 付与条件追加
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddItem_Click(object sender, EventArgs e)
	{
		// 付与条件追加
		var inputGrantConditionsList = GeGrantConditionsInput().ToList();
		var inputGrantConditions = new NoveltyGrantConditionsInput();
		inputGrantConditions.ConditionNo = inputGrantConditionsList.Count + 1;
		// 1つ目は対象金額（以上）に0をセット
		if (inputGrantConditions.ConditionNo == 1)
		{
			inputGrantConditions.AmountBegin = "0";
		}
		inputGrantConditionsList.Add(inputGrantConditions);

		// 画面にセット
		SetGrantConditionsInfo(inputGrantConditionsList.ToArray());
	}

	/// <summary>
	///  付与条件削除
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rGrantConditionsList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// 付与条件削除?
		if (e.CommandName == "delete")
		{
			// 1件の場合は削除不可とする
			var inputGrantConditionsList = GeGrantConditionsInput().ToList();
			if (inputGrantConditionsList.Count == 1) return;

			// 付与条件削除
			inputGrantConditionsList.Remove(inputGrantConditionsList[int.Parse(e.CommandArgument.ToString())]);

			// 画面にセット
			SetGrantConditionsInfo(inputGrantConditionsList.ToArray());
		}
	}

	/// <summary>
	/// 入力域リサイズ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbResize_Click(object sender, EventArgs e)
	{
		// 対象コントロール取得
		var targetIndex = int.Parse(((LinkButton)sender).CommandArgument);
		var tbItems = new TextBox();
		var lbResizeNormal = new LinkButton();
		var lbResizeLarge = new LinkButton();

		switch (((LinkButton)sender).CommandName)
		{
			case "lbBrandResize":
				tbItems = (TextBox)rTargetItemList.Items[targetIndex].FindControl("tbBrandId");
				lbResizeNormal = (LinkButton)rTargetItemList.Items[targetIndex].FindControl("lbBrandResizeNormal");
				lbResizeLarge = (LinkButton)rTargetItemList.Items[targetIndex].FindControl("lbBrandResizeLarge");
				break;

			case "lbCategoryResize":
				tbItems = (TextBox)rTargetItemList.Items[targetIndex].FindControl("tbCategoryId");
				lbResizeNormal = (LinkButton)rTargetItemList.Items[targetIndex].FindControl("lbCategoryResizeNormal");
				lbResizeLarge = (LinkButton)rTargetItemList.Items[targetIndex].FindControl("lbCategoryResizeLarge");
				break;

			case "lbProductResize":
				tbItems = (TextBox)rTargetItemList.Items[targetIndex].FindControl("tbProductId");
				lbResizeNormal = (LinkButton)rTargetItemList.Items[targetIndex].FindControl("lbProductResizeNormal");
				lbResizeLarge = (LinkButton)rTargetItemList.Items[targetIndex].FindControl("lbProductResizeLarge");
				break;

			case "lbProductVariationResize":
				tbItems = (TextBox)rTargetItemList.Items[targetIndex].FindControl("tbProductVariationId");
				lbResizeNormal = (LinkButton)rTargetItemList.Items[targetIndex].FindControl("lbProductVariationResizeNormal");
				lbResizeLarge = (LinkButton)rTargetItemList.Items[targetIndex].FindControl("lbProductVariationResizeLarge");
				break;

			case "lbResize":
				tbItems = (TextBox)rGrantConditionsList.Items[targetIndex].FindControl("tbNoveltyItems");
				lbResizeNormal = (LinkButton)rGrantConditionsList.Items[targetIndex].FindControl("lbResizeNormal");
				lbResizeLarge = (LinkButton)rGrantConditionsList.Items[targetIndex].FindControl("lbResizeLarge");
				break;
		}

		// 対象コントロールに値をセット
		var textBoxRows = ((tbItems.Rows == TEXT_BOX_ROWS_LARGE) ? TEXT_BOX_ROWS_NORMAL : TEXT_BOX_ROWS_LARGE);
		tbItems.Rows = textBoxRows;
		lbResizeNormal.Visible = (textBoxRows == TEXT_BOX_ROWS_LARGE);
		lbResizeLarge.Visible = (textBoxRows == TEXT_BOX_ROWS_NORMAL);
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 表示制御
		switch (this.ActionStatus)
		{
			// 新規登録・コピー新規登録
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				trDisplayNoveltyId.Visible = false;
				trInputNoveltyId.Visible = true;
				trRegsit.Visible = true;
				btnInsertTop.Visible = btnInsertBottom.Visible = true;
				btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = false;
				btnDeleteTop.Visible = btnDeleteBottom.Visible = false;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = false;
				break;
			// 更新
			case Constants.ACTION_STATUS_UPDATE:
				trInputNoveltyId.Visible = false;
				trDisplayNoveltyId.Visible = true;
				trEdit.Visible = true;
				btnInsertTop.Visible = btnInsertBottom.Visible = false;
				btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = true;
				btnDeleteTop.Visible = btnDeleteBottom.Visible = true;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = true;
				// 登録・更新メッセージ表示
				divComp.Visible = this.IsDisplaySuccess;
				// 作成日・更新日・最終更新者表示
				trDateCreated.Visible = trDateChanged.Visible = trLastChanged.Visible = true;
				break;
		}
	}

	/// <summary>
	/// DBからノベルティ設定取得
	/// </summary>
	private NoveltyModel GetNoveltyInfo()
	{
		var model = new NoveltyService().Get(this.LoginOperatorShopId, this.RequestNoveltyId);
		if (model == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		return model;
	}

	/// <summary>
	/// 画面に値をセット
	/// </summary>
	/// <param name="model">モデル</param>
	private void SetValues(NoveltyModel model)
	{
		// 入力情報作成
		var novelty = new NoveltyInput(model);

		tbNoveltyId.Text = lNoveltyId.Text = novelty.NoveltyId;
		tbNoveltyDisplayName.Text = novelty.NoveltyDispName;
		tbNoveltyName.Text = novelty.NoveltyName;
		tbDiscription.Text = novelty.Discription;
		var dateBegin = DateTime.Parse(novelty.DateBegin);
		ucDateTimePickerPeriod.SetStartDate(dateBegin);
		if (novelty.DateEnd != null)
		{
			var dateEnd = DateTime.Parse(novelty.DateEnd);
			ucDateTimePickerPeriod.SetEndDate(dateEnd);
		}
		cbValidFlg.Checked = novelty.IsValid;
		lDateCreated.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				model.DateCreated,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		lDateChanged.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				model.DateChanged,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		lLastChanged.Text = WebSanitizer.HtmlEncode(model.LastChanged);
		// 対象アイテムを画面にセット
		SetTargetItemInfo(novelty.TargetItemList);
		// 付与条件を画面にセット
		SetGrantConditionsInfo(novelty.GrantConditionsList);

		// 自動付与フラグ
		cbAutoAdditionalFlg.Checked = (novelty.AutoAdditionalFlg == Constants.FLG_NOVELTY_AUTO_ADDITIONAL_FLG_VALID);

		// 翻訳設定情報セット
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			this.NoveltyTranslationData = GetNoveltyTranslationData(novelty.NoveltyId);
			rTranslationNoveltyDispName.DataBind();
		}
	}

	/// <summary>
	/// 対象アイテムを画面にセット
	/// </summary>
	/// <param name="targetItemList">対象アイテムリスト</param>
	private void SetTargetItemInfo(NoveltyTargetItemInput[] targetItemList)
	{
		// データバインド
		rTargetItemList.DataSource = targetItemList;
		rTargetItemList.DataBind();
		// 対象アイテム表示切替
		rblTarget_SelectedIndexChanged(null, null);
	}

	/// <summary>
	/// 付与条件を画面にセット
	/// </summary>
	/// <param name="targetItemList">付与条件リスト</param>
	private void SetGrantConditionsInfo(NoveltyGrantConditionsInput[] grantConditionsList)
	{
		// データバインド
		rGrantConditionsList.DataSource = grantConditionsList;
		rGrantConditionsList.DataBind();

		// 会員ランク情報取得
		var memberRanks = Constants.MEMBER_RANK_OPTION_ENABLED ? MemberRankOptionUtility.GetMemberRankList() : new MemberRankModel[0];

		foreach (RepeaterItem item in rGrantConditionsList.Items)
		{
			// 適用対象会員ドロップダウンコントロール取得
			var userRank = ((DropDownList)item.FindControl("ddlUserRank"));

			// 全ユーザ追加
			userRank.Items.Add(
				new ListItem(
					ValueText.GetValueText(
						Constants.TABLE_NOVELTYGRANTCONDITIONS,
						Constants.FIELD_NOVELTYGRANTCONDITIONS_USER_RANK_ID,
						Constants.FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBERRANK_ALL),
					Constants.FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBERRANK_ALL));
			// 会員ランク追加
			foreach (var memberRank in memberRanks)
			{
				userRank.Items.Add(
					new ListItem(
						memberRank.MemberRankName,
						memberRank.MemberRankId));
			}
			// 会員ランクが存在しない or 会員ランクが無効の場合、追加する
			// ※誤って"全ユーザ"で更新することを防止
			var userRankId = grantConditionsList[item.ItemIndex].UserRankId;
			if ((userRankId != Constants.FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBERRANK_ALL)
				&& (userRankId != Constants.FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBER_ONLY))
			{
				var userRankIdText = string.Format("{0}（{1}：{2}）",
					ReplaceTag("@@DispText.common_message.unknown@@"),
					ReplaceTag("@@DispText.member_rank.MemberRankId@@"),
					grantConditionsList[item.ItemIndex].UserRankId);
				userRank.Items.Add(
					new ListItem(
						userRankIdText,
						grantConditionsList[item.ItemIndex].UserRankId));
			}

			// 会員のみ追加
			userRank.Items.Add(
				new ListItem(
					ValueText.GetValueText(
						Constants.TABLE_NOVELTYGRANTCONDITIONS,
						Constants.FIELD_NOVELTYGRANTCONDITIONS_USER_RANK_ID,
						Constants.FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBER_ONLY),
					Constants.FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBER_ONLY));

			// 選択
			userRank.SelectedValue = grantConditionsList[item.ItemIndex].UserRankId;
		}
	}

	/// <summary>
	/// 画面の入力情報を取得
	/// </summary>
	private NoveltyInput GetInputData()
	{
		var input = new NoveltyInput();

		input.ShopId = this.LoginOperatorShopId;
		input.NoveltyId = StringUtility.ToHankaku(tbNoveltyId.Text.Trim());
		input.NoveltyDispName = tbNoveltyDisplayName.Text.Trim();
		input.NoveltyName = tbNoveltyName.Text.Trim();
		input.Discription = tbDiscription.Text.Trim();
		input.DateBegin = ucDateTimePickerPeriod.StartDateTimeString;
		if (string.IsNullOrEmpty(ucDateTimePickerPeriod.HfEndDate.Value) == false)
		{
			input.DateEnd = ucDateTimePickerPeriod.EndDateTimeString;
		}
		else
		{
			input.DateEnd = null;
		}
		input.ValidFlg = (cbValidFlg.Checked ? Constants.FLG_NOVELTY_VALID_FLG_VALID : Constants.FLG_NOVELTY_VALID_FLG_INVALID);
		input.LastChanged = this.LoginOperatorName;
		// 対象アイテム
		input.TargetItemList = GetTargetItemInput().ToArray();
		// 付与条件
		input.GrantConditionsList = GeGrantConditionsInput().ToArray();

		// 自動付与フラグ
		input.AutoAdditionalFlg = (cbAutoAdditionalFlg.Checked
			? Constants.FLG_NOVELTY_AUTO_ADDITIONAL_FLG_VALID
			: Constants.FLG_NOVELTY_AUTO_ADDITIONAL_FLG_INVALID);

		return input;
	}

	/// <summary>
	/// 画面の対象アイテム入力情報を取得
	/// </summary>
	/// <returns>対象アイテム入力情報</returns>
	private IEnumerable<NoveltyTargetItemInput> GetTargetItemInput()
	{
		foreach (RepeaterItem item in rTargetItemList.Items)
		{
			var inputTargetItem = new NoveltyTargetItemInput();
			inputTargetItem.NoveltyTargetItemNo = item.ItemIndex + 1;
			// 条件指定？
			inputTargetItem.IsItemTypeAll
				= (((RadioButtonList)item.FindControl("rblTarget")).SelectedValue == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_ALL);
			if (inputTargetItem.IsItemTypeAll == false)
			{
				var control = (TextBox)item.FindControl("tbBrandId");
				inputTargetItem.BrandId = GetDistinctItems(control.Text.Trim());
				inputTargetItem.BrandIdRows = control.Rows;
				control = (TextBox)item.FindControl("tbCategoryId");
				inputTargetItem.CategoryId = GetDistinctItems(control.Text.Trim());
				inputTargetItem.CategoryIdRows = control.Rows;
				control = (TextBox)item.FindControl("tbProductId");
				inputTargetItem.ProductId = GetDistinctItems(control.Text.Trim());
				inputTargetItem.ProductIdRows = control.Rows;
				control = (TextBox)item.FindControl("tbProductVariationId");
				inputTargetItem.VariationId = GetDistinctItems(control.Text.Trim());
				inputTargetItem.VariationIdRows = control.Rows;
				inputTargetItem.ErrorMessage = ((Label)item.FindControl("lbErrorMessage")).Text;
				inputTargetItem.ShopId = this.LoginOperatorShopId;
			}

			yield return inputTargetItem;
		}
	}

	/// <summary>
	/// 画面の付与条件入力情報を取得
	/// </summary>
	/// <returns>付与条件入力情報</returns>
	private IEnumerable<NoveltyGrantConditionsInput> GeGrantConditionsInput()
	{
		foreach (RepeaterItem item in rGrantConditionsList.Items)
		{
			var inputGrantConditions = new NoveltyGrantConditionsInput();
			inputGrantConditions.ConditionNo = item.ItemIndex + 1;
			inputGrantConditions.UserRankId = ((DropDownList)item.FindControl("ddlUserRank")).SelectedValue;
			inputGrantConditions.UserRankIdText = ((DropDownList)item.FindControl("ddlUserRank")).SelectedItem.Text;
			inputGrantConditions.AmountBegin = ((TextBox)item.FindControl("tbAmountBegin")).Text.Trim();
			inputGrantConditions.AmountEnd = ((TextBox)item.FindControl("tbAmountEnd")).Text.Trim();
			inputGrantConditions.ProductId = GetDistinctItems(((TextBox)item.FindControl("tbNoveltyItems")).Text.Trim());
			inputGrantConditions.ProductIdRows = ((TextBox)item.FindControl("tbNoveltyItems")).Rows;
			inputGrantConditions.ErrorMessage = ((Label)item.FindControl("lbErrorMessage")).Text;
			inputGrantConditions.ShopId = this.LoginOperatorShopId;

			yield return inputGrantConditions;
		}
	}

	/// <summary>
	/// 重複削除後のアイテム（文字列）取得
	/// </summary>
	/// <param name="value">値</param>
	/// <returns>重複削除後のアイテム（文字列）取得</returns>
	private string GetDistinctItems(string value)
	{
		return string.Join(Environment.NewLine, value.Replace(" ", "").Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Distinct()).Trim();
	}

	#region -GetNoveltyTranslationData ノベルティ設定翻訳設定情報
	/// <summary>
	/// ノベルティ設定翻訳設定情報
	/// </summary>
	/// <param name="noveltyId">ノベルティID</param>
	/// <returns>ノベルティ設定翻訳設定情報</returns>
	private NameTranslationSettingModel[] GetNoveltyTranslationData(string noveltyId)
	{
		var searchCondition = new NameTranslationSettingSearchCondition
		{
			DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY,
			MasterId1 = noveltyId,
			MasterId2 = string.Empty,
			MasterId3 = string.Empty,
		};
		var translationData = new NameTranslationSettingService().GetTranslationSettingsByMasterId(searchCondition);
		return translationData;
	}
	#endregion

	#region プロパティ
	/// <summary>作成日</summary>
	protected DateTime? DateCreated
	{
		get { return (DateTime?)ViewState["DateCreated"]; }
		set { ViewState["DateCreated"] = value; }
	}
	/// <summary>登録・更新完了メッセージ表示？</summary>
	protected bool IsDisplaySuccess
	{
		get { return StringUtility.ToEmpty(Request[REQUEST_KEY_SUCCESS]) == "1"; }
	}
	/// <summary>ノベルティ翻訳設定情報</summary>
	protected NameTranslationSettingModel[] NoveltyTranslationData
	{
		get { return (NameTranslationSettingModel[])ViewState["novelty_translation_data"]; }
		set { ViewState["novelty_translation_data"] = value; }
	}
	#endregion

	#region +ノベルティ設定入力クラス
	/// <summary>
	/// ノベルティ設定入力クラス
	/// </summary>
	public class NoveltyInput
	{
		/// <summary>データソース</summary>
		Hashtable DataSource { get; set; }

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public NoveltyInput()
		{
			this.DataSource = new Hashtable();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public NoveltyInput(NoveltyModel model)
			: this()
		{
			this.ShopId = model.ShopId;
			this.NoveltyId = model.NoveltyId;
			this.NoveltyDispName = model.NoveltyDispName;
			this.NoveltyName = model.NoveltyName;
			this.Discription = model.Discription;
			this.DateBegin = model.DateBegin.ToString();
			this.DateEnd = (model.DateEnd != null) ? model.DateEnd.ToString() : null;
			this.ValidFlg = model.ValidFlg;
			this.LastChanged = model.LastChanged;
			// ノベルティ対象アイテム枝番でグループ化し対象アイテムリストをセット
			this.TargetItemList = model.TargetItemList
				.GroupBy(t => t.NoveltyTargetItemNo)
				.Select(t => new NoveltyTargetItemInput(t.ToArray())).ToArray();
			// 付与条件リストをセット
			this.GrantConditionsList = model.GrantConditionsList.Select(g => new NoveltyGrantConditionsInput(g)).ToArray();
			this.AutoAdditionalFlg = model.AutoAdditionalFlg;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public NoveltyModel CreateModel()
		{
			var model = new NoveltyModel
			{
				ShopId = this.ShopId,
				NoveltyId = this.NoveltyId,
				NoveltyDispName = this.NoveltyDispName,
				NoveltyName = this.NoveltyName,
				Discription = this.Discription,
				DateBegin = DateTime.Parse(this.DateBegin),
				DateEnd = (this.DateEnd != null) ? DateTime.Parse(this.DateEnd) : (DateTime?)null,
				ValidFlg = this.ValidFlg,
				LastChanged = this.LastChanged,
				AutoAdditionalFlg = this.AutoAdditionalFlg
			};
			// 対象アイテム
			model.TargetItemList = this.TargetItemList.SelectMany(t => t.CreateModels(model)).ToArray();
			// 付与条件
			model.GrantConditionsList = this.GrantConditionsList.Select(g => g.CreateModel(model)).ToArray();

			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="checkDuplication">重複チェックするか</param>
		/// <returns>正常：true、エラー：false</returns>
		public bool Validate(bool checkDuplication)
		{
			bool result = true;

			// 入力チェック
			var errorMessage = new StringBuilder();
			errorMessage.Append(Validator.Validate("Novelty", this.DataSource));
			// 重複チェック
			if (checkDuplication && new NoveltyService().Get(this.ShopId, this.NoveltyId) != null)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION)
					.Replace("@@ 1 @@", ReplaceTag("@@DispText.novelty.NoveltyId@@")));
			}
			// 日付チェック
			if (string.IsNullOrEmpty(this.DateEnd) == false && this.DateBegin.CompareTo(this.DateEnd) == 1)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOVELTYGRANTCONDITIONS_ITEM_DATE_ERROR));
			}
			// エラーメッセージをセット
			this.ErrorMessage = errorMessage.ToString();
			if (this.ErrorMessage.Length != 0) result = false;

			// 対象アイテム入力チェック
			foreach (var inputTargetItem in this.TargetItemList)
			{
				inputTargetItem.Validate();
				if (inputTargetItem.ErrorMessage.Length != 0) result = false;
			}
			// 付与条件入力チェック
			foreach (var inputGrantConditions in this.GrantConditionsList)
			{
				inputGrantConditions.IsCheckMultiVariation = (this.AutoAdditionalFlg == Constants.FLG_NOVELTY_AUTO_ADDITIONAL_FLG_VALID);
				inputGrantConditions.Validate(this.GrantConditionsList);
				if (inputGrantConditions.ErrorMessage.Length != 0) result = false;
			}

			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_SHOP_ID] = value; }
		}
		/// <summary>ノベルティID</summary>
		public string NoveltyId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_NOVELTY_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_NOVELTY_ID] = value; }
		}
		/// <summary>ノベルティ名（表示用）</summary>
		public string NoveltyDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_NOVELTY_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_NOVELTY_DISP_NAME] = value; }
		}
		/// <summary>ノベルティ名（管理用）</summary>
		public string NoveltyName
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_NOVELTY_NAME]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_NOVELTY_NAME] = value; }
		}
		/// <summary>説明（管理用）</summary>
		public string Discription
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_DISCRIPTION]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_DISCRIPTION] = value; }
		}
		/// <summary>開始日時</summary>
		public string DateBegin
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_DATE_BEGIN]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_DATE_BEGIN] = value; }
		}
		/// <summary>終了日時</summary>
		public string DateEnd
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_DATE_END]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_DATE_END] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_VALID_FLG] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_LAST_CHANGED] = value; }
		}
		/// <summary>
		/// 有効フラグが有効？
		/// </summary>
		public bool IsValid { get { return (this.ValidFlg == Constants.FLG_NOVELTY_VALID_FLG_VALID); } }
		/// <summary>対象アイテムリスト</summary>
		public NoveltyTargetItemInput[] TargetItemList
		{
			get { return (NoveltyTargetItemInput[])this.DataSource["TargetItemList"]; }
			set { this.DataSource["TargetItemList"] = value; }
		}
		/// <summary>付与条件リスト</summary>
		public NoveltyGrantConditionsInput[] GrantConditionsList
		{
			get { return (NoveltyGrantConditionsInput[])this.DataSource["GrantConditionsList"]; }
			set { this.DataSource["GrantConditionsList"] = value; }
		}
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage
		{
			get { return StringUtility.ToEmpty(this.DataSource["ErrorMessage"]); }
			set { this.DataSource["ErrorMessage"] = value; }
		}
		/// <summary>自動付与フラグ</summary>
		public string AutoAdditionalFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_AUTO_ADDITIONAL_FLG]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_AUTO_ADDITIONAL_FLG] = value; }
		}
		#endregion
	}
	#endregion

	#region +ノベルティ対象アイテム入力クラス
	/// <summary>
	/// ノベルティ対象アイテム入力クラス
	/// </summary>
	public class NoveltyTargetItemInput
	{
		/// <summary>データソース</summary>
		Hashtable DataSource { get; set; }

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public NoveltyTargetItemInput()
		{
			this.DataSource = new Hashtable();
			// 行数初期化
			this.BrandIdRows = this.CategoryIdRows = this.ProductIdRows = this.VariationIdRows = TEXT_BOX_ROWS_NORMAL;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="models">モデル列</param>
		public NoveltyTargetItemInput(NoveltyTargetItemModel[] models)
			: this()
		{
			// 条件指定？
			this.IsItemTypeAll = models.Any(model => model.NoveltyTargetItemType == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_ALL);
			if (this.IsItemTypeAll == false)
			{
				// ブランドID
				var brandId = models
					.Where(i => i.NoveltyTargetItemType == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_BRAND)
					.OrderBy(o => o.NoveltyTargetItemTypeSortNo)
					.Select(v => v.NoveltyTargetItemValue);
				this.BrandId = string.Join(Environment.NewLine, brandId);
				// カテゴリID
				var categoryId = models
					.Where(i => i.NoveltyTargetItemType == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_CATEGORY)
					.OrderBy(o => o.NoveltyTargetItemTypeSortNo)
					.Select(v => v.NoveltyTargetItemValue);
				this.CategoryId = string.Join(Environment.NewLine, categoryId);
				// 商品ID
				var productId = models
					.Where(i => i.NoveltyTargetItemType == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_PRODUCT)
					.OrderBy(o => o.NoveltyTargetItemTypeSortNo)
					.Select(v => v.NoveltyTargetItemValue);
				this.ProductId = string.Join(Environment.NewLine, productId);
				// バリエーションID
				var variationId = models
					.Where(i => i.NoveltyTargetItemType == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_VARIATION)
					.OrderBy(o => o.NoveltyTargetItemTypeSortNo)
					.Select(v => v.NoveltyTargetItemValue);
				this.VariationId = string.Join(Environment.NewLine, variationId);
			}
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <param name="novelty">ノベルティ設定モデル</param>
		/// <returns>モデル列</returns>
		public IEnumerable<NoveltyTargetItemModel> CreateModels(NoveltyModel novelty)
		{
			// 共通するプロパティをセット
			var modelBase = new NoveltyTargetItemModel()
			{
				ShopId = novelty.ShopId,
				NoveltyId = novelty.NoveltyId,
				NoveltyTargetItemNo = this.NoveltyTargetItemNo,
				LastChanged = novelty.LastChanged
			};
			// 全商品？
			if (this.IsItemTypeAll)
			{
				var model = (NoveltyTargetItemModel)modelBase.Clone();
				model.NoveltyTargetItemType = Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_ALL;
				model.NoveltyTargetItemValue = "";
				model.NoveltyTargetItemTypeSortNo = 1;

				yield return model;
			}
			else
			{
				// 対象アイテム種別毎に値を格納
				var itemTypeValues = new Dictionary<string, string>()
				{
					{Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_BRAND, Constants.PRODUCT_BRAND_ENABLED ? this.BrandId : ""},
					{Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_CATEGORY, this.CategoryId},
					{Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_PRODUCT, this.ProductId},
					{Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_VARIATION, this.VariationId}
				};
				foreach (var itemType in itemTypeValues.Keys)
				{
					var sortNo = 0;
					foreach (var value in itemTypeValues[itemType].Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
					{
						sortNo++;
						var model = (NoveltyTargetItemModel)modelBase.Clone();
						model.NoveltyTargetItemType = itemType;
						model.NoveltyTargetItemValue = value;
						model.NoveltyTargetItemTypeSortNo = sortNo;

						yield return model;
					}
				}
			}
		}

		/// <summary>
		/// 検証
		/// </summary>
		public void Validate()
		{
			// エラーメッセージを初期化
			this.ErrorMessage = "";

			// 全商品の場合、処理を抜ける
			if (this.IsItemTypeAll) return;

			// 対象アイテム種別毎に値を格納
			var itemTypeValues = new Dictionary<string, string>()
			{
				{Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_BRAND, (Constants.PRODUCT_BRAND_ENABLED ? this.BrandId : "")},
				{Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_CATEGORY, this.CategoryId},
				{Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_PRODUCT, this.ProductId},
				{Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_VARIATION, this.VariationId}
			};

			// 入力チェック
			var isNoItem = true;
			var errorMessage = new StringBuilder();
			foreach (var field in itemTypeValues.Keys)
			{
				foreach (var itemTypeValue in itemTypeValues[field].Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
				{
					isNoItem = false;
					var value = itemTypeValue;

					// バリエーションIDは入力形式チェック
					if (field == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_VARIATION)
					{
						if (value.Split(',').Length != 2)
						{
							errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOVELTY_TARGET_ITEM_VARIATION_INPUT_ERROR).Replace("@@ 1 @@", value));
							continue;
						}
						value = value.Split(',')[1];
					}

					var input = new Hashtable() { { field, value } };
					errorMessage.Append(Validator.Validate("Novelty", input).Replace("@@ 1 @@", itemTypeValue));

					// 商品有効性チェック
					if ((field == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_PRODUCT)
						|| (field == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_VARIATION))
					{
						errorMessage.Append(CheckValidProduct(this.ShopId, itemTypeValue.Split(',')[0]));
					}
				}
			}
			// 条件指定がなし?
			if (isNoItem)
			{
				//「条件指定」
				var messageName = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_NOVELTY,
					Constants.VALUETEXT_PARAM_NOVELTY_REGISTER,
					Constants.VALUETEXT_PARAM_NOVELTY_REGISTER_CONDITION);
				errorMessage.Append(Validator.CheckNecessaryError(messageName, string.Empty));

			}

			// エラーメッセージをセット
			this.ErrorMessage = errorMessage.ToString();
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_SHOP_ID] = value; }
		}
		/// <summary>ノベルティID</summary>
		public string NoveltyId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_ID] = value; }
		}
		/// <summary>ノベルティ対象アイテム枝番</summary>
		public int NoveltyTargetItemNo
		{
			get { return (int)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_NO] = value; }
		}
		/// <summary>ノベルティ対象アイテム種別</summary>
		public string NoveltyTargetItemType
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_TYPE]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_TYPE] = value; }
		}
		/// <summary>ノベルティ対象アイテム値</summary>
		public string NoveltyTargetItemValue
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_VALUE]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_VALUE] = value; }
		}
		/// <summary>ノベルティ対象アイテム並び順</summary>
		public string NoveltyTargetItemTypeSortNo
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_TYPE_SORT_NO]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_TYPE_SORT_NO] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NOVELTYTARGETITEM_LAST_CHANGED] = value; }
		}
		/// <summary>全商品？</summary>
		public bool IsItemTypeAll
		{
			get { return (this.DataSource["IsItemTypeAll"] != null ? (bool)this.DataSource["IsItemTypeAll"] : false); }
			set { this.DataSource["IsItemTypeAll"] = value; }
		}
		/// <summary>ブランドID</summary>
		public string BrandId
		{
			get { return (string)this.DataSource[Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_BRAND]; }
			set { this.DataSource[Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_BRAND] = value; }
		}
		/// <summary>行数（ブランドID）</summary>
		public int BrandIdRows
		{
			get { return (int)this.DataSource["BrandIdRows"]; }
			set { this.DataSource["BrandIdRows"] = value; }
		}
		/// <summary>行幅がノーマル？（ブランドID）</summary>
		public bool IsBrandIdResizeNormal { get { return (this.BrandIdRows == TEXT_BOX_ROWS_NORMAL); } }
		/// <summary>カテゴリID</summary>
		public string CategoryId
		{
			get { return (string)this.DataSource[Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_CATEGORY]; }
			set { this.DataSource[Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_CATEGORY] = value; }
		}
		/// <summary>行数（カテゴリID）</summary>
		public int CategoryIdRows
		{
			get { return (int)this.DataSource["CategoryIdRows"]; }
			set { this.DataSource["CategoryIdRows"] = value; }
		}
		/// <summary>行幅がノーマル？（カテゴリID）</summary>
		public bool IsCategoryIdResizeNormal { get { return (this.CategoryIdRows == TEXT_BOX_ROWS_NORMAL); } }
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_PRODUCT]; }
			set { this.DataSource[Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_PRODUCT] = value; }
		}
		/// <summary>行数（商品ID）</summary>
		public int ProductIdRows
		{
			get { return (int)this.DataSource["ProductIdRows"]; }
			set { this.DataSource["ProductIdRows"] = value; }
		}
		/// <summary>行幅がノーマル？（商品ID）</summary>
		public bool IsProductIdResizeNormal { get { return (this.ProductIdRows == TEXT_BOX_ROWS_NORMAL); } }
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_VARIATION]; }
			set { this.DataSource[Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_VARIATION] = value; }
		}
		/// <summary>行数（バリエーションID）</summary>
		public int VariationIdRows
		{
			get { return (int)this.DataSource["VariationIdRows"]; }
			set { this.DataSource["VariationIdRows"] = value; }
		}
		/// <summary>行幅がノーマル？（バリエーションID）</summary>
		public bool IsVariationIdResizeNormal { get { return (this.VariationIdRows == TEXT_BOX_ROWS_NORMAL); } }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage
		{
			get { return StringUtility.ToEmpty(this.DataSource["ErrorMessage"]); }
			set { this.DataSource["ErrorMessage"] = value; }
		}
		#endregion
	}
	#endregion

	#region +ノベルティ付与条件入力クラス
	/// <summary>
	/// ノベルティ付与条件入力クラス
	/// </summary>
	public class NoveltyGrantConditionsInput
	{
		/// <summary>データソース</summary>
		Hashtable DataSource { get; set; }

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public NoveltyGrantConditionsInput()
		{
			this.DataSource = new Hashtable();
			// 全ユーザ
			this.UserRankId = Constants.FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBERRANK_ALL;
			// 行数初期化
			this.ProductIdRows = TEXT_BOX_ROWS_NORMAL;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public NoveltyGrantConditionsInput(NoveltyGrantConditionsModel model)
			: this()
		{
			this.ShopId = model.ShopId;
			this.NoveltyId = model.NoveltyId;
			this.ConditionNo = model.ConditionNo;
			this.UserRankId = model.UserRankId;
			this.AmountBegin = model.AmountBegin.ToString();
			this.AmountEnd = (model.AmountEnd != null) ? model.AmountEnd.ToString() : null;
			this.ProductId = string.Join(Environment.NewLine, model.GrantItemList.Select(item => item.ProductId));
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <param name="novelty">ノベルティ設定モデル</param>
		/// <returns>モデル</returns>
		public NoveltyGrantConditionsModel CreateModel(NoveltyModel novelty)
		{
			// 付与条件
			decimal? amountEnd = null;
			if (this.IsInputAmountEnd) amountEnd = decimal.Parse(this.AmountEnd);
			var model = new NoveltyGrantConditionsModel
			{
				ShopId = novelty.ShopId,
				NoveltyId = novelty.NoveltyId,
				ConditionNo = this.ConditionNo,
				UserRankId = this.UserRankId,
				AmountBegin = decimal.Parse(this.AmountBegin),
				AmountEnd = amountEnd,
				LastChanged = novelty.LastChanged
			};

			// 付与アイテム
			var grantItemList = new List<NoveltyGrantItemModel>();
			var sortNo = 0;
			foreach(var productId in this.ProductId.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
			{
				sortNo++;
				var grantItem = new NoveltyGrantItemModel
				{
					ShopId = model.ShopId,
					NoveltyId = model.NoveltyId,
					ConditionNo = model.ConditionNo,
					ProductId = productId,
					SortNo = sortNo,
					LastChanged = model.LastChanged
				};
				grantItemList.Add(grantItem);
			}
			model.GrantItemList = grantItemList.ToArray();

			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="grantConditionsList">付与条件リスト</param>
		/// <returns>エラーメッセージ</returns>
		public void Validate(NoveltyGrantConditionsInput[] grantConditionsList)
		{
			var errorMessage = new StringBuilder();

			// 付与条件入力チェック
			errorMessage.Append(Validator.Validate("Novelty", this.DataSource));

			// 対象金額大小エラーチェック
			if (errorMessage.Length == 0)
			{
				var amountBegin = decimal.Parse(this.AmountBegin);
				var amountEnd = (this.IsInputAmountEnd ? decimal.Parse(this.AmountEnd) : decimal.MaxValue);
				if (amountBegin > amountEnd)
				{
					errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOVELTYGRANTCONDITIONS_ITEM_AMOUNT_INPUT_ERROR));
				}
			}

			// 適用対象会員の対象金額範囲チェック
			if (errorMessage.Length == 0)
			{
				// 会員ランクIDでグループ化
				foreach (var grantConditionsListGroupByUserRandId in grantConditionsList.GroupBy(grantConditions => grantConditions.UserRankId))
				{
					// 有効な付与条件取得（※対象金額が有効）
					var grantConditionsListValid =
						grantConditionsListGroupByUserRandId.Where(grantConditions =>
							((Validator.IsHalfwidthDecimal(grantConditions.AmountBegin) && (string.IsNullOrEmpty(grantConditions.AmountBegin) == false))
							&& Validator.IsHalfwidthDecimal(grantConditions.IsInputAmountEnd ? grantConditions.AmountEnd : decimal.MaxValue.ToString())));

					foreach (var grantConditionsValid in grantConditionsListValid)
					{
						// カレント付与条件の対象金額を取得
						var amountBegin = decimal.Parse(grantConditionsValid.AmountBegin);
						var amountEnd = (grantConditionsValid.IsInputAmountEnd ? decimal.Parse(grantConditionsValid.AmountEnd) : decimal.MaxValue);
						// カレント付与条件の以外を取得
						var grantConditionsExcept = grantConditionsListValid.Where(grantConditions => (grantConditions.ConditionNo != grantConditionsValid.ConditionNo));
						// 対象金額範囲重複が存在する?
						grantConditionsExcept = grantConditionsExcept.Where(grantConditions =>
							((decimal.Parse(grantConditions.AmountBegin) <= amountBegin) && (amountBegin <= ((grantConditions.IsInputAmountEnd ? decimal.Parse(grantConditions.AmountEnd) : decimal.MaxValue))))
							|| ((decimal.Parse(grantConditions.AmountBegin) <= amountEnd) && (amountEnd <= ((grantConditions.IsInputAmountEnd ? decimal.Parse(grantConditions.AmountEnd) : decimal.MaxValue)))));
						if (grantConditionsExcept.Any())
						{
							errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOVELTYGRANTCONDITIONS_ITEM_USERRANK_AMOUNT_INPUT_ERROR).Replace("@@ 1 @@", grantConditionsValid.UserRankIdText));
							break;
						}
					}
				}
			}

			// 付与アイテム入力チェック
			var isNoItem = true;
			foreach (var value in this.ProductId.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
			{
				var inputItem = new Hashtable() { { "grant_item_" + Constants.FIELD_PRODUCT_PRODUCT_ID, value } };
				errorMessage.Append(Validator.Validate("Novelty", inputItem).Replace("@@ 1 @@", value));
				
				// 商品有効性チェック
				errorMessage.Append(CheckValidProduct(this.ShopId, value));

				// Check multi product variation
				if (this.IsCheckMultiVariation
					&& (ProductCommon.GetProductInfoUnuseMemberRankPrice(this.ShopId, value).Count > 1))
				{
					errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOVELTY_PRODUCT_VARIATION_MULTIPLE));
				}

				isNoItem = false;
			}
			// 指定なし?
			if (isNoItem)
			{
				var inputItem = new Hashtable() { { "grant_item_" + Constants.FIELD_PRODUCT_PRODUCT_ID + "_necessary", "" } };
				errorMessage.Append(Validator.Validate("Novelty", inputItem));
			}

			// エラーメッセージをセット
			this.ErrorMessage = errorMessage.ToString();
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_SHOP_ID] = value; }
		}
		/// <summary>ノベルティID</summary>
		public string NoveltyId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_NOVELTY_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_NOVELTY_ID] = value; }
		}
		/// <summary>ノベルティ付与条件枝番</summary>
		public int ConditionNo
		{
			get { return (int)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_CONDITION_NO]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_CONDITION_NO] = value; }
		}
		/// <summary>対象会員</summary>
		public string UserRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_USER_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_USER_RANK_ID] = value; }
		}
		/// <summary>対象金額（以上）</summary>
		public string AmountBegin
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_AMOUNT_BEGIN]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_AMOUNT_BEGIN] = value; }
		}
		/// <summary>対象金額（以下）</summary>
		public string AmountEnd
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_AMOUNT_END]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_AMOUNT_END] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_LAST_CHANGED] = value; }
		}
		/// <summary>対象金額（以下）指定あり？</summary>
		public bool IsInputAmountEnd { get { return (this.AmountEnd != string.Empty); } }
		/// <summary>対象会員テキスト</summary>
		public string UserRankIdText
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_USER_RANK_ID + "_text"]); }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_USER_RANK_ID + "_text"] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_PRODUCT_ID]); }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTITEM_PRODUCT_ID] = value; }
		}
		/// <summary>行数（商品ID）</summary>
		public int ProductIdRows
		{
			get { return (int)this.DataSource["ProductIdRows"]; }
			set { this.DataSource["ProductIdRows"] = value; }
		}
		/// <summary>行幅がノーマル？（商品ID）</summary>
		public bool IsProductIdResizeNormal { get { return (this.ProductIdRows == TEXT_BOX_ROWS_NORMAL); } }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage
		{
			get { return StringUtility.ToEmpty(this.DataSource["ErrorMessage"]); }
			set { this.DataSource["ErrorMessage"] = value; }
		}
		/// <summary>Has Check Multi Variation Grant Item</summary>
		public bool IsCheckMultiVariation { get; set; }
		#endregion
	}
	#endregion
}