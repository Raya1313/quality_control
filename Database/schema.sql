
IF DB_ID(N'QC1') IS NULL
    CREATE DATABASE QC1;
GO
USE QC1;
GO

/* ---------- Section 1: project ---------- */

IF OBJECT_ID(N'dbo.Project') IS NULL
CREATE TABLE dbo.Project (
    Id             INT IDENTITY PRIMARY KEY,
    NameFa         NVARCHAR(200) NOT NULL,
    NameEn         NVARCHAR(200) NOT NULL DEFAULT N'',
    ClientFa       NVARCHAR(200) NOT NULL DEFAULT N'',
    ClientEn       NVARCHAR(200) NOT NULL DEFAULT N'',
    ContractorFa   NVARCHAR(200) NOT NULL DEFAULT N'',
    ContractorEn   NVARCHAR(200) NOT NULL DEFAULT N'',
    QcManagerFa    NVARCHAR(200) NOT NULL DEFAULT N'',
    QcManagerEn    NVARCHAR(200) NOT NULL DEFAULT N'',
    QaManagerFa    NVARCHAR(200) NOT NULL DEFAULT N'',
    QaManagerEn    NVARCHAR(200) NOT NULL DEFAULT N'',
    CoordinatorFa  NVARCHAR(200) NOT NULL DEFAULT N'',
    CoordinatorEn  NVARCHAR(200) NOT NULL DEFAULT N'',
    ContractDate   DATE          NOT NULL,
    PhaseCount     INT           NOT NULL DEFAULT 1,
    ReportedPhase  NVARCHAR(100) NOT NULL DEFAULT N'',
    Logo           VARBINARY(MAX) NULL
);
GO

/* ---------- Section 1: parts (Assembly List) ---------- */

IF OBJECT_ID(N'dbo.Part') IS NULL
CREATE TABLE dbo.Part (
    Id           INT IDENTITY PRIMARY KEY,
    ProjectId    INT NOT NULL REFERENCES dbo.Project(Id),
    PartName     NVARCHAR(50)  NOT NULL,
    PartCode     NVARCHAR(50)  NULL,
    DrawingCode  NVARCHAR(100) NULL,
    Qty          INT NOT NULL CHECK (Qty > 0),
    Length       DECIMAL(18,2) NULL,
    Weight       DECIMAL(18,3) NULL,
    CONSTRAINT UQ_Part_Name UNIQUE (ProjectId, PartName)          -- PDF note 2: no duplicates
);
GO

/* ---------- Section 1: connections (Part List) ---------- */

IF OBJECT_ID(N'dbo.Connection') IS NULL
CREATE TABLE dbo.Connection (
    Id              INT IDENTITY PRIMARY KEY,
    ProjectId       INT NOT NULL REFERENCES dbo.Project(Id),
    ConnectionCode  NVARCHAR(50) NOT NULL,
    PlateThickness  DECIMAL(9,1)  NULL,
    Qty             INT           NULL,
    Weight          DECIMAL(18,2) NULL,
    CONSTRAINT UQ_Connection_Code UNIQUE (ProjectId, ConnectionCode)  -- PDF note 2
);
GO

-- many-to-many: a part has several connections, a connection type can sit on several parts
IF OBJECT_ID(N'dbo.PartConnection') IS NULL
CREATE TABLE dbo.PartConnection (
    PartId        INT NOT NULL REFERENCES dbo.Part(Id) ON DELETE CASCADE,
    ConnectionId  INT NOT NULL REFERENCES dbo.Connection(Id),
    CONSTRAINT PK_PartConnection PRIMARY KEY (PartId, ConnectionId)
);
GO

/* ---------- Section 2: pre/during welding checklist ---------- */

