/*
=========================================================================================================
  Module      : Gmo Request Frame Guarantee Update(GmoRequestFrameGuaranteeUpdate.cs)
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
	/// GMOリクエスト枠保証更新
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestFrameGuaranteeUpdate : GmoRequestFrameGuaranteeRegister
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GmoRequestFrameGuaranteeUpdate()
			: base()
		{
			this.TargetBuyer = new TargetBuyer();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <param name="userBusinessOwner">ビジネスオーナーのモデル</param>
		public GmoRequestFrameGuaranteeUpdate(UserModel user, UserBusinessOwnerModel userBusinessOwner)
			: base(user, userBusinessOwner)
		{
			this.TargetBuyer = new TargetBuyer();
			this.TargetBuyer.ShopCustomerId = userBusinessOwner.ShopCustomerId;
		}
		/// <summary>対象購入者</summary>
		[XmlElement("targetBuyer")]
		public TargetBuyer TargetBuyer;
	}
}
