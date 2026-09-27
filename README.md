# Order Management System REST API

## Overview

Order Management System is a backend REST API application built with **ASP.NET Core Web API**. The project represents a simple order processing platform that allows managing customers, products, orders, payments, shipments, and revenue calculations.

The application was designed using a **three-layer architecture** with clear separation of responsibilities between API, business logic, and data access layers.

The system supports installment payments, order lifecycle management, soft deletion, authentication, validation, logging, and automated background processes.

---

## Technologies

* **C#**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server**
* **JWT Bearer Authentication**
* **FluentValidation**
* **xUnit**
* **ILogger**
* **Entity Framework Core Code First**
* **Background Services**

---

# Architecture

The application follows a three-layer architecture:

### API Layer

Responsible for:

* HTTP request handling
* Controllers
* Authentication
* API responses

### Business Layer

Responsible for:

* Business rules
* Order calculations
* Payment validation
* Application workflows

### Data Access Layer

Responsible for:

* Database communication
* Entity Framework Core configuration
* Data persistence

---

# Key Features

## Order Management

The system provides complete order management functionality:

* Creating orders
* Retrieving orders
* Updating orders
* Soft deleting orders
* Filtering active orders

During order creation the system automatically:

* Calculates total order price
* Calculates total order weight
* Selects appropriate shipment method based on weight

Order updates automatically recalculate affected data and maintain consistency between products and orders.

---

## Product Management

Features:

* CRUD operations
* Searching products by name
* Filtering by category
* Product validation

Business rules:

* Product name, category, and weight cannot be modified after creation
* Price and description can be updated
* Changing product price automatically updates related order totals

---

## Customer Management

The application supports two customer types:

* Private customers
* Companies

Customer inheritance is implemented using:

**TPH (Table Per Hierarchy)** strategy.

Features:

* Creating customers
* Updating customer information
* Searching customers
* Retrieving customer details

Business rules:

* PESEL and NIP values cannot be modified
* Customer deletion is not supported

---

## Payment System

The application supports order payments with business validation.

Payment processing checks:

* Whether the order is already fully paid
* Whether payment amount exceeds remaining balance
* Whether payment is completed within the allowed period

Orders must be fully paid within **7 days from creation**.

A scheduled background process runs periodically and automatically cancels expired unpaid orders.

---

## Order Status Management

Order status is represented using an enum:

### Active

Order created and waiting for payment.

### Paid

Order fully paid and ready for processing.

### Cancelled

Order removed or payment deadline exceeded.

---

## Revenue Reporting

The system provides:

* Current revenue calculation based on fully paid orders
* Expected revenue including active unpaid orders

---

# Security

Authentication is implemented using:

**JWT Bearer Authentication**

Protected endpoints require a valid authorization token.

---

# Validation

Input validation is handled using:

**FluentValidation**

Implemented validations include:

* Customer data validation
* Product validation
* Shipment validation
* Required fields checking
* String length validation
* Format validation

---

# Error Handling

Global exception handling is implemented using custom middleware.

Instead of repeating `try-catch` blocks inside controllers, exceptions are handled centrally.

The middleware:

* Captures application exceptions
* Logs errors
* Returns proper HTTP status codes
* Provides meaningful error responses

---

# Logging

Application logging is implemented using:

**ILogger**

Logs include:

* Application errors
* Important application events
* Debugging information

---

# Database

The application uses:

* SQL Server
* Entity Framework Core
* Code First migrations

Most database operations are handled through Entity Framework Core.

Stored procedures are used for selected database-level operations when needed.

---

# Testing

The project contains automated tests covering:

* Business logic
* Validation rules
* Payment scenarios
* Order processing
* Exception handling

---

# Additional Implementation Details

* Soft delete mechanism using `isDeleted` flag
* Background service for payment deadline verification
* Enum-based order status management
* TPH inheritance mapping
* RESTful API design
* Proper HTTP response codes
* Separation of concerns following clean architecture principles

---

# Project Goals

The main goal of this project was to build a realistic backend system demonstrating:

* REST API development
* Entity Framework Core usage
* Database modeling
* Business logic implementation
* Authentication and authorization
* Automated testing
* Production-oriented backend practices

---
<img width="2090" height="951" alt="diagram" src="https://github.com/user-attachments/assets/b842d213-a487-4893-b646-22ae4595a985" />