IF OBJECT_ID(N'dbo.WeldReport') IS NULL
CREATE TABLE dbo.WeldReport (
    Id                 INT IDENTITY PRIMARY KEY,
    ProjectId          INT NOT NULL REFERENCES dbo.Project(Id),
    StartDate          DATE NOT NULL,
    GapBefore          INT  NOT NULL,
    GapDuring          INT  NOT NULL,
    DailyCapacity      INT  NOT NULL,
    ConnectionPercent  DECIMAL(5,2) NOT NULL,
    HallSupervisor     NVARCHAR(200) NULL,
    Hall               NVARCHAR(100) NULL,
    SkeletonTime       NVARCHAR(100) NULL,
    BaseMetal          NVARCHAR(50)  NULL,
    WeldConsumable     NVARCHAR(50)  NULL
);
GO

IF OBJECT_ID(N'dbo.WeldRow') IS NULL
CREATE TABLE dbo.WeldRow (
    Id              INT IDENTITY PRIMARY KEY,
    ReportId        INT NOT NULL REFERENCES dbo.WeldReport(Id) ON DELETE CASCADE,
    PartId          INT NOT NULL REFERENCES dbo.Part(Id),
    InstanceNo      INT NOT NULL,
    ConnectionId    INT NOT NULL REFERENCES dbo.Connection(Id),
    ReportDate      DATE NOT NULL,
    DuringDate      DATE NOT NULL,
    BevelAngle      DECIMAL(9,2) NULL,
    JointPrepOk     BIT NOT NULL DEFAULT 0,
    PlateThickness  DECIMAL(9,1) NULL,
    WpsOk           BIT NOT NULL DEFAULT 0,
    WelderCtrlOk    BIT NOT NULL DEFAULT 0,
    FinalCtrlOk     BIT NOT NULL DEFAULT 0,
    WelderCode      NVARCHAR(50) NULL,
    AmbientTemp     DECIMAL(9,2) NULL,
    TravelSpeed     DECIMAL(9,2) NULL,
    Amperage        DECIMAL(9,2) NULL,
    Ready1 BIT NOT NULL DEFAULT 0, Ready2 BIT NOT NULL DEFAULT 0, Ready3 BIT NOT NULL DEFAULT 0,
    Ready4 BIT NOT NULL DEFAULT 0, Ready5 BIT NOT NULL DEFAULT 0, Ready6 BIT NOT NULL DEFAULT 0,
    Notes           NVARCHAR(500) NULL,
    CONSTRAINT UQ_WeldRow UNIQUE (ReportId, PartId, InstanceNo, ConnectionId)
);
GO

/* ---------- Sections 3-6: DT, VT, UT, PT share one header table ---------- */

IF OBJECT_ID(N'dbo.InspectionReport') IS NULL
CREATE TABLE dbo.InspectionReport (
    Id         INT IDENTITY PRIMARY KEY,
    ProjectId  INT NOT NULL REFERENCES dbo.Project(Id),
    Kind       VARCHAR(4) NOT NULL CHECK (Kind IN ('DT','VT','UT','PT')),
    Sketch     VARBINARY(MAX) NULL,
    CONSTRAINT UQ_InspectionReport UNIQUE (ProjectId, Kind)
);
GO

-- header fields of each form (dates, equipment, standard, ...) as name/value pairs
IF OBJECT_ID(N'dbo.ReportParam') IS NULL
CREATE TABLE dbo.ReportParam (
    ReportId  INT NOT NULL REFERENCES dbo.InspectionReport(Id) ON DELETE CASCADE,
    Name      NVARCHAR(50)  NOT NULL,
    Value     NVARCHAR(500) NOT NULL DEFAULT N'',
    CONSTRAINT PK_ReportParam PRIMARY KEY (ReportId, Name)
);
GO

