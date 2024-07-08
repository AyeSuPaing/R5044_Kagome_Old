/*
=========================================================================================================
  Module      : Score後払い定数定義(ScoreConstants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;
using w2.Common.Helper.Attribute;

namespace w2.App.Common.Order.Payment.Score
{
	/// <summary>処理結果</summary>
	public enum ScoreResult
	{
		/// <summary>OK</summary>
		[EnumTextName("OK")]
		Ok,
		/// <summary>NG</summary>
		[EnumTextName("NG")]
		Ng
	}

	/// <summary>審査結果</summary>
	public enum ScoreAuthorResult
	{
		/// <summary>OK</summary>
		[EnumTextName("OK")]
		Ok,
		/// <summary>NG</summary>
		[EnumTextName("NG")]
		Ng,
		/// <summary>保留</summary>
		[EnumTextName("保留")]
		Pending,
		/// <summary>審査中</summary>
		[EnumTextName("HOLD")]
		Hold,
	}

	/// <summary>決済種別</summary>
	public enum ScorePaymentType
	{
		/// <summary>別送サービス</summary>
		[XmlEnum(Name = "2")]
		SeparateService,
		/// <summary>同梱サービス</summary>
		[XmlEnum(Name = "3")]
		IncludeService
	}

	/// <summary>
	/// スコア後払いの定数クラス
	/// </summary>
	public class ScoreConstants
	{
		/// <summary>入金済みの注文をキャンセル失敗時のエラーコード</summary>
		public static string SCORE_CVSDEF_PAID_ERROR_CODE = "MULT0015";
	}
}
