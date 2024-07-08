/*
=========================================================================================================
  Module      : 注文内容変更API OrderChangeのレスポンスクラス(OrderChangeResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Serialization;

namespace w2.App.Common.LohacoCreatorWebApi.OrderChange
{
	/// <summary>
	/// 注文内容変更API OrderChangeのレスポンスクラス
	/// </summary>
	[XmlRoot("ResultSet")]
	[Serializable]
	public class OrderChangeResponse : BaseResponse
	{
		#region +OrderChangeResponse デフォルトコンストラクタ
		/// <summary>
		/// 注文検索API OrderChangeのレスポンスクラスのデフォルトコンストラクタ
		/// </summary>
		public OrderChangeResponse()
		{
		}
		#endregion

		#region +WriteDebugLogWithMaskedPersonalInfo デバッグログ（個人情報がマスクされる状態）の出力
		/// <summary>
		/// デバッグログ（個人情報がマスクされる状態）の出力
		/// </summary>
		/// <returns>デバッグログ（個人情報がマスクされる状態）内容</returns>
		public override string WriteDebugLogWithMaskedPersonalInfo()
		{
			// 個人情報を含めないので、XML文字列のままに返却
			return WebApiHelper.SerializeXml(this);
		}
		#endregion

		#region プロパティ
		/// <summary>レスポンス一覧</summary>
		[XmlElement("Result")]
		public BaseResult Result { get; set; }
		#endregion
	}
}
