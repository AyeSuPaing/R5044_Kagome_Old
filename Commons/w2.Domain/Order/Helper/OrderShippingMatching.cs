/*
=========================================================================================================
  Module      : 配送先マッチングクラス(OrderShippingMatching.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.Common.Util;
using w2.Domain.Helper;
using w2.Domain.Order.Setting;

namespace w2.Domain.Order.Helper
{
	/// <summary>
	/// 配送先マッチングクラス
	/// </summary>
	public class OrderShippingMatching : BaseMatching
	{
		/// <summary>クエリに埋め込むIndex番号</summary>
		int m_queryIndex = 0;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderShippingMatching()
		{
			this.Setting = OrderShippingMatchingSetting.Setting;
			this.QueryValues = new List<string>();
		}

		/// <summary>
		/// 配送先情報が一致しているか
		/// </summary>
		/// <param name="targetShipping">対象配送先</param>
		/// <param name="shipping">比較対象先</param>
		/// <returns>配送先情報が一致しているか</returns>
		public bool MatchOrderShipping(OrderShippingModel targetShipping, OrderShippingModel shipping)
		{
			var result = MatchUser(
				new ShippingData(targetShipping, this.Setting.MatchCondition.GetUseKeys().ToList()),
				new ShippingData(shipping, this.Setting.MatchCondition.GetUseKeys().ToList()),
				this.Setting.MatchCondition.LogicalOperators);
			return result;
		}

		/// <summary>
		/// 配送先条件置換
		/// </summary>
		/// <param name="orderShipping">条件となる配送先情報</param>
		/// <param name="order">条件となる注文</param>
		/// <param name="orderOwner">条件となる注文者</param>
		/// <returns>条件文字列</returns>
		public string ReplaceConditionOrderShipping(
			OrderShippingModel orderShipping,
			OrderModel order,
			OrderOwnerModel orderOwner)
		{
			this.Order = order;
			this.OrderOwner = orderOwner;
			this.OrderShipping = orderShipping;

			var result = LogicalOperatorsAnalysis(this.Setting.MatchCondition.LogicalOperators);
			return result;
		}

		/// <summary>
		/// 論理演算子解析
		/// </summary>
		/// <param name="logicalOparators">条件情報</param>
		/// <returns>条件文字列</returns>
		private string LogicalOperatorsAnalysis(LogicalOperatorsData logicalOparators)
		{
			var matchQuerys =
				logicalOparators.MatchDatas.Select(CreateQuery).Concat(
					logicalOparators.LogicalOperatorsDatas.Select(
						operation => string.Format("( {0} )", LogicalOperatorsAnalysis(operation))));
			var result = string.Join(GetOperatorKbnString(logicalOparators.Operator), matchQuerys);
			return result;
		}

		/// <summary>
		/// クエリ作成
		/// </summary>
		/// <param name="match">条件情報</param>
		/// <returns>クエリ条件部分</returns>
		private string CreateQuery(MatchData match)
		{
			switch (this.Setting.ConditionTableName)
			{
				case "w2_Order":
					this.QueryValues.Add(StringUtility.ToEmpty(this.Order.DataSource[match.Key]));
					break;

				case "w2_OrderOwner":
					this.QueryValues.Add(StringUtility.ToEmpty(this.OrderOwner.DataSource[match.Key]));
					break;

				case "w2_OrderShipping":
					this.QueryValues.Add(StringUtility.ToEmpty(this.OrderShipping.DataSource[match.Key]));
					break;
			}
			
			var result = string.Format(
				GetQryFormat(match),
				this.Setting.ConditionTableName,
				match.Key,
				m_queryIndex++);
			return result;
		}

		/// <summary>
		/// 条件区分文字列取得
		/// </summary>
		/// <param name="kbn">条件区分</param>
		/// <returns>ANDまたはOR</returns>
		private string GetOperatorKbnString(OperatorKbn kbn)
		{
			switch (kbn)
			{
				case OperatorKbn.And:
					return " AND ";

				case OperatorKbn.Or:
					return " OR ";
			}
			return "";
		}

		/// <summary>
		/// クエリのフォーマット取得
		/// </summary>
		/// <param name="data">条件情報</param>
		/// <returns>クエリのフォーマット文字列</returns>
		private string GetQryFormat(MatchData data)
		{
			switch (data.Qry)
			{
				case QryKbn.Eq:
					return "{0}.{1} = '{{{2}}}'";

				case QryKbn.Sw:
					return "{0}.{1} LIKE SUBSTRING('{{{2}}}', 1, " + data.Length + ") + '%'";
			}
			return "";
		}

		/// <summary>マッチング設定情報</summary>
		public MatchingSetting Setting { get; set; }
		/// <summary>チェック元(OrderShipping)</summary>
		public OrderShippingModel OrderShipping { get; set; }
		/// <summary>チェック元(Order)</summary>
		public OrderModel Order { get; set; }
		/// <summary>チェック元(OrderOwner)</summary>
		public OrderOwnerModel OrderOwner { get; set; }
		/// <summary>条件式の右辺にあたる値</summary>
		public List<string> QueryValues { get; set; }

		#region ユーザーデータクラス
		/// <summary>ユーザーデータクラス</summary>
		private class ShippingData : BaseData
		{
			#region +コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="shipping">配送先情報</param>
			/// <param name="keyList">利用キー列</param>
			public ShippingData(OrderShippingModel shipping, List<string> keyList)
			{
				keyList.ForEach(key => this.Data[key] = shipping.DataSource[key]);
			}
			#endregion
		}
		#endregion

	}
}
