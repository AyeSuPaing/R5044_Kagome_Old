/*
=========================================================================================================
  Module      : 検索パラメータ(SearchParameter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Cs.Search
{
	/// <summary>
	/// 検索パラメータ
	/// </summary>
	[Serializable]
	public class SearchParameter
	{
		/// <summary>
		/// 引数で指定した文字列に対応する、検索項目列挙値を取得します。
		/// nullを指定したときはデフォルトの列挙値を取得します。
		/// </summary>
		/// <param name="value">検索項目文字列</param>
		/// <returns>検索項目列挙値</returns>
		public static SearchTarget GetSearchTarget(string value)
		{
			return (value == null) ? SearchTarget.ContentsAndHeader : (SearchTarget)Enum.Parse(typeof(SearchTarget), value);
		}

		#region プロパティ（追加条件が増えたら「HasAdditionalConditions」プロパティも改修すること）
		/// <summary>キーワード</summary>
		public string Keyword { get; set; }
		/// <summary>検索モード（すべて含む、いずれか含む、等）</summary>
		public SearchMode Mode { get; set; }
		/// <summary>検索項目</summary>
		public SearchTarget Target { get; set; }
		/// <summary>検索項目：その他メッセージ項目群</summary>
		public SearchTargetMessageColumns TargetMessageColumns { get; set; }
		/// <summary>検索項目：その他インシデント項目群</summary>
		public SearchTargetIncidentColumns TargetIncidentColumns { get; set; }
		/// <summary>絞り込み条件：タイプ（種類）</summary>
		public SearchMessageTypes MessageType;
		/// <summary>絞り込み条件：メッセージステータス</summary>
		public string MessageStatusFilter = null;
		/// <summary>絞り込み条件：ステータス</summary>
		public string StatusFilter = null;
		/// <summary>絞り込み条件：カテゴリID</summary>
		public string CategoryFilter = null;
		/// <summary>絞り込み条件：重要度</summary>
		public string ImportanceFilter = null;
		/// <summary>絞り込み条件：VOC-ID</summary>
		public string VocFilter = null;
		/// <summary>絞り込み条件：担当グループID</summary>
		public string GroupFilter = null;
		/// <summary>絞り込み条件：担当オペレータID</summary>
		public string OperatorFilter = null;
		/// <summary>絞り込み条件：集計区分値リスト</summary>
		public Dictionary<int, string> SummaryValues = null;
		/// <summary>絞り込み条件：集計区分入力種別リスト</summary>
		public Dictionary<int, string> SummarySettingTypes = null;
		/// <summary>絞り込み条件：無効な集計区分を表示するか</summary>
		public bool DisplayInvalidSummarySetting;
		/// <summary>絞り込み条件：対象期間（いつから）</summary>
		public DateTime? DateFromFilter = null;
		/// <summary>絞り込み条件：対象期間（いつまで）</summary>
		public DateTime? DateToFilter = null;
		/// <summary>絞り込み条件：対象期間の区分</summary>
		public SearchDateKbn? DateKbn = null;
		/// <summary>ゴミ箱も含めて検索</summary>
		public bool IncludeTrash;
		/// <summary>追加条件を持つか</summary>
		/// <remarks>
		/// 大量データを検索する際、パラメタライズクエリではパフォーマンスが落ちてしまうため
		/// LIKE検索以外の追加条件を行っている場合のみパラメタライズクエリを利用しないようにする
		/// </remarks>
		public bool HasAdditionalConditions
		{
			get
			{
				return (this.MessageType != SearchMessageTypes.NoSelection)
				       || (this.MessageStatusFilter != null)
				       || (this.StatusFilter != null)
				       || (this.CategoryFilter != null)
				       || (this.ImportanceFilter != null)
				       || (this.VocFilter != null)
				       || (this.GroupFilter != null)
				       || (this.OperatorFilter != null)
				       || (this.SummaryValues.Keys.Count != 0)
				       || (this.SummarySettingTypes.Keys.Count != 0)
				       || (this.DateFromFilter != null)
				       || (this.DateToFilter != null);
			}
		}
		#endregion
	}

	#region 列挙体
	/// <summary>検索モード</summary>
	public enum SearchMode
	{
		/// <summary>すべて含む</summary>
		All,
		/// <summary>いずれか含む</summary>
		Any,
		/// <summary>完全一致</summary>
		Exact,
	}

	/// <summary>検索項目</summary>
	public enum SearchTarget
	{
		/// <summary>本文＋ヘッダ / 内容＋回答＋件名＋問合せ元情報</summary>
		ContentsAndHeader,
		/// <summary>本文(Body) / 内容＋回答</summary>
		Contents,
		/// <summary>ヘッダ(From/To/Cc/Bcc/Subject) / 件名＋問合せ元情報</summary>
		Header,
		/// <summary>インシデントID</summary>
		IncidentId,
		/// <summary>その他メッセージ項目群</summary>
		MessageColumns,
		/// <summary>その他インシデント項目群</summary>
		IncidentColumns,
		/// <summary>Search Top</summary>
		SearchTop,
	}

	/// <summary>検索項目：その他メッセージ項目群</summary>
	[Flags]
	public enum SearchTargetMessageColumns
	{
		/// <summary>未選択</summary>
		NoSelection = 0,
		/// <summary>その他メッセージ項目: From</summary>
		MessageFrom = 1,
		/// <summary>その他メッセージ項目: To</summary>
		MessageTo = 2,
		/// <summary>その他メッセージ項目: Cc</summary>
		MessageCc = 4,
		/// <summary>その他メッセージ項目: Bcc</summary>
		MessageBcc = 8,
		/// <summary>その他メッセージ項目: Subject</summary>
		MessageSubject = 16,
		/// <summary>その他メッセージ項目: 氏名</summary>
		MessageUserName = 32,
		/// <summary>その他メッセージ項目: 電話番号</summary>
		MessageUserTel = 64,
		/// <summary>その他メッセージ項目: メールアドレス</summary>
		MessageUserMailAddr = 128,
		/// <summary>その他メッセージ項目: 件名</summary>
		MessageInquiryTitle = 256,
	}

	/// <summary>検索項目：その他インシデント項目群</summary>
	[Flags]
	public enum SearchTargetIncidentColumns
	{
		/// <summary>未選択</summary>
		NoSelection = 0,
		/// <summary>その他インシデント項目: タイトル</summary>
		IncidentTitle = 1,
		/// <summary>その他インシデント項目: VOCメモ</summary>
		IncidentVocMemo = 2,
		/// <summary>その他インシデント項目: 内部メモ</summary>
		IncidentComment = 4,
		/// <summary>その他インシデント項目: 問合せ元名称/連絡先</summary>
		IncidentFrom = 8,
	}

	/// <summary>メッセージタイプ</summary>
	[Flags]
	public enum SearchMessageTypes
	{
		/// <summary>未選択</summary>
		NoSelection = 0,
		/// <summary>メール受信</summary>
		Receive = 1,
		/// <summary>メール送信</summary>
		Send = 2,
		/// <summary>電話受信</summary>
		TellIn = 4,
		/// <summary>電話発信</summary>
		TellOut = 8,
		/// <summary>その他受信</summary>
		OthersIn = 16,
		/// <summary>その他発信</summary>
		OthersOut = 32,
	}

	/// <summary>対象期間の区分</summary>
	public enum SearchDateKbn
	{
		/// <summary>問合せ日（メッセージ）</summary>
		InquiryDate,
		/// <summary>最新問合せ日（メッセージ）</summary>
		LatestInquiryDate,
		/// <summary>最新更新日（メッセージ）</summary>
		MessageDateChanged,
		/// <summary>発生日（インシデント）</summary>
		IncidentDateCreated,
		/// <summary>対応完了日（インシデント）</summary>
		IncidentDateCompleted,
	}
	#endregion
}
