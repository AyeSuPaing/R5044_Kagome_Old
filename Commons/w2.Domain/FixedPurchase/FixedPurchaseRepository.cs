/*
=========================================================================================================
  Module      : 定期購入情報リポジトリ (FixedPurchaseRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Extensions;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.FixedPurchase.Helper;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期購入情報リポジトリ
	/// </summary>
	public class FixedPurchaseRepository : RepositoryBase
	{
		/// <summary>キー名</summary>
		private const string XML_KEY_NAME = "FixedPurchase";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FixedPurchaseRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>モデル</returns>
		public FixedPurchaseModel Get(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;

			return new FixedPurchaseModel(dv[0]);
		}
		#endregion

		#region ~GetUpdLock 更新ロック取得
		/// <summary>
		/// 更新ロック取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>定期購入ID</returns>
		internal string GetUpdLock(string fixedPurchaseId)
		{
			var dv = Get(XML_KEY_NAME, "GetUpdLock", new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId }
			});
			return (string)dv[0][0];
		}
		#endregion

		#region +GetFixedPurchasesByUserId ユーザーIDから定期購入情報取得
		/// <summary>
		/// ユーザーIDから定期購入情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデルリスト</returns>
		public FixedPurchaseModel[] GetFixedPurchasesByUserId(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASE_USER_ID, userId}
			};
			var dv = Get(XML_KEY_NAME, "GetFixedPurchasesByUserId", ht);
			var models = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseModel(drv)).ToArray();

			return models;
		}
		#endregion

		#region +GetFixedPurchasesByProductId 商品IDから定期購入情報取得
		/// <summary>
		/// 商品IDから定期購入情報取得
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <returns>モデルリスト</returns>
		public FixedPurchaseModel[] GetFixedPurchasesByProductId(string productId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID, productId}
			};
			var dv = Get(XML_KEY_NAME, "GetFixedPurchasesByProductId", ht);
			var models = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseModel(drv)).ToArray();

			return models;
		}
		#endregion

		#region +GetTargetsForCreateOrder 注文対象の定期購入取得
		/// <summary>
		/// 注文対象の定期購入取得
		/// </summary>
		/// <returns>モデル列</returns>
		internal FixedPurchaseModel[] GetTargetsForCreateOrder()
		{
			var dv = Get(XML_KEY_NAME, "GetTargetsForCreateOrder");
			var models = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +GetOrdersForLine
		/// <summary>
		/// 定期台帳取得_LINE連携
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="offset">開始位置</param>
		/// <param name="limit">最大件数</param>
		/// <param name="updateAt">取得時間範囲</param>
		/// <returns>定期データリスト</returns>
		public FixedPurchaseModel[] GetFixedPurchasesForLine(
			string userId,
			int offset,
			int limit,
			DateTime updateAt)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_USER_ID, userId },
				{ Constants.FIELD_ORDER_DATE_CHANGED, updateAt },
				{ "offset", offset },
				{ "limit", limit },
			};
			var dataView = Get(XML_KEY_NAME, "GetFixedPurchasesForLine", input);
			var result = dataView.Cast<DataRowView>()
				.Select(row => new FixedPurchaseModel(row))
				.ToArray();
			return result;
		}
		#endregion

		#region +GetContainerWorking 稼働している定期情報すべて取得（日次出荷予測レポート用）
		/// <summary>
		/// 稼働している定期情報すべて取得（日次出荷予測レポート用）
		/// </summary>
		/// <returns>定期データリスト</returns>
		public DataView GetContainerWorking()
		{
			var dv = Get(XML_KEY_NAME, "GetContainerWorking");
			return dv;
		}
		#endregion

		#region 変更期限案内メール送信対象の定期購入取得(全件）
		/// <summary>
		/// 変更期限案内メール送信対象の定期購入取得(全件）
		/// </summary>
		/// <returns>モデル列</returns>
		internal FixedPurchaseModel[] GetTargetsForSendChangeDeadlineMail()
		{
			var dv = Get(XML_KEY_NAME, "GetTargetsForSendChangeDeadlineMail");
			var models = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region ~定期売上集計対象の定期取得
		/// <summary>
		/// 定期売上集計対象の定期取得
		/// </summary>
		/// <param name="lastExecDateTime">最終実行日</param>
		/// <returns>定期一覧</returns>
		internal FixedPurchaseModel[] GetTargetsForForecastAggregate(DateTime lastExecDateTime)
		{
			var ht = new Hashtable
			{
				{"last_exec_date", lastExecDateTime}
			};
			var dv = Get(XML_KEY_NAME, "GetTargetsForForecastAggregate", ht);
			var models = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region ~定期売上集計対象の定期取得(全件)
		/// <summary>
		/// 定期売上集計対象の定期取得
		/// </summary>
		/// <returns>定期一覧</returns>
		internal FixedPurchaseModel[] GetTargetsForForecastAggregate()
		{
			var dv = Get(XML_KEY_NAME, "GetTargetsForForecastAggregate");
			var models = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(FixedPurchaseModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(FixedPurchaseModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +GetShippingAll 配送先取得（全て）
		/// <summary>
		/// 配送先取得（全て）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>モデル列</returns>
		public FixedPurchaseShippingModel[] GetShippingAll(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_ID, fixedPurchaseId}
			};
			var dv = Get(XML_KEY_NAME, "GetShippingAll", ht);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseShippingModel(drv)).ToArray();
		}
		#endregion

		#region +InsertShipping 配送先登録
		/// <summary>
		/// 配送先登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertShipping(FixedPurchaseShippingModel model)
		{
			Exec(XML_KEY_NAME, "InsertShipping", model.DataSource);
		}
		#endregion

		#region +UpdateShipping 配送先更新
		/// <summary>
		/// 配送先更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateShipping(FixedPurchaseShippingModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateShipping", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteShipping 配送先削除
		/// <summary>
		/// 配送先削除
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteShipping(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ID, fixedPurchaseId}
			};
			var result = Exec(XML_KEY_NAME, "DeleteShipping", ht);
			return result;
		}
		#endregion

		#region +GetItemAll 商品取得（全て）
		/// <summary>
		/// 商品取得（全て）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>モデル列</returns>
		public FixedPurchaseItemModel[] GetItemAll(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ID, fixedPurchaseId}
			};
			var dv = Get(XML_KEY_NAME, "GetItemAll", ht);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseItemModel(drv)).ToArray();
		}
		#endregion

		#region +InsertItem 商品登録
		/// <summary>
		/// 商品登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertItem(FixedPurchaseItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertItem", model.DataSource);
		}
		#endregion

		#region +InsertItemForUpdate 商品登録（更新用）
		/// <summary>
		/// 商品登録（更新用）
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertItemForUpdate(FixedPurchaseItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertItemForUpdate", model.DataSource);
		}
		#endregion

		#region +UpdateItem 商品更新
		/// <summary>
		/// 商品更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateItem(FixedPurchaseItemModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateItem", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteItemsAll 商品削除
		/// <summary>
		/// 商品削除
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteItemsAll(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ID, fixedPurchaseId}
			};
			var result = Exec(XML_KEY_NAME, "DeleteItemsAll", ht);
			return result;
		}
		#endregion

		#region +GetHistoryCount 履歴件数取得
		/// <summary>
		/// 履歴件数取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>件数</returns>
		public int GetHistoryCount(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ID, fixedPurchaseId},
			};
			var dv = Get(XML_KEY_NAME, "GetHistoryCount", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region +InsertHistory 履歴登録
		/// <summary>
		/// 履歴登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int InsertHistory(FixedPurchaseHistoryModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertHistory", model.DataSource);
			return result;
		}
		#endregion

		#region +GetCancelReason 解約理由取得
		/// <summary>
		/// 解約理由取得
		/// </summary>
		/// <param name="cancelReasonId">解約理由区分ID</param>
		/// <returns>モデル</returns>
		public FixedPurchaseCancelReasonModel GetCancelReason(string cancelReasonId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_ID, cancelReasonId},
			};
			var dv = Get(XML_KEY_NAME, "GetCancelReason", ht);
			if (dv.Count == 0) return null;

			return new FixedPurchaseCancelReasonModel(dv[0]);
		}
		#endregion

		#region +GetCancelReasonAll 解約理由取得（全て）
		/// <summary>
		/// 解約理由取得（全て）
		/// </summary>
		/// <returns>モデル列</returns>
		public FixedPurchaseCancelReasonModel[] GetCancelReasonAll()
		{
			var dv = Get(XML_KEY_NAME, "GetCancelReasonAll");
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseCancelReasonModel(drv)).OrderBy(cr => cr.DisplayOrder).ToArray();
		}
		#endregion

		#region +GetUsedCancelReasonAll 解約理由取得（定期購入情報に利用されている全て）
		/// <summary>
		/// 解約理由取得（定期購入情報に利用されている全て）
		/// </summary>
		/// <returns>モデル列</returns>
		public FixedPurchaseCancelReasonModel[] GetUsedCancelReasonAll()
		{
			var dv = Get(XML_KEY_NAME, "GetUsedCancelReasonAll");
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseCancelReasonModel(drv)).OrderBy(cr => cr.DisplayOrder).ToArray();
		}
		#endregion

		#region +InsertCancelReason 解約理由登録
		/// <summary>
		/// 解約理由登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int InsertCancelReason(FixedPurchaseCancelReasonModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertCancelReason", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteCancelReasonAll 解約理由削除（全て）
		/// <summary>
		/// 解約理由削除（全て）
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		public int DeleteCancelReasonAll()
		{
			var result = Exec(XML_KEY_NAME, "DeleteCancelReasonAll");
			return result;
		}
		#endregion

		#region +GetContainer 取得（表示及びメール送信用）
		/// <summary>
		/// 取得（表示及びメール送信用）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="isSendMail">メール送信用か</param>
		/// <returns>モデル</returns>
		public FixedPurchaseContainer GetContainer(string fixedPurchaseId, bool isSendMail)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId},
			};
			var dv = (isSendMail) ? Get(XML_KEY_NAME, "GetContainerForSendMail", ht) : Get(XML_KEY_NAME, "GetContainer", ht);
			if (dv.Count == 0) return null;
			var model = new FixedPurchaseContainer(dv[0]);

			// 重複なしリスト作成
			var shippings = new List<FixedPurchaseShippingContainer>();
			var items = new List<FixedPurchaseItemContainer>();
			foreach (DataRowView drv in dv)
			{
				var fixedPurchaseIdTemp = (string)drv[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID];
				var fixedPurchaseShippingNo = (int)drv[Constants.FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_SHIPPING_NO];
				var fixedPurchaseItemNo = (int)drv[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ITEM_NO];
				var shippingType = (drv[Constants.FIELD_PRODUCTVARIATION_SHIPPING_TYPE] != DBNull.Value)
					? drv[Constants.FIELD_PRODUCTVARIATION_SHIPPING_TYPE].ToString()
					: string.Empty;

                model.SubscriptionBoxCourseId = (string)drv[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID];
				if (shippings.Exists(s =>
					(s.FixedPurchaseId == fixedPurchaseIdTemp)
					&& (s.FixedPurchaseShippingNo == fixedPurchaseShippingNo)) == false)
				{
					shippings.Add(new FixedPurchaseShippingContainer(drv));
				}
				if ((drv[Constants.FIELD_ORDERITEM_PRODUCT_NAME] != DBNull.Value)
					&& (items.Exists(i => (i.FixedPurchaseId == fixedPurchaseIdTemp)
						&& (i.FixedPurchaseShippingNo == fixedPurchaseShippingNo)
						&& (i.FixedPurchaseItemNo == fixedPurchaseItemNo)) == false))
				{
					items.Add(new FixedPurchaseItemContainer(drv));
				}
				// 商品は1つでも削除されたフラグの値を設定する
				else
				{
					model.HasDeletedAnyFixedPurchaseItem = true;
				}
				if (string.IsNullOrEmpty(model.ShippingType))
				{
					model.ShippingType = shippingType;
				}
			}
			// 配送先をセット
			model.Shippings = shippings.ToArray();
			// 商品をセット
			foreach (var shipping in model.Shippings)
			{
				shipping.Items =
					items.Where(i =>
						(i.FixedPurchaseId == shipping.FixedPurchaseId)
						&& (i.FixedPurchaseShippingNo == shipping.FixedPurchaseShippingNo)).ToArray();
			}

			return model;
		}
		#endregion

		#region +定期購入一覧検索関連
		/// <summary>
		/// 検索ヒット件数取得（定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <param name="replaces">クエリ置換内容</param>
		/// <returns>件数</returns>
		public int GetCountOfSearchFixedPurchase(FixedPurchaseListSearchCondition searchCondition, KeyValuePair<string, string>[] replaces = null)
		{
			var rep = replaces ?? new KeyValuePair<string, string>[] { };
			var dv = Get(XML_KEY_NAME, "GetCountOfSearchFixedPurchase", searchCondition.CreateHashtableParams(), replaces: rep);
			return (int)dv[0][0];
		}

		/// <summary>
		/// 検索（定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <param name="replaces">クエリ置換内容</param>
		/// <returns>モデル列</returns>
		public FixedPurchaseListSearchResult[] SearchFixedPurchase(FixedPurchaseListSearchCondition searchCondition, KeyValuePair<string, string>[] replaces = null)
		{
			var rep = replaces ?? new KeyValuePair<string, string>[] { };
			var dv = Get(XML_KEY_NAME, "SearchFixedPurchase", searchCondition.CreateHashtableParams(), replaces: rep);
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseListSearchResult(drv)).ToArray();
		}
		#endregion

		#region +定期購入履歴一覧検索関連
		/// <summary>
		/// 検索ヒット件数取得（定期購入履歴一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>件数</returns>
		public int GetCountOfSearchFixedPurchaseHistory(FixedPurchaseHistoryListSearchCondition searchCondition)
		{
			var dv = Get(XML_KEY_NAME, "GetCountOfSearchFixedPurchaseHistory", searchCondition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		/// <summary>
		/// 検索（定期購入履歴一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>モデル列</returns>
		public FixedPurchaseHistoryListSearchResult[] SearchFixedPurchaseHistory(FixedPurchaseHistoryListSearchCondition searchCondition)
		{
			var dv = Get(XML_KEY_NAME, "SearchFixedPurchaseHistory", searchCondition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseHistoryListSearchResult(drv)).ToArray();
		}
		#endregion

		#region +ユーザ定期購入一覧検索関連
		/// <summary>
		/// 検索ヒット件数取得（ユーザ定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>件数</returns>
		public int GetCountOfSearchUserFixedPurchase(UserFixedPurchaseListSearchCondition searchCondition)
		{
			var dv = Get(XML_KEY_NAME, "GetCountOfSearchUserFixedPurchase", searchCondition.CreateHashtableParams());
			return (int)dv[0][0];
		}

		/// <summary>
		/// 検索ヒット件数取得（ユーザ定期購入一覧）注文同梱でのキャンセル除く
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>件数</returns>
		public int GetCountOfSearchUserFixedPurchaseExcludeOrderCombineCancel(UserFixedPurchaseListSearchCondition searchCondition)
		{
			var replaces = new KeyValuePair<string, string>(
				"@@ where_fixedpurchase_status @@",
				CreateWhereOrStatementReplaces(
					Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS,
					ChangeFixedPurchaseStatus(searchCondition.FixedPurchaseStatusParameter)));
			var dv = Get(XML_KEY_NAME, "GetCountOfSearchUserFixedPurchaseExcludeOrderCombineCancel", searchCondition.CreateHashtableParams(), replaces:replaces);
			return (int)dv[0][0];
		}

		/// <summary>
		/// 検索（ユーザ定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>モデル列</returns>
		public UserFixedPurchaseListSearchResult[] SearchUserFixedPurchase(UserFixedPurchaseListSearchCondition searchCondition)
		{
			var result = SearchUserFixedPurchase(searchCondition, "SearchUserFixedPurchase");
			return result;
		}

		/// <summary>
		/// 検索（ユーザ定期購入一覧）注文同梱でのキャンセル除く
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>モデル列</returns>
		public UserFixedPurchaseListSearchResult[] SearchUserFixedPurchaseExcludeOrderCombineCancel(UserFixedPurchaseListSearchCondition searchCondition)
		{
			var replaces = new KeyValuePair<string, string>(
				"@@ where_fixedpurchase_status @@",
				CreateWhereOrStatementReplaces(
					Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS,
					ChangeFixedPurchaseStatus(searchCondition.FixedPurchaseStatusParameter)));
			var result = SearchUserFixedPurchase(searchCondition, "SearchUserFixedPurchaseExcludeOrderCombineCancel", replaces: replaces);
			return result;
		}

		/// <summary>
		/// 検索（ユーザ定期購入一覧）
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		/// <param name="statementName">ステートメント名</param>
		/// <param name="replaces">パラメータ</param>
		/// <returns>モデル列</returns>
		private UserFixedPurchaseListSearchResult[] SearchUserFixedPurchase(UserFixedPurchaseListSearchCondition searchCondition, string statementName , params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, statementName, searchCondition.CreateHashtableParams(), replaces: replaces);
			if (dv.Count == 0) return new UserFixedPurchaseListSearchResult[0];

			// 重複なしリスト作成
			var models = new List<UserFixedPurchaseListSearchResult>();
			var shippings = new List<UserFixedPurchaseShippingListSearchResult>();
			var items = new List<UserFixedPurchaseItemListSearchResult>();
			foreach (DataRowView drv in dv)
			{
				var fixedPurchaseId = (string)drv[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID];
				var fixedPurchaseShippingNo = (int)drv[Constants.FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_SHIPPING_NO];
				var fixedPurchaseItemNo = (int)drv[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ITEM_NO];

				if (models.Exists(m =>
					(m.FixedPurchaseId == fixedPurchaseId)) == false)
				{
					models.Add(new UserFixedPurchaseListSearchResult(drv));
				}
				if (shippings.Exists(s =>
					(s.FixedPurchaseId == fixedPurchaseId)
					&& (s.FixedPurchaseShippingNo == fixedPurchaseShippingNo)) == false)
				{
					shippings.Add(new UserFixedPurchaseShippingListSearchResult(drv));
				}
				if (items.Exists(i =>
					(i.FixedPurchaseId == fixedPurchaseId)
					&& (i.FixedPurchaseShippingNo == fixedPurchaseShippingNo)
					&& (i.FixedPurchaseItemNo == fixedPurchaseItemNo)) == false)
				{
					items.Add(new UserFixedPurchaseItemListSearchResult(drv));
				}
			}
			// 配送先、商品をセット
			foreach (var model in models)
			{
				// 配送先
				model.Shippings =
					shippings
					.Where(s => s.FixedPurchaseId == model.FixedPurchaseId)
					.OrderBy(s => s.FixedPurchaseShippingNo).ToArray();
				// 商品
				foreach (var shipping in model.Shippings)
				{
					shipping.Items =
						items
						.Where(i => (i.FixedPurchaseId == shipping.FixedPurchaseId) && (i.FixedPurchaseShippingNo == shipping.FixedPurchaseShippingNo))
						.OrderBy(i => i.FixedPurchaseItemNo).ToArray();
				}
			}

			return models.ToArray();
		}
		#endregion

		#region +GetFixedPurchaseHistoryListForMailTemplate 定期購入履歴取得(未出荷の受注情報を持つもののみ抽出)
		/// <summary>
		/// 定期購入履歴取得(未出荷の受注情報を持つもののみ抽出)
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>モデル列</returns>
		public FixedPurchaseHistoryModel[] GetFixedPurchaseHistoryListForMailTemplate(string fixedPurchaseId)
		{
			var fixedPurchaseHistoryList = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId},
			};
			var dv = Get(XML_KEY_NAME, "GetFixedPurchaseHistoryListForMailTemplate", fixedPurchaseHistoryList);

			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseHistoryModel(drv)).ToArray();
		}
		#endregion

		#region +GetSubcriptionBoxId
		/// <summary>
		/// 頒布会IDを取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>頒布会ID（取得できない場合は空文字を返す）</returns>
		public string GetSubcriptionBoxId(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId},
			};
			var dv = Get(XML_KEY_NAME, "GetSubcriptionBoxId", ht);
			var result = (dv.Count != 0)
				? (string)dv[0][Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID]
				: string.Empty;
			return result;
		}
		#endregion

		#region +SearchFixedPurchaseBatchMailTmpLogs 定期購入メール一時ログ検索
		/// <summary>
		/// 定期購入メール一時ログ検索
		/// </summary>
		/// <param name="actionMasterId">スケジュール実行ID</param>
		/// <returns>送信メール一覧</returns>
		public FixedPurchaseBatchMailTmpLogModel[] SearchFixedPurchaseBatchMailTmpLogs(string actionMasterId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_ACTION_MASTER_ID, actionMasterId}
			};
			var dv = Get(XML_KEY_NAME, "SearchFixedPurchaseBatchMailTmpLogs", ht);
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseBatchMailTmpLogModel(drv)).ToArray();
		}
		#endregion

		#region +InsertFixedPurchaseBatchMailTmpLog 定期購入メール一時ログ登録
		/// <summary>
		/// 定期購入メール一時ログ登録
		/// </summary>
		/// <param name="model">定期購入メール一時ログモデル</param>
		public void InsertFixedPurchaseBatchMailTmpLog(FixedPurchaseBatchMailTmpLogModel model)
		{
			Exec(XML_KEY_NAME, "InsertFixedPurchaseBatchMailTmpLog", model.DataSource);
		}
		#endregion

		#region +DeleteFixedPurchaseBatchMailTmpLog 定期購入メール一時ログ削除
		/// <summary>
		/// 定期購入メール一時ログ削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="tmpLogId">削除対象のtmp_log_id</param>
		public int DeleteFixedPurchaseBatchMailTmpLog(int tmpLogId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_TMP_LOG_ID, tmpLogId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteFixedPurchaseBatchMailTmpLog", ht);
			return result;
		}
		#endregion

		#region +GetCombinableParentFixedPurchaseWithCondition 定期購入同梱可能な親定期購入情報取得
		/// <summary>
		/// 定期購入同梱可能な親定期購入情報取得
		/// </summary>
		/// <param name="allowCombineFixedPurchaseStatus">定期購入同梱可能な定期購入ステータス</param>
		/// <param name="allowCombineFixedPurchasePaymentStatus">定期購入同梱可能な決済ステータス</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー氏名</param>
		/// <param name="nextShipDateFrom">次回配送日From</param>
		/// <param name="nextShipDateTo">次回配送日To</param>
		/// <param name="startRowNum">取得開始行番号</param>
		/// <param name="endRowNum">取得終了行番号</param>
		/// <returns>モデル</returns>
		internal FixedPurchaseModel[] GetCombinableParentFixedPurchaseWithCondition(
			string[] allowCombineFixedPurchaseStatus,
			string[] allowCombineFixedPurchasePaymentStatus,
			string userId,
			string userName,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo,
			int startRowNum,
			int endRowNum)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "GetCombinableParentFixedPurchaseWithCondition"))
			{
				ReplaceStatementForGetCombinableParentFp(statement, userId, userName, allowCombineFixedPurchaseStatus, allowCombineFixedPurchasePaymentStatus);

				var nextShipDateFromCondition = DateTime.Parse(nextShipDateFrom.ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT));
				var nextShipDateToCondition =
					DateTime.Parse(nextShipDateTo.AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT));

				var ht = new Hashtable
				{
					{"next_shipping_date_from", nextShipDateFromCondition},
					{"next_shipping_date_to", nextShipDateToCondition},
					{"start_row_num", startRowNum},
					{"end_row_num", endRowNum}
				};

				var dv = statement.SelectSingleStatementWithOC(this.Accessor, ht);
				var combinableFixedPurchases = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseModel(drv)).ToArray();

				return combinableFixedPurchases;
			}
		}
		#endregion

		#region +GetCombinableParentFixedPurchaseWithConditionCount 定期購入同梱可能な親定期購入件数取得
		/// <summary>
		/// 定期購入同梱可能な親定期購入件数取得
		/// </summary>
		/// <param name="allowCombineFixedPurchaseStatus">定期購入同梱可能な定期購入ステータス</param>
		/// <param name="allowCombineFixedPurchasePaymentStatus">定期購入同梱可能な決済ステータス</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー氏名</param>
		/// <param name="nextShipDateFrom">次回配送日From</param>
		/// <param name="nextShipDateTo">次回配送日To</param>
		/// <returns>件数</returns>
		internal int GetCombinableParentFixedPurchaseWithConditionCount(
			string[] allowCombineFixedPurchaseStatus,
			string[] allowCombineFixedPurchasePaymentStatus,
			string userId,
			string userName,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "GetCombinableParentFixedPurchaseWithConditionCount"))
			{
				ReplaceStatementForGetCombinableParentFp(statement, userId, userName, allowCombineFixedPurchaseStatus, allowCombineFixedPurchasePaymentStatus);

				var nextShipDateFromCondition = DateTime.Parse(nextShipDateFrom.ToString("yyyy/MM/dd 00:00:00"));
				var nextShipDateToCondition = DateTime.Parse(nextShipDateTo.ToString("yyyy/MM/dd 00:00:00"));

				var ht = new Hashtable
				{
					{"next_shipping_date_from", nextShipDateFromCondition},
					{"next_shipping_date_to", nextShipDateToCondition}
				};

				var dv = statement.SelectSingleStatementWithOC(this.Accessor, ht);
				return (int)dv[0][0];
			}
		}
		#endregion

		#region +GetCombinableFixedPurchase 定期購入同梱可能な定期購入情報取得
		/// <summary>
		/// 定期購入同梱可能な定期購入情報取得
		/// </summary>
		/// <param name="allowCombineFixedPurchaseStatus">定期購入同梱可能な定期購入ステータス</param>
		/// <param name="allowCombineFixedPurchasePaymentStatus">定期購入同梱可能な決済ステータス</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingType">配送種別</param>
		/// <param name="nextShipDateFrom">次回配送日From</param>
		/// <param name="nextShipDateTo">次回配送日To</param>
		/// <param name="parentPaymentKbn">親注文の支払区分</param>
		/// <returns>モデル</returns>
		internal FixedPurchaseModel[] GetCombinableFixedPurchase(
			string[] allowCombineFixedPurchaseStatus,
			string[] allowCombineFixedPurchasePaymentStatus,
			string userId,
			string shippingType,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo,
			string parentPaymentKbn)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "GetCombinableFixedPurchase"))
			{
				ReplaceStatementForGetCombinableFp(statement, allowCombineFixedPurchaseStatus, allowCombineFixedPurchasePaymentStatus);

				var nextShipDateFromCondition = DateTime.Parse(nextShipDateFrom.ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT));
				var nextShipDateToCondition =
					DateTime.Parse(nextShipDateTo.ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT)).AddSeconds(1);

				var ht = new Hashtable
				{
					{Constants.FIELD_FIXEDPURCHASE_USER_ID, userId},
					{Constants.FIELD_PRODUCT_SHIPPING_TYPE, shippingType},
					{"next_shipping_date_from", nextShipDateFromCondition},
					{"next_shipping_date_to", nextShipDateToCondition},
					{Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN, parentPaymentKbn}
				};

				var dv = statement.SelectSingleStatementWithOC(this.Accessor, ht);
				var combinableFixedPurchases = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseModel(drv)).ToArray();

				return combinableFixedPurchases;
			}
		}
		#endregion

		#region +GetCombinableFixedPurchaseCount 定期購入同梱可能な定期購入件数取得
		/// <summary>
		/// 定期購入同梱可能な定期購入件数取得
		/// </summary>
		/// <param name="allowCombineFixedPurchaseStatus">定期購入同梱可能な定期購入ステータス</param>
		/// <param name="allowCombinePaymentStatus">定期購入同梱可能な決済ステータス</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingType">配送種別</param>
		/// <param name="nextShipDateFrom">次回配送日From</param>
		/// <param name="nextShipDateTo">次回配送日To</param>
		/// <param name="parentPaymentKbn">親注文の支払区分</param>
		/// <returns>件数</returns>
		internal int GetCombinableFixedPurchaseCount(
			string[] allowCombineFixedPurchaseStatus,
			string[] allowCombinePaymentStatus,
			string userId,
			string shippingType,
			DateTime nextShipDateFrom,
			DateTime nextShipDateTo,
			string parentPaymentKbn)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "GetCombinableFixedPurchaseCount"))
			{
				ReplaceStatementForGetCombinableFp(statement, allowCombineFixedPurchaseStatus, allowCombinePaymentStatus);

				var nextShipDateFromCondition = DateTime.Parse(nextShipDateFrom.ToString("yyyy/MM/dd 00:00:00"));
				var nextShipDateToCondition = DateTime.Parse(nextShipDateTo.ToString("yyyy/MM/dd 00:00:00"));

				var ht = new Hashtable
				{
					{Constants.FIELD_FIXEDPURCHASE_USER_ID, userId},
					{Constants.FIELD_PRODUCT_SHIPPING_TYPE, shippingType},
					{"next_shipping_date_from", nextShipDateFromCondition},
					{"next_shipping_date_to", nextShipDateToCondition},
					{Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN, parentPaymentKbn}
				};

				var dv = statement.SelectSingleStatementWithOC(this.Accessor, ht);
				return (int)dv[0][0];
			}
		}
		#endregion

		#region +ReplaceStatementForGetCombinableParentFp 定期購入同梱可能な親定期購入取得用SQL条件分置換
		/// <summary>
		/// 定期購入同梱可能な親定期購入取得用SQL条件分置換
		/// </summary>
		/// <param name="statement">SQLステートメント</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー名</param>
		/// <param name="allowFpStatus">同梱を許可する定期購入ステータス</param>
		/// <param name="allowPaymentStatus">同梱を許可する定期購入決済ステータス</param>
		private void ReplaceStatementForGetCombinableParentFp(
			SqlStatement statement,
			string userId,
			string userName,
			string[] allowFpStatus,
			string[] allowPaymentStatus)
		{
			var userIdCondition = (string.IsNullOrEmpty(userId))
				? ""
				: string.Format("AND w2_FixedPurchase.user_id = '{0}'", userId.Replace("'", "''"));
			statement.ReplaceStatement("@@ user_id_condition @@", userIdCondition);

			var userNameCondition = (string.IsNullOrEmpty(userName))
				? ""
				: string.Format("AND w2_User.name like '%{0}%' ESCAPE '#'", StringUtility.SqlLikeStringSharpEscape(userName.Replace("'", "''")));
			statement.ReplaceStatement("@@ user_name_condition @@", userNameCondition);

			var allowFpStatusCondition = string.Join(",", allowFpStatus.Select(status => string.Format("'{0}'", status.Replace("'", "''"))));
			statement.ReplaceStatement("@@ allow_combine_fixedpurchase_status @@", allowFpStatusCondition);

			var allowPaymentStatusCondition = string.Join(",", allowPaymentStatus.Select(status => string.Format("'{0}'", status.Replace("'", "''"))));
			statement.ReplaceStatement("@@ allow_combine_fixedpurchase_payment_status @@", allowPaymentStatusCondition);
		}
		#endregion

		#region +ReplaceStatementForGetCombinableFp 定期購入同梱可能な定期購入取得用SQL条件分置換
		/// <summary>
		/// 定期購入同梱可能な定期購入取得用SQL条件分置換
		/// </summary>
		/// <param name="statement">SQLステートメント</param>
		/// <param name="allowFpStatus">同梱を許可する定期購入ステータス</param>
		/// <param name="allowFpPaymentStatus">同梱を許可する定期購入決済ステータス</param>
		private void ReplaceStatementForGetCombinableFp(SqlStatement statement, string[] allowFpStatus, string[] allowFpPaymentStatus)
		{
			var allowFpStatusCondition = string.Join(",", allowFpStatus.Select(status => string.Format("'{0}'", status.Replace("'", "''"))));
			statement.ReplaceStatement("@@ allow_combine_fixedpurchase_status @@", allowFpStatusCondition);

			var allowPaymentStatusCondition = string.Join(",", allowFpPaymentStatus.Select(status => string.Format("'{0}'", status.Replace("'", "''"))));
			statement.ReplaceStatement("@@ allow_combine_fixedpurchase_payment_status @@", allowPaymentStatusCondition);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion
		#region +DeleteHistory 更新履歴削除
		/// <summary>
		/// 更新履歴削除
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteHistory(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteHistory", ht);
			return result;
		}
		#endregion

		/// <summary>
		/// 注文IDを対象に履歴を削除する
		/// </summary>
		/// <param name="orderId">定期注文に紐づく注文ID</param>
		/// <returns>削除した件数</returns>
		public int DeleteHistoryByOrderId(string orderId)
		{
			var param = new Hashtable
			{
				{ "order_id", orderId }
			};
			var result = Exec(XML_KEY_NAME, "DeleteHistoryByOrderId", param);

			return result;
		}

		#region +GetCountOfDeliveryCompanyFixedPurchaseItems 配送会社、配送種別、配送方法に紐づく定期台帳の商品数取得
		/// <summary>
		/// 配送会社、配送種別、配送方法に紐づく定期台帳の商品数取得
		/// </summary>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="shippingMethod">配送方法</param>
		/// <returns>件数</returns>
		public int GetCountOfDeliveryCompanyFixedPurchaseItems(string deliveryCompanyId, string shippingId, string shippingMethod)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASESHIPPING_DELIVERY_COMPANY_ID, deliveryCompanyId},
				{Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, shippingId },
				{Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_METHOD, shippingMethod},
			};
			var dv = Get(XML_KEY_NAME, "GetCountOfDeliveryCompanyFixedPurchaseItems", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region +GetTargetsForResume 再開対象の定期購入取得
		/// <summary>
		/// 再開対象の定期購入取得
		/// </summary>
		/// <returns>定期購入情報</returns>
		public FixedPurchaseModel[] GetTargetsForResume()
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASE_RESUME_DATE, DateTime.Now.Date }
			};
			var dv = Get(XML_KEY_NAME, "GetTargetsForResume", ht);
			var models = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseModel(drv)).ToArray();

			return models;
		}
		#endregion

		#region +GetFixedPurchaseHistory 外部決済連携ログ取得(定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログの取得)
		/// <summary>
		/// 外部決済連携ログ取得(定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログの取得)
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>定期購入履歴のあるfixedPurchaseIdのあるfixedPurchaseHistoryNoの外部決済連携ログ</returns>
		public FixedPurchaseHistoryModel[] GetFixedPurchaseHistory(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_ID, fixedPurchaseId},
			};

			var dv = Get(XML_KEY_NAME, "GetFixedPurchaseHistory", ht);

			var fixedPurchaseHistories = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseHistoryModel(drv)).ToArray();
			return fixedPurchaseHistories;
		}
		#endregion

		#region +GetFixedPurchaseByFixedPurchaseIdAndCouponUseUser 定期購入IDとクーポン利用ユーザー(メールアドレスorユーザーID)から定期購入情報が取得
		/// <summary>
		/// 定期購入IDとクーポン利用ユーザー(メールアドレスorユーザーID)から定期購入情報が取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="usedUserJudgeType">利用済みユーザー判定方法</param>
		/// <returns>定期購入情報</returns>
		internal FixedPurchaseModel GetFixedPurchaseByFixedPurchaseIdAndCouponUseUser(
			string fixedPurchaseId,
			string couponUseUser,
			string usedUserJudgeType)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId },
				{ Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER, couponUseUser },
				{ Constants.FLG_COUPONUSEUSER_USED_USER_JUDGE_TYPE, usedUserJudgeType }
			};
			var dv = Get(XML_KEY_NAME, "GetFixedPurchaseByFixedPurchaseIdAndCouponUseUser", ht);
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseModel(drv)).FirstOrDefault();
		}
		#endregion

		#region +ClearSkippedCount
		/// <summary>
		/// Clear Skipped Count
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>影響を受けた件数</returns>
		public int ClearSkippedCount(string fixedPurchaseId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId },
			};

			return Exec(XML_KEY_NAME, "ClearSkippedCount", input);
		}
		#endregion

		#region +GetOrderCountByFixedPurchaseWorkflowSetting
		/// <summary>
		/// Get order count by fixed purchase workflow setting
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <returns>Order count</returns>
		internal int GetOrderCountByFixedPurchaseWorkflowSetting(Hashtable searchParam)
		{
			var replace = new[]
			{
				new KeyValuePair<string, string>(
					"@@ where @@",
					(string)searchParam["@@ where @@"])
			};
			searchParam.Remove("@@ where @@");

			var dv = Get(
				XML_KEY_NAME,
				"GetOrderCountByFixedPurchaseWorkflowSetting",
				searchParam,
				replaces: replace);
			return (int)dv[0][0];
		}
		#endregion

		#region +GetCountOrderFixedPurchase
		/// <summary>
		/// Get count order fixed purchase
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <returns>Fixed purchase order quantity</returns>
		public int GetCountOrderFixedPurchase(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEHISTORY_ORDER_ID, orderId}
			};
			var dv = Get(XML_KEY_NAME, "GetCountOrderFixedPurchase", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, statementName, input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, statementName, input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		public void CheckFieldsForGetMaster(Hashtable input, string masterKbn, params KeyValuePair<string, string>[] replaces)
		{
			var statement = string.Empty;

			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE: // 定期購入マスタ表示
					statement = "CheckFixedPurchaseFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM: // 定期購入商品マスタ表示
					statement = "CheckFixedPurchaseItemFields";
					break;
			}
			Get(XML_KEY_NAME, statement, input, replaces: replaces);
		}
		#endregion

		/// <summary>
		/// Create replace statement for search
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <returns>Replace statement</returns>
		private static KeyValuePair<string, string>[] CreateReplaceStatementForSearch(Hashtable searchParam)
		{
			var replace = new[]
			{
				new KeyValuePair<string, string>(
					"@@ where @@",
					(string)searchParam["@@ where @@"]),
				new KeyValuePair<string, string>("@@ order_extend_field_name @@",
					((string.IsNullOrEmpty(StringUtility.ToEmpty(searchParam[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME])) == false)
						? string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, StringUtility.ToEmpty(searchParam[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]))
						: string.Empty)),
			};
			searchParam.Remove("@@ where @@");
			return replace;
		}

		#region +UpdateSubScriptionBoxOrderCount
		/// <summary>
		/// Update SubScriptionBox Order Count
		/// </summary>
		/// <param name="fixedPurchaseId"> Fixed purchase id </param>
		/// <param name="subscriptionBoxOrderCount">Ordering number of times with SubScription Box</param>
		/// <returns>Number of updated rows</returns>
		public int UpdateSubScriptionBoxOrderCount(string fixedPurchaseId, int subscriptionBoxOrderCount)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId},
				{ Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_ORDER_COUNT, subscriptionBoxOrderCount},
			};
			return Exec(XML_KEY_NAME, "UpdateSubScriptionBoxOrderCount", input);
		}
		#endregion

		#region +GetFixedPurchaseWorkflowListNoPagination
		/// <summary>
		/// Get fixed purchase workflow list no pagination
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <returns>Dataview of fixed purchase list</returns>
		internal DataView GetFixedPurchaseWorkflowListNoPagination(Hashtable searchParam)
		{
			var replace = CreateReplaceStatementForSearch(searchParam);

			var result = Get(
				XML_KEY_NAME,
				"GetFixedPurchaseWorkflowListNoPagination",
				searchParam,
				replaces: replace);
			return result;
		}
		#endregion

		#region +GetFixedPurchaseWorkflowList
		/// <summary>
		/// Get fixed purchase workflow list
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <param name="pageNumber">Pager number</param>
		/// <returns>Dataview of fixed purchase list</returns>
		internal DataView GetFixedPurchaseWorkflowList(Hashtable searchParam, int pageNumber)
		{
			searchParam.Add(
				"bgn_row_num",
				(int)searchParam[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_COUNT] * (pageNumber - 1) + 1);
			searchParam.Add(
				"end_row_num",
				(int)searchParam[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_COUNT] * pageNumber);

			var replace = CreateReplaceStatementForSearch(searchParam);

			var dv = Get(
				XML_KEY_NAME,
				"GetFixedPurchaseWorkflowList",
				searchParam,
				replaces: replace);
			return dv;
		}
		#endregion

		/// <summary>
		/// 配列の値からOR句を生成する
		/// </summary>
		/// <param name="fieldName">対象フィールド名</param>
		/// <param name="selectableValue">対象値配列</param>
		/// <returns>配列の値に対応したOR句</returns>
		private string CreateWhereOrStatementReplaces(string fieldName, IEnumerable<string> selectableValue)
		{
			if (selectableValue.Any() == false) return string.Empty;
			var statement =
				$"AND ({selectableValue.Select(value => $"{fieldName} = '{value}'").JoinToString(" OR ")})";
			return statement;
		}

		/// <summary>
		/// 定期購入ステータスの配列作成
		/// </summary>
		/// <param name="status">取得パラメータ</param>
		/// <returns>定期購入ステータス配列</returns>
		private IEnumerable<string> ChangeFixedPurchaseStatus(string status)
		{
			var fixedpurchasestatus = new List<string>();
			if (string.IsNullOrEmpty(status)) return fixedpurchasestatus;
			foreach (var param in new List<string>(status.Split(',')))
			{
				switch (param)
				{
					case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_NORMAL:
						fixedpurchasestatus.Add(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL);
						break;
					case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_COMPLETE:
						fixedpurchasestatus.Add(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_COMPLETE);
						break;
					case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_FAILED:
						fixedpurchasestatus.Add(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_FAILED);
						fixedpurchasestatus.Add(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED);
						fixedpurchasestatus.Add(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_UNAVAILABLE_SHIPPING_AREA);
						break;
					case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_NOSTOCK:
						fixedpurchasestatus.Add(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NOSTOCK);
						break;
					case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_SUSPEND:
						fixedpurchasestatus.Add(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_SUSPEND);
						break;
					case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_CANCEL:
						fixedpurchasestatus.Add(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL);
						break;
				}
			}
			return fixedpurchasestatus;
		}
	}
}
