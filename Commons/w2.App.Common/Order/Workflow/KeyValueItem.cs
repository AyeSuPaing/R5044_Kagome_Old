/*
=========================================================================================================
  Module      : Key value item(KeyValueItem.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// Key value item
	/// </summary>
	[Serializable]
	public class KeyValueItem
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public KeyValueItem()
		{
			this.Key = string.Empty;
			this.Value = string.Empty;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="value">Value</param>
		public KeyValueItem(string key, string value)
		{
			this.Key = key;
			this.Value = value;
		}

		/// <summary>Key</summary>
		[JsonProperty("key")]
		public string Key { get; set; }
		/// <summary>Value</summary>
		[JsonProperty("value")]
		public string Value { get; set; }
	}
}
