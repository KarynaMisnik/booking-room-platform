# 1. Identify the Stakeholders and User Needs

The main stakeholders of the hotel room reservation application are hotel management, hotel staff, customers, guests, the system administrator, and the development/DevOps team. Each stakeholder has different expectations and responsibilities.

### Stakeholders and Their Needs

| Stakeholder                     | Interest / Need                                                            |
| ------------------------------- | -------------------------------------------------------------------------- |
| **Hotel management / owner**    | Efficient hotel operations, room occupancy, revenue, and a reliable system |
| **Receptionists / hotel staff** | Manage reservations, room availability, and customer bookings              |
| **Customers**                   | Find rooms, make reservations, pay, and manage their bookings              |
| **Guests / visitors**           | Browse rooms and get information before deciding to book                   |
| **System administrator**        | Maintain the system, users, security, and technical operation              |
| **Development / DevOps team**   | Develop, test, deploy, monitor, and maintain the application               |

The application has several user roles. A **User** can browse rooms and view information about
availability and prices. A **Customer** has additional functionality, such as making and managing
reservations and paying for a room. **Receptionists** use the management system to manage
reservations and room availability. **Managers** have broader management permissions, including
managing rooms, prices, and hotel information. The **System Administrator** manages technical and
access-related aspects of the application.<br>

### Users / roles in the application

| Application Role | Main Permissions                                                                |
| ---------------- | ------------------------------------------------------------------------------- |
| **User**         | Browse rooms, view information, prices, and availability                        |
| **Customer**     | Browse rooms, make and manage reservations, and pay                             |
| **Receptionist** | Access the management system, manage reservations, and manage room availability |
| **Manager**      | Manage rooms, prices, availability, hotel information, and possibly reports     |
| **System Admin** | Manage system users, permissions, and technical settings                        |

# 2. Define the Overall Application Requirements

**Functional requirements**<br>

| ID   | Requirement                                                                                       |
| ---- | ------------------------------------------------------------------------------------------------- |
| FR1  | The application shall allow users to browse available hotel rooms.                                |
| FR2  | The application shall display room information, including price, description, and availability.   |
| FR3  | The application shall allow customers to search for rooms based on selected dates.                |
| FR4  | The application shall allow customers to create a reservation.                                    |
| FR5  | The application shall allow customers to view and cancel their own reservations.                  |
| FR6  | The application shall allow customers to make payments for reservations.                          |
| FR7  | The application shall allow receptionists to create and manage customer reservations.             |
| FR8  | The application shall allow authorized staff to manage room availability.                         |
| FR9  | The application shall allow managers to add, modify, and remove room information and prices.      |
| FR10 | The application shall prevent double booking of the same room for overlapping dates.              |
| FR11 | The application shall send a reservation confirmation to the customer after a successful booking. |

**Performance requirements**<br>

| ID  | Requirement                                                                                                      |
| --- | ---------------------------------------------------------------------------------------------------------------- |
| PR1 | Normal user requests should receive a response within 2 seconds under normal load.                               |
| PR2 | Room availability searches should return results within 2 seconds under normal load.                             |
| PR3 | The system should support the expected number of simultaneous users without significant performance degradation. |

**Usability requirements**<br>

| ID  | Requirement                                                       |
| --- | ----------------------------------------------------------------- |
| UR1 | Users shall be able to browse rooms without creating an account.  |
| UR2 | The reservation process should be simple and understandable.      |
| UR3 | The interface should provide clear validation and error messages. |
| UR4 | The application should be usable on desktop and mobile devices.   |

**Reliability and availability**<br>

| ID  | Requirement                                                                            |
| --- | -------------------------------------------------------------------------------------- |
| RR1 | The application shall prevent data loss during normal operation.                       |
| RR2 | Reservation data shall remain consistent if an operation fails.                        |
| RR3 | The system shall automatically recover from temporary service failures where possible. |
| RR4 | The application should be available continuously except during planned maintenance.    |
| RR5 | Database backups shall be created regularly.                                           |

**Security**<br>

