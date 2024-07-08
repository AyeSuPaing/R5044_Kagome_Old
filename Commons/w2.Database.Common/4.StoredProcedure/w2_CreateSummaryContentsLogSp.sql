if exists (select * from sys.procedures where name = 'w2_CreateSummaryContentsLogSp')
	drop PROCEDURE [dbo].[w2_CreateSummaryContentsLogSp]
GO
/*
=========================================================================================================
  Module      : コンテンツログ解析データ作成プロシージャ(w2_CreateSummaryContentsLogSp.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_CreateSummaryContentsLogSp] (
					@TARGET_DATE varchar(10)) AS

	-- 変数定義
	DECLARE @DATE datetime
	DECLARE @DELETE_DATE datetime

	-- 日付セット
	SET @DATE = @TARGET_DATE
	SET @DELETE_DATE = DATEADD(MONTH, -1, @DATE)

	-- 削除
	DELETE
	  FROM	w2_ContentsSummaryAnalysis
	 WHERE	date = @DATE

	-- 挿入
	INSERT
	  INTO	w2_ContentsSummaryAnalysis
			(
			date,
			report_type,
			access_kbn,
			contents_type,
			contents_id,
			count,
			price
			)
	SELECT	@DATE,
			w2_ContentsLog.report_type,
			w2_ContentsLog.access_kbn,
			w2_ContentsLog.contents_type,
			w2_ContentsLog.contents_id,
			COUNT(w2_ContentsLog.contents_id),
			SUM(w2_ContentsLog.price)
	  FROM	w2_ContentsLog
	 WHERE	w2_ContentsLog.date BETWEEN @TARGET_DATE + ' 00:00:00' AND @TARGET_DATE + ' 23:59:59.997'
	GROUP BY w2_ContentsLog.report_type, w2_ContentsLog.access_kbn, w2_ContentsLog.contents_type, w2_ContentsLog.contents_id

	-- ログデータ削除(1カ月前)
	DELETE
	  FROM	w2_ContentsLog
	 WHERE	date < @DELETE_DATE

GO