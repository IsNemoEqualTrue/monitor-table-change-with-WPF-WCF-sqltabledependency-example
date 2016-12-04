CREATE TABLE [dbo].[Stocks](
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[Price] [decimal](18, 0) NULL
)
GO
INSERT [dbo].[Stocks] ([Code], [Name], [Price]) VALUES (N'MCD', N'McDonald Corp', CAST(333 AS Decimal(18, 0)))
GO
INSERT [dbo].[Stocks] ([Code], [Name], [Price]) VALUES (N'NKE', N'Nike Inc', CAST(240 AS Decimal(18, 0)))
GO
INSERT [dbo].[Stocks] ([Code], [Name], [Price]) VALUES (N'DIS', N'Walt Disney Co', CAST(130 AS Decimal(18, 0)))
GO
INSERT [dbo].[Stocks] ([Code], [Name], [Price]) VALUES (N'UTX', N'United Technologies Corp', CAST(130 AS Decimal(18, 0)))
GO
INSERT [dbo].[Stocks] ([Code], [Name], [Price]) VALUES (N'MSFT', N'Microsoft Corp', CAST(130 AS Decimal(18, 0)))
GO
INSERT [dbo].[Stocks] ([Code], [Name], [Price]) VALUES (N'PFE', N'Pfizer Inc', CAST(130 AS Decimal(18, 0)))
GO
INSERT [dbo].[Stocks] ([Code], [Name], [Price]) VALUES (N'INTC', N'Intel Corp', CAST(130 AS Decimal(18, 0)))
GO
INSERT [dbo].[Stocks] ([Code], [Name], [Price]) VALUES (N'KO', N'Coca Cola Co', CAST(130 AS Decimal(18, 0)))
GO
INSERT [dbo].[Stocks] ([Code], [Name], [Price]) VALUES (N'JPM', N'JPMorgan Chase and Co', CAST(130 AS Decimal(18, 0)))
GO
INSERT [dbo].[Stocks] ([Code], [Name], [Price]) VALUES (N'CSCO', N'Cisco Systems Inc', CAST(130 AS Decimal(18, 0)))
GO
INSERT [dbo].[Stocks] ([Code], [Name], [Price]) VALUES (N'BA', N'Boeing Co', CAST(130 AS Decimal(18, 0)))
GO
INSERT [dbo].[Stocks] ([Code], [Name], [Price]) VALUES (N'CVX', N'Chevron Corp', CAST(130 AS Decimal(18, 0)))
GO