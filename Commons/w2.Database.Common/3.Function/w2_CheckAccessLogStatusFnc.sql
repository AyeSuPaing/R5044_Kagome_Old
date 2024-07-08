if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CheckAccessLogStatusFnc]') and OBJECTPROPERTY(id, N'IsScalarFunction') = 1)
drop function [dbo].[w2_CheckAccessLogStatusFnc]
GO
/*
=========================================================================================================
  Module      : アクセスログ処理ステータスチェック関数(w2_CheckAccessLogStatusFnc.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE FUNCTION w2_CheckAccessLogStatusFnc(
	@tgt_date [nvarchar] (10),			-- 対象"2006/01/01"
	@file_num [int],					-- 取込ファイル番号(日次取込のみ)
	@day_or_month [int],				-- 1:日次  2:月次★まず日次のみ
	@req_status [int]					-- 要求処理ステータス(01：取込処理、11：加工処理)
	)
RETURNS int	-- 0:取込可能、 -1以下:取込不可
BEGIN
	DECLARE	@st_target_date nvarchar(10)
	DECLARE	@st_day_status nvarchar(2)
	DECLARE	@st_import_files int
	DECLARE	@st_import_files_total int
	DECLARE	@st_month_status nvarchar(2)

	-- 現在のステータス取得
	SELECT	TOP 1
			@st_target_date = target_date,
			@st_day_status = day_status,
			@st_month_status = month_status,
			@st_import_files = import_files,
			@st_import_files_total = import_files_total
	  FROM	w2_AccessLogStatus

	------------------------------------------
	-- ステータスレコードなしの場合-
	------------------------------------------
	IF @st_target_date IS NULL
	BEGIN
		-- 加工要求であればNG
		IF @req_status <> '01'
		BEGIN
			RETURN -1
		END
	END
	------------------------------------------
	-- 前の日以前であればNG
	------------------------------------------
	ELSE IF CONVERT(datetime, @tgt_date) < CONVERT(datetime, @st_target_date)
	BEGIN
		RETURN -10
	END
	------------------------------------------
	-- 同じ日の場合
	------------------------------------------
	ELSE IF CONVERT(datetime, @tgt_date) = CONVERT(datetime, @st_target_date)
	BEGIN
		-- 取込処理要求
		IF @req_status = '01'
		BEGIN
			-- 取込中、加工待ち、加工処理中、加工完了はＮＧ
			IF @st_day_status IN ('01','10','11','20')
			BEGIN
				RETURN -20
			END
			
			-- 一部取り込みはチェック（初期状態は通過）
			IF @st_day_status <> '00'
			BEGIN
				-- 連続した取込でない場合、規定数以上取込の場合はNG
				IF ((@st_import_files +1 <> @file_num) OR (@file_num > @st_import_files_total))
				BEGIN
					RETURN -21
				END
			END
		END
		-- 加工処理要求
		ELSE IF @req_status = '11'
		BEGIN
			-- 加工待ち以外はＮＧ
			IF NOT @st_day_status = '10'
			BEGIN
				RETURN -22
			END
		END
	END
	------------------------------------------
	-- 次の日以降場合
	------------------------------------------
	ELSE IF CONVERT(datetime, @tgt_date) > CONVERT(datetime, @st_target_date)
	BEGIN
		-- 取込処理要求
		IF @req_status = '01'
		BEGIN
			-- 前日 かつ 前日加工完了以外はNG
			IF NOT ((CONVERT(datetime, @tgt_date) = DATEADD(dd,1,CONVERT(datetime, @st_target_date)))  AND (@st_day_status = '20'))
			BEGIN
				RETURN -30
			END
		END
		ELSE
		-- 取込処理要求以外はNG
		BEGIN
			RETURN -31
		END
	END

	-- OK
	RETURN 0
END

