/*
=========================================================================================================
  Module      :  注文関連ファイル取込モジュールインターフェース(IImport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using w2.Common.Util;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Import
{
	/// <summary>
	/// IImport の概要の説明です
	/// </summary>
	public abstract class ImportBase
	{
		/// <summary>
		/// インポート
		/// </summary>
		/// <param name="sr">csvファイルストリーム</param>
		/// <param name="loginOperatorName">ログインオペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>取込結果</returns>
		public abstract bool Import(StreamReader sr, string loginOperatorName, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 外部連携：出荷情報登録
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="orderPaymentKbn">決済種別ID</param>
		/// <param name="shippingCheckNoOld">過去の配送伝票番号</param>
		/// <param name="shippingCheckNoNew">新しい配送伝票番号</param>
		/// <param name="cardTranId">
		/// 決済取引ID
		/// Gmo後払いの場合はGmo取引ID
		/// </param>
		/// <param name="deliveryCompanyType">出荷連携配送会社</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="canShipmentEntry">出荷情報登録可能か</param>
		/// <returns>エラーメッセージ</returns>
		protected string ExternalShipmentEntry(
			string orderId,
			string paymentOrderId,
			string orderPaymentKbn,
			string shippingCheckNoOld,
			string shippingCheckNoNew,
			string cardTranId,
			string deliveryCompanyType,
			UpdateHistoryAction updateHistoryAction,
			out bool canShipmentEntry)
		{
			if (OrderCommon.CanShipmentEntry(orderPaymentKbn))
			{
				canShipmentEntry = true;
				var errorMessage = OrderCommon.ShipmentEntry(
					orderId,
					paymentOrderId,
					orderPaymentKbn,
					shippingCheckNoNew,
					shippingCheckNoOld,
					cardTranId,
					deliveryCompanyType,
					updateHistoryAction);
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					canShipmentEntry = false;
					return errorMessage + " 注文ID：" + orderId;
				}
			}
			else
			{
				canShipmentEntry = false;
			}

			return "";
		}

		/// プロパティ
		/// <summary>処理結果メッセージ</summary>
		public string ErrorMessage
		{
			get { return StringUtility.ToEmpty(m_strErrorMessage); }
		}
		protected string m_strErrorMessage = "";
		/// <summary>更新件数</summary>
		public int UpdatedCount
		{
			get { return m_iUpdatedCount; }
		}
		protected int m_iUpdatedCount = 0;
		/// <summary>処理行数</summary>
		public int LinesCount
		{
			get { return m_iLinesCount; }
		}
		protected int m_iLinesCount = 0;
		/// <summary>Success order info list</summary>
		public List<SuccessInfo> SuccessOrderInfos { get { return this.m_successOrderInfos; } }
		protected List<SuccessInfo> m_successOrderInfos = new List<SuccessInfo>();
		/// <summary>外部出荷登録実行</summary>
		public bool ExecExternalShipmentEntry { get; set; }
		/// <summary>ウケトル連携実行</summary>
		public bool ExecUketoruCooperation { get; set; }
		/// <summary>成功情報</summary>
		public class SuccessInfo
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="lineNo">行番号</param>
			/// <param name="orderId">注文ID</param>
			/// <param name="shippingCheckNoOld">変更前配送伝票番号</param>
			public SuccessInfo(int lineNo, string orderId, string shippingCheckNoOld)
			{
				this.LineNo = lineNo;
				this.OrderId = orderId;
				this.ShippingCheckNoOld = shippingCheckNoOld;
			}
			/// <summary>行番号</summary>
			public int LineNo { get; set; }
			/// <summary>注文ID</summary>
			public string OrderId { get; set; }
			/// <summary>変更前配送伝票番号</summary>
			public string ShippingCheckNoOld { get; set; }
		}
		/// <summary>csvファイルの名前</summary>
		public string CsvFileName { get; set; }
	}
}