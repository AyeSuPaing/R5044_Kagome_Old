if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeleteItAdto]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeleteItAdto]
GO
/*
=========================================================================================================
  Module      : 商品コンバータ設定削除プロシージャ(DeleteItAdto.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteItAdto] @adto_id int
AS
	BEGIN TRANSACTION

	delete from w2_MallPrdcnvRule where adto_id = @adto_id;
	IF @@error <> 0 GOTO CATCH

	delete from w2_MallPrdcnvColumns where adto_id = @adto_id;
	IF @@error <> 0 GOTO CATCH

	delete from w2_MallPrdcnv where adto_id = @adto_id;
	IF @@error <> 0 GOTO CATCH

	COMMIT TRANSACTION
	RETURN 0

  CATCH:
	ROLLBACK TRANSACTION
	RETURN -1
RETURN
