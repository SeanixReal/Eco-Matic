# Eco-Matic Project Paper

Prepared for: CPE 261 Midterm Review  \
Author: Seanix Real  \
Date: November 16, 2025

---

## 1. Project Overview
**Purpose.** Eco-Matic is a console-based vending machine simulator that fuses vending operations with a “trash-to-credit” mechanic. It functions both as a proof-of-concept for sustainable kiosks in Cebu and as an academic showcase of C# skills, including OOP, file I/O, and user experience design.

**Features.** Core capabilities include customer purchasing, a recycling credit flow, admin inventory controls, and CSV-backed persistence. The console interface uses Spectre.Console for styled tables, balance trackers, and receipts.

**Target Audience.** The application targets three groups: (1) instructors who evaluate software engineering rigor, (2) classmates who may reuse the concepts for their own projects, and (3) sustainability advocates who want to see gamified recycling in action.

**Technology Stack.** Built with C# on .NET 9, the solution relies on Spectre.Console for ANSI styling, System.IO for CSV operations, and Spectre-inspired helper classes to keep the single-file codebase organized.

## 2. Requirements
**Software Requirements.** .NET 9 SDK, Spectre.Console package, and an IDE such as VS Code or Visual Studio 2022.

**Installation Steps.** Clone the repository, then run:
```bash
dotnet restore
dotnet build eco-matic/eco-matic.csproj
```
Next launch the simulator from the repo root:
```bash
dotnet run --project eco-matic/eco-matic.csproj
```
This ensures the `data` folder is copied into the build output.

**System Requirements.** Any Windows PC capable of running console apps is sufficient. The memory footprint stays minimal because arrays—rather than heavy collections—store catalog data.

## 3. File Handling Overview
**File Types and Purpose.** Two CSV files power the platform: `inventory.csv` describes each slot (type, name, price, stock, calories/volume), while `eventLog.csv` captures every purchase, recycle event, and admin action with timestamps.

**File Operations.** On startup the program loads both CSV files into arrays. Inventory modifications rewrite the CSV to keep it the single source of truth. Transaction events append instantly to the log. Helper routines also mirror the `data` directory into the `bin` output for debug runs.

**Error Handling.** The loader validates headers, enforces numeric parsing, and regenerates default files if corruption is detected. Input validation ensures prices, IDs, and stock counts remain within safe ranges.

## 4. Code Structure
**Main Program Structure.** `EcoMatic` orchestrates session flow, balances, menu routing, inventory updates, and logging. It leverages specialized trackers for transactions and recycling.

**Code Walkthrough.**
- `VendingItem` (abstract) defines shared fields and CSV serialization.
- `SnackItem`, `DrinkItem`, and `MiscItem` override behavior for flavor text and nutritional info.
- Helper classes (`Write`, `TransactionTracker`, `RecycleTracker`, `SalesReport`) encapsulate console formatting, receipt aggregation, and reporting.

**Modularity and Reusability.** Although the assignment requires a single `Program.cs`, responsibilities are split into regions and helper classes. Constants declare slot counts, maximum stock, and recycling payouts to avoid magic numbers.

## 5. User Interface
**Design and Usability.** Spectre.Console tables create a dashboard-like experience. Color-coded stock dots communicate scarcity at a glance. Receipts summarize purchases and recycled materials at the end of each session.

**Input/Output.** Customers can insert bills, browse/examine items, recycle materials, purchase products, and get change. Admins authenticate with a passcode (`admin123`) to restock, add/remove items, view logs, clear logs, and run sales reports.

**Error Messages.** The `Write` helper centralizes validation messaging. It highlights invalid bill denominations, incorrect item IDs, and out-of-range numeric values without crashing the app.

## 6. Challenges and Solutions
**Development Challenges.**
- CSV corruption risk: Frequent editing sometimes left files half-written.
- Array limitations: Without lists or dictionaries, tracking sessions could get messy.
- Single-file constraint: Maintaining readability inside one `Program.cs` required discipline.

**Problem-Solving.**
- Added validation and auto-rebuild routines for CSV files.
- Reset transaction/recycle trackers at the start of each session to keep indices clean.
- Grouped logic into helper classes and regions, preserving modular thinking inside a single file.

## 7. Testing
**Test Cases.** Normal purchases, insufficient balance, sold-out items, recycling limits, admin restock/add/remove, log viewing/clearing, and sales report generation.

**Results.** Each test scenario produced the expected console flow and updated the CSV/log files appropriately. Receipts aggregate repeated purchases correctly.

**Limitations.** Testing remains manual; planned improvements include scripted regression tests and automated CSV diff checks.

## 8. Future Enhancements
**Planned Features.** Modularize code into multiple files, migrate arrays to collection types, add configurable recycling catalogs, and introduce analytics dashboards.

**Performance Improvements.** Explore caching for price lookups, add inventory valuation and restock forecasting, and experiment with GUI or web clients for kiosk deployment.

## 9. Conclusion
**Reflection.** Eco-Matic strengthened my grasp of object-oriented design, file persistence, and user-centric console interactions. It also demonstrated how sustainability goals can inspire technical features.

**Takeaways.** I gained experience balancing academic constraints (single file, arrays) with production-style safeguards, and I learned how to tell a cohesive sustainability story through software.

## Appendix
- **Source Code:** `eco-matic/Program.cs`, `eco-matic/data/*.csv`, branch `feature/eco-matic-refactoring`.
- **References:** Spectre.Console docs for layout/markup, official .NET documentation for file handling and console APIs, and UN SDG 12 resources for contextual framing.
