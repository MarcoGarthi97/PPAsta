# PPAsta

A desktop application developed in **C#** for the *Associazione Ludico Culturale Peter Pan* to simplify and automate the management of private auctions or events.

PPAsta is a comprehensive desktop management application built with WinUI 3 for organizing and tracking items, sales, and payments for private auctions or events. It provides a structured way to manage games for sale, track buyers, handle payments, and calculate and manage payouts to sellers.

## Features

- **Game Management**: Track a catalog of games, including owner, original price, and sale status.
- **Buyer and Seller Tracking**: Maintain lists of buyers and sellers (game owners), with support for adding, editing, and deleting records.
- **Bulk Data Import**:
    - Load initial game lists from a public Google Sheets URL.
    - Import buyer information from a local CSV file.
- **Payment Processing**:
    - Record sales, including purchase price and buyer details.
    - Automatically calculate shares for the seller and the organizer ("PP").
    - Track the payment status for each buyer and payout status for each seller.
- **Detailed Views**:
    - **Payments Page**: Aggregates all purchases by a single buyer, showing total costs and payment status.
    - **Sellers Page**: Aggregates all games sold by a single seller, showing total earnings and payout status.
- **Data Export**: Export a comprehensive report of all game sales and payment details to a CSV file.
- **Dynamic UI**: Features data grids with robust filtering by year, text search, sorting, and pagination to handle large datasets efficiently.

## Architecture

The application is built using a clean, layered architecture to ensure separation of concerns and maintainability.

- **`PPAsta` (Presentation Layer)**: The main WinUI 3 project that contains the user interface. It is developed using the MVVM (Model-View-ViewModel) pattern and includes all Pages, User Controls, ViewModels, and data converters.

- **`PPAsta.Service` (Business Logic Layer)**: This layer contains the core application logic. It orchestrates operations by interacting with the repository layer and provides data to the ViewModels. It also includes AutoMapper profiles for object mapping.

- **`PPAsta.Repository` (Data Access Layer)**: Responsible for all database interactions. It uses Dapper for high-performance data access to a local SQLite database and implements the repository pattern.

- **`PPAsta.Migration` (Database Migration Layer)**: Manages the database schema evolution. It ensures the database is created and updated correctly as the application version changes.

- **`PPAsta.Abstraction` (Shared Kernel)**: A common project that holds shared interfaces, enums, and base models used across all layers of the application.

## Technologies Used

- **Framework**: .NET 8 / WinUI 3
- **Language**: C#
- **Database**: SQLite with Microsoft.Data.Sqlite
- **ORM**: Dapper
- **Architecture**: Model-View-ViewModel (MVVM)
- **MVVM Toolkit**: CommunityToolkit.Mvvm
- **CSV Parsing**: CsvHelper
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection

## Getting Started

To get a local copy up and running, follow these steps.

### Prerequisites

- Visual Studio 2022 with the **.NET desktop development** workload installed.
- Windows 10 version 1809 (build 17763) or later.

### Installation

1.  **Clone the repository:**
    ```sh
    git clone https://github.com/MarcoGarthi97/PPAsta.git
    ```

2.  **Open the solution:**
    Open the `PPAsta.sln` file in Visual Studio.

3.  **Restore NuGet Packages:**
    Visual Studio should restore the packages automatically. If not, right-click the solution in Solution Explorer and select "Restore NuGet Packages".

4.  **Set Startup Project:**
    Right-click the `PPAsta` project in Solution Explorer and select "Set as Startup Project".

5.  **Run the application:**
    Press `F5` or click the "Start" button to build and run the application. On the first launch, the application will automatically create the SQLite database file (`app.db`) in the `%LOCALAPPDATA%\PPAsta\` directory.

## ❤️ Special Thanks

This project was created with love and dedication for the [**Associazione Ludico Culturale Peter Pan**](https://linktr.ee/peterpancdc), a passionate group of volunteers committed to spreading the joy of tabletop role-playing games.
May PPAsta help make your auctions smoother, your sales perfectly tracked, and your events unforgettable.
