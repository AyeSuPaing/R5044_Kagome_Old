/*
=========================================================================================================
  Module      : セットプロモーション情報登録ページ処理(SetPromotionRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Input;
using w2.App.Common.Option;
using w2.App.Common.RefreshFileManager;
using w2.Common.Extensions;
using w2.Common.Util;
using w2.Domain.SetPromotion;
using w2.App.Common.Util;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

public partial class Form_SetPromotion_SetPromotionRegister : SetPromotionPage
{
	// 対象商品入力テキストボックスのサイズ
	protected const int TEXT_BOX_ROWS_NORMAL = 5;
	protected const int TEXT_BOX_ROWS_LARGE = 20;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// アクションステータスチェック
			if (this.ActionStatus != Constants.ACTION_STATUS_UPDATE
				&& this.ActionStatus != Constants.ACTION_STATUS_COPY_INSERT
				&& this.ActionStatus != Constants.ACTION_STATUS_INSERT)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// コンポーネント初期化
			InitializeComponents();

			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_COPY_INSERT:
					// DBからデータを取得し画面にセット
					SetValues(GetSetPromotionInfo());
					break;

				case Constants.ACTION_STATUS_INSERT:
					// 空データを画面にセット
					var model = new SetPromotionModel
					{
						Items = new[] { new SetPromotionItemModel() }
					};
					SetValues(model);
					break;
			}
		}
		else
		{
			// 対象商品：セットプロモーションアイテム区分
			this.SetPromotionItemKbn = GetItemKbn();
		}
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnToList_Click(object sender, EventArgs e)
	{
		Hashtable listParameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_SETPROMOTION_SEARCH_INFO];
		Response.Redirect(CreateListUrl(listParameters, (int)listParameters[Constants.REQUEST_KEY_PAGE_NO]));
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateDetailUrl(Request[Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_ID], Constants.ACTION_STATUS_COPY_INSERT));
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		// DBから削除
		DeleteSetPromotion(Request[Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_ID]);

		// 各サイトの情報を最新状態にする
		RefreshFileManagerProvider.GetInstance(RefreshFileType.SetPromotion).CreateUpdateRefreshFile();

		// 一覧へリダイレクト
		Hashtable listParameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_SETPROMOTION_SEARCH_INFO];
		Response.Redirect(CreateListUrl(listParameters, (int)listParameters[Constants.REQUEST_KEY_PAGE_NO]));
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// 入力情報取得
		var inputData = GetInputData();

		// 入力チェック
		var errorMessages = inputData.Validate(true);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// DBに登録
		var model = inputData.CreateModel();
		InsertSetPromotion(model);

		// 各サイトの情報を最新状態にする
		RefreshFileManagerProvider.GetInstance(RefreshFileType.SetPromotion).CreateUpdateRefreshFile();

		// リダイレクト
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;
		Response.Redirect(CreateDetailUrl(model.SetpromotionId, Constants.ACTION_STATUS_UPDATE));
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		// 入力情報取得
		var inputData = GetInputData();

		// 入力チェック
		var errorMessages = inputData.Validate(false);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 更新
		var model = inputData.CreateModel();
		UpdateSetPromotion(model);

		// 各サイトの情報を最新状態にする
		RefreshFileManagerProvider.GetInstance(RefreshFileType.SetPromotion).CreateUpdateRefreshFile();

		// リダイレクト
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;
		Response.Redirect(CreateDetailUrl(model.SetpromotionId, Constants.ACTION_STATUS_UPDATE));
	}

	/// <summary>
	/// アイテム追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddItem_Click(object sender, EventArgs e)
	{
		// 入力中のアイテム情報取得
		var itemInputs = GetInputItemData().ToList();

		// アイテム追加
		itemInputs.Add(new SetPromotionItemInput());

		// データバインド
		SetItemInfo(itemInputs.ToArray());
	}

	/// <summary>
	/// 対象商品リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rItemList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName == "delete")
		{
			// 入力中のアイテム情報取得
			var itemInputs = GetInputItemData().ToList();

			// 該当のアイテム削除（アイテムが0になったら空のアイテムを追加）
			itemInputs.Remove(itemInputs[int.Parse(e.CommandArgument.ToString())]);
			if (itemInputs.Count == 0) itemInputs.Add(new SetPromotionItemInput());

			// データバインド
			SetItemInfo(itemInputs.ToArray());
		}
	}

	/// <summary>
	/// 商品割引フラグ変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbProductDiscountFlg_OnCheckedChanged(object sender, EventArgs e)
	{
		divProductDiscountSetting.Visible = cbProductDiscountFlg.Checked;
	}

	/// <summary>
	/// アイテム区分変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblItemKbn_SelectedIndexChanged(object sender, EventArgs e)
	{
		foreach (RepeaterItem ri in rItemList.Items)
		{
			// 検索ボタン、アイテム入力欄の表示制御
			string itemKbn = ((RadioButtonList)ri.FindControl("rblItemKbn")).SelectedValue;
			((System.Web.UI.HtmlControls.HtmlGenericControl)ri.FindControl("divInputItemArea")).Visible = (itemKbn != Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_ALL);
			((System.Web.UI.HtmlControls.HtmlGenericControl)ri.FindControl("spSearchProduct")).Visible = (itemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_PRODUCT);
			((System.Web.UI.HtmlControls.HtmlGenericControl)ri.FindControl("spSearchProductVariation")).Visible = (itemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION);
			((System.Web.UI.HtmlControls.HtmlGenericControl)ri.FindControl("spSearchCategory")).Visible = (itemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_CATEGORY);
		}
	}

	/// <summary>
	/// リサイズボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbResize_Click(object sender, EventArgs e)
	{
		// 対象コントロール取得
		int targetIndex = int.Parse(((LinkButton)sender).CommandArgument);
		TextBox tbItems = (TextBox)rItemList.Items[targetIndex].FindControl("tbItems");
		LinkButton lbResizeNormal = (LinkButton)rItemList.Items[targetIndex].FindControl("lbResizeNormal");
		LinkButton lbResizeLarge = (LinkButton)rItemList.Items[targetIndex].FindControl("lbResizeLarge");

		// 対象コントロールに値を設定
		int textBoxRows = ((tbItems.Rows == TEXT_BOX_ROWS_LARGE) ? TEXT_BOX_ROWS_NORMAL : TEXT_BOX_ROWS_LARGE);
		tbItems.Rows = textBoxRows;
		lbResizeNormal.Visible = (textBoxRows == TEXT_BOX_ROWS_LARGE);
		lbResizeLarge.Visible = (textBoxRows == TEXT_BOX_ROWS_NORMAL);
	}

	/// <summary>
	/// アイテムセットボタン(隠しボタン)クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>検索ポップアップで選択した商品/カテゴリをセットする</remarks>
	protected void lbSetItem_Click(object sender, EventArgs e)
	{
		// 対象レコードの区分と入力テキストボックスの値を取得
		int index = int.Parse(hfAddIndex.Value);
		string itemKbn = ((RadioButtonList)rItemList.Items[index].FindControl("rblItemKbn")).SelectedValue;
		TextBox tbItems = ((TextBox)rItemList.Items[int.Parse(hfAddIndex.Value)].FindControl("tbItems"));

		// 選択したアイテムを追加
		tbItems.Text += (tbItems.Text != "" ? "\r\n" : "");
		switch (itemKbn)
		{
			case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_PRODUCT:
				tbItems.Text += hfAddProductId.Value;
				break;

			case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION:
				tbItems.Text += hfAddProductId.Value + "," + hfAddVariationId.Value;
				break;

			case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_CATEGORY:
				tbItems.Text += hfAddCategoryId.Value;
				break;

			default:
				break;
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 表示制御
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				trRegistTitle.Visible = true;
				trEditTitle.Visible = false;
				trInputSetPromotionId.Visible = true;
				trDispSetPromotionId.Visible = false;
				btnInsertTop.Visible = btnInsertBottom.Visible = true;
				btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = false;
				btnDeleteTop.Visible = btnDeleteBottom.Visible = false;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = false;
				break;

			case Constants.ACTION_STATUS_UPDATE:
				trRegistTitle.Visible = false;
				trEditTitle.Visible = true;
				trInputSetPromotionId.Visible = false;
				trDispSetPromotionId.Visible = true;
				btnInsertTop.Visible = btnInsertBottom.Visible = false;
				btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = true;
				btnDeleteTop.Visible = btnDeleteBottom.Visible = true;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = true;
				break;
		}

		divComp.Visible = ((string)Session[Constants.SESSION_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COMPLETE);
		Session[Constants.SESSION_KEY_ACTION_STATUS] = null;

		// 商品割引設定
		divProductDiscountSetting.Visible = false;

		// 開始日時はデフォルト今日をセット
		ucDisplayPeriod.SetStartDate(DateTime.Today);

		// 会員ランク
		if (Constants.MEMBER_RANK_OPTION_ENABLED)
		{
			ddlTargetMemberRank.Items.Clear();
			ddlTargetMemberRank.Items.Add(new ListItem("", ""));
			ddlTargetMemberRank.Items.AddRange(MemberRankOptionUtility.GetMemberRankList()
				.Select(memberRank => new ListItem(memberRank.MemberRankName, memberRank.MemberRankId))
				.ToArray());
		}

		// 注文区分
		cblTargetOrderKbn.Items.Clear();
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN))
		{
			// モバイルデータの表示OPがOFFの場合は注文区分がモバイル注文を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& (li.Value == Constants.FLG_FIXEDPURCHASE_ORDER_KBN_MOBILE)) continue;
			li.Selected = true;
			cblTargetOrderKbn.Items.Add(li);
		}

		// 対象商品：セットプロモーションアイテム区分
		this.SetPromotionItemKbn = GetItemKbn();

		// 表示優先順にデフォルト「1」をセット
		tbDisplayOrder.Text = "1";
	}

	/// <summary>
	/// DBからセットプロモーション情報取得
	/// </summary>
	private SetPromotionModel GetSetPromotionInfo()
	{
		var model = new SetPromotionService().Get(Request[Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_ID]);
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
	/// <param name="model">セットプロモーションモデル</param>
	private void SetValues(SetPromotionModel model)
	{
		if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
		{
			// セットプロモーションID
			tbSetPromotionId.Text
				= lSetPromotionId.Text
				= model.SetpromotionId;

			// セットプロモーション名
			tbSetPromotionName.Text = model.SetpromotionName;
			tbSetPromotionDispName.Text = model.SetpromotionDispName;

			// プロモーション種別
			cbProductDiscountFlg.Checked = model.IsDiscountTypeProductDiscount;
			cbShippingChargeFreeFlg.Checked = model.IsDiscountTypeShippingChargeFree;
			cbPaymentChargeFreeFlg.Checked = model.IsDiscountTypePaymentChargeFree;

			// 適用優先順
			tbApplyOrder.Text = model.ApplyOrder.ToString();

			// セット価格
			divProductDiscountSetting.Visible = cbProductDiscountFlg.Checked;
			rbProductDiscountKbnDiscountedPrice.Checked = model.IsProductDiscountKbnDiscountedPrice;
			rbProductDiscountKbnDiscountPrice.Checked = model.IsProductDiscountKbnDiscountPrice;
			rbProductDiscountKbnDiscountRate.Checked = model.IsProductDiscountKbnDiscountRate;
			tbDiscountedPrice.Text = (model.IsProductDiscountKbnDiscountedPrice ? model.ProductDiscountSetting.ToPriceString() : "");
			tbDiscountPrice.Text = (model.IsProductDiscountKbnDiscountPrice ? model.ProductDiscountSetting.ToPriceString() : "");
			tbDiscountRate.Text = (model.IsProductDiscountKbnDiscountRate ? DecimalUtility.DecimalRound((model.ProductDiscountSetting ?? 0m), DecimalUtility.Format.RoundDown, 0).ToString() : "");

			// 表示文言
			rblDescriptionKbn.SelectedValue = model.DescriptionKbn;
			tbDescription.Text = model.Description;

			// 開始日時-終了日時
			if (string.IsNullOrEmpty(model.EndDate.ToString()))
			{
				ucDisplayPeriod.SetStartDate(model.BeginDate);
			}
			else
			{
				ucDisplayPeriod.SetPeriodDate(model.BeginDate, model.EndDate.Value);
			}

			// 適用会員ランク
			if (Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				ddlTargetMemberRank.SelectedValue = model.TargetMemberRank;
			}
			// 適用注文区分
			SetSearchCheckBoxValue(cblTargetOrderKbn, model.TargetOrderKbn.Split(','));
			// URL
			tbUrl.Text = model.Url;
			// 表示優先順
			tbDisplayOrder.Text = model.DisplayOrder.ToString();
			// 有効フラグ
			cbValidFlg.Checked = (model.ValidFlg == Constants.FLG_SETPROMOTION_VALID_FLG_VALID);

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var searchCondition = new NameTranslationSettingSearchCondition
				{
					DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION,
					MasterId1 = model.SetpromotionId,
					MasterId2 = string.Empty,
					MasterId3 = string.Empty,
				};
				this.SetPromotionTranslationData = new NameTranslationSettingService().GetTranslationSettingsByMasterId(searchCondition);
				DataBind();
			}
		}

		// 対象商品
		SetItemInfo(model.Items.Select(item => new SetPromotionItemInput(item)).ToArray());

		// ターゲットリストID
		if (model.TargetIds != null) tbTargetList.Text = string.Join("\n", model.TargetIds.Split(','));
	}

	/// <summary>
	/// 対象商品情報を画面にセット
	/// </summary>
	private void SetItemInfo(SetPromotionItemInput[] items)
	{
		rItemList.DataSource = items;
		rItemList.DataBind();

		foreach (RepeaterItem ri in rItemList.Items)
		{
			// アイテム区分によって対象商品入力欄の表示切替
			string itemKbn = items[ri.ItemIndex].SetpromotionItemKbn;
			((RadioButtonList)ri.FindControl("rblItemKbn")).SelectedValue = items[ri.ItemIndex].SetpromotionItemKbn;
			ri.FindControl("divInputItemArea").Visible = (itemKbn != Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_ALL);
			ri.FindControl("spSearchProduct").Visible = (itemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_PRODUCT);
			ri.FindControl("spSearchProductVariation").Visible = (itemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION);
			ri.FindControl("spSearchCategory").Visible = (itemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_CATEGORY);
			
			// テキストボックスのサイズ切り替え
			if (items[ri.ItemIndex].DataSource.Contains("textbox_rows"))
			{
				int textBoxRows = (int)items[ri.ItemIndex].DataSource["textbox_rows"];
				((TextBox)ri.FindControl("tbItems")).Rows = textBoxRows;
				ri.FindControl("lbResizeNormal").Visible = (textBoxRows == TEXT_BOX_ROWS_LARGE);
				ri.FindControl("lbResizeLarge").Visible = (textBoxRows == TEXT_BOX_ROWS_NORMAL);
			}
		}
	}

	/// <summary>
	/// 画面の入力情報を取得
	/// </summary>
	private SetPromotionInput GetInputData()
	{
		var input = new SetPromotionInput();
		input.SetpromotionId = StringUtility.ToHankaku(tbSetPromotionId.Text.Trim());

		// セットプロモーション名
		input.SetpromotionName = tbSetPromotionName.Text.Trim();
		input.SetpromotionDispName = tbSetPromotionDispName.Text.Trim();

		// プロモーション種別
		input.ProductDiscountFlg = cbProductDiscountFlg.Checked ? Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON : Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF;
		input.ShippingChargeFreeFlg = cbShippingChargeFreeFlg.Checked ? Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON : Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_OFF;
		input.PaymentChargeFreeFlg = cbPaymentChargeFreeFlg.Checked ? Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON : Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF;

		// 商品割引設定
		if (cbProductDiscountFlg.Checked)
		{
			input.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_KBN + "_check"]
				= (rbProductDiscountKbnDiscountedPrice.Checked
				|| rbProductDiscountKbnDiscountPrice.Checked
				|| rbProductDiscountKbnDiscountRate.Checked) ? "1" : "";	// 必須チェック用

			if (rbProductDiscountKbnDiscountedPrice.Checked)
			{
				input.ProductDiscountKbn = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNTED_PRICE;
				input.ProductDiscountSetting = StringUtility.ToHankaku(tbDiscountedPrice.Text.Trim());
				input.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_SETTING + "_discounted_price"] = StringUtility.ToHankaku(tbDiscountedPrice.Text.Trim());
			}
			else if (rbProductDiscountKbnDiscountPrice.Checked)
			{
				input.ProductDiscountKbn = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE;
				input.ProductDiscountSetting = StringUtility.ToHankaku(tbDiscountPrice.Text.Trim());
				input.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_SETTING + "_discount_price"] = StringUtility.ToHankaku(tbDiscountPrice.Text.Trim());
			}
			else if (rbProductDiscountKbnDiscountRate.Checked)
			{
				input.ProductDiscountKbn = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_RATE;
				input.ProductDiscountSetting = StringUtility.ToHankaku(tbDiscountRate.Text.Trim());
				input.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_SETTING + "_discount_rate"] = StringUtility.ToHankaku(tbDiscountRate.Text.Trim());
			}
		}
		else
		{
			input.ProductDiscountKbn = "";
			input.ProductDiscountSetting = null;
		}

		// 適用優先順
		if (Constants.SETPROMOTION_APPLY_ORDER_OPTION_ENABLED)
		{
			input.ApplyOrder = cbProductDiscountFlg.Checked 
				? StringUtility.ToHankaku(tbApplyOrder.Text.Trim())
				: Constants.FLG_SETPROMOTION_APPLY_ORDER_DEFAULT.ToString();
		}

		// 表示文言
		input.DescriptionKbn = rblDescriptionKbn.SelectedValue;
		input.Description = tbDescription.Text;

		// 開始日時
		input.BeginDate = ucDisplayPeriod.StartDateTimeString;

		// 終了日時
		if (string.IsNullOrEmpty(ucDisplayPeriod.EndDateTimeString) == false)
		{
			input.EndDate = ucDisplayPeriod.EndDateTimeString;
		}
		else
		{
			input.EndDate = null;
		}

		// 適用会員ランク
		input.TargetMemberRank = Constants.MEMBER_RANK_OPTION_ENABLED ? ddlTargetMemberRank.SelectedValue : "";
		// 適用注文区分
		input.TargetOrderKbn = CreateSearchStringParts(cblTargetOrderKbn.Items);
		// URL
		input.Url = StringUtility.ToHankaku(tbUrl.Text.Trim());
		// 表示優先順
		input.DisplayOrder = StringUtility.ToHankaku(tbDisplayOrder.Text.Trim());
		// 有効フラグ
		input.ValidFlg = cbValidFlg.Checked ? Constants.FLG_SETPROMOTION_VALID_FLG_VALID : Constants.FLG_SETPROMOTION_VALID_FLG_INVALID;

		input.LastChanged = this.LoginOperatorName;

		// セットプロモーション作成
		input.Items = GetInputItemData().ToArray();
		foreach (var item in input.Items)
		{
			item.SetpromotionId = input.SetpromotionId;
		}

		// ターゲットリストID
		if (tbTargetList.Text.Length > 0)
		{
			var targetIdList = tbTargetList.Text.Replace("\r\n","\n").Split('\n');
			input.TargetIds = string.Join(",", targetIdList);
		}
		else
		{
			input.TargetIds = string.Empty;
		}

		return input;
	}

	/// <summary>
	/// 画面のアイテム入力情報を取得
	/// </summary>
	private IEnumerable<SetPromotionItemInput> GetInputItemData()
	{
		foreach (RepeaterItem ri in rItemList.Items)
		{
			var itemInput = new SetPromotionItemInput();
			itemInput.SetpromotionItemNo = (ri.ItemIndex + 1).ToString();
			itemInput.SetpromotionItemKbn = ((RadioButtonList)ri.FindControl("rblItemKbn")).SelectedValue;
			itemInput.SetpromotionItems = ((TextBox)ri.FindControl("tbItems")).Text;
			if (itemInput.SetpromotionItemKbn != Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_ALL)
			{
				itemInput.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEMS + "_check"] = itemInput.SetpromotionItems;	// 必須チェック用
			}
			itemInput.SetpromotionItemQuantity = StringUtility.ToHankaku(((TextBox)ri.FindControl("tbItemCount")).Text.Trim());
			itemInput.DataSource["textbox_rows"] = ((TextBox)ri.FindControl("tbItems")).Rows;
			itemInput.SetpromotionItemQuantityMoreFlg = ((CheckBox)ri.FindControl("cbItemQuantityMoreFlg")).Checked
				? Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY_MORE_FLG_VALID
				: Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY_MORE_FLG_INVALID;
			yield return itemInput;
		}
	}

	/// <summary>
	/// DBに登録
	/// </summary>
	private void InsertSetPromotion(SetPromotionModel model)
	{
		new SetPromotionService().Insert(model);
	}

	/// <summary>
	/// DB更新
	/// </summary>
	private void UpdateSetPromotion(SetPromotionModel model)
	{
		new SetPromotionService().Update(model);
	}

	/// <summary>
	/// DBから削除
	/// </summary>
	private void DeleteSetPromotion(string setpromotionId)
	{
		new SetPromotionService().Delete(setpromotionId);
	}

	/// <summary>
	/// セットプロモーションアイテム区分の取得
	/// </summary>
	private ListItemCollection GetItemKbn()
	{
		var setPromotionItemKbn = new ListItemCollection();
		var itemKbnList = ValueText.GetValueItemList(Constants.TABLE_SETPROMOTION, Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN);
		foreach (ListItem li in itemKbnList)
		{
			if ((Constants.PRODUCT_CTEGORY_OPTION_ENABLE == false)
				&& (Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_CATEGORY == li.Value)) continue;

			setPromotionItemKbn.Add(li);
		}

		return setPromotionItemKbn;
	}

	/// <summary>適用優先順の項目が有効</summary>
	public bool IsApplyOrderSettingEnable
	{
		get { return (Constants.SETPROMOTION_APPLY_ORDER_OPTION_ENABLED && cbProductDiscountFlg.Checked); }
	}
	/// <summary>セットプロモーション翻訳設定情報</summary>
	protected NameTranslationSettingModel[] SetPromotionTranslationData
	{
		get { return (NameTranslationSettingModel[])ViewState["setpromotion_translation_data"]; }
		set { ViewState["setpromotion_translation_data"] = value; }
	}
	/// <summary>セットプロモーション商品区分</summary>
	protected ListItemCollection SetPromotionItemKbn { get; set; }

	#region +セットプロモーションマスタ入力クラス
	/// <summary>
	/// セットプロモーションマスタ入力クラス
	/// </summary>
	public class SetPromotionInput : InputBase<SetPromotionModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SetPromotionInput()
		{
			this.Items = new SetPromotionItemInput[0];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public SetPromotionInput(SetPromotionModel model)
			: this()
		{
			this.SetpromotionId = model.SetpromotionId;
			this.SetpromotionName = model.SetpromotionName;
			this.SetpromotionDispName = model.SetpromotionDispName;
			this.SetpromotionDispNameMobile = model.SetpromotionDispNameMobile;
			this.ProductDiscountFlg = model.ProductDiscountFlg;
			this.ShippingChargeFreeFlg = model.ShippingChargeFreeFlg;
			this.PaymentChargeFreeFlg = model.PaymentChargeFreeFlg;
			this.ProductDiscountKbn = model.ProductDiscountKbn;
			this.ProductDiscountSetting = (model.ProductDiscountSetting != null) ? model.ProductDiscountSetting.ToString() : null;
			this.DescriptionKbn = model.DescriptionKbn;
			this.Description = model.Description;
			this.DescriptionKbnMobile = model.DescriptionKbnMobile;
			this.DescriptionMobile = model.DescriptionMobile;
			this.BeginDate = model.BeginDate.ToString();
			this.EndDate = (model.EndDate != null) ? model.EndDate.ToString() : null;
			this.TargetMemberRank = model.TargetMemberRank;
			this.TargetOrderKbn = model.TargetOrderKbn;
			this.Url = model.Url;
			this.UrlMobile = model.UrlMobile;
			this.DisplayOrder = model.DisplayOrder.ToString();
			this.ValidFlg = model.ValidFlg;
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.LastChanged = model.LastChanged;
			this.Items = model.Items.Select(item => new SetPromotionItemInput(item)).ToArray();
			this.TargetIds = model.TargetIds;
			this.ApplyOrder = model.ApplyOrder.ToString();
		}
		#endregion
		
		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override SetPromotionModel CreateModel()
		{
			var model = new SetPromotionModel
			{
				SetpromotionId = this.SetpromotionId,
				SetpromotionName = this.SetpromotionName,
				SetpromotionDispName = this.SetpromotionDispName,
				SetpromotionDispNameMobile = this.SetpromotionDispNameMobile,
				ProductDiscountFlg = this.ProductDiscountFlg,
				ShippingChargeFreeFlg = this.ShippingChargeFreeFlg,
				PaymentChargeFreeFlg = this.PaymentChargeFreeFlg,
				ProductDiscountKbn = this.ProductDiscountKbn,
				ProductDiscountSetting = (this.ProductDiscountSetting != null) ? decimal.Parse(this.ProductDiscountSetting) : (decimal?)null,
				DescriptionKbn = this.DescriptionKbn,
				Description = this.Description,
				DescriptionKbnMobile = this.DescriptionKbnMobile,
				DescriptionMobile = this.DescriptionMobile,
				BeginDate = DateTime.Parse(this.BeginDate),
				EndDate = (this.EndDate != null) ? DateTime.Parse(this.EndDate) : (DateTime?)null,
				TargetMemberRank = this.TargetMemberRank,
				TargetOrderKbn = this.TargetOrderKbn,
				Url = this.Url,
				UrlMobile = this.UrlMobile,
				DisplayOrder = int.Parse(this.DisplayOrder),
				ValidFlg = this.ValidFlg,
				LastChanged = this.LastChanged,
				TargetIds = this.TargetIds,
				ApplyOrder = (Constants.SETPROMOTION_APPLY_ORDER_OPTION_ENABLED) ? int.Parse(this.ApplyOrder) : Constants.FLG_SETPROMOTION_APPLY_ORDER_DEFAULT,
			};
			model.Items = this.Items.Select(item => item.CreateModel()).ToArray();

			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="checkDuplication">重複チェックするか</param>
		/// <returns>チェック成功時はモデル、失敗時はエラーメッセージを返す</returns>
		public string Validate(bool checkDuplication)
		{
			var errorMessage = new StringBuilder();

			// セットプロモーション情報チェック
			errorMessage.Append(Validator.Validate("SetPromotion", this.DataSource));
			// 重複チェック
			if (checkDuplication && new SetPromotionService().Get(this.SetpromotionId) != null)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION)
					.Replace("@@ 1 @@", "セットプロモーションID"));
			}

			// プロモーション種別チェック
			if ((this.ProductDiscountFlg == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF)
				&& (this.ShippingChargeFreeFlg == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_OFF)
				&& (this.PaymentChargeFreeFlg == Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF))
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SETPROMOTION_PROMOTION_KBN_UNSELECTED));
			}
			// 適用注文区分チェック
			if (this.TargetOrderKbn == "")
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SETPROMOTION_ORDER_KBN_UNSELECTED));
			}

			// 対象商品情報チェック
			var itemErrorMessages = new StringBuilder();
			foreach (var itemValidator in this.Items)
			{
				itemErrorMessages.Append(itemValidator.Validate(true));
			}

			// 日付チェック
			if (string.IsNullOrEmpty(this.EndDate) == false && this.BeginDate.CompareTo(this.EndDate) == 1)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SETPROMOTION_DATE_ERROR));
			}

			// ここまでチェックOKだったら、マスタ存在チェックと商品の配送種別チェック
			if (itemErrorMessages.Length == 0) itemErrorMessages.Append(CheckItemMasterData());

			// エラー情報を返す
			if (itemErrorMessages.Length != 0)
			{
				errorMessage.Append(errorMessage.Length != 0 ? "<br />" : "");
				errorMessage.Append("【対象商品】<br />");
				errorMessage.Append(itemErrorMessages);
			}

			return errorMessage.ToString();
		}

		/// <summary>
		/// 各アイテムのマスタチェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>マスタ存在チェックと配送種別の同一チェック</remarks>
		private string CheckItemMasterData()
		{
			var errorMessage = new StringBuilder();
			var shippingTypeList = new Dictionary<string, List<KeyValuePair<string, string>>>();

			foreach (var itemValidator in this.Items)
			{
				// 区分が「全商品」だったら、アイテムをクリアして次へ
				if (itemValidator.SetpromotionItemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_ALL)
				{
					itemValidator.SetpromotionItems = "";
					break;
				}

				// マスタ情報取得
				var targetItemData = GetMasterInfo(itemValidator);

				//------------------------------------------------------
				// マスタ存在チェック（区分が商品ID、バリエーションIDの場合は配送種別の取得も）
				//------------------------------------------------------
				foreach (string item in itemValidator.ItemList)
				{
					Hashtable productData = targetItemData.ToHashtableList().Find(p => (string)p["item"] == item);
					if (productData != null)
					{
						if ((itemValidator.SetpromotionItemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_PRODUCT) || (itemValidator.SetpromotionItemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION))
						{
							// 配送種別を取得
							string shippingType = (string)productData[Constants.FIELD_PRODUCT_SHIPPING_TYPE];
							if (shippingTypeList.Keys.Contains(shippingType) == false)
							{
								shippingTypeList.Add(shippingType, new List<KeyValuePair<string, string>>());
							}
							shippingTypeList[shippingType].Add(new KeyValuePair<string, string>(itemValidator.SetpromotionItemNo.ToString(), item));

							// 商品有効性チェック
							if ((string)productData[Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
							{
								errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SETPROMOTION_PRODUCT_INVALID)
									.Replace("@@ 1 @@", itemValidator.SetpromotionItemNo.ToString()).Replace("@@ 2 @@", item.Split(',')[0]));
							}
						}
					}
					else
					{
						// 対象商品/カテゴリがなければエラーメッセージ追加
						if ((itemValidator.SetpromotionItemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_PRODUCT) || (itemValidator.SetpromotionItemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION))
						{
							errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SETPROMOTION_PRODUCT_UNFOUND).Replace("@@ 1 @@", itemValidator.SetpromotionItemNo.ToString()).Replace("@@ 2 @@", item));
						}
						else if (itemValidator.SetpromotionItemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_CATEGORY)
						{
							errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SETPROMOTION_PRODUCT_CATEGORY_UNFOUND).Replace("@@ 1 @@", itemValidator.SetpromotionItemNo.ToString()).Replace("@@ 2 @@", item));
						}
					}
				}
			}

			//------------------------------------------------------
			// 配送種別チェック
			//------------------------------------------------------
			if (shippingTypeList.Count > 1)
			{
				// 配送種別が複数あったらエラー
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SETPROMOTION_SHIPPING_TYPE_ERROR));
				// どの商品がどの配送種別に設定されているかをエラーメッセージに追加
				foreach (KeyValuePair<string, List<KeyValuePair<string, string>>> shippingTypeInfo in shippingTypeList)
				{
					errorMessage.Append("- ").Append(shippingTypeInfo.Key).Append(" ");
					errorMessage.Append(GetShopShipping(Constants.CONST_DEFAULT_SHOP_ID, shippingTypeInfo.Key)[0][Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME]).Append("<br />");
					shippingTypeInfo.Value.ForEach(item => errorMessage.Append("　　[ No.").Append(item.Key).Append(" ] ").Append(item.Value).Append("<br />"));
				}
			}

			return errorMessage.ToString();
		}

		/// <summary>
		/// マスタ情報取得
		/// </summary>
		/// <param name="itemInput"></param>
		/// <returns></returns>
		private DataView GetMasterInfo(SetPromotionItemInput itemInput)
		{
			string statementName = null;
			string whereTargetData = null;
			switch (itemInput.SetpromotionItemKbn)
			{
				case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_PRODUCT:
					statementName = "GetTargetProductData";
					whereTargetData = string.Join(",",
						itemInput.ItemList.Select(value => string.Format("'{0}'", value.Replace("'", "''"))));
					break;

				case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION:
					statementName = "GetTargetVariationData";
					whereTargetData = string.Join(" OR ",
						itemInput.ItemList.Select(value =>
							string.Format("(product_id = '{0}' AND variation_id = '{1}')",
								value.Split(',')[0].Replace("'", "''"),
								value.Split(',')[1].Replace("'", "''"))));
					break;

				case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_CATEGORY:
					statementName = "GetTargetCategoryData";
					whereTargetData = string.Join(",",
						itemInput.ItemList.Select(value => string.Format("'{0}'", value.Replace("'", "''"))));
					break;
			}

			DataView targetItemData = null;
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("SetPromotion", statementName))
			{
				// 各マスタの対象ID条件文を置換
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ where_target_data @@", whereTargetData);
				
				var ht = new Hashtable { { Constants.FIELD_PRODUCT_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID } };
				targetItemData = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, ht);
			}
			return targetItemData;
		}
		#endregion

		#region プロパティ
		/// <summary>セットプロモーションID</summary>
		public string SetpromotionId
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_ID]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_ID] = value; }
		}
		/// <summary>管理用セットプロモーション名</summary>
		public string SetpromotionName
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_NAME]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_NAME] = value; }
		}
		/// <summary>表示用セットプロモーション名</summary>
		public string SetpromotionDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME] = value; }
		}
		/// <summary>モバイル用表示用セットプロモーション名</summary>
		public string SetpromotionDispNameMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME_MOBILE]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME_MOBILE] = value; }
		}
		/// <summary>商品金額割引フラグ</summary>
		public string ProductDiscountFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_FLG]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_FLG] = value; }
		}
		/// <summary>配送料無料フラグ</summary>
		public string ShippingChargeFreeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG] = value; }
		}
		/// <summary>決済手数料無料フラグ</summary>
		public string PaymentChargeFreeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG] = value; }
		}
		/// <summary>商品割引区分</summary>
		public string ProductDiscountKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_KBN]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_KBN] = value; }
		}
		/// <summary>商品割引設定</summary>
		public string ProductDiscountSetting
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_SETTING]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_PRODUCT_DISCOUNT_SETTING] = value; }
		}
		/// <summary>表示用文言HTML区分</summary>
		public string DescriptionKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN] = value; }
		}
		/// <summary>表示用文言</summary>
		public string Description
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION] = value; }
		}
		/// <summary>モバイル表示用文言HTML区分</summary>
		public string DescriptionKbnMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN_MOBILE]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_KBN_MOBILE] = value; }
		}
		/// <summary>モバイル表示用文言</summary>
		public string DescriptionMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_MOBILE]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DESCRIPTION_MOBILE] = value; }
		}
		/// <summary>開始日時</summary>
		public string BeginDate
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_BEGIN_DATE]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_BEGIN_DATE] = value; }
		}
		/// <summary>終了日時</summary>
		public string EndDate
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_END_DATE]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_END_DATE] = value; }
		}
		/// <summary>適用会員ランク</summary>
		public string TargetMemberRank
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_TARGET_MEMBER_RANK]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_TARGET_MEMBER_RANK] = value; }
		}
		/// <summary>適用注文区分</summary>
		public string TargetOrderKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_TARGET_ORDER_KBN]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_TARGET_ORDER_KBN] = value; }
		}
		/// <summary>URL</summary>
		public string Url
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_URL]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_URL] = value; }
		}
		/// <summary>モバイルURL</summary>
		public string UrlMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_URL_MOBILE]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_URL_MOBILE] = value; }
		}
		/// <summary>表示優先順</summary>
		public string DisplayOrder
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_LAST_CHANGED] = value; }
		}
		/// <summary>セットプロモーションアイテム入力クラスリスト</summary>
		public SetPromotionItemInput[] Items
		{
			get { return (SetPromotionItemInput[])this.DataSource["Items"]; }
			set { this.DataSource["Items"] = value; }
		}
		/// <summary>ターゲットリスト</summary>
		public string TargetIds
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_TARGET_TARGET_IDS]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_TARGET_TARGET_IDS] = value; }
		}
		/// <summary>適用優先順 </summary>
		public string ApplyOrder
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTION_APPLY_ORDER]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTION_APPLY_ORDER] = value; }
		}
		#endregion
	}
	#endregion

	#region +セットプロモーション商品マスタ入力クラス
	/// <summary>
	/// セットプロモーション商品マスタ入力クラス
	/// </summary>
	public class SetPromotionItemInput : InputBase<SetPromotionItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SetPromotionItemInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public SetPromotionItemInput(SetPromotionItemModel model)
			: this()
		{
			this.SetpromotionId = model.SetpromotionId;
			this.SetpromotionItemNo = model.SetpromotionItemNo.ToString();
			this.SetpromotionItemKbn = model.SetpromotionItemKbn;
			this.SetpromotionItems = model.SetpromotionItems;
			this.SetpromotionItemQuantity = model.SetpromotionItemQuantity.ToString();
			this.SetpromotionItemQuantityMoreFlg = model.SetpromotionItemQuantityMoreFlg;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override SetPromotionItemModel CreateModel()
		{
			var model = new SetPromotionItemModel
			{
				SetpromotionId = this.SetpromotionId,
				SetpromotionItemNo = int.Parse(this.SetpromotionItemNo),
				SetpromotionItemKbn = this.SetpromotionItemKbn,
				SetpromotionItems = this.SetpromotionItems,
				SetpromotionItemQuantity = int.Parse(this.SetpromotionItemQuantity),
				SetpromotionItemQuantityMoreFlg = this.SetpromotionItemQuantityMoreFlg,
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate(bool checkDuplication)
		{
			// Validatorチェック
			var errorMessage = Validator.Validate("SetPromotionItem", this.DataSource).Replace("@@ 1 @@", this.SetpromotionItemNo.ToString());

			// アイテム区分がバリエーションの場合のみ入力形式チェック
			if ((this.SetpromotionItemKbn == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION)
				&& (this.ItemList.Any((s => (s != "") && (s.Split(',').Length != 2)))))
			{
				errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SETPROMOTION_VARIATION_INPUT_ERROR).Replace("@@ 1 @@", this.SetpromotionItemNo.ToString());
			}

			return errorMessage;
		}
		#endregion

		#region プロパティ
		/// <summary>セットプロモーションID</summary>
		public string SetpromotionId
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ID]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ID] = value; }
		}
		/// <summary>セットプロモーションアイテム枝番</summary>
		public string SetpromotionItemNo
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_NO] = value; }
		}
		/// <summary>セットプロモーションアイテム区分</summary>
		public string SetpromotionItemKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN] = value; }
		}
		/// <summary>対象商品</summary>
		public string SetpromotionItems
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEMS]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEMS] = value; }
		}
		/// <summary>数量</summary>
		public string SetpromotionItemQuantity
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY] = value; }
		}
		/// <summary>対象商品</summary>
		public string[] ItemList
		{
			get { return this.SetpromotionItems.Replace("\r\n", "\n").Split('\n').Where(s => s != "").ToArray(); }
		}
		/// <summary>数量以上フラグ</summary>
		public string SetpromotionItemQuantityMoreFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY_MORE_FLG]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY_MORE_FLG] = value; }
		}
		#endregion
	}
	#endregion
}