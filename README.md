# Quality Control Management System

A Windows desktop application for managing project information and recording quality-control inspections for steel fabrication and welding workflows. The application uses a Persian-language interface and stores project, part, connection, welding, and inspection records in Microsoft SQL Server.

> **Project status:** This repository contains an actively developed application. Review and test the workflows against your organization's inspection procedures before using it for operational or compliance purposes.

## Features

### Project Setup
- Create projects with Persian and English names and organization details.
- Record client, contractor, QC manager, QA manager, coordinator, contract date, project phases, and reported phase.
- Attach a project logo.
- Resume the most recently created project or create a new one.

### Parts and Connections
- Maintain project assembly and parts lists.
- Add parts individually or import them from Excel.
- Record part name, part code, drawing code, quantity, length, and weight.
- Maintain connection records, including connection code, plate thickness, quantity, and weight.
- Associate parts with their connections.
- Validate duplicate part names and connection codes within a project.

### Welding and Inspection Records

The application provides the following inspection sections:

1. **Pre-welding and during-welding checklist** – Generate checklists from project parts and connections, record preparation and welding checks, and save reports.
2. **Dimensional Testing (DT)** – Record dimensional inspection checks, including fit-up, welding, dimensional checks, and inspection points.
3. **Visual Testing (VT)** – Record visual weld inspection findings and results.
4. **Ultrasonic Testing (UT)** – Record equipment and inspection parameters, weld measurements, interpretations, and acceptance results.
5. **Liquid Penetrant Testing (PT)** – Record procedure parameters and inspection results.
6. **Nonconformance Reports (NCR)** – Document nonconforming items, severity, descriptions, corrective actions, follow-up, and review results.

### Reports and Printing
- Print-preview functionality for inspection forms.
- Shared report printer supporting report headers, project logos, information fields, tables, sketches where provided, and signature areas.

## Technology Stack

- **Language:** C#
- **Desktop UI:** Windows Forms
- **Runtime:** .NET Framework 4.8
- **Database:** Microsoft SQL Server
- **Data access:** Dapper and Microsoft.Data.SqlClient
- **Excel support:** ClosedXML

## Repository Structure

| Path | Purpose |
|---|---|
| `Data/` | Database connection and repository classes |
| `Database/schema.sql` | SQL Server database creation script |
| `Forms/` | Project, parts, connections, welding, inspection, and NCR forms |
| `Helpers/` | Validation, calendar, messaging, session, and report-printing utilities |
| `Models/` | Project, part, connection, and inspection data models |
| `Templates/` | Excel templates |
| `Properties/` | Application resources and settings |
| `QC1.slnx` | Visual Studio solution |
| `QC1.csproj` | Windows Forms project file |

## Requirements

- Windows
- Visual Studio with the **.NET desktop development** workload
- .NET Framework 4.8 Developer Pack
- Microsoft SQL Server
- NuGet package restore enabled in Visual Studio

## Setup

### 1. Clone the Repository

```bash
git clone https://github.com/Raya1313/quality_control.git
cd quality_control
```

### 2. Create the Database

1. Open SQL Server Management Studio (SSMS).
2. Connect to your SQL Server instance.
3. Open `Database/schema.sql`.
4. Execute the script.

The script creates the `QC1` database and its tables for projects, parts, connections, welding reports, inspection reports, and NCR records.

### 3. Configure the Database Connection

The current connection string is defined in `Data/Db.cs`:

```csharp
const string Cs =
    @"Server=localhost;Database=QC1;Trusted_Connection=True;TrustServerCertificate=True";
```

This configuration expects:
- SQL Server to be reachable at `localhost`.
- A database named `QC1`.
- Windows Authentication to be available to the Windows account running the application.

If your SQL Server uses a named instance or a different host, update the `Server` value. If you use SQL Server Authentication, change the connection string accordingly. Do not commit credentials to source control.

### 4. Open and Build the Application

1. Open `QC1.slnx` in a compatible version of Visual Studio.
2. Allow NuGet packages to restore.
3. Build the solution.
4. Run the application.

On first launch, create a project. On later launches, the application offers to reopen the most recently created project.

## Excel Import

The parts import reads the first worksheet and treats the first row as a header. The expected columns are:

| Column | Value |
|---|---|
| A | Part name |
| B | Quantity |
| C | Length |
| D | Weight |

Use an `.xlsx` workbook. Invalid rows and duplicate part names are reported during import. Imported parts remain in the form until the user confirms the list.

Excel templates are available under `Templates/`.

## Data and Project Scope

Records are associated with the currently selected project. The database schema uses foreign keys to relate inspection records to parts, connections, and reports.

Some report types use shared report-header and parameter tables with separate detail-row tables.

The application currently uses a local SQL Server connection configured in source code. For multi-workstation deployment, move connection settings into an appropriate configuration mechanism and review authentication, permissions, backup, and data-protection requirements.

## Notes

- The application interface and many workflow labels are in Persian; some inspection terminology and report fields are in English.
- The SQL schema script creates missing tables but is not a general-purpose migration system for upgrading an existing database schema.
- Excel import layouts should match the columns expected by the corresponding form.
- This README describes functionality visible in the source code. It does not imply certification against any particular welding or inspection standard.

## License

No license has been specified in this repository. Unless a license is added, standard copyright restrictions apply.
