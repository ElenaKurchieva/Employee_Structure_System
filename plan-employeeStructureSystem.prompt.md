## Plan: MVP Employee Management Foundation

Build a layered ASP.NET Core MVC solution that establishes the long-term architecture now, while implementing only the startup feature set: department CRUD, position CRUD, and a dashboard with total departments and total employees. Use EF Core with SQL Server for persistence, keep provider-specific concerns isolated in Infrastructure, and create the Employee model and table now so the dashboard and future expansion do not require schema rework.

**Steps**
1. Phase 1 - Solution scaffolding. Create a solution with five projects: Web, Application, Domain, Infrastructure, and Tests. Keep references one-way: Web -> Application, Application -> Domain, Infrastructure -> Application plus Domain, and Tests -> the layer under test. Add central configuration for connection strings, environment settings, and dependency injection. This step blocks all later work.
2. Phase 1 - Baseline infrastructure. Configure ASP.NET Core MVC, shared layout, navigation, validation summary handling, and a simple dashboard shell. Add EF Core packages, SQL Server provider, design-time factory or equivalent migration setup, and local development database configuration. This depends on step 1.
3. Phase 2 - Domain model. Define Department, Position, and Employee entities in Domain, even though Employee CRUD is deferred. Capture core invariants that already matter: Department name required and unique at the application or persistence boundary, Position title required and unique at the application or persistence boundary, Employee salary non-negative, and explicit delete behavior for related employees. This depends on step 1 and can run in parallel with part of step 2 once the projects exist.
4. Phase 2 - Application contracts and use cases. Add DTOs, commands or service methods, validation, and query contracts for the startup flows: list/create/update/delete departments, list/create/update/delete positions, and get dashboard counts. Keep business validation here rather than in controllers. This depends on step 3.
5. Phase 3 - Persistence implementation. Create the DbContext, entity configurations, unique indexes for department names and position titles, foreign keys for Employee to Department and Position, and explicit delete restrictions to prevent accidental cascades. Generate the initial migration and database update path. This depends on steps 2 and 3.
6. Phase 3 - Repository or query implementation. Implement the application contracts in Infrastructure using EF Core. Keep queries simple and provider-neutral. For the dashboard, return live counts for departments and employees from the database. This depends on steps 4 and 5.
7. Phase 4 - Department MVC flow. Build the Department controller, view models, and views for index, create, edit, and delete. Enforce validation messages for required name and duplicate-name failures. Prevent deletion when related employees exist and show a clear explanatory message instead of failing silently. This depends on steps 4 through 6.
8. Phase 4 - Position MVC flow. Build the Position controller, view models, and views for index, create, edit, and delete. Mirror the same validation and duplicate-handling patterns used for departments. This depends on steps 4 through 6 and can run in parallel with step 7 after the shared infrastructure is ready.
9. Phase 4 - Dashboard implementation. Build the home or dashboard controller action and view that surface total departments and total employees, plus direct navigation to department and position management. Because employee CRUD is out of scope for the MVP, the employee count will reflect seeded or manually inserted employee records and may remain zero initially. This depends on step 6 and can run in parallel with steps 7 and 8.
10. Phase 5 - UX and operational polish. Add empty states, success and error feedback, confirmation on delete, and a minimal consistent visual style suitable for an internal admin tool. Keep the UI intentionally simple and maintainable. This depends on steps 7 through 9.
11. Phase 5 - Test coverage. Add focused tests for domain and application validation, uniqueness handling, delete restrictions, and dashboard count queries. Prefer unit tests for validation rules and integration tests for EF Core-backed persistence behavior that matters to the MVP. This depends on steps 4 through 9.
12. Phase 5 - Documentation and handoff. Document local setup, connection string configuration, migrations workflow, and known scope boundaries so the next phase can add employee CRUD cleanly without re-architecting the solution. This depends on the preceding steps.

**Relevant files**
- c:\Users\elena.kyurchieva\Desktop\Work\AI\Employee_Structure_System\AGENTS.md — source of product goals, architecture rules, portability constraints, and future scope.
- Solution root under c:\Users\elena.kyurchieva\Desktop\Work\AI\Employee_Structure_System — create the new Web, Application, Domain, Infrastructure, and Tests projects here.
- Web project entry surfaces — Program startup, dependency injection registration, MVC controllers, view models, and Razor views for dashboard, departments, and positions.
- Domain project surfaces — Department, Position, and Employee entities plus any domain-specific guards that are independent of EF or MVC.
- Application project surfaces — service interfaces, DTOs, validation logic, and dashboard query contracts.
- Infrastructure project surfaces — DbContext, EF Core entity configurations, migrations, and application contract implementations.
- Tests project surfaces — validation tests, persistence behavior tests, and dashboard query tests.

**Verification**
1. Restore and build the full solution successfully.
2. Run the initial EF Core migration against the configured SQL Server database and verify the Department, Position, and Employee tables are created with the expected keys and uniqueness constraints.
3. Start the web application and verify the dashboard loads and displays the department count and employee count without authentication.
4. Manually verify department CRUD: create, edit, duplicate-name rejection, and delete behavior.
5. Manually verify position CRUD: create, edit, duplicate-title rejection, and delete behavior.
6. Validate delete restriction behavior when a department or position is referenced by employee data.
7. Run automated tests covering validation and persistence rules.

**Decisions**
- Use a layered solution instead of a single project to match the long-term architecture already defined in AGENTS.md.
- Use ASP.NET Core MVC instead of Razor Pages because the dashboard plus multiple related CRUD areas benefit from explicit controllers and clearer routing separation.
- Use EF Core with SQL Server as the initial provider and keep all provider-specific setup inside Infrastructure and application startup.
- Create the Employee entity and table in the first migration even though employee management screens are deferred; this avoids redesigning the schema just to support the dashboard and future phases.
- Exclude authentication, authorization, employee CRUD UI, filtering, sorting, reporting, export, and advanced admin features from this MVP plan.
- Prefer SQL Server LocalDB or SQL Server Express for local development to keep setup simple on Windows while staying compatible with full SQL Server later.

**Further Considerations**
1. Seed strategy: recommend optional development seed data for one or two departments, positions, and employees so the dashboard is not empty during early demos, while keeping seeding disabled or minimal in production.
2. Delete workflow: for the MVP, recommend blocking deletion of referenced departments or positions rather than building reassignment flows early.
3. Future expansion path: when employee CRUD is added later, reuse the existing Application and Infrastructure seams rather than adding business logic directly into MVC controllers or EF entities.
