/*
=========================================================================================================
  Module      : ターゲットリストテンプレートクラス(TargetListTemplate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace w2.App.Common.TargetList
{
	/// <summary>
	/// ターゲットリストテンプレートクラス
	/// </summary>
	public class TargetListTemplate
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private TargetListTemplate()
		{
		}

		/// <summary>
		/// テンプレートリスト取得
		/// </summary>
		/// <param name="searchCategory">カテゴリの検索</param>
		/// <param name="searchName">テンプレート名の検索</param>
		/// <param name="sortKbn">テンプレートIDでの並び替え区分（１：昇順；１以外：降順）</param>
		/// <returns>テンプレートリスト</returns>
		public static List<TargetListTemplate> GetTemplateList(
			string searchCategory = "",
			string searchName = "",
			string sortKbn = "1")
		{
			var targetListTemplate = new XmlDocument();
			targetListTemplate.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.FILE_XML_TARGETLIST_TEMPLATE));

			var xnTemplates = targetListTemplate.SelectSingleNode("TargetListTemplate");
			var templateList = new List<TargetListTemplate>();
			if (xnTemplates == null) return templateList;

			var groupNo = 1;

			foreach (XmlNode xnTemplate in xnTemplates.ChildNodes)
			{
				if (xnTemplate.Name != "Template") continue;
				
				// 検索条件での絞り込み
				var name = xnTemplate.SelectSingleNode("Name").InnerText;
				var category = xnTemplate.SelectSingleNode("Category").InnerText;
				// カテゴリ(完全一致)
				if ((searchCategory != "") && (category.Split(',').Any(c => c == searchCategory) == false)) continue;
				// テンプレート名（部分一致）
				if ((searchName != "") && (name.Contains(searchName) == false)) continue;
				
				var template = new TargetListTemplate();
				template.TemplateId = xnTemplate.SelectSingleNode("TemplateId").InnerText;
				template.Name = name;
				template.Category = category;
				template.TemplateConditionList = new TargetListConditionList();

				var conditionType = xnTemplate.SelectSingleNode("Conditions").Attributes[0].Value;

				foreach (XmlNode xnConditions in xnTemplate.ChildNodes)
				{
					if (xnConditions.Name != "Conditions") continue;

					var groupCondition = xnConditions.SelectSingleNode("GroupCondition");

					if (groupCondition != null)
					{
						var targetGroup = new TargetListConditionGroup();
						var groupConditionType = groupCondition.Attributes[0].Value;

						foreach (XmlNode conditions in groupCondition.ChildNodes)
						{
							var tlc = CreateTargetListCondition(conditions, conditionType);
							tlc.GroupConditionType = groupConditionType;
							tlc.GroupNo = groupNo;
							targetGroup.TargetGroup.Add(tlc);

							groupNo++;
						}

						template.TemplateConditionList.TargetConditionList.Add(targetGroup);
					}
					else
					{
						if (xnConditions.Name != "Conditions") continue;

						var xnCondition = xnConditions.SelectSingleNode("Condition");
						var condition = CreateTargetListCondition(xnCondition, conditionType);

						template.TemplateConditionList.TargetConditionList.Add(condition);
					}
				}
				templateList.Add(template);
			}

			// テンプレートIDでの並び替え
			templateList.Sort(
				(a, b) => (sortKbn == "1")
					? (int.Parse(a.TemplateId) - int.Parse(b.TemplateId))
					: (int.Parse(b.TemplateId) - int.Parse(a.TemplateId)));

			return templateList;
		}

		/// <summary>
		/// XMLから条件作成
		/// </summary>
		/// <param name="xnCondition">元のXML</param>
		/// <param name="conditionType">AND、OR条件</param>
		/// <returns>作成した条件</returns>
		private static TargetListCondition CreateTargetListCondition(XmlNode xnCondition, string conditionType)
		{
			var fixedPurchasePattern = string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1);

			var condition = new TargetListCondition
			{
				DataKbn = xnCondition.SelectSingleNode("DataKbn").InnerText,
				DataField = xnCondition.SelectSingleNode("DataField").InnerText,
				DataType = xnCondition.SelectSingleNode("DataType").InnerText,
				EqualSign = xnCondition.SelectSingleNode("EqualSign").InnerText,
				OrderExist = xnCondition.SelectSingleNode("OrderExist").InnerText,
				ConditionType = conditionType,
				FixedPurchaseKbn = (xnCondition.SelectSingleNode("DataField").InnerText == fixedPurchasePattern) ? xnCondition.SelectSingleNode("FixedPurchaseKbn").InnerText : "",
			};
			condition.Values.Add(new TargetListCondition.Data(ReplaceDateValue(xnCondition.SelectSingleNode("Value").InnerText)));
			return condition;
		}

		/// <summary>
		/// テンプレートのValueの日付の置き換え
		/// </summary>
		/// <param name="value">Valueの値</param>
		/// <returns>日付文字列</returns>
		private static string ReplaceDateValue(string value)
		{
			// 置き換え用文字列がない場合は、引数の文字列をそのまま返す
			if (value.IndexOf("@@") == -1) return value;

			var pattern = @"@@(.*),(.*),(.*),(.*)@@";
			var match = Regex.Match(value, pattern);

			// 置き換え用文字の指定が不正な場合はから文字列を返す
			var yearParam = 0;
			var monthParam = 0;
			var dayParam = 0;
			if ((match.Success == false)
				|| (match.Groups.Count != 5)
				|| (int.TryParse(match.Groups[2].Value.Trim(), out yearParam) == false)
				|| (int.TryParse(match.Groups[3].Value.Trim(), out monthParam) == false)
				|| (int.TryParse(match.Groups[4].Value.Trim(), out dayParam) == false)) return "";

			var cmd = match.Groups[1].Value.Trim();
			switch (cmd)
			{
				case "AddDate":
					var dateValue = DateTime.Now.AddYears(yearParam)
						.AddMonths(monthParam)
						.AddDays(dayParam);
					return dateValue.ToString("yyyy/MM/dd");

				default:
					return value;
			}
		}

		/// <summary>テンプレートID</summary>
		public string TemplateId { get; private set; }
		/// <summary>テンプレート名</summary>
		public string Name { get; private set; }
		/// <summary>カテゴリ</summary>
		public string Category { get; private set; }
		/// <summary>テンプレート条件</summary>
		public TargetListConditionList TemplateConditionList { get; private set; }
	}
}
