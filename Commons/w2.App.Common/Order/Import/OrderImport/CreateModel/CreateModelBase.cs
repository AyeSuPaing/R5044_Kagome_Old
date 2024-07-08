/*
=========================================================================================================
  Module      : モデル作成基底クラス(CreateModelBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Domain;

namespace w2.App.Common.Order.Import.OrderImport.CreateModel
{
	/// <summary>
	/// モデル作成基底
	/// </summary>
	public abstract class CreateModelBase
	{
		/// <summary>日付型リスト</summary>
		protected List<string> m_FieldsDatetime = new List<string>();
		/// <summary>Int型リスト</summary>
		protected List<string> m_FieldsInt = new List<string>();
		/// <summary>Decimal型リスト</summary>
		protected List<string> m_FieldsDecimal = new List<string>();
		/// <summary>日付型(Null許可)リスト</summary>
		protected List<string> m_FieldsDatetimeNullable = new List<string>();
		/// <summary>Int型(Null許可)リスト</summary>
		protected List<string> m_FieldsIntNullable = new List<string>();
		/// <summary>Decimal型(Null許可)リスト</summary>
		protected List<string> m_FieldsDecimalNullable = new List<string>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected CreateModelBase()
		{
			// リスト作成
			CreateListDatetime();
			CreateListDatetimeNullable();
			CreateListDecimal();
			CreateListDecimalNullable();
			CreateListInt();
			CreateListIntNullable();
		}

		/// <summary>
		/// 日付型リスト作成
		/// </summary>
		internal abstract void CreateListDatetime();

		/// <summary>
		/// 日付型(Null許可)リスト作成
		/// </summary>
		internal abstract void CreateListDatetimeNullable();

		/// <summary>
		/// Decimal型リスト作成
		/// </summary>
		internal abstract void CreateListDecimal();

		/// <summary>
		/// Decimal型(Null許可)リスト作成
		/// </summary>
		internal abstract void CreateListDecimalNullable();

		/// <summary>
		/// Int型リスト作成
		/// </summary>
		internal abstract void CreateListInt();

		/// <summary>
		/// Int型(Null許可)リスト作成
		/// </summary>
		internal abstract void CreateListIntNullable();

		/// <summary>
		/// 値セット
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="order">注文データ</param>
		public void SetData(IModel model, Hashtable order)
		{
			foreach (DictionaryEntry item in order.Cast<DictionaryEntry>()
				.Where(item => model.DataSource.ContainsKey(item.Key)))
			{
				if (m_FieldsDatetime.Contains(item.Key))
				{
					SetDateTime(model, item);
				}
				else if (m_FieldsInt.Contains(item.Key))
				{
					SetInt(model, item);
				}
				else if (m_FieldsDecimal.Contains(item.Key))
				{
					SetDecimal(model, item);
				}
				else if (m_FieldsDatetimeNullable.Contains(item.Key))
				{
					SetDateTime(model, item, true);
				}
				else if (m_FieldsIntNullable.Contains(item.Key))
				{
					SetInt(model, item, true);
				}
				else if (m_FieldsDecimalNullable.Contains(item.Key))
				{
					SetDecimal(model, item, true);
				}
				else
				{
					model.DataSource[item.Key] = (string)item.Value;
				}
			}
		}

		/// <summary>
		/// 日付型セット
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="item">注文データ</param>
		/// <param name="isNullable">Null許可</param>
		private void SetDateTime(IModel model, DictionaryEntry item, bool isNullable = false)
		{
			DateTime tmpDate;
			if (DateTime.TryParse((string)item.Value, out tmpDate))
			{
				model.DataSource[item.Key] = tmpDate;
			}
			else if (isNullable)
			{
				model.DataSource[item.Key] = null;
			}
		}

		/// <summary>
		/// Int型セット
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="item">注文データ</param>
		/// <param name="isNullable">Null許可</param>
		private void SetInt(IModel model, DictionaryEntry item, bool isNullable = false)
		{
			int tmpInt;
			if (int.TryParse((string)item.Value, out tmpInt))
			{
				model.DataSource[item.Key] = tmpInt;
			}
			else if (isNullable)
			{
				model.DataSource[item.Key] = null;
			}
		}

		/// <summary>
		/// Decimal型セット
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="item">注文データ</param>
		/// <param name="isNullable">Null許可</param>
		private void SetDecimal(IModel model, DictionaryEntry item, bool isNullable = false)
		{
			decimal tmpDecimal;
			if (decimal.TryParse((string)item.Value, out tmpDecimal))
			{
				model.DataSource[item.Key] = tmpDecimal;
			}
			else if (isNullable)
			{
				model.DataSource[item.Key] = null;
			}
		}
	}
}
