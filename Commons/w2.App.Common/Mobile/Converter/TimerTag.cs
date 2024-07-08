/*
=========================================================================================================
  Module      : タイマータグ変換クラス(TimerTag.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Text.RegularExpressions;
using w2.Common.Util;

namespace w2.App.Common.Mobile.Converter
{
	///*********************************************************************************************
	/// <summary>
	/// タイマータグ共通処理クラス
	/// </summary>
	///*********************************************************************************************
	public partial class TimerTag
	{
		#region 定数
		// タイマータグ：期間タグ
		private const string TAG_HEAD_TIMER_FROMTO = "<@@TimerFromTo:";
		private const string TAG_FOOT_TIMER_FROMTO = "@@>";
		private const string TAG_END_TIMER_FROMTO = "</@@TimerFromTo@@>";
		// タイマータグ：曜日タグ
		private const string TAG_HEAD_TIMER_DAYOFWEEK = "<@@TimerDayOfWeek:";
		private const string TAG_FOOT_TIMER_DAYOFWEEK = "@@>";
		private const string TAG_END_TIMER_DAYOFWEEK = "</@@TimerDayOfWeek@@>";
		// タイマータグ：変換元のフォーマット
		private const string TAG_TIMER_DATE_FORMAT = "yyyyMMddHHmmss";
		#endregion

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TimerTag()
		{
			// 何もしない
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="html">変換対象文字列</param>
		/// <param name="requestDate">比較対象日付</param>
		public TimerTag(StringBuilder html, string requestDate)
			: this()
		{
			this.TargetDate = GetTargetDate(requestDate);
			this.Html = new StringBuilder(html.ToString());
		}

		/// <summary>
		/// タイマータグを変換
		/// </summary>
		/// <returns>変換後の文字列</returns>
		public StringBuilder Convert()
		{
			ConvertFromTo();
			ConvertDayOfWeek();
			
			return this.Html;
		}

		/// <summary>
		/// 期間タグ変換
		/// </summary>
		private void ConvertFromTo()
		{
			foreach (Match matchFind in GetTagMatches(TAG_HEAD_TIMER_FROMTO, TAG_FOOT_TIMER_FROMTO))
			{
				// 属性取得
				string[] attributes = GetTagInner(matchFind.Value, TAG_HEAD_TIMER_FROMTO, TAG_FOOT_TIMER_FROMTO).Split(':');
				
				// 有効性チェック
				bool valid = CheckValidFromTo(attributes);

				// 判定結果に合わせて、データ置換
				ReplaceTimerTag(valid, matchFind.Value, TAG_END_TIMER_FROMTO);
			}
		}

		/// <summary>
		/// 曜日タグ変換
		/// </summary>
		private void ConvertDayOfWeek()
		{
			foreach (Match matchFind in GetTagMatches(TAG_HEAD_TIMER_DAYOFWEEK, TAG_FOOT_TIMER_DAYOFWEEK))
			{
				// 有効性チェック
				bool valid = false;
				foreach (string attributes in GetTagInner(matchFind.Value, TAG_HEAD_TIMER_DAYOFWEEK, TAG_FOOT_TIMER_DAYOFWEEK).Split(':'))
				{
					// 複数指定可能のためいずれか一致で有効
					if (this.TargetDate.ToString("ddd") == attributes)
					{
						valid = true;
						break;
					}
				}

				// 判定結果に合わせて、データ置換
				ReplaceTimerTag(valid, matchFind.Value, TAG_END_TIMER_DAYOFWEEK);
			}
		}

		/// <summary>
		/// 期間タグの有効性チェック
		/// </summary>
		/// <param name="attributes">属性</param>
		/// <returns>有効なタグか否か</returns>
		/// <remarks>フラグがFalseになるのは、句指定が無い場合と、期限が無効の場合のみ</remarks>
		private bool CheckValidFromTo(string[] attributes)
		{
			//------------------------------------------------------
			// FROM句
			//------------------------------------------------------
			// 指定句があればまずTrue
			bool validFrom = (attributes.Length > 0);
			// 期間From指定句があり かつ 日付指定があり かつ 日付フォーマットが正しい場合に日付チェック
			if (validFrom
				&& (attributes[0] != "")
				&& (Validator.IsDateExact(attributes[0], TAG_TIMER_DATE_FORMAT)))
			{
				validFrom = (this.TargetDate >= DateTime.ParseExact(attributes[0], TAG_TIMER_DATE_FORMAT, null));
			}
			//------------------------------------------------------
			// TO句
			//------------------------------------------------------
			// 指定句があればまずTrue
			bool validTo = (attributes.Length > 1);
			// 期間To指定句があり かつ 日付指定があり かつ 日付フォーマットが正しい場合に日付チェック
			if (validTo
				&& (attributes[1] != "")
				&& (Validator.IsDateExact(attributes[1], TAG_TIMER_DATE_FORMAT)))
			{
				validTo = (this.TargetDate < DateTime.ParseExact(attributes[1], TAG_TIMER_DATE_FORMAT, null));
			}

			return (validFrom && validTo);
		}

		/// <summary>
		/// 期間・曜日タグ変換で比較対象とする日付を取得
		/// </summary>
		/// <param name="requestDate">比較対象日付</param>
		/// <returns>日付</returns>
		/// <remarks>
		/// クエリーパラメータにテスト用日付指定がある場合、
		/// 指定した日付を比較対象とし、フォーマットが不正な場合は現在日時を返す
		/// </remarks>
		private DateTime GetTargetDate(string requestDate)
		{
			if ((StringUtility.ToEmpty(requestDate) != "") && (Validator.IsDateExact(requestDate, TAG_TIMER_DATE_FORMAT)))
			{
				return DateTime.ParseExact(requestDate, TAG_TIMER_DATE_FORMAT, null);
			}
			else
			{
				return DateTime.Now;
			}
		}

		/// <summary>
		/// 期間・曜日タグの判定結果に合わせて、データ置換
		/// </summary>
		/// <param name="tagValid">タグの有効可否</param>
		/// <param name="tagHead">開始タグ</param>
		/// <param name="tagEnd">終了タグ</param>
		private void ReplaceTimerTag(bool tagValid, string tagHead, string tagEnd)
		{
			foreach (Match matchFind in GetTagMatches(tagHead, tagEnd))
			{
				if (tagValid)
				{
					// タグ削除して内部データのみ表示
					this.Html = this.Html.Replace(matchFind.Value, matchFind.Value.Replace(tagHead, "").Replace(tagEnd, ""));
				}
				else
				{
					this.Html = this.Html.Replace(matchFind.Value, "");
				}
			}
		}

		/// <summary>
		/// タグパターンマッチコレクション取得
		/// </summary>
		/// <param name="tagHead"></param>
		/// <param name="tagFoot"></param>
		/// <returns></returns>
		private MatchCollection GetTagMatches(string tagHead, string tagFoot)
		{
			return Regex.Matches(this.Html.ToString(), tagHead + ".*?" + tagFoot, RegexOptions.Singleline | RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// タグ内部属性文字取得
		/// </summary>
		/// <param name="tagData">対象タグデータ</param>
		/// <param name="tagHead">タグ先頭</param>
		/// <param name="tagEnd">タグ終端</param>
		/// <returns>内部属性</returns>
		private string GetTagInner(string tagData, string tagHead, string tagEnd)
		{
			// 先頭 or 終端にマッチするパターン
			StringBuilder pattern = new StringBuilder("(").Append(tagHead).Append(")|(").Append(tagEnd).Append(")");

			// タグ部分を削除（大文字小文字区別しない）
			return Regex.Replace(tagData, pattern.ToString(), "", RegexOptions.IgnoreCase);
		}

		/// <summary>比較対象日付</summary>
		private DateTime TargetDate { get; set; }
		/// <summary>変換対象文字列</summary>
		// HACK: StringBuilderからstringに変えて戻り値を設けるのが理想
		private StringBuilder Html { get; set; }
	}
}
