# Event Attendance System

Welcome to the Event Attendance System! This repository hosts a small C# solution designed with some **Domain-Driven Design (DDD)** principles. It's a simplified system to manage event attendance, not a real-world application.

---

## System Overview

This system simulates a basic event attendance management platform. Its core function is to handle **`Sessions`** (our term for events) and **`Attendees`** who register for them.

Key concepts in this system include:

* **`Session`**: Represents a scheduled event. It has a `MaxCapacity`, and manages its `_registeredAttendees` (those who have secured a spot) and `_waitlistedAttendees` (those waiting for a spot). You'll be adding a `SessionStatus` to reflect its occupancy.
* **`Attendee`**: Represents a person attending or waiting for a session. Attendees have properties like `IsVip` (boolean) and `RegistrationTier` (enum: `Standard`, `Premium`, `Gold`), which contribute to their `PriorityScore`.
* **`PriorityScore`**: An integer score calculated for each `Attendee` to determine their precedence on the waitlist. **For sorting, only the `PriorityScore` matters; higher scores are prioritized.**

---

## How PriorityScore Is Calculated

The `PriorityScore` for an `Attendee` is a crucial factor for waitlist promotion. It's determined by a combination of their `RegistrationTier` and whether they are `IsVip`. This score is calculated internally within the `Attendee` entity.

Here's a breakdown of the calculation logic:

1.  **Base Score from `RegistrationTier`**:
    * `Standard` tier provides a base of `100` points.
    * `Premium` tier provides a base of `200` points.
    * `Gold` tier provides a base of `300` points.

2.  **`IsVip` Modifier**:
    * If an attendee is marked as `IsVip`, their score is intended to be significantly boosted. This should give VIPs a clear advantage.

---

## Project Structure

The solution is organized into projects:

* **`AssistanceRegistry.Domain`**: Contains the core business logic and entities (e.g., `Session`, `Attendee`). This is the **heart of the application**, independent of any external technology. It also defines **Ports** (interfaces) like `IEventRepository`.
* **`AssistanceRegistry.Application`**: Holds application services that orchestrate domain logic to fulfill use cases (e.g., `SessionRegistrationService`, `SessionManagementService`). These services consume Ports defined in the Domain.
* **`AssistanceRegistry.Infrastructure`**: Implements **Adapters** for the `Domain`'s Ports. For this project, you'll find an `InMemoryEventRepository` that acts as a simple database.
* **`AssistanceRegistry`**: The entry point of the application. This project exposes **API endpoints via a controller** to interact with the system's functionalities. It handles dependency injection and wires up the adapters to the application services.

---

## Project Tasks

You will be working on two tasks within this project. Please read them carefully. Feel free to think out loud and ask any questions you have.

### Task 1: Implement Session Status Management (25-30 minutes)

**Context:**
Our system currently manages `Sessions` and `Attendees`, including waitlists. We need to enhance the `Session` entity to actively reflect its current occupancy status. The `Session` entity should have a `Status` property, indicating whether it's `Empty`, `Available`, `Crowded`, or `WaitlistActive`.

**Your Task:**
Implement all necessary logic to ensure the `Session.Status` property is **automatically updated and accurately maintained** whenever the number of registered attendees or waitlisted attendees changes.

### Task 2: Bug Fix: Incorrect Waitlist Prioritization (15-20 minutes)

**Context:**
We've identified a critical bug in how our system manages the `Session` waitlist. The current promotion algorithm is designed to prioritize attendees based solely on their **`PriorityScore`** (highest score first). However, we've observed that high-priority attendees (e.g., VIPs) are sometimes promoted *after* lower-priority attendees, which is incorrect according to our business rules.

**Your Task:**
Investigate and fix the root cause of this incorrect prioritization.

### Task 3: Peer Review a Pull Request

**Context:**
A colleague has submitted a small pull request proposing a new feature: a `Session` should have a way to indicate if it's nearing its maximum capacity. We need a quick peer review to catch any obvious issues before it's merged.

**Your Task:**
You will be provided with the code changes from this pull request. Review the code and identify any **obvious pain points** related to code quality, maintainability, potential bugs, or deviations from best practices. Be prepared to explain *why* something is a pain point and suggest how it could be improved.