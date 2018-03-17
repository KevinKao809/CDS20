
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[CDS20].[WidgetClass]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[WidgetClass];
END
CREATE TABLE [CDS20].[WidgetClass] (
    [Id]                         INT            IDENTITY (1, 1) NOT NULL,
    [Name]                       NVARCHAR (50)  NOT NULL,
    [Key]                        INT            DEFAULT ((0)) NOT NULL,
    [Level]                      NVARCHAR (50)  DEFAULT ('equipment') NOT NULL,
    [PhotoURL]                   NVARCHAR (255) NULL,
    [AllowMultipleAppearOnBoard] BIT            DEFAULT ((1)) NOT NULL,
    [MessageBind]                BIT            DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Key] ASC)
);

GO
CREATE NONCLUSTERED INDEX [IX_WidgetClass_Column]
    ON [CDS20].[WidgetClass]([Level] ASC);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_WidgetClass_Column_1]
    ON [CDS20].[WidgetClass]([Key] ASC);

GO
CREATE Unique INDEX [IX_WidgetClass_Column_1] ON [CDS20].[WidgetClass] ([Key])

GO
IF OBJECT_ID(N'[CDS20].[SystemConfiguration]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[SystemConfiguration];
END
CREATE TABLE [CDS20].[SystemConfiguration] (
    [Id]    INT            NOT NULL,
    [Group] NVARCHAR (256) NULL,
    [Key]   NVARCHAR (256) NOT NULL,
    [Value] NVARCHAR (256) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_SystemConfiguration_Key]
    ON [CDS20].[SystemConfiguration]([Key] ASC);

GO
IF OBJECT_ID(N'[CDS20].[SuperAdmin]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[SuperAdmin];
END
CREATE TABLE [CDS20].[SuperAdmin] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]   NVARCHAR (50)  NULL,
    [LastName]    NVARCHAR (50)  NULL,
    [Email]       NVARCHAR (100) NULL,
    [Password]    NVARCHAR (255) NULL,
    [CreatedAt]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME       NULL,
    [DeletedFlag] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_SuperAdmin_Column]
    ON [CDS20].[SuperAdmin]([Email] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SuperAdmin_Column_1]
    ON [CDS20].[SuperAdmin]([DeletedFlag] ASC);

