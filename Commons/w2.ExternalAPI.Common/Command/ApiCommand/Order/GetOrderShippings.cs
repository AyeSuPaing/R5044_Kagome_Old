/*
=========================================================================================================
  Module      : GetOrderShippingsコマンドクラス(GetOrderShippings.cs)
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
	/// GetOrderShippingsコマンドクラス
	/// </summary>
	public class GetOrderShippings : ApiCommandBase 
	{	
		#region #Execute コマンド実行処理
		/// <summary>
		/// コマンド実行処理
		/// </summary>
		/// <param name="apiCommandArg">コマンド引数クラス</param>
		/// <returns>コマンド実行結果</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
			GetOrderShippingsArg arg = (GetOrderShippingsArg) apiCommandArg;

			ApiLogger.Write(LogLevel.info, "コマンド引数情報:" + GetType().Name,
							string.Format("CreatedTimeSpan:'{0}',UpdatedTimeSpan:'{1}',OrderStatus:'{2}',OrderId:'{3}',OrderPaymentStatus:'{4}', OrderExtendedStatus:'{5}'",
							(arg.CreatedTimeSpan == null) ? "Null" : arg.CreatedTimeSpan.ToString(),
							(arg.UpdatedTimeSpan == null) ? "Null" : arg.UpdatedTimeSpan.ToString(),
										  arg.OrderStatus ?? "Null",
										  arg.OrderId ?? "Null",
                                          arg.OrderPaymentStatus ?? "Null",
										  (arg.OrderExtendedStatusSpecification == null) ? "Null" : arg.OrderExtendedStatusSpecification.ToString()
								));

			// 作成日、更新日チェック
			arg.Validate(arg.CreatedTimeSpan, arg.UpdatedTimeSpan);

			OrderCommon orderCommon = new OrderCommon();
			// 注文情報を取得
			DataTable dataTable = orderCommon.GetOrderShippings(arg.CreatedTimeSpan,
																	arg.UpdatedTimeSpan,
																	arg.OrderStatus,
																	arg.OrderId,
                                                                    arg.OrderPaymentStatus,
                                                                    arg.OrderExtendedStatusSpecification.CreateSqlWhereClause());
			
			return new GetOrderShippingsResult(EnumResultStatus.Complete, dataTable);
		}
		#endregion
	}
	#endregion

	#region Arg
	/// <summary>
	/// GetOrderShippingsコマンド用引数クラス
	/// </summary>
	public class GetOrderShippingsArg : ApiCommandArgBase
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
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GetOrderShippingsArg()
		{
			CreatedTimeSpan = null;
			UpdatedTimeSpan = null;
			OrderStatus = "";
			OrderId = "";
		    OrderPaymentStatus = "";
            OrderExtendedStatusSpecification = OrderExtendedStatusSpecification.GenByString(string.Empty);
		}
	    #endregion
	}
	#endregion

	#region Result
	/// <summary>
	/// GetOrderShippingsコマンド用実行結果クラス
	/// </summary>
	public class GetOrderShippingsResult : ApiCommandResult
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="enumResultStatus"></param>
		/// <param name="dataTable"></param>
		public GetOrderShippingsResult(EnumResultStatus enumResultStatus, DataTable dataTable) : base(enumResultStatus)
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
