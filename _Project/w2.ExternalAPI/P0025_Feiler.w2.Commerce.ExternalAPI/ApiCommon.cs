/*
=========================================================================================================
  Module      : Feiler API Common Utilities (ApiCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Text.RegularExpressions;
using System.Data;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.App.Common.Util;

namespace P0025_Feiler.w2.Commerce.ExternalAPI
{
	public class ApiCommon
	{
		#region Get Wrapping Bag Type
		/// <summary>
		/// Get Wrapping Bag Type
		/// </summary>
		/// <param name="memo">Memo</param>
		/// <param name="mallId">Mall Id</param>
		/// <param name="giftFlag">Gift Flag</param>
		/// <returns>Wrapping Bag Type</returns>
		public static string GetWrappingBagType(string memo, string mallId, bool giftFlag)
		{
			string wrappingBagType = CustomConstants.FLG_ORDERSHIPPING_WRAPPING_BAG_TYPE_NONE;						// ０：無し	

			switch (mallId)
			{
				case CustomConstants.FLG_ORDER_MALL_ID_OWN_SITE:
					switch (GetMemoContent("包装方法", memo))
					{
						case "全ての商品を１つにまとめて包装":
							wrappingBagType = CustomConstants.FLG_ORDERSHIPPING_WRAPPING_BAG_TYPE_ALL;				// １：まとめて
							break;

						case "選択した商品のみ包装":
							wrappingBagType = CustomConstants.FLG_ORDERSHIPPING_WRAPPING_BAG_TYPE_INDIVIDUAL;		// ２：個別
							break;

						case "その他(お客様通信欄にご記入ください)":
							wrappingBagType = CustomConstants.FLG_ORDERSHIPPING_WRAPPING_BAG_TYPE_OTHERS;			// ３：その他	
							break;
					}
					break;

				case CustomConstants.FLG_ORDER_MALL_ID_YAHOO:
				case CustomConstants.FLG_ORDER_MALL_ID_RAKUTEN:
					wrappingBagType = (giftFlag)
													? CustomConstants.FLG_ORDERSHIPPING_WRAPPING_BAG_TYPE_OTHERS	// ３：その他
													: CustomConstants.FLG_ORDERSHIPPING_WRAPPING_BAG_TYPE_NONE;		// ０：無し
					break;
			}

			return wrappingBagType;
		}
		#endregion

		#region Get Order Item
		/// <summary>
		/// 注文商品情報取得
		/// </summary>
		/// <param name="orderId">Order ID</param>
		/// <param name="timeSpan">Time Span</param>
		/// <returns>Order Item List</returns>
		public static DataTable GetOrderItem(string orderId, PastAbsoluteTimeSpan timeSpan)
		{
			var cmd = new GetOrderItems();

			var getOrderItemsArg = new GetOrderItemsArg
			{
				CreatedTimeSpan = timeSpan,
				OrderId = orderId
			};

			// コマンド実行
			return ((GetOrderItemsResult)cmd.Do(getOrderItemsArg)).ResultTable;
		}
		#endregion

		#region Get Memo Content String
		/// <summary>
		/// Gets the content of the memo.
		/// </summary>
		/// <param name="header">The header.</param>
		/// <param name="memo">The memo.</param>
		/// <returns></returns>
		public static string GetMemoContent(string header, string memo)
		{
			var pattern = new Regex(string.Format(@"\[{0}\]\r\n([^\r\n]*)", header));
			string memoString = pattern.IsMatch(memo) ? pattern.Split(memo)[1] : string.Empty;

			var patternMessage = new Regex(@"((.|\n)*)\r\n(－－|\[)");
			return (patternMessage.IsMatch(memoString) ? patternMessage.Split(memoString)[1] : memoString).TrimEnd(new [] {'\r','\n'}).Replace("\r\n", "\\n");
		}

		/// <summary>
		/// Gets the content of the memo in section.
		/// </summary>
		/// <param name="header">The header.</param>
		/// <param name="footer">The footer.</param>
		/// <param name="memo">The memo.</param>
		/// <returns></returns>
		public static string GetMemoContentInSection(string header, string footer, string memo)
		{
			var startIndex = memo.IndexOf(header);
			if (startIndex < 0) { return string.Empty; }

			var length = memo.Substring(startIndex).IndexOf(footer);
			if (length < 0) { return string.Empty; }

			return memo.Substring(startIndex, length).Replace(header, string.Empty).Trim(Environment.NewLine.ToCharArray()); ;
		}
		#endregion
	}
}
