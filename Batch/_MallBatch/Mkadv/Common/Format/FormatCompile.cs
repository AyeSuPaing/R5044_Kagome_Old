/*
=========================================================================================================
  Module      : フォーマット変換・保持クラス(FormatCompile.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Option;
using w2.Common.Util;
using w2.Common.Logger;
using w2.Commerce.MallBatch.Mkadv.Common.ProductInfo;
using w2.Domain.ProductTaxCategory;

namespace w2.Commerce.MallBatch.Mkadv.Common.Format
{
	///**************************************************************************************
	/// <summary>
	/// フォーマット変換・保持クラス（クラス名変更：ColumnFormat.cs → FormatCompile.cs）
	/// </summary>
	///**************************************************************************************
	public class FormatCompile
	{
		List<FormatCommand> m_lCompiledFormats = new List<FormatCommand>(); // コンパイル済みフォーマット格納リスト

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strFormat">出力フォーマット</param>
		/// <remarks>出力フォーマットをコンパイルする</remarks>
		public FormatCompile(string strFormat)
		{
			InitFormatCompile(strFormat);
		}

		/// <summary>
		/// 出力フォーマットをコンパイルする
		/// </summary>
		/// <param name="strFormat">出力フォーマット</param>
		/// <remarks>
		/// FormatCommandクラスにコンパイル済みフォーマットを設定し、メンバ変数のリストに格納する
		/// </remarks>
		private void InitFormatCompile(string strFormat)
		{
			//------------------------------------------------------
			// 入力チェック
			//------------------------------------------------------
			string strInputCheckRegex = "^((((\\?=[a-zA-Z0-9]+(\\(\\?=[^\\(\\)]+,([^\\(\\)]+,)?[^\\(\\)]+\\))+(\\([^\\(\\),]+\\)))|([^\\?]+))(\"[^\"]*\"))*((\\?=[a-zA-Z0-9]+(\\(\\?=[^\\(\\)]+,([^\\(\\)]+,)?[^\\(\\)]+\\))+(\\([^\\(\\),]+\\)))|([^\\?]+))(\"[^\"]*\")?)|(((\"[^\"]*\")((\\?=[a-zA-Z0-9]+(\\(\\?=[^\\(\\)]+,([^\\(\\)]+,)?[^\\(\\)]+\\))+(\\([^\\(\\),]+\\)))|([^\\?]+)))*(\"[^\"]*\")((\\?=[a-zA-Z0-9]+(\\(\\?=[^\\(\\)]+,([^\\(\\)]+,)?[^\\(\\)]+\\))+(\\([^\\(\\),]+\\)))|([^\\?]+))?)$";
			if (Regex.Match(strFormat, strInputCheckRegex) == null)
			{
				// 正しい文法でなければ例外エラーとする
				throw new CompileException("文法違反");
			}

			//------------------------------------------------------
			// 切捨て処理抽出
			//------------------------------------------------------
			int iByteLengthLimit = strFormat.IndexOf(":LENB");
			int iIndexLengthLimit = strFormat.IndexOf(":LEN");
			if (iByteLengthLimit != -1)
			{
				// バイト長指定を取り出し、残りの出力フォーマットをセット
				int iByteLimit;
				if (int.TryParse(strFormat.Substring(iByteLengthLimit + 5, strFormat.Length - (iByteLengthLimit + 5)), out iByteLimit))
				{
					this.ByteLimit = iByteLimit;
				}
				this.Format = strFormat.Substring(0, iByteLengthLimit);
			}
			else if (iIndexLengthLimit != -1)
			{
				// 文字列長指定を取り出し、残りの出力フォーマットをセット
				int iLengthLimit;
				if (int.TryParse(strFormat.Substring(iIndexLengthLimit + 4, strFormat.Length - (iIndexLengthLimit + 4)), out iLengthLimit))
				{
					this.LengthLimit = iLengthLimit;
				}
				this.Format = strFormat.Substring(0, iIndexLengthLimit);
			}
			else
			{
				// 出力フォーマットをセット
				this.Format = strFormat;
			}

			//------------------------------------------------------
			// 出力フォーマットコンパイル
			//------------------------------------------------------
			char[] cFormats = this.Format.ToCharArray();
			bool blCommandMode = true; // 命令モード＝true/固定文字列モード=false 
			int iStartIndex = 0;
			for (int iFormatPos = 0; cFormats.Length > iFormatPos; iFormatPos++)
			{
				if (blCommandMode)
				{
					//------------------------------------------------------
					// 命令モード
					//------------------------------------------------------
					switch (cFormats[iFormatPos])
					{
						// ダブルクォートでコマンド⇔固定文字列を切り替え
						case '"':
							iStartIndex = iFormatPos + 1;
							blCommandMode = (blCommandMode == false);
							continue;

						// 条件分岐
						case '?':
							// 次の文字に処理を進める
							iFormatPos++;

							// かならず ?= になる
							if ((cFormats.Length <= iFormatPos) || (cFormats[iFormatPos] != '='))
							{
								// コンパイル例外をスロー
								throw new CompileException("書式は?=で始まる必要があります。");
							}

							// 次の文字に処理を進める
							iFormatPos++;

							// ( の出現までを抽出、キーにする
							iStartIndex = iFormatPos;
							iFormatPos = GetCharKeyInFormats(cFormats, '(', iStartIndex);
							string strKey = this.Format.Substring(iStartIndex, iFormatPos - iStartIndex);
							iFormatPos++;

							// ( から )までがひとつの命令
							// ?[演算子]即値,v1(,v2)
							CommandType commandType = CommandType.Eob;
							while ((cFormats.Length > iFormatPos) && (cFormats[iFormatPos] != '"'))
							{
								// ()の中を判定
								while ((cFormats.Length > iFormatPos) && ((cFormats[iFormatPos] == '(') || (cFormats[iFormatPos] == ')') || ((this.Format.Substring(iFormatPos, 1).Trim().Length == 0))))
								{
									iFormatPos++;
								}
								if (cFormats.Length <= iFormatPos)
								{
									break;
								}

								// ()の中に固定値があるのでクォートを処理しない。
								if (cFormats[iFormatPos] == '?')
								{
									// 次の文字に処理を進める
									iFormatPos++;
									iStartIndex = iFormatPos;

									// 比較命令
									if (cFormats.Length <= iFormatPos)
									{
										throw new CompileException("不正な命令が入力されました");
									}

									switch (cFormats[iFormatPos])
									{
										case '>':
											if (cFormats[iFormatPos + 1] == '=')
											{
												// 大なりイコール
												iFormatPos += 2;
												commandType = CommandType.BiggerEqual;
											}
											else
											{
												// 大なり
												iFormatPos++;
												commandType = CommandType.Bigger;
											}
											break;

										case '<':
											if (cFormats[iFormatPos + 1] == '=')
											{
												// 小なりイコール
												iFormatPos += 2;
												commandType = CommandType.SmallerEqual;
											}
											else
											{
												// 小なり
												iFormatPos++;
												commandType = CommandType.Smaller;
											}
											break;

										case '=':
											// 一致
											iFormatPos++;
											commandType = CommandType.Equal;
											break;

										case '!':
											// 不一致
											iFormatPos++;
											commandType = CommandType.NotEqual;
											break;

										case '~':
											// 範囲
											iFormatPos++;
											commandType = CommandType.Range;
											break;

										default:
											// 不正値
											commandType = CommandType.Eob;
											break;
									}
									iStartIndex = iFormatPos;
									iFormatPos++;

									// カンマまで取得

									iFormatPos = GetCharKeyInFormats(cFormats, ',', iStartIndex);
									string strArg1 = this.Format.Substring(iStartIndex, iFormatPos - iStartIndex);

									// 引数を確保
									if (commandType == CommandType.Range)
									{
										// 第二引数を取得
										iStartIndex = ++iFormatPos;
										iFormatPos = GetCharKeyInFormats(cFormats, ',', iStartIndex);
										string strArg2 = this.Format.Substring(iStartIndex, iFormatPos - iStartIndex);

										// 値を取得 ) まで
										iStartIndex = ++iFormatPos;
										iFormatPos = GetCharKeyInFormats(cFormats, ')', iStartIndex);
										m_lCompiledFormats.Add(new FormatCommand(commandType, strKey, strArg1, strArg2, this.Format.Substring(iStartIndex, iFormatPos - iStartIndex)));
									}
									else
									{
										// 値を取得 ) まで
										iStartIndex = ++iFormatPos;
										iFormatPos = GetCharKeyInFormats(cFormats, ')', iStartIndex);
										m_lCompiledFormats.Add(new FormatCommand(commandType, strKey, strArg1, this.Format.Substring(iStartIndex, iFormatPos - iStartIndex)));
									}
								}
								else
								{
									// 即値命令
									iStartIndex = iFormatPos;
									iFormatPos = GetCharKeyInFormats(cFormats, ')', iStartIndex);
									m_lCompiledFormats.Add(new FormatCommand(CommandType.StaticString, this.Format.Substring(iStartIndex, iFormatPos - iStartIndex)));
								}
							}
							// 終端命令を追加
							m_lCompiledFormats.Add(new FormatCommand(CommandType.Eob));
							iFormatPos--;
							break;

						case '[':
							// 開始位置を記憶
							iStartIndex = iFormatPos;

							// 次の大閉じ括弧までをキーに出力する
							iFormatPos = GetCharKeyInFormats(cFormats, ']', iStartIndex);
							string strTagKey = this.Format.Substring(iStartIndex, iFormatPos - (iStartIndex - 1));

							// キーをそのまま出力する命令を追加
							m_lCompiledFormats.Add(new FormatCommand(CommandType.Tag, strTagKey));

							// 終端命令を追加
							m_lCompiledFormats.Add(new FormatCommand(CommandType.Eob));

							// Yahoo特殊タグが必要かチェック
							if (this.IsNeedYhoVariationInfo == false)
							{
								this.IsNeedYhoVariationInfo = (strTagKey.Replace("[SP:", "").Replace("]", "").StartsWith("Yahoo"));
							}
							// 楽天カテゴリが必要かチェック
							if (this.IsNeedRtnCategoryInfo == false)
							{
								this.IsNeedRtnCategoryInfo = (strTagKey.Replace("[SP:", "").Replace("]", "").StartsWith("RakutenCategory"));
							}
							blCommandMode = (blCommandMode == false);
							iStartIndex = iFormatPos + 1;
							break;

						// 条件分岐なし
						default:
							// 開始位置を記憶
							iStartIndex = iFormatPos;

							// 次のダブルクォートか文字列の終端までをキーに出力する
							iFormatPos = GetCharKeyInFormats(cFormats, '"', iStartIndex);

							// キーをそのまま出力する命令を追加
							m_lCompiledFormats.Add(new FormatCommand(CommandType.Data, this.Format.Substring(iStartIndex, iFormatPos - iStartIndex)));

							// 終端命令を追加
							m_lCompiledFormats.Add(new FormatCommand(CommandType.Eob));
							blCommandMode = (blCommandMode = false);
							iStartIndex = iFormatPos + 1;
							break;
					}
				}
				else
				{
					//------------------------------------------------------
					// 固定文字列モード
					//------------------------------------------------------
					switch (cFormats[iFormatPos])
					{
						// ダブルクォートでコマンド⇔固定文字列を切り替え
						case '"':
							// 固定文字列命令を追加＊
							m_lCompiledFormats.Add(new FormatCommand(CommandType.StaticString, this.Format.Substring(iStartIndex, iFormatPos - iStartIndex)));
							((FormatCommand)m_lCompiledFormats[m_lCompiledFormats.Count - 1]).IsFlgKey = false;
							m_lCompiledFormats.Add(new FormatCommand(CommandType.Eob));
							blCommandMode = (blCommandMode == false);
							break;

						default:
							break;
					}
				}
			}

			// 終端命令を追加
			m_lCompiledFormats.Add(new FormatCommand(CommandType.Eob));
		}

		/// <summary>
		/// 指定文字までのキーか文字列の終端までのキーを取得する
		/// </summary>
		/// <param name="cFormats">文字列</param>
		/// <param name="cStr">検索文字</param>
		/// <returns>キー</returns>
		private int GetCharKeyInFormats(char[] cFormats, char cStr, int iStartIndex)
		{
			int iPos = new List<char>(cFormats).IndexOf(cStr, iStartIndex);
			return ((iPos != -1) ? iPos : cFormats.Length);
		}

		/// <summary>
		/// 商品データからコンパイル済みフォーマットの文字列を得る
		/// </summary>
		/// <param name="htProduct">商品レコード</param>
		/// <param name="yahooVariation">Yahoo!商品バリエーション</param>
		/// <param name="strMallId">モールID</param>
		/// <returns>フォーマット済み文字文字列</returns>
		public string GetFormatedString(
			Hashtable htProduct,
			YahooVariation yahooVariation,
			string strMallId)
		{
			StringBuilder sbResult = new StringBuilder();
			bool blSkipFlag = false; // 条件に合致したらtrueをセットし、次のeobまで読み飛ばしてfalseに戻す

			//------------------------------------------------------
			// フォーマット済み文字列取得処理
			//------------------------------------------------------
			foreach (FormatCommand formatCommand in m_lCompiledFormats)
			{
				//------------------------------------------------------
				// 読み飛ばし処理
				//------------------------------------------------------
				if (blSkipFlag)
				{
					if (formatCommand.CommandType == CommandType.Eob)
					{
						blSkipFlag = false;
					}
					continue;
				}

				//------------------------------------------------------
				// フォーマットコマンド別文字列取得処理
				//------------------------------------------------------
				switch (formatCommand.CommandType)
				{
					// 固定文字列
					case CommandType.StaticString:
						sbResult.Append(GetFormatString(formatCommand, htProduct));
						break;

					// 対応するデータを出力
					case CommandType.Data:
						sbResult.Append(htProduct[formatCommand.KeyName]);
						break;

					// 対応するデータを出力（特殊タグ）
					case CommandType.Tag:
						Regex regex = new Regex("\\(.*\\)");
						Match match = regex.Match(formatCommand.KeyName);
						if (match.Success)
						{
							sbResult.Append(SpTagCommand(
												regex.Replace(formatCommand.KeyName, ""),
												htProduct,
												yahooVariation,
												strMallId,
												match.Value));
						}
						else
						{
							sbResult.Append(SpTagCommand(
												formatCommand.KeyName,
												htProduct,
												yahooVariation,
												strMallId,
												""));
						}
						break;

					case CommandType.Eob:
						break;

					default:
						//------------------------------------------------------
						// 数値チェック
						//------------------------------------------------------
						bool blIsCastable = true;

						// 数値チェック（コマンド側、引数１）
						int iParsedArg1 = 0;
						if (htProduct.Contains(formatCommand.Arg1))
						{
							if (Int32.TryParse(StringUtility.ToEmpty(htProduct[formatCommand.Arg1]), out iParsedArg1) == false)
							{
								blIsCastable = false;
							}
						}
						else
						{
							if (Int32.TryParse(formatCommand.Arg1, out iParsedArg1) == false)
							{
								blIsCastable = false;
							}
						}

						// 数値チェック（コマンド側、引数２）
						int iParsedArg2 = 0;
						if (formatCommand.CommandType == CommandType.Range)
						{
							// 変換に成功しなければならない（エラーとして読み飛ばす）
							if (Int32.TryParse(formatCommand.Arg2, out iParsedArg2) == false)
							{
								continue;
							}
						}

						// 数値チェック（パラメタ側）
						int iParsedValue = 0;
						if (int.TryParse(StringUtility.ToEmpty(htProduct[formatCommand.KeyName]), out iParsedValue) == false)
						{
							blIsCastable = false;
						}

						//------------------------------------------------------
						// フォーマットコマンド別文字列取得処理
						//------------------------------------------------------
						// トレースメモ：パラメタ側Value(iParsedValue)には商品マスタ、在庫マスタなどのフィールド値が入っている
						if (blIsCastable)
						{
							//------------------------------------------------------
							// 文字列取得処理（数値変換可能の場合）
							//------------------------------------------------------
							switch (formatCommand.CommandType)
							{
								// 引数２つの間であれば出力
								case CommandType.Range:
									if (iParsedArg2 < iParsedArg1)
									{
										if ((iParsedArg2 <= iParsedValue) && (iParsedValue <= iParsedArg1))
										{
											sbResult.Append(GetFormatString(formatCommand, htProduct));
											blSkipFlag = true;
										}
									}
									else
									{
										if ((iParsedArg1 <= iParsedValue) && (iParsedValue <= iParsedArg2))
										{
											sbResult.Append(GetFormatString(formatCommand, htProduct));
											blSkipFlag = true;
										}
									}
									break;

								// パラメタ側Valueの方が大きい
								case CommandType.Bigger:
									if (iParsedArg1 < iParsedValue)
									{
										sbResult.Append(GetFormatString(formatCommand, htProduct));
										blSkipFlag = true;
									}
									break;

								// パラメタ側Valueの方が大きいか、同じ
								case CommandType.BiggerEqual:
									if (iParsedArg1 <= iParsedValue)
									{
										sbResult.Append(GetFormatString(formatCommand, htProduct));
										blSkipFlag = true;
									}
									break;

								// パラメタ側Valueの方が小さい
								case CommandType.Smaller:
									if (iParsedArg1 > iParsedValue)
									{
										sbResult.Append(GetFormatString(formatCommand, htProduct));
										blSkipFlag = true;
									}
									break;

								// パラメタ側Valueの方が小さいか、同じ
								case CommandType.SmallerEqual:
									if (iParsedArg1 >= iParsedValue)
									{
										sbResult.Append(GetFormatString(formatCommand, htProduct));
										blSkipFlag = true;
									}
									break;

								// パラメタ側Valueと同じ
								case CommandType.Equal:
									if (iParsedArg1 == iParsedValue)
									{
										sbResult.Append(GetFormatString(formatCommand, htProduct));
										blSkipFlag = true;
									}
									break;

								// パラメタ側Valueと異なる
								case CommandType.NotEqual:
									if (iParsedArg1 != iParsedValue)
									{
										sbResult.Append(GetFormatString(formatCommand, htProduct));
										blSkipFlag = true;
									}
									break;
							}
						}
						else
						{
							//------------------------------------------------------
							//  文字列取得処理（数値変換不可の場合）
							//------------------------------------------------------
							// 変換不可能な場合は文字列として一致／不一致のみ処理可能
							switch (formatCommand.CommandType)
							{
								// パラメタ側Valueと同じ
								case CommandType.Equal:
									if (formatCommand.Arg1.Equals(StringUtility.ToEmpty(htProduct[formatCommand.KeyName])))
									{
										sbResult.Append(GetFormatString(formatCommand, htProduct));
										blSkipFlag = true;
									}
									break;

								// パラメタ側Valueと異なる
								case CommandType.NotEqual:
									if (formatCommand.Arg1.Equals(StringUtility.ToEmpty(htProduct[formatCommand.KeyName])) == false)
									{
										sbResult.Append(GetFormatString(formatCommand, htProduct));
										blSkipFlag = true;
									}
									break;
							}
						}
						break;
				}
			}

			//------------------------------------------------------
			// 切捨て処理
			//------------------------------------------------------
			if ((this.LengthLimit != 0) && (sbResult.Length > this.LengthLimit))
			{
				return sbResult.ToString().Substring(0, this.LengthLimit);
			}
			else if ((this.ByteLimit != 0) && (GetByteLength(sbResult.ToString()) > this.ByteLimit))
			{
				return GetByteLengthString(sbResult.ToString(), this.ByteLimit);
			}

			return sbResult.ToString();
		}

		/// <summary>
		/// 出力用の文字列を取得する
		/// </summary>
		/// <param name="formatCommand">フォーマットコマンド</param>
		/// <param name="htProduct">商品情報</param>
		/// <returns>出力文字列</returns>
		private string GetFormatString(FormatCommand formatCommand, Hashtable htProduct)
		{
			return (formatCommand.IsFlgKey) ? StringUtility.ToEmpty(htProduct[formatCommand.Output]) : formatCommand.Output;
		}

		/// <summary>
		/// 文字列のバイト長（Shift_JIS）取得
		/// </summary>
		/// <param name="strValue">文字列</param>
		/// <returns>バイト長</returns>
		private int GetByteLength(string strValue)
		{
			return Encoding.GetEncoding("Shift_JIS").GetByteCount(strValue);
		}

		/// <summary>
		/// 文字列の指定バイト長文字列取得（Shift_JIS）
		/// </summary>
		/// <param name="strValue">文字列</param>
		/// <param name="iLimit">指定長</param>
		/// <returns>バイト長</returns>
		private string GetByteLengthString(string strValue, int iLimit)
		{
			StringBuilder sbResult = new StringBuilder();
			int iByteLength = 0;

			foreach (char cValue in strValue.ToCharArray())
			{
				// バイト長計算
				iByteLength += GetByteLength(cValue.ToString());

				// 指定長を超える場合はループを抜ける
				if (iByteLength > iLimit)
				{
					break;
				}

				// １文字セット
				sbResult.Append(cValue);
			}

			return sbResult.ToString();
		}

		/// <summary>
		/// 特殊タグ制御
		/// </summary>
		/// <param name="strKeyName">キー名称</param>
		/// <param name="htProducts">商品情報</param>
		/// <returns>処理結果</returns>
		private string SpTagCommand(
			string strKeyName,
			Hashtable htProducts,
			YahooVariation yahooVariation,
			string strMallId,
			string strKeyParam)
		{
			switch (strKeyName)
			{
				// 汎用
				case "point":
					return CalculatePoint(htProducts);
				case "PricePretax":
					return CalculateTaxIncludedPrice(htProducts);
				case "MallStockAlertDispatch":
					return MallStockAlertDispatch(htProducts, strMallId, strKeyParam);

				// 楽天専用
				case "RakutenSellRange":
					return RakutenSellRange(htProducts);
				case "RakutenCategory":
					return StringUtility.ToEmpty(htProducts[Constants.PRODUCTEXTEND_RAKUTEN_CATEGORY]);
				case "RakutenTaxRate":
					return RakutenTaxRate(htProducts);

				// Yahoo!専用
				case "YahooSalePeriodStart":
					return YahooSalePeriod(htProducts, true);
				case "YahooSalePeriodEnd":
					return YahooSalePeriod(htProducts, false);
				case "YahooSubCode":
					return yahooVariation.SubCode;
				case "YahooOptions":
					return yahooVariation.Options;
				case "YahooPath":
					return YahooPath(htProducts);
				case "YahooTaxRate":
					return YahooTaxRate(htProducts);

				default:
					return "";
			}
		}

		/// <summary>
		/// 特殊タグ制御（ポイント計算）
		/// </summary>
		/// <param name="htProducts">商品情報</param>
		/// <returns>ポイント計算結果</returns>
		private string CalculatePoint(Hashtable htProducts)
		{
			int iBgnIndex = this.Format.IndexOf("[SP:point]");
			int iEndIndex = iBgnIndex + "[SP:point]".Length;
			string strFollowCharacter = this.Format.Substring(iEndIndex).Replace("\"", "");

			// 価格種類設定
			string strPriceKind = "";
			if ((htProducts.Contains(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE)) && (htProducts[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] != DBNull.Value))
			{
				strPriceKind = Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE;
			}
			else if ((htProducts.Contains(Constants.FIELD_PRODUCTVARIATION_PRICE)) && (htProducts[Constants.FIELD_PRODUCTVARIATION_PRICE] != DBNull.Value))
			{
				strPriceKind = Constants.FIELD_PRODUCTVARIATION_PRICE;
			}
			else if ((htProducts.Contains(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE)) && (htProducts[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] != DBNull.Value))
			{
				strPriceKind = Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE;
			}
			else
			{
				strPriceKind = Constants.FIELD_PRODUCT_DISPLAY_PRICE;
			}

			// ポイント情報
			if (iBgnIndex != -1)
			{
				if ((string)htProducts["point_kbn1"] == Constants.FLG_PRODUCT_POINT_KBN1_RATE)
				{
					decimal dPoint = (decimal)htProducts[strPriceKind] * (decimal)htProducts["point1"] / 100;
					return ((int)dPoint).ToString() + strFollowCharacter;
				}
				else if ((string)htProducts["point_kbn1"] == Constants.FLG_PRODUCT_POINT_KBN1_NUM)
				{
					return htProducts["point1"].ToString() + strFollowCharacter;
				}
			}
			return "0";
		}

		/// <summary>
		/// 特殊タグ制御（税込み金額計算）
		/// </summary>
		/// <param name="htProducts">商品情報</param>
		/// <returns>税込み金額計算結果</returns>
		private string CalculateTaxIncludedPrice(Hashtable htProducts)
		{
			var bgnIndex = this.Format.IndexOf("[SP:PricePretax]");
			var endIndex = bgnIndex + "[SP:PricePretax]".Length;

			// 価格種類設定
			string priceKind = "";
			if ((htProducts.Contains(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE)) && (htProducts[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] != null))
			{
				priceKind = Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE;
			}
			else if ((htProducts.Contains(Constants.FIELD_PRODUCTVARIATION_PRICE)) && (htProducts[Constants.FIELD_PRODUCTVARIATION_PRICE] != null))
			{
				priceKind = Constants.FIELD_PRODUCTVARIATION_PRICE;
			}
			else if ((htProducts.Contains(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE)) && (htProducts[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] != null))
			{
				priceKind = Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE;
			}
			else
			{
				priceKind = Constants.FIELD_PRODUCT_DISPLAY_PRICE;
			}

			if (bgnIndex != -1)
			{
				var productTaxCategory = new ProductTaxCategoryService().Get((string)htProducts[Constants.FIELD_PRODUCT_TAX_CATEGORY_ID]);
				return TaxCalculationUtility.GetPriceTaxIncluded(
					(decimal)htProducts[priceKind],
					TaxCalculationUtility.GetTaxPrice((decimal)htProducts[priceKind], productTaxCategory.TaxRate, Constants.TAX_EXCLUDED_FRACTION_ROUNDING)).ToPriceString();
			}
			return "0";
		}

		/// <summary>
		/// 特殊タグ制御（在庫振分制御）
		/// </summary>
		/// <param name="htProducts">商品情報</param>
		/// <param name="strMallId">モールID</param>
		/// <param name="strKeyParam">キー名称</param>
		/// <returns>在庫数</returns>
		private string MallStockAlertDispatch(Hashtable htProducts, string strMallId, string strKeyParam)
		{
			//------------------------------------------------------
			// パラメータ解析
			//------------------------------------------------------
			string[] strParams = strKeyParam.Replace("(", "").Replace(")", "").Split(',');
			string strCompareExtend = null;
			if (htProducts.Contains(StringUtility.ToEmpty(strParams[0])))
			{
				strCompareExtend = StringUtility.ToEmpty(htProducts[StringUtility.ToEmpty(strParams[0])]);
			}
			else
			{
				FileLogger.WriteError("MallStockAlertDispatchのパラメータが不正です。[" + strParams[0] + "]");
				return "";
			}

			//------------------------------------------------------
			// 在庫振分処理
			//------------------------------------------------------
			StringBuilder sbResult = new StringBuilder();
			if ((int)htProducts[Constants.FIELD_PRODUCTSTOCK_STOCK] > (int)htProducts[Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT])
			{
				sbResult.Append(StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTSTOCK_STOCK]));
			}
			else
			{
				// パラメータからモール別に在庫数を振り分ける
				foreach (string strParam in strParams)
				{
					if (strParam.IndexOf(":") != -1)
					{
						string[] strCores = strParam.Split(':');
						if (strMallId == StringUtility.ToEmpty(strCores[1]))
						{
							sbResult.Append((strCompareExtend == StringUtility.ToEmpty(strCores[0]) || (strCompareExtend == "") ? (((int)htProducts[Constants.FIELD_PRODUCTSTOCK_STOCK] > 0) ? StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTSTOCK_STOCK]) : "0") : "0"));
							break;
						}
					}
				}
				// モールIDが指定されていない場合は在庫数をセット(何れの条件にも当てはまらない場合)
				if (sbResult.Length == 0)
				{
					sbResult.Append((((int)htProducts[Constants.FIELD_PRODUCTSTOCK_STOCK] > 0) ? StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTSTOCK_STOCK]) : "0"));
				}
			}

			return sbResult.ToString();
		}

		/// <summary>
		/// 特殊タグ制御（楽天用販売期間）
		/// </summary>
		/// <param name="htProducts">商品情報</param>
		/// <returns>楽天用販売期間</returns>
		private string RakutenSellRange(Hashtable htProducts)
		{
			try
			{
				// 販売期間From
				DateTime sellFromDate = (DateTime)htProducts[Constants.FIELD_PRODUCT_SELL_FROM];
				// 販売期間To（sell_toがnullの場合、"2100/01/01"を固定）
				DateTime sellToDate = (DateTime)StringUtility.ToValue(htProducts[Constants.FIELD_PRODUCT_SELL_TO], DateTime.Parse("2100/01/01"));

				// 1秒以上経っていた場合、分に繰り上げ
				if (sellFromDate.Second > 0)
				{
					sellFromDate = sellFromDate.AddMinutes(1);
				}

				// フォーマット：販売期間From + 半角スペース + 販売期間To
				return sellFromDate.ToString("yyyy/MM/dd HH:mm") + " " + sellToDate.ToString("yyyy/MM/dd HH:mm");
			}
			catch (Exception ex)
			{
				FileLogger.WriteWarn("販売期間From, Toを日付に変換できませんでした。\n" + ex.Message);
			}

			return "";
		}

		/// <summary>
		/// 特殊タグ制御（楽天用販売期間）
		/// </summary>
		/// <param name="htProducts">商品情報</param>
		/// <returns>楽天用販売期間</returns>
		private string RakutenCategory(Hashtable htProducts)
		{
			StringBuilder sbResult = new StringBuilder();
			if (StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTEXTEND_EXTEND112]) != "")
			{
				sbResult.Append(StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTEXTEND_EXTEND112]));
			}
			if (StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTEXTEND_EXTEND113]) != "")
			{
				sbResult.Append(@"\").Append(StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTEXTEND_EXTEND113]));
			}
			if (StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTEXTEND_EXTEND114]) != "")
			{
				sbResult.Append(@"\").Append(StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTEXTEND_EXTEND114]));
			}
			if (StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTEXTEND_EXTEND115]) != "")
			{
				sbResult.Append(@"\").Append(StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTEXTEND_EXTEND115]));
			}
			if (StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTEXTEND_EXTEND116]) != "")
			{
				sbResult.Append(@"\").Append(StringUtility.ToEmpty(htProducts[Constants.FIELD_PRODUCTEXTEND_EXTEND116]));
			}

			return sbResult.ToString();
		}

		/// <summary>
		/// 特殊タグ制御（楽天用税率）
		/// </summary>
		/// <param name="htProducts">商品情報</param>
		/// <returns>楽天用税率</returns>
		private string RakutenTaxRate(Hashtable htProducts)
		{
			var productTaxCategory = new ProductTaxCategoryService().Get((string)htProducts[Constants.FIELD_PRODUCT_TAX_CATEGORY_ID]);
			switch (productTaxCategory.TaxRate.ToString())
			{
				case "10.00":
				case "8.00":
				case "0.00":
					return (productTaxCategory.TaxRate / 100m).ToString();

				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// 特殊タグ制御（Yahoo!用販売期間）
		/// </summary>
		/// <param name="htProducts">商品情報</param>
		/// <param name="blStart">開始日フラグ</param>
		/// <returns>Yahoo!用販売期間</returns>
		private string YahooSalePeriod(Hashtable htProducts, bool blStart)
		{
			string strDate = "";

			try
			{
				if (blStart)
				{
					DateTime dtFrom = (DateTime)htProducts[Constants.FIELD_PRODUCT_SELL_FROM];
					if (dtFrom.Minute > 0)
					{
						dtFrom = dtFrom.AddHours(1);
					}
					strDate = dtFrom.ToString("yyyyMMddHH");
				}
				else
				{
					// 販売期間To（sell_toがnullの場合、"2100/01/01"を固定）
					DateTime dtTo = (DateTime)StringUtility.ToValue(htProducts[Constants.FIELD_PRODUCT_SELL_TO], DateTime.Parse("2100/01/01"));
					strDate = dtTo.ToString("yyyyMMddHH");
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteWarn("日付に変換できませんでした。\n" + ex.Message);
			}

			return strDate;
		}

		/// <summary>
		/// 特殊タグ制御（YahooPath）
		/// </summary>
		/// <param name="htProducts">商品情報</param>
		/// <returns>YahooPath</returns>
		private string YahooPath(Hashtable htProducts)
		{
			StringBuilder sbYahooPath = new StringBuilder();

			// 表示先カテゴリ１～５
			string[] strDispCategoryList = {   Constants.FIELD_PRODUCTEXTEND_EXTEND112,
											   Constants.FIELD_PRODUCTEXTEND_EXTEND113,
											   Constants.FIELD_PRODUCTEXTEND_EXTEND114,
											   Constants.FIELD_PRODUCTEXTEND_EXTEND115,
											   Constants.FIELD_PRODUCTEXTEND_EXTEND116};

			foreach (string strDispCategory in strDispCategoryList)
			{
				//------------------------------------------------------
				// 表示先カテゴリの設定がなければ処理なし
				//------------------------------------------------------
				string strYahooPath = StringUtility.ToEmpty(htProducts[strDispCategory]);
				if (strYahooPath == "")
				{
					continue;
				}

				//------------------------------------------------------
				// カテゴリ毎の文字列作成
				//------------------------------------------------------
				string[] strYahooPaths = strYahooPath.Split(@":\".ToCharArray());
				StringBuilder sbTemp = new StringBuilder();
				StringBuilder sbCategorySubPath = new StringBuilder();
				foreach (string strPath in strYahooPaths)
				{
					if ((sbTemp.ToString() != ""))
					{
						// カテゴリツリー内のカテゴリ毎の行先頭に改行を追記する
						sbCategorySubPath.Append("\n");

						// カテゴリツリー内のカテゴリ毎の区切り文字を追記する
						sbTemp.Append(":");
					}
					else if (sbYahooPath.ToString() != "")
					{
						// カテゴリツリー毎の行先頭に改行を追記する
						sbCategorySubPath.Append("\n");
					}

					sbTemp.Append(strPath);
					sbCategorySubPath.Append(sbTemp);
				}

				//------------------------------------------------------
				// 出力用文字列更新
				//------------------------------------------------------
				sbYahooPath.Append(sbCategorySubPath);
			}

			return sbYahooPath.ToString();
		}

		/// <summary>
		/// 特殊タグ制御（Yahoo用税率）
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <returns>Yahoo用税率</returns>
		private string YahooTaxRate(Hashtable product)
		{
			var productTaxCategory = new ProductTaxCategoryService().Get((string)product[Constants.FIELD_PRODUCT_TAX_CATEGORY_ID]);
			switch (productTaxCategory.TaxRate.ToString())
			{
				case "10.00":
				case "8.00":
					return (productTaxCategory.TaxRate / 100m).ToString();

				default:
					return string.Empty;
			}
		}

		/// <summary>出力フォーマット</summary>
		public string Format { get; set; }

		/// <summary>フォーマットID</summary>
		public int FormatId { get; set; }

		/// <summary>文字列長指定:0の場合指定無しとする</summary>
		public int LengthLimit { get; set; }

		/// <summary>バイト長指定:0の場合指定無しとする</summary>
		public int ByteLimit { get; set; }

		/// <summary>Yahoo!バリエーション要否</summary>
		public bool IsNeedYhoVariationInfo { get; set; }

		/// <summary>楽天カテゴリ要否</summary>
		public bool IsNeedRtnCategoryInfo { get; set; }
	}

	///**************************************************************************************
	/// <summary>
	/// 命令の列挙
	/// </summary>
	///**************************************************************************************
	public enum CommandType
	{
		StaticString,	// 固定文字列
		Data,			// 対応するデータを出力
		Tag,			// 特殊タグを出力
		Range,			// 引数２つの間であれば出力
		Bigger,			// 即値より大きければ出力
		BiggerEqual,	// 即値より大きいか、同じなら出力
		Smaller,		// 即値より小さければ出力
		SmallerEqual,	// 即値より小さいか、同じなら出力
		Equal,			// 即値と同じなら出力
		NotEqual,		// 即値と違ったら出力
		Eob				// 分岐の終端
	}
}
