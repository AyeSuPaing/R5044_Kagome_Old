/*
=========================================================================================================
  Module      : 再与信アクション（コンビニ後払い請求書再発行）クラス(ReprintCvsDefAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.YamatoKa.Utils;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（コンビニ後払い請求書再発行）クラス
	/// </summary>
	public class ReprintCvsDefAction : BaseReauthAction<ReprintCvsDefAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReprintCvsDefAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reprint, "コンビニ後払い請求書再発行", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// コンビニ後払い請求書再発行
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var order = reauthActionParams.Order;
			var owner = order.Owner;
			var shipping = order.Shippings[0];

			var api = new PaymentYamatoKaReprintApi();
			if (api.Exec(
				order.CardTranId,
				"1",
				// 請求内容変更・請求書再発行
				PaymentYamatoKaReprintApi.ReissueReason.Others,
				"お届け希望日変更",
				PaymentYamatoKaUtility.CreateYamatoKaShipYmd(order),
				PaymentYamatoKaSendDiv2.Send,
				owner.OwnerZip,
				new PaymentYamatoKaAddress(owner.OwnerAddr1, owner.OwnerAddr2, owner.OwnerAddr3, owner.OwnerAddr4),
				PaymentYamatoKaUtility.CreateProductItemList(order.LastBilledAmount),
				string.Empty) == false)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: api.ResponseData.ErrorMessages);
			}

			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, order.OrderId, order.CardTranId, null),
				order.CardTranId,
				order.CardTranId);
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