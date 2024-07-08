using System;
/*
=========================================================================================================
  Module      : リクエストデータ用の基底クラス(BaseRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using w2.App.Common.LohacoCreatorWebApi.OrderChange;
using w2.App.Common.LohacoCreatorWebApi.OrderInfo;
using w2.App.Common.LohacoCreatorWebApi.OrderList;
using w2.App.Common.LohacoCreatorWebApi.StockEdit;

namespace w2.App.Common.LohacoCreatorWebApi
{
	/// <summary>
	/// リクエスト用データの基底クラス
	/// </summary>
	[XmlRoot(ElementName = "Req")]
	[XmlInclude(typeof(OrderChangeRequest))]
	[XmlInclude(typeof(OrderInfoRequest))]
	[XmlInclude(typeof(OrderListRequest))]
	[XmlInclude(typeof(StockEditRequest))]
	[Serializable]
	public abstract class BaseRequest
	{
		#region #BaseRequest コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		protected BaseRequest()
		{
			this.FormUrlEncodedParameters = new Dictionary<string, string>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseRequest(string providerId)
		{
			this.FormUrlEncodedParameters = new Dictionary<string, string>();
			this.ProviderId = providerId;
		}
		#endregion

		#region +WriteDebugLog デバッグログの出力
		/// <summary>
		/// デバッグログの出力
		/// </summary>
		/// <param name="isMaskPersonalInfoEnabled">個人情報をマスクかどうか</param>
		/// <returns>デバッグログ内容</returns>
		public virtual string WriteDebugLog(bool isMaskPersonalInfoEnabled = true)
		{
			return (isMaskPersonalInfoEnabled) ? WriteDebugLogWithMaskedPersonalInfo() : WebApiHelper.SerializeXml(this);
		}
		#endregion

		#region +WriteDebugLogWithMaskedPersonalInfo デバッグログ（個人情報がマスクされる状態）の出力
		/// <summary>
		/// デバッグログ（個人情報がマスクされる状態）の出力
		/// </summary>
		/// <returns>デバッグログ（個人情報がマスクされる状態）内容</returns>
		public abstract string WriteDebugLogWithMaskedPersonalInfo();
		#endregion

		#region プロパティ
		/// <summary>リクエストUri</summary>
		[XmlIgnore]
		[JsonIgnore]
		public string Uri { get; set; }
		/// <summary>リクエストのデジタル署名</summary>
		[XmlIgnore]
		[JsonIgnore]
		public string Signature { get; set; }
		/// <summary>リクエストのストアアカウント</summary>
		[XmlIgnore]
		[JsonIgnore]
		public string ProviderId { get; set; }
		[XmlIgnore]
		[JsonIgnore]
		public LohacoConstants.ResponseContentType Accept { get; set; }
		/// <summary>リクエストのコンテンツタイプ</summary>
		[XmlIgnore]
		[JsonIgnore]
		public LohacoConstants.RequestContentType ContentType { get; set; }
		/// <summary>レスポンスのコンテンツタイプ</summary>
		/// <summary>ロハコ秘密鍵</summary>
		[XmlIgnore]
		[JsonIgnore]
		public string LohacoPrivateKey { get; set; }
		/// <summary>リクエストパラメータ一覧</summary>
		[XmlIgnore]
		[JsonIgnore]
		public Dictionary<string, string> FormUrlEncodedParameters { get; set; }
		#endregion
	}
}
