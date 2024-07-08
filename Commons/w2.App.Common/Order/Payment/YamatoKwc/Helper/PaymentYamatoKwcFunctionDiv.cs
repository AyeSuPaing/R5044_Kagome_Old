/*
=========================================================================================================
  Module      : ヤマトKWC 機能区分(PaymentYamatoKwcDeviceDiv.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.YamatoKwc.Helper
{
	/// <summary>
	/// ヤマトKWC機能区分
	/// </summary>
	public enum PaymentYamatoKwcFunctionDiv
	{
		/// <summary>クレジット決済登録（通常）</summary>
		A01,
		/// <summary>クレジット決済登録（３Ｄセキュア） ※未実装</summary>
		A02,
		/// <summary>お預かり情報照会</summary>
		A03,
		/// <summary>お預かり情報変更 ※未実装</summary>
		A04,
		/// <summary>お預かり情報削除</summary>
		A05,
		/// <summary>クレジット決済取消</summary>
		A06,
		/// <summary>クレジット金額変</summary>
		A07,
		/// <summary>トークン決済登録（通常）</summary>
		A08,
		/// <summary>トークン決済登録（３Ｄセキュア）</summary>
		A09,
		/// <summary>コンビニ決済登録（セブン-イレブン）</summary>
		B01,
		/// <summary>コンビニ決済登録（ファミリーマート）</summary>
		B02,
		/// <summary>コンビニ決済登録（ローソン）</summary>
		B03,
		/// <summary>コンビニ決済登録（サークルＫサンクス）</summary>
		B04,
		/// <summary>コンビニ決済登録（ミニストップ）</summary>
		B05,
		/// <summary>コンビニ決済登録（セイコーマート）</summary>
		B06,
		/// <summary>出荷情報登録</summary>
		E01,
		/// <summary>出荷情報取消</summary>
		E02,
		/// <summary>取引情報照会</summary>
		E04,
	}

	/// <summary>
	/// ヤマトKwc機能区分(コンビニ）
	/// </summary>
	public enum YamatoKwcFunctionDivCvs
	{
		/// <summary>コンビニ決済登録（セブン-イレブン）</summary>
		B01,
		/// <summary>コンビニ決済登録（ファミリーマート）</summary>
		B02,
		/// <summary>コンビニ決済登録（ローソン）</summary>
		B03,
		/// <summary>コンビニ決済登録（サークルＫサンクス）</summary>
		B04,
		/// <summary>コンビニ決済登録（ミニストップ）</summary>
		B05,
		/// <summary>コンビニ決済登録（セイコーマート）</summary>
		B06,
	}
}
