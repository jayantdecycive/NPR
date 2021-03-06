/*
   Tuesday, November 5, 20132:58:51 PM
   User: res_admin
   Server: restier2staging.cuwhyqxxofly.us-east-1.rds.amazonaws.com
   Database: npr
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Tickets ADD
	CCNumber varchar(255) NULL,
	CCExpDateMonth INT NULL,
	CCExpDateYear INT NULL,
	CCName varchar(255) NULL,
	CCType varchar(255) NULL
GO
ALTER TABLE dbo.Tickets SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Tickets', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Tickets', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Tickets', 'Object', 'CONTROL') as Contr_Per 