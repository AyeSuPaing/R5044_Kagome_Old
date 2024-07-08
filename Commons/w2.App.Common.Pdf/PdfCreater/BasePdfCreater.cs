/*
=========================================================================================================
  Module      : PDF出力ベースクラス(BasePdfCreater.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.App.Common.Order;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.Pdf.PdfCreater
{
	/// <summary>
	/// PDF作成ベースクラス
	/// </summary>
	public class BasePdfCreater
	{
		// デフォルトファイル格納ディレクトリ名
		protected const string CONST_COMPOSITION_DEFAULT_DIRECTORY = "Default";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BasePdfCreater()
		{
			Initialize();
		}

		/// <summary>
		/// 注文情報取得
		/// </summary>
		/// <param name="strStatement">SQL Statement名</param>
		/// <param name="htParam">検索用パラメータ</param>
		/// <returns>注文情報一覧</returns>
		protected DataView GetOrderData(string strStatement, Hashtable htParam)
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("Order", strStatement))
			{
				// 「@@ where @@」置換
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ where @@", StringUtility.ToEmpty(htParam["@@ where @@"]));
				sqlStatement.CommandTimeout = this.SqlServerTimeout;

				var orderIdLikeEscapedParam =  htParam["replacement_order_id_like_escaped"] ?? htParam[Constants.FIELD_ORDER_ORDER_ID + "_like_escaped"];
				sqlStatement.ReplaceStatement(
					"@@ multi_order_id @@",
					StringUtility.ToEmpty(orderIdLikeEscapedParam).Replace("'", "''").Replace(",", "','"));

				// 「@@ orderby @@」置換
				sqlStatement.ReplaceStatement("@@ orderby @@", OrderCommon.GetOrderSearchOrderByStringForOrderItemListAndWorkflow(((string)htParam["sort_kbn"]), "Order", strStatement));
				sqlStatement.ReplaceStatement("@@ order_shipping_addr1 @@", StringUtility.ToEmpty(htParam[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]));

				// 「@@ replacement_order_id_like_escaped @@」置換
				sqlStatement.ReplaceStatement("@@ replacement_order_id_like_escaped @@", StringUtility.ToEmpty(htParam["replacement_order_id_like_escaped"]));

				sqlStatement.Statement = OrderExtendCommon.ReplaceOrderExtendFieldName(sqlStatement.Statement, Constants.TABLE_ORDER, StringUtility.ToEmpty(htParam[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]));

				var result = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htParam);
				return result;
			}
		}

		/// <summary>
		/// 付帯情報を変換
		/// </summary>
		/// <param name="optionTexts">付帯情報</param>
		/// <returns>変換後付帯情報</returns>
		public DataView ConvertProductOptionTexts(DataView optionTexts)
		{
			if ((Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false)
				|| (optionTexts.Table.Columns.Contains(Constants.FIELD_CART_PRODUCT_OPTION_TEXTS) == false)) return optionTexts;
			foreach (DataRowView drv in optionTexts)
			{
				drv[Constants.FIELD_CART_PRODUCT_OPTION_TEXTS] =
					ProductOptionSettingHelper.GetDisplayProductOptionTexts((string)drv[Constants.FIELD_CART_PRODUCT_OPTION_TEXTS]);
			}
			return optionTexts;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		private void Initialize()
		{
			this.SqlServerTimeout = 60;
		}

		/// <summary>
		/// ステートメント設定
		/// </summary>
		protected string GetStatementName(string strSearchKbn)
		{
			switch (strSearchKbn)
			{
				// 受注情報一覧
				case Constants.KBN_PDF_OUTPUT_ORDER:
					return "GetOrderItemPdf";

				// 注文ワークフロー一覧
				case Constants.KBN_PDF_OUTPUT_ORDER_WORKFLOW:
					return "GetOrderItemWorkflowPdf";

				default:
					// エラーページへ
					throw new ApplicationException("該当検索区分は存在しません");
			}
		}

		/// <summary>出力先ディレクトリパス</summary>
		public static string ExportDirPath
		{
			// HACK: パラメタ受け渡し時の一時ファイル生成に利用されている。リファクタリングで統合したい。
			get { return Constants.PHYSICALDIRPATH_COMMERCE_MANAGER + (Constants.PATH_CONTENTS + "Invoice/").Replace("/", @"\"); }
		}
		/// <summary>一時保存先パス</summary>
		public static string TempDirPath
		{
			// HACK: パラメタ受け渡し時の一時ファイル生成に利用されている。リファクタリングで統合したい。
			get { return ExportDirPath + @"Tmp\"; }
		}
		/// <summary>SQLServerのタイムアウトまでの設定：デフォルト60秒</summary>
		public int SqlServerTimeout { get; set; }
	}
}
