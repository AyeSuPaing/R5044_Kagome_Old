/*
=========================================================================================================
  Module      : タグマネージャーのバージョンアップ用マイグレーション v5.13→v5.14 (Migration.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.Affiliate;

namespace w2.Commerce.Batch.TagManagerMigration
{
	/// <summary>
	/// タグマネージャーのバージョンアップ用マイグレーション v5.13→v5.14
	/// </summary>
	public class Migration
	{
		/// <summary>
		/// マイグレーション処理
		/// </summary>
		public void Process()
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Statements", "InitializeAffiliateProductTagSettingAndAffiliateTagCondition"))
			{
				statement.ExecStatementWithOC(accessor);
			}

			var affiliatetabSettingService = new AffiliateTagSettingService();
			var affilateTagSettingModels = new AffiliateTagSettingService().GetAllIncludeConditionModels();

			foreach (var affiliateTagSettingModel in affilateTagSettingModels)
			{
				// モバイルの場合は無効設定
				if (affiliateTagSettingModel.AffiliateKbn.ToLower() == Constants.FLG_AFFILIATETAGSETTING_AFFILIATE_KBN_MOBILE.ToLower())
				{
					affiliateTagSettingModel.ValidFlg = Constants.FLG_AFFILIATETAGSETTING_VALID_FLG_UNVALID;
					affiliateTagSettingModel.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
					affiliatetabSettingService.Update(affiliateTagSettingModel);
					continue;
				}

				// タグ設定の置換
				affiliateTagSettingModel.OutputLocation = Constants.FLG_AFFILIATETAGSETTING_OUTPUT_LOCATION_BODY_TOP;
				affiliateTagSettingModel.AffiliateKbn = Constants.FLG_AFFILIATETAGSETTING_AFFILIATE_KBN_PC_SP;
				affiliateTagSettingModel.AffiliateTag1 = affiliateTagSettingModel.AffiliateTag1.Replace("%USERID%", "@@LOGIN_USER_ID@@");
				affiliateTagSettingModel.AffiliateTag1 = affiliateTagSettingModel.AffiliateTag1.Replace("%ORDER%", "@@ORDER_ID@@");
				affiliateTagSettingModel.AffiliateTag1 = affiliateTagSettingModel.AffiliateTag1.Replace("%PRICE%", "@@ORDER_PRICE_SUB_TOTAL@@");
				affiliateTagSettingModel.AffiliateTag1 = affiliateTagSettingModel.AffiliateTag1.Replace("%PRICE_TAXEXCLUDED%", "@@ORDER_PRICE_SUB_TOTAL_EXCLUDED@@");
				affiliateTagSettingModel.AffiliateTag1 = affiliateTagSettingModel.AffiliateTag1.Replace("%SEX%", "@@OWNER_USER_SEX@@");
				affiliateTagSettingModel.AffiliateTag1 = affiliateTagSettingModel.AffiliateTag1.Replace("%AGE%", "@@OWNER_USER_AGE@@");
				affiliateTagSettingModel.AffiliateTag1 = affiliateTagSettingModel.AffiliateTag1.Replace("%PREFECTURE%", "@@OWNER_USER_ZIP@@");

				var mcProductTag = Regex.Matches(
					affiliateTagSettingModel.AffiliateTag1,
					"%PRODUCT:(?<delimiter>.*?)%",
					RegexOptions.Singleline | RegexOptions.IgnoreCase);

				AffiliateProductTagSettingModel productTag = null;
				if (mcProductTag.Count > 0)
				{
					var delimiter = mcProductTag[0].Groups["delimiter"].ToString();

					if (string.IsNullOrEmpty(delimiter) == false)
					{
						productTag = affiliatetabSettingService.AffiliateProductTagSettingGetAll()
							.FirstOrDefault(i => (i.TagDelimiter == delimiter));

						if (productTag == null)
						{
							var affiliateProductTagSettingModel = new AffiliateProductTagSettingModel()
							{
								TagDelimiter = delimiter,
								TagContent = "@@VARIATION_ID@@.@@QUANTITY@@.@@PRODUCT_PRICE@@",
								TagName = "商品タグ",
								LastChanged = Constants.FLG_LASTCHANGED_BATCH
							};
							affiliatetabSettingService.AffiliateProductTagSettingInsert(
								affiliateProductTagSettingModel);

							productTag = affiliatetabSettingService.AffiliateProductTagSettingGetAll()
								.FirstOrDefault(i => (i.TagDelimiter == delimiter));
						}

						if (mcProductTag.Count > 1)
						{
							var message = string.Format(
								"ID:{0}  PRODUCTタグが複数存在します。タグを分割して登録して手動で登録してください。",
								affiliateTagSettingModel.AffiliateId);
							Console.WriteLine(message);
							FileLogger.WriteInfo(message);
						}
						else
						{
							affiliateTagSettingModel.AffiliateTag1 = affiliateTagSettingModel.AffiliateTag1.Replace(string.Format("%PRODUCT:{0}%", delimiter), "@@PRODUCT@@");
						}
					}
				}

				// 条件の追加 注文完了画面のみ
				var affiliateTagConditionInsertAll = new []
				{
					new AffiliateTagConditionModel()
					{
						AffiliateId = affiliateTagSettingModel.AffiliateId,
						ConditionSortNo = 1,
						ConditionType = Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PAGE,
						ConditionValue = "Order/OrderComplete.aspx",
						MatchType = Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_PERFECT,
					},
					new AffiliateTagConditionModel()
					{
						AffiliateId = affiliateTagSettingModel.AffiliateId,
						ConditionSortNo = 1,
						ConditionType = Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID,
						ConditionValue = "",
						MatchType = Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_PERFECT,
					},
					new AffiliateTagConditionModel()
					{
						AffiliateId = affiliateTagSettingModel.AffiliateId,
						ConditionSortNo = 1,
						ConditionType = Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADVERTISEMENT_CODE,
						ConditionValue = "",
						MatchType = Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_PERFECT,
					},
					new AffiliateTagConditionModel()
					{
						AffiliateId = affiliateTagSettingModel.AffiliateId,
						ConditionSortNo = 1,
						ConditionType = Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADCODE_MEDIA_TYPE,
						ConditionValue = "",
						MatchType = Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_PERFECT,
					}
				};
				affiliatetabSettingService.AffiliateTagConditionInsertAll(affiliateTagConditionInsertAll);

				if (productTag != null)
				{
					affiliateTagSettingModel.AffiliateProductTagId = productTag.AffiliateProductTagId;
				}

				affiliateTagSettingModel.AffiliateTag1 = "@@ORDER_LOOP_START@@\n" + affiliateTagSettingModel.AffiliateTag1 + "\n@@ORDER_LOOP_END@@";

				affiliatetabSettingService.Update(affiliateTagSettingModel);
			}
		}
	}
}