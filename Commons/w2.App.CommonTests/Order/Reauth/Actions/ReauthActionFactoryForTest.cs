using w2.App.Common.Order.Reauth.Actions;
using w2.Domain.Order;

namespace w2.App.CommonTests.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション ユニットテスト用ファクトリ
	/// </summary>
	public static class ReauthActionFactoryForTest
	{
		/// <summary>
		/// 空の再与信アクション（クレジットカード与信）を生成
		/// </summary>
		/// <returns>再与信アクション（クレジットカード与信）</returns>
		public static ReauthCreditCardAction CreateEmptyReauthCreditCard() =>
			new ReauthCreditCardAction(new ReauthCreditCardAction.ReauthActionParams(new OrderModel()));

		/// <summary>
		/// 空の <see cref="ReauthCvsDefAction" /> を生成
		/// </summary>
		/// <returns><see cref="ReauthCvsDefAction" /></returns>
		public static ReauthCvsDefAction CreateEmptyReauthCvsDef() =>
			new ReauthCvsDefAction(new ReauthCvsDefAction.ReauthActionParams(new OrderModel()));

		/// <summary>
		/// 空の <see cref="ReauthAtodeneExcludingAuthExtensionDefAction" /> を生成
		/// </summary>
		/// <returns><see cref="ReauthAtodeneExcludingAuthExtensionDefAction" /></returns>
		public static ReauthAtodeneExcludingAuthExtensionDefAction CreateEmptyReauthAtodeneExcludingAuthExtensionDef() =>
			new ReauthAtodeneExcludingAuthExtensionDefAction(
				new ReauthCvsDefAction.ReauthActionParams(new OrderModel()),
				originalOrderId: string.Empty);

		/// <summary>
		/// 空の再与信アクション（クレジットカードキャンセル）を生成
		/// </summary>
		/// <returns>再与信アクション（クレジットカードキャンセル）</returns>
		public static CancelCreditCardAction CreateEmptyCancelCreditCard() =>
			new CancelCreditCardAction(new CancelCreditCardAction.ReauthActionParams(new OrderModel()));

		/// <summary>
		/// 空の <see cref="CancelCvsDefAction" /> を生成
		/// </summary>
		/// <returns><see cref="CancelCvsDefAction" /></returns>
		public static CancelCvsDefAction CreateEmptyCancelCvsDef() =>
			new CancelCvsDefAction(new CancelCvsDefAction.ReauthActionParams(new OrderModel()));

		/// <summary>
		/// 空の <see cref="ReduceCvsDefAction" /> を生成
		/// </summary>
		/// <returns><see cref="ReduceCvsDefAction" /></returns>
		public static ReduceCvsDefAction CreateEmptyReduceCvsDef() =>
			new ReduceCvsDefAction(new ReduceCvsDefAction.ReauthActionParams(new OrderModel()));

		/// <summary>
		/// 空の <see cref="UpdateVeritransAfterPayAction" /> を生成
		/// </summary>
		/// <returns><see cref="UpdateVeritransAfterPayAction" /></returns>
		public static UpdateVeritransAfterPayAction CreateEmptyUpdateVeritransAfterPay() =>
			new UpdateVeritransAfterPayAction(new OrderModel());

		/// <summary>
		/// 空の <see cref="UpdateScoreAfterPayAction" /> を生成
		/// </summary>
		/// <returns><see cref="UpdateScoreAfterPayAction" /></returns>
		public static UpdateScoreAfterPayAction CreateEmptyUpdateScoreAfterPay() =>
			new UpdateScoreAfterPayAction(new UpdateScoreAfterPayAction.ReauthActionParams(new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（クレジットカード売上確定）を生成
		/// </summary>
		/// <returns>再与信アクション（クレジットカード売上確定）</returns>
		public static SalesCreditCardAction CreateEmptySalesCreditCard() =>
			new SalesCreditCardAction(new SalesCreditCardAction.ReauthActionParams(new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（LinePay与信）を生成
		/// </summary>
		/// <returns>再与信アクション（LinePay与信）</returns>
		public static ReauthLinePayAction CreateEmptyReauthLinePay() =>
			new ReauthLinePayAction(new ReauthLinePayAction.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（LinePayキャンセル）を生成
		/// </summary>
		/// <returns>再与信アクション（LinePayキャンセル）</returns>
		public static CancelLinePayAction CreateEmptyCancelLinePay() =>
			new CancelLinePayAction(new CancelLinePayAction.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（LinePay売上確定）を生成
		/// </summary>
		/// <returns>再与信アクション（LinePay売上確定）</returns>
		public static SalesLinePayAction CreateEmptySalesLinePay() =>
			new SalesLinePayAction(new SalesLinePayAction.ReauthActionParams(new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（LinePay返金）を生成
		/// </summary>
		/// <returns>再与信アクション（LinePay返金）</returns>
		public static RefundLinePayAction CreateEmptyRefundLinePay() =>
			new RefundLinePayAction(new RefundLinePayAction.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の<see cref="CancelGmoPostAction" />を生成
		/// </summary>
		/// <returns><see cref="CancelGmoPostAction" /></returns>
		public static CancelGmoPostAction CreateEmptyCancelGmoPost() =>
			new CancelGmoPostAction(new CancelGmoPostAction.CancelGmoPostActionParams(new OrderModel()));

		/// <summary>
		/// 空の <see cref="ReduceGmoPostAction" /> を生成
		/// </summary>
		/// <returns><see cref="ReduceGmoPostAction" /></returns>
		public static ReduceGmoPostAction CreateEmptyReduceGmoPost() =>
			new ReduceGmoPostAction(new ReduceGmoPostAction.ReduceGmoPostActionParams(new OrderModel()));

		/// <summary>
		/// 空の<see cref="EditGmoPostAction" />を生成
		/// </summary>
		/// <returns><see cref="EditGmoPostAction" /></returns>
		public static EditGmoPostAction CreateEmptyEditGmoPost() =>
			new EditGmoPostAction(new EditGmoPostAction.EditGmoPostActionParams(new OrderModel()));

		/// <summary>
		/// 空の<see cref="ModifyCancelBillingGmoPostAction" />を生成
		/// </summary>
		/// <returns><see cref="ModifyCancelBillingGmoPostAction" /></returns>
		public static ModifyCancelBillingGmoPostAction CreateEmptyModifyCancelBillingGmoPost() =>
			new ModifyCancelBillingGmoPostAction(
				new ModifyCancelBillingGmoPostAction.ModifyCancelBillingGmoPostActionParams(new OrderModel(), isReturnAll: true));

		/// <summary>
		/// 空の<see cref="CancelGmoAtokaraAction" />を作成
		/// </summary>
		/// <returns><see cref="CancelGmoAtokaraAction" /></returns>
		public static CancelGmoAtokaraAction CreateEmptyCancelGmoAtokara() =>
			new CancelGmoAtokaraAction(new CancelGmoAtokaraAction.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の<see cref="UpdateGmoAtokaraAction" />を作成
		/// </summary>
		/// <returns><see cref="UpdateGmoAtokaraAction" /></returns>
		public static UpdateGmoAtokaraAction CreateEmptyUpdateGmoAtokara() =>
			new UpdateGmoAtokaraAction(new UpdateGmoAtokaraAction.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（Atone与信）を生成
		/// </summary>
		/// <returns>再与信アクション（LinePay与信）</returns>
		public static ReauthAtoneAction CreateEmptyReauthAtone() =>
			new ReauthAtoneAction(new ReauthAtoneAction.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（Atoneキャンセル:変更なし）を生成
		/// </summary>
		/// <returns>再与信アクション（Atoneキャンセル）</returns>
		public static CancelAtoneAction CreateEmptyCancelAtone () =>
			new CancelAtoneAction(new CancelAtoneAction.ReauthActionParams(new OrderModel(), Common.Order.Reauth.ReauthCreatorFacade.ReauthTypes.NoChangeAtone));

		/// <summary>
		/// 空の再与信アクション（Atoneキャンセル：返品交換）を生成
		/// </summary>
		/// <returns>再与信アクション（Atoneキャンセル）</returns>
		public static CancelAtoneAction CreateEmptyCancelAtoneReturnOrExchange () =>
			new CancelAtoneAction(new CancelAtoneAction.ReauthActionParams(new OrderModel(), Common.Order.Reauth.ReauthCreatorFacade.ReauthTypes.AtoneReturnAllItems));

		/// <summary>
		/// 空の<see cref="SalesPaypayAction" />を作成
		/// </summary>
		/// <returns><see cref="SalesPaypayAction" /></returns>
		public static SalesPaypayAction CreateEmptySalesPaypay() =>
			new SalesPaypayAction(new SalesPaypayAction.ReauthActionParams(new OrderModel()));

		/// <summary>
		/// 空の<see cref="RefundPaypayAction" />を作成
		/// </summary>
		/// <returns><see cref="RefundPaypayAction" /></returns>
		public static RefundPaypayAction CreateEmptyRefundPaypay() =>
			new RefundPaypayAction(new RefundPaypayAction.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の<see cref="CancelPaypayAction" />を作成
		/// </summary>
		/// <returns><see cref="CancelPaypayAction" /></returns>
		public static CancelPaypayAction CreateEmptyCancelPaypay() =>
			new CancelPaypayAction(new CancelPaypayAction.ReauthActionParams(new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（Aftee与信）を生成
		/// </summary>
		/// <returns>再与信アクション（LinePay与信）</returns>
		public static ReauthAfteeAction CreateEmptyReauthAftee() =>
			new ReauthAfteeAction(new ReauthAfteeAction.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（Afteeキャンセル:変更なし）を生成
		/// </summary>
		/// <returns>再与信アクション（Afteeキャンセル）</returns>
		public static CancelAfteeAction CreateEmptyCancelAftee() =>
			new CancelAfteeAction(new CancelAfteeAction.ReauthActionParams(new OrderModel(), Common.Order.Reauth.ReauthCreatorFacade.ReauthTypes.NoChangeAftee));

		/// <summary>
		/// 空の再与信アクション（Afteeキャンセル：返品交換）を生成
		/// </summary>
		/// <returns>再与信アクション（Afteeキャンセル）</returns>
		public static CancelAfteeAction CreateEmptyCancelAfteeReturnOrExchange() =>
			new CancelAfteeAction(new CancelAfteeAction.ReauthActionParams(new OrderModel(), Common.Order.Reauth.ReauthCreatorFacade.ReauthTypes.AfteeReturnAllItems));

		/// <summary>
		/// 空の再与信アクション（AmazonPay与信）を生成
		/// </summary>
		/// <returns>再与信アクション（AmazonPay与信）</returns>
		public static ReauthAmazonPayAction CreateEmptyReauthAmazonPay() =>
			new ReauthAmazonPayAction(new ReauthAmazonPayAction.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（AmazonPayキャンセル）を生成
		/// </summary>
		/// <returns>再与信アクション（AmazonPayキャンセル）</returns>
		public static CancelAmazonPayAction CreateEmptyCancelAmazonPay() =>
			new CancelAmazonPayAction(new CancelAmazonPayAction.ReauthActionParams(new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（AmazonPay売上確定）を生成
		/// </summary>
		/// <returns>再与信アクション（AmazonPay売上確定）</returns>
		public static SalesAmazonPayAction CreateEmptySalesAmazonPay() =>
			new SalesAmazonPayAction(new SalesAmazonPayAction.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（AmazonPay返金）を生成
		/// </summary>
		/// <returns>再与信アクション（AmazonPay返金）</returns>
		public static RefundAmazonPayAction CreateEmptyRefundAmazonPay() =>
			new RefundAmazonPayAction(new RefundAmazonPayAction.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（AmazonPayCv2与信）を生成
		/// </summary>
		/// <returns>再与信アクション（AmazonPayCv2与信）</returns>
		public static ReauthAmazonPayCv2Action CreateEmptyReauthAmazonPayCv2() =>
			new ReauthAmazonPayCv2Action(new ReauthAmazonPayCv2Action.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（AmazonPayCv2キャンセル）を生成
		/// </summary>
		/// <returns>再与信アクション（AmazonPayCv2キャンセル）</returns>
		public static CancelAmazonPayCv2Action CreateEmptyCancelAmazonPayCv2() =>
			new CancelAmazonPayCv2Action(new CancelAmazonPayCv2Action.ReauthActionParams(new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（AmazonPayCv2売上確定）を生成
		/// </summary>
		/// <returns>再与信アクション（AmazonPayCv2売上確定）</returns>
		public static SalesAmazonPayCv2Action CreateEmptySalesAmazonPayCv2() =>
			new SalesAmazonPayCv2Action(new SalesAmazonPayCv2Action.ReauthActionParams(new OrderModel(), new OrderModel()));

		/// <summary>
		/// 空の再与信アクション（AmazonPayCv2返金）を生成
		/// </summary>
		/// <returns>再与信アクション（AmazonPayCv2返金）</returns>
		public static RefundAmazonPayCv2Action CreateEmptyRefundAmazonPayCv2() =>
			new RefundAmazonPayCv2Action(new RefundAmazonPayCv2Action.ReauthActionParams(new OrderModel(), new OrderModel()));
	}
}
