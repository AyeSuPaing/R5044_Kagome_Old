/*
=========================================================================================================
  Module      : 注文拡張ステータス設定ページ(OrderExtendStatusSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;

public partial class Form_OrderExtendStatusSetting_OrderExtendStatusSettingList : OrderPage
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
			// 拡張ステータス一覧取得
			//------------------------------------------------------
			DataView dvExtend = GetOrderExtendStatusSettingList(this.LoginOperatorShopId);

			// 拡張ステータス番号ソート用
			dvExtend.Sort = Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO;

			//------------------------------------------------------
			// データ作成
			//------------------------------------------------------
			List<string> lExtendSettings = new List<string>();
			for (int iLoop = 1; iLoop <= Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; iLoop++)
			{
				DataRowView[] drvs = dvExtend.FindRows(iLoop);
				Hashtable htExtendSetting = new Hashtable();
				string strExtendSetting;

				if (drvs.Length != 0)
				{
					// データベースより取得
					strExtendSetting = (string)drvs[0][Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME];
				}
				else
				{
					// 空で作成
					strExtendSetting = "";
				}

				lExtendSettings.Add(strExtendSetting);
			}

			// データバインド
			rExtendList.DataSource = lExtendSettings;
			rExtendList.DataBind();
		}
	}

	/// <summary>
	/// 一括更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAllUpdate_Click(object sender, EventArgs e)
	{
		// 入力チェック
		var errorMessages = new StringBuilder();

		// 拡張ステータスの数だけ
		foreach (RepeaterItem ri in rExtendList.Items)
		{
			var extendStatusNo = ri.ItemIndex + 1;
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME, ((TextBox)ri.FindControl("tbExtendName")).Text }
			};

			// メッセージ追加
			var error = Validator.Validate("OrderExtendStatusSetting", input);
			errorMessages.Append(error.Replace("@@ 1 @@", extendStatusNo.ToString()));
		}

		// エラー？
		if (errorMessages.Length != 0)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 全削除⇒追加で更新
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				// 拡張ステータス全削除
				using (var statement = new SqlStatement("OrderExtendStatusSetting", "DeleteOrderExtendStatusSettingList"))
				{
					var input = new Hashtable
					{
						{ Constants.FIELD_ORDEREXTENDSTATUSSETTING_SHOP_ID, this.LoginOperatorShopId }
					};
					statement.ExecStatement(accessor, input);
				}

				// 拡張ステータス追加
				foreach (RepeaterItem ri in rExtendList.Items)
				{
					var extendStatusName = ((TextBox)ri.FindControl("tbExtendName")).Text;
					var extendStatusNo = ri.ItemIndex + 1;

					// 拡張ステータス名称がない場合は追加しない
					if (extendStatusName.Length == 0) continue;

					// 更新
					using (var statement = new SqlStatement("OrderExtendStatusSetting", "InsertOrderExtendStatusSettingList"))
					{
						var input = new Hashtable
						{
							{ Constants.FIELD_ORDEREXTENDSTATUSSETTING_SHOP_ID, this.LoginOperatorShopId },
							{ Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO, extendStatusNo },
							{ Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME, extendStatusName },
							{ Constants.FIELD_ORDEREXTENDSTATUSSETTING_LAST_CHANGED, this.LoginOperatorName }
						};
						statement.ExecStatement(accessor, input);
					}
				}

				// トランザクションコミット
				accessor.CommitTransaction();
			}
			catch
			{
				// トランザクションロールバック
				accessor.RollbackTransaction();
				throw;
			}
		}
	}

	/// <summary>
	/// システム用（予約済）の注文拡張項目かどうか？
	/// </summary>
	/// <param name="extendStatusNo">拡張ステータス番号</param>
	/// <returns>システム用（予約済）である：true、システム用（予約済）ではない：false</returns>
	protected bool IsSystemReservedOrderExtendStatus(int extendStatusNo)
	{
		// 注文拡張ステータス31～40はシステム用（予約済み）
		var result = ((extendStatusNo >= 31) && (extendStatusNo <= 50));
		return result;
	}
}