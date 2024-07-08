/*
=========================================================================================================
  Module      : 商品セット情報登録ページ処理(ProductSetRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;

public partial class Form_ProductSet_ProductSetRegister : ProductSetPage
{
	List<ProductSetItem> m_lProductSetItems = null;	// ジェネリックにしたいがViewStateから取り出すときにエラーになることがあるため

	public string m_strActionStatus = null;
	string m_strProductSetId = null;

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
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			m_strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState[Constants.REQUEST_KEY_ACTION_STATUS] = m_strActionStatus;

			m_strProductSetId = Request[Constants.REQUEST_KEY_PRODUCT_SET_ID];
			ViewState[Constants.REQUEST_KEY_PRODUCT_SET_ID] = m_strProductSetId;

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(m_strActionStatus);

			//------------------------------------------------------
			// 表示用値設定処理
			//------------------------------------------------------
			switch (m_strActionStatus)
			{
				case Constants.ACTION_STATUS_INSERT:

					// 商品セットアイテム初期化
					m_lProductSetItems = new List<ProductSetItem>();
					break;

				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_COPY_INSERT:

					// 商品セット情報取得
					Hashtable htProductSet = GetProductSet(m_strProductSetId);
					if (htProductSet.Count != 0)
					{
						lProductSetId.Text = WebSanitizer.HtmlEncode(htProductSet[Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID]);
						tbProductSetName.Text = (string)htProductSet[Constants.FIELD_PRODUCTSET_PRODUCT_SET_NAME];
						tbParentMax.Text = StringUtility.ToEmpty(htProductSet[Constants.FIELD_PRODUCTSET_PARENT_MAX]);
						tbParentMin.Text = StringUtility.ToEmpty(htProductSet[Constants.FIELD_PRODUCTSET_PARENT_MIN]);
						tbChildMax.Text = StringUtility.ToEmpty(htProductSet[Constants.FIELD_PRODUCTSET_CHILD_MAX]);
						tbChildMin.Text = StringUtility.ToEmpty(htProductSet[Constants.FIELD_PRODUCTSET_CHILD_MIN]);
						string strDescriptionKbn = StringUtility.ToEmpty(htProductSet[Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN]);
						foreach (ListItem li in rblDescriptionKbn.Items)
						{
							li.Selected = (li.Value == strDescriptionKbn);
						}
						tbDescription.Text = StringUtility.ToEmpty(htProductSet[Constants.FIELD_PRODUCTSET_DESCRIPTION]);
						tbMaxSellQuantity.Text = StringUtility.ToEmpty(htProductSet[Constants.FIELD_PRODUCTSET_MAX_SELL_QUANTITY]);
						cbValidFlg.Checked = (StringUtility.ToEmpty(htProductSet[Constants.FIELD_PRODUCTSET_VALID_FLG]) == Constants.FLG_PRODUCTSET_VALID_FLG_VALID);
					}
					else
					{
						// 該当データ無しの場合、エラーページへ
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}

					// 商品セットアイテム取得
					m_lProductSetItems = GetProductSetItem(m_strProductSetId);
					break;
			}

			// 商品リストデータバインド＆値セット（ビューステートも更新）
			PeoductListDataBind(m_lProductSetItems);
		}
		else
		{
			m_strActionStatus = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];
			m_strProductSetId = (string)ViewState[Constants.REQUEST_KEY_PRODUCT_SET_ID];
			m_lProductSetItems = (List<ProductSetItem>)ViewState["lProductSetItems"];
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		// 項目設定
		trMailtextId.Visible = (strActionStatus == Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// 商品一覧データバインド
	/// </summary>
	/// <param name="lProductSetItems"></param>
	private void PeoductListDataBind(List<ProductSetItem> lProductSetItems)
	{
		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		rProductList.DataSource = lProductSetItems;
		rProductList.DataBind();

		//------------------------------------------------------
		// 各値セット
		//------------------------------------------------------
		foreach (RepeaterItem ri in rProductList.Items)
		{
			ProductSetItem sp = lProductSetItems[ri.ItemIndex];
			((TextBox)ri.FindControl("tbProductSetItemPrice")).Text = sp.SetItemPrice.ToPriceString();
			((TextBox)ri.FindControl("tbCountMin")).Text = sp.CountMin;
			((TextBox)ri.FindControl("tbCountMax")).Text = sp.CountMax;
			foreach (ListItem li in ((RadioButtonList)ri.FindControl("rblFamilyFlg")).Items)
			{
				li.Selected = (sp.FamilyFlg == li.Value);
			}
		}

		if (lProductSetItems.Count == 0)
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
		else
		{
			trListError.Visible = false;
		}

		// 念のためビューステートにも設定
		ViewState["lProductSetItems"] = lProductSetItems;
	}

	/// <summary>
	/// 確認するボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 入力値セット
		//------------------------------------------------------
		Hashtable htInput = new Hashtable();
		htInput.Add(Constants.FIELD_PRODUCTSET_SHOP_ID, this.LoginOperatorShopId);
		htInput.Add(Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID, m_strProductSetId);
		htInput.Add(Constants.FIELD_PRODUCTSET_PRODUCT_SET_NAME, tbProductSetName.Text);
		htInput.Add(Constants.FIELD_PRODUCTSET_PARENT_MAX, (tbParentMax.Text != string.Empty) ? tbParentMax.Text : null);
		htInput.Add(Constants.FIELD_PRODUCTSET_PARENT_MIN, (tbParentMin.Text != string.Empty) ? tbParentMin.Text : null);
		htInput.Add(Constants.FIELD_PRODUCTSET_CHILD_MAX, (tbChildMax.Text != string.Empty) ? tbChildMax.Text : null);
		htInput.Add(Constants.FIELD_PRODUCTSET_CHILD_MIN, (tbChildMin.Text != string.Empty) ? tbChildMin.Text : null);
		htInput.Add(Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN, rblDescriptionKbn.SelectedValue);
		htInput.Add(Constants.FIELD_PRODUCTSET_DESCRIPTION, tbDescription.Text);
		htInput.Add(Constants.FIELD_PRODUCTSET_MAX_SELL_QUANTITY, tbMaxSellQuantity.Text);
		htInput.Add(Constants.FIELD_PRODUCTSET_VALID_FLG,
			(cbValidFlg.Checked ? Constants.FLG_PRODUCTSET_VALID_FLG_VALID : Constants.FLG_PRODUCTSET_VALID_FLG_INVALID));
		htInput.Add(Constants.FIELD_PRODUCTSET_LAST_CHANGED, this.LoginOperatorName);
		htInput.Add("item_count", (rProductList.Items.Count != 0) ? "1" : "");	// 入力チェック用

		// 商品一覧値セット
		List<Hashtable> lProductSetItems = new List<Hashtable>();
		foreach (RepeaterItem ri in rProductList.Items)
		{
			ProductSetItem sp = (ProductSetItem)m_lProductSetItems[ri.ItemIndex];
			sp.SetItemPrice = ((TextBox)ri.FindControl("tbProductSetItemPrice")).Text;
			sp.CountMin = (((TextBox)ri.FindControl("tbCountMin")).Text != string.Empty) ? ((TextBox)ri.FindControl("tbCountMin")).Text : null;
			sp.CountMax = (((TextBox)ri.FindControl("tbCountMax")).Text != string.Empty) ? ((TextBox)ri.FindControl("tbCountMax")).Text : null;
			sp.FamilyFlg = ((RadioButtonList)ri.FindControl("rblFamilyFlg")).SelectedValue;
			sp.DisplayOrder = ((TextBox)ri.FindControl("tbDisplayOrder")).Text;

			Hashtable htInputProduct = sp.GetHashtable();
			lProductSetItems.Add(htInputProduct);
		}

		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		StringBuilder sbErrorMessage = new StringBuilder();
		switch ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS])
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:

				// 新規登録用チェック
				sbErrorMessage.Append(Validator.Validate("ProductSetRegist", htInput));
				break;

			case Constants.ACTION_STATUS_UPDATE:

				// 編集用チェック
				sbErrorMessage.Append(Validator.Validate("ProductSetModify", htInput));
				break;
		}

		// セット商品アイテム用チェック
		foreach (Hashtable htInputProduct in lProductSetItems)
		{
			string strErrorMessages = Validator.Validate("ProductSetItemRegist", htInputProduct).Replace("@@ 1 @@", (string)htInputProduct["name"]);
			sbErrorMessage.Append(strErrorMessages);

			// 商品有効性チェック
			sbErrorMessage.Append(CheckValidProduct(Constants.CONST_DEFAULT_SHOP_ID, (string)htInputProduct[Constants.FIELD_PRODUCTSETITEM_PRODUCT_ID]));
		}

		//------------------------------------------------------
		// 整合性チェック
		//------------------------------------------------------
		// 範囲の整合性
		if (sbErrorMessage.Length == 0)
		{
			// 親商品数
			if ((htInput[Constants.FIELD_PRODUCTSET_PARENT_MAX] != null) && (htInput[Constants.FIELD_PRODUCTSET_PARENT_MIN] != null))
			{
				if (int.Parse((string)htInput[Constants.FIELD_PRODUCTSET_PARENT_MAX]) < int.Parse((string)htInput[Constants.FIELD_PRODUCTSET_PARENT_MIN]))
				{
					sbErrorMessage.Append(WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_NUMBER_OF_PARENT_PRODUCT_INCORRECT));
				}
			}
			// 子商品数
			if ((htInput[Constants.FIELD_PRODUCTSET_CHILD_MAX] != null) && (htInput[Constants.FIELD_PRODUCTSET_CHILD_MIN] != null))
			{
				if (int.Parse((string)htInput[Constants.FIELD_PRODUCTSET_CHILD_MAX]) < int.Parse((string)htInput[Constants.FIELD_PRODUCTSET_CHILD_MIN]))
				{
					sbErrorMessage.Append(WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_SIZE_OF_CHILD_PRODUCT_INCORRECT));
				}
			}

			// セット商品数
			foreach (Hashtable htInputProduct in lProductSetItems)
			{
				if ((htInputProduct[Constants.FIELD_PRODUCTSETITEM_COUNT_MAX] != null) && (htInputProduct[Constants.FIELD_PRODUCTSETITEM_COUNT_MIN] != null))
				{
					if (int.Parse((string)htInputProduct[Constants.FIELD_PRODUCTSETITEM_COUNT_MAX]) < int.Parse((string)htInputProduct[Constants.FIELD_PRODUCTSETITEM_COUNT_MIN]))
					{
						sbErrorMessage.Append(WebMessages.GetMessages(
							WebMessages.ERRMSG_MANAGER_SIZE_AND_NUMBER_OF_PRODUCT_INCORRECT)
								.Replace("@@ 1 @@", StringUtility.ToEmpty(htInputProduct["name"])));
					}
				}
			}
		}

		// 配送種別チェック
		if (sbErrorMessage.Length == 0)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Product", "GetProduct"))
			{
				sqlAccessor.OpenConnection();

				Hashtable htInput2 = new Hashtable();
				htInput2.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);

				string strShippingTypeCurrent = "";
				foreach (RepeaterItem ri in rProductList.Items)
				{
					ProductSetItem sp = (ProductSetItem)m_lProductSetItems[ri.ItemIndex];
					htInput2[Constants.FIELD_PRODUCT_PRODUCT_ID] = sp.ProductId;

					DataView dvProduct = sqlStatement.SelectSingleStatement(sqlAccessor, htInput2);
					if (dvProduct.Count != 0)
					{
						if (strShippingTypeCurrent.Length == 0)
						{
							strShippingTypeCurrent = (string)dvProduct[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE];
						}
						else
						{
							if (strShippingTypeCurrent != (string)dvProduct[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE])
							{
								sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTSET_SHIPPING_TYPE_DIFFERNENT).Replace("@@ 1 @@", sp.Name));
								break;
							}
						}
					}
				}
			}
		}

		//------------------------------------------------------
		// エラーページへ遷移？
		//------------------------------------------------------
		if (sbErrorMessage.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessage.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// 入力チェックＯＫだったらセットアイテムを表示順に並び替える
		//------------------------------------------------------
		List<ProductSetItem> lProductSetItem = new List<ProductSetItem>();
		foreach (ProductSetItem psiInsert in m_lProductSetItems)
		{
			int iDisplayOrder = int.Parse(psiInsert.DisplayOrder);

			int iIndex = 0;
			foreach (ProductSetItem psi in lProductSetItem)
			{
				if (iDisplayOrder < int.Parse(psi.DisplayOrder))
				{
					break;
				}
				else if (iDisplayOrder == int.Parse(psi.DisplayOrder))
				{
					if ((psiInsert.ProductId + psiInsert.VId).CompareTo(psi.ProductId + psi.VId) < 0)
					{
						break;
					}
				}
				iIndex++;
			}
			lProductSetItem.Insert(iIndex, psiInsert);
		}

		//------------------------------------------------------
		// 確認画面へ
		//------------------------------------------------------
		// セッションへパラメタセット
		Session[Constants.SESSION_KEY_PARAM] = htInput;
		Session[Constants.SESSION_KEY_PARAM2] = lProductSetItem;

		// 画面遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSET_CONFIRM + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + m_strActionStatus);
	}

	/// <summary>
	/// 商品追加
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAddProduct_Click(object sender, EventArgs e)
	{
		// すでにセットされていたら抜ける
		foreach (ProductSetItem sp in m_lProductSetItems)
		{
			if ((sp.ProductId == hfAddProductId.Value) && (sp.VId == hfAddVId.Value))
			{
				return;
			}
		}

		// 入力データ更新
		foreach (RepeaterItem ri in rProductList.Items)
		{
			ProductSetItem sp = (ProductSetItem)m_lProductSetItems[ri.ItemIndex];
			sp.SetItemPrice = ((TextBox)ri.FindControl("tbProductSetItemPrice")).Text;
			sp.CountMin = ((TextBox)ri.FindControl("tbCountMin")).Text;
			sp.CountMax = ((TextBox)ri.FindControl("tbCountMax")).Text;
			sp.FamilyFlg = ((RadioButtonList)ri.FindControl("rblFamilyFlg")).SelectedValue;
			sp.DisplayOrder = ((TextBox)ri.FindControl("tbDisplayOrder")).Text;
		}

		// 追加
		m_lProductSetItems.Add(new ProductSetItem(hfAddProductId.Value, hfAddVId.Value, hfAddProductName.Value, hfAddProductPrice.Value, "", "", "", "", ""));

		// 商品リストデータバインド＆値セット（ビューステートも更新）
		PeoductListDataBind(m_lProductSetItems);
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		List<ProductSetItem> lNewProductSetItems = new List<ProductSetItem>();

		// 新しいリスト作成
		foreach (RepeaterItem ri in rProductList.Items)
		{
			ProductSetItem sp = (ProductSetItem)m_lProductSetItems[ri.ItemIndex];
			if (((CheckBox)ri.FindControl("cbProductSelect")).Checked == false)
			{
				sp.SetItemPrice = ((TextBox)ri.FindControl("tbProductSetItemPrice")).Text;
				sp.CountMax = ((TextBox)ri.FindControl("tbCountMax")).Text;
				sp.CountMin = ((TextBox)ri.FindControl("tbCountMin")).Text;
				sp.FamilyFlg = ((RadioButtonList)ri.FindControl("rblFamilyFlg")).SelectedValue;
				sp.DisplayOrder = ((TextBox)ri.FindControl("tbDisplayOrder")).Text;

				lNewProductSetItems.Add(sp);
			}
		}

		if (m_lProductSetItems.Count != lNewProductSetItems.Count)
		{
			// 商品リストデータバインド＆値セット（ビューステートも更新）
			PeoductListDataBind(lNewProductSetItems);
		}
	}
}
