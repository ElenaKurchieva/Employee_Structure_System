# Employee Structure System

## Vision

Build a simple, modern web application for managing an organization's structure. The system should make departments, positions, and employees easy to create, understand, and maintain, while also giving management clear reporting on staffing and salary distribution.

The product is intentionally smaller than a full HR platform. It should prioritize clarity, speed, and maintainability over feature sprawl.

## Product Goals

- Support full CRUD workflows for departments, positions, and employees.
- Model the company structure in a way that is easy to browse and understand.
- Keep business rules separate from UI concerns.
- Provide clear reporting for organizational and salary analysis.
- Make the persistence layer swappable so MS SQL can later be replaced by PostgreSQL or another relational database with minimal disruption.
- Maintain a clean foundation that can later support authentication, authorization, and broader HR workflows.

## Current Scope

The application should allow users to:

- Create, edit, and delete departments.
- Create, edit, and delete positions.
- Create, edit, and delete employees.
- Assign employees to departments.
- View employees by department.
- Filter employees by position.
- Sort employees by name or department.
- Generate a report of all employees and departments.
- Generate salary reports showing total and average salary cost by department, plus overall salary totals.

## Recommended Solution Shape

Use ASP.NET Core for the web application and MS SQL as the initial production database target. Design the system so database-specific details are isolated behind infrastructure boundaries.

Preferred architecture:

- Presentation layer: ASP.NET Core MVC or Razor Pages for the web UI.
- Application layer: use cases, orchestration, validation, reporting queries, and DTOs.
- Domain layer: core entities, domain rules, and business invariants.
- Infrastructure layer: Entity Framework Core, repository implementations if needed, database provider configuration, migrations, and reporting persistence queries.

Keep dependencies flowing inward:

- Presentation depends on Application.
- Application depends on Domain abstractions.
- Infrastructure implements persistence and external concerns.
- Domain should not depend on UI or database packages.

## Database Strategy

MS SQL is the starting database, but the application must remain portable.

Guidance:

- Use Entity Framework Core with provider-specific configuration isolated to startup and infrastructure registration.
- Avoid raw SQL unless there is a clear measured need.
- If raw SQL is required for reporting, isolate it behind interfaces and keep it provider-aware.
- Avoid database-specific column types or SQL features unless there is no practical alternative.
- Keep schema and query design compatible with both SQL Server and PostgreSQL where possible.
- Store connection strings and provider selection in configuration.
- Prepare the codebase so switching providers is mainly an infrastructure and migration concern, not an application rewrite.

## Core Domain Concepts

Minimum entities:

- Department
  - Id
  - Name
  - Description optional
- Position
  - Id
  - Title
  - Description optional
- Employee
  - Id
  - FirstName
  - LastName
  - Email optional unless required later
  - Salary
  - DepartmentId
  - PositionId

Expected relationships:

- A department has many employees.
- A position can be assigned to many employees.
- An employee belongs to one department.
- An employee has one position.

## Business Rules

- Department names should be unique within the organization.
- Position titles should be unique or clearly constrained if duplicates are allowed by business rules.
- Employee salary must be non-negative.
- Required fields must be validated in both UI and server-side application logic.
- Delete behavior must be explicit.
- Prefer preventing destructive deletes when related employees exist, unless the UI clearly handles reassignment or confirmation.

## Reporting Requirements

The first release should support:

- Department directory reporting.
- Employee listing with department and position context.
- Salary totals by department.
- Average salary by department.
- Overall salary total across the organization.

Reports should be easy to render on screen and easy to export later if export support is added.

## UI Expectations

- The interface should be modern, clear, and fast to scan.
- Organizational structure should be visually understandable without deep navigation.
- CRUD screens should favor straightforward forms and readable tables.
- Filtering and sorting should be obvious and responsive.
- Validation messages should be specific and actionable.
- Empty states should explain what data is missing and what the user can do next.

## State And Validation

- Keep view state and business state separate.
- Validate input at the boundary of the application layer, not only in controllers or pages.
- Reuse validation rules across create and update workflows where appropriate.
- Handle edge cases explicitly, including duplicate names, missing related records, and invalid salary values.

## Implementation Principles For Future Agents

- Preserve simplicity. Do not introduce enterprise patterns unless they solve a real problem in this codebase.
- Favor clear application services and query handlers over over-engineered abstractions.
- Keep domain rules testable without web or database dependencies.
- Add new features in the correct layer instead of putting business logic in controllers, page models, or EF entities.
- Keep database portability in mind whenever schema, queries, or migrations change.
- If a feature requires provider-specific behavior, isolate it and document why.
- Do not add authentication scaffolding unless requested.

## Suggested Technical Milestones

1. Scaffold the ASP.NET Core solution with layered projects.
2. Model Department, Position, and Employee entities plus validation rules.
3. Implement CRUD workflows for all core entities.
4. Add employee filtering, sorting, and department-based views.
5. Implement reporting queries for organizational and salary summaries.
6. Add focused automated tests for business rules and reporting logic.
7. Keep infrastructure ready for alternate providers such as PostgreSQL.

## Definition Of Done

A feature is complete when:

- Business rules are implemented in the appropriate layer.
- UI flows are functional and understandable.
- Validation failures are handled cleanly.
- Persistence works through the configured provider without leaking provider-specific assumptions into higher layers.
- The change fits the project vision of a simple, maintainable internal management tool.