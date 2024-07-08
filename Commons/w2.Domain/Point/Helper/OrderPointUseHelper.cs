/*
=========================================================================================================
  Module      : 注文利用ポイント計算用のヘルパクラス (OrderPointUseHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;

namespace w2.Domain.Point.Helper
{
	/// <summary>
	/// 注文利用ポイント計算用のヘルパクラス
	/// </summary>
	public class OrderPointUseHelper
	{
		#region +GetUseablePointFromPrice 利用可能金額から利用可能ポイント数に変換
		/// <summary>
		/// 利用可能金額から利用可能ポイント数に変換
		/// </summary>
		/// <param name="useablePrice">最大利用可能金額</param>
		/// <param name="pointKbn">ポイント区分</param>
		/// <param name="pointMaster">ポイントモデル</param>
		/// <returns>利用可能ポイント数</returns>
		public static decimal GetUseablePointFromPrice(decimal useablePrice, string pointKbn, PointModel pointMaster)
		{
			// ポイントマスタからポイント換算率が取れなければ計算できない
			var point = ((pointMaster == null) || (pointMaster.ExchangeRate == 0))
				? 0
				: Math.Ceiling(useablePrice / pointMaster.ExchangeRate) * pointMaster.UsableUnit;
			return point;
		}
		#endregion

		#region +GetOrderPointUsePrice 注文ポイント利用額取得
		/// <summary>
		/// 注文ポイント利用額取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="orderPointUse">利用ポイント</param>
		/// <returns>注文ポイント利用額（利用する際はToPriceDecimalする必要あり）</returns>
		public static decimal GetOrderPointUsePrice(string deptId, decimal orderPointUse)
		{
			var pointMaster = DomainFacade.Instance.PointService.GetPointMaster()
				.FirstOrDefault(x => x.DeptId == deptId);
			var price = GetOrderPointUsePrice(orderPointUse, pointMaster);
			return price;
		}
		/// <summary>
		/// 注文ポイント利用額取得
		/// </summary>
		/// <param name="orderPointUse">利用ポイント</param>
		/// <param name="pointMaster">ポイントモデル</param>
		/// <returns>注文ポイント利用額（利用する際はToPriceDecimalする必要あり）</returns>
		public static decimal GetOrderPointUsePrice(decimal orderPointUse, PointModel pointMaster)
		{
			// ポイントマスタからポイント利用可能単位が取れなければ計算できない
			var price = (pointMaster == null || pointMaster.UsableUnit == 0)
				? 0
				: (orderPointUse / pointMaster.UsableUnit) * pointMaster.ExchangeRate;
			return price;
		}
		#endregion
	}
}
