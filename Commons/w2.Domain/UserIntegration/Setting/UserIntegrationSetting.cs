/*
=========================================================================================================
  Module      : ユーザー統合設定クラス(UserIntegrationSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace w2.Domain.UserIntegration.Setting
{
	#region +列挙体
	/// <summary>論理演算子区分</summary>
	public enum OperatorKbn
	{
		/// <summary>論理和</summary>
		Or,
		/// <summary>論理積</summary>
		And
	}

	/// <summary>比較区分</summary>
	public enum QryKbn
	{
		/// <summary>等しい</summary>
		Eq,
		/// <summary>○文字まで一致している</summary>
		Sw
	}
	#endregion

	#region +ユーザー統合設定クラス
	/// <summary>
	/// ユーザー統合設定クラス
	/// </summary>
	public class UserIntegrationSetting
	{
		/// <summary>設定ディレクトリ名</summary>
		public static string DIRNAME_SETTINGS = @"Settings";
		/// <summary>ベースディレクトリ名</summary>
		public static string DIRNAME_BASE = @"base";
		/// <summary>ユーザー統合設定ファイル名</summary>
		public const string FILENAME_USERINTEGRATION_SETTING = @"UserIntegrationSetting.xml";

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static UserIntegrationSetting()
		{
			// 設定更新
			UpdateSetting();
		}

		/// <summary>
		/// 設定更新
		/// </summary>
		public static void UpdateSetting()
		{
			// 設定読み込み
			try
			{
				// 案件の設定が存在しない場合ベースの設定を読込
				var filePath = Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, DIRNAME_SETTINGS, FILENAME_USERINTEGRATION_SETTING);
				if (File.Exists(filePath) == false)
				{
					filePath = Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, DIRNAME_SETTINGS, DIRNAME_BASE, FILENAME_USERINTEGRATION_SETTING);
				}
				var setting = XDocument.Load(filePath).Element("UserIntegrationSetting");
				Description = setting.Element("Description").Value;
				MatchCondition = new MatchConditionData(setting.Element("MatchCondition"));
			}
			catch (Exception ex)
			{
				w2.Common.Logger.AppLogger.WriteError("ユーザー統合設定XMLの読み込みに失敗しました。", ex);
			}
		}

		#region +プロパティ
		/// <summary>説明</summary>
		public static string Description { get; private set; }
		/// <summary>一致条件情報</summary>
		public static MatchConditionData MatchCondition { get; private set; }
		#endregion
	}

	/// <summary>
	/// 一致条件クラス
	/// </summary>
	public class MatchConditionData
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="matchCondition">一致条件要素</param>
		public MatchConditionData(XElement matchCondition)
		{
			this.LogicalOperators = new LogicalOperatorsData(matchCondition.Element("LogicalOperators"));
		}
		#endregion

		#region +メソッド
		/// <summary>
		/// 利用キーリスト取得
		/// </summary>
		/// <returns>利用キーリスト</returns>
		public string[] GetUseKeys()
		{
			var useKeys = new List<string>();
			foreach (var key in this.GetMatchList(this.LogicalOperators).Select(k => k.Key).Distinct().ToArray())
			{
				useKeys.AddRange(key.Split('+'));
			}
			return useKeys.Distinct().ToArray();
		}
		/// <summary>
		/// 利用キーリスト取得
		/// </summary>
		/// <param name="logicalOperators">論理演算子情報</param>
		/// <returns>利用キーリスト</returns>
		private IEnumerable<MatchData> GetMatchList(LogicalOperatorsData logicalOperators)
		{
			var matchList = logicalOperators.MatchList.Concat(
				logicalOperators.LogicalOperatorsList.SelectMany(GetMatchList));
			return matchList;
		}
		#endregion

		#region +プロパティ
		/// <summary>論理演算子情報</summary>
		public LogicalOperatorsData LogicalOperators { get; private set; }
		#endregion
	}

	/// <summary>
	/// 論理演算子クラス
	/// </summary>
	public class LogicalOperatorsData
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="logicalOperators">論理演算子要素</param>
		public LogicalOperatorsData(XElement logicalOperators)
		{
			// 論理演算子
			this.Operator = (OperatorKbn)Enum.Parse(typeof(OperatorKbn), logicalOperators.Attribute("Operator").Value);

			// 論理演算子リスト
			this.LogicalOperatorsList =
				logicalOperators.Elements("LogicalOperators").Select(element => new LogicalOperatorsData(element)).ToArray();

			// 一致リスト
			this.MatchList =
				logicalOperators.Elements("Match").Select(element => new MatchData(element)).ToArray();
		}
		#endregion

		#region +プロパティ
		/// <summary>論理演算子</summary>
		public OperatorKbn Operator { get; private set; }
		/// <summary>論理演算子情報リスト</summary>
		public LogicalOperatorsData[] LogicalOperatorsList { get; private set; }
		/// <summary>一致情報リスト</summary>
		public MatchData[] MatchList { get; private set; }
		#endregion
	}

	/// <summary>
	/// 一致クラス
	/// </summary>
	public class MatchData
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="match">一致要素</param>
		public MatchData(XElement match)
		{
			this.Key = match.Attribute("Key").Value;
			this.Qry = (QryKbn)Enum.Parse(typeof(QryKbn), match.Attribute("Qry").Value);
			// 前方一致の場合のみ、文字数をセット
			if ((this.Qry == QryKbn.Sw) && (match.Attribute("Length") != null))
			{
				this.Length = int.Parse(match.Attribute("Length").Value);
			}
		}
		#endregion

		#region +プロパティ
		/// <summary>キー</summary>
		public string Key { get; private set; }
		/// <summary>比較</summary>
		public QryKbn Qry { get; private set; }
		/// <summary>文字数</summary>
		public int? Length { get; private set; }
		#endregion
	}
	#endregion
}