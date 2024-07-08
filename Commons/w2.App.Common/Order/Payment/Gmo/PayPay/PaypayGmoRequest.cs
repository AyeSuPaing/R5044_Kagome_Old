/*
=========================================================================================================
  Module      : Paypay Request (PaypayGmoRequest.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.Paypay
{
	/// <summary>
	/// Paypay Request
	/// </summary>
	public abstract class PaypayGmoRequest : IHttpApiRequestData
	{
		/// <summary>
		/// ポスト文字列作成
		/// </summary>
		/// <returns>ポスト文字列</returns>
		public string CreatePostString()
		{
			switch (this.ApiType)
			{
				case ApiType.Json:
					var json = JsonConvert.SerializeObject(
						this,
						new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore
						});
					return json;

				case ApiType.IdPass:
					var idPass = IdPassDeserializer.Serialize(this);
					return idPass;

				default:
					throw new InvalidOperationException();
			}
		}

		/// <summary>API種別</summary>
		[JsonIgnore, IdPassIgnore]
		protected virtual ApiType ApiType
		{
			get { return ApiType.Json; }
		}
	}

	/// <summary>
	/// IdPassプロパティ属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	internal class IdPassPropertyAttribute : Attribute
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="key">キー</param>
		public IdPassPropertyAttribute(string key)
		{
			this.Key = key;
		}

		/// <summary>キー</summary>
		public string Key { get; set; }
	}

	/// <summary>
	/// IdPass無視属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	internal class IdPassIgnoreAttribute : Attribute
	{
	}
}