GO
IF OBJECT_ID(N'[CDS20].[SubscriptionPlan]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[SubscriptionPlan];
END
CREATE TABLE [CDS20].[SubscriptionPlan] (
    [Id]                               INT            IDENTITY (1, 1) NOT NULL,
    [Name]                             NVARCHAR (50)  NOT NULL,
    [Description]                             NVARCHAR (512) NULL,
    [DefaultRatePer1KMessageIngestion] FLOAT (53)     NULL,
    [DefaultRatePer1KMessageHotStore]  FLOAT (53)     NULL,
    [DefaultRatePer1KMessageColdStore] FLOAT (53)     NULL,
    [DefaultPlanDays]                  INT            DEFAULT ((0)) NOT NULL,
    [DefaultMaxMessageQuotaPerDay]     INT            DEFAULT ((0)) NOT NULL,
    [DefaultStoreHotMessage]           BIT            NULL,
    [DefaultStoreColdMessage]          BIT            NULL,
    [DefaultCosmosDBConnectionString]     NVARCHAR (512) NULL,
    [DefaultCollectionTTL]                  INT            DEFAULT ((86400)) NULL,
    [DefaultCollectionReservedUnits]        INT            DEFAULT ((400)) NULL,
    [DefaultIoTHubConnectionString]    NVARCHAR (512) NULL,
    [DefaultStorageConnectionString]   NVARCHAR (512) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
IF OBJECT_ID(N'[CDS20].[RefCultureInfo]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[RefCultureInfo];
END
CREATE TABLE [CDS20].[RefCultureInfo] (
    [CultureCode] NVARCHAR (10) NOT NULL,
    [Name]        NVARCHAR (30) NULL,
    PRIMARY KEY CLUSTERED ([CultureCode] ASC)
);

GO

IF OBJECT_ID(N'[CDS20].[PermissionCatalog]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[PermissionCatalog];
END
CREATE TABLE [CDS20].[PermissionCatalog] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (100) NULL,
    [Description] NVARCHAR (255) NULL,
    [Code]        INT            NOT NULL,
    CONSTRAINT [PK__Permissi__3214EC07553C9B80] PRIMARY KEY CLUSTERED ([Code] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PermissionCatalog_Name]
    ON [CDS20].[PermissionCatalog]([Name] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PermissionCatalog_PermissionId]
    ON [CDS20].[PermissionCatalog]([Code] ASC);
GO

IF OBJECT_ID(N'[CDS20].[MessageMandatoryElementDef]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[MessageMandatoryElementDef];
END
CREATE TABLE [CDS20].[MessageMandatoryElementDef] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [ElementName]     NVARCHAR (50)  NULL,
    [ElementDataType] NVARCHAR (20)  NULL,
    [MandatoryFlag]   BIT            DEFAULT ((1)) NOT NULL,
    [Description]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
IF OBJECT_ID(N'[CDS20].[IoTDeviceSystemConfiguration]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[IoTDeviceSystemConfiguration];
END
CREATE TABLE [CDS20].[IoTDeviceSystemConfiguration] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50)  NOT NULL,
    [DataType]     NVARCHAR (10)  NOT NULL,
    [Description]  NVARCHAR (255) CONSTRAINT [DF_IoTDeviceSystemConfiguration_Description] DEFAULT ('') NULL,
    [DefaultValue] NVARCHAR (50)  CONSTRAINT [DF_IoTDeviceSystemConfiguration_DefaultValue] DEFAULT ('') NULL,
    CONSTRAINT [PK__IoTDevic__3214EC0772516EE9] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_U_IoTDeviceSystemConfiguration_Name]
    ON [CDS20].[IoTDeviceSystemConfiguration]([Name] ASC);
GO
IF OBJECT_ID(N'[CDS20].[ErrorMessage]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[ErrorMessage];
END
CREATE TABLE [CDS20].[ErrorMessage] (
    [Id]       INT            NOT NULL,
    [Message]  NVARCHAR (256) NULL,
    [Category] NVARCHAR (50)  NULL,
    [App]      NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
IF OBJECT_ID(N'[CDS20].[DeviceType]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[DeviceType];
END
CREATE TABLE [CDS20].[DeviceType] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
IF OBJECT_ID(N'[CDS20].[Company]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[Company];
END
CREATE TABLE [CDS20].[Company] (
    [Id]                      INT             IDENTITY (1, 1) NOT NULL,
    [Name]                    NVARCHAR (100)  NOT NULL,
    [ShortName]               NVARCHAR (10)   NULL,
    [Address]                 NVARCHAR (255)  NULL,
    [CompanyWebSite]          NVARCHAR (255)  NULL,
    [ContactName]             NVARCHAR (50)   NULL,
    [ContactPhone]            NVARCHAR (50)   NULL,
    [ContactEmail]            NVARCHAR (50)   NULL,
    [Latitude]                FLOAT (53)      NULL,
    [Longitude]               FLOAT (53)      NULL,
    [LogoURL]                 NVARCHAR (255)  NULL,
	[IconURL]                 NVARCHAR (255)  NULL,
    [CultureInfo]             NVARCHAR (10)   NULL,
    [AllowDomain]             NVARCHAR (1000) NULL,
    [ExtAppAuthenticationKey] NVARCHAR (255)  NULL,
    [CreatedAt]               DATETIME        DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]               DATETIME        NULL,
    [DeletedFlag]             BIT             DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Company_CultureInfo] FOREIGN KEY ([CultureInfo]) REFERENCES [CDS20].[RefCultureInfo] ([CultureCode])
);


GO
CREATE NONCLUSTERED INDEX [IX_Company_Column]
    ON [CDS20].[Company]([DeletedFlag] ASC);

GO
IF OBJECT_ID(N'[CDS20].[AccumulateUsageLog]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[AccumulateUsageLog];
END
CREATE TABLE [CDS20].[AccumulateUsageLog] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [CompanyId]    INT      NOT NULL,
    [FactoryQty]   INT      NULL,
    [EquipmentQty] INT      NULL,
    [MessageQty]   BIGINT   NOT NULL,
    [DocSizeInMB]  BIGINT   NOT NULL,
    [BlobSizeInMB] INT      NOT NULL,
    [UpdatedAt]    DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AccumulateUsageLog_ToTable] FOREIGN KEY ([CompanyId]) REFERENCES [CDS20].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_AccumulateUsageLog_Column]
    ON [CDS20].[AccumulateUsageLog]([CompanyId] ASC, [UpdatedAt] ASC);

GO
IF OBJECT_ID(N'[CDS20].[MessageCatalog]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[MessageCatalog];
END
CREATE TABLE [CDS20].[MessageCatalog] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [CompanyID]                INT            NOT NULL,
    [Name]                     NVARCHAR (50)  NOT NULL,
    [Description]              NVARCHAR (255) NULL,
    [MonitorFrequenceInMinSec] INT            DEFAULT ((3000)) NULL,
    [ChildMessageFlag]         BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MessageCatalog_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [CDS20].[Company] ([Id])
);

GO
CREATE INDEX [IX_MessageCatalog_Column] ON [CDS20].[MessageCatalog] (CompanyID)

GO
IF OBJECT_ID(N'[CDS20].[MessageElement]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[MessageElement];
END
CREATE TABLE [CDS20].[MessageElement] (
    [Id]                    INT           IDENTITY (1, 1) NOT NULL,
    [MessageCatalogID]      INT           NOT NULL,
    [ElementName]           NVARCHAR (50) NULL,
    [ElementDataType]       NVARCHAR (20) NULL,
    [ChildMessageCatalogID] INT           NULL,
    [MandatoryFlag]         BIT           DEFAULT ((0)) NOT NULL,
    [CDSMandatoryFlag]       BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MessageElement_MessageCatalogID] FOREIGN KEY ([MessageCatalogID]) REFERENCES [CDS20].[MessageCatalog] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_MessageElement_Column]
    ON [CDS20].[MessageElement]([MessageCatalogID] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_U_MessageElement_Column_1]
    ON [CDS20].[MessageElement]([MessageCatalogID] ASC, [ElementName] ASC);


GO
IF OBJECT_ID(N'[CDS20].[EventRuleCatalog]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[EventRuleCatalog];
END
CREATE TABLE [CDS20].[EventRuleCatalog] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]        INT            NOT NULL,
    [MessageCatalogId] INT            NOT NULL,
    [Name]             NVARCHAR (50)  NOT NULL,
    [Description]      NVARCHAR (255) NULL,
    [AggregateInSec]   INT            DEFAULT ((60)) NOT NULL,
    [ActiveFlag]       BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EventRuleCatalog_ToTable_1] FOREIGN KEY ([MessageCatalogId]) REFERENCES [CDS20].[MessageCatalog] ([Id]),
    CONSTRAINT [FK_EventRuleCatalog_ToTable] FOREIGN KEY ([CompanyId]) REFERENCES [CDS20].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_EventRuleCatalog_Column]
    ON [CDS20].[EventRuleCatalog]([CompanyId] ASC);

GO
IF OBJECT_ID(N'[CDS20].[EventRuleItem]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[EventRuleItem];
END
CREATE TABLE [CDS20].[EventRuleItem] (
    [Id]                     INT           IDENTITY (1, 1) NOT NULL,
    [EventRuleCatalogId]     INT           NOT NULL,
    [Ordering]               INT           NOT NULL,
    [MessageElementParentId] INT           NULL,
    [MessageElementId]       INT           NOT NULL,
    [EqualOperation]         NVARCHAR (20) NOT NULL,
    [Value]                  NVARCHAR (50) NOT NULL,
    [BitWiseOperation]       NVARCHAR (10) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EventRuleItem_ToTable] FOREIGN KEY ([EventRuleCatalogId]) REFERENCES [CDS20].[EventRuleCatalog] ([Id]),
    CONSTRAINT [FK_EventRuleItem_ToTable_1] FOREIGN KEY ([MessageElementId]) REFERENCES [CDS20].[MessageElement] ([Id]),
    CONSTRAINT [FK_EventRuleItem_ParentMessageElement] FOREIGN KEY ([MessageElementParentId]) REFERENCES [CDS20].[MessageElement] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_EventRuleItem_Column]
    ON [CDS20].[EventRuleItem]([EventRuleCatalogId] ASC, [Ordering] ASC);

GO
IF OBJECT_ID(N'[CDS20].[Application]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[Application];
END
CREATE TABLE [CDS20].[Application] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [CompanyId]       INT             NOT NULL,
    [Name]            NVARCHAR (50)   NOT NULL,
    [Description]     NVARCHAR (255)  NULL,
    [MessageTemplate] NVARCHAR (2000) NULL,
    [TargetType]      NVARCHAR (50)   NOT NULL,
    [Method]          NVARCHAR (10)   NOT NULL,
    [ServiceURL]      NVARCHAR (255)  NOT NULL,
    [AuthType]        NVARCHAR (20)   NOT NULL,
    [AuthID]          NVARCHAR (50)   NULL,
    [AuthPW]          NVARCHAR (50)   NULL,
    [TokenURL]        NVARCHAR (255)  NULL,
    [HeaderValues]    NVARCHAR (255)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Application_ToTable] FOREIGN KEY ([CompanyId]) REFERENCES [CDS20].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Application_Column]
    ON [CDS20].[Application]([CompanyId] ASC);

GO
IF OBJECT_ID(N'[CDS20].[EventInAction]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[EventInAction];
END
CREATE TABLE [CDS20].[EventInAction] (
    [Id]                 INT IDENTITY (1, 1) NOT NULL,
    [EventRuleCatalogId] INT NULL,
    [ApplicationId]      INT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EventInAction_ToTable] FOREIGN KEY ([EventRuleCatalogId]) REFERENCES [CDS20].[EventRuleCatalog] ([Id]),
    CONSTRAINT [FK_EventInAction_ToTable_1] FOREIGN KEY ([ApplicationId]) REFERENCES [CDS20].[Application] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EventInAction_Column]
    ON [CDS20].[EventInAction]([EventRuleCatalogId] ASC, [ApplicationId] ASC);

GO
IF OBJECT_ID(N'[CDS20].[CompanyInSubscriptionPlan]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[CompanyInSubscriptionPlan];
END
CREATE TABLE [CDS20].[CompanyInSubscriptionPlan] (
    [Id]                             INT            IDENTITY (1, 1) NOT NULL,
    [CompanyID]                      INT            NOT NULL,
    [SubscriptionPlanID]             INT            NOT NULL,
    [SubscriptionName]               NVARCHAR (256) NULL,
    [RatePer1KMessageIngestion]      FLOAT (53)     DEFAULT ((0)) NULL,
    [RatePer1KMessageHotStore]       FLOAT (53)     DEFAULT ((0)) NULL,
    [RatePer1KMessageColdStore]      FLOAT (53)     DEFAULT ((0)) NULL,
    [StartDate]                      DATETIME       DEFAULT (getdate()) NOT NULL,
    [ExpiredDate]                    DATETIME       DEFAULT (getdate()) NOT NULL,
    [MaxMessageQuotaPerDay]          INT            DEFAULT ((0)) NOT NULL,
    [StoreHotMessage]                BIT            DEFAULT ((1)) NOT NULL,
    [StoreColdMessage]               BIT            DEFAULT ((1)) NOT NULL,
    [CosmosDBConnectionString]          NVARCHAR (512) NOT NULL,
    [CosmosDBName]                      NVARCHAR (50)  NOT NULL,
    [CosmosDBCollectionID]              NVARCHAR (50)  NOT NULL,
    [CosmosDBCollectionTTL]                       INT            DEFAULT ((86400)) NULL,
    [CosmosDBCollectionReservedUnits]             INT            DEFAULT ((400)) NULL,
    [IoTHubConnectionString]  NVARCHAR (512) NOT NULL,
    [IoTHubConsumerGroup]     NVARCHAR (50)  NOT NULL,
    [StorageConnectionString] NVARCHAR (512) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompanyInSubscriptionPlan_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [CDS20].[Company] ([Id]),
    CONSTRAINT [FK_CompanyInSubscriptionPlan_SubscriptionPlanID] FOREIGN KEY ([SubscriptionPlanID]) REFERENCES [CDS20].[SubscriptionPlan] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyInSubscriptionPlan_Column]
    ON [CDS20].[CompanyInSubscriptionPlan]([CompanyID] ASC, [SubscriptionPlanID] ASC, [StartDate] ASC, [ExpiredDate] ASC);

GO
IF OBJECT_ID(N'[CDS20].[DeviceCertificate]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[DeviceCertificate];
END
CREATE TABLE [CDS20].[DeviceCertificate] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [CompanyID]  INT            NOT NULL,
    [Name]       NVARCHAR (100) NOT NULL,
    [CertFile]   NVARCHAR (100) NULL,
    [KeyFile]    NVARCHAR (100) NULL,
    [Thumbprint] NVARCHAR (200) NOT NULL,
    [Password]   NVARCHAR (50)  NOT NULL,
    [ExpiredAt]  DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DeviceCertificate_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [CDS20].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_DeviceCertificate_Column]
    ON [CDS20].[DeviceCertificate]([CompanyID] ASC);

GO
IF OBJECT_ID(N'[CDS20].[Employee]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[Employee];
END
CREATE TABLE [CDS20].[Employee] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]      INT            NOT NULL,
    [EmployeeNumber] NVARCHAR (50)  NULL,
    [FirstName]      NVARCHAR (50)  NULL,
    [LastName]       NVARCHAR (50)  NULL,
    [Email]          NVARCHAR (100) NOT NULL,
    [PhotoURL]       NVARCHAR (255) NULL,
	[IconURL]       NVARCHAR (255) NULL,
    [Password]       NVARCHAR (255) NOT NULL,
    [AdminFlag]      BIT            DEFAULT ((0)) NOT NULL,
    [Lang]           NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Employee_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [CDS20].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Employee_Column]
    ON [CDS20].[Employee]([CompanyId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_U_Employee_Email]
    ON [CDS20].[Employee]([Email] ASC);


GO
IF OBJECT_ID(N'[CDS20].[UserRole]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[UserRole];
END
CREATE TABLE [CDS20].[UserRole] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [CompanyId] INT           NOT NULL,
    [Name]      NVARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserRole_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [CDS20].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_UserRole_Column]
    ON [CDS20].[UserRole]([CompanyId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_U_UserRole_Column_1]
    ON [CDS20].[UserRole]([CompanyId] ASC, [Name] ASC);


GO
IF OBJECT_ID(N'[CDS20].[UserRolePermission]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[UserRolePermission];
END
CREATE TABLE [CDS20].[UserRolePermission] (
    [Id]                    INT IDENTITY (1, 1) NOT NULL,
    [UserRoleID]            INT NOT NULL,
    [PermissionCatalogCode] INT NOT NULL,
    CONSTRAINT [PK__UserRole__3214EC0784ACBB00] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserRolePermission_ToTable] FOREIGN KEY ([UserRoleID]) REFERENCES [CDS20].[UserRole] ([Id]),
    CONSTRAINT [FK_UserRolePermission_ToTable_1] FOREIGN KEY ([PermissionCatalogCode]) REFERENCES [CDS20].[PermissionCatalog] ([Code])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_U_UserRolePermission_Column_1]
    ON [CDS20].[UserRolePermission]([UserRoleID] ASC, [PermissionCatalogCode] ASC);

GO
IF OBJECT_ID(N'[CDS20].[EmployeeInRole]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[EmployeeInRole];
END
CREATE TABLE [CDS20].[EmployeeInRole] (
    [Id]         INT IDENTITY (1, 1) NOT NULL,
    [EmployeeID] INT NOT NULL,
    [UserRoleID] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeInRole_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [CDS20].[Employee] ([Id]),
    CONSTRAINT [FK_EmployeeInRole_UserRoleID] FOREIGN KEY ([UserRoleID]) REFERENCES [CDS20].[UserRole] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EmployeeInRole_Column]
    ON [CDS20].[EmployeeInRole]([EmployeeID] ASC, [UserRoleID] ASC);

GO
IF OBJECT_ID(N'[CDS20].[IoTHub]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[IoTHub];
END
CREATE TABLE [CDS20].[IoTHub] (
    [Id]                              INT            IDENTITY (1, 1) NOT NULL,
    [IoTHubName]                      NVARCHAR (50)  NOT NULL,
    [Description]                     NVARCHAR (255) NULL,
    [CompanyID]                       INT            NOT NULL,
    [IoTHubEndPoint]                  NVARCHAR (50)  NULL,
    [IoTHubConnectionString]          NVARCHAR (256) NULL,
    [EventConsumerGroup]              NVARCHAR (50)  NULL,
    [EventHubStorageConnectionString] NVARCHAR (256) NULL,
    [UploadContainer]                 NVARCHAR (50)  NULL,
	[EnableMultipleReceiver]		  BIT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_IoTHub_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [CDS20].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_IoTHub_Column]
    ON [CDS20].[IoTHub]([CompanyID] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_IoTHub_Column_1]
    ON [CDS20].[IoTHub]([CompanyID] ASC, [IoTHubName] ASC);

GO
IF OBJECT_ID(N'[CDS20].[EquipmentClass]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[EquipmentClass];
END
CREATE TABLE [CDS20].[EquipmentClass] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]   INT            NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EquipmentClass_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [CDS20].[Company] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EquipmentClass_Column_1]
    ON [CDS20].[EquipmentClass]([CompanyId] ASC, [Name] ASC);

GO
IF OBJECT_ID(N'[CDS20].[EquipmentClassMessageCatalog]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[EquipmentClassMessageCatalog];
END
CREATE TABLE [CDS20].[EquipmentClassMessageCatalog] (
    [Id]               INT IDENTITY (1, 1) NOT NULL,
    [EquipmentClassID] INT NOT NULL,
    [MessageCatalogID] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EquipmentClassMessageCatalog_IoTHubDeviceID] FOREIGN KEY ([EquipmentClassID]) REFERENCES [CDS20].[EquipmentClass] ([Id]),
    CONSTRAINT [FK_EquipmentClassMessageCatalog_MessageCatalogID] FOREIGN KEY ([MessageCatalogID]) REFERENCES [CDS20].[MessageCatalog] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EquipmentClassMessageCatalog_Column]
    ON [CDS20].[EquipmentClassMessageCatalog]([EquipmentClassID] ASC, [MessageCatalogID] ASC);

GO
IF OBJECT_ID(N'[CDS20].[ExternalDashboard]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[ExternalDashboard];
END
CREATE TABLE [CDS20].[Dashboard] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [CompanyID]   INT            NOT NULL,
    [Level]       NVARCHAR (50)  NOT NULL,
    [ReferenceID] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Dashboard_ToTable] FOREIGN KEY ([CompanyID]) REFERENCES [CDS20].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalDashboard_Column]
    ON [CDS20].[ExternalDashboard]([CompanyId] ASC, [Order] ASC);

GO
IF OBJECT_ID(N'[CDS20].[Factory]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[Factory];
END
CREATE TABLE [CDS20].[Factory] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (255) NULL,
    [CompanyId]   INT            DEFAULT ((0)) NOT NULL,
    [Latitude]    FLOAT (53)     DEFAULT ((0)) NULL,
    [Longitude]   FLOAT (53)     DEFAULT ((0)) NULL,
    [PhotoURL]    NVARCHAR (255) NULL,
    [TimeZone]    INT            DEFAULT ((0)) NOT NULL,
    [CultureInfo] NVARCHAR (10)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Factory_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [CDS20].[Company] ([Id]),
    CONSTRAINT [FK_Factory_CultureInfo] FOREIGN KEY ([CultureInfo]) REFERENCES [CDS20].[RefCultureInfo] ([CultureCode])
);


GO
CREATE NONCLUSTERED INDEX [IX_Factory_Column]
    ON [CDS20].[Factory]([CompanyId] ASC);

GO
IF OBJECT_ID(N'[CDS20].[IoTDevice]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[IoTDevice];
END
CREATE TABLE [CDS20].[IoTDevice] (
    [Id]                        INT             IDENTITY (1, 1) NOT NULL,
    [IoTHubDeviceID]            NVARCHAR (256)  NOT NULL,
    [IoTHubDevicePW]            NVARCHAR (256)  NULL,
    [IoTHubDeviceKey]           NVARCHAR (256)  NULL,
    [IoTHubID]                  INT             NOT NULL,
    [IoTHubProtocol]            NVARCHAR (128)  NULL,
    [CompanyID]                 INT             NOT NULL,
    [FactoryID]                 INT             NOT NULL,
    [AuthenticationType]        NVARCHAR (20)   NULL,
    [DeviceCertificateID]       INT             NULL,
    [DeviceTypeId]              INT             NOT NULL,
    [DeviceVendor]              NVARCHAR (128)  NULL,
    [DeviceModel]               NVARCHAR (128)  NULL,
	[OriginMessage]				NVARCHAR (MAX)  NULL,
	[MessageConvertScript]	    NVARCHAR (Max)  NULL,
	[EnableMessageConvert]      BIT      DEFAULT ((0)) NOT NULL    
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_IoTDevice_IoTHubID] FOREIGN KEY ([IoTHubID]) REFERENCES [CDS20].[IoTHub] ([Id]),
    CONSTRAINT [FK_IoTDevice_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [CDS20].[Company] ([Id]),
    CONSTRAINT [FK_IoTDevice_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [CDS20].[Factory] ([Id]),
    CONSTRAINT [FK_IoTDevice_DeviceTypeId] FOREIGN KEY ([DeviceTypeId]) REFERENCES [CDS20].[DeviceType] ([Id]),
    CONSTRAINT [FK_IoTDevice_DeviceCertificateID] FOREIGN KEY ([DeviceCertificateID]) REFERENCES [CDS20].[DeviceCertificate] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_IoTDevice_Column]
    ON [CDS20].[IoTDevice]([IoTHubDeviceID] ASC, [IoTHubID] ASC, [CompanyID] ASC, [FactoryID] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_IoTDevice_UNIQUE]
    ON [CDS20].[IoTDevice]([CompanyID] ASC, [IoTHubDeviceID] ASC);

GO
IF OBJECT_ID(N'[CDS20].[IoTDeviceCustomizedConfiguration]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[IoTDeviceCustomizedConfiguration];
END
CREATE TABLE [CDS20].[IoTDeviceCustomizedConfiguration] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]    INT            NOT NULL,
    [Name]         NVARCHAR (50)  NOT NULL,
    [DataType]     NVARCHAR (10)  NOT NULL,
    [Description]  NVARCHAR (255) NULL,
    [DefaultValue] NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Company_IoTDeviceCustomizedConfiguration] FOREIGN KEY ([CompanyId]) REFERENCES [CDS20].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_IoTDeviceCustomizedConfiguration]
    ON [CDS20].[IoTDeviceCustomizedConfiguration]([CompanyId] ASC);

GO
IF OBJECT_ID(N'[CDS20].[OperationTask]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[OperationTask];
END
CREATE TABLE [CDS20].[OperationTask] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50)   NOT NULL,
    [TaskStatus]   NVARCHAR (20)   NULL,
    [CompletedAt]  DATETIME        NULL,
    [RetryCounter] INT             NULL,
    [CompanyId]    INT             NOT NULL,
    [Entity]       NVARCHAR (50)   NULL,
    [EntityId]     NVARCHAR (50)   NULL,
    [TaskContent]  NVARCHAR (1500) NULL,
    [TaskLog]      NVARCHAR (3000) NULL,
    [CreatedAt]    DATETIME        DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]    DATETIME        NULL,
    [DeletedFlag]  BIT             DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Task_Column]
    ON [CDS20].[OperationTask]([TaskStatus] ASC, [CompanyId] ASC, [DeletedFlag] ASC);

GO
IF OBJECT_ID(N'[CDS20].[WidgetCatalog]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[WidgetCatalog];
END
CREATE TABLE [CDS20].[WidgetCatalog] (
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [DashboardID]      INT             DEFAULT ((0)) NOT NULL,
    [MessageCatalogID] INT             NULL,
    [Name]             NVARCHAR (50)   NOT NULL,
    [Level]            NVARCHAR (50)   DEFAULT ('equipment') NOT NULL,
    [WidgetClassKey]   INT             NOT NULL,
    [Title]            NVARCHAR (50)   DEFAULT ('Title') NOT NULL,
    [TitleBgColor]     NVARCHAR (10)   NULL,
    [Content]          NVARCHAR (2000) DEFAULT ('{}') NOT NULL,
    [ContentBgColor]   NVARCHAR (10)   NULL,
    [RowNo]            INT             NOT NULL,
    [ColumnSeq]        INT             NULL,
    [WidthSpace]       INT             NOT NULL,
    [HeightPixel]      INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WidgetCatalog_ToTable_2] FOREIGN KEY ([MessageCatalogID]) REFERENCES [CDS20].[MessageCatalog] ([Id]),
    CONSTRAINT [FK_WidgetCatalog_ToTable] FOREIGN KEY ([WidgetClassKey]) REFERENCES [CDS20].[WidgetClass] ([Key])
);


GO
CREATE NONCLUSTERED INDEX [IX_WidgetCatalog_Column]
    ON [CDS20].[WidgetCatalog]([DashboardID] ASC, [MessageCatalogID] ASC, [Level] ASC, [WidgetClassKey] ASC);

GO
IF OBJECT_ID(N'[CDS20].[Equipment]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[Equipment];
END
CREATE TABLE [CDS20].[Equipment] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [EquipmentId]      NVARCHAR (50)  NOT NULL,
    [Name]             NVARCHAR (50)  NOT NULL,
    [EquipmentClassId] INT            NOT NULL,
    [CompanyID]        INT            NOT NULL,
    [FactoryId]        INT            NOT NULL,
    [IoTDeviceID]      INT            NOT NULL,
    [Latitude]         FLOAT (53)     NULL,
    [Longitude]        FLOAT (53)     NULL,
    [MaxIdleInSec]     INT            DEFAULT ((30)) NOT NULL,
    [PhotoURL]         NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Equipment_ToTable_2] FOREIGN KEY ([EquipmentClassId]) REFERENCES [CDS20].[EquipmentClass] ([Id]),
    CONSTRAINT [FK_Equipment_ToTable] FOREIGN KEY ([FactoryId]) REFERENCES [CDS20].[Factory] ([Id]),
    CONSTRAINT [FK_Equipment_ToTable_1] FOREIGN KEY ([CompanyID]) REFERENCES [CDS20].[Company] ([Id]),
    CONSTRAINT [FK_Equipment_ToTable_3] FOREIGN KEY ([IoTDeviceID]) REFERENCES [CDS20].[IoTDevice] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Equipment_Column]
    ON [CDS20].[Equipment]([EquipmentId] ASC, [CompanyID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Equipment_Column_1]
    ON [CDS20].[Equipment]([FactoryId] ASC, [EquipmentId] ASC);

GO
IF OBJECT_ID(N'[CDS20].[Dashboard]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[Dashboard];
END
CREATE TABLE [CDS20].[Dashboard] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [CompanyID]        INT           NOT NULL,
    [DashboardType]    NVARCHAR (50) NULL,
    [FactoryID]        INT           NULL,
    [EquipmentClassID] INT           NULL,
    [EquipmentID]      INT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Dashboard_ToTable_3] FOREIGN KEY ([EquipmentID]) REFERENCES [CDS20].[Equipment] ([Id]),
    CONSTRAINT [FK_Dashboard_ToTable] FOREIGN KEY ([CompanyID]) REFERENCES [CDS20].[Company] ([Id]),
    CONSTRAINT [FK_Dashboard_ToTable_1] FOREIGN KEY ([EquipmentClassID]) REFERENCES [CDS20].[EquipmentClass] ([Id]),
    CONSTRAINT [FK_Dashboard_ToTable_2] FOREIGN KEY ([FactoryID]) REFERENCES [CDS20].[Factory] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Dashboard_Column]
    ON [CDS20].[Dashboard]([CompanyID] ASC, [FactoryID] ASC, [EquipmentClassID] ASC, [EquipmentID] ASC);


GO

IF OBJECT_ID(N'[CDS20].[APIServiceRole]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[APIServiceRole];
END
CREATE TABLE [CDS20].[APIServiceRole] (
    [Name] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_APIServiceRole] PRIMARY KEY CLUSTERED ([Name] ASC)
);

GO

IF OBJECT_ID(N'[CDS20].[APIServiceRefreshToken]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[APIServiceRefreshToken];
END
CREATE TABLE [CDS20].[APIServiceRefreshToken] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [ClientId]        NVARCHAR (50)  NOT NULL,
    [UserId]          INT            NOT NULL,
    [RefreshToken]    NVARCHAR (255) NOT NULL,
    [ProtectedTicket] NVARCHAR (500) NOT NULL,
    [DeletedFlag]     BIT            NOT NULL,
    [IssuedAt]        DATETIME       NOT NULL,
    [ExpiredAt]       DATETIME       NOT NULL,
    [CreatedAt]       DATETIME       NOT NULL,
    [UpdatedAt]       DATETIME       NOT NULL,
    CONSTRAINT [PK_APIServiceRefreshToken] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_APIServiceRefreshToken]
    ON [CDS20].[APIServiceRefreshToken]([RefreshToken] ASC);

GO

IF OBJECT_ID(N'[CDS20].[EquipmentEnrollment]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [CDS20].[EquipmentEnrollment];
END
CREATE TABLE [CDS20].[EquipmentEnrollment]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[CompanyId] INT NOT NULL,
	[FactoryId] InT NOT NULL,
	[EquipmentClassId] INT NOT NULL,
	[EquipmentId] INT NOT NULL,
	[ActivationKey] varchar(255) NULL, 
	[EnrollDate] DATETIME NULL,
	[SuperAdminId] INT NOT NULL,
    [IsActivated] BIT NULL, 
    [ActivationDate] DATETIME NULL, 
    [ActivationExpiredAt] DATETIME NULL, 
    CONSTRAINT [FK_EquipmentEnrollment_Company] FOREIGN KEY (CompanyId) REFERENCES CDS20.Company(ID), 
    CONSTRAINT [FK_EquipmentEnrollment_Factory] FOREIGN KEY (FactoryId) REFERENCES CDS20.Factory(ID), 
    CONSTRAINT [FK_EquipmentEnrollment_EquipmentClass] FOREIGN KEY (EquipmentClassId) REFERENCES CDS20.EquipmentClass(ID), 
    CONSTRAINT [FK_EquipmentEnrollment_SuperAdmin] FOREIGN KEY (SuperAdminId) REFERENCES CDS20.SuperAdmin(ID),
)

GO

CREATE INDEX [IX_EquipmentEnrollment_Column] ON [CDS20].[EquipmentEnrollment] ([EquipmentId], IsActivated)
GO

CREATE TABLE CDS20.MetaDataDefination
(
	[Id] INT Identity(1,1) NOT NULL PRIMARY KEY, 
    [CompanyId] INT NOT NULL, 
    [EntityType] NVARCHAR(50) NOT NULL, 
    [ObjectName] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_MetaDataDefination_ToTable] FOREIGN KEY (CompanyId) REFERENCES CDS20.Company(Id) 
)

