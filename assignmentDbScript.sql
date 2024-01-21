USE [AssignmentDb]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 1/21/2024 4:47:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[id] [uniqueidentifier] NOT NULL,
	[username] [nvarchar](255) NOT NULL,
	[password] [nvarchar](255) NOT NULL,
	[isAdmin] [bit] NULL,
	[age] [int] NOT NULL,
	[hobbies] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT (newid()) FOR [id]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [isAdmin]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT ('') FOR [hobbies]
GO


