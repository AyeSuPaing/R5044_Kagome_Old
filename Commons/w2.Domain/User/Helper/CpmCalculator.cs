/*
=========================================================================================================
  Module      : CPM（顧客ポートフォリオマネジメント）計算クラス(CpmCalculator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Domain.User.Helper
{
	/// <summary>
	/// CPM（顧客ポートフォリオマネジメント）計算クラス
	/// </summary>
	public class CpmCalculator
	{
		/// <summary>インスタンス</summary>
		private static readonly CpmCalculator m_instance = new CpmCalculator();

		/// <summary>
		/// インスタンス取得
		/// </summary>
		public static CpmCalculator GetInstance()
		{
			return m_instance;
		}

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		private CpmCalculator()
		{
		}

		/// <summary>
		/// CPM算出
		/// </summary>
		/// <param name="attr">ユーザー属性</param>
		/// <param name="cpmClusterSettings">CPMクラスタ設定</param>
		/// <returns>CPMクラスタ名</returns>
		public Tuple<CpmClusterSetting1, CpmClusterSetting2> Calculate(UserAttributeModel attr, CpmClusterSettings cpmClusterSettings)
		{
			// 購入回数0回であれば割り当てなし
			if (attr.OrderCountOrderAll == 0) return null;

			// 現役客か離脱客か判定（デフォルトでは離脱期間が240日未満は現役）
			var setting2 = cpmClusterSettings.Settings2.FirstOrDefault(
				setting => (setting.AwayDays.HasValue == false) || (attr.AwayDays < setting.AwayDays));

			// クラスタ判定
			foreach (var setting in cpmClusterSettings.Settings1)
			{
				if (((setting.EnrollmentDays.HasValue == false) || (attr.EnrollmentDays >= setting.EnrollmentDays))
					&& ((setting.BuyCount.HasValue == false) || (attr.OrderCountOrderAll >= setting.BuyCount))
                    && ((setting.BuyAmount.HasValue == false) || (attr.OrderAmountOrderAll >= setting.BuyAmount)))
				{
					return new Tuple<CpmClusterSetting1, CpmClusterSetting2>(setting, setting2);
				}
			}
			throw new Exception("設定エラー：割り当てられるクラスタはありませんでした。");
		}
	}
}
