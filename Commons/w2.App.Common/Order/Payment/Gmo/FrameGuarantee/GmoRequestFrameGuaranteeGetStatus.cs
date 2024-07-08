/*
=========================================================================================================
  Module      : Gmo Request Frame Guarantee Get Status(GmoRequestFrameGuaranteeGetStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.Domain.User;
using w2.Domain.UserBusinessOwner;

namespace w2.App.Common.Order.Payment.GMO.FrameGuarantee
{
	/// <summary>
	/// Gmoリクエスト枠保証ステータス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestFrameGuaranteeGetStatus : BaseGmoRequest
	{
		/// <summary>コンストラクタ</summary>
		public GmoRequestFrameGuaranteeGetStatus()
			: base()
		{
			this.TargetBuyer = new TargetBuyer();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="user">ユーザー</param>
		public GmoRequestFrameGuaranteeGetStatus(UserModel user)
			: base()
		{
			this.TargetBuyer = new TargetBuyer();
			this.TargetBuyer.ShopCustomerId = string.Format("{0}-F", user.UserId);
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userBusinessOwner">ビジネスオーナーのモデル/param>
		public GmoRequestFrameGuaranteeGetStatus(UserBusinessOwnerModel userBusinessOwner)
			: base()
		{
			this.TargetBuyer = new TargetBuyer();
			this.TargetBuyer.ShopCustomerId = userBusinessOwner.ShopCustomerId;
		}

		/// <summary>対象購入者</summary>
		[XmlElement("targetBuyer")]
		public TargetBuyer TargetBuyer;
	}

	#region TargetBuyer
	/// <summary>
	/// 対象購入者
	/// </summary>
	public class TargetBuyer
	{
		/// <summary>コンストラクタ</summary>
		public TargetBuyer()
		{
			this.ShopCustomerId = string.Empty;
		}
		/// <summary>ショップの顧客ID</summary>
		[XmlElement("shopCustomerId")]
		public string ShopCustomerId;
	}
	#endregion
}