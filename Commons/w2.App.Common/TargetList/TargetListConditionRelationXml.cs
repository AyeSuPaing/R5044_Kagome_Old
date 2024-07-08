/*
=========================================================================================================
  Module      : ターゲットリスト条件のXMl関係クラス(TargetListConditionRelationXml.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Xml;

namespace w2.App.Common.TargetList
{
	/// <summary>
	/// ターゲットリストのXML関連クラス
	/// </summary>
	[Serializable]
	public class TargetListConditionRelationXml
	{
		/// <summary>
		/// ターゲットリスト抽出条件リスト作成
		/// </summary>
		/// <param name="targetListConditionXml">抽出条件(XMLデータ)</param>
		/// <returns>ターゲットリスト抽出条件リスト</returns>
		public static TargetListConditionList CreateTargetListConditionList(string targetListConditionXml)
		{
			var result = new TargetListConditionList();

			var xd = new XmlDocument();
			xd.LoadXml(targetListConditionXml);
			try
			{
				var TargetCondition = xd.SelectSingleNode("/TargetCondition");
				var conditionType = TargetCondition.SelectNodes("Conditions").Item(0).Attributes[0].Value;

				var groupIndex = 1;

				foreach (XmlNode conditions in TargetCondition.ChildNodes)
				{
					// グループ化されているか判定
					ITargetListCondition target = null;
					var groupCondition = conditions.SelectSingleNode("GroupCondition");
					if (groupCondition != null)
					{
						var targetGroup = new TargetListConditionGroup();
						var groupConditionType = groupCondition.Attributes[0].Value;
						foreach (XmlNode condition in groupCondition.ChildNodes)
						{
							var tlc = CreateConditionFromXml(condition);
							tlc.GroupNo = groupIndex;
							tlc.ConditionType = conditionType;
							tlc.GroupConditionType = groupConditionType;
							targetGroup.TargetGroup.Add(tlc);
						}

						groupIndex++;
						target = targetGroup;
					}
					else
					{
						var condition = conditions.SelectSingleNode("Condition");
						var tlc = CreateConditionFromXml(condition);
						tlc.ConditionType = conditionType;
						target = tlc;
					}
					result.TargetConditionList.Add(target);
				}
			}
			catch (Exception)
			{
				// やりすごす
			}

			return result;
		}

		/// <summary>
		/// XMlから条件作成
		/// </summary>
		/// <param name="condition">XML条件</param>
		/// <returns>作成した条件</returns>
		private static TargetListCondition CreateConditionFromXml(XmlNode condition)
		{
			TargetListCondition tlc = new TargetListCondition();
			tlc.DataKbn = condition.SelectSingleNode("DataKbn").InnerText;
			tlc.DataKbnString = condition.SelectSingleNode("DataKbnString").InnerText;
			tlc.DataField = condition.SelectSingleNode("DataField").InnerText;
			tlc.DataFieldString = condition.SelectSingleNode("DataFieldString").InnerText;
			tlc.DataType = condition.SelectSingleNode("DataType").InnerText;
			tlc.FixedPurchaseKbn = condition.SelectSingleNode("FixedPurchaseKbn") != null ? condition.SelectSingleNode("FixedPurchaseKbn").InnerText : "";
			foreach (XmlNode value in condition.SelectNodes("Value"))
			{
				if (value.Attributes["name"] != null)
				{
					tlc.Values.Add(new TargetListCondition.Data(value.Attributes["name"].Value, value.InnerText));
				}
				else
				{
					tlc.Values.Add(new TargetListCondition.Data(value.InnerText));
				}
			}
			tlc.EqualSign = condition.SelectSingleNode("EqualSign").InnerText;
			tlc.EqualSignString = condition.SelectSingleNode("EqualSignString").InnerText;
			tlc.OrderExist = condition.SelectSingleNode("OrderExist") != null ? condition.SelectSingleNode("OrderExist").InnerText : "";
			tlc.OrderExistString = condition.SelectSingleNode("OrderExistString") != null ? condition.SelectSingleNode("OrderExistString").InnerText : "";
			tlc.FixedPurchaseOrderExist = (condition.SelectSingleNode("FixedPurchaseOrderExist") != null)
				? condition.SelectSingleNode("FixedPurchaseOrderExist").InnerText 
				: "";
			tlc.FixedPurchaseOrderExistString = (condition.SelectSingleNode("FixedPurchaseOrderExistString") != null)
				? condition.SelectSingleNode("FixedPurchaseOrderExistString").InnerText
				: "";
			tlc.PointExist = condition.SelectSingleNode("PointExist") != null ? condition.SelectSingleNode("PointExist").InnerText : "";
			tlc.PointExistString = condition.SelectSingleNode("PointExistString") != null ? condition.SelectSingleNode("PointExistString").InnerText : "";
			tlc.DmShippingHistoryExist = condition.SelectSingleNode("DmShippingHistoryExist") != null ? condition.SelectSingleNode("DmShippingHistoryExist").InnerText : "";
			tlc.DmShippingHistoryExistString = condition.SelectSingleNode("DmShippingHistoryExistString") != null ? condition.SelectSingleNode("DmShippingHistoryExistString").InnerText : "";
			tlc.FavoriteExist = (condition.SelectSingleNode("FavoriteExist") != null) ? condition.SelectSingleNode("FavoriteExist").InnerText : "";
			tlc.FavoriteExistString = (condition.SelectSingleNode("FavoriteExistString") != null) ? condition.SelectSingleNode("FavoriteExistString").InnerText : "";

			return tlc;
		}

		/// <summary>
		/// ターゲットリスト抽出条件XML作成
		/// </summary>
		/// <param name="targetListCondition">ターゲットリスト抽出条件リスト</param>
		/// <returns>ターゲットリスト抽出条件XML</returns>
		public static string CreateTargetListConditionXml(TargetListConditionList targetListCondition)
		{
			using(var sw = new StringWriter())
			using(var xtw = new XmlTextWriter(sw))
			{
				xtw.WriteProcessingInstruction("xml", "version='1.0' encoding='utf-8'");	// 強制UTF-8
				xtw.WriteStartElement("TargetCondition");

				foreach (var condition in targetListCondition.TargetConditionList)
				{
					xtw.WriteStartElement("Conditions");
					xtw.WriteAttributeString("type", condition.GetConditionType(condition));

					condition.MakeXmlFromCondition(condition, xtw);

					xtw.WriteEndElement();	// Conditions
				}
				xtw.WriteEndElement();	// TargetCondition

				return sw.ToString();
			}
		}
	}
}