| ID  | Requirement                                                              |
| --- | ------------------------------------------------------------------------ |
| SR1 | Users shall only access functionality permitted by their role.           |
| SR2 | Customers shall only be able to view and manage their own reservations.  |
| SR3 | Sensitive customer and payment information shall be protected.           |
| SR4 | Passwords shall not be stored as plain text.                             |
| SR5 | Communication between the client and server shall use HTTPS.             |
| SR6 | Administrative functions shall require authentication and authorization. |

**Maintainability**<br>

| ID  | Requirement                                                                                              |
| --- | -------------------------------------------------------------------------------------------------------- |
| MR1 | The application shall use a modular architecture so individual components can be modified independently. |
| MR2 | The application shall have automated tests for important functionality.                                  |
| MR3 | The application shall use version control.                                                               |
| MR4 | The application shall use automated CI/CD pipelines for building and testing.                            |
| MR5 | Application configuration shall be separated from source code.                                           |
| MR6 | The system shall provide logging to help diagnose errors and failures.                                   |

**Compatibility and integration**<br>

| ID  | Requirement                                                                                  |
| --- | -------------------------------------------------------------------------------------------- |
| CR1 | The application shall provide a REST API for communication between the frontend and backend. |
| CR2 | The application shall use a relational database for storing reservation and room data.       |
| CR3 | The application shall integrate with a payment service for customer payments.                |
| CR4 | The application shall be accessible through modern web browsers.                             |
| CR5 | The application shall support deployment using containers.                                   |

**Constraints**<br>

| ID  | Constraint                                                                                                                |
| --- | ------------------------------------------------------------------------------------------------------------------------- |
| C1  | The application shall comply with applicable data protection legislation, such as GDPR.                                   |
| C2  | The application shall use the technologies selected for the project, such as React, ASP.NET Core, PostgreSQL, and Docker. |
| C3  | The project shall use Git for version control.                                                                            |
| C4  | The application shall be deployable using the project's CI/CD pipeline.                                                   |
| C5  | The project shall be developed within the available course time and resources.                                            |

# 3. Partition the application into logical subsystems

### 1. User & Authentication Subsystem

**Purpose:**
Manage users, accounts, authentication, and permissions.

**Responsibilities**<br>

- User registration/login
- Authentication
- Role management
- Authorization

**Inputs**<br>

- Login/registration data
- User role information

**Outputs**<br>

- Authentication result
- User information
- Access permissions

**Functional Requirements**<br>

- The system shall allow users to log in.
- The system shall assign appropriate permissions based on the user's role.
- Customers shall only access their own reservations.

**Non-functional Requirements**<br>

- Passwords must be securely stored.
- Authentication must use secure communication.
- Unauthorized users must not access protected functionality.

### 2. Room & Availability Subsystem

**Purpose:** Manage hotel rooms and their availability.<br>
**Responsibilities:**<br>

- Store room information
- Display room details
- Manage room availability
- Manage prices

**Inputs:**<br>

- Room information
- Selected dates
- Availability changes from staff

**Outputs:**<br>

- Available rooms

**Room details**<br>

- Prices
- Availability information
- Functional requirements:
- Users shall be able to browse rooms.
- Customers shall be able to search availability by dates.
- Authorized staff shall be able to add, modify, and remove rooms.

**Non-functional requirements:**<br>

- Availability searches should respond within 2 seconds under normal load.
- Room data must remain consistent.

### 3. Reservation Subsystem

**Purpose:**
Create and manage hotel reservations.

**Responsibilities**<br>

- Create reservations
- View reservations
- Modify/cancel reservations
- Prevent double bookings

**Inputs**<br>

- Customer information
- Room
- Check-in/check-out dates
- Reservation changes

**Outputs**<br>

- Reservation confirmation
- Reservation details
- Updated room availability

**Functional Requirements**<br>

- Customers shall be able to make reservations.
- Customers shall be able to view and cancel their reservations.
- Receptionists shall be able to manage customer reservations.
- The system shall prevent overlapping reservations for the same room.

**Non-functional Requirements**<br>

- Reservation operations must be reliable.
- Failed transactions must not leave inconsistent booking data.

### 4. Payment Subsystem

**Purpose:**
Handle payments for reservations.

