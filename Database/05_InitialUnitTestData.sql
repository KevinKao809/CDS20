DELETE FROM [CDS20].[EquipmentClass]
DELETE FROM [CDS20].[DashboardWidgets]
DELETE FROM [CDS20].[WidgetCatalog]
DELETE FROM [CDS20].[Dashboard]
DELETE FROM [CDS20].[UserRolePermission]
DELETE FROM [CDS20].[EmployeeInRole]
DELETE FROM [CDS20].[UserRole]
DELETE FROM [CDS20].[Factory]
DELETE FROM [CDS20].[Employee]
DELETE FROM [CDS20].[Company]
DELETE FROM [CDS20].[RefCultureInfo]



INSERT INTO [CDS20].[RefCultureInfo] ([CultureCode], [Name]) VALUES (N'en-US', N'English (U.S.)')
INSERT INTO [CDS20].[RefCultureInfo] ([CultureCode], [Name]) VALUES (N'zh-TW', N'Chinese (Taiwan)')

SET IDENTITY_INSERT [CDS20].[Company] ON
INSERT INTO [CDS20].[Company] ([Id], [Name], [Address], [ContactName], [Latitude], [Longitude], [CultureInfo], [CreatedAt], [DeletedFlag]) VALUES (01, N'Microsoft (Test)', N'Taipei (Test)', N'York (Test)', 0, 0, N'zh-TW', getdate(), 0)
INSERT INTO [CDS20].[Company] ([Id], [Name], [Address], [ContactName], [Latitude], [Longitude], [CultureInfo], [CreatedAt], [DeletedFlag]) VALUES (99, N'Microsoft99 (Test)', N'Taipei (Test)', N'York (Test)', 0, 0, N'zh-TW', getdate(), 0)
--INSERT INTO [CDS20].[Company] ([Id], [Name], [ShortName], [Address], [CompanyWebSite], [ContactName], [ContactPhone], [ContactEmail], [Latitude], [Longitude], [LogoURL], [CultureInfo], [AllowDomain], [ExtAppAuthenticationKey], [CreatedAt], [UpdatedAt], [DeletedFlag]) VALUES (1, N'Microsoft (Test)',NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'zh-TW', NULL, NULL, getdate(), NULL, 0)
SET IDENTITY_INSERT [CDS20].[Company] OFF

SET IDENTITY_INSERT [CDS20].[Factory] ON
INSERT INTO [CDS20].[Factory] ([Id], [Name], [CompanyId], [Description], [Latitude], [Longitude], [TimeZone], [CultureInfo]) VALUES (01, N'MS99Factory01 (Test)', 01, N'This id a test data', 0, 0,0 , N'zh-TW')
INSERT INTO [CDS20].[Factory] ([Id], [Name], [CompanyId], [Description], [Latitude], [Longitude], [TimeZone], [CultureInfo]) VALUES (99, N'MS99Factory99 (Test)', 01, N'This id a test data', 0, 0,0 , N'zh-TW')
SET IDENTITY_INSERT [CDS20].[Factory] OFF

SET IDENTITY_INSERT [CDS20].[Employee] ON
INSERT INTO [CDS20].[Employee]([Id],[CompanyId],[FirstName],[LastName],[Email],[Password],[AdminFlag]) VALUES (01,01,N'Admin',N'Test',N'AdminFlag',N'AFns/VMam+LzA9lb9bvrSlFyLSJf1fP/C/sU7hoHDCTMaaQLKEfX9yb7V3qyXLWtEg==',1)
INSERT INTO [CDS20].[Employee]([Id],[CompanyId],[FirstName],[LastName],[Email],[Password],[AdminFlag]) VALUES (02,01,N'Without Admin Flag',N'Test',N'NoAdminFlag',N'AFns/VMam+LzA9lb9bvrSlFyLSJf1fP/C/sU7hoHDCTMaaQLKEfX9yb7V3qyXLWtEg==',0)
SET IDENTITY_INSERT [CDS20].[Employee] OFF

SET IDENTITY_INSERT [CDS20].[UserRole] ON
INSERT INTO [CDS20].[UserRole]([Id],[CompanyId],[Name]) VALUES (01,01,N'All Alow')
SET IDENTITY_INSERT [CDS20].[UserRole] OFF

--SET IDENTITY_INSERT [CDS20].[UserRolePermission] ON
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (01,01,10)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (02,01,11)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (03,01,20)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (04,01,21)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (05,01,30)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (06,01,31)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (07,01,32)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (08,01,33)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (09,01,34)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (10,01,35)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (11,01,40)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (12,01,41)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (13,01,42)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (14,01,43)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (15,01,50)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (16,01,51)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (17,01,52)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (18,01,100)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (19,01,101)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (20,01,102)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (21,01,104)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (22,01,105)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (23,01,107)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (24,01,108)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (25,01,109)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (26,01,110)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (27,01,60)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (28,01,61)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (29,01,62)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (30,01,63)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (31,01,64)
--INSERT INTO [CDS20].[UserRolePermission]([Id],[UserRoleID],[PermissionCatalogCode]) VALUES (32,01,65)
--SET IDENTITY_INSERT [CDS20].[UserRolePermission] OFF

SET IDENTITY_INSERT [CDS20].[EmployeeInRole] ON
INSERT INTO [CDS20].[EmployeeInRole]([Id],[EmployeeID],[UserRoleID]) VALUES (01,01,01)
SET IDENTITY_INSERT [CDS20].[EmployeeInRole] OFF







