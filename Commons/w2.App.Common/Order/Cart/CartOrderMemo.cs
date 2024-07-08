/*
=========================================================================================================
  Module      : カート注文メモ情報クラス(CartOrderMemo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Application : w2.Commerce.Common


  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// 注文メモクラス
	/// </summary>
	///*********************************************************************************************
	[Serializable]
	public class CartOrderMemo : ICloneable
	{
		//------------------------------------------------------
		// プロパティ名と同一にする事
		//------------------------------------------------------
		// 注文メモID
		public const string FIELD_ORDER_MEMO_ID = "OrderMemoID";
		// 注文メモ名称
		public const string FIELD_ORDER_MEMO_NAME = "OrderMemoName";
		// CSS class
		public const string FIELD_ORDER_MEMO_CSS = "Class";
		// 既定値文字列
		public const string FIELD_ORDER_MEMO_DEFAULT_TEXT = "DefaultText";
		// 入力文字列
		public const string FIELD_ORDER_MEMO_TEXT = "InputText";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drvOrderMemoSetting">注文メモ設定</param>
		public CartOrderMemo(DataRowView drvOrderMemoSetting, string beforeTranslationOrderMemoName = null)
			: this((string)drvOrderMemoSetting[Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID],
				(string)drvOrderMemoSetting[Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_NAME],
				(drvOrderMemoSetting[Constants.FIELD_ORDERMEMOSETTING_HEIGHT] != DBNull.Value) ? (int?)drvOrderMemoSetting[Constants.FIELD_ORDERMEMOSETTING_HEIGHT] : null,
				(drvOrderMemoSetting[Constants.FIELD_ORDERMEMOSETTING_WIDTH] != DBNull.Value) ? (int?)drvOrderMemoSetting[Constants.FIELD_ORDERMEMOSETTING_WIDTH] : null,
				(string)drvOrderMemoSetting[Constants.FIELD_ORDERMEMOSETTING_CSS_CLASS],
				(int)drvOrderMemoSetting[Constants.FIELD_ORDERMEMOSETTING_MAX_LENGTH],
				(string)drvOrderMemoSetting[Constants.FIELD_ORDERMEMOSETTING_DEFAULT_TEXT],
				(string)drvOrderMemoSetting[Constants.FIELD_ORDERMEMOSETTING_DEFAULT_TEXT],
				beforeTranslationOrderMemoName)
		{
			// なにもしない //
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strOrderMemoId">注文メモID</param>
		/// <param name="strMemoName">注文メモ名称</param>
		/// <param name="iHeight">高さ</param>
		/// <param name="iWidth">横幅</param>
		/// <param name="strCss">Css Class</param>
		/// <param name="iMaxLength">入力可能最大文字数</param>
		/// <param name="strDefaultText">既定文字列</param>
		/// <param name="strInputText">入力テキスト</param>
		public CartOrderMemo(
			string strOrderMemoId,
			string strMemoName,
			int? iHeight,
			int? iWidth,
			string strCss,
			int iMaxLength,
			string strDefaultText,
			string strInputText,
			string beforeTranslationOrderMemoName = null)
		{
			this.OrderMemoID = strOrderMemoId;
			this.OrderMemoName = strMemoName;
			this.Height = iHeight;
			this.Width = iWidth;
			this.Class = strCss;
			this.MaxLength = iMaxLength;
			this.DefaultText = strDefaultText;
			this.InputText = strInputText;
			this.BeforeTranslationOrderMemoName = beforeTranslationOrderMemoName;
		}

		/// <summary>
		/// クローン（入力文値は初期化されます）
		/// </summary>
		/// <returns>クローン</returns>
		public object Clone()
		{
			return new CartOrderMemo(
				this.OrderMemoID,
				this.OrderMemoName,
				this.Height,
				this.Width,
				this.Class,
				this.MaxLength,
				this.DefaultText,
				(this.DefaultText == this.InputText) ? this.DefaultText : "",
				this.BeforeTranslationOrderMemoName);
		}

		/// <summary>注文メモID</summary>
		public string OrderMemoID { get; set; }
		/// <summary>注文メモ名称</summary>
		public string OrderMemoName { get; set; }
		/// <summary>入力欄高さ</summary>
		public int? Height { get; set; }
		/// <summary>入力欄幅</summary>
		public int? Width { get; set; }
		/// <summary>CSS class</summary>
		public string Class { get; set; }
		/// <summary>最大桁数</summary>
		public int MaxLength { get; set; }
		/// <summary>デフォルト文字列</summary>
		public string DefaultText { get; set; }
		/// <summary>入力文字列</summary>
		public string InputText { get; set; }
		/// <summary>翻訳前注文メモ名称</summary>
		public string BeforeTranslationOrderMemoName { get; set; }
	}
}