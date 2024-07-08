/*
=========================================================================================================
  Module      : 再与信インスタンス作成Facade(ReauthCreatorFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Order.Payment.GMO.OrderRegister;
using w2.App.Common.Order.Payment.JACCS.ATODENE.Transaction;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Reauth.Actions;
using w2.Common.Sql;
using w2.Domain;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth
{
	/// <summary>
	/// 再与信インスタンス作成Facade
	/// </summary>
	public class ReauthCreatorFacade
	{
		/// <summary>処理区分</summary>
		public enum ExecuteTypes
		{
			/// <summary>システム連動</summary>
			System,
			/// <summary>強制与信</summary>
			ForcedAuth,
			/// <summary>何もしない</summary>
			None,
		}

		/// <summary>注文処理区分</summary>
		public enum OrderActionTypes
		{
			/// <summary>編集</summary>
			Modify,
			/// <summary>返品</summary>
			Return,
			/// <summary>交換</summary>
			Exchange,
			/// <summary>交換キャンセル</summary>
			ExchangeCancel,
		}

		/// <summary>再与信区分</summary>
		public enum ReauthTypes
		{
			/// <summary>クレジットカード→クレジットカード</summary>
			NoChangeCredit,
			/// <summary>クレジットカード→コンビニ後払い</summary>
			ChangeCreditToCvsDef,
			/// <summary>Change Credit To Gmo Post</summary>
			ChangeCreditToGmoPost,
			/// <summary>クレジットカード→代引き</summary>
			ChangeCreditToCollectOrOthers,
			/// <summary>クレジットカード→キャリア決済</summary>
			ChangeCreditToCarrier,
			/// <summary>クレジットカード→Amazon Pay</summary>
			ChangeCreditToAmazonPay,
			/// <summary>クレジットカード→PayPal</summary>
			ChangeCreditToPayPal,
			/// <summary>クレジットカード→後付款(TriLink後払い)</summary>
			ChangeCreditToTriLinkAfterPay,
			/// <summary>クレジットカード→決済無し</summary>
			ChangeCreditToNoPayment,
			/// <summary>クレジットカード→Paidy Pay</summary>
			ChangeCreditToPaidyPay,
			/// <summary>クレジットカード全返品</summary>
			CreditReturnAllItems,
			/// <summary>クレジットカード→Atone</summary>
			ChangeCreditToAtone,
			/// <summary>クレジットカード→Aftee</summary>
			ChangeCreditToAftee,
			/// <summary>クレジットカード→LINEPay</summary>
			ChangeCreditToLinePay,
			// <summary>クレジットカード→NP後払い</summary>
			ChangeCreditToNPAfterPay,
			/// <summary>クレジットカード→PayPay</summary>
			ChangeCreditToPayPay,
			// <summary>クレジットカード→Boku</summary>
			ChangeCreditToBoku,

			/// <summary>コンビニ後払い→コンビニ後払い</summary>
			NoChangeCvsDef,
			/// <summary>コンビニ後払い→クレジットカード</summary>
			ChangeCvsDefToCredit,
			/// <summary>コンビニ後払い→キャリア決済</summary>
			ChangeCvsDefToCarrier,
			/// <summary>コンビニ後払い→代引き</summary>
			ChangeCvsDefToCollectOrOthers,
			/// <summary>コンビニ後払い→Amazon Pay</summary>
			ChangeCvsDefToAmazonPay,
			/// <summary>コンビニ後払い→PayPal</summary>
			ChangeCvsDefToPayPal,
			/// <summary>コンビニ後払い→後付款(TriLink後払い)</summary>
			ChangeCvsDefToTriLinkAfterPay,
			/// <summary>コンビニ後払い→決済無し</summary>
			ChangeCvsDefToNoPayment,
			/// <summary>コンビニ後払い→Paidy Pay</summary>
			ChangeCvsDefToPaidyPay,
			/// <summary>コンビニ後払い全返品</summary>
			CvsDefReturnAllItems,
			/// <summary>PayAsYouGo Return All Items</summary>
			PayAsYouGoReturnAllItems,
			/// <summary>FramePayment Return All Items</summary>
			FramePaymentReturnAllItems,
			/// <summary>コンビニ後払い→Atone</summary>
			ChangeCvsDefToAtone,
			/// <summary>コンビニ後払い→Aftee</summary>
			ChangeCvsDefToAftee,
			/// <summary>コンビニ後払い→LINEPay</summary>
			ChangeCvsDefToLinePay,
			/// <summary>コンビニ後払い→NP後払い</summary>
			ChangeCvsDefToNPAfterPay,
			/// <summary>コンビニ後払い→PayPay</summary>
			ChangeCvsDefToPayPay,
			/// <summary>コンビニ後払い→Boku</summary>
			ChangeCvsDefToBoku,

			/// <summary>PayPal→PayPal</summary>
			NoChangePayPal,
			/// <summary>PayPal→クレジットカード</summary>
			ChangePayPalToCredit,
			/// <summary>PayPal→コンビニ後払い</summary>
			ChangePayPalToCvsDef,
			/// <summary>Change PayPal To Gmo Post</summary>
			ChangePayPalToGmoPost,
			/// <summary>PayPal→代引き</summary>
			ChangePayPalToCollectOrOthers,
			/// <summary>PayPal→キャリア決済</summary>
			ChangePayPalToCarrier,
			/// <summary>PayPal→Amazon Pay</summary>
			ChangePayPalToAmazonPay,
			/// <summary>PayPal→後付款(TriLink後払い)</summary>
			ChangePayPalToTriLinkAfterPay,
			/// <summary>PayPal→決済無し</summary>
			ChangePayPalToNoPayment,
			/// <summary>PayPal→Paidy Pay</summary>
			ChangePayPalToPaidyPay,
			/// <summary>PayPal全返品</summary>
			PayPalReturnAllItems,
			/// <summary>PayPal→Atone</summary>
			ChangePayPalToAtone,
			/// <summary>PayPal→Aftee</summary>
			ChangePayPalToAftee,
			/// <summary>PayPal→LINEPay</summary>
			ChangePayPalToLinePay,
			/// <summary>PayPal→NP後払い</summary>
			ChangePayPalToNPAfterPay,
			/// <summary>PayPal→PayPay</summary>
			ChangePayPalToPayPay,
			/// <summary>PayPal→Boku</summary>
			ChangePayPalToBoku,

			/// <summary>後付款(TriLink後払い)い→後付款(TriLink後払い)</summary>
			NoChangeTriLinkAfterPay,
			/// <summary>後付款(TriLink後払い)→クレジットカード</summary>
			ChangeTriLinkAfterPayToCredit,
			/// <summary>後付款(TriLink後払い)→コンビニ後払い</summary>
			ChangeTriLinkAfterPayToCvsDef,
			/// <summary>Change TriLinkAfterPay To Gmo Post</summary>
			ChangeTriLinkAfterPayToGmoPost,
			/// <summary>後付款(TriLink後払い)→代引き</summary>
			ChangeTriLinkAfterPayToCollectOrOthers,
			/// <summary>後付款(TriLink後払い)→キャリア決済</summary>
			ChangeTriLinkAfterPayToCarrier,
			/// <summary>後付款(TriLink後払い)→Amazon Pay</summary>
			ChangeTriLinkAfterPayToAmazonPay,
			/// <summary>後付款(TriLink後払い)→PayPal</summary>
			ChangeTriLinkAfterPayToPayPal,
			/// <summary>後付款(TriLink後払い)→決済無し</summary>
			ChangeTriLinkAfterPayToNoPayment,
			/// <summary>後付款(TriLink後払い)→Paidy Pay</summary>
			ChangeTriLinkAfterPayToPaidyPay,
			/// <summary>後付款(TriLink後払い)全返品</summary>
			TriLinkAfterPayReturnAllItems,
			/// <summary>後付款(TriLink後払い)→Atone</summary>
			ChangeTriLinkAfterPayToAtone,
			/// <summary>後付款(TriLink後払い)→Aftee</summary>
			ChangeTriLinkAfterPayToAftee,
			/// <summary>後付款(TriLink後払い)→LINEPay</summary>
			ChangeTriLinkAfterPayToLinePay,
			/// <summary>後付款(TriLink後払い)→NP後払い</summary>
			ChangeTriLinkAfterPayToNPAfterPay,
			/// <summary>後付款(TriLink後払い)→PayPay</summary>
			ChangeTriLinkAfterPayToPayPay,
			/// <summary>後付款(TriLink後払い)→Boku</summary>
			ChangeTriLinkAfterPayToBoku,

			/// <summary>キャリア決済→クレジットカード</summary>
			ChangeCarrierToCredit,
			/// <summary>キャリア決済→コンビニ後払い</summary>
			ChangeCarrierToCvsDef,
			/// <summary>Change Carrier To Gmo Post</summary>
			ChangeCarrierToGmoPost,
			/// <summary>キャリア決済→Amazon Pay</summary>
			ChangeCarrierToAmazonPay,
			/// <summary>キャリア決済→PayPal</summary>
			ChangeCarrierToPayPal,
			/// <summary>キャリア決済→後付款(TriLink後払い)</summary>
			ChangeCarrierToTriLinkAfterPay,
			/// <summary>キャリア決済→代引き</summary>
			ChangeCarrierToCollectOrOthers,
			/// <summary>キャリア決済→Paidy Pay</summary>
			ChangeCarrierToPaidyPay,
			/// <summary>キャリア決済→決済無し</summary>
			ChangeCarrierToNoPayment,
			/// <summary>キャリア決済→Atone</summary>
			ChangeCarrierToAtone,
			/// <summary>キャリア決済→Aftee</summary>
			ChangeCarrierToAftee,
			/// <summary>キャリア決済→LINEPay</summary>
			ChangeCarrierToLinePay,
			/// <summary>キャリア決済→NP後払い</summary>
			ChangeCarrierToNPAfterPay,
			/// <summary>キャリア決済→PayPay</summary>
			ChangeCarrierToPayPay,
			/// <summary>キャリア決済→Boku</summary>
			ChangeCarrierToBoku,

			/// <summary>代引き→クレジットカード</summary>
			ChangeCollectOrOthersToCredit,
			/// <summary>代引き→コンビニ後払い</summary>
			ChangeCollectOrOthersToCvsDef,
			/// <summary>Change Collect Or Others To Gmo Post</summary>
			ChangeCollectOrOthersToGmoPost,
			/// <summary>代引き→キャリア決済</summary>
			ChangeCollectOrOthersToCarrier,
			/// <summary>代引き→Amazon Pay</summary>
			ChangeCollectOrOthersToAmazonPay,
			/// <summary>代引き→PayPal</summary>
			ChangeCollectOrOthersToPayPal,
			/// <summary>代引き→後付款(TriLink後払い)</summary>
			ChangeCollectOrOthersToTriLinkAfterPay,
			/// <summary>代引き→Paidy Pay</summary>
			ChangeCollectOrOthersToPaidyPay,
			/// <summary>代引き→決済無し</summary>
			ChangeCollectOrOthersToNoPayment,
			/// <summary>代引き→Atone</summary>
			ChangeCollectOrOthersToAtone,
			/// <summary>代引き→Aftee</summary>
			ChangeCollectOrOthersToAftee,
			/// <summary>代引き→LINEPay</summary>
			ChangeCollectOrOthersToLinePay,
			/// <summary>代引き→NP後払い</summary>
			ChangeCollectOrOthersToNPAfterPay,
			/// <summary>代引き→PayPay</summary>
			ChangeCollectOrOthersToPayPay,

			/// <summary>Amazon Pay→Amazon Pay</summary>
			NoChangeAmazonPay,
			/// <summary>Amazon Pay→クレジットカード</summary>
			ChangeAmazonPayToCredit,
			/// <summary>Amazon Pay→コンビニ後払い</summary>
			ChangeAmazonPayToCvsDef,
			/// <summary>Change Amazon Pay To Gmo Post</summary>
			ChangeAmazonPayToGmoPost,
			/// <summary>Amazon Pay→代引き</summary>
			ChangeAmazonPayToCollectOrOthers,
			/// <summary>Amazon Pay→キャリア決済</summary>
			ChangeAmazonPayToCarrier,
			/// <summary>Amazon Pay→PayPal</summary>
			ChangeAmazonPayToPayPal,
			/// <summary>Amazon Pay→後付款(TriLink後払い)</summary>
			ChangeAmazonPayToTriLinkAfterPay,
			/// <summary>Amazon Pay→Paidy Pay</summary>
			ChangeAmazonPayToPaidyPay,
			/// <summary>Amazon Pay→決済無し</summary>
			ChangeAmazonPayToNoPayment,
			/// <summary>Amazon Pay全返品</summary>
			AmazonPayReturnAllItems,
			/// <summary>Amazon Pay→Atone</summary>
			ChangeAmazonPayToAtone,
			/// <summary>Amazon Pay→Aftee</summary>
			ChangeAmazonPayToAftee,
			/// <summary>Amazon Pay→LINEPay</summary>
			ChangeAmazonPayToLinePay,
			/// <summary>Amazon Pay→NP後払い</summary>
			ChangeAmazonPayToNPAfterPay,
			/// <summary>Amazon Pay→PayPay</summary>
			ChangeAmazonPayToPayPay,
			/// <summary>Amazon Pay→Boku</summary>
			ChangeAmazonPayToBoku,

			/// <summary>Paidy Pay→Paidy Pay</summary>
			NoChangePaidyPay,
			/// <summary>Paidy→クレジットカード</summary>
			ChangePaidyPayToCredit,
			/// <summary>Paidy Pay→コンビニ後払い</summary>
			ChangePaidyPayToCvsDef,
			/// <summary>Change PaidyPay To Gmo Post</summary>
			ChangePaidyPayToGmoPost,
			/// <summary>Paidy Pay→代引き</summary>
			ChangePaidyPayToCollectOrOthers,
			/// <summary>Paidy Pay→PayPal</summary>
			ChangePaidyPayToPayPal,
			/// <summary>Paidy Pay→キャリア決済</summary>
			ChangePaidyPayToCarrier,
			/// <summary>Paidy Pay→Amazon Pay</summary>
			ChangePaidyPayToAmazonPay,
			/// <summary>Paidy Pay→後付款(TriLink後払い)</summary>
			ChangePaidyPayToTriLinkAfterPay,
			/// <summary>Paidy Pay→決済無し</summary>
			ChangePaidyPayToNoPayment,
			/// <summary>Paidy Pay全返品</summary>
			PaidyPayReturnAllItems,
			/// <summary>Paidy Pay→ LINEPay</summary>
			ChangePaidyPayToLinePay,
			/// <summary>Paidy Pay→NP後払い</summary>
			ChangePaidyPayToNPAfterPay,
			/// <summary>Paidy Pay→PayPay</summary>
			ChangePaidyPayToPayPay,
			/// <summary>Paidy Pay→Boku</summary>
			ChangePaidyPayToBoku,

			/// <summary>決済無し→クレジットカード</summary>
			ChangeNoPaymentToCredit,
			/// <summary>決済無し→コンビニ後払い</summary>
			ChangeNoPaymentToCvsDef,
			/// <summary>Change No Payment To Gmo Post</summary>
			ChangeNoPaymentToGmoPost,
			/// <summary>決済無し→キャリア決済</summary>
			ChangeNoPaymentToCarrier,
			/// <summary>決済無し→Amazon Pay</summary>
			ChangeNoPaymentToAmazonPay,
			/// <summary>決済無し→PayPal</summary>
			ChangeNoPaymentToPayPal,
			/// <summary>決済無し→後付款(TriLink後払い)</summary>
			ChangeNoPaymentToTriLinkAfterPay,
			/// <summary>決済無し→Paidy Pay</summary>
			ChangeNoPaymentToPaidyPay,
			/// <summary>決済無し→代引き</summary>
			ChangeNoPaymentToCollectOrOthers,
			/// <summary>決済無し→LINEPay</summary>
			ChangeNoPaymentToLinePay,
			/// <summary>決済無し→Atone</summary>
			ChangeNoPaymentToAtone,
			/// <summary>決済無し→Aftee</summary>
			ChangeNoPaymentToAftee,
			/// <summary>決済無し→NP後払い</summary>
			ChangeNoPaymentToNPAfterPay,
			/// <summary>決済無し→PayPay</summary>
			ChangeNoPaymentToPayPay,

			/// <summary>Atone</summary>
			NoChangeAtone,
			/// <summary>Atone→クレジットカード</summary>
			ChangeAtoneToCredit,
			/// <summary>Atone→Amazon Pay</summary>
			ChangeAtoneToAmazonPay,
			/// <summary>Atone→コンビニ後払い</summary>
			ChangeAtoneToCvsDef,
			/// <summary>Change Atone To Gmo Post</summary>
			ChangeAtoneToGmoPost,
			/// <summary>Atone→代引き</summary>
			ChangeAtoneToCollectOrOthers,
			/// <summary>Atone→キャリア決済</summary>
			ChangeAtoneToCarrier,
			/// <summary>Atone→PayPal</summary>
			ChangeAtoneToPayPal,
			/// <summary>Atone→LINEPay</summary>
			ChangeAtoneToLinePay,
			/// <summary>Atone→後付款(TriLink後払い)</summary>
			ChangeAtoneToTriLinkAfterPay,
			/// <summary>Atone→決済無し</summary>
			ChangeAtoneToNoPayment,
			/// <summary>Atone→NP後払いし</summary>
			ChangeAtoneToNPAfterPay,
			/// <summary>Atone→Boku</summary>
			ChangeAtoneToBoku,
			/// <summary>Atone→全返品</summary>
			AtoneReturnAllItems,
			/// <summary>Atone→PayPay</summary>
			ChangeAtoneToPayPay,

			/// <summary>Aftee</summary>
			NoChangeAftee,
			/// <summary>Aftee→クレジットカード</summary>
			ChangeAfteeToCredit,
			/// <summary>Aftee→Amazon Pay</summary>
			ChangeAfteeToAmazonPay,
			/// <summary>Aftee→コンビニ後払い</summary>
			ChangeAfteeToCvsDef,
			/// <summary>Change Aftee To Gmo Post</summary>
			ChangeAfteeToGmoPost,
			/// <summary>Aftee→代引き</summary>
			ChangeAfteeToCollectOrOthers,
			/// <summary>Aftee→キャリア決済</summary>
			ChangeAfteeToCarrier,
			/// <summary>Aftee→PayPal</summary>
			ChangeAfteeToPayPal,
			/// <summary>Aftee→LINEPay</summary>
			ChangeAfteeToLinePay,
			/// <summary>Aftee→後付款(TriLink後払い)</summary>
			ChangeAfteeToTriLinkAfterPay,
			/// <summary>Aftee→決済無し</summary>
			ChangeAfteeToNoPayment,
			/// <summary>Aftee→NP後払いし</summary>
			ChangeAfteeToNPAfterPay,
			/// <summary>Aftee→Boku</summary>
			ChangeAfteeToBoku,
			/// <summary>Aftee→全返品</summary>
			AfteeReturnAllItems,
			/// <summary>Aftee→PayPay</summary>
			ChangeAfteeToPayPay,

			/// <summary>LINEPay</summary>
			NoChangeLinePay,
			/// <summary>LINEPay→クレジットカード</summary>
			ChangeLinePayToCredit,
			/// <summary>LINEPay→Amazon Pay</summary>
			ChangeLinePayToAmazonPay,
			/// <summary>LINEPay→コンビニ後払い</summary>
			ChangeLinePayToCvsDef,
			/// <summary>Change LinePay To Gmo Post</summary>
			ChangeLinePayToGmoPost,
			/// <summary>LINEPay→代引き</summary>
			ChangeLinePayToCollectOrOthers,
			/// <summary>LINEPay→キャリア決済</summary>
			ChangeLinePayToCarrier,
			/// <summary>LINEPay→PayPal</summary>
			ChangeLinePayToPayPal,
			/// <summary>LINEPay→後付款(TriLink後払い)</summary>
			ChangeLinePayToTriLinkAfterPay,
			/// <summary>LINEPay→Paidy Pay</summary>
			ChangeLinePayToPaidyPay,
			/// <summary>LINEPay→Aftee</summary>
			ChangeLinePayToAftee,
			/// <summary>LINEPay→Atone</summary>
			ChangeLinePayToAtone,
			/// <summary>LINEPay→決済無し</summary>
			ChangeLinePayToNoPayment,
			/// <summary>LINEPay→NP後払いし</summary>
			ChangeLinePayToNPAfterPay,
			/// <summary>LINEPay→Boku</summary>
			ChangeLinePayToBoku,
			/// <summary>LINEPay→全返品</summary>
			LinePayReturnAllItems,
			/// <summary>LINEPay→PayPay</summary>
			ChangeLinePayToPayPay,

			/// <summary>NP後払い→NP後払い</summary>
			NoChangeNPAfterPay,
			/// <summary>NP後払い→クレジットカード</summary>
			ChangeNPAfterPayToCredit,
			/// <summary>NP後払い→コンビニ後払い</summary>
			ChangeNPAfterPayToCvsDef,
			/// <summary>Change NPAfterPay To Gmo Post</summary>
			ChangeNPAfterPayToGmoPost,
			/// <summary>NP後払い→代引き</summary>
			ChangeNPAfterPayToCollectOrOthers,
			/// <summary>NP後払い→キャリア決済</summary>
			ChangeNPAfterPayToCarrier,
			/// <summary>NP後払い→PayPal</summary>
			ChangeNPAfterPayToPayPal,
			/// <summary>NP後払い→Amazon Pay</summary>
			ChangeNPAfterPayToAmazonPay,
			/// <summary>NP後払い→後付款(TriLink後払い)</summary>
			ChangeNPAfterPayToTriLinkAfterPay,
			/// <summary>NP後払い→Paidy Pay</summary>
			ChangeNPAfterPayToPaidyPay,
			/// <summary>NP後払い→決済無し</summary>
			ChangeNPAfterPayToNoPayment,
			/// <summary>NP後払い→Atone</summary>
			ChangeNPAfterPayToAtone,
			/// <summary>NP後払い→Aftee</summary>
			ChangeNPAfterPayToAftee,
			/// <summary>NP後払い→LINEPay</summary>
			ChangeNPAfterPayToLinePay,
			/// <summary>NP後払い→Boku</summary>
			ChangeNPAfterPayToBoku,
			/// <summary>NP後払い 全返品</summary>
			NPAfterPayReturnAllItems,
			/// <summary>NP後払い→PayPay</summary>
			ChangeNPAfterPayToPayPay,

			/// <summary>EcPay</summary>
			NoChangeEcPay,
			/// <summary>EcPay→クレジットカード</summary>
			ChangeEcPayToCredit,
			/// <summary>EcPay→Amazon Pay</summary>
			ChangeEcPayToAmazonPay,
			/// <summary>EcPay→コンビニ後払い</summary>
			ChangeEcPayToCvsDef,
			/// <summary>Change EcPay To Gmo Post</summary>
			ChangeEcPayToGmoPost,
			/// <summary>EcPay→代引き</summary>
			ChangeEcPayToCollectOrOthers,
			/// <summary>EcPay→キャリア決済</summary>
			ChangeEcPayToCarrier,
			/// <summary>EcPay→PayPal</summary>
			ChangeEcPayToPayPal,
			/// <summary>EcPay→後付款(TriLink後払い)</summary>
			ChangeEcPayToTriLinkAfterPay,
			/// <summary>EcPay→決済無し</summary>
			ChangeEcPayToNoPayment,
			/// <summary>EcPay→PaidyPay</summary>
			ChangeEcPayToPaidyPay,
			/// <summary>EcPay→Aftee</summary>
			ChangeEcPayToAftee,
			/// <summary>EcPay→Atone</summary>
			ChangeEcPayToAtone,
			/// <summary>EcPay→LINEPay</summary>
			ChangeEcPayToLinePay,
			/// <summary>EcPay→NP後払い</summary>
			ChangeEcPayToNPAfterPay,
			/// <summary>EcPay→Boku</summary>
			ChangeEcPayToBoku,
			/// <summary>EcPay→全返品</summary>
			EcPayReturnAllItems,
			/// <summary>EcPay→PayPay</summary>
			ChangeEcPayToPayPay,

			/// <summary>NewebPay</summary>
			NoChangeNewebPay,
			/// <summary>NewebPay→クレジットカード</summary>
			ChangeNewebPayToCredit,
			/// <summary>NewebPay→Amazon Pay</summary>
			ChangeNewebPayToAmazonPay,
			/// <summary>NewebPay→コンビニ後払い</summary>
			ChangeNewebPayToCvsDef,
			/// <summary>Change NewebPay To Gmo Post</summary>
			ChangeNewebPayToGmoPost,
			/// <summary>NewebPay→代引き</summary>
			ChangeNewebPayToCollectOrOthers,
			/// <summary>NewebPay→キャリア決済</summary>
			ChangeNewebPayToCarrier,
			/// <summary>NewebPay→PayPal</summary>
			ChangeNewebPayToPayPal,
			/// <summary>NewebPay→後付款(TriLink後払い)</summary>
			ChangeNewebPayToTriLinkAfterPay,
			/// <summary>NewebPay→決済無し</summary>
			ChangeNewebPayToNoPayment,
			/// <summary>NewebPay→PaidyPay</summary>
			ChangeNewebPayToPaidyPay,
			/// <summary>NewebPay→Aftee</summary>
			ChangeNewebPayToAftee,
			/// <summary>NewebPay→Atone</summary>
			ChangeNewebPayToAtone,
			/// <summary>NewebPay→LINEPay</summary>
			ChangeNewebPayToLinePay,
			/// <summary>NewebPay→NP後払い</summary>
			ChangeNewebPayToNPAfterPay,
			/// <summary>NewebPay→Boku</summary>
			ChangeNewebPayToBoku,
			/// <summary>NewebPay→全返品</summary>
			NewebPayReturnAllItems,
			/// <summary>NewebPay→PayPay</summary>
			ChangeNewebPayToPayPay,

			/// <summary>PayPay→PayPay</summary>
			NoChangePayPay,
			/// <summary>PayPay→クレジットカード</summary>
			ChangePayPayToCredit,
			/// <summary>PayPay→コンビニ後払い</summary>
			ChangePayPayToCvsDef,
			/// <summary>Change PayPay To Gmo Post</summary>
			ChangePayPayToGmoPost,
			/// <summary>PayPay→PayPal</summary>
			ChangePayPayToPayPal,
			/// <summary>PayPay→後付款(TriLink後払い)い</summary>
			ChangePayPayToTriLinkAfterPay,
			/// <summary>PayPay→キャリア決済</summary>
			ChangePayPayToCarrier,
			/// <summary>PayPay→代引き</summary>
			ChangePayPayToCollectOrOthers,
			/// <summary>PayPay→Amazon Pay</summary>
			ChangePayPayToAmazonPay,
			/// <summary>PayPay→PaidyPay</summary>
			ChangePayPayToPaidyPay,
			/// <summary>PayPay→決済無し</summary>
			ChangePayPayToNoPayment,
			/// <summary>PayPay→Atone</summary>
			ChangePayPayToAtone,
			/// <summary>PayPay→Aftee</summary>
			ChangePayPayToAftee,
			/// <summary>PayPay→LINEPay</summary>
			ChangePayPayToLinePay,
			/// <summary>PayPay→NP後払い</summary>
			ChangePayPayToNPAfterPay,
			/// <summary>PayPay→EcPay</summary>
			ChangePayPayToEcPay,
			/// <summary>PayPay→全返品</summary>
			PayPayReturnAllItems,

			/// <summary>Boku</summary>
			NoChangeBoku,
			/// <summary>Boku→クレジットカード</summary>
			ChangeBokuToCredit,
			/// <summary>Boku→Amazon Pay</summary>
			ChangeBokuToAmazonPay,
			/// <summary>Boku→コンビニ後払い</summary>
			ChangeBokuToCvsDef,
			/// <summary>Boku→代引き</summary>
			ChangeBokuToCollectOrOthers,
			/// <summary>Boku→キャリア決済</summary>
			ChangeBokuToCarrier,
			/// <summary>Boku→PayPal</summary>
			ChangeBokuToPayPal,
			/// <summary>Boku→後付款(TriLink後払い)</summary>
			ChangeBokuToTriLinkAfterPay,
			/// <summary>Boku→決済無し</summary>
			ChangeBokuToNoPayment,
			/// <summary>Boku→PaidyPay</summary>
			ChangeBokuToPaidyPay,
			/// <summary>Boku→Aftee</summary>
			ChangeBokuToAftee,
			/// <summary>Boku→Atone</summary>
			ChangeBokuToAtone,
			/// <summary>Boku→LINEPay</summary>
			ChangeBokuToLinePay,
			/// <summary>Boku→NP後払い</summary>
			ChangeBokuToNPAfterPay,

			/// <summary>No Change PayAsYouGo</summary>
			NoChangePayAsYouGo,
			/// <summary>Change PayAsYouGo To Credit</summary>
			ChangePayAsYouGoToCredit,
			/// <summary>Change PayAsYouGo To Carrier</summary>
			ChangePayAsYouGoToCarrier,
			/// <summary>Change PayAsYouGo To Collect Or Others</summary>
			ChangePayAsYouGoToCollectOrOthers,
			/// <summary>Change PayAsYouGo To AmazonPay</summary>
			ChangePayAsYouGoToAmazonPay,
			/// <summary>Change PayAsYouGo To PayPal</summary>
			ChangePayAsYouGoToPayPal,
			/// <summary>Change PayAsYouGo To TriLinkAfterPay</summary>
			ChangePayAsYouGoToTriLinkAfterPay,
			/// <summary>Change PayAsYouGo To NoPayment</summary>
			ChangePayAsYouGoToNoPayment,
			/// <summary>Change PayAsYouGo To PaidyPay</summary>
			ChangePayAsYouGoToPaidyPay,
			/// <summary>Change PayAsYouGo To Atone</summary>
			ChangePayAsYouGoToAtone,
			/// <summary>Change PayAsYouGo To Aftee</summary>
			ChangePayAsYouGoToAftee,
			/// <summary>Change PayAsYouGo To LinePay</summary>
			ChangePayAsYouGoToLinePay,
			/// <summary>Change PayAsYouGo To NPAfterPay</summary>
			ChangePayAsYouGoToNPAfterPay,
			/// <summary>Change PayAsYouGo To PayPay</summary>
			ChangePayAsYouGoToPayPay,
			/// <summary>Change PayAsYouGo To FramePayment</summary>
			ChangePayAsYouGoToFramePayment,

			/// <summary>No Change FramePayment</summary>
			NoChangeFramePayment,
			/// <summary>Change FramePayment To Credit</summary>
			ChangeFramePaymentToCredit,
			/// <summary>Change FramePayment To Carrier</summary>
			ChangeFramePaymentToCarrier,
			/// <summary>Change Frame Payment To Collect Or Others</summary>
			ChangeFramePaymentToCollectOrOthers,
			/// <summary>Change FramePayment To AmazonPay</summary>
			ChangeFramePaymentToAmazonPay,
			/// <summary>Change FramePayment To PayPal</summary>
			ChangeFramePaymentToPayPal,
			/// <summary>Change FramePayment To TriLink AfterPay</summary>
			ChangeFramePaymentToTriLinkAfterPay,
			/// <summary>Change FramePayment To No Payment</summary>
			ChangeFramePaymentToNoPayment,
			/// <summary>Change FramePayment To PaidyPay</summary>
			ChangeFramePaymentToPaidyPay,
			/// <summary>Change FramePayment To Atone</summary>
			ChangeFramePaymentToAtone,
			/// <summary>Change FramePayment To Aftee</summary>
			ChangeFramePaymentToAftee,
			/// <summary>Change FramePayment To LinePay</summary>
			ChangeFramePaymentToLinePay,
			/// <summary>Change FramePayment To NPAfterPay</summary>
			ChangeFramePaymentToNPAfterPay,
			/// <summary>Change FramePayment To PayPay</summary>
			ChangeFramePaymentToPayPay,
			/// <summary>Change FramePayment To PayAsYouGo</summary>
			ChangeFramePaymentToPayAsYouGo,

			/// <summary>GMOアトカラ</summary>
			NoChangeGmoAtokara,
			/// <summary>GMOアトカラ→クレジットカード</summary>
			ChangeGmoAtokaraToCredit,
			/// <summary>GMOアトカラ→Amazon Pay</summary>
			ChangeGmoAtokaraToAmazonPay,
			/// <summary>GMOアトカラ→コンビニ後払い</summary>
			ChangeGmoAtokaraToCvsDef,
			/// <summary>GMOアトカラ→代引き</summary>
			ChangeGmoAtokaraToCollectOrOthers,
			/// <summary>GMOアトカラ→キャリア決済</summary>
			ChangeGmoAtokaraToCarrier,
			/// <summary>GMOアトカラ→PayPal</summary>
			ChangeGmoAtokaraToPayPal,
			/// <summary>GMOアトカラ→後付款(TriLink後払い)</summary>
			ChangeGmoAtokaraToTriLinkAfterPay,
			/// <summary>GMOアトカラ→決済無し</summary>
			ChangeGmoAtokaraToNoPayment,
			/// <summary>GMOアトカラ→PaidyPay</summary>
			ChangeGmoAtokaraToPaidyPay,
			/// <summary>GMOアトカラ→Aftee</summary>
			ChangeGmoAtokaraToAftee,
			/// <summary>GMOアトカラ→Atone</summary>
			ChangeGmoAtokaraToAtone,
			/// <summary>GMOアトカラ→LINEPay</summary>
			ChangeGmoAtokaraToLinePay,
			/// <summary>GMOアトカラ→NP後払い</summary>
			ChangeGmoAtokaraToNPAfterPay,

			/// <summary>何もしない</summary>
			None,
		}

		/// <summary>
		/// コンストラクタ（変更前の再与信処理区分省略）
		/// </summary>
		/// <param name="orderOld">注文情報（変更前）</param>
		/// <param name="orderNew">注文情報（変更後）</param>
		/// <param name="executeType">変更後の再与信処理区分</param>
		/// <param name="orderActionType">注文処理区分</param>
		/// <param name="accessor">SQLアクセサ</param>
		public ReauthCreatorFacade(
			OrderModel orderOld,
			OrderModel orderNew,
			ExecuteTypes executeType,
			OrderActionTypes orderActionType,
			SqlAccessor accessor = null)
		{
			this.Accessor = accessor;
			this.OrderOld = orderOld;
			this.OrderNew = orderNew;
			this.OldExecuteType = null;
			this.ExecuteType = executeType;
			this.OrderActionType = orderActionType;
			this.IsReturnAllItems = InspectReturnAllItems();
			this.IsCvsDefPayComplete = CheckCsvDefPayComplete();
			this.PreCancelFlg = false;
			this.IsAuthResultHold = false;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderOld">注文情報（変更前）</param>
		/// <param name="orderNew">注文情報（変更後）</param>
		/// <param name="oldExecuteType">変更前の再与信処理区分</param>
		/// <param name="newExecuteType">変更後の再与信処理区分</param>
		/// <param name="orderActionType">注文処理区分</param>
		public ReauthCreatorFacade(
			OrderModel orderOld,
			OrderModel orderNew,
			ExecuteTypes oldExecuteType,
			ExecuteTypes newExecuteType,
			OrderActionTypes orderActionType)
		{
			this.OrderOld = orderOld;
			this.OrderNew = orderNew;
			this.OldExecuteType = oldExecuteType;
			this.ExecuteType = newExecuteType;
			this.OrderActionType = orderActionType;
			this.IsReturnAllItems = InspectReturnAllItems();
			this.IsCvsDefPayComplete = CheckCsvDefPayComplete();
			this.PreCancelFlg = false;
			this.IsAuthResultHold = false;
		}

		/// <summary>
		/// 再与信インスタンス作成
		/// </summary>
		/// <returns>再与信インスタンス</returns>
		public ReauthExecuter CreateReauth()
		{
			IReauthAction reauthAction = CreateDoNothingAction();
			IReauthAction cancelAction = CreateDoNothingAction();
			IReauthAction reduceAction = CreateDoNothingAction();
			IReauthAction refundAction = CreateDoNothingAction();
			IReauthAction updateAction = CreateDoNothingAction();
			IReauthAction reprintAction = CreateDoNothingAction();
			IReauthAction salesAction = CreateDoNothingAction();
			IReauthAction billingAction = CreateDoNothingAction();
			var reauthType = this.GetReauthType();
			switch (reauthType)
			{
				// クレジットカード変更なし
				case ReauthTypes.NoChangeCredit:
					// 強制与信
					if (this.ExecuteType == ExecuteTypes.ForcedAuth)
					{
						reauthAction = CreateReauthCreditCardOrDoNothingAction();
						cancelAction = CreateCancelCreditCardOrDoNothingAction();
						salesAction = CreateSalesCreditCardOrDoNothingAction();
					}
					// 返品・交換
					else if (this.IsChangedAmount
						&& (this.IsBilledAmountGreaterThanZero)
						&& ((this.OrderActionType == OrderActionTypes.Return)
							|| (this.OrderActionType == OrderActionTypes.Exchange)))
					{
						reauthAction = CreateReauthCreditCardOrDoNothingAction();
						cancelAction = CreateCancelCreditCardOrDoNothingAction();
						salesAction = CreateSalesCreditCardOrDoNothingAction();
					}
					// 交換注文キャンセル
					else if (this.OrderActionType == OrderActionTypes.ExchangeCancel)
					{
						reauthAction = CreateReauthCreditCardOrDoNothingAction();
						cancelAction = CreateCancelCreditCardOrDoNothingAction();
						// 最後の注文は返品注文、又は、キャンセル中の交換注文の場合は売上確定処理を行う
						if (this.OrderNew.IsReturnOrder
							|| (this.OrderNew.IsExchangeOrder
								&& (this.OrderOld.OrderId == this.OrderNew.OrderId)))
						{
							salesAction = CreateSalesCreditCardOrDoNothingAction();
						}
					}
					// 金額変更
					// または、クレジットカードを別のものに変更
					// または、クレジットカードの支払回数変更
					else if (this.IsChangedAmount
						|| this.IsNoChangeCreditButAnotherCard
						|| this.IsNoChangeCreditButInstallmentsCode)
					{
						reauthAction = CreateReauthCreditCardOrDoNothingAction();

						if ((this.IsChangedAmount == false)
							|| (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten)
							|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
								&& (this.IsBilledAmountGreaterThanZero == false)))
						{
							cancelAction = CreateCancelCreditCardOrDoNothingAction();
							if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten
								&& this.IsBilledAmountGreaterThanZero)
							{
								this.OrderNew.PaymentOrderId = OrderCommon.CreatePaymentOrderId(this.OrderNew.ShopId);
							}
						}
					}
					break;
				// クレジットカード→コンビニ後払い
				case ReauthTypes.ChangeCreditToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelCreditCardOrDoNothingAction();
					break;
				// Credit To Gmo Post
				case ReauthTypes.ChangeCreditToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelCreditCardOrDoNothingAction();
					break;
				// クレジットカード→Amazon Pay
				case ReauthTypes.ChangeCreditToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					cancelAction = CreateCancelCreditCardOrDoNothingAction();
					salesAction = CreateSalesAmazonPayOrDoNothingAction();
					break;
				// クレジットカード→Paidy Pay
				case ReauthTypes.ChangeCreditToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelCreditCardOrDoNothingAction();
					break;
				// クレジットカード→PayPal決済
				case ReauthTypes.ChangeCreditToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelCreditCardOrDoNothingAction();
					salesAction = CreateSalesCreditCardOrDoNothingAction();
					break;
				// クレジットカード→後付款(TriLink後払い)
				case ReauthTypes.ChangeCreditToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelCreditCardOrDoNothingAction();
					break;
				// クレジットカード→LINEPay
				case ReauthTypes.ChangeCreditToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelCreditCardOrDoNothingAction();
					break;
				// クレジットカード→キャリア決済
				// クレジットカード→代引き
				// クレジットカード→決済無し
				// クレジットカード全返品（最終請求金額＝0）
				// クレジットカード→Boku
				case ReauthTypes.ChangeCreditToCarrier:
				case ReauthTypes.ChangeCreditToCollectOrOthers:
				case ReauthTypes.ChangeCreditToNoPayment:
				case ReauthTypes.CreditReturnAllItems:
				case ReauthTypes.ChangeCreditToBoku:
					cancelAction = CreateCancelCreditCardOrDoNothingAction();
					break;

				// クレジットカード→Atone
				case ReauthTypes.ChangeCreditToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelCreditCardOrDoNothingAction();
					break;

				// クレジットカード→Atone
				case ReauthTypes.ChangeCreditToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelCreditCardOrDoNothingAction();
					break;

				// クレジットカード→NP後払い
				case ReauthTypes.ChangeCreditToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelCreditCardOrDoNothingAction();
					break;

				// クレジットカード→PayPay
				case ReauthTypes.ChangeCreditToPayPay:
					cancelAction = CreateCancelCreditCardOrDoNothingAction();
					break;

				// コンビニ後払い変更なし
				case ReauthTypes.NoChangeCvsDef:
					// 強制与信
					if ((this.ExecuteType == ExecuteTypes.ForcedAuth) && (this.IsAtodeneCvsDef == false))
					{
						reauthAction = CreateReauthCvsDefOrDoNothingAction();
						cancelAction = CreateCancelCvsDefOrDoNothingAction();
					}
					// アトディーネの場合
					else if (this.IsAtodeneCvsDef)
					{
						if (this.ExecuteType == ExecuteTypes.None)
						{
							// 連携なしの場合は何もしない
						}
						else if ((this.ExecuteType == ExecuteTypes.System)
							&& (this.OrderActionType == OrderActionTypes.Return))
						{
							// システム連動するかつ返品交換の時
							// 請求金額変更されており、0より多ければ再与信、そうでなければ取消し
							if (this.IsChangedAmount && this.IsBilledAmountGreaterThanZero)
							{
								reauthAction = CreateReauthAtodeneExcludingAuthExtensionDefAction(this.OrderNew.OrderId);
							}
							else
							{
								cancelAction = CreateCancelCvsDefOrDoNothingAction();
							}
						}
						else if (IsDifferentOrderId())
						{
							// 注文同梱やアップセルのときの再与信
							reauthAction = CreateReauthAtodeneExcludingAuthExtensionDefAction(this.OrderOld.OrderId);
						}
						else if (this.IsAtodeneCvsDefOrderChange())
						{
							// 変更ある場合再与信する
							reauthAction = CreateReauthAtodeneExcludingAuthExtensionDefAction(this.OrderNew.OrderId);
							// 返品交換時などでゼロ円以下の場合、キャンセルを行う
							if (this.IsBilledAmountGreaterThanZero == false)
							{
								cancelAction = CreateCancelCvsDefOrDoNothingAction();
							}
						}
					}
					// DSK後払いの場合
					else if (this.IsDskCvsDef)
					{
						if (this.ExecuteType == ExecuteTypes.None)
						{
							// 連携なしの場合は何もしない
						}
						else
						{
							if (this.IsBilledAmountGreaterThanZero || IsDifferentOrderId() || IsRecommendOrder())
							{
								reauthAction = CreateReauthCvsDefOrDoNothingAction();
								cancelAction = CreateCancelCvsDefOrDoNothingAction();
							}
							else
							{
								cancelAction = CreateCancelCvsDefOrDoNothingAction();
							}
						}
					}
					// ベリトランス後払いの場合
					else if (this.IsVeritransDef)
					{
						if (this.ExecuteType == ExecuteTypes.None)
						{
							// 連携なしの場合は何もしない
						}
						else
						{
							if (this.OrderActionType == OrderActionTypes.Modify)
							{
								if (this.IsChangedAmount && this.IsBilledAmountGreaterThanZero)
								{
									reauthAction = CreateReauthCvsDefOrDoNothingAction();
									cancelAction = CreateCancelCvsDefOrDoNothingAction();
								}
								else
								{
									updateAction = CreateUpdateVeritransAfterPayAction();
								}
							}
							else
							{
								reauthAction = CreateReauthCvsDefOrDoNothingAction();
								cancelAction = CreateCancelCvsDefOrDoNothingAction();
							}
						}
					}
					// 金額変更（増額）
					// GMOコンビニ後払い：
					//   注文情報変更がある（金額、注文者情報、配送先情報）
					// ヤマトコンビニ後払い：
					//   金額変更（増額）
					//   注文者名/電話番号変更有
					//   配送希望日変更有
					//   お届け希望日変更
					// 返品・交換：金額変更有
					else if ((this.IsGmoCvsDef && this.IsGmoCvsDefOrderChange())
						|| (this.IsAtobaraicomCvsDef && this.IsGmoCvsDefOrderChange())
						|| ((this.IsGmoCvsDef == false)
							&& (this.IsIncreasedBilledAmount || this.IsChangeOwnerNameOrTel || this.IsChangeShippingDate))
						|| (((this.OrderActionType == OrderActionTypes.Return)
							|| (this.OrderActionType == OrderActionTypes.Exchange)
							|| this.OrderActionType == OrderActionTypes.ExchangeCancel) && this.IsChangedAmount))
					{
						reauthAction = CreateReauthCvsDefOrDoNothingAction();
						cancelAction = CreateCancelCvsDefOrDoNothingAction();
					}
					// スコア後払いの場合
					else if (this.IsScoreCvsDef)
					{
						if (this.ExecuteType == ExecuteTypes.None)
						{
							// 連携なしの場合は何もしない
						}
						else
						{
							if (this.OrderActionType == OrderActionTypes.Modify)
							{
								updateAction = CreateUpdateScoreAfterPayAction();
							}
							else
							{
								reauthAction = CreateReauthCvsDefOrDoNothingAction();
								cancelAction = CreateCancelCvsDefOrDoNothingAction();
							}
						}
					}
					// 金額変更（減額）
					else if (this.IsReducedBilledAmount)
					{
						reduceAction = CreateReduceCvsDefOrDoNothingAction();
					}
					break;

				// コンビニ後払い→クレジットカード
				case ReauthTypes.ChangeCvsDefToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelCvsDefOrDoNothingAction();
					salesAction = CreateSalesCreditCardOrDoNothingAction();
					break;
				// コンビニ後払い→Amazon Pay
				case ReauthTypes.ChangeCvsDefToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					cancelAction = CreateCancelCvsDefOrDoNothingAction();
					salesAction = CreateSalesAmazonPayOrDoNothingAction();
					break;
				// コンビニ後払い→Paidy Pay
				case ReauthTypes.ChangeCvsDefToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelCvsDefOrDoNothingAction();
					break;
				// コンビニ後払い→PayPal決済
				case ReauthTypes.ChangeCvsDefToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelCvsDefOrDoNothingAction();
					salesAction = CreateSalesPayPalOrDoNothingAction();
					break;
				// コンビニ後払い→後付款(TriLink後払い)
				case ReauthTypes.ChangeCvsDefToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelCvsDefOrDoNothingAction();
					break;
				// コンビニ後払い→LINEPay
				case ReauthTypes.ChangeCvsDefToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelCvsDefOrDoNothingAction();
					break;
				// コンビニ後払い→キャリア決済
				// コンビニ後払い→代引き
				// コンビニ後払い→決済無し
				// コンビニ後払い全返品（最終請求金額＝0）
				// コンビニ後払い→PayPay
				// コンビニ後払い→Boku
				case ReauthTypes.ChangeCvsDefToCarrier:
				case ReauthTypes.ChangeCvsDefToCollectOrOthers:
				case ReauthTypes.ChangeCvsDefToNoPayment:
				case ReauthTypes.CvsDefReturnAllItems:
				case ReauthTypes.ChangeCvsDefToPayPay:
				case ReauthTypes.ChangeCvsDefToBoku:
					cancelAction = CreateCancelCvsDefOrDoNothingAction();
					break;

				// コンビニ後払い→Atone
				case ReauthTypes.ChangeCvsDefToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelCvsDefOrDoNothingAction();
					break;

				// コンビニ後払い→Aftee
				case ReauthTypes.ChangeCvsDefToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelCvsDefOrDoNothingAction();
					break;

				// コンビニ後払い→NP後払い
				case ReauthTypes.ChangeCvsDefToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelCvsDefOrDoNothingAction();
					break;

				// Change PayAsYouGo To Credit
				case ReauthTypes.ChangePayAsYouGoToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change PayAsYouGo To AmazonPay
				case ReauthTypes.ChangePayAsYouGoToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change PayAsYouGo To PaidyPay
				case ReauthTypes.ChangePayAsYouGoToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change PayAsYouGo To PayPal
				case ReauthTypes.ChangePayAsYouGoToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change PayAsYouGo To TriLink AfterPay
				case ReauthTypes.ChangePayAsYouGoToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change PayAsYouGo To LinePay
				case ReauthTypes.ChangePayAsYouGoToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change PayAsYouGo To FramePayment
				case ReauthTypes.ChangePayAsYouGoToFramePayment:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change PayAsYouGo To Carrier
				// Change PayAsYouGo To CollectOrOthers
				// Change PayAsYouGo To No Payment
				// Change PayAsYouGo To PayPay
				// PayAsYouGo Return All Items
				case ReauthTypes.ChangePayAsYouGoToCarrier:
				case ReauthTypes.ChangePayAsYouGoToCollectOrOthers:
				case ReauthTypes.ChangePayAsYouGoToNoPayment:
				case ReauthTypes.ChangePayAsYouGoToPayPay:
				case ReauthTypes.PayAsYouGoReturnAllItems:
					if (this.OrderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
					{
						billingAction = CreateModifyCancelBillingGmoPostOrDoNothingAction(isReturnAll: true);
					}
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Return or Exchange PayAsYouGo
				case ReauthTypes.NoChangePayAsYouGo:
					if (this.IsChangedAmount
						&& (this.IsBilledAmountGreaterThanZero)
						&& (this.OrderActionType == OrderActionTypes.Exchange))
					{
						// cannot exchange
						break;
					}
					else if (this.IsChangedAmount
						&& (this.IsBilledAmountGreaterThanZero)
						&& (this.OrderActionType == OrderActionTypes.Return))
					{
						reduceAction = CreateReduceGmoPostOrDoNothingAction();
					}
					else if (this.IsChangedAmount
						&& (this.IsBilledAmountGreaterThanZero)
						&& (this.OrderActionType == OrderActionTypes.Modify))
					{
						if (this.OrderOld.IsAlreadyShipped == false)
						{
							updateAction = CreateEditGmoPostOrDoNothingAction();
						}
					}
					else 
					{
						updateAction = CreateEditGmoPostOrDoNothingAction();
					}
					break;

				// Change PayAsYouGo To Atone
				case ReauthTypes.ChangePayAsYouGoToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;

				// Change PayAsYouGo To Aftee
				case ReauthTypes.ChangePayAsYouGoToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;

				// Change PayAsYouGo To NPAfterPay
				case ReauthTypes.ChangePayAsYouGoToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;

				// Change FramePayment To Credit
				case ReauthTypes.ChangeFramePaymentToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change FramePayment To AmazonPay
				case ReauthTypes.ChangeFramePaymentToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change FramePayment To PaidyPay
				case ReauthTypes.ChangeFramePaymentToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change FramePayment To PayPal
				case ReauthTypes.ChangeFramePaymentToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change FramePayment To TriLink AfterPay
				case ReauthTypes.ChangeFramePaymentToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change FramePayment To LinePay
				case ReauthTypes.ChangeFramePaymentToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change FramePayment To PayAsYouGo
				case ReauthTypes.ChangeFramePaymentToPayAsYouGo:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// Change FramePayment To Carrier
				// Change FramePayment To CollectO r Others
				// Change FramePayment To No Payment
				// Change FramePayment To PayPay
				// FramePayment Return All Items
				case ReauthTypes.ChangeFramePaymentToCarrier:
				case ReauthTypes.ChangeFramePaymentToCollectOrOthers:
				case ReauthTypes.ChangeFramePaymentToNoPayment:
				case ReauthTypes.ChangeFramePaymentToPayPay:
				case ReauthTypes.FramePaymentReturnAllItems:
					if (this.OrderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
					{
						billingAction = CreateModifyCancelBillingGmoPostOrDoNothingAction(isReturnAll: true);
					}
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;
				// No Change FramePayment
				case ReauthTypes.NoChangeFramePayment:
					if (this.IsChangedAmount
						&& (this.IsBilledAmountGreaterThanZero)
						&& (this.OrderActionType == OrderActionTypes.Exchange))
					{
						// cannot exchange
						break;
					}
					else if (this.IsChangedAmount
						&& (this.IsBilledAmountGreaterThanZero)
						&& (this.OrderActionType == OrderActionTypes.Return))
					{
						reduceAction = CreateReduceGmoPostOrDoNothingAction();
					}
					else if (this.IsChangedAmount
						&& (this.IsBilledAmountGreaterThanZero)
						&& (this.OrderActionType == OrderActionTypes.Modify))
					{
						if (this.OrderOld.IsAlreadyShipped == false)
						{
							updateAction = CreateEditGmoPostOrDoNothingAction();
						}
					}
					else
					{
						updateAction = CreateEditGmoPostOrDoNothingAction();
					}
					break;

				// Change FramePayment To Atone
				case ReauthTypes.ChangeFramePaymentToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;

				// Change FramePayment To Aftee
				case ReauthTypes.ChangeFramePaymentToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;

				// Change FramePayment To NPAfterPay
				case ReauthTypes.ChangeFramePaymentToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelGmoPostOrDoNothingAction();
					break;

				// PayPal変更なし
				case ReauthTypes.NoChangePayPal:
					// 強制与信
					if (this.ExecuteType == ExecuteTypes.ForcedAuth)
					{
						reauthAction = CreateReauthPayPalOrDoNothingAction();
						cancelAction = CreateCancelPayPalOrDoNothingAction();
						salesAction = CreateSalesPayPalOrDoNothingAction();
					}
					// 返品・交換
					else if (this.IsChangedAmount
						&& (this.IsBilledAmountGreaterThanZero)
						&& ((this.OrderActionType == OrderActionTypes.Return)
							|| (this.OrderActionType == OrderActionTypes.Exchange)))
					{
						reauthAction = CreateReauthPayPalOrDoNothingAction();
						cancelAction = CreateCancelPayPalOrDoNothingAction();
						salesAction = CreateSalesPayPalOrDoNothingAction();
					}
					// 交換注文キャンセル
					else if (this.OrderActionType == OrderActionTypes.ExchangeCancel)
					{
						reauthAction = CreateReauthPayPalOrDoNothingAction();
						cancelAction = CreateCancelPayPalOrDoNothingAction();
						// 最後の注文は返品注文、又は、キャンセル中の交換注文の場合は売上確定処理を行う
						if (this.OrderNew.IsReturnOrder
							|| (this.OrderNew.IsExchangeOrder
								&& (this.OrderOld.OrderId == this.OrderNew.OrderId)))
						{
							salesAction = CreateSalesPayPalOrDoNothingAction();
						}
					}
					// 金額変更（増額） or 配送先変更　※減額は与信立てるときに下げられるっぽい
					else if (this.IsIncreasedBilledAmount || this.IsChangeShippingInfo)
					{
						reauthAction = CreateReauthPayPalOrDoNothingAction();
						cancelAction = CreateCancelPayPalOrDoNothingAction();
					}
					break;
				// PayPal→クレジットカード
				case ReauthTypes.ChangePayPalToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelPayPalOrDoNothingAction();
					salesAction = CreateSalesCreditCardOrDoNothingAction();
					break;
				// PayPal→コンビニ後払い
				case ReauthTypes.ChangePayPalToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelPayPalOrDoNothingAction();
					break;
				// PayPal To Gmo Post
				case ReauthTypes.ChangePayPalToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelPayPalOrDoNothingAction();
					break;
				// PayPal→Amazon Pay
				case ReauthTypes.ChangePayPalToAmazonPay:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					break;
				// PayPal→Paidy Pay
				case ReauthTypes.ChangePayPalToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelPayPalOrDoNothingAction();
					break;
				// PayPal→後付款(TriLink後払い)
				case ReauthTypes.ChangePayPalToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelPayPalOrDoNothingAction();
					break;
				// PayPal→LINEPay
				case ReauthTypes.ChangePayPalToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelPayPalOrDoNothingAction();
					break;
				// PayPal→キャリア決済
				// PayPal→代引き
				// PayPal→決済無し
				// PayPal全返品（最終請求金額＝0）
				// PayPal→PayPay
				// PayPal→Boku
				case ReauthTypes.ChangePayPalToCarrier:
				case ReauthTypes.ChangePayPalToCollectOrOthers:
				case ReauthTypes.ChangePayPalToNoPayment:
				case ReauthTypes.PayPalReturnAllItems:
				case ReauthTypes.ChangePayPalToPayPay:
				case ReauthTypes.ChangePayPalToBoku:
					cancelAction = CreateCancelPayPalOrDoNothingAction();
					break;

				// PayPal→Atone
				case ReauthTypes.ChangePayPalToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelPayPalOrDoNothingAction();
					break;

				// PayPal→Aftee
				case ReauthTypes.ChangePayPalToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelPayPalOrDoNothingAction();
					break;

				// PayPal→NP後払い
				case ReauthTypes.ChangePayPalToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelPayPalOrDoNothingAction();
					break;

				// 後付款(TriLink後払い)変更なし
				case ReauthTypes.NoChangeTriLinkAfterPay:
					// 強制与信
					if (this.ExecuteType == ExecuteTypes.ForcedAuth)
					{
						reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
						cancelAction = CreateCancelTriLinkAfterPayOrDoNothingAction();
					}
					// 返品・交換
					else if (this.IsChangedAmount
						&& (this.IsBilledAmountGreaterThanZero)
						&& ((this.OrderActionType == OrderActionTypes.Return)
							|| (this.OrderActionType == OrderActionTypes.Exchange)
							|| (this.OrderActionType == OrderActionTypes.ExchangeCancel)))
					{
						reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
						cancelAction = CreateCancelTriLinkAfterPayOrDoNothingAction();
					}
					// 住所変更または金額変更
					else if (this.IsChangeShippingAddress
						|| this.IsIncreasedBilledAmount
						|| this.IsReducedBilledAmount)
					{
						updateAction = CreateReduceTriLinkAfterPayOrDoNothingAction();
					}
					break;
				// 後付款(TriLink後払い)→クレジットカード
				case ReauthTypes.ChangeTriLinkAfterPayToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelTriLinkAfterPayOrDoNothingAction();
					salesAction = CreateSalesCreditCardOrDoNothingAction();
					break;
				// 後付款(TriLink後払い)→コンビニ後払い
				case ReauthTypes.ChangeTriLinkAfterPayToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelTriLinkAfterPayOrDoNothingAction();
					break;
				// TriLink To Gmo Post
				case ReauthTypes.ChangeTriLinkAfterPayToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelTriLinkAfterPayOrDoNothingAction();
					break;
				// 後付款(TriLink後払い)→PayPal決済
				case ReauthTypes.ChangeTriLinkAfterPayToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelTriLinkAfterPayOrDoNothingAction();
					salesAction = CreateSalesPayPalOrDoNothingAction();
					break;
				// 後付款(TriLink後払い)→Paidy Pay
				case ReauthTypes.ChangeTriLinkAfterPayToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelTriLinkAfterPayOrDoNothingAction();
					break;
				// 後付款(TriLink後払い)→LINEPay
				case ReauthTypes.ChangeTriLinkAfterPayToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelTriLinkAfterPayOrDoNothingAction();
					break;
				// 後付款(TriLink後払い)→キャリア決済
				// 後付款(TriLink後払い)→代引き
				// 後付款(TriLink後払い)→決済無し
				// 後付款(TriLink後払い)全返品（最終請求金額＝0）
				// 後付款(TriLink後払い)→PayPay
				// 後付款(TriLink後払い)→Boku
				case ReauthTypes.ChangeTriLinkAfterPayToCarrier:
				case ReauthTypes.ChangeTriLinkAfterPayToCollectOrOthers:
				case ReauthTypes.ChangeTriLinkAfterPayToNoPayment:
				case ReauthTypes.TriLinkAfterPayReturnAllItems:
				case ReauthTypes.ChangeTriLinkAfterPayToPayPay:
				case ReauthTypes.ChangeTriLinkAfterPayToBoku:
					cancelAction = CreateCancelTriLinkAfterPayOrDoNothingAction();
					break;

				// 後付款(TriLink後払い)→Atone
				case ReauthTypes.ChangeTriLinkAfterPayToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelTriLinkAfterPayOrDoNothingAction();
					break;

				// 後付款(TriLink後払い)→Aftee
				case ReauthTypes.ChangeTriLinkAfterPayToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelTriLinkAfterPayOrDoNothingAction();
					break;

				// 後付款(TriLink後払い)→NP後払い
				case ReauthTypes.ChangeTriLinkAfterPayToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelTriLinkAfterPayOrDoNothingAction();
					break;

				// キャリア決済→クレジットカード
				case ReauthTypes.ChangeCarrierToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					salesAction = CreateSalesCreditCardOrDoNothingAction();
					cancelAction = CreateCancelSBPSMultiPaymentOrDoNothingAction();
					break;
				// キャリア決済→コンビニ後払い
				case ReauthTypes.ChangeCarrierToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelSBPSMultiPaymentOrDoNothingAction();
					break;
				// Carrier To Gmo Post
				case ReauthTypes.ChangeCarrierToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelSBPSMultiPaymentOrDoNothingAction();
					break;
				// キャリア決済→後付款(TriLink後払い)
				case ReauthTypes.ChangeCarrierToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelSBPSMultiPaymentOrDoNothingAction();
					break;
				// キャリア決済→代引き
				// キャリア決済→決済無し
				// キャリア決済→PayPay
				// キャリア決済→Boku
				case ReauthTypes.ChangeCarrierToCollectOrOthers:
				case ReauthTypes.ChangeCarrierToNoPayment:
				case ReauthTypes.ChangeCarrierToPayPay:
				case ReauthTypes.ChangeCarrierToBoku:
					cancelAction = CreateCancelSBPSMultiPaymentOrDoNothingAction();
					break;
				// キャリア決済→Amazon Pay
				case ReauthTypes.ChangeCarrierToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					cancelAction = CreateCancelSBPSMultiPaymentOrDoNothingAction();
					salesAction = CreateSalesAmazonPayOrDoNothingAction();
					break;
				// キャリア決済→Paidy Pay
				case ReauthTypes.ChangeCarrierToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelSBPSMultiPaymentOrDoNothingAction();
					break;
				// キャリア決済→PayPal決済
				case ReauthTypes.ChangeCarrierToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelSBPSMultiPaymentOrDoNothingAction();
					salesAction = CreateSalesPayPalOrDoNothingAction();
					break;
				// キャリア決済→LINEPay
				case ReauthTypes.ChangeCarrierToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelSBPSMultiPaymentOrDoNothingAction();
					break;

				// キャリア決済→Atone
				case ReauthTypes.ChangeCarrierToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelSBPSMultiPaymentOrDoNothingAction();
					break;

				// キャリア決済→Aftee
				case ReauthTypes.ChangeCarrierToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelSBPSMultiPaymentOrDoNothingAction();
					break;

				// キャリア決済→NP後払い
				case ReauthTypes.ChangeCarrierToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelSBPSMultiPaymentOrDoNothingAction();
					break;

				// 代引き→クレジットカード
				case ReauthTypes.ChangeCollectOrOthersToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					salesAction = CreateSalesCreditCardOrDoNothingAction();
					break;
				// 代引き→コンビニ後払い
				case ReauthTypes.ChangeCollectOrOthersToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					break;
				// Collect Or Others To Gmo Post
				case ReauthTypes.ChangeCollectOrOthersToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					break;
				// 代引き→AmazonPay
				case ReauthTypes.ChangeCollectOrOthersToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					salesAction = CreateSalesAmazonPayOrDoNothingAction();
					break;
				// 代引き→PayPal決済
				case ReauthTypes.ChangeCollectOrOthersToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					salesAction = CreateSalesPayPalOrDoNothingAction();
					break;
				// 代引き→後付款(TriLink後払い)
				case ReauthTypes.ChangeCollectOrOthersToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					break;
				// 代引き→PaidyPay
				case ReauthTypes.ChangeCollectOrOthersToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					break;
				// 代引き→LINEPay
				case ReauthTypes.ChangeCollectOrOthersToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					break;

				// 代引き→Atone
				case ReauthTypes.ChangeCollectOrOthersToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					break;

				// 代引き→Aftee
				case ReauthTypes.ChangeCollectOrOthersToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					break;

				// 代引き→NP後払い
				case ReauthTypes.ChangeCollectOrOthersToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					break;

				// AmazonPay変更なし
				case ReauthTypes.NoChangeAmazonPay:
					// 強制与信
					if (this.ExecuteType == ExecuteTypes.ForcedAuth)
					{
						reauthAction = CreateReauthAmazonPayOrDoNothingAction();
						cancelAction = CreateCancelAmazonPayOrDoNothingAction(
							isReauth: reauthAction.GetActionType() == ActionTypes.Reauth);
					}
					// 返品・交換
					else if (this.IsChangedAmount
						&& ((this.OrderActionType == OrderActionTypes.Return)
							|| (this.OrderActionType == OrderActionTypes.Exchange)))
					{
						reauthAction = CreateReauthAmazonPayOrDoNothingAction();
						cancelAction = CreateCancelAmazonPayOrDoNothingAction(
							isReauth: reauthAction.GetActionType() == ActionTypes.Reauth);
						refundAction = CreateRefundAmazonPayOrDoNothingAction();
						salesAction = CreateSalesAmazonPayOrDoNothingAction();
					}
					// 金額変更(増額)
					// または別の決済注文IDに変更
					else if (this.IsIncreasedBilledAmount
							|| IsChangeAmazonPaymentOrderId)
					{
						reauthAction = CreateReauthAmazonPayOrDoNothingAction();
						cancelAction = CreateCancelAmazonPayOrDoNothingAction(
							isReauth: reauthAction.GetActionType() == ActionTypes.Reauth);
						salesAction = CreateSalesAmazonPayOrDoNothingAction();
					}
					// 金額変更(減額)
					else if (this.IsReducedBilledAmount)
					{
						reauthAction = CreateReauthAmazonPayOrDoNothingAction();
						cancelAction = CreateCancelAmazonPayOrDoNothingAction(
							isReauth: reauthAction.GetActionType() == ActionTypes.Reauth);
						refundAction = CreateRefundAmazonPayOrDoNothingAction();
					}
					break;
				// AmazonPay→クレジットカード
				case ReauthTypes.ChangeAmazonPayToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					break;
				// AmazonPay→コンビニ後払い
				case ReauthTypes.ChangeAmazonPayToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					break;
				// AmazonPay To Gmo Post
				case ReauthTypes.ChangeAmazonPayToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					break;
				// AmazonPay→PayPal
				case ReauthTypes.ChangeAmazonPayToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					break;
				// AmazonPay→Paidy Pay
				case ReauthTypes.ChangeAmazonPayToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					break;
				// AmazonPay→後付款(TriLink後払い)
				case ReauthTypes.ChangeAmazonPayToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					break;
				// AmazonPay全返品（最終請求金額＝0）
				case ReauthTypes.AmazonPayReturnAllItems:
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					refundAction = CreateRefundAmazonPayOrDoNothingAction();
					break;
				// AmazonPay→LINEPay
				case ReauthTypes.ChangeAmazonPayToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					break;
				// AmazonPay→キャリア決済
				// AmazonPay→代引き
				// AmazonPay→決済無し
				// AmazonPay→PayPay
				// AmazonPay→Boku
				case ReauthTypes.ChangeAmazonPayToCarrier:
				case ReauthTypes.ChangeAmazonPayToCollectOrOthers:
				case ReauthTypes.ChangeAmazonPayToNoPayment:
				case ReauthTypes.ChangeAmazonPayToPayPay:
				case ReauthTypes.ChangeAmazonPayToBoku:
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					break;
				// AmazonPay→Atone
				case ReauthTypes.ChangeAmazonPayToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					break;
				// AmazonPay→Aftee
				case ReauthTypes.ChangeAmazonPayToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					break;
				// AmazonPay→NP後払い
				case ReauthTypes.ChangeAmazonPayToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelAmazonPayOrDoNothingAction();
					break;

				// 決済無し→クレジットカード
				case ReauthTypes.ChangeNoPaymentToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					salesAction = CreateSalesCreditCardOrDoNothingAction();
					break;
				// 決済無し→コンビニ後払い
				case ReauthTypes.ChangeNoPaymentToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					break;
				// No Payment To Gmo Post
				case ReauthTypes.ChangeNoPaymentToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					break;
				// 決済無し→Amazon Pay
				case ReauthTypes.ChangeNoPaymentToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					salesAction = CreateSalesAmazonPayOrDoNothingAction();
					break;
				// 決済無し→Paidy Pay
				case ReauthTypes.ChangeNoPaymentToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					break;
				// キャリア決済→PayPal決済
				case ReauthTypes.ChangeNoPaymentToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					salesAction = CreateSalesPayPalOrDoNothingAction();
					break;
				// 決済無し→後付款(TriLink後払い)
				case ReauthTypes.ChangeNoPaymentToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					break;
				// 決済無し→Atone
				case ReauthTypes.ChangeNoPaymentToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					break;
				// 決済無し→Aftee
				case ReauthTypes.ChangeNoPaymentToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					break;
				// 決済無し→LINEPay
				case ReauthTypes.ChangeNoPaymentToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					break;
				// 決済無し→NP後払い
				case ReauthTypes.ChangeNoPaymentToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					break;

				// PaidyPay変更なし
				case ReauthTypes.NoChangePaidyPay:
					// 強制与信
					if (this.ExecuteType == ExecuteTypes.ForcedAuth)
					{
						reauthAction = CreateReauthPaidyPayOrDoNothingAction();
						cancelAction = CreateCancelPaidyPayOrDoNothingAction();
						salesAction = CreateSalesPaidyPayOrDoNothingAction();
					}
					// 返品・交換
					else if (this.IsChangedAmount
						&& (this.IsBilledAmountGreaterThanZero)
						&& ((this.OrderActionType == OrderActionTypes.Return)
							|| (this.OrderActionType == OrderActionTypes.Exchange)))
					{
						if (this.IsReturnAllItems == false)
						{
							reauthAction = CreateReauthPaidyPayOrDoNothingAction();
							cancelAction = CreateCancelPaidyPayOrDoNothingAction();
							salesAction = CreateSalesPaidyPayOrDoNothingAction();
						}
						else
						{
							refundAction = CreateRefundPaidyPayOrDoNothingAction();
						}
					}
					else if (this.IsChangedAmount
						|| this.IsChangeShippingAddress
						|| this.IsChangeOwnerAddress)
					{
						reauthAction = CreateReauthPaidyPayOrDoNothingAction();
						cancelAction = CreateCancelPaidyPayOrDoNothingAction();
					}
					break;
				// PaidyPay→クレジットカード
				case ReauthTypes.ChangePaidyPayToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelPaidyPayOrDoNothingAction();
					break;
				// PaidyPay→コンビニ後払い
				case ReauthTypes.ChangePaidyPayToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelPaidyPayOrDoNothingAction();
					break;
				// PaidyPay To Gmo Post
				case ReauthTypes.ChangePaidyPayToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelPaidyPayOrDoNothingAction();
					break;
				// PaidyPay→PayPal
				case ReauthTypes.ChangePaidyPayToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelPaidyPayOrDoNothingAction();
					break;
				// 決済無し→Amazon Pay
				case ReauthTypes.ChangePaidyPayToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					cancelAction = CreateCancelPaidyPayOrDoNothingAction();
					break;
				// PaidyPay→後付款(TriLink後払い)
				case ReauthTypes.ChangePaidyPayToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelPaidyPayOrDoNothingAction();
					break;
				// PaidyPay→LinePay
				case ReauthTypes.ChangePaidyPayToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelPaidyPayOrDoNothingAction();
					break;
				// PaidyPay→NP後払い
				case ReauthTypes.ChangePaidyPayToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelPaidyPayOrDoNothingAction();
					break;
				// PaidyPay→キャリア決済
				// PaidyPay→代引き
				// PaidyPay→決済無し
				// PaidyPay→PayPay
				// PaidyPay→Boku
				case ReauthTypes.ChangePaidyPayToCarrier:
				case ReauthTypes.ChangePaidyPayToCollectOrOthers:
				case ReauthTypes.ChangePaidyPayToNoPayment:
				case ReauthTypes.ChangePaidyPayToBoku:
				case ReauthTypes.PaidyPayReturnAllItems:
				case ReauthTypes.ChangePaidyPayToPayPay:
					cancelAction = CreateCancelPaidyPayOrDoNothingAction();
					break;

				// No change Atone
				case ReauthTypes.NoChangeAtone:
					if (this.ExecuteType == ExecuteTypes.ForcedAuth)
					{
						reauthAction = CreateReauthAtoneOrDoNothingAction();
						cancelAction = CreateCancelAtoneOrDoNothingAction();
					}
					// 返品・交換
					else if ((this.OrderActionType == OrderActionTypes.Return)
						|| (this.OrderActionType == OrderActionTypes.Exchange))
					{
						reauthAction = CreateReauthAtoneOrDoNothingAction();
						cancelAction = CreateCancelAtoneOrDoNothingAction();
					}
					else if (this.IsChangedAmount
						|| this.IsChangeOrderInfo
						|| this.IsChangeProductInfomation)
					{
						reauthAction = CreateReauthAtoneOrDoNothingAction();
						cancelAction = CreateCancelAtoneOrDoNothingAction();
					}
					break;

				// Atone→クレジットカード
				case ReauthTypes.ChangeAtoneToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelAtoneOrDoNothingAction();
					break;
				// Atone→コンビニ後払い
				case ReauthTypes.ChangeAtoneToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelAtoneOrDoNothingAction();
					break;
				// Atone To Gmo Post
				case ReauthTypes.ChangeAtoneToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelAtoneOrDoNothingAction();
					break;
				// Atone→PayPal
				case ReauthTypes.ChangeAtoneToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelAtoneOrDoNothingAction();
					break;
				// Atone→Line Pay
				case ReauthTypes.ChangeAtoneToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelAtoneOrDoNothingAction();
					break;
				// Atone→Amazon Pay
				case ReauthTypes.ChangeAtoneToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					cancelAction = CreateCancelAtoneOrDoNothingAction();
					break;
				// Atone→後付款(TriLink後払い)
				case ReauthTypes.ChangeAtoneToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelAtoneOrDoNothingAction();
					break;
				// Atone→NP後払い
				case ReauthTypes.ChangeAtoneToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelAtoneOrDoNothingAction();
					break;
				// Atone→キャリア決済
				// Atone→代引き
				// Atone→決済無し
				// Atone→PayPay
				// Atone→Boku
				case ReauthTypes.ChangeAtoneToCarrier:
				case ReauthTypes.ChangeAtoneToCollectOrOthers:
				case ReauthTypes.ChangeAtoneToNoPayment:
				case ReauthTypes.ChangeAtoneToBoku:
				case ReauthTypes.AtoneReturnAllItems:
				case ReauthTypes.ChangeAtoneToPayPay:
					cancelAction = CreateCancelAtoneOrDoNothingAction();
					break;

				// No change Aftee
				case ReauthTypes.NoChangeAftee:
					if (this.ExecuteType == ExecuteTypes.ForcedAuth)
					{
						reauthAction = CreateReauthAfteeOrDoNothingAction();
						cancelAction = CreateCancelAfteeOrDoNothingAction();
					}
					// 返品・交換
					else if ((this.OrderActionType == OrderActionTypes.Return)
						|| (this.OrderActionType == OrderActionTypes.Exchange))
					{
						reauthAction = CreateReauthAfteeOrDoNothingAction();
						cancelAction = CreateCancelAfteeOrDoNothingAction();
					}
					else if (this.IsChangedAmount
						|| this.IsChangeOrderInfo
						|| this.IsChangeProductInfomation)
					{
						reauthAction = CreateReauthAfteeOrDoNothingAction();
						cancelAction = CreateCancelAfteeOrDoNothingAction();
					}
					break;

				// Aftee→クレジットカード
				case ReauthTypes.ChangeAfteeToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelAfteeOrDoNothingAction();
					break;
				// Aftee→コンビニ後払い
				case ReauthTypes.ChangeAfteeToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelAfteeOrDoNothingAction();
					break;
				// Aftee To Gmo Post
				case ReauthTypes.ChangeAfteeToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelAfteeOrDoNothingAction();
					break;
				// Aftee→PayPal
				case ReauthTypes.ChangeAfteeToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelAfteeOrDoNothingAction();
					break;
				// Aftee→Line Pay
				case ReauthTypes.ChangeAfteeToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelAfteeOrDoNothingAction();
					break;
				// Aftee→Amazon Pay
				case ReauthTypes.ChangeAfteeToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					cancelAction = CreateCancelAfteeOrDoNothingAction();
					break;
				// Aftee→後付款(TriLink後払い)
				case ReauthTypes.ChangeAfteeToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelAfteeOrDoNothingAction();
					break;
				// Aftee→NP後払い
				case ReauthTypes.ChangeAfteeToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelAfteeOrDoNothingAction();
					break;
				// Aftee→キャリア決済
				// Aftee→代引き
				// Aftee→決済無し
				// Aftee→PayPay
				// Aftee→Boku
				case ReauthTypes.ChangeAfteeToCarrier:
				case ReauthTypes.ChangeAfteeToCollectOrOthers:
				case ReauthTypes.ChangeAfteeToNoPayment:
				case ReauthTypes.ChangeAfteeToBoku:
				case ReauthTypes.AfteeReturnAllItems:
				case ReauthTypes.ChangeAfteeToPayPay:
					cancelAction = CreateCancelAfteeOrDoNothingAction();
					break;

				// No change Line Pay
				case ReauthTypes.NoChangeLinePay:
					// 強制与信
					if (this.ExecuteType == ExecuteTypes.ForcedAuth)
					{
						reauthAction = CreateReauthLinePayOrDoNothingAction();
						cancelAction = CreateCancelLinePayOrDoNothingAction();
					}
					// 返品・交換
					else if (this.IsChangedAmount
						&& ((this.OrderActionType == OrderActionTypes.Return)
							|| (this.OrderActionType == OrderActionTypes.Exchange)))
					{
						reauthAction = CreateReauthLinePayOrDoNothingAction();
						salesAction = CreateSalesLinePayOrDoNothingAction();
						refundAction = CreateRefundLinePayOrDoNothingAction();
					}
					else if (this.IsChangedAmount)
					{
						reauthAction = CreateReauthLinePayOrDoNothingAction();
						cancelAction = CreateCancelLinePayOrDoNothingAction();
					}
					break;

				// LINEPay→クレジットカード
				case ReauthTypes.ChangeLinePayToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelLinePayOrDoNothingAction();
					break;
				// LINEPay→コンビニ後払い
				case ReauthTypes.ChangeLinePayToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelLinePayOrDoNothingAction();
					break;
				// LinePay To Gmo Post
				case ReauthTypes.ChangeLinePayToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelLinePayOrDoNothingAction();
					break;
				// LINEPay→PayPal
				case ReauthTypes.ChangeLinePayToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelLinePayOrDoNothingAction();
					break;
				// LINEPay→Atone
				case ReauthTypes.ChangeLinePayToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelLinePayOrDoNothingAction();
					break;
				// LINEPay→Aftee
				case ReauthTypes.ChangeLinePayToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelLinePayOrDoNothingAction();
					break;
				// LINEPay→Amazon Pay
				case ReauthTypes.ChangeLinePayToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					cancelAction = CreateCancelLinePayOrDoNothingAction();
					break;
				// LINEPay→後付款(TriLink後払い)
				case ReauthTypes.ChangeLinePayToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelLinePayOrDoNothingAction();
					break;
				// LINEPay→Paidy Pay
				case ReauthTypes.ChangeLinePayToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelLinePayOrDoNothingAction();
					break;
				// LINEPay→NP後払い
				case ReauthTypes.ChangeLinePayToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelLinePayOrDoNothingAction();
					break;
				// LINEPay→キャリア決済
				// LINEPay→代引き
				// LINEPay→決済無し
				// LINEPay→PayPay
				// LINEPay→Boku
				case ReauthTypes.ChangeLinePayToCarrier:
				case ReauthTypes.ChangeLinePayToCollectOrOthers:
				case ReauthTypes.ChangeLinePayToNoPayment:
				case ReauthTypes.ChangeLinePayToBoku:
				case ReauthTypes.LinePayReturnAllItems:
				case ReauthTypes.ChangeLinePayToPayPay:
					cancelAction = CreateCancelLinePayOrDoNothingAction();
					break;

				// NP後払い 変更なし
				case ReauthTypes.NoChangeNPAfterPay:
					// 強制与信
					if (this.ExecuteType == ExecuteTypes.ForcedAuth)
					{
						reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
						cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
						updateAction = CreateUpdateNPAfterPayOrDoNothingAction();
					}
					// 返品・交換
					else if (this.IsChangedAmount
						&& ((this.OrderActionType == OrderActionTypes.Return)
							|| (this.OrderActionType == OrderActionTypes.Exchange)))
					{
						reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
						cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
						updateAction = CreateUpdateNPAfterPayOrDoNothingAction();
					}
					// 住所変更・金額変更・商品変更
					else if ((this.IsChangeOrderInfo
						|| this.IsChangedAmount
						|| this.IsChangeProductInfomation))
					{
						reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
						cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
						updateAction = CreateUpdateNPAfterPayOrDoNothingAction();
					}
					break;
				// NP後払い→クレジットカード
				case ReauthTypes.ChangeNPAfterPayToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
					salesAction = CreateSalesCreditCardOrDoNothingAction();
					break;
				// NP後払い→コンビニ後払い
				case ReauthTypes.ChangeNPAfterPayToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
					break;
				// NPAfterPay To Gmo Post
				case ReauthTypes.ChangeNPAfterPayToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
					break;
				// NP後払い→PayPal決済
				case ReauthTypes.ChangeNPAfterPayToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
					salesAction = CreateSalesPayPalOrDoNothingAction();
					break;
				// NP後払い→Amazon Pay
				case ReauthTypes.ChangeNPAfterPayToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
					break;
				// NP後払い→PaidyPay
				case ReauthTypes.ChangeNPAfterPayToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
					break;
				// NP後払い→LINEPay
				case ReauthTypes.ChangeNPAfterPayToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
					break;
				// NP後払い→Atone
				case ReauthTypes.ChangeNPAfterPayToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
					break;
				// NP後払い→Aftee
				case ReauthTypes.ChangeNPAfterPayToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
					break;
				// NP後払い→キャリア決済
				// NP後払い→代引き
				// NP後払い→決済無し
				// NP後払い全返品（最終請求金額＝0）
				// NP後払い→PayPay
				// NP後払い→Boku
				case ReauthTypes.ChangeNPAfterPayToCarrier:
				case ReauthTypes.ChangeNPAfterPayToCollectOrOthers:
				case ReauthTypes.ChangeNPAfterPayToNoPayment:
				case ReauthTypes.ChangeNPAfterPayToBoku:
				case ReauthTypes.NPAfterPayReturnAllItems:
				case ReauthTypes.ChangeNPAfterPayToPayPay:
					cancelAction = CreateCancelNPAfterPayOrDoNothingAction();
					break;

				// No change Ec Pay
				case ReauthTypes.NoChangeEcPay:
					if (this.ExecuteType == ExecuteTypes.ForcedAuth)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
						cancelAction = CreateCancelEcPayOrDoNothingAction();
					}
					// 返品・交換
					else if ((this.OrderActionType == OrderActionTypes.Return)
						|| (this.OrderActionType == OrderActionTypes.Exchange))
					{
						salesAction = CreateSalesEcPayOrDoNothingAction();
						refundAction = CreateRefundEcPayOrDoNothingAction();

						// If Execute From Order Workflow Then Create Action Cancel
						if (this.OrderOld.IsReturnExchangeProcessAtWorkflow)
						{
							cancelAction = CreateCancelEcPayOrDoNothingAction();
						}
					}
					else if (this.IsChangedAmount)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					break;

				// EcPay→クレジットカード
				case ReauthTypes.ChangeEcPayToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelEcPayOrDoNothingAction();
					}
					break;

				// EcPay→コンビニ後払い
				case ReauthTypes.ChangeEcPayToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelEcPayOrDoNothingAction();
					}
					break;
				// EcPay ToGmoPost
				case ReauthTypes.ChangeEcPayToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelEcPayOrDoNothingAction();
					}
					break;

				// EcPay→PayPal
				case ReauthTypes.ChangeEcPayToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelEcPayOrDoNothingAction();
					}
					break;

				// EcPay→Atone
				case ReauthTypes.ChangeEcPayToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelEcPayOrDoNothingAction();
					}
					break;

				// EcPay→Aftee
				case ReauthTypes.ChangeEcPayToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelEcPayOrDoNothingAction();
					}
					break;

				// EcPay→Amazon Pay
				case ReauthTypes.ChangeEcPayToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelEcPayOrDoNothingAction();
					}
					break;

				// EcPay→後付款(TriLink後払い)
				case ReauthTypes.ChangeEcPayToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelEcPayOrDoNothingAction();
					}
					break;

				// EcPay→PaidyPay
				case ReauthTypes.ChangeEcPayToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelEcPayOrDoNothingAction();
					}
					break;

				// EcPay→LINEPay
				case ReauthTypes.ChangeEcPayToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelEcPayOrDoNothingAction();
			}
					break;

				// EcPay→NP後払い
				case ReauthTypes.ChangeEcPayToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelEcPayOrDoNothingAction();
					}
					break;

				// EcPay→キャリア決済
				// EcPay→代引き
				// EcPay→決済無し
				// EcPay→PayPay
				// EcPay→Boku
				case ReauthTypes.ChangeEcPayToCarrier:
				case ReauthTypes.ChangeEcPayToCollectOrOthers:
				case ReauthTypes.ChangeEcPayToNoPayment:
				case ReauthTypes.ChangeEcPayToBoku:
				case ReauthTypes.EcPayReturnAllItems:
				case ReauthTypes.ChangeEcPayToPayPay:
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundEcPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelEcPayOrDoNothingAction();
					}
					break;

				// No Change NewebPay
				case ReauthTypes.NoChangeNewebPay:
					if (this.ExecuteType == ExecuteTypes.ForcedAuth)
					{
						refundAction = CreateRefundNewebPayOrDoNothingAction();
						cancelAction = CreateCancelNewebPayOrDoNothingAction();
					}
					// Return / Exchange
					else if ((this.OrderActionType == OrderActionTypes.Return)
						|| (this.OrderActionType == OrderActionTypes.Exchange))
					{
						salesAction = CreateSalesNewebPayOrDoNothingAction();
						refundAction = CreateRefundNewebPayOrDoNothingAction();

						// If Execute From Order Workflow Then Create Action Cancel
						if (this.OrderOld.IsReturnExchangeProcessAtWorkflow)
						{
							cancelAction = CreateCancelNewebPayOrDoNothingAction();
						}
					}
					else if (this.IsChangedAmount)
					{
						refundAction = CreateRefundNewebPayOrDoNothingAction();
					}
					break;

				// NewebPay→クレジットカード
				case ReauthTypes.ChangeNewebPayToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelNewebPayOrDoNothingAction();
					break;

				// NewebPay→コンビニ後払い
				case ReauthTypes.ChangeNewebPayToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundNewebPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelNewebPayOrDoNothingAction();
					}
					break;

				// NewebPay To Gmo Post
				case ReauthTypes.ChangeNewebPayToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundNewebPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelNewebPayOrDoNothingAction();
					}
					break;

				// NewebPay→PayPal
				case ReauthTypes.ChangeNewebPayToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundNewebPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelNewebPayOrDoNothingAction();
					}
					break;

				// NewebPay→PaidyPay
				case ReauthTypes.ChangeNewebPayToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundNewebPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelNewebPayOrDoNothingAction();
					}
					break;

				// NewebPay→後付款(TriLink後払い)
				case ReauthTypes.ChangeNewebPayToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundNewebPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelNewebPayOrDoNothingAction();
					}
					break;

				// NewebPay→LINEPay
				case ReauthTypes.ChangeNewebPayToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundNewebPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelNewebPayOrDoNothingAction();
					}
					break;

				// NewebPay→キャリア決済
				// NewebPay→代引き
				// NewebPay→決済無し
				// NewebPay→PayPay
				// NewebPay→Boku
				case ReauthTypes.ChangeNewebPayToCarrier:
				case ReauthTypes.ChangeNewebPayToCollectOrOthers:
				case ReauthTypes.ChangeNewebPayToNoPayment:
				case ReauthTypes.ChangeNewebPayToBoku:
				case ReauthTypes.NewebPayReturnAllItems:
				case ReauthTypes.ChangeNewebPayToPayPay:
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundNewebPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelNewebPayOrDoNothingAction();
					}
					break;

				// NewebPay→Atone
				case ReauthTypes.ChangeNewebPayToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundNewebPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelNewebPayOrDoNothingAction();
					}
					break;

				// NewebPay→Aftee
				case ReauthTypes.ChangeNewebPayToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundNewebPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelNewebPayOrDoNothingAction();
					}
					break;

				// NewebPay→NP後払い
				case ReauthTypes.ChangeNewebPayToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					if (this.IsSalesOrder)
					{
						refundAction = CreateRefundNewebPayOrDoNothingAction();
					}
					else
					{
						cancelAction = CreateCancelNewebPayOrDoNothingAction();
					}
					break;

				// PayPay→PayPay
				case ReauthTypes.NoChangePayPay:
					// 返品・交換
					if (this.IsChangedAmount
						&& this.IsBilledAmountGreaterThanZero
						&& ((this.OrderActionType == OrderActionTypes.Return)
							|| (this.OrderActionType == OrderActionTypes.Exchange)))
					{
						if (this.IsSalesOrderPayPay == false)
						{
							salesAction = CreateSalesPaypayOrDoNothingAction();
						}
						refundAction = CreateRefundPaypayOrDoNothingAction();
					}
					// 強制与信・再与信は減額のみ対応
					else if (this.IsReducedBilledAmount)
					{
						if (this.IsSalesOrderPayPay == false)
						{
							salesAction = CreateSalesPaypayOrDoNothingAction();
						}
						refundAction = CreateRefundPaypayOrDoNothingAction();
					}
					break;

				// PayPay→クレジットカード
				case ReauthTypes.ChangePayPayToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					salesAction = CreateSalesCreditCardOrDoNothingAction();
					cancelAction = CreateCancelPaypayOrDoNothingAction();
					break;

				// PayPay→コンビニ後払い
				case ReauthTypes.ChangePayPayToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					salesAction = CreateSalesCreditCardOrDoNothingAction();
					cancelAction = CreateCancelPaypayOrDoNothingAction();
					break;

				// PayPay To Gmo Post
				case ReauthTypes.ChangePayPayToGmoPost:
					reauthAction = CreateReauthGmoPostOrDoNothingAction();
					cancelAction = CreateCancelPaypayOrDoNothingAction();
					break;

				// PayPay→PayPal
				case ReauthTypes.ChangePayPayToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					salesAction = CreateSalesPayPalOrDoNothingAction();
					cancelAction = CreateCancelPaypayOrDoNothingAction();
					break;

				// PayPay→TriLinkAfterPay
				case ReauthTypes.ChangePayPayToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelPaypayOrDoNothingAction();
					break;

				// PayPay→Carrier
				// PayPay→CollectOrOthers
				// PayPay→NoPayment
				case ReauthTypes.ChangePayPayToCarrier:
				case ReauthTypes.ChangePayPayToCollectOrOthers:
				case ReauthTypes.ChangePayPayToNoPayment:
				case ReauthTypes.PayPayReturnAllItems:
					cancelAction = CreateCancelPaypayOrDoNothingAction();
					break;

				// PayPay→AmazonPay
				case ReauthTypes.ChangePayPayToAmazonPay:
					reauthAction = CreateReauthAmazonPayOrDoNothingAction();
					salesAction = CreateSalesCreditCardOrDoNothingAction();
					cancelAction = CreateCancelPaypayOrDoNothingAction();
					break;

				// PayPay→PaidyPay
				case ReauthTypes.ChangePayPayToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					salesAction = CreateSalesPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelPaypayOrDoNothingAction();
					break;

				// PayPay→Atone
				case ReauthTypes.ChangePayPayToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelPaypayOrDoNothingAction();
					break;

				// PayPay→Aftee
				case ReauthTypes.ChangePayPayToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelPaypayOrDoNothingAction();
					break;

				// PayPay→LinePay
				case ReauthTypes.ChangePayPayToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					salesAction = CreateSalesLinePayOrDoNothingAction();
					cancelAction = CreateCancelPaypayOrDoNothingAction();
					break;

				// PayPay→NPAfterPay
				case ReauthTypes.ChangePayPayToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelPaypayOrDoNothingAction();
					break;

				// No Change Boku
				case ReauthTypes.NoChangeBoku:
					if (this.ExecuteType == ExecuteTypes.ForcedAuth)
					{
						cancelAction = CreateCancelBokuOrDoNothingAction();
					}
					// Return
					else if (this.OrderActionType == OrderActionTypes.Return)
					{
						cancelAction = CreateCancelBokuOrDoNothingAction();
					}
					else if (this.IsChangedAmount)
					{
						reauthAction = CreateReauthBokuOrDoNothingAction();
						cancelAction = CreateCancelBokuOrDoNothingAction();
			}
					break;

				// Boku→クレジットカード
				case ReauthTypes.ChangeBokuToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelBokuOrDoNothingAction();
					break;

				// Boku→コンビニ後払い
				case ReauthTypes.ChangeBokuToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelBokuOrDoNothingAction();
					break;

				// Boku→PayPal
				case ReauthTypes.ChangeBokuToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelBokuOrDoNothingAction();
					break;

				// Boku→PaidyPay
				case ReauthTypes.ChangeBokuToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelBokuOrDoNothingAction();
					break;

				// Boku→後付款(TriLink後払い)
				case ReauthTypes.ChangeBokuToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelBokuOrDoNothingAction();
					break;

				// Boku→LINEPay
				case ReauthTypes.ChangeBokuToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelBokuOrDoNothingAction();
					break;

				// Boku→キャリア決済
				// Boku→代引き
				// Boku→決済無し
				case ReauthTypes.ChangeBokuToCarrier:
				case ReauthTypes.ChangeBokuToCollectOrOthers:
				case ReauthTypes.ChangeBokuToNoPayment:
					cancelAction = CreateCancelBokuOrDoNothingAction();
					break;

				// Boku→Atone
				case ReauthTypes.ChangeBokuToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelBokuOrDoNothingAction();
					break;

				// Boku→Aftee
				case ReauthTypes.ChangeBokuToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelBokuOrDoNothingAction();
					break;

				// Boku→NP後払い
				case ReauthTypes.ChangeBokuToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelBokuOrDoNothingAction();
					break;

				// GMOアトカラ変更なし
				case ReauthTypes.NoChangeGmoAtokara:
					updateAction = CreateUpdateGmoAtokaraOrDoNothingAction();
					break;

				// GMOアトカラ→クレジットカード
				case ReauthTypes.ChangeGmoAtokaraToCredit:
					reauthAction = CreateReauthCreditCardOrDoNothingAction();
					cancelAction = CreateCancelGmoAtokaraOrDoNothingAction();
					break;

				// GMOアトカラ→コンビニ後払い
				case ReauthTypes.ChangeGmoAtokaraToCvsDef:
					reauthAction = CreateReauthCvsDefOrDoNothingAction();
					cancelAction = CreateCancelGmoAtokaraOrDoNothingAction();
					break;

				// GMOアトカラ→PayPal
				case ReauthTypes.ChangeGmoAtokaraToPayPal:
					reauthAction = CreateReauthPayPalOrDoNothingAction();
					cancelAction = CreateCancelGmoAtokaraOrDoNothingAction();
					break;

				// GMOアトカラ→PaidyPay
				case ReauthTypes.ChangeGmoAtokaraToPaidyPay:
					reauthAction = CreateReauthPaidyPayOrDoNothingAction();
					cancelAction = CreateCancelGmoAtokaraOrDoNothingAction();
					break;

				// GMOアトカラ→後付款(TriLink後払い)
				case ReauthTypes.ChangeGmoAtokaraToTriLinkAfterPay:
					reauthAction = CreateReauthTriLinkAfterPayOrDoNothingAction();
					cancelAction = CreateCancelGmoAtokaraOrDoNothingAction();
					break;

				// GMOアトカラ→LINEPay
				case ReauthTypes.ChangeGmoAtokaraToLinePay:
					reauthAction = CreateReauthLinePayOrDoNothingAction();
					cancelAction = CreateCancelGmoAtokaraOrDoNothingAction();
					break;

				// GMOアトカラ→キャリア決済
				// GMOアトカラ→代引き
				// GMOアトカラ→決済無し
				case ReauthTypes.ChangeGmoAtokaraToCarrier:
				case ReauthTypes.ChangeGmoAtokaraToCollectOrOthers:
				case ReauthTypes.ChangeGmoAtokaraToNoPayment:
					cancelAction = CreateCancelGmoAtokaraOrDoNothingAction();
					break;

				// GMOアトカラ→Atone
				case ReauthTypes.ChangeGmoAtokaraToAtone:
					reauthAction = CreateReauthAtoneOrDoNothingAction();
					cancelAction = CreateCancelGmoAtokaraOrDoNothingAction();
					break;

				// GMOアトカラ→Aftee
				case ReauthTypes.ChangeGmoAtokaraToAftee:
					reauthAction = CreateReauthAfteeOrDoNothingAction();
					cancelAction = CreateCancelGmoAtokaraOrDoNothingAction();
					break;

				// GMOアトカラ→NP後払い
				case ReauthTypes.ChangeGmoAtokaraToNPAfterPay:
					reauthAction = CreateReauthNPAfterPayOrDoNothingAction();
					cancelAction = CreateCancelGmoAtokaraOrDoNothingAction();
					break;
			}

			// 支払変更：変更前の再与信処理区分が「連動しない」場合はキャンセルしない
			if ((this.OldExecuteType == ExecuteTypes.None)
				&& (reauthType != ReauthTypes.NoChangeAmazonPay)
				&& (reauthType != ReauthTypes.NoChangePayPal)
				&& (reauthType != ReauthTypes.NoChangePaidyPay)
				&& (reauthType != ReauthTypes.NoChangeCvsDef)
				&& (reauthType != ReauthTypes.NoChangeAtone)
				&& (reauthType != ReauthTypes.NoChangeAftee)
				&& (reauthType != ReauthTypes.NoChangeEcPay)
				&& (this.OrderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				&& (this.OrderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
				&& (this.OrderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
				&& (reauthType != ReauthTypes.NoChangeLinePay)
				&& (reauthType != ReauthTypes.NoChangeNPAfterPay)
				&& ((reauthType != ReauthTypes.NoChangeCredit)
					|| (this.IsNoChangeCreditButAnotherCard)
					|| (this.IsNoChangeCreditButInstallmentsCode)))
			{
				cancelAction = CreateDoNothingAction();
			}

			// 支払変更：変更後の再与信処理区分が「連動しない」場合は連動しない
			if (this.ExecuteType == ExecuteTypes.None)
			{
				reauthAction = CreateDoNothingAction();
				reduceAction = CreateDoNothingAction();
				refundAction = CreateDoNothingAction();
				updateAction = CreateDoNothingAction();
				reprintAction = CreateDoNothingAction();
				salesAction = CreateDoNothingAction();
				billingAction = CreateDoNothingAction();
				if (this.OrderActionType != OrderActionTypes.Modify) cancelAction = CreateDoNothingAction();
			}

			return new ReauthExecuter(reauthAction, cancelAction, reduceAction, updateAction, reprintAction, salesAction, refundAction, billingAction, this.PreCancelFlg);
		}

		/// <summary>
		/// 再与信区分取得
		/// </summary>
		/// <returns>再与信区分</returns>
		private ReauthTypes GetReauthType()
		{
			// 再与信しない場合は何もしない
			if (this.ExecuteType == ExecuteTypes.None
				&& this.OldExecuteType == ExecuteTypes.None) return ReauthTypes.None;

			// 変更前後の決済種別が外部決済以外の場合は何もしない
			if (this.IsChangeCollectToNoPayment
				|| this.IsChangeNoPaymentToCollect) return ReauthTypes.None;

			// キャリア決済 OR 代引き OR 決済無しの全返品
			if (this.IsReturnAllItemsCarrierOrCollectOrNoPayment) return ReauthTypes.None;

			// クレジットカード全返品 (最終請求金額=0)
			if (this.IsCreditReturnAllItems) return ReauthTypes.CreditReturnAllItems;

			// クレジットカード変更なし
			if (this.IsNoChangeCredit) return ReauthTypes.NoChangeCredit;

			// クレジットカード→コンビニ後払い
			if (this.IsChangeCreditToCvsDef) return ReauthTypes.ChangeCreditToCvsDef;

			// クレジットカード→GmoPost
			if (this.IsChangeCreditToGmoPost) return ReauthTypes.ChangeCreditToGmoPost;

			// クレジットカード→PayPal
			if (this.IsChangeCreditToPayPal) return ReauthTypes.ChangeCreditToPayPal;

			// クレジットカード→後付款(TriLink後払い)
			if (this.IsChangeCreditToTriLinkAfterPay) return ReauthTypes.ChangeCreditToTriLinkAfterPay;

			// クレジットカード→キャリア決済
			if (this.IsChangeCreditToCarrier) return ReauthTypes.ChangeCreditToCarrier;

			// クレジットカード→代引き
			if (this.IsChangeCreditToCollect) return ReauthTypes.ChangeCreditToCollectOrOthers;

			// クレジットカード→Amazon Pay
			if (this.IsChangeCreditToAmazonPay) return ReauthTypes.ChangeCreditToAmazonPay;

			// クレジットカード→Paidy Pay
			if (this.IsChangeCreditToPaidyPay) return ReauthTypes.ChangeCreditToPaidyPay;

			// クレジットカード→決済無し
			if (this.IsChangeCreditToNoPayment) return ReauthTypes.ChangeCreditToNoPayment;

			// クレジットカード→Atone
			if (this.IsChangeCreditToAtone) return ReauthTypes.ChangeCreditToAtone;

			// クレジットカード→Aftee
			if (this.IsChangeCreditToAftee) return ReauthTypes.ChangeCreditToAftee;

			// クレジットカード→LINEPay
			if (this.IsChangeCreditToLinePay) return ReauthTypes.ChangeCreditToLinePay;

			// クレジットカード→PayPay
			if (this.IsChangeCreditToPayPay) return ReauthTypes.ChangeCreditToPayPay;

			// クレジットカード→Boku
			if (this.IsChangeCreditToBoku) return ReauthTypes.ChangeCreditToBoku;

			// コンビニ後払い全返品
			if (this.IsCvsDefReturnAllItems) return ReauthTypes.CvsDefReturnAllItems;

			// Is PayAsYouGo Return AllI tems
			if (this.IsPayAsYouGoReturnAllItems) return ReauthTypes.PayAsYouGoReturnAllItems;

			// Is FramePayment Return AllI tems
			if (this.IsFramePaymentReturnAllItems) return ReauthTypes.FramePaymentReturnAllItems;

			// コンビニ後払い変更なし
			if (this.IsNoChangeCvsDef) return ReauthTypes.NoChangeCvsDef;

			// Is No Change PayAsYouGo
			if (this.IsNoChangePayAsYouGo) return ReauthTypes.NoChangePayAsYouGo;

			// Is No Change FramePayment
			if (this.IsNoChangeFramePayment) return ReauthTypes.NoChangeFramePayment;

			// コンビニ後払い→クレジットカード
			if (this.IsChangeCvsDefToCredit) return ReauthTypes.ChangeCvsDefToCredit;

			// コンビニ後払い→後付款(TriLink後払い)
			if (this.IsChangeCvsDefToTriLinkAfterPay) return ReauthTypes.ChangeCvsDefToTriLinkAfterPay;

			// コンビニ後払い→キャリア決済
			if (this.IsChangeCvsDefToCarrier) return ReauthTypes.ChangeCvsDefToCarrier;

			// コンビニ後払い→PayPal
			if (this.IsChangeCvsDefToPayPal) return ReauthTypes.ChangeCvsDefToPayPal;

			// コンビニ後払い→代引き
			if (this.IsChangeCvsDefToCollect) return ReauthTypes.ChangeCvsDefToCollectOrOthers;

			// コンビニ後払い→Amazon Pay
			if (this.IsChangeCvsDefToAmazonPay) return ReauthTypes.ChangeCvsDefToAmazonPay;

			// コンビニ後払い→Paidy Pay
			if (this.IsChangeCvsDefToPaidyPay) return ReauthTypes.ChangeCvsDefToPaidyPay;

			// コンビニ後払い→決済無し
			if (this.IsChangeCvsDefToNoPayment) return ReauthTypes.ChangeCvsDefToNoPayment;

			// コンビニ後払い→Atone
			if (this.IsChangeCvsDefToAtone) return ReauthTypes.ChangeCvsDefToAtone;

			// コンビニ後払い→Aftee
			if (this.IsChangeCvsDefToAftee) return ReauthTypes.ChangeCvsDefToAftee;

			// コンビニ後払い→LINEPay
			if (this.IsChangeCvsDefToLinePay) return ReauthTypes.ChangeCvsDefToLinePay;

			// コンビニ後払い→PayPay
			if (this.IsChangeCvsDefToPayPay) return ReauthTypes.ChangeCvsDefToPayPay;

			// コンビニ後払い→Boku
			if (this.IsChangeCvsDefToBoku) return ReauthTypes.ChangeCvsDefToBoku;

			// Is Change PayAsYouGo To Credit
			if (this.IsChangePayAsYouGoToCredit) return ReauthTypes.ChangePayAsYouGoToCredit;

			// Is Change PayAsYouGo To TriLinkAfterPay
			if (this.IsChangePayAsYouGoToTriLinkAfterPay) return ReauthTypes.ChangePayAsYouGoToTriLinkAfterPay;

			// Is Change PayAsYouGo To Carrier
			if (this.IsChangePayAsYouGoToCarrier) return ReauthTypes.ChangePayAsYouGoToCarrier;

			// Is Change PayAsYouGo To PayPal
			if (this.IsChangePayAsYouGoToPayPal) return ReauthTypes.ChangePayAsYouGoToPayPal;

			// Is Change PayAsYouGo To Collect
			if (this.IsChangePayAsYouGoToCollect) return ReauthTypes.ChangePayAsYouGoToCollectOrOthers;

			// Is Change PayAsYouGo To AmazonPay
			if (this.IsChangePayAsYouGoToAmazonPay) return ReauthTypes.ChangePayAsYouGoToAmazonPay;

			// Is Change PayAsYouGo To PaidyPay
			if (this.IsChangePayAsYouGoToPaidyPay) return ReauthTypes.ChangePayAsYouGoToPaidyPay;

			// Is Change PayAsYouGo To No Payment
			if (this.IsChangePayAsYouGoToNoPayment) return ReauthTypes.ChangePayAsYouGoToNoPayment;

			// Is Change PayAsYouGo To Atone
			if (this.IsChangePayAsYouGoToAtone) return ReauthTypes.ChangePayAsYouGoToAtone;

			// Is Change PayAsYouGo To Aftee
			if (this.IsChangePayAsYouGoToAftee) return ReauthTypes.ChangePayAsYouGoToAftee;

			// Is Change PayAsYouGo To LinePay
			if (this.IsChangePayAsYouGoToLinePay) return ReauthTypes.ChangePayAsYouGoToLinePay;

			// Is Change PayAsYouGo To LinePay
			if (this.IsChangePayAsYouGoToFramePayment) return ReauthTypes.ChangePayAsYouGoToFramePayment;

			// Is Change PayAsYouGo To PayPay
			if (this.IsChangeFramePaymentToPayPay) return ReauthTypes.ChangeFramePaymentToPayPay;

			// Is Change FramePayment To Credit
			if (this.IsChangeFramePaymentToCredit) return ReauthTypes.ChangeFramePaymentToCredit;

			// Is Change FramePayment To TriLinkAfterPay
			if (this.IsChangeFramePaymentToTriLinkAfterPay) return ReauthTypes.ChangeFramePaymentToTriLinkAfterPay;

			// Is Change FramePayment To Carrier
			if (this.IsChangeFramePaymentToCarrier) return ReauthTypes.ChangeFramePaymentToCarrier;

			// Is Change FramePayment To PayPal
			if (this.IsChangeFramePaymentToPayPal) return ReauthTypes.ChangeFramePaymentToPayPal;

			// Is Change FramePayment To Collect
			if (this.IsChangeFramePaymentToCollect) return ReauthTypes.ChangeFramePaymentToCollectOrOthers;

			// Is Change FramePayment To AmazonPay
			if (this.IsChangeFramePaymentToAmazonPay) return ReauthTypes.ChangeFramePaymentToAmazonPay;

			// Is Change FramePayment To PaidyPay
			if (this.IsChangeFramePaymentToPaidyPay) return ReauthTypes.ChangeFramePaymentToPaidyPay;

			// Is Change FramePayment To No Payment
			if (this.IsChangeFramePaymentToNoPayment) return ReauthTypes.ChangeFramePaymentToNoPayment;

			// Is Change FramePayment To Atone
			if (this.IsChangeFramePaymentToAtone) return ReauthTypes.ChangeFramePaymentToAtone;

			// Is Change FramePayment To Aftee
			if (this.IsChangeFramePaymentToAftee) return ReauthTypes.ChangeFramePaymentToAftee;

			// Is Change FramePayment To LinePay
			if (this.IsChangeFramePaymentToLinePay) return ReauthTypes.ChangeFramePaymentToLinePay;

			// Is Change FramePayment To PayPay
			if (this.IsChangeFramePaymentToPayPay) return ReauthTypes.ChangeFramePaymentToPayPay;

			// Is Change FramePayment To PayAsYouGo
			if (this.IsChangeFramePaymentToPayAsYouGo) return ReauthTypes.ChangeFramePaymentToPayAsYouGo;

			// PayPal全返品 (最終請求金額=0)
			if (this.IsPayPalReturnAllItems) return ReauthTypes.PayPalReturnAllItems;

			// PayPal変更なし
			if (this.IsNoChangePayPal) return ReauthTypes.NoChangePayPal;

			// PayPal→クレジットカード
			if (this.IsChangePayPalToCredit) return ReauthTypes.ChangePayPalToCredit;

			// PayPal→コンビニ後払い
			if (this.IsChangePayPalToCvsDef) return ReauthTypes.ChangePayPalToCvsDef;

			// PayPal→GmoPost
			if (this.IsChangePayPalToGmoPost) return ReauthTypes.ChangePayPalToGmoPost;

			// PayPal→後付款(TriLink後払い)
			if (this.IsChangePayPalToTriLinkAfterPay) return ReauthTypes.ChangePayPalToTriLinkAfterPay;

			// PayPal→キャリア決済
			if (this.IsChangePayPalToCarrier) return ReauthTypes.ChangePayPalToCarrier;

			// PayPal→代引き
			if (this.IsChangePayPalToCollect) return ReauthTypes.ChangePayPalToCollectOrOthers;

			// PayPal→Amazon Pay
			if (this.IsChangePayPalToCollect) return ReauthTypes.ChangePayPalToAmazonPay;

			// PayPal→Paidy Pay
			if (this.IsChangePayPalToPaidyPay) return ReauthTypes.ChangePayPalToPaidyPay;

			// PayPal→決済無し
			if (this.IsChangePayPalToNoPayment) return ReauthTypes.ChangePayPalToNoPayment;

			// PayPal→Atone
			if (this.IsChangePayPalToAtone) return ReauthTypes.ChangePayPalToAtone;

			// PayPal→Aftee
			if (this.IsChangePayPalToAftee) return ReauthTypes.ChangePayPalToAftee;

			// PayPal→LINEPay
			if (this.IsChangePayPalToLinePay) return ReauthTypes.ChangePayPalToLinePay;

			// PayPal→PayPay
			if (this.IsChangePayPalToPayPay) return ReauthTypes.ChangePayPalToPayPay;

			// PayPal→Boku
			if (this.IsChangePayPalToBoku) return ReauthTypes.ChangePayPalToBoku;

			// 後付款(TriLink後払い)全返品
			if (this.IsTriLinkAfterPayReturnAllItems) return ReauthTypes.TriLinkAfterPayReturnAllItems;

			// 後付款(TriLink後払い)変更なし
			if (this.IsNoChangeTriLinkAfterPay) return ReauthTypes.NoChangeTriLinkAfterPay;

			// 後付款(TriLink後払い)→クレジットカード
			if (this.IsChangeTriLinkAfterPayToCredit) return ReauthTypes.ChangeTriLinkAfterPayToCredit;

			// 後付款(TriLink後払い)→コンビニ後払い
			if (this.IsChangeTriLinkAfterPayToCvsDef) return ReauthTypes.ChangeTriLinkAfterPayToCvsDef;

			// 後付款(TriLink後払い)→GmoPost
			if (this.IsChangeTriLinkAfterPayToGmoPost) return ReauthTypes.ChangeTriLinkAfterPayToGmoPost;

			// 後付款(TriLink後払い)→Amazon Pay
			if (this.IsChangeTriLinkAfterPayToAmazonPay) return ReauthTypes.ChangeTriLinkAfterPayToAmazonPay;

			// 後付款(TriLink後払い)→Paidy Pay
			if (this.IsChangeTriLinkAfterPayToPaidyPay) return ReauthTypes.ChangeTriLinkAfterPayToPaidyPay;

			// 後付款(TriLink後払い)→PayPal
			if (this.IsChangeTriLinkAfterPayToPayPal) return ReauthTypes.ChangeTriLinkAfterPayToPayPal;

			// 後付款(TriLink後払い)→キャリア決済
			if (this.IsChangeTriLinkAfterPayToCarrier) return ReauthTypes.ChangeTriLinkAfterPayToCarrier;

			// 後付款(TriLink後払い)→代引き
			if (this.IsChangeTriLinkAfterPayToCollect) return ReauthTypes.ChangeTriLinkAfterPayToCollectOrOthers;

			// 後付款(TriLink後払い)→決済無し
			if (this.IsChangeTriLinkAfterPayToNoPayment) return ReauthTypes.ChangeTriLinkAfterPayToNoPayment;

			// 後付款(TriLink後払い)→Atone
			if (this.IsChangeTriLinkAfterPayToAtone) return ReauthTypes.ChangeTriLinkAfterPayToAtone;

			// 後付款(TriLink後払い)→Aftee
			if (this.IsChangeTriLinkAfterPayToAftee) return ReauthTypes.ChangeTriLinkAfterPayToAftee;

			// 後付款(TriLink後払い)→LINEPay
			if (this.IsChangeTriLinkAfterPayToLinePay) return ReauthTypes.ChangeTriLinkAfterPayToLinePay;

			// 後付款(TriLink後払い)→PayPay
			if (this.IsChangeTriLinkAfterPayToPayPay) return ReauthTypes.ChangeTriLinkAfterPayToPayPay;

			// 後付款(TriLink後払い)→Boku
			if (this.IsChangeTriLinkAfterPayToBoku) return ReauthTypes.ChangeTriLinkAfterPayToBoku;

			// キャリア決済→クレジットカード
			if (this.IsChangeCarrierToCredit) return ReauthTypes.ChangeCarrierToCredit;

			// キャリア決済→コンビニ後払い
			if (this.IsChangeCarrierToCvsDef) return ReauthTypes.ChangeCarrierToCvsDef;

			// キャリア決済→GmoPost
			if (this.IsChangeCarrierToGmoPost) return ReauthTypes.ChangeCarrierToGmoPost;

			// キャリア決済→PayPal
			if (this.IsChangeCarrierToPayPal) return ReauthTypes.ChangeCarrierToPayPal;

			// キャリア決済→後付款(TriLink後払い)
			if (this.IsChangeCarrierToTriLinkAfterPay) return ReauthTypes.ChangeCarrierToTriLinkAfterPay;

			// キャリア決済→代引き
			if (this.IsChangeCarrierToCollect) return ReauthTypes.ChangeCarrierToCollectOrOthers;

			// キャリア決済→Amazon Pay
			if (this.IsChangeCarrierToAmazonPay) return ReauthTypes.ChangeCarrierToAmazonPay;

			// キャリア決済→Paidy Pay
			if (this.IsChangeCarrierToPaidyPay) return ReauthTypes.ChangeCarrierToPaidyPay;

			// キャリア決済→決済無し
			if (this.IsChangeCarrierToNoPayment) return ReauthTypes.ChangeCarrierToNoPayment;

			// キャリア決済→Atone
			if (this.IsChangeCarrierToAtone) return ReauthTypes.ChangeCarrierToAtone;

			// キャリア決済→Aftee
			if (this.IsChangeCarrierToAftee) return ReauthTypes.ChangeCarrierToAftee;

			// キャリア決済→LINEPay
			if (this.IsChangeCarrierToLinePay) return ReauthTypes.ChangeCarrierToLinePay;

			// キャリア決済→PayPay
			if (this.IsChangeCarrierToPayPay) return ReauthTypes.ChangeCarrierToPayPay;

			// キャリア決済→Boku
			if (this.IsChangeCarrierToBoku) return ReauthTypes.ChangeCarrierToBoku;

			// 代引き→クレジットカード
			if (this.IsChangeCollectToCredit) return ReauthTypes.ChangeCollectOrOthersToCredit;

			// 代引き→コンビニ後払い
			if (this.IsChangeCollectToCvsDef) return ReauthTypes.ChangeCollectOrOthersToCvsDef;

			// 代引き→GmoPost
			if (this.IsChangeCollectToGmoPost) return ReauthTypes.ChangeCollectOrOthersToGmoPost;

			// 代引き→PayPal
			if (this.IsChangeCollectToPayPal) return ReauthTypes.ChangeCollectOrOthersToPayPal;

			// 代引き→コンビニ後払い
			if (this.IsChangeCollectToTriLinkAfterPay) return ReauthTypes.ChangeCollectOrOthersToTriLinkAfterPay;

			// 代引き→キャリア決済
			if (this.IsChangeCollectToCarrier) return ReauthTypes.ChangeCollectOrOthersToCarrier;

			// 代引き→Amazon Pay
			if (this.IsChangeCollectToAmazonPay) return ReauthTypes.ChangeCollectOrOthersToAmazonPay;

			// 代引き→Paidy Pay
			if (this.IsChangeCollectToPaidyPay) return ReauthTypes.ChangeCollectOrOthersToPaidyPay;

			// 代引き→決済無し
			if (this.IsChangeCollectToNoPayment) return ReauthTypes.ChangeCollectOrOthersToNoPayment;

			// 代引き→Atone
			if (this.IsChangeCollectToAtone) return ReauthTypes.ChangeCollectOrOthersToAtone;

			// 代引き→Aftee
			if (this.IsChangeCollectToAftee) return ReauthTypes.ChangeCollectOrOthersToAftee;

			// 代引き→LINEPay
			if (this.IsChangeCollectToLinePay) return ReauthTypes.ChangeCollectOrOthersToLinePay;

			// 代引き→PayPay
			if (this.IsChangeCollectToPayPay) return ReauthTypes.ChangeCollectOrOthersToPayPay;

			// 決済無し→クレジットカード
			if (this.IsChangeNoPaymentToCredit) return ReauthTypes.ChangeNoPaymentToCredit;

			// 決済無し→コンビニ後払い
			if (this.IsChangeNoPaymentToCvsDef) return ReauthTypes.ChangeNoPaymentToCvsDef;

			// 決済無し→GmoPost
			if (this.IsChangeNoPaymentToGmoPost) return ReauthTypes.ChangeNoPaymentToGmoPost;

			// 決済無し→PayPal
			if (this.IsChangeNoPaymentToPayPal) return ReauthTypes.ChangeNoPaymentToPayPal;

			// 決済無し→後付款(TriLink後払い)
			if (this.IsChangeNoPaymentToTriLinkAfterPay) return ReauthTypes.ChangeNoPaymentToTriLinkAfterPay;

			// 決済無し→キャリア決済
			if (this.IsChangeNoPaymentToCarrier) return ReauthTypes.ChangeNoPaymentToCarrier;

			// 決済無し→代引き
			if (this.IsChangeNoPaymentToCollect) return ReauthTypes.ChangeNoPaymentToCollectOrOthers;

			// 決済無し→LINEPay
			if (this.IsChangeNoPaymentToLinePay) return ReauthTypes.ChangeNoPaymentToLinePay;

			// 決済無し→Amazon Pay
			if (this.IsChangeNoPaymentToAmazonPay) return ReauthTypes.ChangeNoPaymentToAmazonPay;

			// 決済無し→Paidy Pay
			if (this.IsChangeNoPaymentToPaidyPay) return ReauthTypes.ChangeNoPaymentToPaidyPay;

			// 決済無し→PayPay
			if (this.IsChangeNoPaymentToPayPay) return ReauthTypes.ChangeNoPaymentToPayPay;

			// Amazon Pay全返品 (最終請求金額=0)
			if (this.IsAmazonPayReturnAllItems) return ReauthTypes.AmazonPayReturnAllItems;

			// Amazon Pay変更なし
			if (this.IsNoChangeAmazonPay) return ReauthTypes.NoChangeAmazonPay;

			// Amazon Pay→クレジットカード
			if (this.IsChangeAmazonPayToCredit) return ReauthTypes.ChangeAmazonPayToCredit;

			// Amazon Pay→コンビニ後払い
			if (this.IsChangeAmazonPayToCvsDef) return ReauthTypes.ChangeAmazonPayToCvsDef;

			// Amazon Pay→GmoPost
			if (this.IsChangeAmazonPayToGmoPost) return ReauthTypes.ChangeAmazonPayToGmoPost;

			// Amazon Pay→キャリア決済
			if (this.IsChangeAmazonPayToCarrier) return ReauthTypes.ChangeAmazonPayToCarrier;

			// Amazon Pay→代引き
			if (this.IsChangeAmazonPayToCollect) return ReauthTypes.ChangeAmazonPayToCollectOrOthers;

			// Amazon Pay→PayPal
			if (this.IsChangeNoPaymentToPayPal) return ReauthTypes.ChangeAmazonPayToPayPal;

			// Amazon Pay→Paidy Pay
			if (this.IsChangeAmazonPayToPaidyPay) return ReauthTypes.ChangeAmazonPayToPaidyPay;

			// Amazon Pay→後付款(TriLink後払い)
			if (this.IsChangeAmazonPayToTriLinkAfterPay) return ReauthTypes.ChangeAmazonPayToTriLinkAfterPay;

			// Amazon Pay→決済無し
			if (this.IsChangeAmazonPayToNoPayment) return ReauthTypes.ChangeAmazonPayToNoPayment;

			// Amazon Pay→PayPay
			if (this.IsChangeAmazonPayToPayPay) return ReauthTypes.ChangeAmazonPayToPayPay;

			// Amazon Pay→Boku
			if (this.IsChangeAmazonPayToBoku) return ReauthTypes.ChangeAmazonPayToBoku;

			// Paidy Pay全返品 (最終請求金額=0)
			if (this.IsPaidyPayReturnAllItems) return ReauthTypes.PaidyPayReturnAllItems;

			// Paidy Pay変更なし
			if (this.IsNoChangePaidyPay) return ReauthTypes.NoChangePaidyPay;

			// Paidy Pay→クレジットカード
			if (this.IsChangePaidyPayToCredit) return ReauthTypes.ChangePaidyPayToCredit;

			// Paidy Pay→コンビニ後払い
			if (this.IsChangePaidyPayToCvsDef) return ReauthTypes.ChangePaidyPayToCvsDef;

			// Paidy Pay→GmoPost
			if (this.IsChangePaidyPayToGmoPost) return ReauthTypes.ChangePaidyPayToGmoPost;

			// Paidy Pay→キャリア決済
			if (this.IsChangePaidyPayToCarrier) return ReauthTypes.ChangePaidyPayToCarrier;

			// Paidy Pay→代引き
			if (this.IsChangePaidyPayToCollect) return ReauthTypes.ChangePaidyPayToCollectOrOthers;

			// Paidy Pay→PayPal
			if (this.IsChangePaidyPayToPayPal) return ReauthTypes.ChangePaidyPayToPayPal;

			// Paidy Pay→Amazon Pay
			if (this.IsChangePaidyPayToAmazonPay) return ReauthTypes.ChangePaidyPayToAmazonPay;

			// Paidy Pay→後付款(TriLink後払い)
			if (this.IsChangePaidyPayToTriLinkAfterPay) return ReauthTypes.ChangePaidyPayToTriLinkAfterPay;

			// Paidy Pay→LinePay
			if (this.IsChangePaidyPayToLinePay) return ReauthTypes.ChangePaidyPayToLinePay;

			// Paidy Pay→決済無し
			if (this.IsChangePaidyPayToNoPayment) return ReauthTypes.ChangePaidyPayToNoPayment;

			// Paidy Pay→PayPay
			if (this.IsChangePaidyPayToPayPay) return ReauthTypes.ChangePaidyPayToPayPay;

			// Paidy Pay→Boku
			if (this.IsChangePaidyPayToBoku) return ReauthTypes.ChangePaidyPayToBoku;

			// Amazon Pay→Atone
			if (this.IsChangeAmazonPayToAtone) return ReauthTypes.ChangeAmazonPayToAtone;

			// Amazon Pay→Aftee
			if (this.IsChangeAmazonPayToAftee) return ReauthTypes.ChangeAmazonPayToAftee;
			
			// No change Atone
			if (this.IsNoChangeAtone) return ReauthTypes.NoChangeAtone;

			// Atone→クレジットカード
			if (this.IsChangeAtoneToCredit) return ReauthTypes.ChangeAtoneToCredit;

			// Atone To AmazonPay
			if (this.IsChangeAtoneToAmazonPay) return ReauthTypes.ChangeAtoneToAmazonPay;

			// Atone To TriLinkAfterPay
			if (this.IsChangeAtoneToTriLinkAfterPay) return ReauthTypes.ChangeAtoneToTriLinkAfterPay;

			// Atone To Collect
			if (this.IsChangeAtoneToCollect) return ReauthTypes.ChangeAtoneToCollectOrOthers;

			// Atone To Carrier
			if (this.IsChangeAtoneToCarrier) return ReauthTypes.ChangeAtoneToCarrier;

			// Atone To No payment
			if (this.IsChangeAtoneToNoPayment) return ReauthTypes.ChangeAtoneToNoPayment;

			// Atone To pay pal
			if (this.IsChangeAtoneToPayPal) return ReauthTypes.ChangeAtoneToPayPal;

			// Atone To Line Pay
			if (this.IsChangeAtoneToLinePay) return ReauthTypes.ChangeAtoneToLinePay;

			// Atone return all
			if (this.IsAtoneReturnAllItems) return ReauthTypes.AtoneReturnAllItems;

			// Atone return Cvs
			if (this.IsChangeAtoneToCvsDef) return ReauthTypes.ChangeAtoneToCvsDef;

			// Atone To Gmo Post
			if (this.IsChangeAtoneToGmoPost) return ReauthTypes.ChangeAtoneToGmoPost;

			// Atone To Paypay
			if (this.IsChangeAtoneToPayPay) return ReauthTypes.ChangeAtoneToPayPay;

			// Atone to Boku
			if (this.IsChangeAtoneToBoku) return ReauthTypes.ChangeAtoneToBoku;

			// No change Aftee
			if (this.IsNoChangeAftee) return ReauthTypes.NoChangeAftee;

			// Aftee→クレジットカード
			if (this.IsChangeAfteeToCredit) return ReauthTypes.ChangeAfteeToCredit;

			// Aftee To AmazonPay
			if (this.IsChangeAfteeToAmazonPay) return ReauthTypes.ChangeAfteeToAmazonPay;

			// Aftee To TriLinkAfterPay
			if (this.IsChangeAfteeToTriLinkAfterPay) return ReauthTypes.ChangeAfteeToTriLinkAfterPay;

			// Aftee To Collect
			if (this.IsChangeAfteeToCollect) return ReauthTypes.ChangeAfteeToCollectOrOthers;

			// Aftee To Carrier
			if (this.IsChangeAfteeToCarrier) return ReauthTypes.ChangeAfteeToCarrier;

			// Aftee To No payment
			if (this.IsChangeAfteeToNoPayment) return ReauthTypes.ChangeAfteeToNoPayment;

			// Aftee To pay pal
			if (this.IsChangeAfteeToPayPal) return ReauthTypes.ChangeAfteeToPayPal;

			// Aftee To Line Pay
			if (this.IsChangeAfteeToLinePay) return ReauthTypes.ChangeAfteeToLinePay;

			// Aftee return all
			if (this.IsAfteeReturnAllItems) return ReauthTypes.AfteeReturnAllItems;

			// Aftee return all
			if (this.IsChangeAfteeToCvsDef) return ReauthTypes.ChangeAfteeToCvsDef;

			// Aftee return Gmo Post
			if (this.IsChangeAfteeToGmoPost) return ReauthTypes.ChangeAfteeToGmoPost;

			// Aftee To Paypay
			if (this.IsChangeAfteeToPayPay) return ReauthTypes.ChangeAfteeToPayPay;

			// Aftee to Boku
			if (this.IsChangeAfteeToBoku) return ReauthTypes.ChangeAfteeToBoku;

			// Amazon Pay→LINEPay
			if (this.IsChangeAmazonPayToLinePay) return ReauthTypes.ChangeAmazonPayToLinePay;

			// EcPay全返品 (最終請求金額=0)
			if (this.IsEcPayReturnAllItems) return ReauthTypes.EcPayReturnAllItems;

			// EcPay変更なし
			if (this.IsNoChangeEcPay) return ReauthTypes.NoChangeEcPay;

			// EcPay→クレジットカード
			if (this.IsChangeEcPayToCredit) return ReauthTypes.ChangeEcPayToCredit;

			// EcPay→コンビニ後払い
			if (this.IsChangeEcPayToCvsDef) return ReauthTypes.ChangeEcPayToCvsDef;

			// EcPay→GmoPost
			if (this.IsChangeEcPayToGmoPost) return ReauthTypes.ChangeEcPayToGmoPost;

			// EcPay→キャリア決済
			if (this.IsChangeEcPayToCarrier) return ReauthTypes.ChangeEcPayToCarrier;

			// EcPay→代引き
			if (this.IsChangeEcPayToCollect) return ReauthTypes.ChangeEcPayToCollectOrOthers;

			// EcPay→PayPal
			if (this.IsChangeEcPayToPayPal) return ReauthTypes.ChangeEcPayToPayPal;

			// EcPay→Amazon Pay
			if (this.IsChangeEcPayToAmazonPay) return ReauthTypes.ChangeEcPayToAmazonPay;

			// EcPay→後付款(TriLink後払い)
			if (this.IsChangeEcPayToTriLinkAfterPay) return ReauthTypes.ChangeEcPayToTriLinkAfterPay;

			// EcPay→Paidy Pay
			if (this.IsChangeEcPayToPaidyPay) return ReauthTypes.ChangeEcPayToPaidyPay;

			// EcPay→Aftee
			if (this.IsChangeEcPayToAftee) return ReauthTypes.ChangeEcPayToAftee;

			// EcPay→Atone
			if (this.IsChangeEcPayToAtone) return ReauthTypes.ChangeEcPayToAtone;

			// EcPay→LINEPay
			if (this.IsChangeEcPayToLinePay) return ReauthTypes.ChangeEcPayToLinePay;

			// EcPay→NP後払い
			if (this.IsChangeEcPayToNPAfterPay) return ReauthTypes.ChangeEcPayToNPAfterPay;

			// EcPay→決済無し
			if (this.IsChangeEcPayToNoPayment) return ReauthTypes.ChangeEcPayToNoPayment;

			// EcPay→PayPay
			if (this.IsChangeEcPayToPayPay) return ReauthTypes.ChangeEcPayToPayPay;

			// EcPay→Boku
			if (this.IsChangeEcPayToBoku) return ReauthTypes.ChangeEcPayToBoku;

			// NewebPay変更なし
			if (this.IsNoChangeNewebPay) return ReauthTypes.NoChangeNewebPay;

			// NewebPay→クレジットカード
			if (this.IsChangeNewebPayToCredit) return ReauthTypes.ChangeNewebPayToCredit;

			// NewebPay→コンビニ後払い
			if (this.IsChangeNewebPayToCvsDef) return ReauthTypes.ChangeNewebPayToCvsDef;

			// NewebPay→GmoPost
			if (this.IsChangeNewebPayToGmoPost) return ReauthTypes.ChangeNewebPayToGmoPost;

			// NewebPay→キャリア決済
			if (this.IsChangeNewebPayToCarrier) return ReauthTypes.ChangeNewebPayToCarrier;

			// NewebPay→代引き
			if (this.IsChangeNewebPayToCollect) return ReauthTypes.ChangeNewebPayToCollectOrOthers;

			// NewebPay→PayPal
			if (this.IsChangeNewebPayToPayPal) return ReauthTypes.ChangeNewebPayToPayPal;

			// NewebPay→Amazon Pay
			if (this.IsChangeNewebPayToAmazonPay) return ReauthTypes.ChangeNewebPayToAmazonPay;

			// NewebPay→後付款(TriLink後払い)
			if (this.IsChangeNewebPayToTriLinkAfterPay) return ReauthTypes.ChangeNewebPayToTriLinkAfterPay;

			// NewebPay→Paidy Pay
			if (this.IsChangeNewebPayToPaidyPay) return ReauthTypes.ChangeNewebPayToPaidyPay;

			// NewebPay→Aftee
			if (this.IsChangeNewebPayToAftee) return ReauthTypes.ChangeNewebPayToAftee;

			// NewebPay→Atone
			if (this.IsChangeNewebPayToAtone) return ReauthTypes.ChangeNewebPayToAtone;

			// NewebPay→LINEPay
			if (this.IsChangeNewebPayToLinePay) return ReauthTypes.ChangeNewebPayToLinePay;

			// NewebPay→NP後払い
			if (this.IsChangeNewebPayToNPAfterPay) return ReauthTypes.ChangeNewebPayToNPAfterPay;

			// NewebPay→決済無し
			if (this.IsChangeNewebPayToNoPayment) return ReauthTypes.ChangeNewebPayToNoPayment;

			// NewebPay→Boku
			if (this.IsChangeNewebPayToBoku) return ReauthTypes.ChangeNewebPayToBoku;

			// NewebPay全返品 (最終請求金額=0)
			if (this.IsNewebPayReturnAllItems) return ReauthTypes.NewebPayReturnAllItems;

			// NewebPay→PayPay
			if (this.IsChangeNewebPayToPayPal) return ReauthTypes.ChangeNewebPayToPayPay;

			// LINEPay→コンビニ後払い
			if (this.IsChangeLinePayToCvsDef) return ReauthTypes.ChangeLinePayToCvsDef;

			// LINEPay→GmoPost
			if (this.IsChangeLinePayToGmoPost) return ReauthTypes.ChangeLinePayToGmoPost;

			// LINEPay変更なし
			if (this.IsNoChangeLinePay) return ReauthTypes.NoChangeLinePay;

			// LINEPay→クレジットカード
			if (this.IsChangeLinePayToCredit) return ReauthTypes.ChangeLinePayToCredit;

			// LINE Pay→Amazon Pay
			if (this.IsChangeLinePayToAmazonPay) return ReauthTypes.ChangeLinePayToAmazonPay;

			// LINEPay→後付款(TriLink後払い)
			if (this.IsChangeLinePayToTriLinkAfterPay) return ReauthTypes.ChangeLinePayToTriLinkAfterPay;

			// LINEPay→Paidy Pay
			if (this.IsChangeLinePayToPaidyPay) return ReauthTypes.ChangeLinePayToPaidyPay;

			// LINEPay→代引き
			if (this.IsChangeLinePayToCollect) return ReauthTypes.ChangeLinePayToCollectOrOthers;

			// LINEPay→キャリア決済
			if (this.IsChangeLinePayToCarrier) return ReauthTypes.ChangeLinePayToCarrier;

			// LINEPay→決済無し
			if (this.IsChangeLinePayToNoPayment) return ReauthTypes.ChangeLinePayToNoPayment;

			// LINEPay→PayPal
			if (this.IsChangeLinePayToPayPal) return ReauthTypes.ChangeLinePayToPayPal;

			// LINEPay→Aftee
			if (this.IsChangeLinePayToAftee) return ReauthTypes.ChangeLinePayToAftee;

			// LINEPay→Atone
			if (this.IsChangeLinePayToAtone) return ReauthTypes.ChangeLinePayToAtone;

			// LINEPay全返品 (最終請求金額=0)
			if (this.IsLinePayReturnAllItems) return ReauthTypes.LinePayReturnAllItems;

			// LINEPay→コンビニ後払い
			if (this.IsChangeLinePayToCvsDef) return ReauthTypes.ChangeLinePayToCvsDef;

			// LINEPay→GmoPost
			if (this.IsChangeLinePayToGmoPost) return ReauthTypes.ChangeLinePayToGmoPost;

			// LINEPay→PayPay
			if (this.IsChangeLinePayToPayPay) return ReauthTypes.ChangeLinePayToPayPay;

			// LINEPay→コンビニ後払い
			if (this.IsChangeLinePayToBoku) return ReauthTypes.ChangeLinePayToBoku;

			// クレジットカード→NP後払い
			if (this.IsChangeCreditToNPAfterPay) return ReauthTypes.ChangeCreditToNPAfterPay;

			// コンビニ後払い→NP後払い
			if (this.IsChangeCvsDefToNPAfterPay) return ReauthTypes.ChangeCvsDefToNPAfterPay;

			// PayPal→NP後払い
			if (this.IsChangePayPalToNPAfterPay) return ReauthTypes.ChangePayPalToNPAfterPay;

			// 後付款(TriLink後払い)→NP後払い
			if (this.IsChangeTriLinkAfterPayToNPAfterPay) return ReauthTypes.ChangeTriLinkAfterPayToNPAfterPay;

			// キャリア決済→NP後払い
			if (this.IsChangeCarrierToNPAfterPay) return ReauthTypes.ChangeCarrierToNPAfterPay;

			// 代引き→NP後払い
			if (this.IsChangeCollectToNPAfterPay) return ReauthTypes.ChangeCollectOrOthersToNPAfterPay;

			// 決済無し→NP後払い
			if (this.IsChangeNoPaymentToNPAfterPay) return ReauthTypes.ChangeNoPaymentToNPAfterPay;

			// Amazon Pay→NP後払い
			if (this.IsChangeAmazonPayToNPAfterPay) return ReauthTypes.ChangeAmazonPayToNPAfterPay;

			// Paidy Pay→NP後払い
			if (this.IsChangePaidyPayToNPAfterPay) return ReauthTypes.ChangePaidyPayToNPAfterPay;

			// Atone→NP後払い
			if (this.IsChangeAtoneToNPAfterPay) return ReauthTypes.ChangeAtoneToNPAfterPay;

			// Aftee→NP後払い
			if (this.IsChangeAfteeToNPAfterPay) return ReauthTypes.ChangeAfteeToNPAfterPay;

			// LINEPay→NP後払い
			if (this.IsChangeLinePayToNPAfterPay) return ReauthTypes.ChangeLinePayToNPAfterPay;

			// NP後払い→ Pay変更なし
			if (this.IsNoChangeNPAfterPay) return ReauthTypes.NoChangeNPAfterPay;

			// NP後払い→クレジットカード
			if (this.IsChangeNPAfterPayToCredit) return ReauthTypes.ChangeNPAfterPayToCredit;

			// NP後払い→コンビニ後払い
			if (this.IsChangeNPAfterPayToCvsDef) return ReauthTypes.ChangeNPAfterPayToCvsDef;

			// NP後払い→GmoPost
			if (this.IsChangeNPAfterPayToGmoPost) return ReauthTypes.ChangeNPAfterPayToGmoPost;

			// NP後払い→Amazon Pay
			if (this.IsChangeNPAfterPayToAmazonPay) return ReauthTypes.ChangeNPAfterPayToAmazonPay;

			// NP後払い→後付款(TriLink後払い)
			if (this.IsChangeNPAfterPayToTriLinkAfterPay) return ReauthTypes.ChangeNPAfterPayToTriLinkAfterPay;

			// NP後払い→代引き
			if (this.IsChangeNPAfterPayToCollect) return ReauthTypes.ChangeNPAfterPayToCollectOrOthers;

			// NP後払い→Pay→キャリア決済
			if (this.IsChangeNPAfterPayToCarrier) return ReauthTypes.ChangeNPAfterPayToCarrier;

			// NP後払い→決済無し
			if (this.IsChangeNPAfterPayToNoPayment) return ReauthTypes.ChangeNPAfterPayToNoPayment;

			// NP後払い→PayPal
			if (this.IsChangeNPAfterPayToPayPal) return ReauthTypes.ChangeNPAfterPayToPayPal;

			// NP後払い→Atone
			if (this.IsChangeNPAfterPayToAtone) return ReauthTypes.ChangeNPAfterPayToAtone;

			// NP後払い→Aftee
			if (this.IsChangeNPAfterPayToAftee) return ReauthTypes.ChangeNPAfterPayToAftee;

			// NP後払い→Line Pay
			if (this.IsChangeNPAfterPayToLinePay) return ReauthTypes.ChangeNPAfterPayToLinePay;

			// NP後払い→Paidy Pay
			if (this.IsChangeNPAfterPayToPaidyPay) return ReauthTypes.ChangeNPAfterPayToPaidyPay;

			// NP後払い→PayPay
			if (this.IsChangeNPAfterPayToPayPay) return ReauthTypes.ChangeNPAfterPayToPayPay;

			// NP後払い→Boku
			if (this.IsChangeNPAfterPayToBoku) return ReauthTypes.ChangeNPAfterPayToBoku;

			// NP後払い→Pay全返品 (最終請求金額=0)
			if (this.IsNPAfterPayReturnAllItems) return ReauthTypes.NPAfterPayReturnAllItems;

			// PayPay変更なし
			if (this.IsNoChangePayPay) return ReauthTypes.NoChangePayPay;

			// PayPay→クレジットカード
			if (this.IsChangePayPayToCredit) return ReauthTypes.ChangePayPayToCredit;

			// PayPay→コンビニ後払い
			if (this.IsChangePayPayToCvsDef) return ReauthTypes.ChangePayPayToCvsDef;

			// PayPay→GmoPost
			if (this.IsChangePayPayToGmoPost) return ReauthTypes.ChangePayPayToGmoPost;

			// PayPay→代引き
			if (this.IsChangePayPayToCollect) return ReauthTypes.ChangePayPayToCollectOrOthers;

			// PayPay→キャリア決済
			if (this.IsChangePayPayToCarrier) return ReauthTypes.ChangePayPayToCarrier;

			// PayPay→Amazon Pay
			if (this.IsChangePayPayToAmazonPay) return ReauthTypes.ChangePayPayToAmazonPay;

			// PayPay→決済無し
			if (this.IsChangePayPayToNoPayment) return ReauthTypes.ChangePayPayToNoPayment;

			// PayPay全返品
			if (this.IsPayPayReturnAllItems) return ReauthTypes.PayPayReturnAllItems;

			// GMOアトカラ変更なし
			if (this.IsNoChangeGmoAtokara) return ReauthTypes.NoChangeGmoAtokara;

			// GMOアトカラ→クレジットカード
			if (this.IsChangeGmoAtokaraToCredit) return ReauthTypes.ChangeGmoAtokaraToCredit;

			// GMOアトカラ→コンビニ後払い
			if (this.IsChangeGmoAtokaraToCvsDef) return ReauthTypes.ChangeGmoAtokaraToCvsDef;

			// GMOアトカラ→キャリア決済
			if (this.IsChangeGmoAtokaraToCarrier) return ReauthTypes.ChangeGmoAtokaraToCarrier;

			// GMOアトカラ→代引き
			if (this.IsChangeGmoAtokaraToCollect) return ReauthTypes.ChangeGmoAtokaraToCollectOrOthers;

			// GMOアトカラ→PayPal
			if (this.IsChangeGmoAtokaraToPayPal) return ReauthTypes.ChangeGmoAtokaraToPayPal;

			// GMOアトカラ→Amazon Pay
			if (this.IsChangeGmoAtokaraToAmazonPay) return ReauthTypes.ChangeGmoAtokaraToAmazonPay;

			// GMOアトカラ→後付款(TriLink後払い)
			if (this.IsChangeGmoAtokaraToTriLinkAfterPay) return ReauthTypes.ChangeGmoAtokaraToTriLinkAfterPay;

			// GMOアトカラ→Paidy Pay
			if (this.IsChangeGmoAtokaraToPaidyPay) return ReauthTypes.ChangeGmoAtokaraToPaidyPay;

			// GMOアトカラ→Aftee
			if (this.IsChangeGmoAtokaraToAftee) return ReauthTypes.ChangeGmoAtokaraToAftee;

			// GMOアトカラ→Atone
			if (this.IsChangeGmoAtokaraToAtone) return ReauthTypes.ChangeGmoAtokaraToAtone;

			// GMOアトカラ→LINEPay
			if (this.IsChangeGmoAtokaraToLinePay) return ReauthTypes.ChangeGmoAtokaraToLinePay;

			// GMOアトカラ→NP後払い
			if (this.IsChangeGmoAtokaraToNPAfterPay) return ReauthTypes.ChangeGmoAtokaraToNPAfterPay;

			// GMOアトカラ→決済無し
			if (this.IsChangeGmoAtokaraToNoPayment) return ReauthTypes.ChangeGmoAtokaraToNoPayment;

			// 変更なしかつ、外部決済なし
			if (this.IsNoChangeNotExternalPayment) return ReauthTypes.None;

			// PayPay変更なし
			if (this.IsNoChangePayPay) return ReauthTypes.NoChangePayPay;

			// PayPay→クレジットカード
			if (this.IsChangePayPayToCredit) return ReauthTypes.ChangePayPayToCredit;

			// PayPay→コンビニ後払い
			if (this.IsChangePayPayToCvsDef) return ReauthTypes.ChangePayPayToCvsDef;

			// PayPay→GmoPost
			if (this.IsChangePayPayToGmoPost) return ReauthTypes.ChangePayPayToGmoPost;

			// PayPay→代引き
			if (this.IsChangePayPayToCollect) return ReauthTypes.ChangePayPayToCollectOrOthers;

			// PayPay→キャリア決済
			if (this.IsChangePayPayToCarrier) return ReauthTypes.ChangePayPayToCarrier;

			// PayPay→Amazon Pay
			if (this.IsChangePayPayToAmazonPay) return ReauthTypes.ChangePayPayToAmazonPay;

			// PayPay→決済無し
			if (this.IsChangePayPayToNoPayment) return ReauthTypes.ChangePayPayToNoPayment;

			// PayPay全返品
			if (this.IsPayPayReturnAllItems) return ReauthTypes.PayPayReturnAllItems;

			// Boku変更なし
			if (this.IsNoChangeBoku) return ReauthTypes.NoChangeBoku;

			// Boku→クレジットカード
			if (this.IsChangeNewebPayToCredit) return ReauthTypes.ChangeBokuToCredit;

			// Boku→コンビニ後払い
			if (this.IsChangeBokuToCvsDef) return ReauthTypes.ChangeBokuToCvsDef;

			// Boku→キャリア決済
			if (this.IsChangeBokuToCarrier) return ReauthTypes.ChangeBokuToCarrier;

			// Boku→代引き
			if (this.IsChangeBokuToCollect) return ReauthTypes.ChangeBokuToCollectOrOthers;

			// Boku→PayPal
			if (this.IsChangeBokuToPayPal) return ReauthTypes.ChangeBokuToPayPal;

			// Boku→Amazon Pay
			if (this.IsChangeBokuToAmazonPay) return ReauthTypes.ChangeBokuToAmazonPay;

			// Boku→後付款(TriLink後払い)
			if (this.IsChangeBokuToTriLinkAfterPay) return ReauthTypes.ChangeBokuToTriLinkAfterPay;

			// Boku→Paidy Pay
			if (this.IsChangeBokuToPaidyPay) return ReauthTypes.ChangeBokuToPaidyPay;

			// Boku→Aftee
			if (this.IsChangeBokuToAftee) return ReauthTypes.ChangeBokuToAftee;

			// Boku→Atone
			if (this.IsChangeBokuToAtone) return ReauthTypes.ChangeBokuToAtone;

			// Boku→LINEPay
			if (this.IsChangeBokuToLinePay) return ReauthTypes.ChangeBokuToLinePay;

			// Boku→NP後払い
			if (this.IsChangeBokuToNPAfterPay) return ReauthTypes.ChangeBokuToNPAfterPay;

			// Boku→決済無し
			if (this.IsChangeBokuToNoPayment) return ReauthTypes.ChangeBokuToNoPayment;

			// 変更なしかつ、外部決済なし
			if (this.IsNoChangeNotExternalPayment) return ReauthTypes.None;

			// 上記以外の決済
			return ReauthTypes.None;
		}

		/// <summary>
		/// 注文商品が全て返品されているか？
		/// </summary>
		/// <returns>全て返品している：true、全て返品していない：false</returns>
		private bool InspectReturnAllItems()
		{
			// 返品以外はfalseを返す
			if (this.OrderActionType != OrderActionTypes.Return) return false;

			// 返品交換含む注文情報に返品注文情報を追加
			var relatedOrders = DomainFacade.Instance.OrderService.GetRelatedOrders(
					this.OrderNew.OrderIdOrg,
					this.Accessor)
				.ToList();
			var orderNewModify = this.OrderNew.Clone();
			orderNewModify.OrderReturnExchangeStatus = Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE;
			relatedOrders.Add(orderNewModify);

			// 全返品している？
			return DomainFacade.Instance.OrderService.InspectReturnAllItems(relatedOrders.ToArray(), this.Accessor);
		}

		/// <summary>
		/// コンビニ後払いで外部連携ステータスが「入金済み」かどうか（元注文・返品・交換いずれか）
		/// </summary>
		/// <returns>true：入金済み、false：未入金</returns>
		private bool CheckCsvDefPayComplete()
		{
			// 元注文・返品・交換注文取得
			var orderId = string.IsNullOrEmpty(this.OrderOld.OrderIdOrg) ? this.OrderOld.OrderId : this.OrderOld.OrderIdOrg;
			var relatedOrders = DomainFacade.Instance.OrderService.GetRelatedOrders(orderId, this.Accessor);
			// いずれか入金済みか
			return relatedOrders.Any(order => (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& ((order.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP)
					|| (order.OrderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)));
		}

		/// <summary>
		/// 再与信アクション（クレジットカード与信 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthCreditCardOrDoNothingAction()
		{
			// 下記の場合は何もしない
			// ・コンビニ後払いかつ外部決済ステータスが「入金済み」
			// ・カード保存不可
			// ・カード支番無し
			// ・最終請求金額が0
			if (this.IsCvsDefPayComplete
				|| (OrderCommon.CreditCardRegistable == false)
				|| (this.OrderNew.CreditBranchNo == null)
				|| (this.OrderNew.LastBilledAmount <= 0)
				|| (Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN) == false))
			{
				return CreateDoNothingAction();
			}

			var requestActionParams = new ReauthCreditCardAction.ReauthActionParams(this.OrderNew);
			// 楽天クレジットの場合のみ
			// 金額変更処理時に決済注文IDの再発行を行わないため、条件追加
			if ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
				&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT))
			{
				requestActionParams.IsExecModify = (this.OrderActionType == OrderActionTypes.Modify);
			}

			// ベリトランスクレジットの場合のみ
			// 返品交換時に返品前の決済情報が必要な為追加
			if ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
				&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT))
			{
				requestActionParams.OldPaymentKbn = this.OrderOld.OrderPaymentKbn;
			}

			return CreateReauthCreditCardAction(requestActionParams);
		}

		/// <summary>
		/// 再与信アクション（クレジットカード売上確定 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateSalesCreditCardOrDoNothingAction()
		{
			// 下記の場合は何もしない
			// ・返品時クレジットカードの自動売上確定がOFF
			// ・コンビニ後払いかつ外部決済ステータスが「入金済み」
			// ・カード支番無し
			// ・最終請求金額が0
			// ・カード決済区分がYamatoKwc
			// ・注文編集
			// ・注文交換（交換注文キャンセル含まない）
			if ((Constants.PAYMENT_SETTING_CREDIT_RETURN_AUTOSALES_ENABLED == false)
				|| this.IsCvsDefPayComplete
				|| (this.OrderNew.CreditBranchNo == null)
				|| (this.OrderNew.LastBilledAmount <= 0)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc)
				|| (this.OrderActionType == OrderActionTypes.Modify)
				|| (this.OrderActionType == OrderActionTypes.Exchange)
				|| ((Constants.PAYMENT_SETTING_GMO_PAYMENTMETHOD == Constants.GmoCreditCardPaymentMethod.Capture) && (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo))) return CreateDoNothingAction();

			return CreateSalesCreditCardAction(new SalesCreditCardAction.ReauthActionParams(this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（クレジットカードキャンセル or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelCreditCardOrDoNothingAction()
		{
			// 決済取引カードIDが「ブランク」の場合は何もしない
			// GMO,Veritrans3dsの場合、決済取引IDが入っているが与信取得されていないケースがあるため、与信取得日が空の場合は何もしない
			if (string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| (Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN) == false)
				|| ((this.OrderOld.ExternalPaymentAuthDate == null)
					&& ((this.OrderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE)
						|| (this.OrderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED)
						&& ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo)
								&& Constants.PAYMENT_SETTING_GMO_3DSECURE)
							|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
								&& Constants.PAYMENT_VERITRANS4G_CREDIT_3DSECURE))))
			{
				return CreateDoNothingAction();
			}

			return CreateCancelCreditCardAction(new CancelCreditCardAction.ReauthActionParams(this.OrderOld));
		}

		/// <summary>
		/// 再与信アクション（コンビニ後払い与信 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthCvsDefOrDoNothingAction()
		{
			// コンビニ後払いかつ外部決済ステータスが「入金済み」またはヤマト後払いSMS認証連携決済の場合は何もしない
			if (this.IsCvsDefPayComplete
				|| (this.OrderNew.LastBilledAmount <= 0)
				|| (Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN) == false)
				|| (Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF == OrderNew.OrderPaymentKbn))
			{
				return CreateDoNothingAction();
			}

			return CreateReauthCvsDefAction(new ReauthCvsDefAction.ReauthActionParams(this.OrderNew));
		}

		/// <summary>
		/// Create reauth action (create Gmokb transaction or do nothing)
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthGmoPostOrDoNothingAction()
		{
			if (this.OrderNew.LastBilledAmount <= 0)
			{
				return CreateDoNothingAction();
			}

			return CreateReauthGmoPostAction(new ReauthGmoPostAction.ReauthGmoPostActionParams(this.OrderNew));
		}

		/// <summary>
		/// Create reauth action (edit Gmokb transaction or do nothing)
		/// </summary>
		/// <param name="orderModel">注文情報モデル</param>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateEditGmoPostOrDoNothingAction(OrderModel orderModel = null)
		{
			if (this.OrderNew.LastBilledAmount <= 0)
			{
				return CreateDoNothingAction();
			}

			if (orderModel != null)
			{
				return CreateEditGmoPostAction(new EditGmoPostAction.EditGmoPostActionParams(orderModel, isNeedCallBillingConfirm: true));
			}

			return CreateEditGmoPostAction(new EditGmoPostAction.EditGmoPostActionParams(this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（コンビニ後払いキャンセル or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelCvsDefOrDoNothingAction()
		{
			// 決済注文IDが「ブランク」の場合は何もしない
			// コンビニ後払いかつ外部決済ステータスが「入金済み」の場合は何もしない
			if (string.IsNullOrEmpty(this.OrderOld.PaymentOrderId)
				|| this.IsCvsDefPayComplete
				|| (Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN) == false))
			{
				return CreateDoNothingAction();
			}

			return CreateCancelCvsDefAction(new CancelCvsDefAction.ReauthActionParams(this.OrderOld));
		}

		/// <summary>
		/// Create reauth action (cancel Gmokb transaction or do nothing)
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelGmoPostOrDoNothingAction()
		{
			if (string.IsNullOrEmpty(this.OrderOld.PaymentOrderId))
			{
				return CreateDoNothingAction();
			}

			return CreateCancelGmoPostAction(new CancelGmoPostAction.CancelGmoPostActionParams(this.OrderOld));
		}

		/// <summary>
		/// Create reauth action (modify or cancel Gmokb billing or do nothing)
		/// </summary>
		/// <param name="isReturnAll">全て返却</param>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateModifyCancelBillingGmoPostOrDoNothingAction(bool isReturnAll)
		{
			if (string.IsNullOrEmpty(this.OrderOld.PaymentOrderId))
			{
				return CreateDoNothingAction();
			}

			return CreateModifyCancelBillingGmoPostAction(new ModifyCancelBillingGmoPostAction.ModifyCancelBillingGmoPostActionParams(this.OrderOld, isReturnAll));
		}

		/// <summary>
		/// Create reauth action (reduce Gmokb transaction or do nothing)
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReduceGmoPostOrDoNothingAction()
		{
			if ((string.IsNullOrEmpty(this.OrderNew.PaymentOrderId))
				|| (this.OrderNew.LastBilledAmount <= 0))
			{
				return CreateDoNothingAction();
			}

			var finalOrder = OrderCommon.GetFinalOrderAfterReturnOrExchange(this.OrderNew, this.OrderOld);
			return CreateReduceGmoPostAction(new ReduceGmoPostAction.ReduceGmoPostActionParams(finalOrder));
		}

		/// <summary>
		/// 再与信アクション（コンビニ後払い請求金額減額 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReduceCvsDefOrDoNothingAction()
		{
			// 決済取引カードIDが「ブランク」の場合は何もしない
			// コンビニ後払いかつ外部決済ステータスが「入金済み」の場合は何もしない
			if (string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| this.IsCvsDefPayComplete
				|| (this.OrderNew.LastBilledAmount <= 0)
				|| (Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN) == false))
			{
				return CreateDoNothingAction();
			}

			return CreateReduceCvsDefAction(new ReduceCvsDefAction.ReauthActionParams(this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（コンビニ後払い請求書再発行 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReprintCvsDefOrDoNothingAction()
		{
			// 決済取引カードIDが「ブランク」の場合は何もしない
			if (string.IsNullOrEmpty(this.OrderOld.CardTranId)) return CreateDoNothingAction();

			// コンビニ後払いかつ「出荷報告連携済」の場合は何もしない
			if (this.IsCvsDefShipRegistComp) return CreateDoNothingAction();

			return CreateReprintCvsDefAction(new ReprintCvsDefAction.ReauthActionParams(this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（Atodeneかつ与信延長ではない）インスタンス作成
		/// </summary>
		/// <param name="originalOrderId">再与信前のもとの受注ID</param>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthAtodeneExcludingAuthExtensionDefAction(string originalOrderId)
		{
			// コンビニ後払いかつ外部決済ステータスが「入金済み」の場合は何もしない
			if (IsTargetToCvsReauth())
			{
				return CreateDoNothingAction();
			}

			return CreateReauthAtodeneExcludingAuthExtensionDefAction(
				new ReauthCvsDefAction.ReauthActionParams(this.OrderNew),
				originalOrderId);
		}

		/// <summary>
		/// 再与信の対象かどうかの確認
		/// </summary>
		/// <returns>再与信対象かどうか</returns>
		private bool IsTargetToCvsReauth()
		{
			var result =
				this.IsCvsDefPayComplete
					|| (this.OrderNew.LastBilledAmount <= 0)
					|| (Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN) == false);
			return result;
		}

		/// <summary>
		/// 再与信アクション（後付款(TriLink後払い)与信 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthTriLinkAfterPayOrDoNothingAction()
		{
			// 下記の場合は何もしない
			// ・コンビニ後払いかつ外部決済ステータスが「入金済み」
			// ・最終請求金額が0
			if ((this.IsCvsDefPayComplete)
				|| (this.OrderNew.LastBilledAmount <= 0))
			{
				return CreateDoNothingAction();
			}

			return CreateReauthTriLinkAfterPayAction(new ReauthTriLinkAfterPayAction.ReauthActionParams(this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（後付款(TriLink後払い)キャンセル or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelTriLinkAfterPayOrDoNothingAction()
		{
			// 下記の場合は何もしない
			// ・決済注文IDが「ブランク」
			// ・コンビニ後払いかつ外部決済ステータスが「入金済み」
			if ((string.IsNullOrEmpty(this.OrderOld.PaymentOrderId))
				|| (this.IsCvsDefPayComplete))
			{
				return CreateDoNothingAction();
			}

			return CreateCancelTriLinkAfterPayAction(new CancelTriLinkAfterPayAction.ReauthActionParams(this.OrderOld));
		}

		/// <summary>
		/// 再与信アクション（後付款(TriLink後払い)注文情報更新 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReduceTriLinkAfterPayOrDoNothingAction()
		{
			// 下記の場合は何もしない
			// ・決済注文IDが「ブランク」の場合は何もしない
			// ・コンビニ後払いかつ外部決済ステータスが「入金済み」の場合は何もしない
			// ・最終請求金額が0
			if ((string.IsNullOrEmpty(this.OrderOld.PaymentOrderId))
				|| (this.IsCvsDefPayComplete)
				|| (this.OrderNew.LastBilledAmount <= 0))
			{
				return CreateDoNothingAction();
			}

			return CreateUpdateTriLinkAfterPayAction(new UpdateTriLinkAfterPayAction.ReauthActionParams(this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（PayPal与信 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthPayPalOrDoNothingAction()
		{
			// 下記の場合は何もしない
			// ・コンビニ後払いかつ外部決済ステータスが「入金済み」
			// ・カード保存不可
			// ・カード支番無し
			// ・最終請求金額が0
			if (this.IsCvsDefPayComplete
				|| (this.OrderNew.CreditBranchNo == null)
				|| (this.OrderNew.LastBilledAmount <= 0))
			{
				return CreateDoNothingAction();
			}

			return CreateReauthPayPalAction(new ReauthPayPalAction.ReauthActionParams(this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（PayPal売上確定 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateSalesPayPalOrDoNothingAction()
		{
			// 下記の場合は何もしない
			// ・返品時PayPalの自動売上確定がOFF
			// ・コンビニ後払いかつ外部決済ステータスが「入金済み」
			// ・カード支番無し
			// ・最終請求金額が0
			// ・注文編集
			// ・注文返品、交換（交換注文キャンセル含まない）
			if (this.IsCvsDefPayComplete
				|| (this.OrderNew.CreditBranchNo == null)
				|| (this.OrderNew.LastBilledAmount <= 0)
				|| (this.OrderActionType == OrderActionTypes.Modify)
				|| (this.OrderActionType == OrderActionTypes.Return)
				|| (this.OrderActionType == OrderActionTypes.Exchange)) return CreateDoNothingAction();

			return CreateSalesPayPalAction(new SalesPayPalAction.ReauthActionParams(this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（PayPalキャンセル or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelPayPalOrDoNothingAction()
		{
			// 決済取引カードIDが「ブランク」の場合は何もしない
			if (string.IsNullOrEmpty(this.OrderOld.CardTranId))
			{
				return CreateDoNothingAction();
			}

			return CreateCancelPayPalAction(new CancelPayPalAction.ReauthActionParams(this.OrderOld));
		}

		/// <summary>
		/// 再与信アクション（SBPSマルチペイメントキャンセル or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelSBPSMultiPaymentOrDoNothingAction()
		{
			// 決済取引カードIDが「ブランク」の場合、又は、キャンセル未対応の場合は何もしない
			if (string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| (Constants.REAUTH_CANCEL_PAYMENT_LIST.Contains(this.OrderOld.OrderPaymentKbn) == false)) return CreateDoNothingAction();

			return CreateCancelSBPSMultiPaymentAction(new CancelSBPSMultiPaymentAction.ReauthActionParams(this.OrderOld));
		}

		/// <summary>
		/// 再与信アクション（何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private DoNothingAction CreateDoNothingAction()
		{
			return new DoNothingAction(new DoNothingAction.ReauthActionParams());
		}

		/// <summary>
		/// 再与信アクション（クレジットカード与信）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthCreditCardAction CreateReauthCreditCardAction(
			ReauthCreditCardAction.ReauthActionParams reauthActionParams)
		{
			return new ReauthCreditCardAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（クレジットカードキャンセル）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelCreditCardAction CreateCancelCreditCardAction(
			CancelCreditCardAction.ReauthActionParams reauthActionParams)
		{
			return new CancelCreditCardAction(reauthActionParams, this.Accessor);
		}

		/// <summary>
		/// 再与信アクション（クレジットカード売上確定）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private SalesCreditCardAction CreateSalesCreditCardAction(
			SalesCreditCardAction.ReauthActionParams reauthActionParams)
		{
			return new SalesCreditCardAction(reauthActionParams);
		}

		/// <summary>
		/// Create Sales Atone Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>Sales Atone Action</returns>
		private SalesAtoneAction CreateSalesAtoneAction(
			SalesAtoneAction.ReauthActionParams reauthActionParams)
		{
			return new SalesAtoneAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（コンビニ後払い与信）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthCvsDefAction CreateReauthCvsDefAction(
			ReauthCvsDefAction.ReauthActionParams reauthActionParams)
		{
			return new ReauthCvsDefAction(reauthActionParams);
		}

		/// <summary>
		/// Create reauth action (create Gmokb transaction)
		/// </summary>
		/// <param name="reauthGmoPostActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthGmoPostAction CreateReauthGmoPostAction(
			ReauthGmoPostAction.ReauthGmoPostActionParams reauthGmoPostActionParams)
		{
			return new ReauthGmoPostAction(reauthGmoPostActionParams);
		}

		/// <summary>
		/// Create reauth action (edit Gmokb transaction)
		/// </summary>
		/// <param name="editGmoPostActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private EditGmoPostAction CreateEditGmoPostAction(
			EditGmoPostAction.EditGmoPostActionParams editGmoPostActionParams)
		{
			return new EditGmoPostAction(editGmoPostActionParams);
		}

		/// <summary>
		/// Create reauth action (edit Gmokb transaction)
		/// </summary>
		/// <param name="reduceGmoPostActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReduceGmoPostAction CreateReduceGmoPostAction(
			ReduceGmoPostAction.ReduceGmoPostActionParams reduceGmoPostActionParams)
		{
			return new ReduceGmoPostAction(reduceGmoPostActionParams);
		}

		/// <summary>
		/// 再与信アクション（コンビニ後払いキャンセル）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelCvsDefAction CreateCancelCvsDefAction(
			CancelCvsDefAction.ReauthActionParams reauthActionParams)
		{
			return new CancelCvsDefAction(reauthActionParams);
		}

		/// <summary>
		/// Create reauth action (cancel Gmokb transaction)
		/// </summary>
		/// <param name="cancelGmoPostActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelGmoPostAction CreateCancelGmoPostAction(
			CancelGmoPostAction.CancelGmoPostActionParams cancelGmoPostActionParams)
		{
			return new CancelGmoPostAction(cancelGmoPostActionParams);
		}

		/// <summary>
		/// Create reauth action (modify or cancel Gmokb billing)
		/// </summary>
		/// <param name="modifyCancelBillingGmoPostActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ModifyCancelBillingGmoPostAction CreateModifyCancelBillingGmoPostAction(
			ModifyCancelBillingGmoPostAction.ModifyCancelBillingGmoPostActionParams modifyCancelBillingGmoPostActionParams)
		{
			return new ModifyCancelBillingGmoPostAction(modifyCancelBillingGmoPostActionParams);
		}

		/// <summary>
		/// 再与信アクション（Atodene後払い）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <param name="orignalOrderId">再与信の元の受注ID</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthAtodeneExcludingAuthExtensionDefAction CreateReauthAtodeneExcludingAuthExtensionDefAction(
			ReauthCvsDefAction.ReauthActionParams reauthActionParams,
			string orignalOrderId)
		{
			return new ReauthAtodeneExcludingAuthExtensionDefAction(reauthActionParams, orignalOrderId);
		}

		/// <summary>
		/// 再与信アクション（PayPal与信）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthPayPalAction CreateReauthPayPalAction(
			ReauthPayPalAction.ReauthActionParams reauthActionParams)
		{
			return new ReauthPayPalAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（PayPalキャンセル）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelPayPalAction CreateCancelPayPalAction(
			CancelPayPalAction.ReauthActionParams reauthActionParams)
		{
			return new CancelPayPalAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（PayPal売上確定）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private SalesPayPalAction CreateSalesPayPalAction(
			SalesPayPalAction.ReauthActionParams reauthActionParams)
		{
			return new SalesPayPalAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（SBPSマルチペイメントキャンセル）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelSBPSMultiPaymentAction CreateCancelSBPSMultiPaymentAction(
			CancelSBPSMultiPaymentAction.ReauthActionParams reauthActionParams)
		{
			return new CancelSBPSMultiPaymentAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（コンビニ後払い請求金額減額）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReduceCvsDefAction CreateReduceCvsDefAction(
			ReduceCvsDefAction.ReauthActionParams reauthActionParams)
		{
			return new ReduceCvsDefAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（コンビニ後払い請求書再発行）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReprintCvsDefAction CreateReprintCvsDefAction(
			ReprintCvsDefAction.ReauthActionParams reauthActionParams)
		{
			return new ReprintCvsDefAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay与信 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthAmazonPayOrDoNothingAction()
		{
			if (Constants.AMAZON_PAYMENT_CV2_ENABLED) return CreateReauthAmazonPayCv2OrDoNothingAction();
			// 下記の場合は何もしない
			// ・変更前注文が交換注文
			// ・最終請求金額が0
			// ・コンビニ後払いかつ外部決済ステータスが「入金済み」
			// ・「Amazon Pay変更なし」かつオンライン決済ステータス「売上確定済」かつ減額
			if (this.OrderOld.IsExchangeOrder
				|| (this.OrderNew.LastBilledAmount <= 0)
				|| this.IsCvsDefPayComplete
				|| (this.IsNoChangeAmazonPay
					&& (this.OrderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
					&& this.IsReducedBilledAmount))
			{
				return CreateDoNothingAction();
			}

			// 「Amazon Payで決済注文IDに変更なし」かつオンライン決済ステータス「未連携」の場合与信前キャンセルを行う
			if (this.IsNoChangeAmazonPaymentOrderId
				&& (OrderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE))
			{
				this.PreCancelFlg = true;
			}

			return CreateReauthAmazonPayAction(new ReauthAmazonPayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（Amazon Payキャンセル or 何もしない）インスタンス作成
		/// </summary>
		/// <param name="isReauth">Amazon Pay与信処理を実施するか</param>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelAmazonPayOrDoNothingAction(bool isReauth = false)
		{
			if (Constants.AMAZON_PAYMENT_CV2_ENABLED) return CreateCancelAmazonPayCv2OrDoNothingAction(isReauth);
			// 下記の場合は何もしない
			// ・変更前注文が交換注文
			// ・決済取引カードIDが「ブランク」
			// ・「Amazon Payで決済注文IDに変更なし」かつオンライン決済ステータスが「売上確定済」
			// ・返品注文
			// ・再与信取得時のキャンセルアクション
			if (this.OrderOld.IsExchangeOrder
				|| string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| ((this.IsNoChangeAmazonPaymentOrderId)
					&& (OrderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED))
				|| this.OrderOld.IsReturnOrder
				|| isReauth)
			{
				return CreateDoNothingAction();
			}

			return CreateCancelAmazonPayAction(new CancelAmazonPayAction.ReauthActionParams(this.OrderOld));
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay売上確定 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateSalesAmazonPayOrDoNothingAction()
		{
			if (Constants.AMAZON_PAYMENT_CV2_ENABLED) return CreateSalesAmazonPayCv2OrDoNothingAction();
			// 下記の場合何もしない
			// ・変更前注文が交換注文
			// ・「Amazon Pay変更なし」でないかつ注文編集
			// ・最終請求金額が0
			// ・コンビニ後払いかつ外部決済ステータスが「入金済み」
			// ・「Amazon Pay変更なし」かつオンライン決済ステータス「売上確定済」かつ増額でない
			// ・即時売上でない(返品時以外)
			// ・返品時、自動売上確定ではない
			if (this.OrderOld.IsExchangeOrder
				|| ((this.IsNoChangeAmazonPay == false)
					&& (this.OrderActionType == OrderActionTypes.Modify))
				|| (this.OrderNew.LastBilledAmount <= 0)
				|| this.IsCvsDefPayComplete
				|| (this.IsNoChangeAmazonPay
					&& (this.OrderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
					&& (this.IsIncreasedBilledAmount == false))
				|| ((this.OrderActionType != OrderActionTypes.Return)
					&& (Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW == false))
				|| ((this.OrderActionType != OrderActionTypes.Return)
					&& (Constants.PAYMENT_AMAZON_PAYMENT_RETURN_AUTOSALES_ENABLED == false))
				)
			{
				return CreateDoNothingAction();
			}

			return CreateSalesAmazonPayAction(new SalesAmazonPayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay返金 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateRefundAmazonPayOrDoNothingAction()
		{
			if (Constants.AMAZON_PAYMENT_CV2_ENABLED) return CreateRefundAmazonPayCv2OrDoNothingAction();
			// 下記の場合は何もしない
			// ・変更前注文が交換注文
			// ・決済取引カードIDが「ブランク」
			// ・Amazon Payで決済注文IDに変更あり
			// ・オンライン決済ステータスが「売上確定済」でない かつ 返品注文でない
			// ・減額でない
			if (this.OrderOld.IsExchangeOrder
				|| string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| this.IsChangeAmazonPaymentOrderId
				|| ((OrderOld.OnlinePaymentStatus != Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
					&& (this.OrderOld.IsReturnOrder == false))
				|| (this.IsReducedBilledAmount == false))
			{
				return CreateDoNothingAction();
			}

			return CreateRefundAmazonPayAction(new RefundAmazonPayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay与信）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthAmazonPayAction CreateReauthAmazonPayAction(
			ReauthAmazonPayAction.ReauthActionParams reauthActionParams)
		{
			return new ReauthAmazonPayAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（Amazon Payキャンセル）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelAmazonPayAction CreateCancelAmazonPayAction(
			CancelAmazonPayAction.ReauthActionParams reauthActionParams)
		{
			return new CancelAmazonPayAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay売上確定）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private SalesAmazonPayAction CreateSalesAmazonPayAction(
			SalesAmazonPayAction.ReauthActionParams reauthActionParams)
		{
			return new SalesAmazonPayAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay返金）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private RefundAmazonPayAction CreateRefundAmazonPayAction(
			RefundAmazonPayAction.ReauthActionParams reauthActionParams)
		{
			return new RefundAmazonPayAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay(CV2)与信 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthAmazonPayCv2OrDoNothingAction()
		{
			var facade = ExternalApiFacade.Instance.AmazonCv2ApiFacade;
			var oldCharge = facade.GetCharge(this.OrderOld.CardTranId);
			var newCharge = facade.GetCharge(this.OrderNew.CardTranId);

			// 下記の場合は何もしない
			// ・最終請求金額が0
			// ・支払方法がAmazonPaymentではない
			// ・住所の変更で新旧amazonリファレンスIDが同時に存在する
			if ((this.OrderNew.LastBilledAmount <= 0)
				|| (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn) == false)
				|| (oldCharge.ChargePermissionId != newCharge.ChargePermissionId))
			{
				return CreateDoNothingAction();
			}

			// 「Amazon Pay(CV2)で決済注文IDに変更なし」かつオンライン決済ステータス「未連携」の場合与信前キャンセルを行う
			if (this.IsNoChangeAmazonPaymentOrderId
				&& (OrderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE))
			{
				this.PreCancelFlg = true;
			}

			return CreateReauthAmazonPayCv2Action(new ReauthAmazonPayCv2Action.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay(CV2)キャンセル）インスタンス作成
		/// </summary>
		/// <param name="isReauth">Amazon Pay与信処理を実施するか</param>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelAmazonPayCv2OrDoNothingAction(bool isReauth = false)
		{
			var isCancelActionExecutable = (isReauth == false)
				&& (this.IsReducedBilledAmount
					|| (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn) == false));
			return isCancelActionExecutable
				? CreateCancelAmazonPayCv2Action(new CancelAmazonPayCv2Action.ReauthActionParams(this.OrderOld))
				: (IReauthAction)CreateDoNothingAction();
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay(CV2)売上確定 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateSalesAmazonPayCv2OrDoNothingAction()
		{
			// 下記の場合何もしない
			// ・変更前注文が交換注文
			// ・「Amazon Pay(CV2)変更なし」でないかつ注文編集
			// ・最終請求金額が0
			// ・コンビニ後払いかつ外部決済ステータスが「入金済み」
			// ・「Amazon Pay(CV2)変更なし」かつオンライン決済ステータス「売上確定済」かつ増額でない
			// ・即時売上でない(返品時以外)
			// ・返品時、自動売上確定ではない
			if (this.OrderOld.IsExchangeOrder
				|| ((this.IsNoChangeAmazonPay == false)
					&& (this.OrderActionType == OrderActionTypes.Modify))
				|| (this.OrderNew.LastBilledAmount <= 0)
				|| this.IsCvsDefPayComplete
				|| (this.IsNoChangeAmazonPay
					&& (this.OrderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
					&& (this.IsIncreasedBilledAmount == false))
				|| ((this.OrderActionType != OrderActionTypes.Return)
					&& (Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW == false))
				|| ((this.OrderActionType == OrderActionTypes.Return)
					&& (Constants.PAYMENT_AMAZON_PAYMENT_RETURN_AUTOSALES_ENABLED == false))
				)
			{
				return CreateDoNothingAction();
			}

			return CreateSalesAmazonPayCv2Action(new SalesAmazonPayCv2Action.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay(CV2)返金 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateRefundAmazonPayCv2OrDoNothingAction()
		{
			// 下記の場合は何もしない
			// ・変更前注文が交換注文
			// ・決済取引カードIDが「ブランク」
			// ・Amazon Pay(CV2)で決済注文IDに変更あり
			// ・オンライン決済ステータスが「売上確定済」でない
			// ・新注文の決済種別がAmazonPay
			if (this.OrderOld.IsExchangeOrder
				|| string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| this.IsChangeAmazonPaymentOrderId
				|| (OrderOld.OnlinePaymentStatus != Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				|| OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn))
			{
				return CreateDoNothingAction();
			}

			return CreateRefundAmazonPayCv2Action(new RefundAmazonPayCv2Action.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay(CV2)与信）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthAmazonPayCv2Action CreateReauthAmazonPayCv2Action(
			ReauthAmazonPayCv2Action.ReauthActionParams reauthActionParams)
		{
			return new ReauthAmazonPayCv2Action(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay(CV2)キャンセル）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelAmazonPayCv2Action CreateCancelAmazonPayCv2Action(
			CancelAmazonPayCv2Action.ReauthActionParams reauthActionParams)
		{
			return new CancelAmazonPayCv2Action(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay(CV2)売上確定）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private SalesAmazonPayCv2Action CreateSalesAmazonPayCv2Action(
			SalesAmazonPayCv2Action.ReauthActionParams reauthActionParams)
		{
			return new SalesAmazonPayCv2Action(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（Amazon Pay(CV2)返金）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private RefundAmazonPayCv2Action CreateRefundAmazonPayCv2Action(
			RefundAmazonPayCv2Action.ReauthActionParams reauthActionParams)
		{
			return new RefundAmazonPayCv2Action(reauthActionParams);
		}


		/// <summary>
		/// 再与信アクション（後付款(TriLink後払い)与信）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthTriLinkAfterPayAction CreateReauthTriLinkAfterPayAction(
			ReauthTriLinkAfterPayAction.ReauthActionParams reauthActionParams)
		{
			return new ReauthTriLinkAfterPayAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（後付款(TriLink後払い)キャンセル）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelTriLinkAfterPayAction CreateCancelTriLinkAfterPayAction(
			CancelTriLinkAfterPayAction.ReauthActionParams reauthActionParams)
		{
			return new CancelTriLinkAfterPayAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（後付款(TriLink後払い)注文情報更新）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private UpdateTriLinkAfterPayAction CreateUpdateTriLinkAfterPayAction(
			UpdateTriLinkAfterPayAction.ReauthActionParams reauthActionParams)
		{
			return new UpdateTriLinkAfterPayAction(reauthActionParams);
		}

		/// <summary>
		/// Create Cancel Paidy Pay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelPaidyPayOrDoNothingAction()
		{
			var isReturnExchange = (this.OrderActionType == OrderActionTypes.Return)
				|| (this.OrderActionType == OrderActionTypes.Exchange);
			if (string.IsNullOrEmpty(this.OrderOld.PaymentOrderId)
				|| ((this.IsChangedAmount == false)
					&& (this.OrderNew.OrderPaymentKbn == this.OrderOld.OrderPaymentKbn)
					&& (Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)))
			{
				return CreateDoNothingAction();
			}

			return CreateCancelPaidyPayAction(new CancelPaidyPayAction.ReauthActionParams(this.OrderOld, this.OrderNew, isReturnExchange));
		}

		/// <summary>
		/// Create Refund Paidy Pay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateRefundPaidyPayOrDoNothingAction()
		{
			if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
				|| string.IsNullOrEmpty(this.OrderOld.PaymentOrderId))
			{
				return CreateDoNothingAction();
			}

			return CreateRefundPaidyPayAction(new RefundPaidyPayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// Create Reauth Paidy Pay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthPaidyPayOrDoNothingAction()
		{
			if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
				|| this.IsCvsDefPayComplete
				|| (this.OrderNew.CreditBranchNo == null)
				|| (this.OrderNew.LastBilledAmount <= 0))
			{
				return CreateDoNothingAction();
			}

			return CreateReauthPaidyPayAction(new ReauthPaidyPayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// Create Sales Paidy Pay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateSalesPaidyPayOrDoNothingAction()
		{
			if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
				|| string.IsNullOrEmpty(this.OrderNew.PaymentOrderId)
				|| (this.OrderNew.LastBilledAmount <= 0)
				|| (this.OrderActionType == OrderActionTypes.Modify))
			{
				return CreateDoNothingAction();
			}

			return CreateSalesPaidyPayAction(new SalesPaidyPayAction.ReauthActionParams(this.OrderNew));
		}

		/// <summary>
		/// Create Reauth Paidy Pay Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthPaidyPayAction CreateReauthPaidyPayAction(
			ReauthPaidyPayAction.ReauthActionParams reauthActionParams)
		{
			return new ReauthPaidyPayAction(reauthActionParams);
		}

		/// <summary>
		/// Create Cancel Paidy Pay Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelPaidyPayAction CreateCancelPaidyPayAction(
			CancelPaidyPayAction.ReauthActionParams reauthActionParams)
		{
			return new CancelPaidyPayAction(reauthActionParams);
		}

		/// <summary>
		/// Create Refund Paidy Pay Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private RefundPaidyPayAction CreateRefundPaidyPayAction(
			RefundPaidyPayAction.ReauthActionParams reauthActionParams)
		{
			return new RefundPaidyPayAction(reauthActionParams);
		}

		/// <summary>
		/// Create Sales Paidy Pay Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private SalesPaidyPayAction CreateSalesPaidyPayAction(
			SalesPaidyPayAction.ReauthActionParams reauthActionParams)
		{
			return new SalesPaidyPayAction(reauthActionParams);
		}

		/// <summary>
		/// Create Cancel Boku Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelBokuAction CreateCancelBokuAction(
			CancelBokuAction.ReauthActionParams reauthActionParams)
		{
			return new CancelBokuAction(reauthActionParams);
		}

		/// <summary>
		/// Create cancel boku or do nothing action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelBokuOrDoNothingAction()
		{
			// 決済取引カードIDが「ブランク」の場合、又は、キャンセル未対応の場合は何もしない
			if (string.IsNullOrEmpty(this.OrderOld.PaymentOrderId)
				|| (this.OrderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)) return CreateDoNothingAction();

			return CreateCancelBokuAction(new CancelBokuAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// Create reauth boku action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthBokuAction CreateReauthBokuAction(
			ReauthBokuAction.ReauthActionParams reauthActionParams)
		{
			return new ReauthBokuAction(reauthActionParams);
		}

		/// <summary>
		/// Create reauth boku or do nothing action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthBokuOrDoNothingAction()
		{
			if (this.IsBilledAmountGreaterThanZero == false)
			{
				return CreateDoNothingAction();
			}

			return CreateReauthBokuAction(new ReauthBokuAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// キャリア決済
		/// OR リクルートかんたん支払い
		/// OR PayPal
		/// OR 楽天ペイであるかどうか
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>対象決済であるかどうか</returns>
		public bool IsCarrierPaymentOrOthers(OrderModel order)
		{
			return ((order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG)
					|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG)
					|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AUMATOMETE_ORG)
					|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_SBPS)
					|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS)
					|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS)
					|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS)
					|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS)
					|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS)
					|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS));
		}

		/// <summary>
		/// 代引き決済
		/// OR クレジットカード、コンビニ後払い、キャリア決済、Amazon Pay、後付款(TriLink後払い)、決済無しの以外
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>対象決済であるかどうか</returns>
		public bool IsCollectPaymentOrOthers(OrderModel order)
		{
			return (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
				|| ((IsCarrierPaymentOrOthers(order) == false)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA));
		}

		/// <summary>
		/// GMOコンビニ後払い
		/// 注文情報）変更があるかどうか
		/// </summary>
		/// <returns>注文情報変更があるかどうか</returns>
		public bool IsGmoCvsDefOrderChange()
		{
			var oldInfo = new GmoRequestOrderRegister(this.OrderOld);
			var newInfo = new GmoRequestOrderRegister(this.OrderNew);
			// 注文情報（合計以外）変更があるかどうか
			return (oldInfo.CreatePostString() != newInfo.CreatePostString());
		}

		/// <summary>
		/// Atodene後払い
		/// 変更があるかどうか
		/// </summary>
		/// <returns>
		/// True：変更アリ
		/// False：変更ナシ
		/// </returns>
		public bool IsAtodeneCvsDefOrderChange()
		{
			var oldOrder = new AtodeneTransactionModelAdapter(this.OrderOld);
			var newOrder = new AtodeneTransactionModelAdapter(this.OrderNew);
			// POST内容が異なる場合は変更アリとする
			return (oldOrder.CreateRequest().CreatePostString() != newOrder.CreateRequest().CreatePostString());
		}

		/// <summary>
		/// Create Reauth LINE Pay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthLinePayOrDoNothingAction()
		{
			if (this.IsCvsDefPayComplete
				|| (this.IsBilledAmountGreaterThanZero == false)
				|| ((this.OrderNew.IsExchangeOrder
						|| this.OrderNew.IsReturnOrder)
					&& this.IsReducedBilledAmount))
			{
				return CreateDoNothingAction();
			}
			return CreateReauthLinePayAction(new ReauthLinePayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// Create Sales LINE Pay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateSalesLinePayOrDoNothingAction()
		{
			if (this.OrderOld.IsExchangeOrder
				&& (this.OrderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				|| this.OrderOld.IsReturnOrder
				|| string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| (this.OrderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED))
			{
				return CreateDoNothingAction();
			}
			return CreateSalesLinePayAction(new SalesLinePayAction.ReauthActionParams(this.OrderOld));
		}

		/// <summary>
		/// Create Cancel Atone Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelAtoneOrDoNothingAction()
		{
			if (this.OrderNew.IsUpdateAtonePaymentFromMyPage
				|| string.IsNullOrEmpty(this.OrderOld.CardTranId))
			{
				return CreateDoNothingAction();
			}

			var reauthType = ((this.OrderActionType == OrderActionTypes.Return)
				|| (this.OrderActionType == OrderActionTypes.Exchange))
					? ReauthCreatorFacade.ReauthTypes.AtoneReturnAllItems
					: GetReauthType();
			return CreateCancelAtoneAction(
				new CancelAtoneAction.ReauthActionParams(this.OrderOld, reauthType));
		}

		/// <summary>
		/// Create refund Atone Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateRefundAtoneOrDoNothingAction()
		{
			if (this.OrderOld.IsExchangeOrder
				|| string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| ((this.OrderOld.LastBilledAmount - this.OrderNew.LastBilledAmount) <= 0))
			{
				return CreateDoNothingAction();
			}
			return CreateRefundAtoneAction(
				new RefundAtoneAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// Create Reauth Atone Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthAtoneOrDoNothingAction()
		{
			this.PreCancelFlg = true;
			if (this.OrderNew.IsUpdateAtonePaymentFromMyPage
				|| (this.OrderNew.LastBilledAmount <= 0))
			{
				return CreateDoNothingAction();
			}
			return CreateReauthAtoneAction(
				new ReauthAtoneAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// Create Reauth Atone Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthAtoneAction CreateReauthAtoneAction(
			ReauthAtoneAction.ReauthActionParams reauthActionParams)
		{
			return new ReauthAtoneAction(reauthActionParams);
		}

		/// <summary>
		/// Create Cancel Atone Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelAtoneAction CreateCancelAtoneAction(
			CancelAtoneAction.ReauthActionParams reauthActionParams)
		{
			return new CancelAtoneAction(reauthActionParams);
		}

		/// <summary>
		/// Create Refund Atone Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private RefundAtoneAction CreateRefundAtoneAction(
			RefundAtoneAction.ReauthActionParams reauthActionParams)
		{
			return new RefundAtoneAction(reauthActionParams);
		}

		/// <summary>
		/// Create Cancel Atone Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelAfteeOrDoNothingAction()
		{
			if (this.OrderNew.IsUpdateAfteePaymentFromMyPage || string.IsNullOrEmpty(this.OrderOld.CardTranId))
			{
				return CreateDoNothingAction();
			}

			var reauthType = ((this.OrderActionType == OrderActionTypes.Return)
				|| (this.OrderActionType == OrderActionTypes.Exchange))
					? ReauthCreatorFacade.ReauthTypes.AfteeReturnAllItems
					: GetReauthType();
			return CreateCancelAfteeAction(
				new CancelAfteeAction.ReauthActionParams(this.OrderOld, reauthType));
		}

		/// <summary>
		/// Create Reauth Aftee Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthAfteeOrDoNothingAction()
		{
			if (this.OrderNew.IsUpdateAfteePaymentFromMyPage
				|| (this.OrderNew.LastBilledAmount <= 0))
			{
				return CreateDoNothingAction();
			}
			return CreateReauthAfteeAction(new ReauthAfteeAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// Create Reauth Aftee Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthAfteeAction CreateReauthAfteeAction(
			ReauthAfteeAction.ReauthActionParams reauthActionParams)
		{
			return new ReauthAfteeAction(reauthActionParams);
		}

		/// <summary>
		/// Create Cancel Aftee Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelAfteeAction CreateCancelAfteeAction(
			CancelAfteeAction.ReauthActionParams reauthActionParams)
		{
			return new CancelAfteeAction(reauthActionParams);
		}

		/// <summary>
		/// Create Cancel LINE Pay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelLinePayOrDoNothingAction()
		{
			if (string.IsNullOrEmpty(this.OrderOld.CardTranId))
			{
				return CreateDoNothingAction();
			}
			return CreateCancelLinePayAction(new CancelLinePayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// Create refund LINE Pay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateRefundLinePayOrDoNothingAction()
		{
			var orderId =
				string.IsNullOrEmpty(this.OrderOld.OrderIdOrg)
					? this.OrderOld.OrderId
					: this.OrderOld.OrderIdOrg;
			var orderOldOrg = DomainFacade.Instance.OrderService.Get(orderId);

			if (string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| ((this.OrderOld.IsExchangeOrder
						|| this.OrderOld.IsReturnOrder)
					&& (orderOldOrg.OnlinePaymentStatus != Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)))
			{
				return CreateDoNothingAction();
			}
			return CreateRefundLinePayAction(new RefundLinePayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// Create Reauth LINE Pay Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthLinePayAction CreateReauthLinePayAction(
			ReauthLinePayAction.ReauthActionParams reauthActionParams)
		{
			return new ReauthLinePayAction(reauthActionParams);
		}

		/// <summary>
		/// Create Sales LINE Pay Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private SalesLinePayAction CreateSalesLinePayAction(
			SalesLinePayAction.ReauthActionParams reauthActionParams)
		{
			return new SalesLinePayAction(reauthActionParams);
		}

		/// <summary>
		/// Create Refund LINE Pay Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private RefundLinePayAction CreateRefundLinePayAction(
			RefundLinePayAction.ReauthActionParams reauthActionParams)
		{
			return new RefundLinePayAction(reauthActionParams);
		}

		/// <summary>
		/// Create Cancel LINE Pay Action
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelLinePayAction CreateCancelLinePayAction(
			CancelLinePayAction.ReauthActionParams reauthActionParams)
		{
			return new CancelLinePayAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（NP後払い再与 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateReauthNPAfterPayOrDoNothingAction()
		{
			if (this.IsCvsDefPayComplete
				|| (this.IsBilledAmountGreaterThanZero == false)
				|| (this.IsNoChangeNPAfterPay
					&& ((NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(this.OrderOld.ExternalPaymentStatus) == false)
						|| (NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(this.OrderOld.ExternalPaymentStatus) 
							&& CheckNPAfterPayHasPaid(this.OrderOld)))))
			{
				return CreateDoNothingAction();
			}

			this.PreCancelFlg = (this.IsNoChangeNPAfterPay
				&& NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(this.OrderOld.ExternalPaymentStatus));
			return CreateReauthNPAfterPayAction(new ReauthNPAfterPayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション（NP後払いキャンセル or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelNPAfterPayOrDoNothingAction()
		{
			if (string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| (this.IsNoChangeNPAfterPay
					&& (((this.IsNPAfterPayReturnAllItems == false)
							&& ((this.OrderOld.IsReturnExchangeProcessAtWorkflow == false)
								&& NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(this.OrderOld.ExternalPaymentStatus) == false)
							|| (NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(this.OrderOld.ExternalPaymentStatus)
								&& CheckNPAfterPayHasPaid(this.OrderOld)))
						|| (this.IsNPAfterPayReturnAllItems
							&& (NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(this.OrderOld.ExternalPaymentStatus)
							&& CheckNPAfterPayHasPaid(this.OrderOld))))))
			{
				return CreateDoNothingAction();
			}
			return CreateCancelNPAfterPayAction(new CancelNPAfterPayAction.ReauthActionParams(this.OrderOld));
		}

		/// <summary>
		/// 再与信アクション（NP後払い注文情報更新 or 何もしない）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateUpdateNPAfterPayOrDoNothingAction()
		{
			if (this.IsCvsDefPayComplete
				|| string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| (this.IsBilledAmountGreaterThanZero == false)
				|| (this.IsNoChangeNPAfterPay
					&& NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(this.OrderOld.ExternalPaymentStatus)))
			{
				return CreateDoNothingAction();
			}
			return CreateUpdateNPAfterPayAction(new UpdateNPAfterPayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// 再与信アクション NP後払い）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private ReauthNPAfterPayAction CreateReauthNPAfterPayAction(
			ReauthNPAfterPayAction.ReauthActionParams reauthActionParams)
		{
			return new ReauthNPAfterPayAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（NP後払いキャンセル）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelNPAfterPayAction CreateCancelNPAfterPayAction(
			CancelNPAfterPayAction.ReauthActionParams reauthActionParams)
		{
			return new CancelNPAfterPayAction(reauthActionParams);
		}

		/// <summary>
		/// 再与信アクション（NP後払い注文情報更新）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private UpdateNPAfterPayAction CreateUpdateNPAfterPayAction(
			UpdateNPAfterPayAction.ReauthActionParams reauthActionParams)
		{
			return new UpdateNPAfterPayAction(reauthActionParams);
		}

		/// <summary>
		/// Check NP After Pay Has Paid
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>True: Payment has paid, otherwise: false</returns>
		public bool CheckNPAfterPayHasPaid(OrderModel order)
		{
			if ((this.IsNoChangeNPAfterPay == false)
				|| (NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(this.OrderOld.ExternalPaymentStatus) == false)) return false;

			var result = NPAfterPayUtility.IsNPAfterPayHasPaid(order.CardTranId);

			return result;
		}

		/// <summary>
		/// Create Cancel EcPay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelEcPayOrDoNothingAction()
		{
			var isPaymentEcPayWithCredit = (this.OrderOld.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT);
			var isOrderReturnExchange = ((this.OrderActionType == OrderActionTypes.Return)
				|| (this.OrderActionType == OrderActionTypes.Exchange));

			if (string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| (this.IsSalesOrder && isOrderReturnExchange)
				|| (isPaymentEcPayWithCredit == false)
				|| (isPaymentEcPayWithCredit
					&& isOrderReturnExchange
					&& (this.IsSalesOrder == false)
					&& (this.IsReducedBilledAmount == false)))
			{
				return CreateDoNothingAction();
			}
			return new CancelEcPayAction(new CancelEcPayAction.ReauthActionParams(this.OrderOld));
		}

		/// <summary>
		/// Create Refund EcPay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateRefundEcPayOrDoNothingAction()
		{
			var isPaymentEcPayWithCredit = (this.OrderOld.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT);
			var isChangePayment = (this.OrderOld.OrderPaymentKbn != this.OrderNew.OrderPaymentKbn);
			if (string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| ((this.OrderActionType == OrderActionTypes.Modify) && (this.IsSalesOrder == false))
				|| (this.OrderOld.IsReturnExchangeProcessAtWorkflow && (this.IsSalesOrder == false))
				|| (isPaymentEcPayWithCredit == false)
				|| ((isChangePayment == false) && (this.IsChangedAmount == false))
				|| (isPaymentEcPayWithCredit
					&& (this.IsReducedBilledAmount == false)
					&& (this.IsSalesOrder == false)))
			{
				return CreateDoNothingAction();
			}
			return new RefundEcPayAction(
				new RefundEcPayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// Create Sales EcPay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateSalesEcPayOrDoNothingAction()
		{
			var order = ((this.OrderActionType == OrderActionTypes.Return)
				|| (this.OrderActionType == OrderActionTypes.Exchange))
					? this.OrderOld
					: this.OrderNew;

			// If the original order has sales then not call Api
			var isSalesOrder = this.IsSalesOrder;
			if (string.IsNullOrEmpty(order.OrderIdOrg) == false)
			{
				var orderOriginal = new OrderService().Get(order.OrderIdOrg, this.Accessor);
				isSalesOrder = (orderOriginal.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED);
			}

			var isPaymentEcPayWithCredit = (this.OrderOld.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT);
			if (string.IsNullOrEmpty(this.OrderNew.CardTranId)
				|| (order.LastBilledAmount <= 0)
				|| (this.OrderActionType == OrderActionTypes.Modify)
				|| (isPaymentEcPayWithCredit == false)
				|| isSalesOrder
				|| order.IsReturnExchangeProcessAtWorkflow)
			{
				return CreateDoNothingAction();
			}
			return new SalesEcPayAction(new SalesEcPayAction.ReauthActionParams(order));
		}

		/// <summary>
		/// Create sales PayPay or do nothing action
		/// </summary>
		/// <returns>Sales PayPay action</returns>
		private IReauthAction CreateSalesPaypayOrDoNothingAction()
		{
			if ((this.CanExecOrderPayPayAction == false)
				|| (this.OrderOld.LastBilledAmount <= 0)
				|| this.IsSalesOrderPayPay)
			{
				return CreateDoNothingAction();
			}
			return CreateSalesPaypayAction(new SalesPaypayAction.ReauthActionParams(this.OrderOld));
		}

		/// <summary>
		/// Create sales PayPay action
		/// </summary>
		/// <param name="reauthActionParams">Reauth action params</param>
		/// <returns>Sales PayPay action</returns>
		private SalesPaypayAction CreateSalesPaypayAction(
			SalesPaypayAction.ReauthActionParams reauthActionParams)
		{
			return new SalesPaypayAction(reauthActionParams);
		}

		/// <summary>
		/// Create refund PayPay or do nothing action
		/// </summary>
		/// <returns>Create refund PayPay or do nothing action</returns>
		private IReauthAction CreateRefundPaypayOrDoNothingAction()
		{
			if ((this.CanExecOrderPayPayAction == false)
				|| this.IsIncreasedBilledAmount)
			{
				return CreateDoNothingAction();
			}
			return CreateRefundPaypayAction(new RefundPaypayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// Create refund PayPay action
		/// </summary>
		/// <param name="reauthActionParams">Reauth Action Params</param>
		/// <returns>Refund PayPay</returns>
		private RefundPaypayAction CreateRefundPaypayAction(
			RefundPaypayAction.ReauthActionParams reauthActionParams)
		{
			return new RefundPaypayAction(reauthActionParams);
		}

		/// <summary>
		/// Create cancel PayPay or do nothing action
		/// </summary>
		/// <returns>Cancel PayPay action</returns>
		private IReauthAction CreateCancelPaypayOrDoNothingAction()
		{
			if (this.CanExecOrderPayPayAction == false)
			{
				return CreateDoNothingAction();
			}
			return CreateCancelPaypayAction(new CancelPaypayAction.ReauthActionParams(this.OrderOld));
		}

		/// <summary>
		/// Create cancel PayPay action
		/// </summary>
		/// <param name="reauthActionParams">Reauth action params</param>
		/// <returns>Cancel PayPay action</returns>
		private CancelPaypayAction CreateCancelPaypayAction(
			CancelPaypayAction.ReauthActionParams reauthActionParams)
		{
			return new CancelPaypayAction(reauthActionParams);
		}

		/// <summary>
		/// 異なる受注IDかどうか
		/// </summary>
		/// <returns>異なる受注IDかどうか</returns>
		private bool IsDifferentOrderId()
		{
			// NOTE:新旧の注文IDが違う状態の再与信は同梱とカート新規作成のときなど
			var result = (this.OrderOld.OrderId != this.OrderNew.OrderId);
			return result;
		}

		/// <summary>
		/// Create Cancel NewebPay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelNewebPayOrDoNothingAction()
		{
			var isPaymentNewebPayWithCredit = (this.OrderOld.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT);
			var isOrderReturnExchange = ((this.OrderActionType == OrderActionTypes.Return)
				|| (this.OrderActionType == OrderActionTypes.Exchange));

			if (string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| (this.IsSalesOrder && isOrderReturnExchange)
				|| (isPaymentNewebPayWithCredit == false)
				|| (isPaymentNewebPayWithCredit
					&& isOrderReturnExchange
					&& (this.IsSalesOrder == false)
					&& (this.IsReducedBilledAmount == false)))
			{
				return CreateDoNothingAction();
			}
			return new CancelNewebPayAction(new CancelNewebPayAction.ReauthActionParams(this.OrderOld));
		}

		/// <summary>
		/// Create Refund NewebPay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateRefundNewebPayOrDoNothingAction()
		{
			var isPaymentNewebPayWithCredit = (this.OrderOld.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT);
			var isChangePayment = (this.OrderOld.OrderPaymentKbn != this.OrderNew.OrderPaymentKbn);
			if (string.IsNullOrEmpty(this.OrderOld.CardTranId)
				|| ((this.OrderActionType == OrderActionTypes.Modify) && (this.IsSalesOrder == false))
				|| (this.OrderOld.IsReturnExchangeProcessAtWorkflow && (this.IsSalesOrder == false))
				|| (isPaymentNewebPayWithCredit == false)
				|| ((isChangePayment == false) && (this.IsChangedAmount == false))
				|| (isPaymentNewebPayWithCredit
					&& (this.IsReducedBilledAmount == false)
					&& (this.IsSalesOrder == false)))
			{
				return CreateDoNothingAction();
			}
			return new RefundNewebPayAction(
				new RefundNewebPayAction.ReauthActionParams(this.OrderOld, this.OrderNew));
		}

		/// <summary>
		/// Create Sales NewebPay Or Do Nothing Action
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateSalesNewebPayOrDoNothingAction()
		{
			var order = ((this.OrderActionType == OrderActionTypes.Return)
				|| (this.OrderActionType == OrderActionTypes.Exchange))
					? this.OrderOld
					: this.OrderNew;

			// If the original order has sales then not call Api
			var isSalesOrder = this.IsSalesOrder;
			if (string.IsNullOrEmpty(order.OrderIdOrg) == false)
			{
				var orderOriginal = new OrderService().Get(order.OrderIdOrg, this.Accessor);
				isSalesOrder = (orderOriginal.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED);
			}

			var isPaymentNewebPayWithCredit = (this.OrderOld.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT);
			if (string.IsNullOrEmpty(this.OrderNew.CardTranId)
				|| (order.LastBilledAmount <= 0)
				|| (this.OrderActionType == OrderActionTypes.Modify)
				|| (isPaymentNewebPayWithCredit == false)
				|| isSalesOrder
				|| order.IsReturnExchangeProcessAtWorkflow)
			{
				return CreateDoNothingAction();
			}
			return new SalesNewebPayAction(new SalesNewebPayAction.ReauthActionParams(order));
		}

		/// <summary>
		/// 再与信アクション（スコア後払い注文情報更新）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateUpdateScoreAfterPayAction()
		{
			var updateAction = new UpdateScoreAfterPayAction(new UpdateScoreAfterPayAction.ReauthActionParams(this.OrderNew));
			return updateAction;
		}

		/// <summary>
		/// 再与信アクション（ベリトランス後払い注文情報更新）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateUpdateVeritransAfterPayAction()
		{
			var updateAction = new UpdateVeritransAfterPayAction(this.OrderNew);
			return updateAction;
		}

		/// <summary>
		/// レコメンド適用による再与信か
		/// </summary>
		/// <returns>レコメン適用の注文か</returns>
		private bool IsRecommendOrder()
		{
			var result = ((this.OrderOld.Items.Any(item => item.IsRecommendItem) == false) && this.OrderNew.Items.Any(item => item.IsRecommendItem));
			return result;
		}

		/// <summary>
		/// 再与信アクション（GMOアトカラ注文情報更新）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateUpdateGmoAtokaraOrDoNothingAction()
		{
			var updateAction = new UpdateGmoAtokaraAction(new UpdateGmoAtokaraAction.ReauthActionParams(this.OrderOld, this.OrderNew));
			return updateAction;
		}

		/// <summary>
		/// キャンセルアクション（GMOアトカラ）インスタンス作成
		/// </summary>
		/// <returns>該当クラスのインスタンス</returns>
		private IReauthAction CreateCancelGmoAtokaraOrDoNothingAction()
		{
			var cancelActiton = CreateCancelGmoAtokaraAction(new CancelGmoAtokaraAction.ReauthActionParams(this.OrderOld, this.OrderNew));
			return cancelActiton;
		}

		/// <summary>
		/// キャンセルアクション（GMOアトカラ）インスタンス作成
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>該当クラスのインスタンス</returns>
		private CancelGmoAtokaraAction CreateCancelGmoAtokaraAction(
			CancelGmoAtokaraAction.ReauthActionParams reauthActionParams)
		{
			return new CancelGmoAtokaraAction(reauthActionParams);
		}

		#region プロパティ
		/// <summary>注文情報（変更前）</summary>
		public OrderModel OrderOld { get; }
		/// <summary>注文情報（変更後）</summary>
		public OrderModel OrderNew { get; }
		/// <summary>変更前・再与信処理区分</summary>
		public ExecuteTypes? OldExecuteType { get; }
		/// <summary>処理区分</summary>
		public ExecuteTypes ExecuteType { get; }
		/// <summary>注文処理区分</summary>
		public OrderActionTypes OrderActionType { get; }
		/// <summary>注文商品が全て返品されているか？</summary>
		public bool IsReturnAllItems { get; }
		/// <summary>コンビニ後払いで外部連携ステータスが「入金済み」(元注文・返品・交換いずれか)</summary>
		public bool IsCvsDefPayComplete { get; }
		/// <summary>与信前キャンセルフラグ</summary>
		public bool PreCancelFlg { get; private set; }
		/// <summary>与信結果がHOLDか(現在はコンビニ後払い「DSK」のみ利用)</summary>
		public bool IsAuthResultHold { get; }
		#endregion

		#region 再与信判定用プロパティ
		/// <summary>決済種別がクレジットカード変更なし</summary>
		public bool IsNoChangeCredit
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>決済種別がクレジットカード→コンビニ後払いに変更</summary>
		public bool IsChangeCreditToCvsDef
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			}
		}
		/// <summary>Creditを請求に変更</summary>
		public bool IsChangeCreditToGmoPost
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));
			}
		}
		/// <summary>決済種別がクレジットカード→PayPalに変更</summary>
		public bool IsChangeCreditToPayPal
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>決済種別がクレジットカード→Atone</summary>
		public bool IsChangeCreditToAtone
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			}
		}
		/// <summary>決済種別がクレジットカード→Aftee</summary>
		public bool IsChangeCreditToAftee
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			}
		}
		/// <summary>決済種別がクレジットカード→LinePay</summary>
		public bool IsChangeCreditToLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>決済種別がクレジットカード→後付款(TriLink後払い)に変更</summary>
		public bool IsChangeCreditToTriLinkAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>決済種別がクレジットカード→代引きに変更</summary>
		public bool IsChangeCreditToCollect
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>決済種別がクレジットカード→キャリア決済</summary>
		public bool IsChangeCreditToCarrier
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>決済種別がクレジットカード→Amazon Pay</summary>
		public bool IsChangeCreditToAmazonPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>決済種別がクレジットカード→Paidy Pay</summary>
		public bool IsChangeCreditToPaidyPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
		}
		}
		/// <summary>決済種別がクレジットカード→決済無しに変更</summary>
		public bool IsChangeCreditToNoPayment
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>決済種別がクレジットカード→PayPay</summary>
		public bool IsChangeCreditToPayPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY);
			}
		}
		/// <summary>決済種別がクレジットカード→Boku</summary>
		public bool IsChangeCreditToBoku
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>
		/// クレジットカード全返品（最終請求金額=0）
		/// </summary>
		public bool IsCreditReturnAllItems
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems;
			}
		}
		/// <summary>決済種別がコンビニ後払い変更なし</summary>
		public bool IsNoChangeCvsDef
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			}
		}
		/// <summary>支払方法は都度与信-に変更しないとき</summary>
		public bool IsNoChangePayAsYouGo
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO);
			}
		}

		/// <summary>支払方法は枠保証に変更しないとき</summary>
		public bool IsNoChangeFramePayment
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE);
			}
		}

		/// <summary>決済種別がコンビニ後払い→クレジットカードに変更</summary>
		public bool IsChangeCvsDefToCredit
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>決済種別がコンビニ後払い→PayPalに変更</summary>
		public bool IsChangeCvsDefToPayPal
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>決済種別がコンビニ後払い→後付款(TriLink後払い)に変更</summary>
		public bool IsChangeCvsDefToTriLinkAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>決済種別がコンビニ後払い→代引きに変更</summary>
		public bool IsChangeCvsDefToCollect
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>決済種別がコンビニ後払い→キャリア決済</summary>
		public bool IsChangeCvsDefToCarrier
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>決済種別がコンビニ後払い→Amazon Pay</summary>
		public bool IsChangeCvsDefToAmazonPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>決済種別がコンビニ後払い→Paidy Pay</summary>
		public bool IsChangeCvsDefToPaidyPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			}
		}
		/// <summary>決済種別がコンビニ後払い→決済無しに変更</summary>
		public bool IsChangeCvsDefToNoPayment
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>決済種別がコンビニ後払い→Atone</summary>
		public bool IsChangeCvsDefToAtone
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			}
		}
		/// <summary>決済種別がコンビニ後払い→Aftee</summary>
		public bool IsChangeCvsDefToAftee
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			}
		}
		/// <summary>決済種別がコンビニ後払い→LINEPay</summary>
		public bool IsChangeCvsDefToLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>決済種別がコンビニ後払い→PayPay</summary>
		public bool IsChangeCvsDefToPayPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			}
		}

		/// <summary>Is Change PayAsYouGo To Credit</summary>
		public bool IsChangePayAsYouGoToCredit
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>Is Change PayAsYouGo To PayPal</summary>
		public bool IsChangePayAsYouGoToPayPal
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>Is Change PayAsYouGo To TriLinkAfterPay</summary>
		public bool IsChangePayAsYouGoToTriLinkAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>Is Change PayAsYouGo To Collect</summary>
		public bool IsChangePayAsYouGoToCollect
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>Is Change PayAsYouGo To Carrier</summary>
		public bool IsChangePayAsYouGoToCarrier
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>Is Change PayAsYouGo To AmazonPay</summary>
		public bool IsChangePayAsYouGoToAmazonPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>Is Change PayAsYouGo To PaidyPay</summary>
		public bool IsChangePayAsYouGoToPaidyPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			}
		}
		/// <summary>Is Change PayAsYouGo To No Payment</summary>
		public bool IsChangePayAsYouGoToNoPayment
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>Is Change PayAsYouGo To Atone</summary>
		public bool IsChangePayAsYouGoToAtone
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			}
		}
		/// <summary>Is Change PayAsYouGo To Aftee</summary>
		public bool IsChangePayAsYouGoToAftee
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			}
		}
		/// <summary>Is Change PayAsYouGo To LinePay</summary>
		public bool IsChangePayAsYouGoToLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>Is Change PayAsYouGo To Frame Payment</summary>
		public bool IsChangePayAsYouGoToFramePayment
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));
			}
		}
		/// <summary>Is Change PayAsYouGo PayPay</summary>
		public bool IsChangePayAsYouGoToPayPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			}
		}

		/// <summary>Is Change FramePayment To Credit</summary>
		public bool IsChangeFramePaymentToCredit
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>Is Change FramePayment To PayPal</summary>
		public bool IsChangeFramePaymentToPayPal
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>Is Change FramePayment To TriLinkAfterPay</summary>
		public bool IsChangeFramePaymentToTriLinkAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>Is Change FramePayment To Collect</summary>
		public bool IsChangeFramePaymentToCollect
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>Is Change FramePayment To Carrier</summary>
		public bool IsChangeFramePaymentToCarrier
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>Is Change FramePayment To AmazonPay</summary>
		public bool IsChangeFramePaymentToAmazonPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>Is Change FramePayment To PaidyPay</summary>
		public bool IsChangeFramePaymentToPaidyPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			}
		}
		/// <summary>Is Change FramePayment To No Payment</summary>
		public bool IsChangeFramePaymentToNoPayment
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>Is Change FramePayment To Atone</summary>
		public bool IsChangeFramePaymentToAtone
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			}
		}
		/// <summary>Is Change FramePayment To Aftee</summary>
		public bool IsChangeFramePaymentToAftee
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			}
		}
		/// <summary>Is Change FramePayment To LinePay</summary>
		public bool IsChangeFramePaymentToLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>Is Change FramePayment To PayPay</summary>
		public bool IsChangeFramePaymentToPayPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			}
		}
		/// <summary>決済種別がコンビニ後払い→Boku</summary>
		public bool IsChangeCvsDefToBoku
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>Is Change FramePayment To PayPay</summary>
		public bool IsChangeFramePaymentToPayAsYouGo
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO));
			}
		}
		/// <summary>
		/// コンビニ後払い全返品（最終請求金額=0）
		/// </summary>
		public bool IsCvsDefReturnAllItems
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems;
			}
		}

		/// <summary>
		/// Is PayAsYouGo Return All Items
		/// </summary>
		public bool IsPayAsYouGoReturnAllItems
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& this.IsReturnAllItems;
			}
		}
		/// <summary>
		/// Is FramePayment Return All Items
		/// </summary>
		public bool IsFramePaymentReturnAllItems
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					&& this.IsReturnAllItems;
			}
		}
		/// <summary>決済種別がPayPal変更なし</summary>
		public bool IsNoChangePayPal
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>決済種別がPayPal→クレジットカードに変更</summary>
		public bool IsChangePayPalToCredit
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>決済種別がPayPal→コンビニ後払いに変更</summary>
		public bool IsChangePayPalToCvsDef
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			}
		}
		/// <summary>Is Change PayPal To Gmo Post</summary>
		public bool IsChangePayPalToGmoPost
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));
			}
		}
		/// <summary>決済種別がPayPal→後付款(TriLink後払い)に変更</summary>
		public bool IsChangePayPalToTriLinkAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>決済種別がPayPal→代引きに変更</summary>
		public bool IsChangePayPalToCollect
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>決済種別がPayPal→キャリア決済</summary>
		public bool IsChangePayPalToCarrier
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>決済種別がPayPal→決済無しに変更</summary>
		public bool IsChangePayPalToNoPayment
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>決済種別がPayPal→Atone</summary>
		public bool IsChangePayPalToAtone
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			}
		}
		/// <summary>決済種別がPayPal→Aftee</summary>
		public bool IsChangePayPalToAftee
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			}
		}
		/// <summary>決済種別がPayPal→Paidy Pay</summary>
		public bool IsChangePayPalToPaidyPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			}
		}
		/// <summary>決済種別がPayPal→LINEPay</summary>
		public bool IsChangePayPalToLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>決済種別がPayPal→PayPay</summary>
		public bool IsChangePayPalToPayPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			}
		}
		/// <summary>決済種別がPayPal→Boku</summary>
		public bool IsChangePayPalToBoku
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>
		/// PayPal全返品（最終請求金額=0）
		/// </summary>
		public bool IsPayPalReturnAllItems
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems;
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)変更なし</summary>
		public bool IsNoChangeTriLinkAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→クレジットカードに変更</summary>
		public bool IsChangeTriLinkAfterPayToCredit
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→コンビニ後払いに変更</summary>
		public bool IsChangeTriLinkAfterPayToCvsDef
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			}
		}
		/// <summary>Is Change TriLinkAfterPay To Gmo Post</summary>
		public bool IsChangeTriLinkAfterPayToGmoPost
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→Amazon Payに変更</summary>
		public bool IsChangeTriLinkAfterPayToAmazonPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→Paidy Pay</summary>
		public bool IsChangeTriLinkAfterPayToPaidyPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→PayPayに変更</summary>
		public bool IsChangeTriLinkAfterPayToPayPal
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→代引きに変更</summary>
		public bool IsChangeTriLinkAfterPayToCollect
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→キャリア決済</summary>
		public bool IsChangeTriLinkAfterPayToCarrier
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→決済無しに変更</summary>
		public bool IsChangeTriLinkAfterPayToNoPayment
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→Atone</summary>
		public bool IsChangeTriLinkAfterPayToAtone
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→Aftee</summary>
		public bool IsChangeTriLinkAfterPayToAftee
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→LINEPay</summary>
		public bool IsChangeTriLinkAfterPayToLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→PayPay</summary>
		public bool IsChangeTriLinkAfterPayToPayPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→Boku</summary>
		public bool IsChangeTriLinkAfterPayToBoku
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>
		/// 後付款(TriLink後払い)全返品（最終請求金額=0）
		/// </summary>
		public bool IsTriLinkAfterPayReturnAllItems
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems;
			}
		}
		/// <summary>決済種別がキャリア決済→クレジットカードに変更</summary>
		public bool IsChangeCarrierToCredit
		{
			get
			{
				return (IsCarrierPaymentOrOthers(this.OrderOld))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>決済種別がキャリア決済→コンビニ後払いに変更</summary>
		public bool IsChangeCarrierToCvsDef
		{
			get
			{
				return IsCarrierPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			}
		}
		/// <summary>Is Change Carrier To Gmo Post</summary>
		public bool IsChangeCarrierToGmoPost
		{
			get
			{
				return IsCarrierPaymentOrOthers(this.OrderOld)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));
			}
		}
		/// <summary>決済種別がキャリア決済→PayPalに変更</summary>
		public bool IsChangeCarrierToPayPal
		{
			get
			{
				return IsCarrierPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>決済種別がキャリア決済→後付款(TriLink後払い)に変更</summary>
		public bool IsChangeCarrierToTriLinkAfterPay
		{
			get
			{
				return IsCarrierPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>決済種別がキャリア決済→代引きに変更</summary>
		public bool IsChangeCarrierToCollect
		{
			get
			{
				return IsCarrierPaymentOrOthers(this.OrderOld)
					&& IsCollectPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>決済種別がキャリア決済→Amazon Pay</summary>
		public bool IsChangeCarrierToAmazonPay
		{
			get
			{
				return IsCarrierPaymentOrOthers(this.OrderOld)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>決済種別がキャリア決済→Paidy Pay</summary>
		public bool IsChangeCarrierToPaidyPay
		{
			get
			{
				return IsCarrierPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			}
		}
		/// <summary>決済種別がキャリア決済→決済無しに変更</summary>
		public bool IsChangeCarrierToNoPayment
		{
			get
			{
				return IsCarrierPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>決済種別がキャリア決済→Atone</summary>
		public bool IsChangeCarrierToAtone
		{
			get
			{
				return IsCarrierPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			}
		}
		/// <summary>決済種別がキャリア決済→Aftee</summary>
		public bool IsChangeCarrierToAftee
		{
			get
			{
				return IsCarrierPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			}
		}
		/// <summary>決済種別がキャリア決済→LINEPay</summary>
		public bool IsChangeCarrierToLinePay
		{
			get
			{
				return (IsCarrierPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>決済種別がキャリア決済→PayPay</summary>
		public bool IsChangeCarrierToPayPay
		{
			get
			{
				return (IsCarrierPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			}
		}
		/// <summary>決済種別がキャリア決済→Boku</summary>
		public bool IsChangeCarrierToBoku
		{
			get
			{
				return (IsCarrierPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>決済種別が代引き→クレジットカードに変更</summary>
		public bool IsChangeCollectToCredit
		{
			get
			{
				return IsCollectPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>決済種別が代引き→コンビニ後払いに変更</summary>
		public bool IsChangeCollectToCvsDef
		{
			get
			{
				return IsCollectPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			}
		}
		/// <summary>Is Change Collect To Gmo Post</summary>
		public bool IsChangeCollectToGmoPost
		{
			get
			{
				return IsCollectPaymentOrOthers(this.OrderOld)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));
		}
		}
		/// <summary>決済種別が代引き→PayPalに変更</summary>
		public bool IsChangeCollectToPayPal
		{
			get
			{
				return IsCollectPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>決済種別が代引き→後付款(TriLink後払い)に変更</summary>
		public bool IsChangeCollectToTriLinkAfterPay
		{
			get
			{
				return IsCollectPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>決済種別が代引き→キャリア決済</summary>
		public bool IsChangeCollectToCarrier
		{
			get
			{
				return IsCollectPaymentOrOthers(this.OrderOld)
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>決済種別が代引き→Amazon Pay</summary>
		public bool IsChangeCollectToAmazonPay
		{
			get
			{
				return IsCollectPaymentOrOthers(this.OrderOld)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>決済種別が代引き→Paidy Pay</summary>
		public bool IsChangeCollectToPaidyPay
		{
			get
			{
				return IsCollectPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			}
		}
		/// <summary>決済種別が代引き→決済無しに変更</summary>
		public bool IsChangeCollectToNoPayment
		{
			get
			{
				return (IsCollectPaymentOrOthers(this.OrderOld))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>決済種別が代引き→Atone</summary>
		public bool IsChangeCollectToAtone
		{
			get
			{
				return (IsCollectPaymentOrOthers(this.OrderOld))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			}
		}
		/// <summary>決済種別が代引き→Aftee</summary>
		public bool IsChangeCollectToAftee
		{
			get
			{
				return (IsCollectPaymentOrOthers(this.OrderOld))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			}
		}
		/// <summary>決済種別が代引き→LINEPay</summary>
		public bool IsChangeCollectToLinePay
		{
			get
			{
				return ((IsCollectPaymentOrOthers(this.OrderOld))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>決済種別が代引き→PayPay</summary>
		public bool IsChangeCollectToPayPay
		{
			get
			{
				return ((IsCollectPaymentOrOthers(this.OrderOld))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			}
		}
		/// <summary>決済種別がAmazonPay変更なし</summary>
		public bool IsNoChangeAmazonPay
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>決済種別がAmazonPay→クレジットカードに変更</summary>
		public bool IsChangeAmazonPayToCredit
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>決済種別がAmazonPay→コンビニ後払いに変更</summary>
		public bool IsChangeAmazonPayToCvsDef
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			}
		}
		/// <summary>AmazonPayを請求に変更</summary>
		public bool IsChangeAmazonPayToGmoPost
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));
			}
		}
		/// <summary>決済種別がAmazonPay→後付款(TriLink後払い)に変更</summary>
		public bool IsChangeAmazonPayToTriLinkAfterPay
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>決済種別がAmazonPay→Paidy Pay</summary>
		public bool IsChangeAmazonPayToPaidyPay
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			}
		}
		/// <summary>決済種別がAmazonPay→代引きに変更</summary>
		public bool IsChangeAmazonPayToCollect
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (IsCollectPaymentOrOthers(this.OrderNew));
		}
		}
		/// <summary>決済種別がAmazonPay→キャリア決済</summary>
		public bool IsChangeAmazonPayToCarrier
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>決済種別がAmazonPay→決済無しに変更</summary>
		public bool IsChangeAmazonPayToNoPayment
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>決済種別がAmazonPay→Atone</summary>
		public bool IsChangeAmazonPayToAtone
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			}
		}
		/// <summary>決済種別がAmazonPay→Aftee</summary>
		public bool IsChangeAmazonPayToAftee
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			}
		}
		/// <summary>決済種別がAmazonPay→LINEPay</summary>
		public bool IsChangeAmazonPayToLinePay
		{
			get
			{
				return ((OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>決済種別がAmazonPay→PayPay</summary>
		public bool IsChangeAmazonPayToPayPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			}
		}
		/// <summary>決済種別がAmazonPay→Boku</summary>
		public bool IsChangeAmazonPayToBoku
		{
			get
			{
				return ((OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>
		/// AmazonPay全返品（最終請求金額=0）
		/// </summary>
		public bool IsAmazonPayReturnAllItems
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems;
			}
		}
		/// <summary>Is No Change Atone</summary>
		public bool IsNoChangeAtone
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			}
		}
		/// <summary>Atone to credit</summary>
		public bool IsChangeAtoneToCredit
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>Atone To AmazonPay</summary>
		public bool IsChangeAtoneToAmazonPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>Atone To TriLink AfterPay</summary>
		public bool IsChangeAtoneToTriLinkAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>Is Change Atone To Collect</summary>
		public bool IsChangeAtoneToCollect
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>Change Atone To Cvs Def</summary>
		public bool IsChangeAtoneToCvsDef
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			}
		}
		/// <summary>Atoneを請求に変更</summary>
		public bool IsChangeAtoneToGmoPost
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));
			}
		}
		/// <summary>Atone To Carrier</summary>
		public bool IsChangeAtoneToCarrier
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>Atone→決済無しに変更</summary>
		public bool IsChangeAtoneToNoPayment
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>Change Atone To Pay Pal</summary>
		public bool IsChangeAtoneToPayPal
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>Change Atone To Line Pay</summary>
		public bool IsChangeAtoneToLinePay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY);
			}
		}
		/// <summary>Change Atone To PayPay</summary>
		public bool IsChangeAtoneToPayPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY);
			}
		}
		/// <summary>Change Atone To Boku</summary>
		public bool IsChangeAtoneToBoku
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>
		/// Is Atone Return All Items
		/// </summary>
		public bool IsAtoneReturnAllItems
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems;
			}
		}
		/// <summary>Is No Change Atone</summary>
		public bool IsNoChangeAftee
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			}
		}
		/// <summary>Aftee to credit</summary>
		public bool IsChangeAfteeToCredit
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>Aftee To AmazonPay</summary>
		public bool IsChangeAfteeToAmazonPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>Aftee To TriLink AfterPay</summary>
		public bool IsChangeAfteeToTriLinkAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>Is Change Aftee To Collect</summary>
		public bool IsChangeAfteeToCollect
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>Change Aftee To Cvs Def</summary>
		public bool IsChangeAfteeToCvsDef
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			}
		}
		/// <summary>Afteeを請求に変更</summary>
		public bool IsChangeAfteeToGmoPost
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));
		}
		}
		/// <summary>Aftee To Carrier</summary>
		public bool IsChangeAfteeToCarrier
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>Aftee→決済無しに変更</summary>
		public bool IsChangeAfteeToNoPayment
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>Change Aftee To Pay Pal</summary>
		public bool IsChangeAfteeToPayPal
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>Change Aftee To Line Pay</summary>
		public bool IsChangeAfteeToLinePay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY);
			}
		}
		/// <summary>Change Aftee To PayPay</summary>
		public bool IsChangeAfteeToPayPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY);
			}
		}
		/// <summary>Change Aftee To Boku</summary>
		public bool IsChangeAfteeToBoku
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>
		/// Is Aftee Return All Items
		/// </summary>
		public bool IsAfteeReturnAllItems
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems;
			}
		}
		/// <summary>Is No Change Line Pay</summary>
		public bool IsNoChangeLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>Is Change Line Pay To Credit</summary>
		public bool IsChangeLinePayToCredit
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT));
			}
		}
		/// <summary>Is Change Line Pay To Amazon Pay</summary>
		public bool IsChangeLinePayToAmazonPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn)));
			}
		}
		/// <summary>Is Change Line Pay To TriLink After Pay</summary>
		public bool IsChangeLinePayToTriLinkAfterPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY));
			}
		}
		/// <summary>Is Change Line Pay To Paidy Pay</summary>
		public bool IsChangeLinePayToPaidyPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY));
			}
		}
		/// <summary>Is Change Line Pay To Collect</summary>
		public bool IsChangeLinePayToCollect
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (IsCollectPaymentOrOthers(this.OrderNew)));
			}
		}
		/// <summary>Is Change Line Pay To CvsDef</summary>
		public bool IsChangeLinePayToCvsDef
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));
			}
		}
		/// <summary>Line Payを請求に変更</summary>
		public bool IsChangeLinePayToGmoPost
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)));
			}
		}
		/// <summary>Is Change Line Pay To Carrier</summary>
		public bool IsChangeLinePayToCarrier
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& IsCarrierPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>Is Change Line Pay To No Payment</summary>
		public bool IsChangeLinePayToNoPayment
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT));
			}
		}
		/// <summary>Is Change Line Pay To PayPal</summary>
		public bool IsChangeLinePayToPayPal
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL));
			}
		}
		/// <summary>Is Change Line Pay To Aftee</summary>
		public bool IsChangeLinePayToAftee
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE));
			}
		}
		/// <summary>Is Change Line Pay To Atone</summary>
		public bool IsChangeLinePayToAtone
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE));
			}
		}
		/// <summary>Is Change Line Pay To PayPay</summary>
		public bool IsChangeLinePayToPayPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			}
		}
		/// <summary>Is Change Line Pay To Boku</summary>
		public bool IsChangeLinePayToBoku
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>
		/// Is Line Pay Return All Items
		/// </summary>
		public bool IsLinePayReturnAllItems
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems);
			}
		}
		/// <summary>決済種別が決済無し→クレジットカードに変更</summary>
		public bool IsChangeNoPaymentToCredit
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>決済種別が決済無し→コンビニ後払いに変更</summary>
		public bool IsChangeNoPaymentToCvsDef
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			}
		}
		/// <summary>支払なしを請求に変更</summary>
		public bool IsChangeNoPaymentToGmoPost
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));
			}
		}
		/// <summary>決済種別が決済無し→後付款(TriLink後払い)に変更</summary>
		public bool IsChangeNoPaymentToTriLinkAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>決済種別が決済無し→PayPalに変更</summary>
		public bool IsChangeNoPaymentToPayPal
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>決済種別が決済無し→キャリア決済</summary>
		public bool IsChangeNoPaymentToCarrier
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>決済種別が決済無し→代引きに変更</summary>
		public bool IsChangeNoPaymentToCollect
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					&& IsCollectPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>決済種別が決済無し→Atone</summary>
		public bool IsChangeNoPaymentToAtone
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& IsCollectPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>決済種別が決済無し→Aftee</summary>
		public bool IsChangeNoPaymentToAftee
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& IsCollectPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>決済種別が決済無し→LINEPay</summary>
		public bool IsChangeNoPaymentToLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>決済種別が決済無し→Amazon Pay</summary>
		public bool IsChangeNoPaymentToAmazonPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>決済種別が決済無し→Paidy Pay</summary>
		public bool IsChangeNoPaymentToPaidyPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			}
		}
		/// <summary>決済種別が決済無し→PayPay</summary>
		public bool IsChangeNoPaymentToPayPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY);
			}
		}
		/// <summary>キャリア決済 OR 代引き OR 決済無しの全返品（最終請求金額＝0）</summary>
		public bool IsReturnAllItemsCarrierOrCollectOrNoPayment
		{
			get
			{
				return (IsCollectPaymentOrOthers(this.OrderOld)
						|| (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
						|| IsCarrierPaymentOrOthers(this.OrderOld))
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems;
			}
		}
		/// <summary>決済種別がPaidyPay変更なし</summary>
		public bool IsNoChangePaidyPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			}
		}
		/// <summary>決済種別がPaidyPay→クレジットカードに変更</summary>
		public bool IsChangePaidyPayToCredit
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>決済種別がPaidyPay→コンビニ後払いに変更</summary>
		public bool IsChangePaidyPayToCvsDef
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			}
		}
		/// <summary>PaidyPayを請求に変更</summary>
		public bool IsChangePaidyPayToGmoPost
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));
			}
		}
		/// <summary>決済種別がPaidyPay→後付款(TriLink後払い)に変更</summary>
		public bool IsChangePaidyPayToTriLinkAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>決済種別がPaidyPay→LINEPay</summary>
		public bool IsChangePaidyPayToLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>決済種別がPaidyPay→代引きに変更</summary>
		public bool IsChangePaidyPayToCollect
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>決済種別がPaidyPay→キャリア決済</summary>
		public bool IsChangePaidyPayToCarrier
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>決済種別がPaidyPay→PayPayに変更</summary>
		public bool IsChangePaidyPayToPayPal
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>決済種別がPaidyPay→Amazon Pay</summary>
		public bool IsChangePaidyPayToAmazonPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>決済種別がPaidyPay→決済無しに変更</summary>
		public bool IsChangePaidyPayToNoPayment
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>決済種別がPaidyPay→PayPay</summary>
		public bool IsChangePaidyPayToPayPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY);
			}
		}
		/// <summary>決済種別がPaidyPay→Boku</summary>
		public bool IsChangePaidyPayToBoku
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>PaidyPay全返品（最終請求金額=0</summary>
		public bool IsPaidyPayReturnAllItems
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems;
			}
		}
		/// <summary>
		/// 決済種別がクレジットカードで、別のカードに変更されたとき
		/// </summary>
		public bool IsNoChangeCreditButAnotherCard
		{
			get
			{
				// 管理画面注文変更で別のカードに変更する時
				return this.IsNoChangeCredit
					&& (this.OrderNew.CreditBranchNo != this.OrderOld.CreditBranchNo);
			}
		}
		/// <summary>
		/// 決済種別がクレジットカードで、支払回数を変更したとき
		/// </summary>
		public bool IsNoChangeCreditButInstallmentsCode
		{
			get
			{
				// 管理画面注文変更で支払回数変更した時
				return this.IsNoChangeCredit
					&& (this.OrderNew.CardInstallmentsCode != this.OrderOld.CardInstallmentsCode);
			}
		}
		/// <summary>
		/// 決済種別がAmazon Payで、決済注文IDに変更があるとき
		/// </summary>
		public bool IsChangeAmazonPaymentOrderId
		{
			get
			{
				return this.IsNoChangeAmazonPay
					&& (this.OrderNew.PaymentOrderId != this.OrderOld.PaymentOrderId);
			}
		}
		/// <summary>
		/// 決済種別がAmazon Payで、決済注文IDに変更がないとき
		/// </summary>
		public bool IsNoChangeAmazonPaymentOrderId
		{
			get
			{
				return this.IsNoChangeAmazonPay
					&& (this.OrderNew.PaymentOrderId == this.OrderOld.PaymentOrderId);
			}
		}
		/// <summary>
		/// 決済種別がAtoneで、決済注文IDに変更がないとき
		/// </summary>
		public bool IsNoChangeAtoneCardTranId
		{
			get
			{
				return this.IsNoChangeAtone
					&& (this.OrderNew.CardTranId == this.OrderOld.CardTranId);
			}
		}
		/// <summary>
		/// 決済種別がAfteeで、決済注文IDに変更がないとき
		/// </summary>
		public bool IsNoChangeAfteeCardTranId
		{
			get
			{
				return this.IsNoChangeAftee
					&& (this.OrderNew.CardTranId == this.OrderOld.CardTranId);
			}
		}
		/// <summary>クレジットカード→NP後払い</summary>
		public bool IsChangeCreditToNPAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>決済種別がコンビニ後払い→NP後払い</summary>
		public bool IsChangeCvsDefToNPAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>決済種別がPayPal→NP後払い</summary>
		public bool IsChangePayPalToNPAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>決済種別が後付款(TriLink後払い)→NP後払い</summary>
		public bool IsChangeTriLinkAfterPayToNPAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>決済種別がキャリア決済→NP後払い</summary>
		public bool IsChangeCarrierToNPAfterPay
		{
			get
			{
				return IsCarrierPaymentOrOthers(this.OrderOld)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>決済種別が代引き→NP後払い</summary>
		public bool IsChangeCollectToNPAfterPay
		{
			get
			{
				return (IsCollectPaymentOrOthers(this.OrderOld))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>決済種別がAmazonPay→NP後払い</summary>
		public bool IsChangeAmazonPayToNPAfterPay
		{
			get
			{
				return (OrderCommon.IsAmazonPayment(this.OrderOld.OrderPaymentKbn))
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>Is No Change NP After Pay</summary>
		public bool IsNoChangeNPAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>Is Change NP After Pay To Credit</summary>
		public bool IsChangeNPAfterPayToCredit
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			}
		}
		/// <summary>Is Change NP After Pay To AmazonPay</summary>
		public bool IsChangeNPAfterPayToAmazonPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn));
			}
		}
		/// <summary>Is Change NP After Pay To TriLink AfterPay</summary>
		public bool IsChangeNPAfterPayToTriLinkAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			}
		}
		/// <summary>Is Change NP After Pay To Paidy Pay</summary>
		public bool IsChangeNPAfterPayToPaidyPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			}
		}
		/// <summary>Is Change NP After Pay To Collect</summary>
		public bool IsChangeNPAfterPayToCollect
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>Is Change NP After Pay To Cvs Def</summary>
		public bool IsChangeNPAfterPayToCvsDef
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			}
		}
		/// <summary>NP After Payを請求に変更</summary>
		public bool IsChangeNPAfterPayToGmoPost
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));
			}
		}
		/// <summary>Is Change NP After Pay To Carrier</summary>
		public bool IsChangeNPAfterPayToCarrier
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& IsCarrierPaymentOrOthers(this.OrderNew);
			}
		}
		/// <summary>Is Change NP After Pay To No Payment</summary>
		public bool IsChangeNPAfterPayToNoPayment
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			}
		}
		/// <summary>Is Change NP After Pay To Pay Pal</summary>
		public bool IsChangeNPAfterPayToPayPal
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			}
		}
		/// <summary>Is Change NP After Pay To Atone</summary>
		public bool IsChangeNPAfterPayToAtone
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			}
		}
		/// <summary>Is Change NP After Pay To Aftee</summary>
		public bool IsChangeNPAfterPayToAftee
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			}
		}
		/// <summary>Is Change NP After Pay To Line Pay</summary>
		public bool IsChangeNPAfterPayToLinePay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY);
			}
		}
		/// <summary>Is Change NP After Pay To PayPay</summary>
		public bool IsChangeNPAfterPayToPayPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY);
			}
		}
		/// <summary>Is Change NP After Pay To Boku</summary>
		public bool IsChangeNPAfterPayToBoku
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>決済種別が決済無し→NP After Pay</summary>
		public bool IsChangeNoPaymentToNPAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>決済種別がPaidyPay→NP後払い</summary>
		public bool IsChangePaidyPayToNPAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>決済種別がAftee→NP後払い</summary>
		public bool IsChangeAfteeToNPAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>決済種別がAtone→NP後払い</summary>
		public bool IsChangeAtoneToNPAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>決済種別がLINEPay→NP後払い</summary>
		public bool IsChangeLinePayToNPAfterPay
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			}
		}
		/// <summary>
		/// Is NP After Pay Return All Items
		/// </summary>
		public bool IsNPAfterPayReturnAllItems
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems);
			}
		}
		/// <summary>Is No Change EcPay</summary>
		public bool IsNoChangeEcPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY));
			}
		}
		/// <summary>EcPay To Credit</summary>
		public bool IsChangeEcPayToCredit
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT));
			}
		}
		/// <summary>EcPay To AmazonPay</summary>
		public bool IsChangeEcPayToAmazonPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (OrderCommon.IsAmazonPayment(this.OrderNew.OrderPaymentKbn)));
			}
		}
		/// <summary>EcPay To TriLink AfterPay</summary>
		public bool IsChangeEcPayToTriLinkAfterPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY));
			}
		}
		/// <summary>Is Change EcPay To Collect</summary>
		public bool IsChangeEcPayToCollect
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>Change EcPay To Cvs Def</summary>
		public bool IsChangeEcPayToCvsDef
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));
			}
		}
		/// <summary>EcPayを請求に変更</summary>
		public bool IsChangeEcPayToGmoPost
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)));
			}
		}
		/// <summary>EcPay To Carrier</summary>
		public bool IsChangeEcPayToCarrier
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& IsCarrierPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>EcPay→決済無しに変更</summary>
		public bool IsChangeEcPayToNoPayment
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT));
			}
		}
		/// <summary>Change EcPay To Pay Pal</summary>
		public bool IsChangeEcPayToPayPal
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL));
			}
		}
		/// <summary>Change EcPay To PaidyPay</summary>
		public bool IsChangeEcPayToPaidyPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY));
			}
		}
		/// <summary>Change EcPay To Atone</summary>
		public bool IsChangeEcPayToAtone
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE));
			}
		}
		/// <summary>Change EcPay To Aftee</summary>
		public bool IsChangeEcPayToAftee
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE));
			}
		}
		/// <summary>Change EcPay To LINEPay</summary>
		public bool IsChangeEcPayToLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>Change EcPay To NP後払い</summary>
		public bool IsChangeEcPayToNPAfterPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY));
			}
		}
		/// <summary>Change EcPay To PayPay</summary>
		public bool IsChangeEcPayToPayPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			}
		}
		/// <summary>Change EcPay To Boku</summary>
		public bool IsChangeEcPayToBoku
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>Is EcPay Return All Items</summary>
		public bool IsEcPayReturnAllItems
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems);
			}
		}
		/// <summary>Is No Change NewebPay</summary>
		public bool IsNoChangeNewebPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY));
			}
		}
		/// <summary>NewebPay To Credit</summary>
		public bool IsChangeNewebPayToCredit
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT));
			}
		}
		/// <summary>NewebPay To AmazonPay</summary>
		public bool IsChangeNewebPayToAmazonPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT));
			}
		}
		/// <summary>NewebPay To TriLink AfterPay</summary>
		public bool IsChangeNewebPayToTriLinkAfterPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY));
			}
		}
		/// <summary>NewebPay To Collect</summary>
		public bool IsChangeNewebPayToCollect
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>NewebPay To Cvs Def</summary>
		public bool IsChangeNewebPayToCvsDef
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));
			}
		}
		/// <summary>NewebPayを請求に変更</summary>
		public bool IsChangeNewebPayToGmoPost
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)));
			}
		}
		/// <summary>NewebPay To Carrier</summary>
		public bool IsChangeNewebPayToCarrier
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& IsCarrierPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>NewebPay→決済無しに変更</summary>
		public bool IsChangeNewebPayToNoPayment
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT));
			}
		}
		/// <summary>NewebPay To Pay Pal</summary>
		public bool IsChangeNewebPayToPayPal
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL));
			}
		}
		/// <summary>NewebPay To PaidyPay</summary>
		public bool IsChangeNewebPayToPaidyPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY));
			}
		}
		/// <summary>NewebPay To Atone</summary>
		public bool IsChangeNewebPayToAtone
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE));
			}
		}
		/// <summary>NewebPay To Aftee</summary>
		public bool IsChangeNewebPayToAftee
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE));
			}
		}
		/// <summary>NewebPay To LINEPay</summary>
		public bool IsChangeNewebPayToLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>NewebPay To NP後払い</summary>
		public bool IsChangeNewebPayToNPAfterPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY));
			}
		}
		/// <summary>NewebPay To Paypay</summary>
		public bool IsChangeNewebPayToPaypay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			}
		}
		/// <summary>NewebPay To Boku</summary>
		public bool IsChangeNewebPayToBoku
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>Is NewebPay Return All Items</summary>
		public bool IsNewebPayReturnAllItems
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems);
			}
		}
		/// <summary>Is No Change Boku</summary>
		public bool IsNoChangeBoku
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			}
		}
		/// <summary>Boku To Credit</summary>
		public bool IsChangeBokuToCredit
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT));
			}
		}
		/// <summary>Boku To AmazonPay</summary>
		public bool IsChangeBokuToAmazonPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT));
			}
		}
		/// <summary>Boku To TriLink AfterPay</summary>
		public bool IsChangeBokuToTriLinkAfterPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY));
			}
		}
		/// <summary>Boku To Collect</summary>
		public bool IsChangeBokuToCollect
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>Boku To Cvs Def</summary>
		public bool IsChangeBokuToCvsDef
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));
			}
		}
		/// <summary>Boku To Carrier</summary>
		public bool IsChangeBokuToCarrier
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& IsCarrierPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>Boku→決済無しに変更</summary>
		public bool IsChangeBokuToNoPayment
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT));
			}
		}
		/// <summary>Boku To Pay Pal</summary>
		public bool IsChangeBokuToPayPal
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL));
			}
		}
		/// <summary>Boku To PaidyPay</summary>
		public bool IsChangeBokuToPaidyPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY));
			}
		}
		/// <summary>Boku To Atone</summary>
		public bool IsChangeBokuToAtone
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE));
			}
		}
		/// <summary>Boku To Aftee</summary>
		public bool IsChangeBokuToAftee
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE));
			}
		}
		/// <summary>Boku To LINEPay</summary>
		public bool IsChangeBokuToLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>Boku To NP後払い</summary>
		public bool IsChangeBokuToNPAfterPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY));
			}
		}
		/// <summary>GMOアトカラ変更なし</summary>
		public bool IsNoChangeGmoAtokara
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA));
			}
		}
		/// <summary>GMOアトカラ → Credit</summary>
		public bool IsChangeGmoAtokaraToCredit
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT));
			}
		}
		/// <summary>GMOアトカラ → AmazonPay</summary>
		public bool IsChangeGmoAtokaraToAmazonPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT));
			}
		}
		/// <summary>GMOアトカラ → TriLink AfterPay</summary>
		public bool IsChangeGmoAtokaraToTriLinkAfterPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY));
			}
		}
		/// <summary>GMOアトカラ → Collect</summary>
		public bool IsChangeGmoAtokaraToCollect
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>GMOアトカラ → Cvs Def</summary>
		public bool IsChangeGmoAtokaraToCvsDef
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));
			}
		}
		/// <summary>GMOアトカラ → Carrier</summary>
		public bool IsChangeGmoAtokaraToCarrier
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& IsCarrierPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>GmoAtokara→決済無しに変更</summary>
		public bool IsChangeGmoAtokaraToNoPayment
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT));
			}
		}
		/// <summary>GMOアトカラ → Pay Pal</summary>
		public bool IsChangeGmoAtokaraToPayPal
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL));
			}
		}
		/// <summary>GMOアトカラ → PaidyPay</summary>
		public bool IsChangeGmoAtokaraToPaidyPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY));
			}
		}
		/// <summary>GMOアトカラ → Atone</summary>
		public bool IsChangeGmoAtokaraToAtone
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE));
			}
		}
		/// <summary>GMOアトカラ → Aftee</summary>
		public bool IsChangeGmoAtokaraToAftee
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE));
			}
		}
		/// <summary>GMOアトカラ → LINEPay</summary>
		public bool IsChangeGmoAtokaraToLinePay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
			}
		}
		/// <summary>GMOアトカラ → NP後払い</summary>
		public bool IsChangeGmoAtokaraToNPAfterPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY));
			}
		}
		/// <summary>Is Sales Order</summary>
		public bool IsSalesOrder
		{
			get
			{
				var result = ((this.OrderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
					&& (this.OrderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP));
				return result;
			}
		}
		/// <summary>外部決済なしで変更なし
		/// ※キャリア決済含む。（EC管理で与信不可のため）
		/// </summary>
		public bool IsNoChangeNotExternalPayment
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == this.OrderNew.OrderPaymentKbn)
					&& (IsCollectPaymentOrOthers(this.OrderNew)
						|| (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
						|| IsCarrierPaymentOrOthers(this.OrderNew)
						|| (Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN) == false)
						|| (Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN) == false)));
			}
		}
		/// <summary>
		/// 請求金額の増額あり？
		/// </summary>
		public bool IsIncreasedBilledAmount
		{
			get
			{
				return (this.OrderNew.LastBilledAmount > this.OrderOld.LastBilledAmount);
			}
		}
		/// <summary>
		/// 請求金額の減額あり？
		/// </summary>
		public bool IsReducedBilledAmount
		{
			get
			{
				return (this.OrderNew.LastBilledAmount < this.OrderOld.LastBilledAmount);
			}
		}
		/// <summary>
		/// 最終請求金額の変更あり？
		/// </summary>
		public bool IsChangedAmount
		{
			get
			{
				return (this.IsIncreasedBilledAmount || this.IsReducedBilledAmount);
			}
		}
		/// <summary>
		/// 請求金額が0より大きい？
		/// </summary>
		public bool IsBilledAmountGreaterThanZero
		{
			get
			{
				return (this.OrderNew.LastBilledAmount > 0);
			}
		}
		/// <summary>
		/// 注文者名 or 電話番号を変更あり？
		/// </summary>
		public bool IsChangeOwnerNameOrTel
		{
			get
			{
				return ((this.OrderOld.Owner.OwnerName != this.OrderNew.Owner.OwnerName)
					|| (this.OrderOld.Owner.OwnerTel1 != this.OrderNew.Owner.OwnerTel1));
			}
		}
		/// <summary>Is Change Owner Address?</summary>
		public bool IsChangeOwnerAddress
		{
			get
			{
				return ((this.OrderOld.Owner.OwnerZip != this.OrderNew.Owner.OwnerZip)
					|| (this.OrderOld.Owner.OwnerAddr1 != this.OrderNew.Owner.OwnerAddr1)
					|| (this.OrderOld.Owner.OwnerAddr2 != this.OrderNew.Owner.OwnerAddr2)
					|| (this.OrderOld.Owner.OwnerAddr3 != this.OrderNew.Owner.OwnerAddr3)
					|| (this.OrderOld.Owner.OwnerAddr4 != this.OrderNew.Owner.OwnerAddr4)
					|| (this.OrderOld.Owner.OwnerAddr5 != this.OrderNew.Owner.OwnerAddr5));
			}
		}
		/// <summary>配送先住所変更有?</summary>
		public bool IsChangeShippingAddress
		{
			get
			{
				return ((this.OrderOld.Shippings[0].ShippingZip != this.OrderNew.Shippings[0].ShippingZip)
					|| (this.OrderOld.Shippings[0].ShippingAddr1 != this.OrderNew.Shippings[0].ShippingAddr1)
					|| (this.OrderOld.Shippings[0].ShippingAddr2 != this.OrderNew.Shippings[0].ShippingAddr2)
					|| (this.OrderOld.Shippings[0].ShippingAddr3 != this.OrderNew.Shippings[0].ShippingAddr3)
					|| (this.OrderOld.Shippings[0].ShippingAddr4 != this.OrderNew.Shippings[0].ShippingAddr4)
					|| (this.OrderOld.Shippings[0].ShippingAddr5 != this.OrderNew.Shippings[0].ShippingAddr5));
			}
		}
		/// <summary>
		/// 配送先変更あり？
		/// </summary>
		public bool IsChangeShippingInfo
		{
			get
			{
				return ((this.OrderOld.Shippings[0].ShippingName1 != this.OrderNew.Shippings[0].ShippingName1)
						|| (this.OrderOld.Shippings[0].ShippingName2 != this.OrderNew.Shippings[0].ShippingName2)
						|| (this.OrderOld.Shippings[0].ShippingName != this.OrderNew.Shippings[0].ShippingName)
						|| (this.OrderOld.Shippings[0].ShippingCompanyName != this.OrderNew.Shippings[0].ShippingCompanyName)
						|| (this.OrderOld.Shippings[0].ShippingAddr5 != this.OrderNew.Shippings[0].ShippingAddr5)
						|| (this.OrderOld.Shippings[0].ShippingAddr4 != this.OrderNew.Shippings[0].ShippingAddr4)
						|| (this.OrderOld.Shippings[0].ShippingAddr3 != this.OrderNew.Shippings[0].ShippingAddr3)
						|| (this.OrderOld.Shippings[0].ShippingAddr2 != this.OrderNew.Shippings[0].ShippingAddr2)
						|| (this.OrderOld.Shippings[0].ShippingAddr1 != this.OrderNew.Shippings[0].ShippingAddr1)
						|| (this.OrderOld.Shippings[0].ShippingZip != this.OrderNew.Shippings[0].ShippingZip)
						|| (this.OrderOld.Shippings[0].ShippingCountryIsoCode != this.OrderNew.Shippings[0].ShippingCountryIsoCode));
			}
		}
		/// <summary>Is Change Owner Info ?</summary>
		public bool IsChangeOwnerInfo
		{
			get
			{
				return ((this.OrderOld.Owner.OwnerName1 != this.OrderNew.Owner.OwnerName1)
					|| (this.OrderOld.Owner.OwnerName2 != this.OrderNew.Owner.OwnerName2)
					|| (this.OrderOld.Owner.OwnerNameKana1 != this.OrderNew.Owner.OwnerNameKana1)
					|| (this.OrderOld.Owner.OwnerNameKana2 != this.OrderNew.Owner.OwnerNameKana2)
					|| (this.OrderOld.Owner.OwnerMailAddr != this.OrderNew.Owner.OwnerMailAddr)
					|| (this.OrderOld.Owner.OwnerMailAddr2 != this.OrderNew.Owner.OwnerMailAddr2)
					|| (this.OrderOld.Owner.OwnerZip != this.OrderNew.Owner.OwnerZip)
					|| (this.OrderOld.Owner.OwnerAddr1 != this.OrderNew.Owner.OwnerAddr1)
					|| (this.OrderOld.Owner.OwnerAddr2 != this.OrderNew.Owner.OwnerAddr2)
					|| (this.OrderOld.Owner.OwnerAddr3 != this.OrderNew.Owner.OwnerAddr3)
					|| (this.OrderOld.Owner.OwnerAddr4 != this.OrderNew.Owner.OwnerAddr4)
					|| (this.OrderOld.Owner.OwnerAddr5 != this.OrderNew.Owner.OwnerAddr5)
					|| (this.OrderOld.Owner.OwnerTel1 != this.OrderNew.Owner.OwnerTel1)
					|| (this.OrderOld.Owner.OwnerTel2 != this.OrderNew.Owner.OwnerTel2)
					|| (this.OrderOld.Owner.OwnerTel3 != this.OrderNew.Owner.OwnerTel3)
					|| (this.OrderOld.Owner.OwnerSex != this.OrderNew.Owner.OwnerSex)
					|| (this.OrderOld.Owner.OwnerBirth != this.OrderNew.Owner.OwnerBirth)
					|| (this.OrderOld.Owner.OwnerCompanyName != this.OrderNew.Owner.OwnerCompanyName));
			}
		}
		/// <summary>
		/// Is Change Shipping Name Kana Or Tel
		/// </summary>
		public bool IsChangeShippingNameKanaOrTel
		{
			get
			{
				return ((this.OrderOld.Shippings[0].ShippingNameKana1 != this.OrderNew.Shippings[0].ShippingNameKana1)
					|| (this.OrderOld.Shippings[0].ShippingNameKana2 != this.OrderNew.Shippings[0].ShippingNameKana2)
					|| (this.OrderOld.Shippings[0].ShippingNameKana != this.OrderNew.Shippings[0].ShippingNameKana)
					|| (this.OrderOld.Shippings[0].ShippingTel1 != this.OrderNew.Shippings[0].ShippingTel1)
					|| (this.OrderOld.Shippings[0].ShippingTel2 != this.OrderNew.Shippings[0].ShippingTel2)
					|| (this.OrderOld.Shippings[0].ShippingTel3 != this.OrderNew.Shippings[0].ShippingTel3));
			}
		}
		/// <summary>
		/// Is Change Order Info
		/// </summary>
		public bool IsChangeOrderInfo
		{
			get
			{
				return (this.IsChangeShippingInfo
					|| this.IsChangeOwnerInfo
					|| this.IsChangeShippingNameKanaOrTel);
			}
		}
		/// <summary>Is Change Product Infomation ?</summary>
		public bool IsChangeProductInfomation
		{
			get
			{
				var productInfomationsOrderOld = this.OrderOld.Shippings[0].Items
					.Select(item => string.Format(
						"{0}{1}{2}{3}{4}{5}",
						item.ProductId,
						item.VariationId,
						item.ProductName,
						item.ProductNameKana,
						item.ItemQuantity,
						item.ProductPrice))
					.ToArray();
				var productInfomationsOrderNew = this.OrderNew.Shippings[0].Items
					.Select(item => string.Format(
						"{0}{1}{2}{3}{4}{5}",
						item.ProductId,
						item.VariationId,
						item.ProductName,
						item.ProductNameKana,
						item.ItemQuantity,
						item.ProductPrice))
					.ToArray();
				var isChangeInfomationProduct = productInfomationsOrderNew.Any(item => productInfomationsOrderOld.Contains(item) == false);
				return isChangeInfomationProduct;
			}
		}
		/// <summary>
		/// 配送希望日変更有？
		/// </summary>
		public bool IsChangeShippingDate
		{
			get
			{
				return (this.OrderOld.Shippings[0].ShippingDate != this.OrderNew.Shippings[0].ShippingDate);
			}
		}
		/// <summary>
		/// コンビニ後払いで出荷報告連携済？
		/// </summary>
		public bool IsCvsDefShipRegistComp
		{
			get
			{
				return (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF) && (new List<string>
				{
					Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP,
					Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP,
					Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_DELI_COMP,
					Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP,
					Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
				}.Contains(this.OrderOld.ExternalPaymentStatus));
			}
		}
		/// <summary>
		/// GMOコンビニ後払いであるか？
		/// </summary>
		public bool IsGmoCvsDef
		{
			get { return Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo; }
		}
		/// <summary>
		/// Atodeneコンビニ後払いであるか？
		/// </summary>
		public bool IsAtodeneCvsDef
		{
			get { return Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene; }
		}
		/// <summary>DSKコンビニ後払いであるか？</summary>
		public bool IsDskCvsDef
		{
			get { return Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk; }
		}
		/// <summary>スコア後払いであるか？</summary>
		public bool IsScoreCvsDef
		{
			get { return Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score; }
		}
		/// <summary>ベリトランス後払いであるか？</summary>
		public bool IsVeritransDef
		{
			get { return Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans; }
		}
		/// <summary>PayPay変更なし</summary>
		public bool IsNoChangePayPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY));
			}
		}
		/// <summary>決済がPayPay → クレジットカード</summary>
		public bool IsChangePayPayToCredit
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT));
			}
		}
		/// <summary>決済がPayPay → コンビニ後払い</summary>
		public bool IsChangePayPayToCvsDef
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));
			}
		}
		/// <summary>PayPayを請求に変更</summary>
		public bool IsChangePayPayToGmoPost
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
					&& ((this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)));
			}
		}
		/// <summary>決済がPayPay → 代引き/summary>
		public bool IsChangePayPayToCollect
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
					&& IsCollectPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>決済がPayPay → キャリア決済</summary>
		public bool IsChangePayPayToCarrier
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
					&& IsCarrierPaymentOrOthers(this.OrderNew));
			}
		}
		/// <summary>決済がPayPay → AmazonPay</summary>
		public bool IsChangePayPayToAmazonPay
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT));
			}
		}
		/// <summary>決済がPayPay → 決済なし</summary>
		public bool IsChangePayPayToNoPayment
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
					&& (this.OrderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT));
			}
		}
		/// <summary>PayPay全返品</summary>
		public bool IsPayPayReturnAllItems
		{
			get
			{
				return ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
					&& (this.OrderNew.LastBilledAmount == 0)
					&& this.IsReturnAllItems);
			}
		}
		/// <summary>PayPay処理が実行可能か</summary>
		public bool CanExecOrderPayPayAction
		{
			get
			{
				return ((string.IsNullOrEmpty(this.OrderOld.CardTranId) == false)
					&& (string.IsNullOrEmpty(this.OrderOld.PaymentOrderId) == false));
			}
		}
		/// <summary>PayPay売上確定か(ベリトランスの場合、複数回返金できるため、外部決済ステータスを見ない)</summary>
		private bool IsSalesOrderPayPay
		{
			get
			{
				return (this.IsNoChangePayPay
					&& ((this.OrderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
						|| (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans))
					&& (this.OrderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED));
			}
		}
		/// <summary>
		/// 後払いcomコンビニ後払いであるか？
		/// </summary>
		public bool IsAtobaraicomCvsDef
		{
			get { return Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom; }
		}
		/// <summary>
		/// Sql Accessor
		/// </summary>
		public SqlAccessor Accessor { get; }
		#endregion
	}
}
