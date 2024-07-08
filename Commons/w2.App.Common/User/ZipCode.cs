/*
=========================================================================================================
  Module      : Zip Code (ZipCode.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.User
{
	/// <summary>
	/// Zip code
	/// </summary>
	public class ZipCode
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ZipCode()
		{
			this.Zip1 = string.Empty;
			this.Zip2 = string.Empty;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="inputZipCode">Input zip code</param>
		public ZipCode(string inputZipCode)
			: this()
		{
			if (inputZipCode.Length < 7) return;

			var inputZipCodes = inputZipCode.Contains("-")
				? inputZipCode.Split('-')
				: inputZipCode.Split('ｰ');
			if (inputZipCodes.Length > 1)
			{
				this.Zip1 = inputZipCodes[0];
				this.Zip2 = inputZipCodes[1];
				return;
			}

			this.Zip1 = inputZipCode.Substring(0, 3);
			this.Zip2 = inputZipCode.Substring(3);
		}

		/// <summary>Zip 1</summary>
		public string Zip1 { get; set; }
		/// <summary>Zip 2</summary>
		public string Zip2 { get; set; }
		/// <summary>Zip</summary>
		public string Zip
		{
			get
			{
				if (string.IsNullOrEmpty(this.Zip1)
					|| string.IsNullOrEmpty(this.Zip2))
				{
					return string.Empty;
				}

				var zip = string.Format("{0}-{1}", this.Zip1, this.Zip2);
				return zip;
			}
		}
	}
}