GO

CREATE unique INDEX [IX_MetaDataDefination_Column] ON CDS20.[MetaDataDefination] (CompanyId, [EntityType], [ObjectName])
GO

CREATE TABLE CDS20.MetaDataValue
(
	[Id] INT Identity(1,1) NOT NULL PRIMARY KEY, 
    [MetaDataDefinationId] INT NOT NULL, 
    [ObjectValue] NVARCHAR(MAX) NULL, 
    [ReferenceId] INT NOT NULL, 
    CONSTRAINT [FK_MetaDataValue_ToTable] FOREIGN KEY (MetaDataDefinationId) REFERENCES CDS20.MetaDataDefination(ID)
)

GO

CREATE INDEX [IX_MetaDataValue_Column] ON CDS20.[MetaDataValue] (MetaDataDefinationId, ReferenceId)
GO

CREATE TRIGGER CDS20.TR_MetaDataDef_Delete ON CDS20.[MetaDataDefination] INSTEAD OF DELETE
	AS
	BEGIN
		Delete CDS20.MetaDataValue WHERE MetaDataDefinationId IN (SELECT deleted.Id from deleted);
		Delete CDS20.MetaDataDefination Where Id IN (SELECT deleted.Id from deleted);
	END
GO


CREATE TABLE [CDS20].[DeviceCommandCatalog] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId] INT            NOT NULL,
    [Method]    NVARCHAR (50)  NOT NULL,
    [Name]      NVARCHAR (100) NOT NULL,
    [Content]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_IoTDeviceCommandCatalog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DeviceCommandCatalog_ToTable] FOREIGN KEY ([CompanyId]) REFERENCES [CDS20].[Company] ([Id])
);
GO

