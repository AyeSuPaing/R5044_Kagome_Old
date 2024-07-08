/*
=========================================================================================================
  Module      : 注文カートユーザーコントロール（配送先定期系）(OrderCartUserControl_OrderShipping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.UI.WebControls;
using w2.App.Common.Web.Process;

/// <summary>
/// 注文カートユーザーコントロール（配送先定期系）
/// </summary>
public partial class OrderCartUserControl
{
	#region 配送情報（定期）入力画面系処理

	/// <summary>
	/// 定期購入設定作成
	/// </summary>
	protected void CreateFixedPurchaseSettings()
	{
		this.Process.CreateFixedPurchaseSettings();
	}

	/// <summary>
	/// 定期購入区分（1, 2, 3, 4）のデフォルトチェックの優先順位を設定
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="p1">順位1の区分（1 or 2 or 3 or 4）</param>
	/// <param name="p2">順位2の区分（1 or 2 or 3 or 4）</param>
	/// <param name="p3">順位3の区分（1 or 2 or 3 or 4）</param>
	/// <param name="p4">順位4の区分（1 or 2 or 3 or 4）</param>
	/// <returns>空文字列 ""</returns>
	protected string SetFixedPurchaseDefaultCheckPriority(int index, int p1, int p2, int p3, int p4 = 4)
	{
		return this.Process.SetFixedPurchaseDefaultCheckPriority(index, p1, p2, p3, p4);
	}

	/// <summary>
	/// 配送希望日のドロップダウン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlFixedPurchaseShippingDate_OnCheckedChanged(object sender, System.EventArgs e)
	{
		this.Process.ddlFixedPurchaseShippingDate_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// 配送パターン選択のラジオボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbFixedPurchaseShippingPattern_OnCheckedChanged(object sender, System.EventArgs e)
	{
		this.Process.rbFixedPurchaseShippingPattern_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// 配送パターン各アイテムのドロップダウン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlFixedPurchaseShippingPatternItem_OnCheckedChanged(object sender, System.EventArgs e)
	{
		this.Process.ddlFixedPurchaseShippingPatternItem_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// 次回配送日変更用ドロップダウンへのデータバインド完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlNextShippingDate_OnDataBound(object sender, System.EventArgs e)
	{
		this.Process.ddlNextShippingDate_OnDataBound(sender, e);
	}

	/// <summary>
	/// 指定した配送パターンの有効性を取得
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="fixedPurchaseKbn">定期配送区分</param>
	/// <param name="isInputArea">入力エリアか</param>
	/// <returns>配送パターンの有効性</returns>
	protected bool GetFixedPurchaseShippingPaternEnabled(
		int index,
		string fixedPurchaseKbn,
		bool isInputArea)
	{
		var visible = false;
		switch (fixedPurchaseKbn)
		{
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
				visible = (isInputArea
						? GetFixedPurchaseKbnInputChecked(index, 1)
						: GetFixedPurchaseKbnEnabled(index, 1))
					&& ((GetFixedPurchaseIntervalDropdown(index, true).Length > 1));
				break;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
				visible = (isInputArea
						? GetFixedPurchaseKbnInputChecked(index, 2)
						: GetFixedPurchaseKbnEnabled(index, 2))
					&& ((GetFixedPurchaseIntervalDropdown(index, true, true).Length > 1));
				break;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
				visible = (isInputArea
						? GetFixedPurchaseKbnInputChecked(index, 3)
						: GetFixedPurchaseKbnEnabled(index, 3))
					&& (Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG
						? (GetFixedPurchaseIntervalDropdown(index, false).Length > 0)
						: (GetFixedPurchaseIntervalDropdown(index, false).Length > 1));
				break;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
				visible = (isInputArea
						? GetFixedPurchaseKbnInputChecked(index, 4)
						: GetFixedPurchaseKbnEnabled(index, 4))
					&& ((GetFixedPurchaseEveryNWeekDropdown(index, true).Length > 1));
				break;
		}
		return visible;
	}

	/// <summary>
	/// 定期購入配送間隔日・月設定値ドロップダウンリスト作成
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="isIntervalMonths">配送間隔月か
	/// （TRUE：配送間隔月；FALSE：配送間隔日）</param>
	/// （TRUE：月間隔・週・曜日指定パターン；FALSE：月間隔日付指定パターン）<param name="isKbn2">区分2か
	/// </param>
	/// <param name="isDays">日付か</param>
	/// <returns>定期購入配送間隔日・月設定値ドロップダウンリスト</returns>
	protected ListItem[] GetFixedPurchaseIntervalDropdown(int index, bool isIntervalMonths, bool isKbn2 = false, bool isDays = false)
	{
		return this.Process.GetFixedPurchaseIntervalDropdown(index, isIntervalMonths, isKbn2, isDays);
	}

	/// <summary>
	/// 定期購入配送週間隔・曜日設定値ドロップダウンリスト作成
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="isIntervalWeek">配送週間隔か（TRUE：配送週間隔；FALSE：曜日）</param>
	/// <returns>定期購入配送週間隔・曜日設定値ドロップダウンリスト</returns>
	protected ListItem[] GetFixedPurchaseEveryNWeekDropdown(int index, bool isIntervalWeek)
	{
		var everyNWeekDropdown = this.Process.GetFixedPurchaseEveryNWeekDropdown(index, isIntervalWeek);
		return everyNWeekDropdown;
	}

	/// <summary>
	/// 定期購入区分有効判定
	/// </summary>
	/// <param name="iItemIndex">カートindex</param>
	/// <param name="iKbnNo">区分番号</param>
	/// <returns>有効判定</returns>
	protected bool GetFixedPurchaseKbnEnabled(int iItemIndex, int iKbnNo)
	{
		return this.Process.GetFixedPurchaseKbnEnabled(iItemIndex, iKbnNo);
	}

	/// <summary>
	/// 定期購入配送パターン表示フラグ
	/// </summary>
	/// <param name="repeaterItem">repeaterItem</param>
	/// <returns>定期購入配送パターンを表示するか</returns>
	protected bool DisplayFixedPurchaseShipping(RepeaterItem repeaterItem)
	{
		return this.Process.DisplayFixedPurchaseShipping(repeaterItem);
	}

	/// <summary>
	/// 定期購入判断
	/// </summary>
	/// <param name="repeaterItem">repeaterItem</param>
	/// <returns>定期購入を含むかどうか</returns>
	protected bool HasFixedPurchase(RepeaterItem repeaterItem)
	{
		return this.Process.HasFixedPurchase(repeaterItem);
	}

	/// <summary>
	/// 定期購入区分有効判定（全区分）
	/// </summary>
	/// <param name="itemIndex">カートindex</param>
	/// <returns>有効判定</returns>
	protected bool GetAllFixedPurchaseKbnEnabled(int itemIndex)
	{
		return this.Process.GetAllFixedPurchaseKbnEnabled(itemIndex);
	}

	/// <summary>
	/// 定期購入区分入力部分表示判定
	/// </summary>
	/// <param name="iKbnNo">区分番号</param>
	/// <param name="iItemIndex">カートindex</param>
	/// <returns>選択判定</returns>
	protected bool GetFixedPurchaseKbnInputChecked(int iItemIndex, int iKbnNo)
	{
		return this.Process.GetFixedPurchaseKbnInputChecked(iItemIndex, iKbnNo);
	}

	/// <summary>
	/// 定期購入区分入力部分表示判定
	/// </summary>
	/// <param name="iItemIndex">カートindex</param>
	/// <param name="strName">値名称</param>
	/// <returns>選択判定</returns>
	protected string GetFixedPurchaseSelectedValue(int iItemIndex, string strName)
	{
		return this.Process.GetFixedPurchaseSelectedValue(iItemIndex, strName);
	}

	/// <summary>
	/// First shipping date on data bound
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlFirstShippingDate_OnDataBound(object sender, System.EventArgs e)
	{
		this.Process.ddlFirstShippingDate_OnDataBound(sender, e);
	}

	/// <summary>
	/// First shipping date drop down list item selected
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlFirstShippingDate_ItemSelected(object sender, System.EventArgs e)
	{
		this.Process.ddlFirstShippingDate_ItemSelected(sender, e);
	}

	/// <summary>
	/// Next shipping date dropdown list item selected
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlNextShippingDate_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.Process.ddlNextShippingDate_OnSelectedIndexChanged(sender, e);
	}
	#endregion
}