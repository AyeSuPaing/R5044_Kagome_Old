/*
=========================================================================================================
  Module      : 購入者情報要素 (BuyerElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using jp.veritrans.tercerog.mdk.dto;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Veritrans.ObjectElement
{
	/// <summary>
	/// 購入者情報要素
	/// </summary>
	public class BuyerElement : ScoreatpayContactDto
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public BuyerElement()
		{
			this.FullName = string.Empty;
			this.FullKanaName = string.Empty;
			this.Tel = string.Empty;
			this.Mobile = string.Empty;
			this.Email = string.Empty;
			this.MobileEmail = string.Empty;
			this.ZipCode = string.Empty;
			this.Address1 = string.Empty;
			this.Address2 = string.Empty;
			this.Address3 = string.Empty;
			this.CompanyName = string.Empty;
			this.DepartmentName = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート情報</param>
		public BuyerElement(CartObject cart)
			: this()
		{
			this.FullName = cart.Owner.Name;
			this.FullKanaName = StringUtility.ToZenkakuKatakana(cart.Owner.NameKana);
			this.Tel = cart.Owner.Tel1.Replace("-", string.Empty);
			this.Mobile = cart.Owner.Tel2.Replace("-", string.Empty);
			this.Email = StringUtility.ToEmpty(cart.Owner.MailAddr);
			this.MobileEmail = cart.Owner.MailAddr2;
			this.ZipCode = cart.Owner.Zip;
			this.Address1 = cart.Owner.Addr1;
			this.Address2 = cart.Owner.Addr2;
			this.Address3 = cart.Owner.Addr3.Length == 1
				? cart.Owner.Addr3 + "　"
				: cart.Owner.Addr3;
			this.CompanyName = StringUtility.ToEmpty(cart.Owner.CompanyName);
			this.DepartmentName = StringUtility.ToEmpty(cart.Owner.CompanyPostName);
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		public BuyerElement(OrderModel order)
			: this()
		{
			this.FullName = order.Owner.OwnerName;
			this.FullKanaName = StringUtility.ToZenkakuKatakana(order.Owner.OwnerNameKana);
			this.Tel = order.Owner.OwnerTel1.Replace("-", string.Empty);
			this.Mobile = order.Owner.OwnerTel2.Replace("-", string.Empty);
			this.Email = order.Owner.OwnerMailAddr;
			this.MobileEmail = order.Owner.OwnerMailAddr2;
			this.ZipCode = order.Owner.OwnerZip;
			this.Address1 = order.Owner.OwnerAddr1;
			this.Address2 = order.Owner.OwnerAddr2;
			this.Address3 = order.Owner.OwnerAddr3.Length == 1
				? order.Owner.OwnerAddr3 + "　"
				: order.Owner.OwnerAddr3;
			this.CompanyName = order.Owner.OwnerCompanyName;
			this.DepartmentName = order.Owner.OwnerCompanyPostName;
		}
	}
}
