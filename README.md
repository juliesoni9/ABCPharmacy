# ABC Pharmacy - Medicine Management System

A Single Page Application for ABC Pharmacy to manage medicine inventory and sale records.

## Architecture

| Layer | Technology |
|-------|------------|
| Frontend | Angular 20 (SPA) |
| Backend | ASP.NET Core Web API |
| Storage | JSON files on server (`Data/medicines.json`, `Data/sales.json`) |

## Features

- **Medicine list** — Grid displaying Full Name, Brand, Expiry Date, Quantity, and Price (Notes are stored but not shown in the grid)
- **Color indicators**
  - Red: expiry within 30 days
  - Yellow: quantity below 10 (when not expiring soon)
- **Add medicine** — Form to capture all medicine attributes
- **Search** — Filter medicines by name
- **Sale records** — Record sales from the grid; stock is decremented automatically; sale history is displayed below

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+)
- Angular CLI (`npm install -g @angular/cli`)

---

## Steps to Launch

### Step 1: Create the project using the `dotnet` command

The ABC Pharmacy backend was created with the Web API template (not a console app, since this application exposes REST endpoints):

```bash
cd d:\Projects\ABCPharmacy
dotnet new webapi -n PharmacyApi -o PharmacyApi --use-controllers
```

> **Note:** For a blank console app, the syntax is:
> `dotnet new console -n MyConsoleApp -o MyConsoleApp`
>
> (`--MyConsoleApp` is not valid; use `-n` for name or `-o` for output folder.)

This project is already set up. You only need Step 1 if you are creating it from scratch.

### Step 2: Build and run the Web API

From the `ABCPharmacy` solution folder:

```bash
cd d:\Projects\ABCPharmacy
dotnet run --project PharmacyApi
```

Or from inside the API project folder:

```bash
cd d:\Projects\ABCPharmacy\PharmacyApi
dotnet run
```

The API starts at **http://localhost:5151**.

Verify it is running by opening: **http://localhost:5151/api/medicines**

### Step 3: Open Preview and enter the application

**Web API (backend)**

1. Run the API (Step 2).
2. In Cursor/VS Code, use **Simple Browser** or **Open Preview**.
3. Enter: `http://localhost:5151/api/medicines`
4. You should see JSON data for the medicine list.

**Angular UI (frontend)**

The user interface is a separate Angular app. Start it in a second terminal:

```bash
cd d:\Projects\ABCPharmacy\pharmacy-ui
npm start
```

Then open Preview (or your browser) and enter: **http://localhost:4200**

> The Angular app calls the API at `http://localhost:5151`. **Both must be running** for the full application to work.

---

## Quick Reference

| Component | Command | URL |
|-----------|---------|-----|
| Web API | `dotnet run --project PharmacyApi` | http://localhost:5151 |
| Angular UI | `npm start` (in `pharmacy-ui`) | http://localhost:4200 |
| API — medicines | — | http://localhost:5151/api/medicines |
| API — sales | — | http://localhost:5151/api/sales |

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/medicines?search={name}` | List medicines (optional name search) |
| GET | `/api/medicines/{id}` | Get medicine by ID |
| POST | `/api/medicines` | Add a new medicine |
| GET | `/api/sales` | List all sale records |
| POST | `/api/sales` | Record a sale |

## Sample Data

The API ships with three sample medicines in `PharmacyApi/Data/medicines.json` for demonstration.

---

## Deploy to Render (free public URL)

The app is configured for **single-URL hosting**: the API serves the Angular UI and REST endpoints together.

1. Sign up at [render.com](https://render.com) and connect your GitHub account.
2. Click **New +** → **Blueprint**.
3. Select the `juliesoni9/ABCPharmacy` repository.
4. Render reads `render.yaml` and creates the **abc-pharmacy** web service.
5. Click **Apply** and wait for the Docker build to finish (~5–10 minutes).
6. Open your live URL, e.g. `https://abc-pharmacy.onrender.com`.

> **Note:** On Render's free tier the app sleeps after inactivity; the first visit may take ~30 seconds to wake up. JSON data may reset when the service redeploys.
