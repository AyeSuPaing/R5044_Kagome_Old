/*
=========================================================================================================
  Module      : Page Method Exception Logger(PageMethodExceptionLogger.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.IO;
using System.Web;

namespace w2.Common.Logger
{
	/// <summary>
	/// Page Method Exception Logger
	/// </summary>
	public class PageMethodExceptionLogger : Stream
	{
		/// <summary>Http response</summary>
		private readonly HttpResponse _response;
		/// <summary>Base stream</summary>
		private readonly Stream _baseStream;
		/// <summary>Captured stream</summary>
		private readonly MemoryStream _capturedStream = new MemoryStream();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="response">Http response</param>
		public PageMethodExceptionLogger(HttpResponse response)
		{
			_response = response;
			_baseStream = response.Filter;
		}

		/// <summary>
		/// Close
		/// </summary>
		public override void Close()
		{
			// When call ajax request has error, export the error to log
			if ((_response.StatusCode == 500) && (_response.Headers["jsonerror"] == "true"))
			{
				try
				{
					_capturedStream.Position = 0;
					using (var sr = new StreamReader(_capturedStream))
					{
						// Get and create log error message from exception json object
						var responseJson = sr.ReadToEnd();
						var exceptionObject = JsonConvert.DeserializeObject<JsonExceptionResponse>(responseJson);
						var errorMessage = (exceptionObject != null)
							? string.Format(
								"{0}-> {1}{0}{2}{0}   ExceptionType: {3}",
								Environment.NewLine,
								exceptionObject.Message,
								exceptionObject.StackTrace,
								exceptionObject.ExceptionType)
							: responseJson;
						FileLogger.WriteError(errorMessage);
					}
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
				}
			}

			_baseStream.Close();
			base.Close();
		}

		/// <summary>
		/// Flush
		/// </summary>
		public override void Flush()
		{
			_baseStream.Flush();
		}

		/// <summary>
		/// Seek
		/// </summary>
		/// <param name="offset">Offset</param>
		/// <param name="origin">Origin</param>
		/// <returns>The new position within the current stream</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			return _baseStream.Seek(offset, origin);
		}

		/// <summary>
		/// Set length
		/// </summary>
		/// <param name="value">Value</param>
		public override void SetLength(long value)
		{
			_baseStream.SetLength(value);
		}

		/// <summary>
		/// Read
		/// </summary>
		/// <param name="buffer">Buffer</param>
		/// <param name="offset">Offset</param>
		/// <param name="count">Count</param>
		/// <returns>The total number of bytes read into the buffer</returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			return _baseStream.Read(buffer, offset, count);
		}

		/// <summary>
		/// Write
		/// </summary>
		/// <param name="buffer">Buffer</param>
		/// <param name="offset">Offset</param>
		/// <param name="count">Count</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			_baseStream.Write(buffer, offset, count);
			_capturedStream.Write(buffer, offset, count);
		}

		/// <summary>Can read</summary>
		public override bool CanRead { get { return _baseStream.CanRead; } }
		/// <summary>Can seek</summary>
		public override bool CanSeek { get { return _baseStream.CanSeek; } }
		/// <summary>Can write</summary>
		public override bool CanWrite { get { return _baseStream.CanWrite; } }
		/// <summary>Length</summary>
		public override long Length { get { return _baseStream.Length; } }
		/// <summary>Position</summary>
		public override long Position
		{
			get { return _baseStream.Position; }
			set { _baseStream.Position = value; }
		}
	}

	/// <summary>
	/// Json Exception Response
	/// </summary>
	internal class JsonExceptionResponse
	{
		/// <summary>Message</summary>
		[JsonProperty("Message")]
		public string Message { get; set; }
		/// <summary>Stack trace</summary>
		[JsonProperty("StackTrace")]
		public string StackTrace { get; set; }
		/// <summary>Exception type</summary>
		[JsonProperty("ExceptionType")]
		public string ExceptionType { get; set; }
	}
}
