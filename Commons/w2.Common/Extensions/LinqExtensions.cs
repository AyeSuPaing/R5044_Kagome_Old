/*
=========================================================================================================
  Module      : Linq拡張モジュール(LinqExtensions.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace w2.Common.Extensions
{
	/// <summary>
	/// Linq拡張モジュール
	/// </summary>
	public static class LinqExtensions
	{
		/// <summary>
		/// 指定サイズで分割されたシーケンスを返却する
		/// </summary>
		/// <typeparam name="T">シーケンスの型</typeparam>
		/// <param name="source">対象となる値のシーケンス</param>
		/// <param name="count">分割するサイズ</param>
		/// <returns>指定サイズで分割されたシーケンス</returns>
		public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int count)
		{
			var result = new List<T>(count);
			foreach (var item in source)
			{
				result.Add(item);
				if (result.Count == count)
				{
					yield return result;
					result = new List<T>(count);
				}
			}

			if (result.Count != 0)
			{
				yield return result.ToArray();
			}
		}

		/// <summary>
		/// 文字列として結合
		/// </summary>
		/// <typeparam name="T">シーケンスの型</typeparam>
		/// <param name="source">連結する文字列を格納しているコレクション</param>
		/// <param name="separator">区切りとして使用する文字列</param>
		/// <returns>連結された文字列</returns>
		public static string JoinToString<T>(this IEnumerable<T> source, string separator = "")
		{
			var joined = string.Join(separator, source);
			return joined;
		}

		/// <summary>
		/// IDataReader型に変換して取得
		/// </summary>
		/// <typeparam name="T">シーケンスの型</typeparam>
		/// <param name="source">対象となる値のシーケンス</param>
		/// <returns>IDataReader型に変換されたオブジェクト</returns>
		public static IDataReader AsDataReader<T>(this IEnumerable<T> source)
		{
			return new EnumerableDataReader<T>(source);
		}

		/// <summary>
		/// 要素追加
		/// </summary>
		/// <typeparam name="T">シーケンスの型</typeparam>
		/// <param name="sequence">シーケンス</param>
		/// <param name="elem">追加するオブジェクト</param>
		/// <returns><paramref name="elem"/>が追加されたシーケンス</returns>
		public static IEnumerable<T> AppendElement<T>(this IEnumerable<T> sequence, T elem)
		{
			foreach (var obj in sequence)
			{
				yield return obj;
			}

			yield return elem;
		}

		/// <summary>
		/// Distinct拡張メソッド
		/// </summary>
		/// <typeparam name="T">シーケンスの型</typeparam>
		/// <typeparam name="TKey">キー</typeparam>
		/// <param name="source">対象となる値のシーケンス</param>
		/// <param name="keySelector">キーの指定</param>
		/// <returns>Distinct内で比較</returns>
		public static IEnumerable<T> Distinct<T, TKey>(
			this IEnumerable<T> source,
			Func<T, TKey> keySelector)
		{
			var distincted = source.Distinct(new DelegateComparer<T, TKey>(keySelector));
			return distincted;
		}

		/// <summary>
		/// Enumerable型のオブジェクトにIDataReaderの機能を実装
		/// </summary>
		/// <typeparam name="T">シーケンスの型</typeparam>
		class EnumerableDataReader<T> : IDataReader
		{
			/// <summary>IDataReaderの機能を実装するオブジェクト</summary>
			private readonly IEnumerator<T> m_source;
			/// <summary>プロパティ名の一覧</summary>
			private readonly List<PropertyInfo> m_listProp = new List<PropertyInfo>();

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="source">IDataReaderの機能を実装するオブジェクト</param>
			internal EnumerableDataReader(IEnumerable<T> source)
			{
				m_source = source.GetEnumerator();

				m_listProp.AddRange(typeof(T).GetProperties());
			}

			/// <summary>
			/// 要素を取得し、ポインタを次に進める
			/// </summary>
			/// <returns>列挙子が次の要素に正常に進んだ場合は true。列挙子がコレクションの末尾を越えた場合は false。</returns>
			public bool Read()
			{
				return m_source.MoveNext();
			}

			/// <summary>
			/// フィールド数を取得
			/// </summary>
			public int FieldCount
			{
				get { return m_listProp.Count; }
			}

			/// <summary>
			/// 値を取得する
			/// </summary>
			/// <param name="i"></param>
			/// <returns></returns>
			public object GetValue(int i)
			{
				return m_listProp[i].GetValue(m_source.Current, null);
			}

			/// <summary>
			/// IDataReaderの実装、何もしない
			/// </summary>
			public void Close()
			{
			}

			/// <summary>
			/// IDataReaderの実装、何もしない
			/// </summary>
			public void Dispose()
			{
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public int Depth
			{
				get { throw new NotImplementedException(); }
			}

			#region IDataReader メンバー

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public DataTable GetSchemaTable()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public bool IsClosed
			{
				get { throw new NotImplementedException(); }
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public bool NextResult()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public int RecordsAffected
			{
				get { throw new NotImplementedException(); }
			}

			#endregion

			#region IDataRecord メンバー

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public bool GetBoolean(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public byte GetByte(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public char GetChar(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public IDataReader GetData(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public string GetDataTypeName(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public DateTime GetDateTime(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public decimal GetDecimal(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public double GetDouble(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public Type GetFieldType(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public float GetFloat(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public Guid GetGuid(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public short GetInt16(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public int GetInt32(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public long GetInt64(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public string GetName(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public int GetOrdinal(string name)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public string GetString(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public int GetValues(object[] values)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public bool IsDBNull(int i)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public object this[string name]
			{
				get { throw new NotImplementedException(); }
			}

			/// <summary>
			/// IDataReaderの実装、例外を返すだけ
			/// </summary>
			public object this[int i]
			{
				get { throw new NotImplementedException(); }
			}

			#endregion
		}

		/// <summary>
		/// Distinct比較用クラス
		/// </summary>
		/// <typeparam name="T">シーケンス型</typeparam>
		/// <typeparam name="TKey">キー</typeparam>
		private class DelegateComparer<T, TKey> : IEqualityComparer<T>
		{
			/// <summary>セレクタ関数</summary>
			private readonly Func<T, TKey> _selector;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="keySelector">比較関数</param>
			public DelegateComparer(Func<T, TKey> keySelector)
			{
				// キーを指定する関数を受け取る
				_selector = keySelector;
			}

			/// <summary>
			/// 比較
			/// </summary>
			/// <param name="x">値1</param>
			/// <param name="y">値2</param>
			/// <returns>同一か</returns>
			public bool Equals(T x, T y)
			{
				return _selector(x).Equals(_selector(y));
			}

			/// <summary>
			/// ハッシュ値取得
			/// </summary>
			/// <param name="obj">値</param>
			/// <returns>ハッシュ値</returns>
			public int GetHashCode(T obj)
			{
				return _selector(obj).GetHashCode();
			}
		}
	}
}
