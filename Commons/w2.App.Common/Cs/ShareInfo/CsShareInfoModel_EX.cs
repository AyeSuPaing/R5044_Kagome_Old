/*
=========================================================================================================
  Module      : 共有情報モデルのパーシャルクラス(CsShareInfoModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.Common.Util;

namespace w2.App.Common.Cs.ShareInfo
{
	/// <summary>
	/// 共有情報モデルのパーシャルクラス
	/// </summary>
	public partial class CsShareInfoModel : ModelBase<CsShareInfoModel>
	{
		/// <summary>共有情報テキスト区分名</summary>
		public string EX_InfoTextKbnName
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSSHAREINFO, Constants.FIELD_CSSHAREINFO_INFO_TEXT_KBN, this.InfoTextKbn); }
		}
		/// <summary>共有情報テキスト（区分がHTMLのときはHTMLタグ除外）</summary>
		public string EX_InfoTextExceptHtmlTag
		{
			get { return (this.InfoTextKbn == Constants.FLG_CSSHAREINFO_INFO_TEXT_KBN_HTML) ? Regex.Replace(this.InfoText, "<(\"[^\"]*\"|'[^']*'|[^'\">])*>", "") : this.InfoText; }
		}
		/// <summary>共有情報区分名</summary>
		public string EX_InfoKbnName
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSSHAREINFO, Constants.FIELD_CSSHAREINFO_INFO_KBN, this.InfoKbn); }
		}
		/// <summary>共有情報重要度名</summary>
		public string EX_InfoImportanceName
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSSHAREINFO, Constants.FIELD_CSSHAREINFO_INFO_IMPORTANCE, this.InfoImportance); }
		}
		/// <summary>送信元オペレータ名</summary>
		public string EX_SenderName
		{
			get
			{
				var sender = (this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME] != DBNull.Value)
					? StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME])
					: ValueText.GetValueText(Constants.TABLE_CSSHAREINFO, Constants.FIELD_CSSHAREINFO_SENDER, "");
				return sender;
			}
			set { this.DataSource[Constants.FIELD_SHOPOPERATOR_NAME] = value; }
		}
		/// <summary>既読フラグ（オペレータ向け画面で利用）</summary>
		public string EX_ReadFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFOREAD_READ_FLG]); }
		}
		/// <summary>ピンフラグ（オペレータ向け画面で利用）</summary>
		public string EX_PinnedFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSHAREINFOREAD_PINNED_FLG]); }
		}
		/// <summary>ヒット件数（作成画面で利用）</summary>
		public int EX_RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
		}
		/// <summary>共有件数（作成画面で利用）</summary>
		public int EX_ShareCount
		{
			get { return (int)StringUtility.ToValue(this.DataSource["share_count"], 0); }
		}
		/// <summary>既読件数（作成画面で利用）</summary>
		public int EX_ReadCount
		{
			get { return (int)StringUtility.ToValue(this.DataSource["read_count"], 0); }
		}
		/// <summary>既読状況</summary>
		public string EX_ReadRateString
		{
			get { return (this.EX_ShareCount == 0) ? "100" : (this.EX_ReadCount * 100 / this.EX_ShareCount).ToString(); }
		}
	}
}
