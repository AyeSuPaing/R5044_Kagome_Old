/*
=========================================================================================================
  Module      : SubscriptionBox Repository (SubscriptionBoxRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.SubscriptionBox
{
	/// <summary>
	/// Subscription Box Repository
	/// </summary>
	internal class SubscriptionBoxRepository : RepositoryBase
	{
		/// <returns>SubscriptionBox xml file name</returns>
		private const string XML_KEY_NAME = "SubscriptionBox";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal SubscriptionBoxRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SubscriptionBoxRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		/// <summary>
		/// １回目に登録されているデフォルト商品を取得
		/// </summary>
		/// <param name="courseId">コースID</param>
		/// <returns>１回目に登録されているデフォルト商品</returns>
		internal SubscriptionBoxDefaultItemModel[] GetFirstDefaultItem(string courseId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID, courseId }
			};
			var dv = Get(XML_KEY_NAME, "GetFirstDefaultItem", ht) ;
			return dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxDefaultItemModel(drv)).ToArray();
		}

		/// <summary>
		/// デフォルト商品詳細情報を取得（日別予測レポートバッチ用）
		/// </summary>
		/// <param name="courseId">コースID</param>
		/// <returns>１回目に登録されているデフォルト商品</returns>
		internal SubscriptionBoxDefaultItemModel[] GetDefaultItemDetails(string courseId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID, courseId }
			};
			var dv = Get(XML_KEY_NAME, "GetDefaultItemDetails", ht);
			return dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxDefaultItemModel(drv)).ToArray();
		}

		/// <summary>
		/// 頒布会コースID重複チェック
		/// </summary>
		/// <returns>件数</returns>
		internal bool CheckDuplicationCourseId(string courseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID, courseId}
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			return (dv.Count > 0);
		}

		/// <summary>
		/// Get all
		/// </summary>
		/// <returns>List SubscriptionBox Model</returns>
		internal SubscriptionBoxModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll", new Hashtable());
			return dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxModel(drv)).ToArray();
		}

		/// <summary>
		/// 有効な頒布会を全件取得
		/// </summary>
		/// <returns>頒布会</returns>
		internal SubscriptionBoxModel[] GetValidAll()
		{
			var dv = Get(XML_KEY_NAME, "GetValidAll");
			return dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxModel(drv)).ToArray();
		}

		/// <summary>
		/// 選択可能商品を全件取得
		/// </summary>
		/// <returns>頒布会選択可能商品</returns>
		internal SubscriptionBoxItemModel[] GetAllValidItems()
		{
			var dv = Get(XML_KEY_NAME, "GetAllValidItems");
			return dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxItemModel(drv)).ToArray();
		}

		/// <summary>
		/// 頒布会デフォルト注文商品を全件取得
		/// </summary>
		/// <returns>頒布会選択可能商品</returns>
		internal SubscriptionBoxDefaultItemModel[] GetAllValidDefaultItems()
		{
			var dv = Get(XML_KEY_NAME, "GetAllValidDefaultItems");
			return dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxDefaultItemModel(drv)).ToArray();
		}

		/// <summary>
		/// Get by id
		/// </summary>
		/// <param name="subscriptionBoxCourseId">Subscription Box Course Id </param>
		/// <returns>SubscriptionBox Model</returns>
		internal SubscriptionBoxModel Get(string subscriptionBoxCourseId)
		{
			var ds = GetWithChilds(
				new[]
				{
					new KeyValuePair<string ,string>(XML_KEY_NAME, "Get"),
					new KeyValuePair<string ,string>(XML_KEY_NAME, "GetItems"),
					new KeyValuePair<string ,string>(XML_KEY_NAME, "GetDefaultItems"),
				},
				new Hashtable
				{
					{ Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID, subscriptionBoxCourseId },
				});

			var subscriptionBox = (ds.Tables[0].DefaultView.Count != 0) ? new SubscriptionBoxModel(ds.Tables[0].DefaultView[0]) : null;
			if (subscriptionBox == null) return null;

			subscriptionBox.SelectableProducts = ds.Tables[1].DefaultView.Cast<DataRowView>().Select(drv => new SubscriptionBoxItemModel(drv)).ToArray();
			subscriptionBox.DefaultOrderProducts = ds.Tables[2].DefaultView.Cast<DataRowView>().Select(drv => new SubscriptionBoxDefaultItemModel(drv)).ToArray();
			return subscriptionBox;
		}

		#region ~GetByTaxCategoryId 商品税率カテゴリと紐づけられた頒布会を取得
		/// <summary>
		/// 商品税率カテゴリと紐づけられた頒布会を取得
		/// </summary>
		/// <param name="taxCategoryId">商品税率カテゴリID</param>
		/// <returns>頒布会情報</returns>
		internal SubscriptionBoxModel GetByTaxCategoryId(string taxCategoryId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SUBSCRIPTIONBOX_TAX_CATEGORY_ID, taxCategoryId },
			};
			var dv = Get(XML_KEY_NAME, "GetByTaxCategoryId", ht);
			var subscriptionBox = (dv.Count != 0) ? new SubscriptionBoxModel(dv[0]) : null;
			return subscriptionBox;
		}
		#endregion

		#region ~GetCourseList
		/// <summary>
		/// 頒布会コースリストを取得
		/// </summary>
		/// <param name="displayQuantity">表示件数</param>
		/// <returns>表示件数コース</returns>
		internal SubscriptionBoxModel[] GetCourseList(int displayQuantity)
		{
			var ht = new Hashtable
			{
				{ "display_quantity", displayQuantity },
			};
			var dv = Get(XML_KEY_NAME, "GetCourseList", ht);
			var result = dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxModel(drv)).ToArray();
			return result;
		}
		#endregion

		/// <summary>
		/// Get Valid Subscription Box
		/// </summary>
		/// <returns></returns>
		internal SubscriptionBoxModel[] GetValidSubscriptionBox()
		{
			var dv = Get(XML_KEY_NAME, "GetValidSubscriptionBox", new Hashtable());
			return dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxModel(drv)).ToArray();
		}

		/// <summary>
		/// Update SubscriptionBox
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		internal int Update(SubscriptionBoxModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}

		/// <summary>
		/// Delete SubscriptionBox by course id
		/// </summary>
		/// <param name="subscriptionBoxCourseId"> Subscription Box Course Id </param>
		/// <returns></returns>
		internal int Delete(string subscriptionBoxCourseId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID, subscriptionBoxCourseId },
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}

		/// <summary>
		/// Delete SubscriptionBox by course id
		/// </summary>
		/// <param name="subscriptionBoxCourseId">Subscription Box Course Id</param>
		/// <returns></returns>
		internal int DeleteItems(string subscriptionBoxCourseId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID, subscriptionBoxCourseId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteItems", ht);
			return result;
		}

		/// <summary>
		/// Delete SubscriptionBox by course id
		/// </summary>
		/// <param name="subscriptionBoxCourseId">Subscription Box Course Id</param>
		/// <returns></returns>
		internal int DeleteDefaultItems(string subscriptionBoxCourseId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID, subscriptionBoxCourseId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteDefaultItems", ht);
			return result;
		}

		/// <summary>
		/// Insert new SubscriptionBox
		/// </summary>
		/// <param name="model"></param>
		internal void Insert(SubscriptionBoxModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}

		/// <summary>
		/// Insert new SubscriptionBox Items
		/// </summary>
		/// <param name="model"></param>
		internal void Insert(SubscriptionBoxItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertItems", model.DataSource);
		}

		/// <summary>
		/// Insert new SubscriptionBox Default Items
		/// </summary>
		/// <param name="model"></param>
		internal void Insert(SubscriptionBoxDefaultItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertDefaultItems", model.DataSource);
		}

		/// <summary>
		/// Get count
		/// </summary>
		/// <returns>Count SubscriptionBox</returns>
		public int? GetCount(Hashtable htSqlParam)
		{
			var dv = Get(XML_KEY_NAME, "GetSubscriptionBoxCount", htSqlParam);
			var data = (int)dv[0]["row_count"];
			return data;
		}

		/// <summary>
		/// 頒布会検索（DataView）
		/// </summary>
		/// <param name="htSqlParam">検索パラメタ</param>
		/// <returns>検索結果</returns>
		internal DataView SearchSubscriptionBoxesAtDataView(Hashtable htSqlParam)
		{
			var dv = Get(XML_KEY_NAME, "GetSubscriptionBoxSearch", htSqlParam);
			return dv;
		}

		/// <summary>
		/// 頒布会注文（あるいは頒布会定期）チェック
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>True：頒布会注文（あるいは頒布会定期）、False：頒布会注文（あるいは頒布会定期）以外</returns>
		public bool IsSubscriptionBoxOrderOrFixedPurchase(Hashtable condition)
		{
			var dv = Get(XML_KEY_NAME, "IsSubscriptionBoxOrderOrFixedPurchase", condition);
			var result = (string.IsNullOrEmpty((string)dv[0][0]) == false);
			return result;
		}

		/// <summary>
		/// 注文または定期台帳の頒布会コースIDを取得
		/// </summary>
		/// <returns>頒布会コースID</returns>
		public string GetSubscriptionBoxCourseIdOfOrderOrFixedPurchase(Hashtable htSqlParam)
		{
			var dv = Get(XML_KEY_NAME, "GetSubscriptionBoxCourseIdOfOrderOrFixedPurchase", htSqlParam);
			var data = (string)dv[0][Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID];
			return data;
		}

		/// <summary>
		/// Get Product Belong To subscription box IDs
		/// </summary>
		/// <param name="productId"></param>
		/// <param name="variationId"></param>
		/// <param name="shopId"></param>
		/// <returns></returns>
		public SubscriptionBoxModel[] GetProductBelongToHanpukaiIDs(
			string shopId,
			string productId,
			string variationId)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetProductBelongToHanpukaiIDs",
				new Hashtable
				{
					{ Constants.FIELD_SUBSCRIPTIONBOXITEM_SHOP_ID, shopId },
					{ Constants.FIELD_SUBSCRIPTIONBOXITEM_PRODUCT_ID, productId },
					{ Constants.FIELD_SUBSCRIPTIONBOXITEM_VARIATION_ID, variationId },
				});

			return dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxModel(drv)).ToArray();
		}

		/// <summary>
		/// Get products By HanpukaiID and Now date
		/// </summary>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public SubscriptionBoxDefaultItemModel[] GetProductsByHanpukaiIDAndNowDate(string courseId)
		{
			var input = new Hashtable 
			{
				{ Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID, courseId }
			};
			var dv = Get(XML_KEY_NAME, "GetProductsByHanpukaiIDAndNowDate", input);
			return dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxDefaultItemModel(drv)).ToArray();
		}

		/// <summary>
		/// GetNameDisplay
		/// </summary>
		/// <returns>Get Name Display of SubscriptionBox</returns>
		public string GetNameDisplay(string subscriptionBoxCourseId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID, subscriptionBoxCourseId }
			};
			var dv = Get(XML_KEY_NAME, "GetNameDisplay", ht);
			var data = (string)dv[0][Constants.FIELD_SUBSCRIPTIONBOX_DISPLAY_NAME];
			return data;
		}

		/// <summary>
		/// Get subscription box item available
		/// </summary>
		/// <returns>SubscriptionBoxCourseId</returns>
		public SubscriptionBoxItemModel[] GetSubscriptionBoxItemAvailable(string SubscriptionBoxCourseId, string date)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID, SubscriptionBoxCourseId },
				{ "date", date }
			};
			var dv = Get(XML_KEY_NAME, "GetItemsAvailable", ht);
			var data = dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxItemModel(drv)).ToArray();
			return data;
		}

		/// <summary>
		/// Get Number Of Display
		/// </summary>
		/// <returns>Count Number Display</returns>
		public int? GetNumberOfDisplay(Hashtable htSqlParam)
		{
			var dv = Get(XML_KEY_NAME, "GetNumberOfDisplay", htSqlParam);
			var data = (int)dv[0]["numberDisplay"];
			return data;
		}

		/// <summary>
		/// Get List Product
		/// </summary>
		/// <param name="htSqlParam">SQLパラメタ</param>
		/// <returns>結果</returns>
		internal DataView GetListProduct(Hashtable htSqlParam)
		{
			var dv = Get(XML_KEY_NAME, "GetListProduct", htSqlParam);
			return dv;
		}

		/// <summary>
		/// GetNextDate
		/// </summary>
		/// <returns>Return next date delivery</returns>
		public DateTime? GetNextDate(Hashtable htSqlParam)
		{
			var dv = Get(XML_KEY_NAME, "GetNextDate", htSqlParam);
			var data = (DateTime?)dv[0]["next_date"];
			return data;
		}

		#region ~GetSubscriptionBoxItemList
		/// <summary>
		/// 頒布会商品データー一覧取得
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="date">次回配送日付</param>
		/// <returns>散布会商品データー一覧</returns>
		public SubscriptionBoxItemModel[] GetSubscriptionBoxItemList(string shopId, string subscriptionBoxCourseId, DateTime date)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SUBSCRIPTIONBOXITEM_SHOP_ID, shopId },
				{ Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID, subscriptionBoxCourseId },
				{ "date", date }
			};
			var dv = Get(XML_KEY_NAME, "GetSubscriptionBoxItemList", ht);
			var data = dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxItemModel(drv)).ToArray();
			return data;
		}
		#endregion

		#region ~GetDisplayItems
		/// <summary>
		/// 頒布会詳細での表示内容取得
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <returns>表示内容</returns>
		internal SubscriptionBoxDefaultItemModel[] GetDisplayItems(string subscriptionBoxCourseId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SUBSCRIPTION_BOX_COURSE_ID, subscriptionBoxCourseId }
			};
			var dv = Get(XML_KEY_NAME, "GetDisplayItems", ht);
			if (dv.Count == 0) return null;
			var model = dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxDefaultItemModel(drv)).ToArray();
			return model;
		}
		#endregion

		#region GetValidSubscriptionBoxByVariationId
		/// <summary>
		/// バリエーションIDに紐づいた有効な頒布会コースを取得
		/// </summary>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>有効な頒布会コース</returns>
		public SubscriptionBoxModel[] GetValidSubscriptionBoxByVariationId(string variationId)
		{
			var input = new Hashtable()
			{
				{ Constants.FIELD_SUBSCRIPTIONBOXITEM_VARIATION_ID, variationId }
			};
			var dv = Get(XML_KEY_NAME, "GetValidSubscriptionBoxByVariationId", input);
			var result = dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxModel(drv)).ToArray();
			return result;
		}
		#endregion

		#region GetSubscriptionItemByProductId
		/// <summary>
		/// 商品IDに紐づく頒布会コースを取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>頒布会コースID</returns>
		public SubscriptionBoxItemModel[] GetSubscriptionItemByProductId(string shopId, string productId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SUBSCRIPTIONBOXITEM_SHOP_ID, shopId },
				{ Constants.FIELD_SUBSCRIPTIONBOXITEM_PRODUCT_ID, productId }
			};
			var dv = Get(XML_KEY_NAME, "GetSubscriptionBoxByProductId", input);
			var result = dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxItemModel(drv)).ToArray();
			return result;
		}
		#endregion

		#region GetSubscriptionItemByProductVariationId
		/// <summary>
		/// 商品バリエーションIDに紐づく頒布会コースを取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>頒布会コースID</returns>
		public SubscriptionBoxItemModel[] GetSubscriptionItemByProductVariationId(string shopId, string productId, string variationId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SUBSCRIPTIONBOXITEM_SHOP_ID, shopId },
				{ Constants.FIELD_SUBSCRIPTIONBOXITEM_PRODUCT_ID, productId },
				{ Constants.FIELD_SUBSCRIPTIONBOXITEM_VARIATION_ID, variationId }
			};
			var dv = Get(XML_KEY_NAME, "GetSubscriptionBoxByProductVariationId", input);
			var result = dv.Cast<DataRowView>().Select(drv => new SubscriptionBoxItemModel(drv)).ToArray();
			return result;
		}
		#endregion

		/// <summary>
		/// 頒布会選択可能商品を取得<br />
		/// （商品情報等も含む）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="subscriptionBoxCourseId">コースID</param>
		/// <returns>頒布会選択可能商品コンテナ配列</returns>
		public SubscriptionBoxItemContainerList GetSubscriptionItemsWithProductInfo(
			string fixedPurchaseId,
			string subscriptionBoxCourseId)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetSubscriptionItemsWithProductInfo",
				new Hashtable
				{
					{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId },
					{ Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID, subscriptionBoxCourseId }
				});

			var result = new SubscriptionBoxItemContainerList(
				dv.Cast<DataRowView>()
					.Select(drv => new SubscriptionBoxItemContainer(drv))
					.Where(i => i.IsInTerm(DateTime.Now))
					.ToArray());
			return result;
		}

		/// <summary>
		/// 頒布会初回選択可能商品取得
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <returns>頒布会初回選択商品</returns>
		internal DataView GetFirstSelectableItems(string subscriptionBoxCourseId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID, subscriptionBoxCourseId},
			};

			var dv = Get(XML_KEY_NAME, "GetFirstSelectableItems", ht);
			return dv;
		}
	}
}
