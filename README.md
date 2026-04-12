# HubCinema WebUser (Customer)

Customer-facing frontend for the HubCinema movie ticketing system, built with **ASP.NET Core MVC (.NET 8)**.  
This application focuses on end-user experience: browsing movies, selecting showtimes, choosing seats, adding combos, and completing online payments.

## Overview

HubCinema WebUser is the **customer-facing** part of the HubCinema ecosystem.  
All business data (movies, cinemas, showtimes, combos, orders, accounts, etc.) is retrieved through the backend API built by the team.

## Key Features

- User registration, login, and forgot password
- Homepage with featured movie content and highlights
- Movie detail page
- Complete ticket booking flow:
  - Get seat map by showtime
  - Select seats
  - Select combos
  - Checkout/payment
- Integrated with 2 payment gateways:
  - **PayPal**
  - **VNPay**
- Bilingual support: **Vietnamese / English**
- Member page and transaction history
- Display tickets after payment and allow users to review booked tickets

## Integration Architecture

- WebUser acts as the MVC frontend for end users.
- The backend API handles business logic and data services.
- WebUser calls the API to:
  - Fetch movies, cinemas, showtimes, and combos
  - Handle user information
  - Synchronize order/payment data
- After successful payment, the system returns results for ticket display and transaction history storage.

## Technology Stack

- **.NET 8 / ASP.NET Core MVC**
- **Razor Views**
- **Newtonsoft.Json**
- **QRCoder**
- ASP.NET Core Session & Middleware
- Localization (vi/en)

## Main Folder Structure

- `Controllers/`: request/response flow handling for pages and features
- `Models/`: data models and DTOs used in WebUser
- `Services/`: API integration, PayPal/VNPay payment services, transactions
- `Views/`: Razor UI views by module
- `wwwroot/`: static assets (CSS, JS, images)
- `middlewares/`: middleware used in the request pipeline

## Team & Contribution Scope

This project was developed collaboratively:

- Backend API and admin components were built by the backend/admin team.
- The customer WebUser side was implemented by team members across major modules, including:
  - User authentication
  - Homepage and movie details
  - Booking and payment flow
  - Bilingual support
  - Member page and transaction history

The goal is to ensure a seamless booking experience from movie selection to ticket receipt after payment.

## Environment Requirements

- .NET SDK 8.0+

## Run Locally

```bash
dotnet restore
dotnet build
dotnet run --project MovieTicketWebsite.csproj
```

After running, open the URL shown in the terminal (usually `https://localhost:<port>`).

## Configuration

- Update configuration values in:
  - `appsettings.json`
  - `appsettings.Development.json`
- Set backend API endpoints and payment configuration (PayPal/VNPay) based on your target environment.

## Notes

- This repository is for **WebUser (Customer)**.
- Backend API and Admin are deployed in other repositories/services within the HubCinema system.
