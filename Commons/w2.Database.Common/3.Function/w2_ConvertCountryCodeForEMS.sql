if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_ConvertCountryCodeForEMS]') and OBJECTPROPERTY(id, N'IsScalarFunction') = 1)
drop function [dbo].[w2_ConvertCountryCodeForEMS]
GO
/*
=========================================================================================================
  Module      : EMS用国コード変換(w2_ConvertCountryCodeForEMS.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE FUNCTION dbo.w2_ConvertCountryCodeForEMS(
	@s VARCHAR(10)
	)
RETURNS VARCHAR(10) AS
BEGIN
	/* アンドラ → フランス */
	SET @s = REPLACE(@s, 'AD', 'FR')
	/* ガーンジー → イギリス */
	SET @s = REPLACE(@s, 'GG', 'GB')
	/* マン島 → イギリス */
	SET @s = REPLACE(@s, 'IM', 'GB')
	
	RETURN @s
END