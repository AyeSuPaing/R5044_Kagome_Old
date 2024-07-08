/*
=========================================================================================================
  Module      : 注文拡張項目 共通処理(OrderExtendCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Order.Cart;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;

namespace w2.App.Common.OrderExtend
{
	/// <summary>
	/// 注文拡張項目 共通処理
	/// </summary>
	public class OrderExtendCommon
	{
		/// <summary>
		/// 注文拡張項目 入力内容の作成
		/// </summary>
		/// <returns>入力内容</returns>
		public static Dictionary<string, CartOrderExtendItem> CreateOrderExtend()
		{
			var result =
				Constants.ORDER_EXTEND_ATTRIBUTE_FIELD_LIST.ToDictionary(v => v, v => new CartOrderExtendItem());
			return result;
		}
		/// <summary>
		/// 注文拡張項目 入力内容の作成
		/// </summary>
		/// <param name="model">注文モデル</param>
		/// <returns>入力内容</returns>
		public static Dictionary<string, CartOrderExtendItem> ConvertOrderExtend(OrderModel model)
		{
			var result = Constants.ORDER_EXTEND_ATTRIBUTE_FIELD_LIST.ToDictionary(
				v => v,
				v => new CartOrderExtendItem()
				{
					Value = (string)model.DataSource[v],
					IsFixedPurchaseTakeOverNext = false,
				});
			return result;
		}
		/// <summary>
		/// 注文拡張項目 入力内容の作成
		/// </summary>
		/// <param name="model">定期モデル</param>
		/// <returns>入力内容</returns>
		public static Dictionary<string, CartOrderExtendItem> ConvertOrderExtend(FixedPurchaseModel model)
		{
			var result = Constants.ORDER_EXTEND_ATTRIBUTE_FIELD_LIST.ToDictionary(
				v => v,
				v => new CartOrderExtendItem()
				{
					Value = (string)model.DataSource[v],
					IsFixedPurchaseTakeOverNext = string.IsNullOrEmpty((string)model.DataSource[v]),
				});
			return result;
		}
		/// <summary>
		/// 注文拡張項目 入力内容の作成
		/// </summary>
		/// <param name="drv">注文データ</param>
		/// <returns>入力内容</returns>
		public static Dictionary<string, CartOrderExtendItem> ConvertOrderExtend(DataRowView drv)
		{
			var result = Constants.ORDER_EXTEND_ATTRIBUTE_FIELD_LIST.ToDictionary(
				v => v,
				v => new CartOrderExtendItem()
				{
					Value = StringUtility.ToEmpty(drv[v]),
					IsFixedPurchaseTakeOverNext = string.IsNullOrEmpty(StringUtility.ToEmpty(drv[v])),
				});
			return result;
		}
		/// <summary>
		/// 注文拡張項目 入力内容の作成
		/// </summary>
		/// <param name="ht">注文データ</param>
		/// <returns>入力内容</returns>
		public static Dictionary<string, CartOrderExtendItem> ConvertOrderExtend(Hashtable ht)
		{
			var result = Constants.ORDER_EXTEND_ATTRIBUTE_FIELD_LIST.ToDictionary(
				v => v,
				v => new CartOrderExtendItem()
				{
					Value = StringUtility.ToEmpty(ht[v]),
					IsFixedPurchaseTakeOverNext = string.IsNullOrEmpty(StringUtility.ToEmpty(ht[v])),
				});
			return result;
		}

		/// <summary>
		/// 注文拡張項目 入力内容の作成 管理画面
		/// </summary>
		/// <returns>入力内容</returns>
		public static Dictionary<string, string> CreateOrderExtendForManager()
		{
			var orderExtend = SetDefaultInput(CreateOrderExtend());
			var result = orderExtend.ToDictionary(v => v.Key, v => v.Value.Value);
			return result;
		}
		/// <summary>
		/// 注文拡張項目 入力内容の作成 管理画面
		/// </summary>
		/// <param name="model">注文モデル</param>
		/// <returns>入力内容</returns>
		public static Dictionary<string, string> CreateOrderExtendForManager(OrderModel model)
		{
			var result = ConvertOrderExtend(model).ToDictionary(v => v.Key, v => v.Value.Value);
			return result;
		}
		/// <summary>
		/// 注文拡張項目 入力内容の作成 管理画面
		/// </summary>
		/// <param name="model">定期モデル</param>
		/// <returns>入力内容</returns>
		public static Dictionary<string, string> CreateOrderExtendForManager(FixedPurchaseModel model)
		{
			var result = ConvertOrderExtend(model).ToDictionary(v => v.Key, v => v.Value.Value);
			return result;
		}
		/// <summary>
		/// 注文拡張項目 入力内容の作成 管理画面
		/// </summary>
		/// <param name="drv">注文データ</param>
		/// <returns>入力内容</returns>
		public static Dictionary<string, string> CreateOrderExtendForManager(DataRowView drv)
		{
			var result = ConvertOrderExtend(drv).ToDictionary(v => v.Key, v => v.Value.Value);
			return result;
		}
		/// <summary>
		/// 注文拡張項目 入力内容の作成 管理画面
		/// </summary>
		/// <param name="ht">注文データ</param>
		/// <returns>入力内容</returns>
		public static Dictionary<string, string> CreateOrderExtendForManager(Hashtable ht)
		{
			var result = ConvertOrderExtend(ht).ToDictionary(v => v.Key, v => v.Value.Value);
			return result;
		}

		/// <summary>
		/// 一覧検索用 @@ user_extend_field_name @@を条件文に置換
		/// </summary>
		/// <param name="sqlStatement">SQL本文</param>
		/// <param name="name">ユーザー拡張名</param>
		/// <returns></returns>
		public static string ReplaceOrderExtendFieldName(string sqlStatement, string table, string name)
		{
			var result = sqlStatement.Replace(
				"@@ order_extend_field_name @@",
				string.Format("{0}.{1}", table, StringUtility.ToEmpty(name)));
			return result;
		}

		/// <summary>
		/// 一覧検索用 検索クエリの置換内容を取得
		/// </summary>
		/// <param name="target">置換内容</param>
		/// <param name="table">対象テーブル</param>
		/// <param name="name">対象の注文拡張項目</param>
		/// <returns>置換内容</returns>
		public static KeyValuePair<string, string>[] SetReplaceOrderExtendFieldName(
			KeyValuePair<string, string>[] target,
			string table,
			string name)
		{
			var temp = target.ToDictionary(i => i.Key, i => i.Value);

			if (temp.ContainsKey("@@ order_extend_field_name @@"))
			{
				temp["@@ order_extend_field_name @@"] = string.Format("{0}.{1}", table, StringUtility.ToEmpty(name));
			}
			else
			{
				temp.Add("@@ order_extend_field_name @@", string.Format("{0}.{1}", table, StringUtility.ToEmpty(name)));
			}

			var result = temp.Select(t => new KeyValuePair<string, string>(t.Key, t.Value)).ToArray();
			return result;
		}

		/// <summary>
		/// ワークフロー用 絞り込みクエリの作成
		/// </summary>
		/// <param name="searchValue">対象条件</param>
		/// <param name="searchField">対象フィールド</param>
		/// <param name="targetTable">対象テーブル</param>
		/// <returns>クエリ内容</returns>
		public static string CreateWhereQuerryOrderWorkflow(string searchValue, string searchField, string targetTable)
		{
			if (Constants.ORDER_EXTEND_OPTION_ENABLED == false) return string.Empty;

			var orderExtendQuery = new StringBuilder();
			if (string.IsNullOrEmpty(searchValue) == false)
			{
				var model = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels
					.FirstOrDefault(m => m.SettingId == searchField);
				var values = searchValue.Split(',');
				var appendList = new List<string>();
				if (model != null)
				{
					switch (model.InputType)
					{
						case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT:
							if (values.Length > 0)
							{
								orderExtendQuery.Append("(");
								foreach (var value in values)
								{
									switch (value)
									{
										case "1":
											appendList.Add(
												string.Format("( {0}.{1} <> '')", targetTable, model.SettingId));
											break;
										case "0":
											appendList.Add(
												string.Format("( {0}.{1} = '')", targetTable, model.SettingId));
											break;
										default:
											break;
									}
								}

								orderExtendQuery.Append(string.Join("OR", appendList));
								orderExtendQuery.Append(")");
							}

							break;

						case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
						case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO:
							if (values.Length > 0)
							{
								orderExtendQuery.Append("(");
								appendList.AddRange(
									values.Select(
										value => string.Format(
											"( {0}.{1} = '{2}')",
											targetTable,
											model.SettingId,
											value)));
								orderExtendQuery.Append(string.Join("OR", appendList));
								orderExtendQuery.Append(")");
							}

							break;

						case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
							if (values.Length > 0)
							{
								orderExtendQuery.Append("(");
								foreach (var value in values)
								{
									var temp = new StringBuilder();
									temp.Append("(");
									temp.Append(
										string.Format("({0}.{1} = '{2}')", targetTable, model.SettingId, value));
									temp.Append("OR");
									temp.Append(
										string.Format(
											"({0}.{1} LIKE '%,' + '{2}' + ',%' ESCAPE '#')",
											targetTable,
											model.SettingId,
											value));
									temp.Append("OR");
									temp.Append(
										string.Format(
											"({0}.{1} LIKE + '{2}' + ',%' ESCAPE '#')",
											targetTable,
											model.SettingId,
											value));
									temp.Append("OR");
									temp.Append(
										string.Format(
											"({0}.{1}  LIKE '%,' + '{2}' ESCAPE '#')",
											targetTable,
											model.SettingId,
											value));
									temp.Append(")");
									appendList.Add(temp.ToString());
								}

								orderExtendQuery.Append(string.Join("OR", appendList));
								orderExtendQuery.Append(")");
							}

							break;
					}
				}
			}

			return orderExtendQuery.ToString();
		}


		/// <summary>
		/// 注文拡張項目 入力内容セット
		/// </summary>
		/// <param name="targetModel">変更内容</param>
		/// <param name="orderExtendItenms">セットするカート注文拡張項目</param>
		public static void SetOrderExtend(
			FixedPurchaseModel targetModel,
			Dictionary<string, CartOrderExtendItem> orderExtendItenms)
		{
			if (Constants.ORDER_EXTEND_OPTION_ENABLED == false) return;

			foreach (var orderExtend in orderExtendItenms)
			{
				targetModel.DataSource[orderExtend.Key] = (orderExtend.Value.IsFixedPurchaseTakeOverNext)
					? orderExtend.Value.Value
					: string.Empty;
			}
		}
		/// <summary>
		/// 注文拡張項目 入力内容セット
		/// </summary>
		/// <param name="target">変更内容</param>
		/// <param name="orderExtendItenms">セットするカート注文拡張項目</param>
		public static void SetOrderExtend(Hashtable target, Dictionary<string, CartOrderExtendItem> orderExtendItenms)
		{
			if (Constants.ORDER_EXTEND_OPTION_ENABLED == false) return;

			foreach (var orderExtend in orderExtendItenms)
			{
				if (target.ContainsKey(orderExtend.Key))
				{
					target[orderExtend.Key] = orderExtend.Value.Value;
				}
				else
				{
					target.Add(orderExtend.Key, orderExtend.Value.Value);
				}
			}
		}

		/// <summary>
		/// デフォルト入力値をセット
		/// </summary>
		/// <param name="target">セット対象</param>
		/// <returns>セットされたカート 注文拡張項目の入力値</returns>
		public static Dictionary<string, CartOrderExtendItem> SetDefaultInput(
			Dictionary<string, CartOrderExtendItem> target)
		{
			var result = target;
			// テキストボックスの場合に初期値を設定
			foreach (var value in result)
			{
				var settingModel = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData
					.SettingModels.FirstOrDefault(m => m.SettingId == value.Key);

				if (settingModel == null) continue;

				switch (settingModel.InputType)
				{
					case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT:
						if (string.IsNullOrEmpty(settingModel.InputDefault)) continue;
						result[value.Key].Value = settingModel.InputDefault;
						break;

					case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO:
					case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
						var rdResult = settingModel.InputDefault.Split(';')
							.FirstOrDefault(t => CheckKeyExists(t.Split(','), "selected"));
						if (rdResult == null) continue;
						var rbValue = rdResult.Split(',');
						result[value.Key].Value = (rbValue.Length > 0) ? rbValue[0] : target[value.Key].Value;
						break;

					case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
						var cbArray = settingModel.InputDefault.Split(';')
							.Where(t => CheckKeyExists(t.Split(','), "selected")).Select(t => t.Split(','))
							.Where(t => t.Length > 0).Select(t => t[0]).ToArray();
						if (cbArray.Length == 0) continue;

						result[value.Key].Value = string.Join(",", cbArray);
						break;
				}
			}

			return result;
		}

		/// <summary>
		/// 初期値情報を取得 ※プロパティからしか呼ばない フロント用
		/// </summary>
		/// <param name="inputDefault">デフォルト値</param>
		/// <returns>テキスト：string テキスト以外：ListItemリスト</returns>
		public static List<ListItem> GetListItemForFront(string inputDefault)
		{
			return GetListItem(inputDefault, true);
		}

		/// <summary>
		/// 初期値情報を取得 ※プロパティからしか呼ばない 管理画面用
		/// </summary>
		/// <param name="inputDefault">デフォルト値</param>
		/// <returns>テキスト：string テキスト以外：ListItemリスト</returns>
		public static List<ListItem> GetListItemForManager(string inputDefault)
		{
			return GetListItem(inputDefault, false);
		}

		/// <summary>
		/// 初期値情報を取得 ※プロパティからしか呼ばない
		/// </summary>
		/// <param name="inputDefault">デフォルト値</param>
		/// <param name="isFront">Frontアプリケーションからの呼び出しか</param>
		/// <returns>テキスト：string テキスト以外：ListItemリスト</returns>
		private static List<ListItem> GetListItem(string inputDefault, bool isFront)
		{
			var listItem = new List<ListItem>();

			foreach (var keyValue in inputDefault.Split(';'))
			{
				var elements = keyValue.Split(',');
				var li = new ListItem(elements[1], elements[0]);

				// フロントサイトからの呼び出しではない または 「非表示にする」設定がなければ表示用リストへ追加
				if ((isFront == false) || (CheckKeyExists(elements, "hide") == false))
				{
					listItem.Add(li);
				}
			}

			return listItem;
		}

		/// <summary>
		/// 「選択済みにする」「非表示にする」キー設定が存在するかチェック
		/// </summary>
		/// <param name="elements">リストアイテム内の1要素</param>
		/// <param name="key">キー文字列</param>
		/// <returns>true：存在する</returns>
		private static bool CheckKeyExists(string[] elements, string key)
		{
			var result = false;
			if (elements.Length >= 3)
			{
				// 3 or 4要素目をチェックしキー（selected,hide）が存在すればTrue
				result = ((elements[2] == key) || ((elements.Length == 4) && (elements[3] == key)));
			}

			return result;
		}

		/// <summary>
		/// 表示名を取得する
		/// </summary>
		/// <param name="inputType">入力方式</param>
		/// <param name="inputDefault">デフォルト内容</param>
		/// <param name="inputValue">入力内容</param>
		/// <remarks>チェックボックスの場合は半角カンマ区切り</remarks>
		/// <returns>表示名</returns>
		public static string GetValueDisplayName(string inputType, string inputDefault, string inputValue)
		{
			if (inputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT) return inputValue;

			var text = new StringBuilder();
			foreach (var keyValue in (inputDefault).Split(';'))
			{
				var item = keyValue.Split(',');
				if (item.Length < 2) continue;

				// セミコロンで区切って格納されている場合には、それぞれ対応する表示名にして表示
				switch (inputType)
				{
					case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
						if (inputValue == item[0])
						{
							text.Append(item[1]);
						}

						break;

					case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
						if (((IList)inputValue.Split(',')).Contains(item[0]))
						{
							text.Append((text.ToString() != "") ? "," : "");
							text.Append(item[1]);
						}

						break;

					default:
						// なにもしない
						break;
				}
			}
			return text.ToString();
		}
	}
}