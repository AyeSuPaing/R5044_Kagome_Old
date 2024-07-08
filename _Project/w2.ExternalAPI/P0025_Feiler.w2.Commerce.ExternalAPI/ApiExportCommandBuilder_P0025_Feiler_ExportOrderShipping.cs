/*
=========================================================================================================
  Module      : Feiler 受注配送情報出力クラス(ApiExportCommandBuilder_P0025_Feiler_ExportOrderShipping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Export;
using System.Data;
using System.Text.RegularExpressions;

namespace P0025_Feiler.w2.Commerce.ExternalAPI
{
	public class ApiExportCommandBuilder_P0025_Feiler_ExportOrderShipping : ApiExportCommandBuilder
	{
		#region #Export 出力処理
		/// <summary>出力処理</summary>
		protected override object[] Export(IDataRecord record)
		{
			DataRowView orderData = OrderCommon.GetOrder(StringUtility.ToEmpty(record["OrderID"]))[0];

			DataTable orderItem = ApiCommon.GetOrderItem(record["OrderID"].ToString(), new PastAbsoluteTimeSpan(0, DateTime.Now.AddDays(-int.Parse(Properties["Timespan"])), DateTime.Now));

			return new object[]{
				record["OrderID"],
				record["ShippingNo"],
				orderData["owner_name1"],
				orderData["owner_name2"],
				record["SenderNameKana1"],
				record["SenderNameKana2"],
				record["SenderCompanyName"],
				record["SenderCompanyPostName"],
				record["SenderZip"],
				record["SenderAddr1"],
				record["SenderAddr2"],
				record["SenderAddr3"],
				record["SenderAddr4"],
				record["SenderTel"],
				record["ShippingName1"],
				record["ShippingName2"],
				record["ShippingNameKana1"],
				record["ShippingNameKana2"],
				record["ShippingCompanyName"],
				record["ShippingCompanyPostName"],
				record["ShippingZip"],
				record["ShippingAddr1"],
				record["ShippingAddr2"],
				record["ShippingAddr3"],
				record["ShippingAddr4"],
				record["ShippingTel"],
				record["ShippingCompany"],
				record["ShippingDate"],
				record["ShippingTime"],
				record["ShippingCheckNo"],
				GetWrappingPaperType(orderData),																					// Wrapping Paper Type
				GetWrappingPaperName(orderData),																					// Wrapping Paper Name
				ApiExportCommandBuilder_P0025_Feiler_XmlSetting.Instance.GetWrappingBagTypeSetting(orderData),								// Wrapping Bag Type From Setting
				ApiCommon.GetWrappingBagType(orderData["memo"].ToString(), 
											 orderData["mall_id"].ToString(), 
											 IsGift(orderData) || IsW2Gift(orderItem) ),	// Wrapping Bag Type
				GetMessageCardType(orderData["memo"].ToString(), orderData["mall_id"].ToString()),									// Message Card Type
				GetMessage(orderData["memo"].ToString(), orderData["mall_id"].ToString()),											// Message
				GetDisplaySenderName(orderData["memo"].ToString(), orderData["mall_id"].ToString())									// Display Sender Name
			};
		}
		#endregion

		#region #Init 初期化処理
		/// <summary>初期化処理</summary>
		public override DataTable GetDataTable()
		{
			//APIコマンド作る
			var cmd = new GetOrderShippings();

			var getOrderShippingsArg = new GetOrderShippingsArg
			{
				CreatedTimeSpan = new PastAbsoluteTimeSpan(0,
													DateTime.Now.AddDays(-int.Parse(Properties["Timespan"])),
													DateTime.Now),
				OrderPaymentStatus = (string.IsNullOrEmpty(Properties["PaymentStatus"]) || Properties["PaymentStatus"] == "true") ?
								"" : Properties["PaymentStatus"], // 入金ステータス
				OrderStatus = Properties["OrderStatus"], // 注文ステータス
				OrderExtendedStatusSpecification =  // 連携フラグがOFFかつ連携作業中フラグがON
					OrderExtendedStatusSpecification.GenByString(string.Format("({0}F & {1}T)", Properties["IntgFlag"], Properties["IntgWorkingFlag"]))
			};

			// コマンド実行
			var getOrderShippingsResult = (GetOrderShippingsResult)cmd.Do(getOrderShippingsArg);

			return getOrderShippingsResult.ResultTable;
		}
		#endregion

		#region Get Wrapping Paper Type
		/// <summary>
		/// Get Wrapping Paper Type
		/// </summary>
		/// <param name="record">Order Data</param>
		/// <returns>Wrapping Paper Type</returns>
		private string GetWrappingPaperType(DataRowView record)
		{
			switch (record["mall_id"].ToString())
			{
				case CustomConstants.FLG_ORDER_MALL_ID_OWN_SITE:
					string memoString = ApiCommon.GetMemoContent("のし紙", record["memo"].ToString());
					return GetWrappingTypeFromMemo(memoString);

				case CustomConstants.FLG_ORDER_MALL_ID_RAKUTEN:
					var patternRelationMemo = new Regex(@"\[のし\] ([^\r\n]*)");
					string relationMemoString = patternRelationMemo.IsMatch(record["relation_memo"].ToString()) ? patternRelationMemo.Split(record["relation_memo"].ToString())[1] : string.Empty;
					return GetWrappingTypeFromMemo(relationMemoString);

				default:
					return "00";
			}
		}

		/// <summary>
		/// Get Wrapping Type From Memo
		/// </summary>
		/// <param name="memo">Memo</param>
		/// <returns>Type</returns>
		private string GetWrappingTypeFromMemo(string memo)
		{
			switch (memo)
			{
				case "結婚祝【御祝】（紅白10本結び切り）":
					return "01";

				case "結婚祝お返し【内祝】（紅白10本結び切り）":
					return "02";

				case "一般の御祝【御祝】（紅白5本蝶結び）":
					return "03";

				case "一般の御祝お返し【内祝】（紅白5本蝶結び）":
					return "04";

				case "出産祝【御祝】（紅白5本蝶結び）":
					return "05";

				case "出産祝お返し【内祝】（紅白5本蝶結び）":
					return "06";

				case "病気見舞【御見舞】（紅白5本結び切り）":
					return "07";

				case "病気見舞お返し【快気祝】（紅白5本結び切り）":
					return "08";

				case "香典返し【志】（白黒5本結び切り）":
					return "09";

				case "その他（上記9つに当てはまらない場合）":
					return "10";

				default:
					return "00";
			}
		}
		#endregion

		#region Get Wrapping Paper Name
		/// <summary>
		/// Get Wrapping Paper Name
		/// </summary>
		/// <param name="record">Order Data</param>
		/// <returns>Wrapping Paper Name</returns>
		private string GetWrappingPaperName(DataRowView record)
		{
			string memo = record["memo"].ToString();

			switch (record["mall_id"].ToString())
			{
				case CustomConstants.FLG_ORDER_MALL_ID_OWN_SITE:
					switch (ApiCommon.GetMemoContent("のし名入れ", memo))
					{
						case "名入れなし":
							return string.Empty;

						case "氏のみ":
							return record["owner_name1"].ToString();

						case "氏名":
							return record["owner_name1"].ToString() + record["owner_name2"].ToString();

						case "その他":
							return ApiCommon.GetMemoContent("のし名入れ名称", memo);

						default:
							return string.Empty;
					}

				case CustomConstants.FLG_ORDER_MALL_ID_YAHOO:
					var pattern = new Regex(@"－－のし名入れ－－\r\n([^\r\n]*)");
					string memoYahoo = pattern.IsMatch(memo) ? pattern.Split(memo)[1] : string.Empty;
					var patternYahooName = new Regex(@"((.|\n)*)\r\n\[");
					return (patternYahooName.IsMatch(memoYahoo) ? patternYahooName.Split(memoYahoo)[1] : memoYahoo).TrimEnd(new[] { '\r', '\n' }).Replace("\r\n", "\\n");

				default:
					return string.Empty;
			}
		}
		#endregion

		#region Get Message Card Type
		/// <summary>
		/// Get Message Card Type
		/// </summary>
		/// <param name="memo">Memo</param>
		/// <param name="mallId">Mall Id</param>
		/// <returns>Message</returns>
		private string GetMessageCardType(string memo, string mallId)
		{
			return string.IsNullOrEmpty(GetMessage(memo, mallId)) ? "0" : "1";
		}
		#endregion

		#region Get Message
		/// <summary>
		/// Get Message
		/// </summary>
		/// <param name="memo">Memo</param>
		/// <param name="mallId">Mall Id</param>
		/// <returns>Message</returns>
		private string GetMessage(string memo, string mallId)
		{
			var pattern = new Regex(@"\[メッセージカード\]\r\n(.*)\r\n\[メッセージカード\]\r\n([^\[]*)");
			string memoString = (pattern.IsMatch(memo) && (mallId == CustomConstants.FLG_ORDER_MALL_ID_OWN_SITE)) ? pattern.Split(memo)[2] : string.Empty;

			var patternMessage = new Regex(@"((.|\n)*)\r\n\[");
			return (patternMessage.IsMatch(memoString) ? patternMessage.Split(memoString)[1] : memoString).Replace("\r\n", "\\n");
		}
		#endregion

		#region Get Display Sender Name
		/// <summary>
		/// Get Display Sender Name
		/// </summary>
		/// <param name="memo">Memo</param>
		/// <param name="mallId">Mall Id</param>
		/// <returns>Display Sender Name</returns>
		private string GetDisplaySenderName(string memo, string mallId)
		{
			var pattern = new Regex(@"\[送り主名\]\r\n([^\r\n]*)");
			string memoString = (pattern.IsMatch(memo) && (mallId == CustomConstants.FLG_ORDER_MALL_ID_OWN_SITE)) ? pattern.Split(memo)[1] : string.Empty;

			var patternMessage = new Regex(@"((.|\n)*)\r\n(－－|\[)");
			return (patternMessage.IsMatch(memoString) ? patternMessage.Split(memoString)[1] : memoString).TrimEnd(new[] { '\r', '\n' }).Replace("\r\n", "\\n");
		}
		#endregion


		/// <summary>
		/// ギフトかどうかを判定
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		private bool IsGift(DataRowView record)
		{
			switch (record["mall_id"].ToString())
			{
				case CustomConstants.FLG_ORDER_MALL_ID_RAKUTEN:
					return IsRakutenGift(record["relation_memo"].ToString());

				case CustomConstants.FLG_ORDER_MALL_ID_YAHOO:
					return IsYahooGift(record["regulation_memo"].ToString());

				default:
					return false;
			}
		}

		/// <summary>
		/// ｗ２のギフトかどうか
		/// </summary>
		/// <param name="orderItem"></param>
		/// <returns></returns>
		public bool IsW2Gift(DataTable orderItem)
		{
			return orderItem.Rows.Cast<DataRow>().Any(row => row["ProductID"].ToString().EndsWith(CustomConstants.FLG_SUFFIX_PRODUCT_ID));
		}

		/// <summary>
		/// 楽天ギフトかどうか
		/// </summary>
		/// <returns></returns>
		private bool IsRakutenGift(string apiMemo)
		{
			return apiMemo.Contains("[ラッピング]");
		}

		/// <summary>
		/// ヤフーのギフトかどうか
		/// </summary>
		/// <returns></returns>
		private bool IsYahooGift(string regulationMemo)
		{
			return regulationMemo.Contains("－－ギフト手数料－－");
		}
	}
}
