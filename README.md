# Travel and Accommodation Booking Platform

## 📌 Overview

This project is a **Travel and Accommodation Booking Platform** that allows users to book accommodations and manage their reservations seamlessly.
The system integrates **PayPal for payment processing** and follows **Clean Architecture** principles.

## 🔥 Features

- **User Authentication & Authorization** (JWT-based)
- **Booking System**
- **Payment Integration** with PayPal
- **Invoice Generation** using QuestPDF
- **Email Notifications** for booking confirmations


## 🏗️ Tech Stack

- **Backend:** ASP.NET Core
- **Database:** SQL Server
- **Payment Gateway:** PayPal
- **Email Service:** SMTP
- **Infrastructure:** Clean Architecture

## 📂 Project Structure

```
Travel_and_Accommodation_Booking_Platform/
│-- Application/       # Business logic & use cases
│-- Domain/            # Entities & core domain models
│-- Infrastructure/    # External services (DB, Email, Payment, etc.)
│-- Presentation/      # API Controllers & UI Layer
│-- TABPTesting/       # Unit Tests
│-- README.md          # Project Documentation
```

## 🚀 Setup & Run Locally

### **1️⃣ Clone the Repository**

```sh
git clone https://github.com/your-repo-url.git](https://github.com/a7iiid/Travel_and_Accommodation_Booking_Platform.git
cd Travel_and_Accommodation_Booking_Platform
```

### **2️⃣ Configure Environment Variables**

Create a `.env` file in the root directory and add:

```env

#emailconfiguration
SmtpHost=smtp.example.com
SmtpPort=587
EnableSSL=true
FromEmail=your-email@example.com
FromPassword=your-email-password
UserName=TABP

#paypallconfiguration
ClientId=your-paypal-client-id
ClientSecret=your-paypal-secret
Mode=sandbox
ReturnUrl=your url
CancelUrl=your url

#jwtconfiguration
Issuer=your url
Audience=your url
SecretKey=your secret Key
```


### **3️⃣ Access the Application**

- **API Endpoint:** `http://localhost:5000/api`
- **Swagger UI:** `http://localhost:5000/swagger`

## 📧 Invoice Generation & Email Sending

- Uses **QuestPDF** for generating PDF invoices.
- Sends payment confirmation emails with invoice attachment.
- Ensure to configure SMTP settings in `.env` file.



---


