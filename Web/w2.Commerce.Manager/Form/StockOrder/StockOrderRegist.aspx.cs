/*
=========================================================================================================
  Module      : 発注情報登録ページ処理(StockOrderRegist.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Web.UI.WebControls;

public partial class Form_StockOrder_StockOrderRegist : ProductPage
{
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
			// 発注日ドロップダウンセット
			ucOrderDate.SetDate(DateTime.Now);


			//------------------------------------------------------
			// 一覧取得・画面セット
			//------------------------------------------------------
			DataView dvResult = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("StockOrder", "GetProductStockForOrder"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, this.LoginOperatorShopId);

				dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			if (dvResult.Count != 0)
			{
				rInput.DataSource = dvResult;
				rInput.DataBind();
			}
			else
			{

				trListEditError.Visible = true;
			}
		}
	}

	/// <summary>
	/// 更新結果表示
	/// </summary>
	/// <param name="objResult">結果(null/true/false)</param>
	/// <returns>結果文字列</returns>
	protected string DisplayResult(object objResult)
	{
		string strResult = null;

		if (objResult == null)
		{
			strResult = "－";
		}
		else if ((bool)objResult == true)
		{
			strResult = "○";
		}
		else if ((bool)objResult == false)
		{
			strResult = "×";
		}

		return strResult;
	}

	/// <summary>
	/// 発注ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOrder_Click(object sender, EventArgs e)
	{
		Hashtable htInputOrder = new Hashtable();
		ArrayList alInputOrderItem = new ArrayList();
		
		//------------------------------------------------------
		// 入力チェック（発注情報）
		//------------------------------------------------------
		htInputOrder.Add(Constants.FIELD_STOCKORDER_SHOP_ID, this.LoginOperatorShopId);
		htInputOrder.Add(Constants.FIELD_STOCKORDER_RELATION_ID, tbRelationId.Text);
		if ((ucOrderDate.Year != "") && (ucOrderDate.Month != "") && (ucOrderDate.Day != ""))
		{
			htInputOrder.Add(Constants.FIELD_STOCKORDER_ORDER_DATE, ucOrderDate.DateString);
		}
		else
		{
			htInputOrder.Add(Constants.FIELD_STOCKORDER_ORDER_DATE, null);
		}
		htInputOrder.Add(Constants.FIELD_STOCKORDER_LAST_CHANGED, this.LoginOperatorName);
		htInputOrder.Add("items", "x");// 入力チェック用ダミー

		string strErrorMessages = Validator.Validate("StockOrder", htInputOrder);

		//------------------------------------------------------
		// 入力チェック（発注商品情報）
		//------------------------------------------------------
		if (strErrorMessages.Length == 0)
		{
			// デフォルト入力分
			foreach (RepeaterItem ri in rInput.Items)
			{
				Hashtable htInputOrderItem = (Hashtable)htInputOrder.Clone();
				htInputOrderItem.Add(Constants.FIELD_STOCKORDERITEM_PRODUCT_ID, ((HiddenField)ri.FindControl("hfProductId")).Value);
				htInputOrderItem.Add(Constants.FIELD_STOCKORDERITEM_VARIATION_ID, ((HiddenField)ri.FindControl("hfVariationId")).Value);
				htInputOrderItem.Add(Constants.FIELD_STOCKORDERITEM_ORDER_COUNT, ((TextBox)ri.FindControl("tbOrderCount")).Text);

				// 入力チェック
				strErrorMessages = Validator.Validate("StockOrderItem", htInputOrderItem);
				if (strErrorMessages != "")
				{
					break;
				}

				// 発注商品数0の場合は発注リストから除外する
				if (int.Parse((string)htInputOrderItem[Constants.FIELD_STOCKORDERITEM_ORDER_COUNT]) == 0)
				{
					continue;
				}

				// 格納
				alInputOrderItem.Add(htInputOrderItem);
			}
		}

		if (strErrorMessages.Length == 0)
		{
			// 追加入力分
			foreach (RepeaterItem ri in rInput2.Items)
			{
				if (((TextBox)ri.FindControl("tbProductId")).Text == "")
				{
					continue;
				}

				Hashtable htInputOrderItem = (Hashtable)htInputOrder.Clone();
				htInputOrderItem.Add(Constants.FIELD_STOCKORDERITEM_PRODUCT_ID, ((TextBox)ri.FindControl("tbProductId")).Text);
				htInputOrderItem.Add(Constants.FIELD_STOCKORDERITEM_VARIATION_ID, ((TextBox)ri.FindControl("tbProductId")).Text + ((TextBox)ri.FindControl("tbVId")).Text);
				htInputOrderItem.Add(Constants.FIELD_STOCKORDERITEM_ORDER_COUNT, ((TextBox)ri.FindControl("tbOrderCount")).Text);

				// 入力チェック
				strErrorMessages = Validator.Validate("StockOrderItem", htInputOrderItem);
				if (strErrorMessages != "")
				{
					break;
				}

				// 発注商品数0の場合は発注リストから除外する
				if (int.Parse((string)htInputOrderItem[Constants.FIELD_STOCKORDERITEM_ORDER_COUNT]) == 0)
				{
					continue;
				}

				// 商品存在チェック
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("Product", "GetProduct"))
				{
					DataView dvProduct = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInputOrderItem);
					bool blFind = false;
					foreach (DataRowView drv in dvProduct)
					{
						if ((string)htInputOrderItem[Constants.FIELD_STOCKORDERITEM_VARIATION_ID] == (string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])
						{
							blFind = true;
							break;
						}
					}
					if (blFind == false)
					{
						strErrorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_STOCKORDER_NOPRODUCT_ERROR);
						strErrorMessages = strErrorMessages.Replace("@@ 1 @@", (string)htInputOrderItem[Constants.FIELD_STOCKORDERITEM_PRODUCT_ID]);
						strErrorMessages = strErrorMessages.Replace("@@ 2 @@", (string)htInputOrderItem[Constants.FIELD_STOCKORDERITEM_VARIATION_ID]);
						break;
					}
				}

				// 重複チェック
				foreach (Hashtable htTemp in alInputOrderItem)
				{
					if (((string)htTemp[Constants.FIELD_STOCKORDERITEM_PRODUCT_ID] == (string)htInputOrderItem[Constants.FIELD_STOCKORDERITEM_PRODUCT_ID])
						&& ((string)htTemp[Constants.FIELD_STOCKORDERITEM_VARIATION_ID] == (string)htInputOrderItem[Constants.FIELD_STOCKORDERITEM_VARIATION_ID]))
					{
						strErrorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_STOCKORDER_DUPLICATION_ERROR);
						strErrorMessages = strErrorMessages.Replace("@@ 1 @@", (string)htInputOrderItem[Constants.FIELD_STOCKORDERITEM_PRODUCT_ID]);
						strErrorMessages = strErrorMessages.Replace("@@ 2 @@", (string)htInputOrderItem[Constants.FIELD_STOCKORDERITEM_VARIATION_ID]);
						break;
					}
				}

				// 格納
				alInputOrderItem.Add(htInputOrderItem);
			}
		}

		if (strErrorMessages.Length == 0)
		{
			// 商品数を格納しもう一度入力チェック
			htInputOrder[Constants.FIELD_STOCKORDER_ORDER_ITEM_COUNT] = alInputOrderItem.Count.ToString();
			htInputOrder["items"] = (alInputOrderItem.Count != 0) ? "x":"";

			strErrorMessages = Validator.Validate("StockOrder", htInputOrder);
		}

		if (strErrorMessages.Length != 0)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// データインサート
		//------------------------------------------------------
		// 発注ID発行
		string newStockOrderId = NumberingUtility.CreateKeyId(this.LoginOperatorShopId, Constants.NUMBER_KEY_STOCK_ORDER_ID, 10);

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				// 発注情報インサート
				using (SqlStatement sqlStatement = new SqlStatement("StockOrder", "InsertStockOrder"))
				{
					htInputOrder.Add(Constants.FIELD_STOCKORDER_STOCK_ORDER_ID, newStockOrderId);
					sqlStatement.ExecStatement(sqlAccessor, htInputOrder);
				}

				// 発注商品情報インサート
				using (SqlStatement sqlStatement = new SqlStatement("StockOrder", "InsertStockOrderItem"))
				{
					foreach (Hashtable htInputOrderItem in alInputOrderItem)
					{
						htInputOrderItem.Add(Constants.FIELD_STOCKORDER_STOCK_ORDER_ID, newStockOrderId);
						sqlStatement.ExecStatement(sqlAccessor, htInputOrderItem);
					}
				}

				// 論理在庫増加
				using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "AddProductStock"))
				{
					foreach (Hashtable htInputOrderItem in alInputOrderItem)
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_PRODUCTSTOCK_SHOP_ID, htInputOrderItem[Constants.FIELD_STOCKORDERITEM_SHOP_ID]);
						htInput.Add(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, htInputOrderItem[Constants.FIELD_STOCKORDERITEM_PRODUCT_ID]);
						htInput.Add(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, htInputOrderItem[Constants.FIELD_STOCKORDERITEM_VARIATION_ID]);
						htInput.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, htInputOrderItem[Constants.FIELD_STOCKORDERITEM_ORDER_COUNT]);
						htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK, 0);
						htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, 0);
						htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, 0);
						htInput.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, 0);
						htInput.Add(Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, htInputOrderItem[Constants.FIELD_STOCKORDERITEM_LAST_CHANGED]);

						int iUpdate = sqlStatement.ExecStatement(sqlAccessor, htInput);
						if (iUpdate == 0)
						{
							System.Text.StringBuilder sbError = new System.Text.StringBuilder();
							sbError.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_STOCK_UPDATE_FAIL));
							sbError.Append(" ").Append(Constants.FIELD_PRODUCTSTOCK_SHOP_ID).Append("=").Append(htInput[Constants.FIELD_PRODUCTSTOCK_SHOP_ID]);
							sbError.Append(" ").Append(Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID).Append("=").Append(htInput[Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID]);
							sbError.Append(" ").Append(Constants.FIELD_PRODUCTSTOCK_VARIATION_ID).Append("=").Append(htInput[Constants.FIELD_PRODUCTSTOCK_VARIATION_ID]);

							throw new ApplicationException(sbError.ToString());
						}
					}
				}
				
				// 在庫履歴追加
				using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "InsertProductStockHistory"))
				{
					foreach (Hashtable htInputOrderItem in alInputOrderItem)
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, "");
						htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, htInputOrderItem[Constants.FIELD_STOCKORDERITEM_SHOP_ID]);
						htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, htInputOrderItem[Constants.FIELD_STOCKORDERITEM_PRODUCT_ID]);
						htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, htInputOrderItem[Constants.FIELD_STOCKORDERITEM_VARIATION_ID]);
						htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_ORDER);
						htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, htInputOrderItem[Constants.FIELD_STOCKORDERITEM_ORDER_COUNT]);
						htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, 0);
						htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B, 0);
						htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C, 0);
						htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, 0);
						htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO, "");
						htInput.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED, htInputOrderItem[Constants.FIELD_STOCKORDERITEM_LAST_CHANGED]);

						sqlStatement.ExecStatement(sqlAccessor, htInput);
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

	/// <summary>
	/// 商品追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddItem_Click(object sender, EventArgs e)
	{
		ArrayList alInputs = new ArrayList();
		foreach (RepeaterItem ri in rInput2.Items)
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_STOCKORDERITEM_PRODUCT_ID, ((TextBox)ri.FindControl("tbProductId")).Text);
			htInput.Add(Constants.FIELD_PRODUCTVARIATION_V_ID, ((TextBox)ri.FindControl("tbVId")).Text);
			htInput.Add(Constants.FIELD_STOCKORDERITEM_ORDER_COUNT, ((TextBox)ri.FindControl("tbOrderCount")).Text);
			alInputs.Add(htInput);
		}
		alInputs.Add(new Hashtable());

		rInput2.DataSource = alInputs;
		rInput2.DataBind();

		if (rInput.Items.Count + rInput2.Items.Count != 0)
		{
			trListEditError.Visible = false;
		}
		else
		{
			trListEditError.Visible = true;
		}
	}
}