CREATE INDEX [IX_DeviceCommandCatalog_Column] ON [CDS20].[DeviceCommandCatalog] (CompanyId)
GO

CREATE Unique INDEX [IX_DeviceCommandCatalog_Column_1] ON [CDS20].[DeviceCommandCatalog] ([CompanyId], [Name])
GO

CREATE TABLE [CDS20].[IoTDeviceCommand] (
    [Id]              INT IDENTITY (1, 1) NOT NULL,
    [IoTDeviceId]     INT NOT NULL,
    [DeviceCommandId] INT NOT NULL,
    CONSTRAINT [PK_IoTDeviceCommand] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_IoTDeviceCommand_ToTable] FOREIGN KEY (IoTDeviceId) REFERENCES CDS20.IoTDevice(ID), 
    CONSTRAINT [FK_IoTDeviceCommand_ToTable_1] FOREIGN KEY (DeviceCommandId) REFERENCES CDS20.DeviceCommandCatalog(ID)
);
GO


CREATE TRIGGER CDS20.TR_Company_Insert on CDS20.Company AFTER Insert
AS
Begin
	insert into CDS20.UserRole(CompanyID, Name) select ID, 'Default' from inserted
	insert into CDS20.EquipmentClass(CompanyId, Name) select ID, 'Default' from inserted	