**Responsibilities**<br>

- Process payments
- Associate payments with reservations
- Store payment status

**Inputs**<br>

- Reservation information
- Payment information

**Outputs**<br>

- Payment confirmation
- Payment status
- Failed-payment notification

**Functional Requirements**<br>

- Customers shall be able to pay for their reservations.
- The system shall record payment status.
- The system shall associate a successful payment with the corresponding reservation.

**Non-functional Requirements**<br>

- Payment information must be protected.
- The system should use a secure external payment provider rather than storing card details directly.

### 5. Administration & Management Subsystem

**Purpose:**
Provide hotel staff and managers with management functionality.

**Responsibilities**<br>

- Manage rooms
- Manage prices
- Manage availability
- Manage reservations
- View management information

**Inputs**<br>

- Room changes
- Price changes
- Availability changes
- Reservation changes

**Outputs**<br>

- Updated room information
- Updated availability
- Updated reservation information
- Reports/statistics if implemented

**Functional Requirements**<br>

- Receptionists shall be able to manage reservations.
- Managers shall be able to manage rooms and prices.
- Managers shall be able to manage room availability.

**Non-functional Requirements**<br>

- Only authorized staff shall access the management system.
- Administrative actions should be logged.

# 4. Quality Function Deployment (QFD)

### Step 1 — Customer/User needs (WHATs)

| ID  | Customer/User need                            | Importance |
| --- | --------------------------------------------- | ---------: |
| N1  | Easily find available rooms                   |          5 |
| N2  | Easily understand room information and prices |          4 |
| N3  | Make a reservation easily                     |          5 |
| N4  | Manage or cancel my reservation               |          4 |
| N5  | Make a secure payment                         |          5 |
| N6  | Avoid double bookings                         |          5 |
| N7  | Staff can efficiently manage reservations     |          5 |
| N8  | Staff can manage rooms and availability       |          4 |
| N9  | System is reliable and available              |          5 |
| N10 | Customer data is secure                       |          5 |

### Step 2 — Technical requirements (HOWs)

| ID  | Technical requirement                       |
| --- | ------------------------------------------- |
| T1  | Room availability search                    |
| T2  | Room information and pricing database       |
| T3  | Reservation management API                  |
| T4  | Customer reservation management             |
| T5  | Secure payment integration                  |
| T6  | Booking validation and database constraints |
| T7  | Staff management interface                  |
| T8  | Room and availability management            |
| T9  | Automated backups and recovery              |
| T10 | Authentication, authorization and HTTPS     |
| T11 | Automated testing and CI/CD                 |
| T12 | Logging and monitoring                      |

### Relationship between WHAT and HOW

| User need ↓ / Technical requirement → |  T1 |  T2 |  T3 |  T5 |  T6 |  T7 |  T8 |  T9 | T10 |
| ------------------------------------- | --: | --: | --: | --: | --: | --: | --: | --: | --: |
| **N1 Find available rooms**           |   ● |   ● |     |     |   ● |     |   ● |     |     |
| **N2 Room information/prices**        |     |   ● |     |     |     |     |   ● |     |     |
| **N3 Make reservation**               |   ● |     |   ● |     |   ● |     |     |     |     |
| **N4 Manage reservation**             |     |     |   ● |   ● |   ● |     |     |     |   ● |
| **N5 Secure payment**                 |     |     |   ● |   ● |     |     |     |     |   ● |
| **N6 Avoid double booking**           |   ● |     |   ● |     |   ● |     |   ● |     |     |
| **N7 Staff manage reservations**      |     |     |   ● |     |   ● |   ● |     |     |   ● |
| **N8 Staff manage rooms**             |     |   ● |     |     |     |   ● |   ● |     |     |
| **N9 Reliability**                    |     |     |   ● |     |   ● |     |     |   ● |     |
| **N10 Data security**                 |     |     |     |   ● |     |     |     |     |   ● |

# 5. Draw use-case and sequence diagrams for the application.

### Use Case Diagram

![Use Case Diagram](docs/uml/use-case-diagram.png)

### Sequence Diagram

![Sequence Diagram](docs/uml/sequence-diagram.png)
