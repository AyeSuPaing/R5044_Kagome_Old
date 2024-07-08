/*
=========================================================================================================
  Module      : ABテストユーティリティ(AbTestUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.Domain.AbTest;

namespace w2.App.Common.ABTest.Util
{
	public class AbTestUtil
	{
		/// <summary>
		/// エラー内容列挙
		/// </summary>
		public enum ValidateStatus
		{
			NoError = 0,
			NoPage404 = 1,
			PrivateAbTest = 2,
			OutOfPeriodAbTest = 3,
			MisMatchedPageId = 4
		}

		/// <summary>
		/// ABTestの有効性
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <param name="landingPageId">LPID</param>
		/// <returns>エラー番号</returns>
		public static ValidateStatus ValidateAbTest(string abTestId, string landingPageId = null)
		{
			var abTest = new AbTestService().Get(abTestId);

			if (abTest == null) return ValidateStatus.NoPage404;
			if (abTest.PublicStatus == Constants.FLG_ABTEST_PUBLISH_PRIVATE) return ValidateStatus.PrivateAbTest;

			//ABテストの公開期間内か判定
			if ((DateTime.Now < abTest.PublicStartDatetime)
				|| ((abTest.PublicEndDatetime != null) && (DateTime.Today > (DateTime)abTest.PublicEndDatetime)))
			{
				return ValidateStatus.OutOfPeriodAbTest;
			}

			if (string.IsNullOrEmpty(landingPageId) == false)
			{
				var abTestItems = new AbTestService().GetAllItemByAbTestId(abTestId);
				if (abTestItems.Any(item => (item.PageId == landingPageId)) == false)
				{
					return ValidateStatus.MisMatchedPageId;
				}
			}

			return ValidateStatus.NoError;
		}
	}
}
