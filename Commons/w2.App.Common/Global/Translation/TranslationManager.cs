/*
=========================================================================================================
  Module      : 翻訳管理クラス(TranslationManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Config;
using w2.App.Common.Global.Translation.Google;
using w2.Common.Logger;
using System.Threading.Tasks;
using System.Security.Cryptography;
using w2.Domain.AutoTranslationWord;
using w2.App.Common.RefreshFileManager;

namespace w2.App.Common.Global.Translation
{
	/// <summary>
	/// 翻訳管理クラス
	/// </summary>
	public class TranslationManager
	{
		/// <summary>
		/// テキストを翻訳する処理
		/// </summary>
		/// <param name="text">翻訳する文</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>翻訳後の文</returns>
		public string TranslationApi(string text, string languageCode, string lastChanged = "")
		{
			if (string.IsNullOrEmpty(text) || (GlobalConfigUtil.GlobalTranslationEnabled() == false)) return text;

			var translatedText = "";
			var wordHashKey = WordHash(text);
			var model = DataCacheControllerFacade.GetAutoTranslationWordCacheController().GetAutoTranslationWordModel(wordHashKey, languageCode);

			if (model == null)
			{
				switch (GlobalConfigUtil.GetTranslation().TranslationApiKbn)
				{
					// Google
					case Constants.AUTO_TRANSLATION_API_KBN_GOOGLE:
						translatedText = new GoogleTranslation().Translation(text, languageCode, lastChanged);
						break;
				}
			}
			else
			{
				translatedText = model.WordAfter;
			}

			if (StackUsedWords.Any(m => (m.WordHashKey == wordHashKey) && (m.LanguageCode == languageCode))) return translatedText;

			// 翻訳ワードの利用履歴をスタック
			var stackUsedWords = new AutoTranslationWordModel()
			{
				WordHashKey = wordHashKey,
				LanguageCode = languageCode
			};
			ControlStackUsedWords(stackUsedWords);

			return translatedText;
		}

		/// <summary>
		/// 定期処理スレッドの起動
		/// </summary>
		public static void WorkerRun()
		{
			if (GlobalConfigUtil.GlobalTranslationEnabled() == false) return;

			FileLogger.WriteInfo("自動翻訳APIを開始。1分間隔で翻訳ワードの登録及び利用状況の更新を行います。");
			StackInsertWords = new List<AutoTranslationWordModel>();
			StackUsedWords = new List<AutoTranslationWordModel>();
			Task.Run(() => Worker());
		}

		/// <summary>
		/// 定期処理スレッド
		/// </summary>
		private static async void Worker()
		{
			while (true)
			{
				Work();
				// 1分間隔で実行 元スレッドには戻さない
				await Task.Delay(60 * 1000).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// 定期処理スレッドの処理内容
		/// ・登録ワードスタックのインサート
		/// ・利用ワードスタックの利用状況アップデート
		/// </summary>
		public static void Work()
		{
			var workStackInsertWords = new List<AutoTranslationWordModel>(StackInsertWords);
			ControlStackInsertWords(null, true);

			var workStackUsedWords = new List<AutoTranslationWordModel>(StackUsedWords);
			ControlStackUsedWords(null, true);

			try
			{
				InsertWordModel(workStackInsertWords);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("登録ワードスタックのインサートに失敗しました。", ex);
			}

			try
			{
				UpdateUsedWordModel(workStackUsedWords);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("利用ワードスタックの利用状況アップデートに失敗しました。", ex);
			}
		}

		/// <summary>
		/// 登録ワードスタックのインサート
		/// </summary>
		/// <param name="words">登録ワードスタック</param>
		private static void InsertWordModel(List<AutoTranslationWordModel> words)
		{
			// 同一(ワード、言語)は除外
			var wordModels = words.Distinct(new AutoTranslationWordModelComparer()).ToArray();

			if (wordModels.Length == 0) return;

			var service = new AutoTranslationWordService();

			foreach (var model in wordModels)
			{
				service.Insert(model);
			}

			// リフレファイル更新(キャッシュの再生成)
			RefreshFileManagerProvider.GetInstance(RefreshFileType.AutoTranslationWord).CreateUpdateRefreshFile();
		}

		/// <summary>
		/// 利用ワードスタックの利用状況アップデート
		/// </summary>
		/// <param name="words">利用ワードスタック</param>
		private static void UpdateUsedWordModel(List<AutoTranslationWordModel> words)
		{
			// 同一(ワード、言語)は除外
			var wordModels = words.Distinct(new AutoTranslationWordModelComparer()).ToArray();

			if (wordModels.Length == 0) return;

			new AutoTranslationWordService().UsedUpdate(wordModels.ToArray());
		}

		/// <summary>
		/// 翻訳ワードのリスト内重複チェック用モデル
		/// </summary>
		private class AutoTranslationWordModelComparer : IEqualityComparer<AutoTranslationWordModel>
		{
			/// <summary>
			/// 比較
			/// </summary>
			/// <param name="frontModel">前方モデル</param>
			/// <param name="backModel">後方モデル</param>
			/// <returns>一致:true 不一致:false</returns>
			public bool Equals(AutoTranslationWordModel frontModel, AutoTranslationWordModel backModel)
			{
				var result = (frontModel.WordHashKey == backModel.WordHashKey) && (frontModel.LanguageCode == backModel.LanguageCode);
				return result;
			}

			/// <summary>
			/// モデルのハッシュ化
			/// </summary>
			/// <param name="obj">対象モデル</param>
			/// <returns>ハッシュ化したモデル</returns>
			public int GetHashCode(AutoTranslationWordModel obj)
			{
				var hashWordHashKey = (obj.WordHashKey == null) ? 0 : obj.WordHashKey.GetHashCode();
				var hashLanguageCode = (obj.LanguageCode == null) ? 0 : obj.LanguageCode.GetHashCode();
				var result = hashWordHashKey ^ hashLanguageCode;
				return result;
			}
		}

		/// <summary>
		/// ワードのハッシュ化
		/// sha256形式
		/// </summary>
		/// <param name="word">ハッシュ化するワード</param>
		/// <returns>ハッシュ化したワード</returns>
		public static String WordHash(string word)
		{
			var hasher = new SHA256CryptoServiceProvider();
			var bytes = Encoding.Unicode.GetBytes(word);
			var hashText = string.Join(string.Empty, hasher.ComputeHash(bytes).Select(item => item.ToString("X2")));
			return hashText;
		}

		/// <summary>
		/// 登録ワードスタックの操作
		/// スレッドセーフ
		/// ・スタックへモデル追加
		/// ・スタックのモデルを全削除
		/// </summary>
		/// <param name="addModel">登録ワードスタックに追加するモデル</param>
		/// <param name="allClearFlg">登録ワードスタックの全削除</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void ControlStackInsertWords(AutoTranslationWordModel addModel = null, bool allClearFlg = false)
		{
			if (allClearFlg) StackInsertWords.Clear();

			if (addModel != null) StackInsertWords.Add(addModel);
		}

		/// <summary>
		/// 利用ワードスタックスタックの操作
		/// スレッドセーフ
		/// ・スタックへモデル追加
		/// ・スタックのモデルを全削除
		/// </summary>
		/// <param name="addModel">利用ワードスタックスタックに追加するモデル</param>
		/// <param name="allClearFlg">利用ワードスタックスタックの全削除</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void ControlStackUsedWords(AutoTranslationWordModel addModel = null, bool allClearFlg = false)
		{
			if (allClearFlg) StackUsedWords.Clear();

			if (addModel != null) StackUsedWords.Add(addModel);
		}

		/// <summary>登録ワードスタック</summary>
		public static List<AutoTranslationWordModel> StackInsertWords { get; set; }
		/// <summary>利用ワードスタック</summary>
		public static List<AutoTranslationWordModel> StackUsedWords { get; set; }
	}
}
