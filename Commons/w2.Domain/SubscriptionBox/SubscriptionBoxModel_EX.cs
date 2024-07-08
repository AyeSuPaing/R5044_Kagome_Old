/*
=========================================================================================================
  Module      : SubscriptionBox Model (SubscriptionBoxModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using System.Linq;

namespace w2.Domain.SubscriptionBox
{
	/// <summary>
	/// Extends of Subscription Box Model
	/// </summary>
	public partial class SubscriptionBoxModel
	{
		/// <summary>
		/// 指定回数のデフォルト商品取得
		/// </summary>
		/// <param name="displayCount">回数</param>
		/// <param name="termPeriod">期間判定基準日時</param>
		/// <returns>初回配送商品配列</returns>
		public SubscriptionBoxDefaultItemModel[] GetDefaultProducts(int displayCount = 1, DateTime? termPeriod = null)
		{
			if (termPeriod.HasValue == false)
			{
				termPeriod = DateTime.Now;
			}

			var firstProducts = this.IsNumberTime
				? this.DefaultOrderProducts
					.Where(defaultItem => (defaultItem.Count == displayCount)
						&& defaultItem.IsInSelectableTerm(termPeriod.Value))
					.ToArray()
				: this.DefaultOrderProducts
					.Where(defaultItem => (defaultItem.IsInTerm(termPeriod.Value)))
					.ToArray();
			return firstProducts;
		}

		/// <summary>
		/// デフォルト注文商品を取得
		/// </summary>
		/// <param name="criteria">基準日時</param>
		/// <param name="orderCount">注文回数</param>
		/// <returns>デフォルト注文商品配列</returns>
		public SubscriptionBoxDefaultItemModel[] GetDefaultOrderProduct(DateTime criteria, int orderCount)
		{
			var isTakeOrver = string.IsNullOrEmpty(this.DefaultOrderProducts.FirstOrDefault()?.ProductId) == false;

			if (isTakeOrver == false)
			{
				if (this.IsNumberTime)
				{
					return this.DefaultOrderProducts.Where(dop => dop.Count == orderCount).ToArray();
				}

				if (this.IsDeterminationTypePeriod)
				{
					return this.DefaultOrderProducts.OrderBy(a => a.TermUntil).Where(dop => dop.IsInTermUntil(criteria)).ToArray();
				}
			}
			else
			{
				var result = this.DefaultOrderProducts
					.Where(defaultProduct => string.IsNullOrEmpty(defaultProduct.ProductId) == false)
					.OrderByDescending(defaultProduct => defaultProduct.Count)
					.ToArray();

				return result;
			}
			throw new Exception("商品決定方法の特定に失敗しました。コースID=" + this.CourseId);
		}

		/// <summary>自動更新設定が有効か</summary>
		public bool IsAutoRenewal
		{
			get { return (this.AutoRenewal == Constants.FLG_SUBSCRIPTIONBOX_AUTO_RENEWAL_TRUE); }
			set
			{
				this.AutoRenewal = value
					? Constants.FLG_SUBSCRIPTIONBOX_AUTO_RENEWAL_TRUE
					: Constants.FLG_SUBSCRIPTIONBOX_AUTO_RENEWAL_FALSE;
			}
		}
		/// <summary>Selectable Products</summary>
		public SubscriptionBoxItemModel[] SelectableProducts { get; set; }
		/// <summary>Default Order Products</summary>
		public SubscriptionBoxDefaultItemModel[] DefaultOrderProducts { get; set; }
		/// <summary>回数指定かどうか</summary>
		public bool IsNumberTime
		{
			get
			{
				return (this.OrderItemDeterminationType == Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME);
			}
		}
		/// <summary>商品決定方法が日付基準</summary>
		public bool IsDeterminationTypePeriod
		{
			get { return this.OrderItemDeterminationType == Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD; }
		}
		/// <summary>有効かどうか</summary>
		public bool IsValid
		{
			get
			{
				return (this.ValidFlg == Constants.FLG_SUBSCRIPTIONBOX_VALID_TRUE);
			}
		}
		/// <summary>マイページでの商品変更可能か</summary>
		public bool IsItemsChangeableByUser
		{
			get
			{
				return (this.ItemsChangeableByUser == Constants.FLG_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER_TRUE);
			}
		}
		/// <summary>頒布会初回選択画面が有効か</summary>
		public bool CanFirstSelectable
		{
			get { return this.FirstSelectableFlg == Constants.FLG_SUBSCRIPTIONBOX_FIRST_SELECTABLE_PAGE_TRUE; }
		}
		/// <summary>最終商品の無期限繰り返しか</summary>
		public bool IsIndefinitePeriod
		{
			get { return this.IndefinitePeriod == Constants.FLG_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_TRUE; }
			set
			{
				this.IndefinitePeriod = value
					? Constants.FLG_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_TRUE
					: Constants.FLG_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_FALSE;
			}
		}
	}
}
