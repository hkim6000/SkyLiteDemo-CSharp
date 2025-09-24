<h2>SkyLiteDemo Project: A SkyLite Framework Showcase</h2>
Attached MS-SQL Database : SKYLITEDEMO.BAK (full-backup)<br>
<h4>Introduction</h4>
This SkyLiteDemo project serves as a comprehensive and practical showcase of the SkyLite framework's capabilities for building modern, data-driven web applications. It demonstrates how various UI controls and core framework features can be seamlessly integrated to create a cohesive, interactive, and personalized user experience.
The application presents a classic dashboard-style interface, typical of an administrative portal or a logged-in user's home page, and effectively utilizes the framework's server-centric, AJAX-driven architecture.
<h3>Architectural and Feature Highlights</h3>
<h4>1. Core Architectural Patterns & Design Choices</h4>
The project consistently follows several powerful architectural patterns that are central to the SkyLite philosophy:<br>
•	Model-View (MV/EV) Pattern: The project is architected around a clear separation of concerns using "Main Views" (MV) for displaying lists of data and "Edit Views" (EV) for creating or editing individual records. This pattern is facilitated by a custom WebBase class that manages a ViewPart object, which acts as a ViewModel, holding the state and data for the current view.<br>
•	Single-Page Application (SPA)-like Navigation: The application avoids full page reloads for most actions. A main "shell" page (e.g., XysUser.vb) loads the master layout, and the content area is dynamically replaced with partial views (MV or EV) using ApiResponse commands (SetElementContents), creating a seamless user experience.<br>
•	Robust Role-Based Access Control (RBAC): Security is deeply integrated. The WebBase class centralizes permission checking for both page access (ViewAccess()) and specific actions/methods (ViewMethods()). The UI is then dynamically rendered based on these permissions, ensuring users only see and interact with the functions they are authorized to use.<br>
<h4>2. Key Modules and Functionality</h4>
•	Authentication Flow (XysSignin, XysPass, XysVerify): A complete and secure authentication module demonstrating user lookup, password verification, cookie-based session management (AppKey), and a sign-up flow with email-based One-Time Passcodes (OTP).<br>
•	WebBase.vb - The Application's Core: This central class inherits from WebPage and provides shared functionality to all other pages, including session management, permission checking, dynamic menu/button generation, and loading of translation dictionaries. This drastically reduces code duplication and enforces consistency.<br>
•	Data-Driven UI (SQLGridSection): The "Main View" (*MV.vb) pages showcase the power of the SQLGridSection. The entire UI for displaying, paging, and sorting complex data is generated from a single, declarative SQLGridInfo object, which is directly tied to a SQL query.<br>
•	Dynamic Edit Forms: The "Edit View" (*EV.vb) pages demonstrate how to build data entry forms using a combination of Texts, Dropdown, CheckBox, and other controls. They contain the SaveView and DeleteView logic, including the use of parameterized queries for secure database updates.<br>
•	Translation Engine (Translator): The application is fully internationalized. All visible text is abstracted using Translator.Format("key"), with language dictionaries being loaded from the database for each page.<br>
•	File Management: The bulletin and user profile sections demonstrate handling file uploads, associating them with data records, and providing secure download links.<br>
<h4>3. Code-Level Best Practices</h4>
•	Parameterized Queries: All data manipulation functions use SqlParameter lists to prevent SQL injection vulnerabilities, a critical security practice.<br>
•	Centralized Constants: The References structure provides a single, strongly-typed source for all page names, session keys, and element IDs, making the code highly maintainable and reducing the risk of typos.<br>
•	Thin Client: The JavaScript files are kept minimal and are primarily responsible for gathering data from the DOM and initiating an $ApiRequest. All significant logic, validation, and UI orchestration are handled securely on the server.<br>
<h3>Conclusion</h3>
The SkyLiteDemo project is a masterclass in building a secure, scalable, and maintainable web application with the SkyLite framework. Its architecture is perfectly suited for complex business applications like ERPs, CRMs, or internal admin portals where data integrity, role-based security, and rapid development of standardized forms are paramount. The clear separation of concerns through the MV/EV pattern and the power of high-level data-driven controls showcase a framework that is both powerful and elegantly designed for its purpose.