End
GO
CREATE TRIGGER CDS20.TR_UserRole_Delete ON [CDS20].[UserRole] INSTEAD OF DELETE
	AS
	BEGIN
		Delete CDS20.UserRolePermission WHERE UserRoleID IN (SELECT deleted.Id from deleted);
		Delete CDS20.EmployeeInRole Where UserRoleID IN (SELECT deleted.Id from deleted);
		Delete CDS20.UserRole Where Id IN (SELECT deleted.Id from deleted);
	END
GO
CREATE TRIGGER [CDS20].[TR_PermissionCatalog_Delete] ON [CDS20].[PermissionCatalog] INSTEAD OF DELETE
	AS
	BEGIN
		Delete CDS20.UserRolePermission WHERE PermissionCatalogCode IN (SELECT deleted.Code from deleted);
		Delete CDS20.PermissionCatalog WHERE Id IN (SELECT deleted.Id from deleted);
	END
GO
CREATE TRIGGER CDS20.TR_MessageCatalog_Insert on CDS20.MessageCatalog AFTER Insert
AS
Begin
	DECLARE @ID int, @ChildMessage bit
	SET @ID = (SELECT Id FROM inserted)
	SET @ChildMessage = (SELECT ChildMessageFlag FROM inserted)

	IF (@ChildMessage = 0)
	BEGIN
		insert into CDS20.MessageElement (MessageCatalogID, ElementName, ElementDataType, MandatoryFlag, CDSMandatoryFlag) 
			select @ID, ElementName, ElementDataType, MandatoryFlag, MandatoryFlag from CDS20.MessageMandatoryElementDef;
	End
