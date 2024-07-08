/*
=========================================================================================================
  Module      : GetEntireOrderコマンドクラス(GetEntireOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System.Data;
using w2.App.Common.Util;
using w2.App.Common.Order;
using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Command.ApiCommand.EntireOrder
{
	#region Command
	/// <summary>
	/// GetEntireOrderコマンドクラス
	/// </summary>
	public class GetEntireOrder : ApiCommandBase
	{
		#region #Execute コマンド実行処理
		/// <summary>
		/// コマンド実行処理
		/// </summary>
		/// <param name="apiCommandArg">コマンド引数クラス</param>
		/// <returns>コマンド実行結果</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
			GetEntireOrderArg arg = (GetEntireOrderArg)apiCommandArg;

			// 注文情報を取得
			DataTable dataTable = GetEntireOrderData(arg.DataType, arg.ShopId, arg.OrderId);

			return new GetEntireOrderResult(EnumResultStatus.Complete, dataTable);
		}
		#endregion

		#region GetEntireOrder
		/// <summary>
		/// 注文情報取得
		/// </summary>
		/// <param name="orderDataType">取得する注文テーブル</param>
		/// <param name="shopId">ショップID</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文情報</returns>
		private DataTable GetEntireOrderData(OrderDataType orderDataType, string shopId, string orderId)
		{
			OrderCommon orderCommon = new OrderCommon();
			switch (orderDataType)
			{
				case OrderDataType.Order:
					return orderCommon.GetEntireOrderById(shopId, orderId);

				case OrderDataType.OrderOwner:
					return orderCommon.GetEntireOrderOwnerById(orderId);

				case OrderDataType.OrderShipping:
					return orderCommon.GetEntireOrderShippingById(orderId);

				case OrderDataType.OrderItem:
					return orderCommon.GetEntireOrderItemById(shopId, orderId);
			}
			return null;
		}
		#endregion
	}
	#endregion

	#region Arg
	/// <summary>
	/// GetEntireOrderコマンド用引数クラス
	/// </summary>
	public class GetEntireOrderArg : ApiCommandArgBase
	{
		#region プロパティ
		/// <summary>データタイプ</summary>
		public OrderDataType DataType;
		/// <summary>ショップID</summary>
		public string ShopId;
		/// <summary>注文ID</summary>
		public string OrderId;
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GetEntireOrderArg()
		{
			DataType = OrderDataType.Order;
			ShopId = null;
			OrderId = null;
		}
		#endregion
	}
	#endregion

	#region Result
	/// <summary>
	/// GetEntireOrderコマンド用実行結果クラス
	/// </summary>
	public class GetEntireOrderResult : ApiCommandResult
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="enumResultStatus"></param>
		/// <param name="dataTable"></param>
		public GetEntireOrderResult(EnumResultStatus enumResultStatus, DataTable dataTable)
			: base(enumResultStatus)
		{
			ResultTable = dataTable;
		}
		#endregion

		#region プロパティ

		public DataTable ResultTable { get; set; }

		#endregion
	}
	#endregion

	public enum OrderDataType
	{
		Order,
		OrderOwner,
		OrderShipping,
		OrderItem,
	}
}
