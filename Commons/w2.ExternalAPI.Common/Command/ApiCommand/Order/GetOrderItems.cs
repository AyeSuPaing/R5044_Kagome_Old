/*
=========================================================================================================
  Module      : GetOrderItemsコマンドクラス(GetOrderItems.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System.Data;
using w2.App.Common.Util;
using w2.App.Common.Order;
using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Order
{
	#region Command
	/// <summary>
	/// GetOrderItemsコマンドクラス
	/// </summary>
	public class GetOrderItems : ApiCommandBase 
	{
		#region #Execute コマンド実行処理
		/// <summary>
		/// コマンド実行処理
		/// </summary>
		/// <param name="apiCommandArg">コマンド引数クラス</param>
		/// <returns>コマンド実行結果</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
			GetOrderItemsArg arg = (GetOrderItemsArg)apiCommandArg;

			ApiLogger.Write(LogLevel.info, "コマンド引数情報:" + GetType().Name,
							string.Format("CreatedTimeSpan:'{0}',UpdatedTimeSpan:'{1}',OrderStatus:'{2}',OrderId:'{3}',OrderPaymentStatus:'{4}', OrderExtendedStatus:'{5}', ReturnExchangeKbn:'{6}'",
							(arg.CreatedTimeSpan == null) ? "Null" : arg.CreatedTimeSpan.ToString(),
							(arg.UpdatedTimeSpan == null) ? "Null" : arg.UpdatedTimeSpan.ToString(),
							arg.OrderStatus ?? "Null",
							arg.OrderId ?? "Null",
                            arg.OrderPaymentStatus ?? "Null",
							(arg.OrderExtendedStatusSpecification == null) ? "Null" : arg.OrderExtendedStatusSpecification.ToString(),
							arg.ReturnExchangeKbn ?? "Null"
							));

			// 作成日、更新日チェック
			arg.Validate(arg.CreatedTimeSpan, arg.UpdatedTimeSpan);

			OrderCommon orderCommon = new OrderCommon();
			// 注文情報を取得
			DataTable dataTable = orderCommon.GetOrderItems(arg.CreatedTimeSpan,
																arg.UpdatedTimeSpan,
																arg.OrderStatus,
																arg.OrderId,
                                                                arg.OrderPaymentStatus,
                                                                arg.OrderExtendedStatusSpecification.CreateSqlWhereClause(),
																arg.ReturnExchangeKbn);
			return new GetOrderItemsResult(EnumResultStatus.Complete, dataTable);
		}
		#endregion
	}
	#endregion

	#region Arg
	/// <summary>
	/// GetOrderItemsコマンド用引数クラス
	/// </summary>
	public class GetOrderItemsArg : ApiCommandArgBase
	{
		#region プロパティ
		/// <summary>注文情報作成日　過去における厳密期間</summary>
		public PastAbsoluteTimeSpan CreatedTimeSpan;
		/// <summary>注文情報更新日　過去における厳密期間</summary>
		public PastAbsoluteTimeSpan UpdatedTimeSpan;
		/// <summary>注文ステータス</summary>
		public string OrderStatus;
		/// <summary>注文ID</summary>
		public string OrderId;
        /// <summary>入金ステータス</summary>
        public string OrderPaymentStatus;
        /// <summary>Order Extended Status Specification</summary>
        public OrderExtendedStatusSpecification OrderExtendedStatusSpecification;
		/// <summary>Return Exchange Kbn</summary>
		public string ReturnExchangeKbn;
		#endregion
		
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GetOrderItemsArg()
        {
            CreatedTimeSpan = null;
            UpdatedTimeSpan = null;
            OrderStatus = "";
            OrderId = "";
            OrderPaymentStatus = "";
            OrderExtendedStatusSpecification = OrderExtendedStatusSpecification.GenByString(string.Empty);
			ReturnExchangeKbn = string.Empty;
        }
        #endregion
	}
	#endregion

	#region Result
	/// <summary>
	/// GetOrderItemsコマンド用実行結果クラス
	/// </summary>
	public class GetOrderItemsResult : ApiCommandResult
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="enumResultStatus"></param>
		/// <param name="dataTable"></param>
		public GetOrderItemsResult(EnumResultStatus enumResultStatus, DataTable dataTable) : base(enumResultStatus)
		{
			ResultTable = dataTable;
		}
		#endregion

		#region プロパティ

		public DataTable ResultTable { get; set; }

		#endregion
	}
	#endregion

}
