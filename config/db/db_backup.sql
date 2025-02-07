USE [master]
GO
/****** Object:  Database [ScaffoldDb]    Script Date: 1/01/2021 23:01:24 ******/
CREATE DATABASE [ScaffoldDb]
USE [ScaffoldDb]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 1/01/2021 23:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Role] [nvarchar](100) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 1/01/2021 23:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](150) NULL,
	[LastName] [nvarchar](150) NULL,
	[Email] [nvarchar](500) NULL,
	[Password] [nvarchar](150) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[RoleId] [int] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([Id], [Role]) VALUES (1, N'Admin')
INSERT [dbo].[Roles] ([Id], [Role]) VALUES (2, N'User')
SET IDENTITY_INSERT [dbo].[Roles] OFF
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([Id], [FirstName], [LastName], [Email], [Password], [CreatedDate], [ModifiedDate], [RoleId]) VALUES (1, N'Prince', N'Kumar', N'princeofficial88@gmail.com', N'Jt5ZlUk3EKQznNAcS9b1rV03PprCnjAL9d8oK1eroX8=', CAST(N'2021-01-01T22:23:09.843' AS DateTime), NULL, 1)
INSERT [dbo].[Users] ([Id], [FirstName], [LastName], [Email], [Password], [CreatedDate], [ModifiedDate], [RoleId]) VALUES (2, N'Prince', N'Kumar', N'princeofficial881@gmail.com', N'Jt5ZlUk3EKQznNAcS9b1rV03PprCnjAL9d8oK1eroX8=', CAST(N'2021-01-01T22:25:33.483' AS DateTime), NULL, 1)
SET IDENTITY_INSERT [dbo].[Users] OFF
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles]
GO
/****** Object:  StoredProcedure [dbo].[spi_Users]    Script Date: 1/01/2021 23:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[spi_Users] 
     @FirstName NVARCHAR(200) = NULL
	,@LastName NVARCHAR(200) = NULL
	,@Email NVARCHAR(300) = NULL
	,@Password NVARCHAR(300) = NULL
	,@RoleId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return INT
		,@CustId INT

	IF NOT EXISTS (
			SELECT TOP 1 1
			FROM Users
			WHERE UPPER(Email) = UPPER(@Email)
			)
	BEGIN
		INSERT INTO Users (
			 FirstName
			,LastName
			,Email
			,Password
			,CreatedDate
			,RoleId
			)
		VALUES (
			 @FirstName
			,@LastName
			,@Email
			,@Password
			,GETDATE()
			,@RoleId
			)

SET @CustId = SCOPE_IDENTITY()
		SELECT au.Id  
	   ,au.FirstName
	   ,au.LastName 
	   ,au.Email    
	   ,r.Role 
	   ,au.CreatedDate  
	   ,au.ModifiedDate  
  FROM Users au  
  LEFT JOIN [Roles] r ON au.RoleId = r.Id  
WHERE au.Id = @CustId
		

		SET @return = 100 -- creation success                    
	END
	ELSE
	BEGIN
		SET @return = 105 -- Record exists                    
	END

	RETURN @return
END
GO
/****** Object:  StoredProcedure [dbo].[sps_AuthUser]    Script Date: 1/01/2021 23:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
    
--sps_AuthUser 'bharat@gmail.com','P',5                
CREATE PROCEDURE [dbo].[sps_AuthUser] @Email NVARCHAR(100)    
 ,@Password NVARCHAR(100)    
AS    
BEGIN    
 DECLARE @return INT;    
    
 IF EXISTS (    
   SELECT TOP 1 1    
   FROM Users    
   WHERE lower(Email) = lower(@Email)    
    AND Password = @Password    
   )    
 BEGIN    

    
		SELECT au.Id  
	   ,au.FirstName
	   ,au.LastName 
	   ,au.Email    
	   ,r.Role 
	   ,au.CreatedDate  
	   ,au.ModifiedDate  
  FROM Users au  
  LEFT JOIN [Roles] r ON au.RoleId = r.Id  
  WHERE au.Email = @Email      
   AND au.[Password] = @Password      
    
  SET @return = 100 -- creation success                          
 END    
 ELSE    
 BEGIN    
  SET @return = 105 -- Record exists                          
 END    
    
 RETURN @return    
END
GO
USE [master]
GO
ALTER DATABASE [ScaffoldDb] SET  READ_WRITE 
GO
