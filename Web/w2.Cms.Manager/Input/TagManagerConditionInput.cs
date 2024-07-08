/*
=========================================================================================================
  Module      : タグマネージャー 表示条件Input(TagManagerConditionInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Domain.Affiliate;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// タグマネージャー 表示条件Input
	/// </summary>
	public class TagManagerConditionInput : InputBase<AffiliateTagConditionModel>
	{
		/// <summary>条件有無選択 ラジオボタン 条件なし</summary>
		public const string CONDITION_RADIOBUTTON_NO_VALUE = "CONDITION_NO";
		/// <summary>条件有無選択 ラジオボタン 条件あり</summary>
		public const string CONDITION_RADIOBUTTON_YES_VALUE = "CONDITION_YES";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TagManagerConditionInput()
		{
			this.IsNoConditionAdcodeMediaType = CONDITION_RADIOBUTTON_NO_VALUE;
			this.IsNoConditionAdvertisementCode = CONDITION_RADIOBUTTON_NO_VALUE;
			this.IsNoConditionProductIdCode = CONDITION_RADIOBUTTON_NO_VALUE;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="affiliateId">アフィリエイトタグID</param>
		/// <param name="models">条件モデルリスト</param>
		public TagManagerConditionInput(string affiliateId, AffiliateTagConditionModel[] models)
			: this()
		{
			this.AffiliateId = affiliateId;
			this.ConditionValuesPage = CreateDisplayValue(
				models,
				Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PAGE);

			this.ConditionValuesAdcodeMediaType = CreateDisplayValue(
				models,
				Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADCODE_MEDIA_TYPE);
			this.MatchTypeAdcodeMediaType = GetMatchType(
				models,
				Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADCODE_MEDIA_TYPE);

			this.ConditionValuesAdvertisementCode = CreateDisplayValue(
				models,
				Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADVERTISEMENT_CODE);
			this.MatchTypeAdvertisementCode = GetMatchType(
				models,
				Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADVERTISEMENT_CODE);

			this.ConditionValuesProductId = CreateDisplayValue(
				models,
				Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID);
			this.MatchTypeProductId = GetMatchType(
				models,
				Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID);

			this.IsMatchTypeForwordAdcodeMediaType = (this.MatchTypeAdcodeMediaType
				== Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_FORWARD);

			this.IsMatchTypeForwordAdvertisementCode = (this.MatchTypeAdvertisementCode
				== Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_FORWARD);

			this.IsMatchTypeForwordProductId =
				(this.MatchTypeProductId == Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_FORWARD);

			this.IsNoConditionAdvertisementCode =
				(string.IsNullOrEmpty(this.ConditionValuesAdvertisementCode) == false)
					? CONDITION_RADIOBUTTON_YES_VALUE
					: CONDITION_RADIOBUTTON_NO_VALUE;

			this.IsNoConditionAdcodeMediaType =
				(string.IsNullOrEmpty(this.ConditionValuesAdcodeMediaType) == false)
					? CONDITION_RADIOBUTTON_YES_VALUE
					: CONDITION_RADIOBUTTON_NO_VALUE;

			this.IsNoConditionProductIdCode =
				(string.IsNullOrEmpty(this.ConditionValuesProductId) == false)
					? CONDITION_RADIOBUTTON_YES_VALUE
					: CONDITION_RADIOBUTTON_NO_VALUE;

		}

		/// <summary>
		/// 基底クラス モデル生成
		/// </summary>
		/// <returns>モデル</returns>
		public override AffiliateTagConditionModel CreateModel()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// モデル生成
		/// </summary>
		/// <returns>条件モデルリスト</returns>
		public IEnumerable<AffiliateTagConditionModel> CreateModels()
		{
			this.ConditionValuesAdcodeMediaType = (this.IsNoConditionAdcodeMediaType == CONDITION_RADIOBUTTON_NO_VALUE)
				? string.Empty
				: this.ConditionValuesAdcodeMediaType;
			this.MatchTypeAdcodeMediaType =
				((this.IsNoConditionAdcodeMediaType == CONDITION_RADIOBUTTON_NO_VALUE)
					|| (this.IsMatchTypeForwordAdcodeMediaType == false))
					? Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_PERFECT
					: Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_FORWARD;

			this.ConditionValuesAdvertisementCode = (this.IsNoConditionAdvertisementCode == CONDITION_RADIOBUTTON_NO_VALUE)
				? string.Empty
				: this.ConditionValuesAdvertisementCode;
			this.MatchTypeAdvertisementCode =
				((this.IsNoConditionAdvertisementCode == CONDITION_RADIOBUTTON_NO_VALUE)
					|| (this.IsMatchTypeForwordAdvertisementCode == false))
					? Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_PERFECT
					: Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_FORWARD;

			this.ConditionValuesProductId = (this.IsNoConditionProductIdCode == CONDITION_RADIOBUTTON_NO_VALUE)
				? string.Empty
				: this.ConditionValuesProductId;
			this.MatchTypeProductId =
				((this.IsNoConditionProductIdCode == CONDITION_RADIOBUTTON_NO_VALUE)
					|| (this.IsMatchTypeForwordProductId == false))
					? Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_PERFECT
					: Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_FORWARD;

			var baseModel = new AffiliateTagConditionModel
			{
				AffiliateId = int.Parse(this.AffiliateId),
			};

			var itemTypeValues = new Dictionary<string, string>
			{
				{
					Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PAGE, 
					this.ConditionValuesPage
				},
				{
					Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADCODE_MEDIA_TYPE,
					this.ConditionValuesAdcodeMediaType
				},
				{
					Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADVERTISEMENT_CODE,
					this.ConditionValuesAdvertisementCode
				},
				{
					Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID,
					this.ConditionValuesProductId
				}
			};

			// 条件の入力内容から条件モデルを作成
			foreach (var itemType in itemTypeValues.Keys)
			{
				var values = itemTypeValues[itemType].Split(
					new[] { Environment.NewLine },
					StringSplitOptions.RemoveEmptyEntries);

				var sortNo = 0;
				if (values.Length != 0)
				{
					foreach (var value in values)
					{
						if (string.IsNullOrEmpty(value)) continue;

						sortNo++;
						var model = baseModel.Clone();
						model.ConditionType = itemType;
						model.ConditionValue = value;
						model.MatchType = GetMatchType(itemType);
						model.ConditionSortNo = sortNo;
						yield return model;
					}
				}

				if (sortNo == 0)
				{
					var model = baseModel.Clone();
					model.ConditionType = itemType;
					model.ConditionValue = string.Empty;
					model.MatchType = Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_PERFECT;
					model.ConditionSortNo = 1;
					yield return model;
				}
			}
		}

		/// <summary>
		/// 対応した条件タイプの改行リストを取得
		/// </summary>
		/// <param name="models">表示モデル</param>
		/// <param name="conditionType">条件タイプ</param>
		/// <returns>対応した条件タイプの改行リスト</returns>
		private string CreateDisplayValue(AffiliateTagConditionModel[] models, string conditionType)
		{
			var tempValue = models
				.Where(i => i.ConditionType == conditionType)
				.OrderBy(o => o.ConditionSortNo)
				.Select(v => v.ConditionValue);
			return string.Join(Environment.NewLine, tempValue);
		}

		/// <summary>
		/// 条件モデルの中から一致する条件タイプの一致条件を取得
		/// </summary>
		/// <param name="models">条件モデル</param>
		/// <param name="conditionType">条件タイプ</param>
		/// <returns>一致条件</returns>
		private string GetMatchType(AffiliateTagConditionModel[] models, string conditionType)
		{
			if (models.Any(i => i.ConditionType == conditionType) == false) return string.Empty;

			var matchType = models.FirstOrDefault(i => i.ConditionType == conditionType).MatchType;
			return matchType;
		}

		/// <summary>
		/// 一致条件の取得
		/// </summary>
		/// <param name="conditionType">条件モデル</param>
		/// <returns>一致条件</returns>
		private string GetMatchType(string conditionType)
		{
			switch (conditionType)
			{
				case Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PAGE:
					return Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_PERFECT;

				case Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADCODE_MEDIA_TYPE:
					return this.MatchTypeAdcodeMediaType;

				case Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADVERTISEMENT_CODE:
					return this.MatchTypeAdvertisementCode;

				case Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID:
					return this.MatchTypeProductId;

				default:
					return string.Empty;
			}
		}

		#region プロパティ
		/// <summary>アフィリエイトタグID</summary>
		public string AffiliateId
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_AFFILIATE_ID]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_AFFILIATE_ID] = value; }
		}
		/// <summary>内容 画面</summary>
		public string ConditionValuesPage
		{
			get
			{
				return (string)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PAGE];
			}
			set
			{
				this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PAGE] = value;
			}
		}
		/// <summary>内容 広告媒体区分</summary>
		public string ConditionValuesAdcodeMediaType
		{
			get
			{
				return (string)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADCODE_MEDIA_TYPE];
			}
			set
			{
				this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADCODE_MEDIA_TYPE] = value;
			}
		}
		/// <summary>一致条件タイプ 広告媒体区分</summary>
		public string MatchTypeAdcodeMediaType
		{
			get
			{
				return (string)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_MATCH_TYPE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADCODE_MEDIA_TYPE];
			}
			set
			{
				this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_MATCH_TYPE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADCODE_MEDIA_TYPE] = value;
			}
		}
		/// <summary>内容 広告コード</summary>
		public string ConditionValuesAdvertisementCode
		{
			get
			{
				return (string)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADVERTISEMENT_CODE];
			}
			set
			{
				this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADVERTISEMENT_CODE] = value;
			}
		}
		/// <summary>一致条件タイプ 広告コード</summary>
		public string MatchTypeAdvertisementCode
		{
			get
			{
				return (string)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_MATCH_TYPE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADVERTISEMENT_CODE];
			}
			set
			{
				this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_MATCH_TYPE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADVERTISEMENT_CODE] = value;
			}
		}
		/// <summary>内容 商品ID</summary>
		public string ConditionValuesProductId
		{
			get
			{
				return (string)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID];
			}
			set
			{
				this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID] = value;
			}
		}
		/// <summary>一致条件タイプ 商品ID</summary>
		public string MatchTypeProductId
		{
			get
			{
				return (string)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_MATCH_TYPE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID];
			}
			set
			{
				this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_MATCH_TYPE
					+ Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID] = value;
			}
		}
		/// <summary>条件有無 選択状況 広告媒体</summary>
		public string IsNoConditionAdcodeMediaType { get; set; }
		/// <summary>条件有無 選択状況 広告コード</summary>
		public string IsNoConditionAdvertisementCode { get; set; }
		/// <summary>条件有無 選択状況 商品ID</summary>
		public string IsNoConditionProductIdCode { get; set; }
		/// <summary>前方一致 チェック状況 広告媒体</summary>
		public bool IsMatchTypeForwordAdcodeMediaType { get; set; }
		/// <summary>前方一致 チェック状況 広告コード</summary>
		public bool IsMatchTypeForwordAdvertisementCode { get; set; }
		/// <summary>前方一致 チェック状況 商品ID</summary>
		public bool IsMatchTypeForwordProductId { get; set; }
		#endregion
	}
}