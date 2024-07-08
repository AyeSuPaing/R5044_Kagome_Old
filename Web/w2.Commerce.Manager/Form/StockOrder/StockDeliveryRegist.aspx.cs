/*
=========================================================================================================
  Module      : 入庫情報登録ページ処理(StockDeliveryRegist.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using w2.App.Common.Util;

public partial class Form_StockOrder_StockDeliveryRegist : ProductPage
{
	// データバインド用
	protected DataRowView m_drvStockOrder = null;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 初期表示？
		//------------------------------------------------------
		if (!IsPostBack)
		{
			string strPsOrderId = Request[Constants.REQUEST_KEY_STOCKORDER_STOCK_ORDER_ID];

			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_STOCKORDER_SHOP_ID, this.LoginOperatorShopId);
			htInput.Add(Constants.FIELD_STOCKORDER_STOCK_ORDER_ID, strPsOrderId);

			//------------------------------------------------------
			// 発注情報、発注商品情報
			//------------------------------------------------------
			DataView dvStockOrder = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("StockOrder", "GetStockOrder"))
			{
				dvStockOrder = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			DataView dvStockOrderItem = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("StockOrder", "GetStockOrderItem"))
			{
				dvStockOrderItem = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// 画面セット
			//------------------------------------------------------
			// 基本情報
			if (dvStockOrder.Count != 0)
			{
				m_drvStockOrder = dvStockOrder[0];
				DataBind();
			}

			// 商品情報
			rInput.DataSource = dvStockOrderItem;
			rInput.DataBind();

			//------------------------------------------------------
			// 入荷日ドロップダウンセット
			//------------------------------------------------------
			// 年
			ddlDeliveryDateYear.Items.Add("");
			ddlDeliveryDateYear.Items.AddRange(DateTimeUtility.GetBackwardYearListItem());
			// 月
			ddlDeliveryDateMonth.Items.Add("");
			ddlDeliveryDateMonth.Items.AddRange(DateTimeUtility.GetMonthListItem());
			// 日
			ddlDeliveryDateDay.Items.Add("");
			ddlDeliveryDateDay.Items.AddRange(DateTimeUtility.GetDayListItem());

			if (StringUtility.ToEmpty(m_drvStockOrder[Constants.FIELD_STOCKORDER_DELIVERY_DATE]) != "")
			{
				// 入荷日が格納されている場合はその日付をデフォルト選択
				DateTime dt = (DateTime)m_drvStockOrder[Constants.FIELD_STOCKORDER_DELIVERY_DATE];
				foreach (ListItem li in ddlDeliveryDateYear.Items)
				{
					li.Selected = (li.Value == dt.Year.ToString());
				}
				foreach (ListItem li in ddlDeliveryDateMonth.Items)
				{
					li.Selected = (li.Value == dt.Month.ToString());
				}
				foreach (ListItem li in ddlDeliveryDateDay.Items)
				{
					li.Selected = (li.Value == dt.Day.ToString());
				}
			}
			else
			{
				// 入荷日が格納されていない場合は今日の日付をデフォルト設定
				foreach (ListItem li in ddlDeliveryDateYear.Items)
				{
					li.Selected = (li.Value == DateTime.Now.Year.ToString());
				}
				foreach (ListItem li in ddlDeliveryDateMonth.Items)
				{
					li.Selected = (li.Value == DateTime.Now.Month.ToString());
				}
				foreach (ListItem li in ddlDeliveryDateDay.Items)
				{
					li.Selected = (li.Value == DateTime.Now.Day.ToString());
				}
			}
		}
	}

	/// <summary>
	/// 入庫登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOrder_Click(object sender, EventArgs e)
	{
		Hashtable htInputDelivery = new Hashtable();
		ArrayList alInputDeliveryItem = new ArrayList();

		//------------------------------------------------------
		// 入力チェック（発注情報）
		//------------------------------------------------------
		htInputDelivery.Add(Constants.FIELD_STOCKORDER_SHOP_ID, this.LoginOperatorShopId);
		htInputDelivery.Add(Constants.FIELD_STOCKORDER_STOCK_ORDER_ID, hfPsOrderId.Value);
		htInputDelivery.Add(Constants.FIELD_STOCKORDER_RELATION_ID, tbRelationId.Text);

		if ((ddlDeliveryDateYear.SelectedValue != "") && (ddlDeliveryDateMonth.SelectedValue != "") && (ddlDeliveryDateDay.SelectedValue != ""))
		{
			htInputDelivery.Add(Constants.FIELD_STOCKORDER_DELIVERY_DATE, ddlDeliveryDateYear.SelectedValue + "/" + ddlDeliveryDateMonth.SelectedValue + "/" + ddlDeliveryDateDay.SelectedValue);
		}
		else
		{
			htInputDelivery.Add(Constants.FIELD_STOCKORDER_DELIVERY_DATE, null);
		}
		htInputDelivery.Add(Constants.FIELD_STOCKORDER_LAST_CHANGED, this.LoginOperatorName);

		string strErrorMessages = Validator.Validate("StockDelivery", htInputDelivery);

		//------------------------------------------------------
		// 入力チェック（発注商品情報）
		//------------------------------------------------------
		int iDelivered = 0;
		int iDeliveredInPart = 0;
		if (strErrorMessages.Length == 0)
		{
			// デフォルト入力分
			foreach (RepeaterItem ri in rInput.Items)
			{
				Hashtable htInputDeliveryItem = (Hashtable)htInputDelivery.Clone();
				htInputDeliveryItem.Add(Constants.FIELD_STOCKORDERITEM_PRODUCT_ID, ((HiddenField)ri.FindControl("hfProductId")).Value);
				htInputDeliveryItem.Add(Constants.FIELD_STOCKORDERITEM_VARIATION_ID, ((HiddenField)ri.FindControl("hfVariationId")).Value);
				htInputDeliveryItem.Add(Constants.FIELD_STOCKORDERITEM_DELIVERY_COUNT, ((TextBox)ri.FindControl("tbDeliveryCount")).Text);
				htInputDeliveryItem.Add("delivery_count_before", ((HiddenField)ri.FindControl("hfDeliveryCountBefore")).Value);

				// 入力チェック
				strErrorMessages = Validator.Validate("StockDeliveryItem", htInputDeliveryItem);
				if (strErrorMessages != "")
				{
					break;
				}

				// 入荷数チェック
				int iOrderCount = int.Parse(((HiddenField)ri.FindControl("hfOrderCount")).Value);
				int iDeliveryCount = int.Parse(((TextBox)ri.FindControl("tbDeliveryCount")).Text);
				int iDeliveryCountBefore = int.Parse(((HiddenField)ri.FindControl("hfDeliveryCountBefore")).Value);
				int iRealstockUnreserved = int.Parse(((HiddenField)ri.FindControl("hfRealstockUnreserved")).Value);
				if (iDeliveryCount < 0)
				{
					// 負の数入力エラー(Validatorでチェック済み)
				}
				else if (iDeliveryCount > iOrderCount)
				{
					// 発注数より大きい数入力エラー
					strErrorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_STOCKDELIVERY_PRODUCTCOUNT_ERROR);
					break;
					
				}
				else if (iDeliveryCountBefore - iDeliveryCount > iRealstockUnreserved)
				{
					// 戻し可能な実在庫がないエラー
					strErrorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_STOCKDELIVERY_NOUNRESERVED_PRODUCTCOUNT_ERROR);
					break;
				}

				// ステータス設定
				if (iDeliveryCount == 0)
				{
					htInputDeliveryItem.Add(Constants.FIELD_STOCKORDERITEM_DELIVERY_STATUS, Constants.FLG_STOCKORDERITEM_DELIVERY_STATUS_UNDELIVERED);
				}
				else if (iDeliveryCount == iOrderCount)
				{
					htInputDeliveryItem.Add(Constants.FIELD_STOCKORDERITEM_DELIVERY_STATUS, Constants.FLG_STOCKORDERITEM_DELIVERY_STATUS_DELIVERED);
					iDelivered++;
				}
				else
				{
					htInputDeliveryItem.Add(Constants.FIELD_STOCKORDERITEM_DELIVERY_STATUS, Constants.FLG_STOCKORDERITEM_DELIVERY_STATUS_DELIVERED_INPART);
					iDeliveredInPart++;
				}

				// 格納
				alInputDeliveryItem.Add(htInputDeliveryItem);
			}
		}

		if (strErrorMessages.Length != 0)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// データ調整（ステータスなど）
		//------------------------------------------------------
		// 入庫ステータスセット
		if (iDelivered == alInputDeliveryItem.Count)
		{
			htInputDelivery.Add(Constants.FIELD_STOCKORDER_DELIVERY_STATUS, Constants.FLG_STOCKORDER_DELIVERY_STATUS_DELIVERED);
		}
		else if ((iDelivered == 0) && (iDeliveredInPart == 0))
		{
			htInputDelivery.Add(Constants.FIELD_STOCKORDER_DELIVERY_STATUS, Constants.FLG_STOCKORDER_DELIVERY_STATUS_UNDELIVERED);
		}
		else
		{
			htInputDelivery.Add(Constants.FIELD_STOCKORDER_DELIVERY_STATUS, Constants.FLG_STOCKORDER_DELIVERY_STATUS_DELIVERED_INPART);
		}

		// 入庫済み商品点数セット
		htInputDelivery.Add(Constants.FIELD_STOCKORDER_DELIVERY_ITEM_COUNT, iDelivered);

		//------------------------------------------------------
		// データアップデート
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				// 発注情報アップデート
				using (SqlStatement sqlStatement = new SqlStatement("StockOrder", "UpdateStockOrder"))
				{
					sqlStatement.ExecStatement(sqlAccessor, htInputDelivery);
				}

				// 発注商品情報アップデート
				using (SqlStatement sqlStatement = new SqlStatement("StockOrder", "UpdateStockOrderItem"))
				{
					foreach (Hashtable htInputDeliveryItem in alInputDeliveryItem)
					{
						// 数に更新があったもののみ更新
						if (htInputDeliveryItem["delivery_count_before"].ToString() != htInputDeliveryItem[Constants.FIELD_STOCKORDERITEM_DELIVERY_COUNT].ToString())
						{
							sqlStatement.ExecStatement(sqlAccessor, htInputDeliveryItem);
						}
					}
				}

				// 実在庫増加
				using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "AddProductStock"))
				{
					foreach (Hashtable htInputDeliveryItem in alInputDeliveryItem)
					{
						// 数に更新があったもののみ更新
						int iDeliveryCountAdd = int.Parse(htInputDeliveryItem[Constants.FIELD_STOCKORDERITEM_DELIVERY_COUNT].ToString()) - int.Parse(htInputDeliveryItem["delivery_count_before"].ToString());
						if (iDeliveryCountAdd != 0)
						{
							Hashtable htInput = new Hashtable();
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, htInputDeliveryItem[Constants.FIELD_STOCKORDERITEM_SHOP_ID]);
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, htInputDeliveryItem[Constants.FIELD_STOCKORDERITEM_PRODUCT_ID]);
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, htInputDeliveryItem[Constants.FIELD_STOCKORDERITEM_VARIATION_ID]);
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, 0);
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK, iDeliveryCountAdd);
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, 0);
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, 0);
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, 0);
							htInput.Add(Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, htInputDeliveryItem[Constants.FIELD_STOCKORDERITEM_LAST_CHANGED]);

							int iUpdate = sqlStatement.ExecStatement(sqlAccessor, htInput);
							if (iUpdate == 0)
							{
								System.Text.StringBuilder sbError = new System.Text.StringBuilder();
								sbError.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_STOCK_DELIVERY_UPDATE_FAIL));
								sbError.Append(" ").Append(Constants.FIELD_PRODUCTSTOCK_SHOP_ID).Append("=").Append(htInput[Constants.FIELD_PRODUCTSTOCK_SHOP_ID]);
								sbError.Append(" ").Append(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID).Append("=").Append(htInput[Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID]);
								sbError.Append(" ").Append(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID).Append("=").Append(htInput[Constants.FIELD_PRODUCTSTOCK_VARIATION_ID]);

								throw new ApplicationException(sbError.ToString());
							}
						}
					}
				}

				// 在庫履歴追加
				using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "InsertProductStockHistory"))
				{
					foreach (Hashtable htInputDeliveryItem in alInputDeliveryItem)
					{
						// 数に更新があったもののみ更新
						int iDeliveryCountAdd = int.Parse(htInputDeliveryItem[Constants.FIELD_STOCKORDERITEM_DELIVERY_COUNT].ToString()) - int.Parse(htInputDeliveryItem["delivery_count_before"].ToString());
						if (iDeliveryCountAdd != 0)
						{
							Hashtable htInput = new Hashtable();
							htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, "");
							htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, htInputDeliveryItem[Constants.FIELD_STOCKORDERITEM_SHOP_ID]);
							htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, htInputDeliveryItem[Constants.FIELD_STOCKORDERITEM_PRODUCT_ID]);
							htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, htInputDeliveryItem[Constants.FIELD_STOCKORDERITEM_VARIATION_ID]);
							htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_DELIVERED);
							htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, 0);
							htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, iDeliveryCountAdd);
							htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B, 0);
							htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C, 0);
							htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, 0);
							htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO, "");
							htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED, htInputDeliveryItem[Constants.FIELD_STOCKORDERITEM_LAST_CHANGED]);

							sqlStatement.ExecStatement(sqlAccessor, htInput);
						}
					}
				}
			}
			catch (ApplicationException ae)
			{
				// トランザクションロールバック
				sqlAccessor.RollbackTransaction();

				Session[Constants.SESSION_KEY_ERROR_MSG] = ae.Message;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
			catch (Exception)
			{
				// トランザクションロールバック
				sqlAccessor.RollbackTransaction();

				throw;
			}

			// トランザクションコミット
			sqlAccessor.CommitTransaction();
		}

		//------------------------------------------------------
		// とりあえず一覧画面へ戻る
		//------------------------------------------------------
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_STOCKORDER_LIST);
	}
	
}
