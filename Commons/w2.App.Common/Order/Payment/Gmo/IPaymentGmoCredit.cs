/*
=========================================================================================================
  Module      : GMOクレジット決済モジュールインターフェース(IPaymentGmoCredit.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Order.Register;
using w2.Common.Sql;

namespace w2.App.Common.Order.Payment.GMO
{
	/// <summary>
	/// GMOクレジット決済モジュールインターフェース
	/// </summary>
	public interface IPaymentGmoCredit : IService, IPaymentGmo
	{
		/// <summary>
		/// 取引登録実行
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="priceTotal">合計金額</param>
		/// <param name="execTypes">注文実行種別(未指定時は管理画面)</param>
		/// <returns>True:成功、False:失敗</returns>
		bool EntryTran(
			string gmoOrderId,
			decimal priceTotal,
			OrderRegisterBase.ExecTypes execTypes = OrderRegisterBase.ExecTypes.CommerceManager);

		/// <summary>
		/// 決済実行（カード入力）
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="creditCardNo">カード番号</param>
		/// <param name="creditCardExpire">有効期限</param>
		/// <param name="creditCardInstallments">支払方法</param>
		/// <param name="securityCode">セキュリティコード</param>
		/// <returns>True:成功、False:失敗</returns>
		[Obsolete("古い形式です。Tokenを利用した決済実行を利用するようにしてください。")]
		bool ExecTran(
			string gmoOrderId,
			string creditCardNo,
			string creditCardExpire,
			string creditCardInstallments,
			string securityCode);

		/// <summary>
		/// 決済実行（登録カード利用）
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <param name="creditCardInstallments">支払方法</param>
		/// <param name="securityCode">セキュリティコード</param>
		/// <param name="returnUrl">戻りURL</param>
		/// <returns>True:成功、False:失敗</returns>
		bool ExecTranUseCard(
			string gmoOrderId,
			string cardTranId,
			string gmoMemberId,
			string creditCardInstallments,
			string securityCode,
			string returnUrl = null);

		/// <summary>
		/// 決済実行（トークン利用）
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="creditCardInstallments">支払方法</param>
		/// <param name="token">カードトークン</param>
		/// <param name="returnUrl">戻りURL</param>
		/// <returns>True:成功、False:失敗</returns>
		bool ExecTran(
			string gmoOrderId,
			string creditCardInstallments,
			string token,
			string returnUrl = null);

		/// <summary>
		/// 決済実行（3Dセキュア1.0利用）
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="paRes">本人認証サービス結果</param>
		/// <param name="md">取引ID</param>
		/// <returns>True:成功、False:失敗</returns>
		bool SecureTran(
			string gmoOrderId,
			string paRes,
			string md);

		/// <summary>
		/// 決済実行（3Dセキュア2.0利用）
		/// </summary>
		/// <param name="cardTranId">決済取引ID</param>
		/// <returns>True:成功、False:失敗</returns>
		bool SecureTran2(string cardTranId);

		/// <summary>
		/// 仮売上⇒実売上実行
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="priceTotal">合計金額</param>
		/// <returns>True:成功、False:失敗</returns>
		bool Sales(
			string gmoOrderId,
			string cardTranId,
			decimal priceTotal);

		/// <summary>
		/// 会員参照実行
		/// </summary>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <returns>True:成功、False:失敗</returns>
		bool SearchMember(string gmoMemberId);

		/// <summary>
		/// カード参照実行
		/// </summary>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <returns>True:成功、False:失敗</returns>
		bool SearchCard(string gmoMemberId);

		/// <summary>
		/// 取引参照実行
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <returns>True:成功、False:失敗</returns>
		bool SearchTrade(string gmoOrderId);

		/// <summary>
		/// 金額変更があったかどうか？
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="priceTotal">合計金額</param>
		/// <returns>True:成功、False:失敗</returns>
		bool IsPriceChange(string gmoOrderId, string cardTranId, decimal priceTotal);

		/// <summary>
		/// 金額変更実行
		/// </summary>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="priceTotal">合計金額</param>
		/// <returns>True:成功、False:失敗</returns>
		bool ChangeTran(string gmoOrderId, string cardTranId, decimal priceTotal);

		/// <summary>
		/// 会員登録（※） ⇒ カード登録実行
		/// ※会員が存在しなければ登録
		/// </summary>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <param name="userName">ユーザ名</param>
		/// <param name="token">トークン文字列</param>
		/// <param name="authorName">名義人</param>
		/// <returns>True:成功、False:失敗</returns>
		bool SaveMemberAndCard(
			string gmoMemberId,
			string userName,
			string token,
			string authorName);

		/// <summary>
		/// 会員登録（※） ⇒ 決済後カード登録実行
		/// ※会員が存在しなければ登録
		/// </summary>
		/// <param name="gmoMemberId">GMO会員ID</param>
		/// <param name="userName">ユーザ名</param>
		/// <param name="gmoOrderId">GMO注文ID</param>
		/// <param name="authorName">名義人</param>
		/// <returns>True:成功、False:失敗</returns>
		bool SaveMemberAndTraedCard(
			string gmoMemberId,
			string userName,
			string gmoOrderId,
			string authorName);

		/// <summary>
		/// キャンセル処理(与信、売上確定のの日付に応じたキャンセルを行う)
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="cardTranId">決済カート取引ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns></returns>
		bool Cancel(string paymentOrderId, string cardTranId, string orderId, SqlAccessor accessor = null);
	}
}
