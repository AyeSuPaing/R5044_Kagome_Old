/*
=========================================================================================================
  Module      : UpdateOrderExtendedStatusコマンドクラス(UpdateOrderExtendedStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Globalization;
using w2.App.Common.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Order
{

    #region Command

	/// <summary>
	/// UpdateOrderExtendedStatusコマンドクラス
	/// </summary>
    public class UpdateOrderExtendedStatus : ApiCommandBase
    {
        /// <summary>
        /// コマンド実行処理
        /// </summary>
        /// <param name="apiCommandArg"></param>
        /// <returns></returns>
        protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
        {
            var arg = (UpdateOrderExtendedStatusArg) apiCommandArg;

            ApiLogger.Write(LogLevel.info, "コマンド引数情報:" + GetType().Name,
                            string.Format(
								"OrderId:{0}, StatusNumber:'{1}',Flag:'{2}',Status:'{3}',UpdateDate:'{4}'",
								arg.OrderId ?? "Null",
                                arg.StatusNumber,
                                arg.Flag,
								arg.Status ?? "Null",
                                arg.UpdateDate));

			OrderCommon.UpdateOrderExtendStatus(
				arg.OrderId,
				arg.StatusNumber,
				arg.Flag,
				arg.Status,
				arg.UpdateDate,
				UpdateHistoryAction.Insert);

            return new UpdateOrderExtendedStatusResult(EnumResultStatus.Complete);
        }
    }

    #endregion

    #region Arg

	/// <summary>
	/// UpdateOrderExtendedStatusコマンド用引数クラス
	/// </summary>
    public class UpdateOrderExtendedStatusArg : ApiCommandArgBase
    {
		/// <summary>注文ID</summary>
        public string OrderId { get; set; }

		/// <summary>拡張ステータス項番</summary>
        public int StatusNumber { get; set; }

		/// <summary>更新日付</summary>
        public DateTime UpdateDate { get; set; }

		/// <summary>フラグ</summary>
        public bool? Flag { get; set; }

        /// <summary>ステータス（文字列）</summary>
        public string Status { get; set; }

		/// <summary>
		/// UpdateOrderExtendedStatusコマンド用引数
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="statusNumber">拡張ステータス項番</param>
		/// <param name="flag">フラグ</param>
		/// <param name="status">ステータス</param>
		/// <param name="updateDate">更新日付</param>
		public UpdateOrderExtendedStatusArg(string orderId,
											int statusNumber,
                                            bool? flag = null,
											string status = "0",
                                            DateTime? updateDate = null
                                            )
        {
            OrderId = orderId;
            StatusNumber = statusNumber;
            UpdateDate = updateDate.HasValue ? updateDate.Value : DateTime.Now;
            Flag = flag;
			Status = status;
        }
    }

    #endregion

    #region Result

	/// <summary>
	/// UpdateOrderExtendedStatusコマンド用実行結果クラス
	/// </summary>
    public class UpdateOrderExtendedStatusResult : ApiCommandResult
    {
        /// <summary>
        /// コンストラクタ <see cref="UpdateOrderExtendedStatusResult" />
        /// </summary>
        /// <param name="enumResultStatus">結果ステータス</param>
        public UpdateOrderExtendedStatusResult(EnumResultStatus enumResultStatus) : base(enumResultStatus)
        {
        }
    }

    #endregion
}