End
GO
CREATE TRIGGER CDS20.TR_MessageCatalog_Delete ON [CDS20].[MessageCatalog] INSTEAD OF DELETE
	AS
	BEGIN
		Delete CDS20.MessageElement WHERE MessageCatalogID IN (SELECT deleted.Id from deleted);
		Delete CDS20.EquipmentClassMessageCatalog Where MessageCatalogID IN (SELECT deleted.Id from deleted);
		Delete CDS20.WidgetCatalog Where MessageCatalogID IN (SELECT deleted.Id from deleted);
		Delete CDS20.MessageCatalog Where Id IN (SELECT deleted.Id from deleted);
	END
GO

CREATE TRIGGER CDS20.TR_Factory_Delete on CDS20.Factory INSTEAD OF DELETE
AS
Begin	
	Delete CDS20.Dashboard where DashboardType = 'factory' and FactoryID in (SELECT deleted.Id from deleted);
	Delete CDS20.Factory where Id in (SELECT deleted.Id from deleted);
End
GO
CREATE TRIGGER CDS20.TR_EquipmentClass_Insert on CDS20.EquipmentClass AFTER Insert
AS
Begin	
	insert into CDS20.Dashboard(CompanyID , DashboardType, EquipmentClassID) select A.CompanyId, 'equipmentclass', A.Id from inserted A
