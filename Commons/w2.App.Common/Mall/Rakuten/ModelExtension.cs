/*
=========================================================================================================
  Module      : 楽天APIモデル拡張クラス (ModelExtension.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Reflection;

namespace w2.App.Common.Mall.Rakuten
{
	public partial class baseResponseModel : ToJsonString { }

	public partial class baseRequestModel : ToJsonString { }

	public partial class asyncResultModel : ToJsonString { }

	public partial class cancelModel : ToJsonString { }

	[MaskedJsonAttribute]
	public partial class cardModel : ToMaskedJsonString { }

	[MaskedJsonAttribute]
	public partial class cardSearchModel : ToMaskedJsonString { }

	public partial class changeEnclosureParentModel : ToJsonString { }

	public partial class couponModel : ToJsonString { }

	public partial class taxSummaryModel : ToJsonString { }

	public partial class enclosureTaxSummaryModel : ToJsonString { }

	public partial class deliveryModel : ToJsonString { }

	public partial class deliveryCvsModel : ToJsonString { }

	public partial class enclosureGroupModel : ToJsonString { }

	public partial class gbuyBidInventoryModel : ToJsonString { }

	public partial class gbuyGchoiceModel : ToJsonString { }

	public partial class gbuyItemModel : ToJsonString { }

	public partial class gbuyOrderModel : ToJsonString { }

	public partial class itemModel : ToJsonString { }

	public partial class nominateEnclosureModel : ToJsonString { }

	public partial class normalItemModel : ToJsonString { }

	public partial class normalOrderModel : ToJsonString { }

	public partial class orderModel : ToJsonString { }

	public partial class orderSearchModel : ToJsonString { }

	public partial class orderStatusModel : ToJsonString { }

	public partial class packageModel : ToJsonString { }

	[MaskedJsonAttribute]
	public partial class personModel : ToMaskedJsonString { }

	public partial class pointModel : ToJsonString { }

	public partial class rBankModel : ToJsonString { }

	public partial class saOrderModel : ToJsonString { }

	public partial class settlementModel : ToJsonString { }

	public partial class UiAuthoriRequestModel : ToJsonString { }

	public partial class UiCancelRequestModel : ToJsonString { }

	public partial class UiRCCSRequestResultModel : ToJsonString { }

	public partial class UiRCCSReplyModel : ToJsonString { }

	public partial class UiRCCSRequestStatusModel : ToJsonString { }

	[MaskedJsonAttribute]
	public partial class UiRCCSResultModel : ToMaskedJsonString { }

	public partial class UiRCCSResultSearchModel : ToJsonString { }

	public partial class UiRCCSResultSearchResultModel : ToJsonString { }

	public partial class UiRequestIdModel : ToJsonString { }

	public partial class UiSalesRequestModel : ToJsonString { }

	public partial class UnitErrorModel : ToJsonString { }

	public partial class unitErrorModel : ToJsonString { }

	public partial class userAuthModel : ToJsonString { }

	public partial class UserAuthModel : ToJsonString { }

	public partial class wrappingModel : ToJsonString { }

	/// <summary>
	/// ToStringをJson出力するようにオーバーライドしたクラス
	/// </summary>
	public class ToJsonString
	{
		/// <summary>
		/// ToString関数をJson出力するようにオーバーライド
		/// </summary>
		/// <returns>Json形式文字列</returns>
		public override string ToString()
		{
			return Model2JsonString.GenerateJson(this);
		}
	}

	/// <summary>
	/// ToStringをJson出力するようにオーバーライドしたクラス（個人情報につきマスクしたというメッセージのみ）
	/// </summary>
	public class ToMaskedJsonString
	{
		/// <summary>
		/// ToString関数をJson出力するようにオーバーライド
		/// </summary>
		/// <returns>Json形式文字列</returns>
		public override string ToString()
		{
			return string.Format("{{{0}:'個人情報につきマスク'}}", this.GetType().Name);
		}
	}

	/// <summary>
	/// オブジェクトをJson形式文字列に変換する。プロパティだけ出力する。
	/// </summary>
	static class Model2JsonString
	{
		/// <summary>
		/// オブジェクトをもとにJson形式文字列を生成する。
		/// </summary>
		/// <param name="obj">Json形式に変換したいオブジェクト</param>
		/// <returns>Json形式文字列</returns>
		public static string GenerateJson(object obj)
		{
			if (obj == null) return "null";

			// マスクされていれば、単にToStringのみ出力
			if (obj.GetType().GetCustomAttributes(typeof(MaskedJsonAttribute), true).Length > 0)
			{
				return string.Format("{0}", obj.ToString());
			}

			PropertyInfo[] properties = obj.GetType().GetProperties();

			// 配列は展開する
			if (obj.GetType().IsArray)
			{
				return string.Format("[{0}]", GenerateArrayToJson((Array)obj));
			}
			// Systemもしくはプロパティ無は値だけ出力する
			else if (obj.GetType().Namespace.StartsWith("System") || (properties.Length == 0))
			{
				return string.Format("'{0}'", obj.ToString());
			}
			// それ以外は「名:値」のように出力する
			else
			{
				string[] propertyTxts = new string[properties.Length];
				int index = 0;
				foreach (PropertyInfo property in properties)
				{
					string value = GenerateJson(obj.GetType().InvokeMember(property.Name, BindingFlags.GetProperty, null, obj, null));
					propertyTxts[index++] = string.Format("{0}:{1}", property.Name, value);
				}
				return string.Format("{{{0}:{{{1}}}}}", obj.GetType().Name, string.Join(", ", propertyTxts));
			}
		}

		/// <summary>
		/// 配列を展開してJsonにする 
		/// </summary>
		/// <param name="array">配列オブジェクト</param>
		/// <returns>Json形式文字列</returns>
		private static string GenerateArrayToJson(Array array)
		{
			string[] txts = new string[array.Length];
			for (int index = 0; index < array.Length; index++)
			{
				txts[index] = GenerateJson(array.GetValue(index));
			}

			return string.Join(", ", txts);
		}
	}

	/// <summary>
	/// Json出力するとき、情報をマスクする属性
	/// </summary>
	class MaskedJsonAttribute : Attribute { }
}
