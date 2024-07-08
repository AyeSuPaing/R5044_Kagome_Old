/*
=========================================================================================================
  Module      : Gmo Response Frame Guarantee Get Status(GmoResponseFrameGuaranteeGetStatus.cs)
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
	public class GmoResponseFrameGuaranteeGetStatus : BaseGmoResponse
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
		/// <summary>審査: OK?</summary>
		public bool IsOk
		{
			get { return ((this.Result == ResultCode.OK) && (this.Examination.Status == "OK")); }
		}
		/// <summary>審査: NG?</summary>
		public bool IsNG
		{
			get { return ((this.Result == ResultCode.OK) && (this.Examination.Status == "NG")); }
		}
		/// <summary>審査: 審査中？</summary>
		public bool IsInReview
		{
			get { return ((this.Result == ResultCode.OK) && (this.Examination.Status == Constants.CONST_RESPONSE_AUTHOR_RESULT_INREVIEW)); }
		}

		/// <summary>審査</summary>
		[XmlElement("examination")]
		public Examination Examination;
	}
	#region examination
	/// <summary>
	/// 審査
	/// </summary>
	public class Examination
	{
		/// <summary>コンストラクタ</summary>
		public Examination()
		{
			this.Status = string.Empty;
			this.ResultReasons = string.Empty;
		}

		/// <summary>ステータス</summary>
		[XmlElement("status")]
		public string Status;

		/// <summary>結果理由</summary>
		[XmlElement("resultReasons")]
		public string ResultReasons;
	}
	#endregion
}