IF OBJECT_ID(N'dbo.DtRow') IS NULL
CREATE TABLE dbo.DtRow (
    Id          INT IDENTITY PRIMARY KEY,
    ReportId    INT NOT NULL REFERENCES dbo.InspectionReport(Id) ON DELETE CASCADE,
    PartId      INT NOT NULL REFERENCES dbo.Part(Id),
    InstanceNo  INT NOT NULL,
    Qty         INT NULL,
    FitUpOk     BIT NOT NULL DEFAULT 0,
    WeldOk      BIT NOT NULL DEFAULT 0,
    DimOk       BIT NOT NULL DEFAULT 0,
    CpA BIT NOT NULL DEFAULT 0, CpB BIT NOT NULL DEFAULT 0, CpC BIT NOT NULL DEFAULT 0,
    CpD BIT NOT NULL DEFAULT 0, CpE BIT NOT NULL DEFAULT 0, CpF BIT NOT NULL DEFAULT 0,
    CpG BIT NOT NULL DEFAULT 0, CpH BIT NOT NULL DEFAULT 0, CpI BIT NOT NULL DEFAULT 0,
    CpJ BIT NOT NULL DEFAULT 0, CpK BIT NOT NULL DEFAULT 0, CpL BIT NOT NULL DEFAULT 0,
    CONSTRAINT UQ_DtRow UNIQUE (ReportId, PartId, InstanceNo)
);
GO

IF OBJECT_ID(N'dbo.VtRow') IS NULL
CREATE TABLE dbo.VtRow (
    Id              INT IDENTITY PRIMARY KEY,
    ReportId        INT NOT NULL REFERENCES dbo.InspectionReport(Id) ON DELETE CASCADE,
    PartId          INT NOT NULL REFERENCES dbo.Part(Id),
    InstanceNo      INT NOT NULL,
    ItemType        NVARCHAR(50) NOT NULL DEFAULT N'',
    Porosity        BIT NOT NULL DEFAULT 0,
    Undercut        BIT NOT NULL DEFAULT 0,
    Crack           BIT NOT NULL DEFAULT 0,
    EndCrater       BIT NOT NULL DEFAULT 0,
    Lof             BIT NOT NULL DEFAULT 0,
    Other           BIT NOT NULL DEFAULT 0,
    Result          VARCHAR(30) NOT NULL DEFAULT 'Accepted'
                    CHECK (Result IN ('Accepted','Repair','AcceptedAfterRepair')),
    DefectPosition  NVARCHAR(300) NULL,
    CONSTRAINT UQ_VtRow UNIQUE (ReportId, PartId, InstanceNo)
);
GO

IF OBJECT_ID(N'dbo.UtRow') IS NULL
CREATE TABLE dbo.UtRow (
    Id              INT IDENTITY PRIMARY KEY,
    ReportId        INT NOT NULL REFERENCES dbo.InspectionReport(Id) ON DELETE CASCADE,
    PartId          INT NOT NULL REFERENCES dbo.Part(Id),
    InstanceNo      INT NOT NULL,
    WeldId          NVARCHAR(20) NOT NULL,          -- W1, W2, ...
    WelderNo        NVARCHAR(50) NULL,
    ScanSurface     NVARCHAR(20) NULL,
    ThicknessMm     DECIMAL(9,2) NULL,
    TestedLengthMm  DECIMAL(9,2) NULL,
    ProbeAngle      NVARCHAR(20) NULL,
    IndX DECIMAL(9,2) NULL, IndY DECIMAL(9,2) NULL, IndD DECIMAL(9,2) NULL, IndL DECIMAL(9,2) NULL,
    Interpretation  NVARCHAR(200) NULL,
    Accepted        BIT NOT NULL DEFAULT 1,
    Remarks         NVARCHAR(300) NULL,
    CONSTRAINT UQ_UtRow UNIQUE (ReportId, PartId, InstanceNo, WeldId)
);
GO

