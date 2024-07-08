/*
=========================================================================================================
  Module      : 頒布会デフォルト注文商品入力クラス (SubscriptionBoxDefaultItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Input;
using w2.Common.Extensions;
using w2.Domain.SubscriptionBox;

namespace Input.SubscriptionBox
{
	/// <summary>
	/// 頒布会デフォルト注文商品入力クラス
	/// </summary>
	/// <remarks>
	/// ユーザビリティのために管理画面の構成とDBは一致させていないため、Inputクラスは管理画面に寄せて作ってある。<br />
	/// 構造が違う部分はこのクラスのメソッドで相互に変換する。
	/// </remarks>
	[Serializable]
	public class SubscriptionBoxDefaultItemInput : InputBase<SubscriptionBoxDefaultItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SubscriptionBoxDefaultItemInput()
		{
			this.Items = new List<SubscriptionBoxDefaultSubItemInput>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public SubscriptionBoxDefaultItemInput(SubscriptionBoxDefaultItemModel model)
			: this()
		{
			this.SubscriptionBoxCourseId = model.SubscriptionBoxCourseId;
			this.TermSince = (model.TermSince != null)
				? model.TermSince.ToString()
				: null;
			this.TermUntil = (model.TermUntil != null)
				? model.TermUntil.ToString()
				: null;
			this.Count = (model.Count != null)
				? model.Count.ToString()
				: null;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデルから作成
		/// </summary>
		/// <param name="models">モデル配列</param>
		/// <param name="orderItemDeterminationType">注文商品決定方法</param>
		/// <returns>入力クラス</returns>
		public static SubscriptionBoxDefaultItemInput[] CreateFromModel(
			SubscriptionBoxDefaultItemModel[] models,
			string orderItemDeterminationType)
		{
			var branchNo = 0;
			var result = new List<SubscriptionBoxDefaultItemInput>(); 
			switch (orderItemDeterminationType)
			{
				case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME:
					foreach (var group in models.GroupBy(m => m.Count))
					{
						result.Add(
							new SubscriptionBoxDefaultItemInput
							{
								Count = group.Key.ToString(),
								TermSince = null,
								TermUntil = null,
								Items = group
									.Select(
										inGroup => new SubscriptionBoxDefaultSubItemInput
										{
											BranchNo = (++branchNo).ToString(),
											ShopId = inGroup.ShopId,
											ProductId = inGroup.ProductId,
											VariationId = inGroup.VariationId,
											ItemQuantity = inGroup.ItemQuantity.ToString(),
											NecessaryProductFlg = inGroup.NecessaryProductFlg
										})
									.ToList(),
							});
					}

					return result.ToArray();

				case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD:
					foreach (var group in models.GroupBy(m => new { Since = m.TermSince, Until = m.TermUntil }))
					{
						result.Add(
							new SubscriptionBoxDefaultItemInput
							{
								Count = "0",
								TermSince = group.Key.Since.Value.ToString(SubscriptionBoxModel.DATE_FORMAT),
								TermUntil = group.Key.Until.Value.ToString(SubscriptionBoxModel.DATE_FORMAT),
								Items = group
									.Select(
										inGroup => new SubscriptionBoxDefaultSubItemInput
										{
											BranchNo = (++branchNo).ToString(),
											ShopId = inGroup.ShopId,
											ProductId = inGroup.ProductId,
											VariationId = inGroup.VariationId,
											ItemQuantity = inGroup.ItemQuantity.ToString(),
											NecessaryProductFlg = inGroup.NecessaryProductFlg
										})
									.ToList(),
							});
					}

					return result.ToArray();

				default:
					throw new ArgumentException("orderItemDeterminationType");
			}
		}

		/// <summary>
		/// モデル作成
		/// </summary>
		/// <param name="inputs">入力クラス配列</param>
		/// <param name="orderItemDeterminationType">注文商品決定方法</param>
		/// <returns>モデル配列</returns>
		public static SubscriptionBoxDefaultItemModel[] CreateModel(
			SubscriptionBoxDefaultItemInput[] inputs,
			string orderItemDeterminationType)
		{
			var branchNo = 0;
			var result = new List<SubscriptionBoxDefaultItemModel>();
			switch (orderItemDeterminationType)
			{
				case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME:
					foreach (var input in inputs)
					{
						foreach (var inner in input.Items)
						{
							result.Add(
								new SubscriptionBoxDefaultItemModel
								{
									SubscriptionBoxCourseId = input.SubscriptionBoxCourseId,
									BranchNo = ++branchNo,
									Count = int.Parse(input.Count),
									ShopId = inner.ShopId,
									ProductId = inner.ProductId,
									VariationId = inner.VariationId,
									ItemQuantity = int.Parse(inner.ItemQuantity),
									TermSince = null,
									TermUntil = null,
									NecessaryProductFlg = inner.NecessaryProductFlg
								});
						}
					}

					return result.ToArray();

				case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD:
					foreach (var input in inputs)
					{
						foreach (var inner in input.Items)
						{
							result.Add(
								new SubscriptionBoxDefaultItemModel
								{
									SubscriptionBoxCourseId = input.SubscriptionBoxCourseId,
									BranchNo = ++branchNo,
									Count = null,
									ShopId = inner.ShopId,
									ProductId = inner.ProductId,
									VariationId = inner.VariationId,
									ItemQuantity = int.Parse(inner.ItemQuantity),
									TermSince = DateTime.Parse(input.TermSince),
									TermUntil = DateTime.Parse(input.TermUntil),
									NecessaryProductFlg = inner.NecessaryProductFlg
								});
						}
					}

					return result.ToArray();

				default:
					throw new ArgumentException("orderItemDeterminationType");
			}
		}

		/// <summary>
		/// 次の回数を取得
		/// </summary>
		/// <param name="inputs">入力クラス配列</param>
		/// <returns>次のcount</returns>
		public static string GetNextCountNumber(SubscriptionBoxDefaultItemInput[] inputs)
		{
			var counts = inputs
				.Select(
					i =>
					{
						int tmp;
						return int.TryParse(i.Count, out tmp) ? tmp : (int?)null;
					})
				.Where(i => i.HasValue)
				.Select(i => i.Value)
				.ToArray();

			var result = counts.Any() ? counts.Max() + 1 : 1;
			return result.ToString();
		}

		/// <summary>
		/// 次の枝番を取得
		/// </summary>
		/// <param name="inputs">Inputクラス配列</param>
		/// <returns>枝番</returns>
		public static string GetNextBranchNo(SubscriptionBoxDefaultItemInput[] inputs)
		{
			var branchNos = inputs
				.SelectMany(i => i.Items)
				.Select(
					i =>
					{
						int tmp;
						return int.TryParse(i.BranchNo, out tmp) ? tmp : (int?)null;
					})
				.Where(i => i.HasValue)
				.Select(i => i.Value)
				.ToArray();

			var result = branchNos.Any() ? branchNos.Max() + 1 : 1;
			return result.ToString();
		}

		/// <summary>
		/// モデル作成 ※利用不可
		/// </summary>
		/// <returns>モデル</returns>
		[Obsolete("利用不可メソッド", true)]
		public override SubscriptionBoxDefaultItemModel CreateModel()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="item">選択可能商品</param>
		/// <param name="shouldValidatePeriods">デフォルト注文商品の期間の検証をする</param>
		/// <returns>エラー</returns>
		public string Validate(
			List<SubscriptionBoxItemInput> item,
			bool shouldValidatePeriods)
		{
			var errorMessage = Enumerable.Empty<string>()
				.AppendElement(Validator.Validate("SubscriptionBoxDefaultItem", this.DataSource))
				.Concat(this.Items.Select(i => Validator.Validate("SubscriptionBoxDefaultItem", i.DataSource)))
				.JoinToString(Environment.NewLine);
			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			// 選択可能商品の期間が、回数/期間別デフォルト注文の期間内か
			foreach (var product in this.Items.Where(_ => shouldValidatePeriods))
			{
				var selectableProduct = item.FirstOrDefault(
					i => (i.ShopId == product.ShopId)
						&& (i.ProductId == product.ProductId)
						&& (i.VariationId == product.VariationId));
				if (selectableProduct == null) continue;

				DateTime tmp;
				var selectableSince = DateTime.TryParse(selectableProduct.SelectableSince, out tmp) ? tmp : DateTime.MinValue;
				var selectableUntil = DateTime.TryParse(selectableProduct.SelectableUntil, out tmp) ? tmp : DateTime.MaxValue;
				var termSince = DateTime.TryParse(this.TermSince, out tmp) ? tmp : DateTime.MinValue;
				var termUntil = DateTime.TryParse(this.TermUntil, out tmp) ? tmp : DateTime.MaxValue;

				var result = ((selectableSince <= termSince) && (selectableUntil >= termUntil));
				if (result == false)
				{
					errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_PRODUCT_DIFFERENT_PERIOD);
				}
			}
			return errorMessage;
		}

		/// <summary>
		/// カレンダーテキストボックス用 TermSince取得<br />
		/// 日付形式はISO8601準拠
		/// </summary>
		/// <returns>yyyy-MM-dd形式</returns>
		public string GetTermSinceForCalendarTextBox()
		{
			var result = ParseTermsInternal(this.TermSince);
			return result.HasValue ? result.Value.ToString(SubscriptionBoxModel.DATE_FORMAT) : "";
		}

		/// <summary>
		/// カレンダーテキストボックス用 TermUntil取得<br />
		/// 日付形式はISO8601準拠
		/// </summary>
		/// <returns>yyyy-MM-dd形式</returns>
		public string GetTermUntilForCalendarTextBox()
		{
			var result = ParseTermsInternal(this.TermUntil);
			return result.HasValue ? result.Value.ToString(SubscriptionBoxModel.DATE_FORMAT) : "";
		}

		/// <summary>
		/// パース内部処理
		/// </summary>
		/// <returns>DateTime</returns>
		private DateTime? ParseTermsInternal(string termValue)
		{
			DateTime tmp;
			var result = DateTime.TryParse(termValue, out tmp)
				? tmp
				: (DateTime?)null;
			return result;
		}

		#endregion

		#region プロパティ
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>回数</summary>
		public string Count
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_COUNT]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_COUNT] = value; }
		}
		/// <summary>期間開始（日付型）</summary>
		public DateTime? TermSinceDate
		{
			get
			{
				DateTime dateTmp;
				if (DateTime.TryParse(this.TermSince, out dateTmp) == false)
				{
					return null;
				}

				return dateTmp;
			}
		}
		/// <summary>期間開始</summary>
		public string TermSince
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_TERM_SINCE]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_TERM_SINCE] = value; }
		}
		/// <summary>期間終了（日付型）</summary>
		public DateTime? TermUntilDate
		{
			get
			{
				DateTime dateTmp;
				if (DateTime.TryParse(this.TermUntil, out dateTmp) == false)
				{
					return null;
				}

				return dateTmp;
			}
		}
		/// <summary>期間終了</summary>
		public string TermUntil
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_TERM_UNTIL]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_TERM_UNTIL] = value; }
		}
		/// <summary>サブアイテム</summary>
		public List<SubscriptionBoxDefaultSubItemInput> Items { get; set; }
		#endregion
	}

	/// <summary>
	/// デフォルト注文商品入力クラス
	/// </summary>
	[Serializable]
	public class SubscriptionBoxDefaultSubItemInput : InputBase<SubscriptionBoxDefaultItemModel>
	{
		/// <summary>
		/// モデル作成 ※利用不可
		/// </summary>
		[Obsolete("利用不可メソッド", true)]
		public override SubscriptionBoxDefaultItemModel CreateModel()
		{
			throw new NotSupportedException();
		}

		/// <summary>枝番</summary>
		public string BranchNo
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_BRANCH_NO] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_PRODUCT_ID] = value; }
		}
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_VARIATION_ID] = value; }
		}
		/// <summary>数量</summary>
		public string ItemQuantity
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY] = value; }
		}
		/// <summary>必須商品フラグ</summary>
		public string NecessaryProductFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_NECESSARY_PRODUCT_FLG]; }
			set { this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_NECESSARY_PRODUCT_FLG] = value; }
		}
		/// <summary>必須商品か</summary>
		public bool IsNecessary
		{
			get { return ((string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_NECESSARY_PRODUCT_FLG] == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID); }
		}
	}
}
