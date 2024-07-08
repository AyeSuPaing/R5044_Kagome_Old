/*
=========================================================================================================
  Module      : 明細情報要素 (DetailsElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using jp.veritrans.tercerog.mdk.dto;
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.Domain.Order;
using w2.Domain.SubscriptionBox;

namespace w2.App.Common.Order.Payment.Veritrans.ObjectElement
{
	/// <summary>
	/// 明細情報要素
	/// </summary>
	public class DetailsElement
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DetailsElement()
		{
			this.Details = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		public DetailsElement(OrderModel order)
		{
			if (order.IsSubscriptionBox
				&& (order.SubscriptionBoxFixedAmount != null))
			{
				var detailName = new SubscriptionBoxService().GetDisplayName(order.SubscriptionBoxCourseId);
				this.Details = new ScoreatpayDetailDto[] { new DetailElement(detailName, (decimal)order.SubscriptionBoxFixedAmount, 1) };
			}
			else
			{
				this.Details = order.Items
					.Select(item => new DetailElement(item.ProductName, item.ItemPrice, item.ItemQuantity))
					.Cast<ScoreatpayDetailDto>()
					.ToArray();
			}
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート情報</param>
		public DetailsElement(CartObject cart)
		{
			this.Details = (cart.IsSubscriptionBox && (cart.SubscriptionBoxFixedAmount != null))
				? new ScoreatpayDetailDto[] { new DetailElement(cart.SubscriptionBoxDisplayName, (decimal)cart.SubscriptionBoxFixedAmount, 1) }
				: cart.Items
					.Select(item => new DetailElement(item.ProductJointName, item.Price, item.Count))
					.Cast<ScoreatpayDetailDto>()
					.ToArray();
		}

		/// <summary>明細詳細情報要素</summary>
		public ScoreatpayDetailDto[] Details { get; set; }
	}

	#region 明細詳細情報要素
	/// <summary>
	/// 明細詳細情報要素
	/// </summary>
	public class DetailElement : ScoreatpayDetailDto
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DetailElement()
		{
			this.DetailName = string.Empty;
			this.DetailPrice = string.Empty;
			this.DetailQuantity = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="detailName">詳細名</param>
		/// <param name="detailPrice">単価</param>
		/// <param name="detailQuantity">注文数</param>
		public DetailElement(string detailName, decimal detailPrice, int detailQuantity)
			: this()
		{
			this.DetailName = detailName;
			this.DetailPrice = detailPrice.ToPriceString();
			this.DetailQuantity = detailQuantity.ToString();
		}
	}
	#endregion
}
