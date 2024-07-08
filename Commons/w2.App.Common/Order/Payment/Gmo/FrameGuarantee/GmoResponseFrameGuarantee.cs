/*
=========================================================================================================
  Module      : Gmo Response Frame Guarantee(GmoResponseFrameGuarantee.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.FrameGuarantee
{
	/// <summary>
	/// APIレスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseFrameGuarantee : BaseGmoResponse
	{
		/// <summary>結果：OK？</summary>
		public bool IsResultOk
		{
			get { return (this.Result == ResultCode.OK); }
		}
		/// <summary>結果：NG？</summary>
		public bool IsResultNg
		{
			get { return (this.Result == ResultCode.NG); }
		}
	}

	#region TransactionResultElement
	/// <summary>
	/// 取引登録結果要素
	/// </summary>
	public class TransactionResultElement
	{
		/// <summary>加盟店取引ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId;

		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;

		/// <summary>自動審査結果</summary>
		[XmlElement("authorResult")]
		public string AuthorResult;
	}
	#endregion
}