End
GO
CREATE TRIGGER CDS20.TR_EquipmentClass_Delete ON [CDS20].[EquipmentClass] INSTEAD OF DELETE
	AS
	BEGIN
		Delete CDS20.EquipmentClassMessageCatalog WHERE EquipmentClassID IN (SELECT deleted.Id from deleted);
		Delete CDS20.Dashboard WHERE EquipmentClassID in (SELECT deleted.Id from deleted);
		Delete CDS20.EquipmentClass WHERE Id in (SELECT deleted.Id from deleted);
	END
GO

CREATE TRIGGER CDS20.TR_Dashboard_Delete on CDS20.Dashboard INSTEAD OF DELETE
AS
Begin
	Delete CDS20.DashboardWidgets where DashboardID IN (SELECT deleted.Id from deleted);
	Delete CDS20.Dashboard where Id IN (SELECT deleted.Id from deleted);
End
GO
CREATE TRIGGER CDS20.TR_Application_Delete ON [CDS20].[Application] INSTEAD OF DELETE
AS
BEGIN
	Delete CDS20.EventInAction WHERE ApplicationId IN (SELECT deleted.Id from deleted);
	Delete CDS20.[Application] WHERE ID IN (SELECT deleted.Id from deleted);
END
GO
CREATE TRIGGER CDS20.TR_EventRuleCatalog_Delete ON [CDS20].[EventRuleCatalog] INSTEAD OF DELETE
AS
BEGIN
	Delete CDS20.EventInAction WHERE EventRuleCatalogId IN (SELECT deleted.Id from deleted);
	Delete CDS20.EventRuleItem Where EventRuleCatalogId IN (SELECT deleted.Id from deleted);
	Delete CDS20.EventRuleCatalog Where Id IN (SELECT deleted.Id from deleted);
END
GO

CREATE TRIGGER [CDS20].[TR_Employee_Delete] ON [CDS20].[Employee] INSTEAD OF DELETE
	AS
	BEGIN
		Delete CDS20.EmployeeInRole WHERE EmployeeID IN (SELECT deleted.Id from deleted);	
		Delete CDS20.Employee where Id in (SELECT deleted.Id from deleted);	
	END
GO