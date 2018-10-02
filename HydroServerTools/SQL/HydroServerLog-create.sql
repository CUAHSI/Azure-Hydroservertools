USE [hiscentral_logging]
GO

IF OBJECT_ID(N'dbo.HydroServerLog', N'U') IS NOT NULL
BEGIN
	DROP TABLE [dbo].[HydroServerLog]
END

IF OBJECT_ID(N'dbo.HydroServerError', N'U') IS NOT NULL
BEGIN
	DROP TABLE [dbo].[HydroServerError]
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[HydroServerLog](
	[ClusteredId] [int] identity NOT NULL,
	[SessionId] [nvarchar](50) NOT NULL,
	[IPAddress] [nvarchar](50) NOT NULL,
	[Domain] [nvarchar](255) NOT NULL,
	[EmailAddress] [nvarchar](255) NULL,
	[StartDateTime] [datetimeoffset](7) NOT NULL,
	[EndDateTime] [datetimeoffset](7) NOT NULL,
	[MethodName] [nvarchar](255) NOT NULL,
	[Parameters] [nvarchar](max) NULL,
	[Returns] [nvarchar](max) NULL,
	[Message] [nvarchar](255) NOT NULL,
	[LogLevel] [nvarchar](20) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE CLUSTERED INDEX IX_Log 
    ON dbo.HydroServerLog (ClusteredId);

GO

CREATE TABLE [dbo].[HydroServerError](
	[ClusteredId] [int] identity NOT NULL,
	[SessionId] [nvarchar](50) NOT NULL,
	[IPAddress] [nvarchar](50) NOT NULL,
	[Domain] [nvarchar](255) NOT NULL,
	[OccurrenceDateTime] [datetimeoffset](7) NOT NULL,
	[MethodName] [nvarchar](255) NOT NULL,
	[Parameters] [nvarchar](max) NULL,
	[ExceptionType] [nvarchar](50) NOT NULL,
	[ExceptionMessage] [nvarchar](255) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE CLUSTERED INDEX IX_Error 
    ON dbo.HydroServerError (ClusteredId);

GO

