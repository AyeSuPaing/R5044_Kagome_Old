/*
=========================================================================================================
  Module      : Payment Boku Base Request(PaymentBokuBaseRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.Boku
{
	/// <summary>
	/// Payment Boku Base Request
	/// </summary>
	public abstract class PaymentBokuBaseRequest : IHttpApiRequestData
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Setting</param>
		public PaymentBokuBaseRequest(PaymentBokuSetting setting)
		{
			this.MerchantId = setting.MerchantId;
			this.MerchantRequestId = setting.MerchantRequestId;
		}
		#endregion

		#region Methods
		/// <summary>
		/// ポスト値生成
		/// </summary>
		/// <returns>生成したポスト値</returns>
		public virtual string CreatePostString()
		{
			return SerializeHelper.Serialize(this);
		}
		#endregion

		#region Properties
		/// <summary>Merchant id</summary>
		[XmlElement("merchant-id")]
		public string MerchantId { get; set; }
		/// <summary>Merchant reuqest id</summary>
		[XmlElement("merchant-request-id")]
		public string MerchantRequestId { get; set; }
		#endregion
	}
}