IF OBJECT_ID(N'dbo.PtRow') IS NULL
CREATE TABLE dbo.PtRow (
    Id          INT IDENTITY PRIMARY KEY,
    ReportId    INT NOT NULL REFERENCES dbo.InspectionReport(Id) ON DELETE CASCADE,
    PartId      INT NOT NULL REFERENCES dbo.Part(Id),
    InstanceNo  INT NOT NULL,
    Position    NVARCHAR(100) NULL,
    LevelDir    NVARCHAR(50)  NULL,
    DiscType    NVARCHAR(20)  NULL,
    DiscSizeMm  DECIMAL(9,2)  NULL,
    Result      VARCHAR(5) NOT NULL DEFAULT 'ACC' CHECK (Result IN ('ACC','REP')),
    AfterRep    BIT NOT NULL DEFAULT 0,
    CONSTRAINT UQ_PtRow UNIQUE (ReportId, PartId, InstanceNo)
);
GO

/* ---------- Section 7: nonconformance (NCR) ---------- */

IF OBJECT_ID(N'dbo.Ncr') IS NULL
CREATE TABLE dbo.Ncr (
    Id               INT IDENTITY PRIMARY KEY,
    ProjectId        INT NOT NULL REFERENCES dbo.Project(Id),
    PartId           INT NOT NULL REFERENCES dbo.Part(Id),
    InstanceNo       INT NOT NULL,
    NcrDate          DATE NOT NULL,
    ProductType      NVARCHAR(100)  NOT NULL DEFAULT N'',
    Stage            NVARCHAR(100)  NOT NULL DEFAULT N'',
    HallSupervisor   NVARCHAR(200)  NOT NULL DEFAULT N'',
    Inspector        NVARCHAR(200)  NOT NULL DEFAULT N'',
    Preparer         NVARCHAR(200)  NOT NULL DEFAULT N'',
    FollowUpBy       NVARCHAR(200)  NOT NULL DEFAULT N'',
    Description      NVARCHAR(2000) NOT NULL,
    Severity         NVARCHAR(50)   NOT NULL DEFAULT N'',
    TaskDescription  NVARCHAR(2000) NOT NULL DEFAULT N'',
    SessionDuration  NVARCHAR(50)   NOT NULL DEFAULT N'',
    Deadline         DATE NULL,
    ExecResult       NVARCHAR(2000) NOT NULL DEFAULT N'',
    InspResult       NVARCHAR(2000) NOT NULL DEFAULT N'',
    QtyAccepted      INT NOT NULL DEFAULT 0,
    QtyRework        INT NOT NULL DEFAULT 0,
    QtyReturned      INT NOT NULL DEFAULT 0,
    QtyOtherUse      INT NOT NULL DEFAULT 0,
    QtyScrap         INT NOT NULL DEFAULT 0,
    ReviewDuration   NVARCHAR(50)   NOT NULL DEFAULT N'',
    FinalTask        NVARCHAR(2000) NOT NULL DEFAULT N''
);
GO

-- databases created before FollowUpBy existed
IF COL_LENGTH(N'dbo.Ncr', N'FollowUpBy') IS NULL
    ALTER TABLE dbo.Ncr ADD FollowUpBy NVARCHAR(200) NOT NULL DEFAULT N'';
GO

IF OBJECT_ID(N'dbo.NcrAction') IS NULL
CREATE TABLE dbo.NcrAction (
    Id         INT IDENTITY PRIMARY KEY,
    NcrId      INT NOT NULL REFERENCES dbo.Ncr(Id) ON DELETE CASCADE,
    Material   NVARCHAR(200) NULL,
    Tools      NVARCHAR(200) NULL,
    Executor   NVARCHAR(200) NULL,
    Execution  NVARCHAR(500) NULL
);
GO

IF OBJECT_ID(N'dbo.NcrCheck') IS NULL
CREATE TABLE dbo.NcrCheck (
    Id         INT IDENTITY PRIMARY KEY,
    NcrId      INT NOT NULL REFERENCES dbo.Ncr(Id) ON DELETE CASCADE,
    Spec       NVARCHAR(200) NULL,
    Tolerance  NVARCHAR(200) NULL,
    Method     NVARCHAR(200) NULL,
    Sampling   NVARCHAR(200) NULL,
    Notes      NVARCHAR(500) NULL
);
GO
