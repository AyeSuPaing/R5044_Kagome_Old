/*
=========================================================================================================
  Module      : 再与信アクション（コンビニ後払い請求金額減額）クラス(ReduceCvsDefAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.YamatoKa.Utils;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（コンビニ後払い請求金額減額）クラス
	/// </summary>
	public class ReduceCvsDefAction : BaseReauthAction<ReduceCvsDefAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReduceCvsDefAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reduce, "コンビニ後払い請求金額減額", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// コンビニ後払い請求金額減額
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var order = reauthActionParams.Order;
			var isSms = OrderCommon.CheckPaymentYamatoKaSms(order.OrderPaymentKbn);

			var api = new PaymentYamatoKaReduceApi();
			if (api.Exec(
				order.CardTranId,
				order.OrderDate.Value.ToString("yyyyMMdd"),
				PaymentYamatoKaUtility.CreateYamatoKaShipYmd(order),
				order.Owner.OwnerZip,
				new PaymentYamatoKaAddress(order.Owner.OwnerAddr1, order.Owner.OwnerAddr2, order.Owner.OwnerAddr3, order.Owner.OwnerAddr4),
				PaymentYamatoKaUtility.CreateProductItemList(order.LastBilledAmount),
				order.LastBilledAmount,
				(isSms
					? PaymentYamatoKaSendDiv2.SmsAuth
					: PaymentYamatoKaSendDiv2.Send),
				string.Empty) == false)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(api.ResponseData.ErrorCode, api.ResponseData.ErrorMessages));
			}

			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, order.CardTranId, order.LastBilledAmount),
				order.CardTranId,
				order.PaymentOrderId);
		}
		#endregion

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public new class ReauthActionParams : IReauthActionParams
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="order">注文情報</param>
			public ReauthActionParams(OrderModel order)
			{
				this.Order = order;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報</summary>
			public OrderModel Order { get; private set; }
			#endregion
		}
	}
}