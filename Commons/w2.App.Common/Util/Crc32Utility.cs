/*
=========================================================================================================
  Module      : Crc32 Utility (Crc32Utility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Util
{
	/// <summary>
	/// Performs 32-bit reversed cyclic redundancy checks.
	/// </summary>
	public class Crc32Utility
	{
		/// <summary>Generator polynomial (module 2) for the reversed CRC32 algorithm</summary>
		private const UInt32 s_generator = 0xEDB88320;

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		static Crc32Utility()
		{
			// Constructs the checksum lookup table. Used to optimize the checksum.
			m_checksumTable = Enumerable.Range(0, 256)
				.Select(item =>
				{
					var tableEntry = (uint)item;
					for (var index = 0; index < 8; ++index)
					{
						tableEntry = ((tableEntry & 1) != 0)
							? (s_generator ^ (tableEntry >> 1))
							: (tableEntry >> 1);
					}

					return tableEntry;
				})
				.ToArray();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Calculates the checksum of the byte stream.
		/// </summary>
		/// <param name="byteStream">The byte stream to calculate the checksum for</param>
		/// <returns>A 32-bit reversed checksum</returns>
		public static UInt32 Get<T>(IEnumerable<T> byteStream)
		{
			try
			{
				// Initialize checksumRegister to 0xFFFFFFFF and calculate the checksum
				return ~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
					(m_checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8)));
			}
			catch (Exception e)
			{
				throw new Exception("Could not read the stream out as bytes.", e);
			}
		}
		#endregion

		#region Properties
		/// <summary>Contains a cache of calculated checksum chunks</summary>
		private static readonly UInt32[] m_checksumTable;
		#endregion
	}
}
