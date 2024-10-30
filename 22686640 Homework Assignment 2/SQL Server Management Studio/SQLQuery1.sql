USE [master]
GO
/****** Object:  Database [RandomNames]    Script Date: 2018/07/15 2:51:01 PM ******/
CREATE DATABASE [SocialClub]
GO
USE [SocialClub]
GO
/****** Object:  Table [dbo].[Name]    Script Date: 2018/07/15 2:51:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Members](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Club] [varchar](200) NOT NULL,
	[Age] [varchar](200) NOT NULL,
	[Fee] [int] NOT NULL,

 CONSTRAINT [PK_Name] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

USE [master]
GO
ALTER DATABASE [SocialClub] SET  READ_WRITE 
GO